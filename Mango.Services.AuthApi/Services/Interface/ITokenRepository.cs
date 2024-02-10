using Mango.Services.AuthApi.Models;

namespace Mango.Services.AuthApi.Services.Interface
{
    public interface ITokenRepository
    {
        string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    }
}
