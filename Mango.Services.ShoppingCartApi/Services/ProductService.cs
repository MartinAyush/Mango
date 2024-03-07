using Mango.Services.ShoppingCartApi.Models.Dto;
using Mango.Services.ShoppingCartApi.Models.DTO;
using Mango.Services.ShoppingCartApi.Services.Interface;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClient;

        public ProductService(IHttpClientFactory httpClient)
        {
            this._httpClient = httpClient;
        }
        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            var client = _httpClient.CreateClient("Product");
            var httpResponse = await client.GetAsync("api/ProductApi/GetAll");
            var apiContent = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            
            if (response.IsSuccess)
            {
                var products = JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(response.Result.ToString());
                return products;
            }

            return new List<ProductDto>();
        }
    }
}
