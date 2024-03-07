using Mango.Services.RewardApi.Message;

namespace Mango.Services.RewardApi.Services
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardMessage rewardMessage);
    }
}
