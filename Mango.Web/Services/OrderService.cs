using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Mango.Web.Utitlity;

namespace Mango.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;

        public OrderService(IBaseService baseService)
        {
            this._baseService = baseService;
        }

        public async Task<ResponseDto> CreateOrder(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.OrderApiBaseUrl + "/api/order/CreateOrder",
                Data = cartDto,
            });
        }

        public async Task<ResponseDto> CreateStripeSession(StripeRequestDto requestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.OrderApiBaseUrl + "/api/order/CreateStripeSession",
                Data = requestDto,
            });
        }

        public async Task<ResponseDto> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.OrderApiBaseUrl + "/api/order/GetOrder/" + orderId,
            });
        }

        public async Task<ResponseDto> GetOrders(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.OrderApiBaseUrl + "/api/order/GetOrders/" + userId,
            });
        }

        public async Task<ResponseDto> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.OrderApiBaseUrl + "/api/order/UpdateOrderStatus/" + orderId,
                Data = newStatus,
            });
        }

        public async Task<ResponseDto> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.OrderApiBaseUrl + "/api/order/ValidateStripeSession",
                Data = orderHeaderId,
            });
        }
    }
}
