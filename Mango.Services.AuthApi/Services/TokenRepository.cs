using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Services.AuthApi.Services
{
    public class TokenRepository : ITokenRepository
    {
        private readonly JwtOptions _jwtOptions;

        public TokenRepository(IOptions<JwtOptions> jwtOptions)
        {
            this._jwtOptions = jwtOptions.Value;
        }
        public string GenerateToken(ApplicationUser user, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtKey = Encoding.ASCII.GetBytes(_jwtOptions.Key);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = _jwtOptions.Audience,
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(jwtToken);
        }
    }
}
