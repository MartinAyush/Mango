using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Mango.Web.Utitlity;

namespace Mango.Web.Services
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;

        public CouponService(IBaseService baseService)
        {
            this._baseService = baseService;
        }

        public async Task<ResponseDto> CreateCoupon(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Url = StaticDetails.CouponApiBaseUrl + "/api/CouponApi/CreateCoupon",
                Data = couponDto,
            });
        }

        public async Task<ResponseDto> DeleteCoupon(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = StaticDetails.CouponApiBaseUrl + "/api/CouponApi/DeleteCoupon?id=" + id,
            });
        }

        public async Task<ResponseDto> GetAllCoupons()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CouponApiBaseUrl + "/api/CouponApi/GetAll",
            });
        }

        public async Task<ResponseDto> GetCouponByCouponId(int couponId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CouponApiBaseUrl + "/api/CouponApi/GetById/" + couponId,
            });
        }

        public async Task<ResponseDto> GetCouponByName(string couponName)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CouponApiBaseUrl + "/api/CouponApi/GetByCode/" + couponName,
            });
        }

        public async Task<ResponseDto> UpdateCoupon(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Url = StaticDetails.CouponApiBaseUrl + "/api/CouponApi/UpdateCoupon",
                Data = couponDto,
            });
        }
    }
}
