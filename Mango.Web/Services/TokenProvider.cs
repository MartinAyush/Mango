using Mango.Web.Services.Interface;
using Mango.Web.Utitlity;

namespace Mango.Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        public void ClearToken()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(StaticDetails.JwtTokenCookie);
        }

        public string? GetToken()
        {
            string? token = null;

            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(StaticDetails.JwtTokenCookie, out token);
            
            return token;
        }

        public void SetToken(string token)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(StaticDetails.JwtTokenCookie, token);
        }
    }
}
