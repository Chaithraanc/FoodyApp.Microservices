using Foody.Services.ShoppingCartAPI.Models.Dto;

namespace Foody.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService 
    {
        Task<IEnumerable<ProductDto>> GetProducts();

    }
}
