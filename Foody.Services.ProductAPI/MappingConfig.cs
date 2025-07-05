using AutoMapper;
using Foody.Services.ProductAPI.Models;
using Foody.Services.ProductAPI.Models.Dto;

namespace Foody.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
               

            });
            return mappingConfig;
        }
    }
}
