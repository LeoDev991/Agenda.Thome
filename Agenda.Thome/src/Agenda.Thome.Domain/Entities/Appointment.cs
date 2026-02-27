namespace Agenda.Thome.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string PatientName { get; private set; } = string.Empty;
    public string PatientEmail { get; private set; } = string.Empty;
    public string PatientPhone { get; private set; } = string.Empty;
    public DateTime ScheduledAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; } = null!;

    protected Appointment() { }

    public Appointment(Guid userId, string patientName, string patientEmail, string patientPhone, DateTime scheduledAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        PatientName = patientName;
        PatientEmail = patientEmail;
        PatientPhone = patientPhone;
        ScheduledAt = scheduledAt;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string patientName, string patientEmail, string patientPhone, DateTime scheduledAt)
    {
        PatientName = patientName;
        PatientEmail = patientEmail;
        PatientPhone = patientPhone;
        ScheduledAt = scheduledAt;
    }
}
