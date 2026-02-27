using Agenda.Thome.Domain.Entities;
using Agenda.Thome.Domain.Interfaces;
using Agenda.Thome.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Thome.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByBookingTokenAsync(Guid bookingToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.BookingToken == bookingToken);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
