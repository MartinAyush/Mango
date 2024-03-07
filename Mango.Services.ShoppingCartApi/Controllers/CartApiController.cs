using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ProductApi.Data;
using Mango.Services.ShoppingCartApi.Models;
using Mango.Services.ShoppingCartApi.Models.Dto;
using Mango.Services.ShoppingCartApi.Models.DTO;
using Mango.Services.ShoppingCartApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartApiController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private ResponseDto _responseDto;

        public CartApiController(AppDbContext dbContext, IMapper mapper, IProductService productService, 
            ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
            this._productService = productService;
            this._couponService = couponService;
            this._messageBus = messageBus;
            this._configuration = configuration;
            this._responseDto = new();
        }

        [HttpGet]
        [Route("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                var cartHeader = _dbContext.CartHeaders.FirstOrDefault(x => x.UserId == userId);
                CartDto userCart = new CartDto()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(cartHeader)
                };

                userCart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_dbContext.CartDetails
                    .Where(x => x.CartHeaderId == userCart.CartHeader.CartHeaderId).ToList());

                var products = await _productService.GetAllProducts();

                foreach (var cd in userCart.CartDetails)
                {
                    cd.Product = products.First(p => p.ProductId == cd.ProductId);
                    userCart.CartHeader.CartTotal += (cd.Product.Price * cd.Count);
                }

                if (!string.IsNullOrEmpty(userCart.CartHeader.CouponCode))
                {
                    var appliedCoupon = await _couponService.GetCouponByName(userCart.CartHeader.CouponCode);
                    if(appliedCoupon.MinAmmount < userCart.CartHeader.CartTotal)
                    {
                        userCart.CartHeader.CartTotal -= appliedCoupon.DiscountAmmount;
                        userCart.CartHeader.Discount = appliedCoupon.DiscountAmmount;
                    }
                }

                _responseDto.Result = userCart;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [HttpPost]
        [Route("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                // Check if user have items in cart
                var cartHeaderFromDb = await _dbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cartDto.CartHeader.UserId);

                if (cartHeaderFromDb != null)
                {
                    // check if current product is already in cart
                    var cartDetailsFromDb = await _dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        c => c.CartHeaderId == cartHeaderFromDb.CartHeaderId &&
                        c.ProductId == cartDto.CartDetails.First().ProductId);

                    if (cartDetailsFromDb != null)
                    {
                        // user is updating item count
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        _dbContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        // user adding new item into cart
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    // user is adding item for the first time
                    var newCartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    var cartHeader = _dbContext.CartHeaders.Add(newCartHeader);
                    await _dbContext.SaveChangesAsync();

                    cartDto.CartDetails.First().CartHeaderId = newCartHeader.CartHeaderId;
                    var cartDetail = _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _dbContext.SaveChangesAsync();
                }
                _responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpDelete]
        [Route("RemoveCartItem")]
        public async Task<ResponseDto> RemoveCartItem([FromBody] int cartDetailsId)
        {
            try
            {
                var cartDetail = _dbContext.CartDetails.FirstOrDefault(x => x.CartDetailsId == cartDetailsId);
                if (cartDetail != null)
                {
                    var noOfItems = _dbContext.CartDetails.Where(x => x.CartHeaderId == cartDetail.CartHeaderId).Count();

                    if (noOfItems == 1)
                    {
                        // remove cart header
                        var cartHeader = _dbContext.CartHeaders.FirstOrDefault(x => x.CartHeaderId == cartDetail.CartHeaderId);
                        _dbContext.CartHeaders.Remove(cartHeader);
                    }

                    _dbContext.CartDetails.Remove(cartDetail);

                    await _dbContext.SaveChangesAsync();
                }
                _responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }

            return _responseDto;
        }

        [HttpPost]
        [Route("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartHeader = _dbContext.CartHeaders.First(x => x.UserId == cartDto.CartHeader.UserId);
                cartHeader.CouponCode = cartDto.CartHeader.CouponCode;
                _dbContext.CartHeaders.Update(cartHeader);
                await _dbContext.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }

            return _responseDto;
        }

        [HttpPost]
        [Route("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PublishMessage(cartDto, _configuration["TopicAndQueueNames:EmailShoppingCartQueue"]);
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }

            return _responseDto;
        }
    }
}
