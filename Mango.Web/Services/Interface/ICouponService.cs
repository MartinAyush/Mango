using Mango.Web.Models;

namespace Mango.Web.Services.Interface
{
    public interface ICouponService
    {
        Task<ResponseDto> GetAllCoupons();
        Task<ResponseDto> GetCouponByCouponId(int couponId);
        Task<ResponseDto> GetCouponByName(string couponName);
        Task<ResponseDto> CreateCoupon(CouponDto couponDto);
        Task<ResponseDto> UpdateCoupon(CouponDto couponDto);
        Task<ResponseDto> DeleteCoupon(int id);
    }
}
