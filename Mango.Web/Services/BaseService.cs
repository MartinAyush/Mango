using Mango.Web.Models;
using Mango.Web.Services.Interface;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        public readonly IHttpClientFactory _httpClientFactory;
		private readonly ITokenProvider _tokenProvider;

		public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
			this._tokenProvider = tokenProvider;
		}
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("MangoApi");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "Application/json");

                var jwtToken = _tokenProvider.GetToken();
                message.Headers.Add("Authorization", $"Bearer {jwtToken}");

                message.RequestUri = new Uri(requestDto.Url);
                if(requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage responseMessage = null;

                switch (requestDto.ApiType)
                {
                    case Utitlity.StaticDetails.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case Utitlity.StaticDetails.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case Utitlity.StaticDetails.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                responseMessage = await httpClient.SendAsync(message);
                switch (responseMessage.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                        break;
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                        break;
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                        break;
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                        break;
                    default:
                        var apiContent = await responseMessage.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                        break;
                }
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString()
                };
            }
        }
    }
}
