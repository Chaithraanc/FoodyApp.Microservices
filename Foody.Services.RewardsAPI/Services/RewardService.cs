
using Foody.Services.RewardsAPI.Data;
using Foody.Services.RewardsAPI.Message;
using Foody.Services.RewardsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Foody.Services.RewardsAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _dboptions;

        public RewardService(DbContextOptions<AppDbContext> dboptions)
        {
            this._dboptions = dboptions;
        }

       
        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Reward reward = new Reward
                {
                    UserId = rewardsMessage.UserId,
                    OrderId = rewardsMessage.OrderId,
                    RewardsActivity = rewardsMessage.RewardsActivity,
                    RewardsDate = DateTime.Now
                };

                await using (var context = new AppDbContext(_dboptions))
                {
                    await context.Rewards.AddAsync(reward);
                    await context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw;
            }
        }

      

        
    }
}
