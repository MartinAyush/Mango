using AutoMapper;
using Mango.MessageBus;
using Mango.Services.OrderApi.Data;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Models.Dto;
using Mango.Services.OrderApi.Services.Interface;
using Mango.Services.OrderApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderApi.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        protected ResponseDto _responseDto;

        public OrderApiController(AppDbContext dbContext, IProductService productService,
            IMapper mapper, IMessageBus messageBus, IConfiguration configuration)
        {
            this._dbContext = dbContext;
            this._productService = productService;
            this._mapper = mapper;
            this._messageBus = messageBus;
            this._configuration = configuration;
            this._responseDto = new ResponseDto();
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = StaticDetails.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailDto>>(cartDto.CartDetails);

                OrderHeader orderHeader = _dbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _dbContext.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderHeader.OrderHeaderId;
                _responseDto.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }

            return _responseDto;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession(StripeRequestDto requestDto)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = requestDto.ApprovedUrl,
                    CancelUrl = requestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",

                };

                var DiscountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions()
                    {
                        Coupon = requestDto.OrderHeader.CouponCode,
                    }
                };

                foreach (var item in requestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "inr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                if (requestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountsObj;
                }

                var service = new SessionService();
                Session session = service.Create(options);
                requestDto.StripSessionUrl = session.Url;

                OrderHeader? orderHeader = _dbContext.OrderHeaders.FirstOrDefault(x => x.OrderHeaderId == requestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _dbContext.SaveChanges();

                _responseDto.Result = requestDto;

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public async Task<ResponseDto> GetOrder(int id)
        {
            try
            {
                var orderHeader = _dbContext.OrderHeaders.Include(x => x.OrderDetails).First(u => u.OrderHeaderId == id);
                _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<ResponseDto> GetOrders(string? userId)
        {
            try
            {
                IEnumerable<OrderHeader> orders;
                if (User.IsInRole(StaticDetails.RoleAdmin))
                {
                    orders = _dbContext.OrderHeaders.Include(x => x.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                else
                {
                    orders = _dbContext.OrderHeaders.Include(x => x.OrderDetails).Where(y => y.UserId == userId).OrderByDescending(u => u.OrderHeaderId);
                }
                _responseDto.Result = _mapper.Map <IEnumerable<OrderHeaderDto>>(orders);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _dbContext.OrderHeaders.First(x => x.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = StaticDetails.Status_Approved;
                    _dbContext.SaveChanges();

                    RewardDto rewardDto = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId,
                    };

                    string topicName = _configuration["TopicAndQueueNames:OrderCreatedTopic"];
                    await _messageBus.PublishMessage(rewardDto, topicName);

                    _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _dbContext.OrderHeaders.First(u => u.OrderHeaderId == orderId);

                if(orderHeader != null)
                {
                    if(newStatus == StaticDetails.Status_Cancelled)
                    {

                        var refundCreateOption = new RefundCreateOptions()
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId
                        };

                        var service = new RefundService();
                        Refund refund = service.Create(refundCreateOption);
                    }

                    orderHeader.Status = newStatus;
                    _dbContext.SaveChanges();
                }
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
