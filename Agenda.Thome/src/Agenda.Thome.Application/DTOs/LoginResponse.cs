namespace Agenda.Thome.Application.DTOs;

public record LoginResponse(
    string Token,
    string Name,
    string Email,
    Guid BookingToken
);
