using Mango.Services.AuthApi.Models.DTO;

namespace Mango.Services.AuthApi.Services.Interface
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto requestDto);
        Task<LoginResponseDto> Login(LoginRequestDto requestDto);
        Task<bool> AssignRole(string email, string role);
    }
}
