using Agenda.Thome.Application.DTOs;
using Agenda.Thome.Application.Interfaces;
using Agenda.Thome.Domain.Entities;
using Agenda.Thome.Domain.Interfaces;

namespace Agenda.Thome.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        var token = _tokenService.GenerateToken(user.Id, user.Email);

        return new LoginResponse(token, user.Name, user.Email, user.BookingToken);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser is not null)
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(request.Name, request.Email, passwordHash);

        await _userRepository.AddAsync(user);

        var token = _tokenService.GenerateToken(user.Id, user.Email);

        return new LoginResponse(token, user.Name, user.Email, user.BookingToken);
    }
}
