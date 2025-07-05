using AutoMapper;
using Foody.MessageBus;
using Foody.Services.OrderAPI.Data;
using Foody.Services.OrderAPI.Models;
using Foody.Services.OrderAPI.Models.Dto;
using Foody.Services.OrderAPI.Service.IService;
using Foody.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Foody.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;
        private RefundCreateOptions options;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public OrderAPIController(IMapper mapper, AppDbContext db, IProductService productService, IConfiguration configuration, IMessageBus messageBus)
        {
            _mapper = mapper;
            _db = db;
            _productService = productService;
            _response = new ResponseDto();
            _configuration = configuration;
            _messageBus = messageBus;

        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending; // Set initial status to Pending
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = _db.OrderHeader.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,

                    LineItems = new List<SessionLineItemOptions>(),

                    Mode = "payment"

                };
                var DiscountObj = new List<SessionDiscountOptions>
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // Convert to cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName

                            },
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);

                }

                if (stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountObj;
                }

                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeader.FirstOrDefault(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _db.SaveChanges();
                _response.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;

        }


        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeader.FirstOrDefault(u => u.OrderHeaderId == orderHeaderId);
                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);
                var PaymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = PaymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    orderHeader.Status = SD.Status_Approved;
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    _db.SaveChanges();

                    RewardsDto rewardsDto = new RewardsDto
                    {
                        UserId = orderHeader.UserId,
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal)

                    };

                    // Publish the rewards activity to the message bus
                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage(rewardsDto, topicName);


                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Payment not successful";
                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;

        }

        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDto? Get(string? userId)
        {
            if(userId == null || userId == "")
            {
                // If userId is not provided, get it from the claims
               // userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        //        userId = User.Claims.FirstOrDefault(c =>
        //c.Type == JwtRegisteredClaimNames.Sub ||
        //c.Type == ClaimTypes.NameIdentifier ||
        //c.Type == "userId" || c.Type == "nameid")?.Value;

                 userId = User.Claims.FirstOrDefault(c =>
                    c.Type == JwtRegisteredClaimNames.Name ||
                    c.Type == ClaimTypes.Name ||
                    c.Type == "name")?.Value;

                string? email = User.Claims.FirstOrDefault(c =>
                    c.Type == JwtRegisteredClaimNames.Email ||
                    c.Type == ClaimTypes.Email ||
                    c.Type == "email")?.Value;

                string? role = User.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.Role ||
                    c.Type == "role")?.Value;
            }
            else
            {
                // If userId is provided, use it directly
                userId = userId;
            }
        
            try
            {
               

                IEnumerable<OrderHeader> objList;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    objList = _db.OrderHeader.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();

                }
                else
                {
                    objList = _db.OrderHeader.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.OrderHeaderId).ToList();

                }
                    _response.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(objList);
                
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [Authorize]
        [HttpGet("GetOrder/{Id:int}")]
        public ResponseDto? Get(int Id)
        {
            try
            {
                OrderHeader? orderHeader = _db.OrderHeader.Include(u => u.OrderDetails).FirstOrDefault(u => u.OrderHeaderId == Id);
                if (orderHeader == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Order not found";
                }
                else
                {
                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader? orderHeader = _db.OrderHeader.FirstOrDefault(u => u.OrderHeaderId == orderId);
                if (orderHeader == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Order not found";
                }
                else
                {
                    if(newStatus == SD.Status_Cancelled)
                    {
                        //If the order is cancelled , provide refund
                        options = new RefundCreateOptions
                        {
                            PaymentIntent = orderHeader.PaymentIntentId,
                            Reason = RefundReasons.RequestedByCustomer
                        };

                        var _service = new RefundService();
                        Refund refund = _service.Create(options);
                    }
                    orderHeader.Status = newStatus;
                    _db.OrderHeader.Update(orderHeader);
                    await _db.SaveChangesAsync();

                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);

                }
                        
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;


        }
    }
}




