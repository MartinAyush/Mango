using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Mango.Web.Utitlity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            this._cartService = cartService;
            this._orderService = orderService;
        }

        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutCart(CartDto cartDto)
        {
            var cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;

            var response = await _orderService.CreateOrder(cart);
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(response.Result.ToString());

            if (response != null && response.IsSuccess)
            {
                // Get stripe session and redirect to stripe to place order
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDto requestDto = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.OrderHeaderId,
                    CancelUrl = domain + "cart/Checkout",
                    OrderHeader = orderHeaderDto,
                };

                var stripeResponse = await _orderService.CreateStripeSession(requestDto);
                StripeRequestDto stripeRequestDto = JsonConvert.DeserializeObject<StripeRequestDto>(stripeResponse.Result.ToString());

                Response.Headers.Add("Location", stripeRequestDto.StripSessionUrl);
                return new StatusCodeResult(303); // Redirect
            }
            return RedirectToAction(nameof(Checkout));
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDto response = await _orderService.ValidateStripeSession(orderId);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDto orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(response.Result.ToString());
                if(orderHeader.Status == StaticDetails.Status_Approved)
                {
                    return View(orderId);
                }
            }

            // TODO - handle other status and show the other views respectively
            return View(orderId);
        }

        public async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _cartService.GetCartByUserIdAsync(userId);

            if(response != null && response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }

            return new CartDto();
        }

        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var response = await _cartService.ApplyCouponAsync(cartDto);
            if (response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            var cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

            var response = await _cartService.EmailCart(cart);
            if (response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            var response = await _cartService.ApplyCouponAsync(cartDto);
            if (response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        public async Task<IActionResult> RemoveItem(int cartDetailsId)
        {
            var response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response.IsSuccess)
            {
                TempData["success"] = "Item removed successfully";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();  
        }

    }
}
