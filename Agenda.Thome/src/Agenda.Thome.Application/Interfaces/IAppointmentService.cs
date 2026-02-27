using Agenda.Thome.Application.DTOs;

namespace Agenda.Thome.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentResponse> CreateAsync(Guid userId, AppointmentRequest request);
    Task<AppointmentResponse> UpdateAsync(Guid userId, Guid appointmentId, AppointmentRequest request);
    Task DeleteAsync(Guid userId, Guid appointmentId);
    Task<IEnumerable<AppointmentResponse>> GetAllByUserAsync(Guid userId);
    Task<AppointmentResponse?> GetByIdAsync(Guid userId, Guid appointmentId);
}
