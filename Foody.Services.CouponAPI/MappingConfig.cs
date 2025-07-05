using AutoMapper;
using Foody.Services.CouponAPI.Models;
using Foody.Services.CouponAPI.Models.DTO;

namespace Foody.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDTO, Coupon>();
                config.CreateMap<Coupon, CouponDTO>();

            });
            return mappingConfig;
        }
    }
}
