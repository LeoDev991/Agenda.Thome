namespace Agenda.Thome.Application.DTOs;

public record AvailableSlotResponse(
    DateTime DateTime,
    bool IsAvailable
);
