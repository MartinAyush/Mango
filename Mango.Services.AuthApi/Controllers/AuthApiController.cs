using Mango.MessageBus;
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
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        protected ResponseDto _responseDto;
        public AuthApiController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            this._authService = authService;
            this._messageBus = messageBus;
            this._configuration = configuration;
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
            await _messageBus.PublishMessage(requestDto.Email, _configuration["TopicAndQueueNames:registerUserQueue"]);
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

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto requestDto)
        {
            var response = await _authService.AssignRole(requestDto.Email, requestDto.RoleName.ToUpper());
            if (!response)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error occured";
                return BadRequest(_responseDto);
            }
            else
            {
                _responseDto.Result = response;
                _responseDto.IsSuccess = true;
                _responseDto.Message = "";
            }

            return Ok(_responseDto);
        }
    }
}
