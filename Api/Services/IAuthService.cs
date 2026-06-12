using Api.DTOs.Auth;

namespace Api.Services;

public interface IAuthService
{
    Task Register(RegisterDto dto);
    Task<string> Login(LoginDto dto);
}
