using Foody.Services.ShoppingCartAPI.Models.Dto;
using Foody.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Foody.Services.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/Product");
            var responseString = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResponseDto>(responseString);
            if (res != null && res.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(res.Result));
            }
           
                return new List<ProductDto>();
          
        }
    }
}
