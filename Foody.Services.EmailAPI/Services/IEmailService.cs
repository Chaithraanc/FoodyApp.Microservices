using Foody.Services.EmailAPI.Message;
using Foody.Services.EmailAPI.Models.Dto;

namespace Foody.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
        Task RegisterEmailAndLog(string email);

        Task LogOrderPlaced(RewardsMessage rewardsDto);

    }
}
