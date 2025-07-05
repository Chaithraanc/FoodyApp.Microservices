using Foody.Web.Models;
using Foody.Web.Models.Dto;

namespace Foody.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);

        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto);

        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);

        Task<ResponseDto?> GetOrder(int orderId);

        Task<ResponseDto?> GetAllOrders(string? userId);

        Task<ResponseDto> UpdateOrderStatus(int orderId, string orderStatus);

    }
}
