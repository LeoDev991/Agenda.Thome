using Agenda.Thome.Application.DTOs;

namespace Agenda.Thome.Application.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<AvailableSlotResponse>> GetAvailableSlotsAsync(Guid bookingToken, DateTime date);
    Task<AppointmentResponse> BookAsync(Guid bookingToken, BookingRequest request);
}
