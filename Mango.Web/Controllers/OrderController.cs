using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Mango.Web.Utitlity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            this._orderService = orderService;
        }

        [Authorize]
        public IActionResult OrderIndex()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDto orderHeaderDto = new();
            string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault().Value;

            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(response.Result.ToString());
            }

            if(!User.IsInRole(StaticDetails.RoleAdmin) && userId != orderHeaderDto.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDto);
        }

        public IActionResult GetAll(string status)
        {
            string userId = "";
            List<OrderHeaderDto> result;
            if (!User.IsInRole(StaticDetails.RoleAdmin))
            {
                userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault().Value;
            }

            ResponseDto response = _orderService.GetOrders(userId).GetAwaiter().GetResult();
            if(response != null && response.IsSuccess)
            {
                 result = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(response.Result.ToString());

                switch (status)
                {
                    case "approved":
                        result = result.Where(x => x.Status == StaticDetails.Status_Approved).ToList();
                        break;
                    case "readyForPickup":
                        result = result.Where(x => x.Status == StaticDetails.Status_ReadyForPickup).ToList();
                        break;
                    case "cancelled":
                        result = result.Where(x => x.Status == StaticDetails.Status_Cancelled).ToList();
                        break;
                    default:
                        break;

                }
            }
            else
            {
                result = new();
            }
            return Json(new { data = result });
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_ReadyForPickup);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status Update Successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_Completed);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status Update Successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_Cancelled);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status Update Successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }
    }
}
