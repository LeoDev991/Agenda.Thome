using Agenda.Thome.Application.DTOs;
using Agenda.Thome.Application.Interfaces;
using Agenda.Thome.Domain.Entities;
using Agenda.Thome.Domain.Interfaces;

namespace Agenda.Thome.Application.Services;

public class BookingService : IBookingService
{
    private readonly IUserRepository _userRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    private static readonly int StartHour = 8;
    private static readonly int EndHour = 18;

    public BookingService(IUserRepository userRepository, IAppointmentRepository appointmentRepository)
    {
        _userRepository = userRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<IEnumerable<AvailableSlotResponse>> GetAvailableSlotsAsync(Guid bookingToken, DateTime date)
    {
        var user = await _userRepository.GetByBookingTokenAsync(bookingToken)
            ?? throw new KeyNotFoundException("Link de agendamento inválido.");

        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new InvalidOperationException("Não há horários disponíveis nos finais de semana.");

        var existingAppointments = await _appointmentRepository.GetByUserIdAndDateAsync(user.Id, date.Date);
        var bookedHours = existingAppointments.Select(a => a.ScheduledAt.Hour).ToHashSet();

        var slots = new List<AvailableSlotResponse>();

        for (var hour = StartHour; hour < EndHour; hour++)
        {
            var slotDateTime = date.Date.AddHours(hour);
            var isAvailable = !bookedHours.Contains(hour);

            slots.Add(new AvailableSlotResponse(slotDateTime, isAvailable));
        }

        return slots;
    }

    public async Task<AppointmentResponse> BookAsync(Guid bookingToken, BookingRequest request)
    {
        var user = await _userRepository.GetByBookingTokenAsync(bookingToken)
            ?? throw new KeyNotFoundException("Link de agendamento inválido.");

        ValidateBookingTime(request.ScheduledAt);

        var existingAppointments = await _appointmentRepository.GetByUserIdAndDateAsync(user.Id, request.ScheduledAt.Date);
        var isSlotTaken = existingAppointments.Any(a => a.ScheduledAt == request.ScheduledAt);

        if (isSlotTaken)
            throw new InvalidOperationException("Este horário já está ocupado. Por favor, escolha outro.");

        var appointment = new Appointment(
            user.Id,
            request.PatientName,
            request.PatientEmail,
            request.PatientPhone,
            request.ScheduledAt
        );

        await _appointmentRepository.AddAsync(appointment);

        return new AppointmentResponse(
            appointment.Id,
            appointment.PatientName,
            appointment.PatientEmail,
            appointment.PatientPhone,
            appointment.ScheduledAt,
            appointment.CreatedAt
        );
    }

    private static void ValidateBookingTime(DateTime scheduledAt)
    {
        if (scheduledAt.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new InvalidOperationException("Não é possível agendar nos finais de semana.");

        if (scheduledAt.Hour < StartHour || scheduledAt.Hour >= EndHour)
            throw new InvalidOperationException("O horário deve estar dentro do horário comercial (08:00 às 18:00).");

        if (scheduledAt.Minute != 0 || scheduledAt.Second != 0)
            throw new InvalidOperationException("Os agendamentos devem ser em horas cheias.");

        if (scheduledAt <= DateTime.UtcNow)
            throw new InvalidOperationException("Não é possível agendar no passado.");
    }
}
