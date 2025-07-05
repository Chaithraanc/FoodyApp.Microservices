using Foody.Web.Models;

namespace Foody.Web.Service.IService
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetCouponAsync(string couponCode);
        Task<ResponseDto?> GetAllCouponAsync();

        Task<ResponseDto?> GetCouponByIdAsync(int couponId);
        Task<ResponseDto?> CreateCouponAsync(CouponDTO couponDto);
        Task<ResponseDto?> UpdateCouponAsync(CouponDTO CouponDto);

        Task<ResponseDto?> DeleteCouponAsync(int couponId);
    }
}
