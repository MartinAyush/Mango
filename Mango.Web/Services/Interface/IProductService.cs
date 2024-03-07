using Mango.Web.Models;

namespace Mango.Web.Services.Interface
{
	public interface IProductService
	{
		Task<ResponseDto> Create(ProductDto productDto);
		Task<ResponseDto> GetAll();
		Task<ResponseDto> GetById(int id);
		Task<ResponseDto> Update(ProductDto productDto);
		Task<ResponseDto> Delete(int id);
	}
}
