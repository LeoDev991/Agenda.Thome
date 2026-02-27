using Agenda.Thome.Application.Interfaces;
using Agenda.Thome.Application.Services;
using Agenda.Thome.Domain.Interfaces;
using Agenda.Thome.Infrastructure.Auth;
using Agenda.Thome.Infrastructure.Data;
using Agenda.Thome.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Agenda.Thome.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("AgendaThomeDb"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}
