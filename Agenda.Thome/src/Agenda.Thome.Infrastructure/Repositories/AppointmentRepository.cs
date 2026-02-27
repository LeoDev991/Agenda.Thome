using Agenda.Thome.Domain.Entities;
using Agenda.Thome.Domain.Interfaces;
using Agenda.Thome.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Thome.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _context.Appointments.FindAsync(id);
    }

    public async Task<IEnumerable<Appointment>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Appointments
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByUserIdAndDateAsync(Guid userId, DateTime date)
    {
        return await _context.Appointments
            .Where(a => a.UserId == userId && a.ScheduledAt.Date == date.Date)
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync();
    }

    public async Task AddAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment is not null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }
}
