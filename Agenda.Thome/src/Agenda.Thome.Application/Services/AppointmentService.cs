using Agenda.Thome.Application.DTOs;
using Agenda.Thome.Application.Interfaces;
using Agenda.Thome.Domain.Entities;
using Agenda.Thome.Domain.Interfaces;

namespace Agenda.Thome.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<AppointmentResponse> CreateAsync(Guid userId, AppointmentRequest request)
    {
        ValidateBusinessHours(request.ScheduledAt);
        await ValidateSlotAvailability(userId, request.ScheduledAt);

        var appointment = new Appointment(
            userId,
            request.PatientName,
            request.PatientEmail,
            request.PatientPhone,
            request.ScheduledAt
        );

        await _appointmentRepository.AddAsync(appointment);

        return MapToResponse(appointment);
    }

    public async Task<AppointmentResponse> UpdateAsync(Guid userId, Guid appointmentId, AppointmentRequest request)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId)
            ?? throw new KeyNotFoundException("Consulta não encontrada.");

        if (appointment.UserId != userId)
            throw new UnauthorizedAccessException("Você não tem permissão para editar esta consulta.");

        ValidateBusinessHours(request.ScheduledAt);

        if (appointment.ScheduledAt != request.ScheduledAt)
            await ValidateSlotAvailability(userId, request.ScheduledAt);

        appointment.Update(request.PatientName, request.PatientEmail, request.PatientPhone, request.ScheduledAt);
        await _appointmentRepository.UpdateAsync(appointment);

        return MapToResponse(appointment);
    }

    public async Task DeleteAsync(Guid userId, Guid appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId)
            ?? throw new KeyNotFoundException("Consulta não encontrada.");

        if (appointment.UserId != userId)
            throw new UnauthorizedAccessException("Você não tem permissão para excluir esta consulta.");

        await _appointmentRepository.DeleteAsync(appointmentId);
    }

    public async Task<IEnumerable<AppointmentResponse>> GetAllByUserAsync(Guid userId)
    {
        var appointments = await _appointmentRepository.GetByUserIdAsync(userId);
        return appointments.Select(MapToResponse);
    }

    public async Task<AppointmentResponse?> GetByIdAsync(Guid userId, Guid appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

        if (appointment is null || appointment.UserId != userId)
            return null;

        return MapToResponse(appointment);
    }

    private static void ValidateBusinessHours(DateTime scheduledAt)
    {
        var hour = scheduledAt.Hour;

        if (scheduledAt.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new InvalidOperationException("Não é possível agendar nos finais de semana.");

        if (hour < 8 || hour >= 18)
            throw new InvalidOperationException("O horário deve estar dentro do horário comercial (08:00 às 18:00).");

        if (scheduledAt.Minute != 0 || scheduledAt.Second != 0)
            throw new InvalidOperationException("Os agendamentos devem ser em horas cheias.");
    }

    private async Task ValidateSlotAvailability(Guid userId, DateTime scheduledAt)
    {
        var existingAppointments = await _appointmentRepository.GetByUserIdAndDateAsync(userId, scheduledAt.Date);
        var isSlotTaken = existingAppointments.Any(a => a.ScheduledAt == scheduledAt);

        if (isSlotTaken)
            throw new InvalidOperationException("Este horário já está ocupado.");
    }

    private static AppointmentResponse MapToResponse(Appointment appointment)
    {
        return new AppointmentResponse(
            appointment.Id,
            appointment.PatientName,
            appointment.PatientEmail,
            appointment.PatientPhone,
            appointment.ScheduledAt,
            appointment.CreatedAt
        );
    }
}
