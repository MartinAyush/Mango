using AutoMapper;
using Mango.Services.CouponApi.Data;
using Mango.Services.CouponApi.Models;
using Mango.Services.CouponApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Mango.Services.CouponApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CouponApiController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public CouponApiController(AppDbContext dbContext, IMapper mapper)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
        }

        [HttpGet]
        [Route("GetAll")]
        public ResponseDto GetAll()
        {
            try
            {
                var coupons = _dbContext.Coupons.ToList();
                return new()
                {
                    Result = _mapper.Map<IEnumerable<CouponDto>>(coupons)
                };
            }   
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString()
                };
            }
        }

        [HttpGet]
        [Route("GetById/{id:int}")]
        public ResponseDto GetById(int id)
        {
            try
            {
                var coupon = _dbContext.Coupons.First(c => c.CouponId == id);
                return new()
                {
                    Result = _mapper.Map<CouponDto>(coupon)
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString()
                };
            }
        }

        [HttpGet]
        [Route("GetByCode/{couponCode}")]
        public ResponseDto GetByCode(string couponCode)
        {
            try
            {
                var coupon = _dbContext.Coupons.First(c => c.CouponCode == couponCode);
                return new()
                {
                    Result = _mapper.Map<CouponDto>(coupon)
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString()
                };
            }
        }

        [HttpPost]
        [Route("CreateCoupon")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto CreateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                var response = _dbContext.Coupons.Add(coupon);
                var result = _dbContext.SaveChanges() > 0;

                // Create Coupon in Stripe
                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = 30,
                    Name = couponDto.CouponCode,
                    Currency = "inr",
                    Id = couponDto.CouponCode
                };
                var service = new Stripe.CouponService();
                service.Create(options);
                // End

                return new()
                {
                    Result = _mapper.Map<CouponDto>(coupon)
                };
            }   
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString()
                };
            }
        }

        [HttpPut]
        [Route("UpdateCoupon")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto UpdateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon couponModel = _mapper.Map<Coupon>(couponDto);
                _dbContext.Coupons.Update(couponModel);
                var result = _dbContext.SaveChanges() > 0;
                return new()
                {
                    Result = _mapper.Map<CouponDto>(couponModel)
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString()
                };
            }
        }

        [HttpDelete]
        [Route("DeleteCoupon")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto DeleteCoupon([FromQuery] int id)
        {
            try
            {
                var coupon = _dbContext.Coupons.First(c => c.CouponId == id);
                _dbContext.Remove(coupon);
                var result = _dbContext.SaveChanges() > 0;

                // Delete Coupon from Stripe
                var service = new Stripe.CouponService();
                service.Delete(coupon.CouponCode);
                // End

                return new()
                {
                    Result = _mapper.Map<CouponDto>(coupon)
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString()
                };
            }
        }
    }
}
