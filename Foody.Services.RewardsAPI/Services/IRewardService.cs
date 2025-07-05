using Foody.Services.RewardsAPI.Message;

namespace Foody.Services.RewardsAPI.Services
{
    public interface IRewardService
    {
    
        Task UpdateRewards(RewardsMessage rewardsMessage);

    }
}
