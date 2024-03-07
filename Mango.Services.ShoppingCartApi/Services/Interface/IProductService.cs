using Mango.Services.ShoppingCartApi.Models.Dto;

namespace Mango.Services.ShoppingCartApi.Services.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProducts();
    }
}
