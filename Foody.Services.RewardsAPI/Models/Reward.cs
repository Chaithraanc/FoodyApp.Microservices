namespace Foody.Services.RewardsAPI.Models
{
    public class Reward
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public int RewardsActivity { get; set; }
        public DateTime RewardsDate { get; set; } = DateTime.UtcNow;
      
    }
}
