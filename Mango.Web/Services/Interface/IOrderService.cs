using Mango.Web.Models;

namespace Mango.Web.Services.Interface
{
    public interface IOrderService
    {
        Task<ResponseDto> CreateOrder(CartDto cartDto);
        Task<ResponseDto> CreateStripeSession(StripeRequestDto requestDto);
        Task<ResponseDto> ValidateStripeSession(int orderHeaderId);
        Task<ResponseDto> GetOrders(string userId);
        Task<ResponseDto> GetOrder(int orderId);
        Task<ResponseDto> UpdateOrderStatus(int orderId, string newStatus);
        
    }
}
