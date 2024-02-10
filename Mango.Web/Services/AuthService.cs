using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Mango.Web.Utitlity;

namespace Mango.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            this._baseService = baseService;
        }
        public async Task<ResponseDto> AssignRoleAsync(RegistrationRequestDto requestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.AuthApiBaseUrl + "/api/AuthApi/AssignRole",
                Data = requestDto,
            });
        }

        public async Task<ResponseDto> LoginAsync(LoginRequestDto requestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.AuthApiBaseUrl + "/api/AuthApi/login",
                Data = requestDto,
            });
        }

        public async Task<ResponseDto> RegisterAsync(RegistrationRequestDto requestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.AuthApiBaseUrl + "/api/AuthApi/register",
                Data = requestDto,
            });
        }
    }
}
