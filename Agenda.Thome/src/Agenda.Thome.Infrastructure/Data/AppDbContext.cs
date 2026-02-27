using Agenda.Thome.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Thome.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(150);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.BookingToken).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();

            entity.HasMany(u => u.Appointments)
                  .WithOne(a => a.User)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.PatientName).IsRequired().HasMaxLength(150);
            entity.Property(a => a.PatientEmail).IsRequired().HasMaxLength(200);
            entity.Property(a => a.PatientPhone).IsRequired().HasMaxLength(20);
            entity.Property(a => a.ScheduledAt).IsRequired();
        });
    }
}
