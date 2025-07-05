using Foody.Services.ShoppingCartAPI.Models.Dto;

namespace Foody.Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    { 
        Task<CouponDTO> GetCoupon(string couponCode);
    }
}
