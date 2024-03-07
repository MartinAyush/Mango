using Mango.Services.ShoppingCartApi.Models.Dto;
using Mango.Services.ShoppingCartApi.Models.DTO;

namespace Mango.Services.ShoppingCartApi.Services.Interface
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponByName(string couponCode);
    }
}
