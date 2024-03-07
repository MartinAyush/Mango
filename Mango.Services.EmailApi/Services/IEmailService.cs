using Mango.Services.EmailApi.Message;
using Mango.Services.EmailApi.Models.Dto;

namespace Mango.Services.EmailApi.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
        Task RegisterUserAndLog(string email);
        Task LogOrderPlaced(RewardMessage rewardsDto);
    }
}
