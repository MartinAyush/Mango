using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Mango.Web.Utitlity;

namespace Mango.Web.Services
{
	public class ProductService : IProductService
	{
		private readonly IBaseService _baseService;

		public ProductService(IBaseService baseService)
        {
			this._baseService = baseService;
		}
        public async Task<ResponseDto> Create(ProductDto productDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = StaticDetails.ApiType.POST,
				Url = StaticDetails.ProductApiBaseUrl + "/api/ProductApi/Create",
				Data = productDto,
			});
		}

		public async Task<ResponseDto> Delete(int id)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = StaticDetails.ApiType.DELETE,
				Url = StaticDetails.ProductApiBaseUrl + "/api/ProductApi/delete/" + id,
			});
		}

		public async Task<ResponseDto> GetAll()
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = StaticDetails.ApiType.GET,
				Url = StaticDetails.ProductApiBaseUrl + "/api/ProductApi/GetAll"
			});
		}

		public async Task<ResponseDto> GetById(int id)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = StaticDetails.ApiType.GET,
				Url = StaticDetails.ProductApiBaseUrl + "/api/ProductApi/GetById/" + id,
			});
		}

		public async Task<ResponseDto> Update(ProductDto productDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = StaticDetails.ApiType.PUT,
				Url = StaticDetails.ProductApiBaseUrl + "/api/ProductApi/update",
				Data = productDto,
			});
		}
	}
}
