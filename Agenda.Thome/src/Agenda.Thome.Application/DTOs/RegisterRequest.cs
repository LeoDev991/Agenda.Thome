using System.ComponentModel.DataAnnotations;

namespace Agenda.Thome.Application.DTOs;

public record RegisterRequest(
    [Required] string Name,
    [Required] string Email,
    [Required] [MinLength(6)] string Password
);
