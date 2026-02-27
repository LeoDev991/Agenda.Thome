using System.ComponentModel.DataAnnotations;

namespace Agenda.Thome.Application.DTOs;

public record BookingRequest(
    [Required] string PatientName,
    [Required] [EmailAddress] string PatientEmail,
    [Required] string PatientPhone,
    [Required] DateTime ScheduledAt
);
