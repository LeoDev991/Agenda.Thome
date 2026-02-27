using Agenda.Thome.Domain.Entities;

namespace Agenda.Thome.Domain.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Appointment>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Appointment>> GetByUserIdAndDateAsync(Guid userId, DateTime date);
    Task AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(Guid id);
}
