using Agenda.Thome.Application.DTOs;

namespace Agenda.Thome.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(Guid userId, string email);
}
