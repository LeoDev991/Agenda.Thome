using Agenda.Thome.Application.Interfaces;
using Agenda.Thome.Domain.Entities;

namespace Agenda.Thome.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context, IPasswordHasher passwordHasher)
    {
        if (context.Users.Any())
            return;

        var nextBusinessDay = GetNextBusinessDay(DateTime.UtcNow);

        var passwordHash = passwordHasher.Hash("123456");

        var user = new User("Dr. João Silva", "joao@email.com", passwordHash);

        context.Users.Add(user);
        context.SaveChanges();

        var appointment1 = new Appointment(
            user.Id,
            "Maria Oliveira",
            "maria@email.com",
            "(11) 99999-1111",
            nextBusinessDay.Date.AddHours(9)
        );

        var appointment2 = new Appointment(
            user.Id,
            "Carlos Santos",
            "carlos@email.com",
            "(11) 99999-2222",
            nextBusinessDay.Date.AddHours(14)
        );

        context.Appointments.AddRange(appointment1, appointment2);
        context.SaveChanges();
    }

    private static DateTime GetNextBusinessDay(DateTime from)
    {
        var next = from.AddDays(1);

        while (next.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            next = next.AddDays(1);
        }

        return next;
    }
}
