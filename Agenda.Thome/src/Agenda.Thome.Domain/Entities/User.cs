namespace Agenda.Thome.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public Guid BookingToken { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

    protected User() { }

    public User(string name, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        BookingToken = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateEmail(string email)
    {
        Email = email;
    }

    public void RegenerateBookingToken()
    {
        BookingToken = Guid.NewGuid();
    }
}
