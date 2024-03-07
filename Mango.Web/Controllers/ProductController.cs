using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            this._productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto?> products = null;
            var result = await _productService.GetAll();
            if (result != null && result.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(result.Result.ToString());
                return View(products);
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return View();
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> ProductCreate(ProductDto productDto)
		{
            if (ModelState.IsValid)
            {
                var result = await _productService.Create(productDto);
				if (result != null && result.IsSuccess)
				{
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = result.Message;
                }
            }
			return View(productDto);
		}

        public async Task<IActionResult> ProductDelete(int id)
        {
            var response = await _productService.GetById(id);
            if (response.IsSuccess)
            {
                var result = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(result);
            }
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> ProductDelete(ProductDto productDto)
		{
            var result = await _productService.Delete(productDto.ProductId);
			if (result != null && result.IsSuccess)
			{
                TempData["success"] = "Product delete successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return View();
		}

        public async Task<IActionResult> ProductEdit(int id)
        {
            var response = await _productService.GetById(id);
            if (response.IsSuccess)
            {
                var result = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(result);
            }
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> ProductEdit(ProductDto productDto)
        {
            var result = await _productService.Update(productDto);
            if (result != null && result.IsSuccess)
            {
                TempData["success"] = "Product Update successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return View();
        }
    }
}
