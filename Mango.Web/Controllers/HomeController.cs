using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICartService cartService)
        {
            this._productService = productService;
            this._cartService = cartService;
        }

        public async Task<IActionResult> Index()
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

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDto? products = null;
            var result = await _productService.GetById(productId);
            if (result != null && result.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<ProductDto>(result.Result.ToString());
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return View(products);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            var userId = HttpContext.User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault().Value;
            var cartDto = new CartDto()
            {
                CartHeader = new()
                {
                    UserId = userId,
                }
            };

            CartDetailsDto cartDetails = new()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };

            cartDto.CartDetails = new List<CartDetailsDto>() { cartDetails };
            var response = await _cartService.UpsertCartAsync(cartDto);

            if (response.IsSuccess)
            {
                TempData["success"] = "Item has been added successfully to the Shopping cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return View(productDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
