﻿namespace Mango.Services.CouponApi.Models.DTO
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public double DiscountAmmount { get; set; }
        public int MinAmmount { get; set; }
    }
}
