using Mango.Services.ShoppingCartApi.Models.DTO;
using Mango.Services.ShoppingCartApi.Services.Interface;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartApi.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClient;

        public CouponService(IHttpClientFactory httpClient)
        {
            this._httpClient = httpClient;
        }
        public async Task<CouponDto> GetCouponByName(string couponCode)
        {
            var client = _httpClient.CreateClient("Coupon");
            var httpRespMessage = await client.GetAsync($"api/CouponApi/GetByCode/{couponCode}");
            var response = await httpRespMessage.Content.ReadAsStringAsync();

            var responseDto = JsonConvert.DeserializeObject<ResponseDto>(response.ToString());
            if (responseDto.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDto>(responseDto.Result.ToString());
                return coupon;
            }

            return new CouponDto();
        }
    }
}
