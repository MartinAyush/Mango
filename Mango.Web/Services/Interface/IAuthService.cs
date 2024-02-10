using Mango.Web.Models;

namespace Mango.Web.Services.Interface
{
    public interface IAuthService
    {
        Task<ResponseDto> LoginAsync(LoginRequestDto requestDto);
        Task<ResponseDto> RegisterAsync(RegistrationRequestDto requestDto);
        Task<ResponseDto> AssignRoleAsync(RegistrationRequestDto requestDto);
    }
}
