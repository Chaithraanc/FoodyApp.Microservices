using Foody.Services.OrderAPI.Models;
using Foody.Services.OrderAPI.Models.Dto;

namespace Foody.Services.OrderAPI.Service.IService
{
    public interface IProductService 
    {
        Task<IEnumerable<ProductDto>> GetProducts();

    }
}
