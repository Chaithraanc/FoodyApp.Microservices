using Foody.Web.Models.Dto;
using Foody.Web.Models;

namespace Foody.Web.Service.IService
{
    public interface IcartService
    {
        Task<ResponseDto?> GetCartByUserIdAsync(string userId);
        Task<ResponseDto?> UpsertCartAsync(CartDto cartDto);
        Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto);

        Task<ResponseDto?> EmailCart(CartDto cartDto);
            
        
    }
}
