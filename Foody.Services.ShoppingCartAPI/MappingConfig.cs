using AutoMapper;
using Foody.Services.ShoppingCartAPI.Models;
using Foody.Services.ShoppingCartAPI.Models.Dto;

namespace Foody.Services.ShoppingCartAPI.MappingConfig
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();


            });
            return mappingConfig;
        }
    }
}
