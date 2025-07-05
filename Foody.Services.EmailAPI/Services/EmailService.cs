using Foody.Services.EmailAPI.Data;
using Foody.Services.EmailAPI.Message;
using Foody.Services.EmailAPI.Models;
using Foody.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Foody.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dboptions;

        public EmailService(DbContextOptions<AppDbContext> dboptions)
        {
            this._dboptions = dboptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/> Cart email requested");
            message.AppendLine("<br/>Total " + cartDto.CartHeader.CartTotal);
            message.AppendLine("<br/>");
            message.AppendLine("<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                message.AppendLine("<li>");
                message.AppendLine(item.Product.Name + " - " + item.Product.Price + " x " + item.Count);
                message.AppendLine("</li>");
            }
            message.AppendLine("<ul/>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        public Task LogOrderPlaced(RewardsMessage rewardsDto)
        {
            string message = "Order placed for UserId: " + rewardsDto.UserId + " with OrderId: " + rewardsDto.OrderId + " at " + DateTime.Now;
            return LogAndEmail(message, rewardsDto.UserId);
        }

        public async Task RegisterEmailAndLog(string email)
        {
            string message = "Email registered: " + email + " at " + DateTime.Now;
            await LogAndEmail(message, email);
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    Message = message,
                    MessageSent = DateTime.Now
                };
                using (var db = new AppDbContext(_dboptions))
                {
                    await db.EmailLoggers.AddAsync(emailLog);
                    await db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return false;
            }

        }
    }
}
