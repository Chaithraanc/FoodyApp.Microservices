using Foody.Services.RewardsAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace Foody.Services.RewardsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        public DbSet<Reward> Rewards { get; set; }


    }
}
