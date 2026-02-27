using Agenda.Thome.Domain.Entities;

namespace Agenda.Thome.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByBookingTokenAsync(Guid bookingToken);
    Task AddAsync(User user);
}
