using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            this._couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto?> coupons = null;
            var result = await _couponService.GetAllCoupons();
            if (result != null && result.IsSuccess)
            {
                coupons = JsonConvert.DeserializeObject<List<CouponDto>>(result.Result.ToString());
                return View(coupons);
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return View();
        }

		public async Task<IActionResult> CouponCreate(CouponDto couponDto)
		{
            if (ModelState.IsValid)
            {
                var result = await _couponService.CreateCoupon(couponDto);
				if (result != null && result.IsSuccess)
				{
                    TempData["success"] = "Coupon created successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = result.Message;
                }
            }
			return View(couponDto);
		}

		public async Task<IActionResult> CouponDelete(int couponId)
		{
            var result = await _couponService.DeleteCoupon(couponId);
			if (result != null && result.IsSuccess)
			{
                TempData["success"] = "Coupon delete successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = result.Message;
            }
            return View();
		}
	}
}
