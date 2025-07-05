using Foody.Services.ShoppingCartAPI.Models.Dto;
using Foody.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Foody.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
    public CouponService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<CouponDTO> GetCoupon(string couponCode)
    {
        var client = _httpClientFactory.CreateClient("Coupon");
        var response = await client.GetAsync($"/api/Coupon/GetByCode/{couponCode}");
        var responseString = await response.Content.ReadAsStringAsync();
        var res = JsonConvert.DeserializeObject<ResponseDto>(responseString);
        if (res != null && res.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(res.Result));
        }
        return new CouponDTO();
    }
}

    
}
