
using Mango.Services.OrderApi.Models.Dto;

namespace Mango.Services.OrderApi.Services.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProducts();
    }
}
