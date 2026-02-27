namespace Agenda.Thome.Application.DTOs;

public record AppointmentResponse(
    Guid Id,
    string PatientName,
    string PatientEmail,
    string PatientPhone,
    DateTime ScheduledAt,
    DateTime CreatedAt
);
