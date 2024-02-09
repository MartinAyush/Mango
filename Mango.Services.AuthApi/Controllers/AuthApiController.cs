using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.AuthApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;
        public AuthApiController(IAuthService authService)
        {
            this._authService = authService;
            _responseDto = new ResponseDto();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto requestDto)
        {
            var response = await _authService.Register(requestDto);
            if (!string.IsNullOrEmpty(response))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = response;
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
        {
            var response = await _authService.Login(requestDto);
            if(response.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Username or password is incorrect";
                return BadRequest(_responseDto);
            }
            else
            {
                _responseDto.Result = response;
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Login success";
            }

            return Ok(_responseDto);
        }
    }
}
