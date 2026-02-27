using System.ComponentModel.DataAnnotations;

namespace Agenda.Thome.Application.DTOs;

public record LoginRequest(
    [Required] string Email,
    [Required] string Password
);
