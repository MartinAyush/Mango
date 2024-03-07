using AutoMapper;
using Azure;
using Mango.Services.ProductApi.Data;
using Mango.Services.ProductApi.Models;
using Mango.Services.ProductApi.Models.Dto;
using Mango.Services.ProductApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductApiController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly AppDbContext _dbContext;

		public ProductApiController(IMapper mapper, AppDbContext dbContext)
        {
			this._mapper = mapper;
			this._dbContext = dbContext;
		}

		// Create
		[HttpPost]
		[Route("Create")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Create(ProductDto requestDto)
		{
			var product = _mapper.Map<Product>(requestDto);
			ResponseDto response = new ResponseDto();
			try
			{
				_dbContext.Products.Add(product);
				response.IsSuccess = _dbContext.SaveChanges() > 0;
				response.Result = _mapper.Map<ProductDto>(product);
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
			}
			return response;
		}

		// Read
		[HttpGet]
		[Route("GetAll")]
		public ResponseDto GetAll()
		{
			var products = _dbContext.Products.ToList();
			ResponseDto response = new ResponseDto();

			if(products != null && products.Any())
			{
				response.IsSuccess = true;
				response.Message = "";
				response.Result = products;
			}
			else
			{
				response.IsSuccess = false;
				response.Message = "Something went wrong!";
				response.Result = null;
			}
			return response;
		}

		[HttpGet]
		[Route("GetById/{id:int}")]
		[Authorize]
		public ResponseDto GetById(int id)
		{
			ResponseDto response = new ResponseDto();
			var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == id);
			if (product != null)
			{
				response.IsSuccess = true;
				response.Message = "";
				response.Result = product;
			}
			else
			{
				response.IsSuccess = false;
				response.Message = "Something went wrong!";
				response.Result = null;
			}
			return response;
		}

		// Update
		[HttpPut]
		[Route("update")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Update(ProductDto productDto)
		{
			ResponseDto response = new ResponseDto();
			try
			{
				var product = _mapper.Map<Product>(productDto); 
				_dbContext.Products.Update(product);
				response.Result = _dbContext.SaveChanges() > 0;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
			}
			return response;
		}

		// Delete
		[HttpDelete]
		[Route("delete/{id:int}")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto Delete(int id)
		{
			ResponseDto response = new ResponseDto();
			try
			{
				var product = _dbContext.Products.FirstOrDefault(x => x.ProductId == id);
				if(product != null)
				{
					_dbContext.Products.Remove(product);
				}
				response.IsSuccess = _dbContext.SaveChanges() > 0;
				response.Result = product;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
			}
			return response;
		}
	}
}
