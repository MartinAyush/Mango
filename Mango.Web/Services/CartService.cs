using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Mango.Web.Utitlity;

namespace Mango.Web.Services
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {
            this._baseService = baseService;
        }
        public async Task<ResponseDto> ApplyCouponAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.ShoppingCartBaseUrl + "/api/CartApi/ApplyCoupon",
                Data = cartDto,
            });
        }

        public async Task<ResponseDto> EmailCart(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.ShoppingCartBaseUrl + "/api/CartApi/EmailCartRequest",
                Data = cartDto,
            });
        }

        public async Task<ResponseDto> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.ShoppingCartBaseUrl + "/api/CartApi/GetCart/" + userId,
            });
        }

        public async Task<ResponseDto> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = StaticDetails.ShoppingCartBaseUrl + "/api/CartApi/RemoveCartItem",
                Data = cartDetailsId,
            });
        }

        public async Task<ResponseDto> UpsertCartAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.ShoppingCartBaseUrl + "/api/CartApi/CartUpsert/",
                Data = cartDto,
            });
        }
    }
}
