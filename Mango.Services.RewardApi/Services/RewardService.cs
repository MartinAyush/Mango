using Azure.Core.Amqp;
using Mango.Services.RewardApi.Message;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Mango.Services.RewardApi.Data;
using Mango.Services.RewardApi.Models;

namespace Mango.Services.RewardApi.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public RewardService(DbContextOptions<AppDbContext> options)
        {
            this._dbOptions = options;
        }

        public async Task UpdateRewards(RewardMessage rewardMessage)
        {
            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardMessage.OrderId,
                    RewardsActivity = rewardMessage.RewardsActivity,
                    UserId = rewardMessage.UserId,
                    RewardsDate = DateTime.Now
                };

                await using var _db = new AppDbContext(_dbOptions);
                await _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
