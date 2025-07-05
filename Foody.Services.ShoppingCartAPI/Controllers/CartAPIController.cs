using AutoMapper;
using Foody.MessageBus;
using Foody.Services.ShoppingCartAPI.Data;
using Foody.Services.ShoppingCartAPI.Models;
using Foody.Services.ShoppingCartAPI.Models.Dto;
using Foody.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Foody.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;
        private ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private IConfiguration _configuration;

        public CartAPIController(AppDbContext db,IConfiguration configuration , IMapper mapper ,IMessageBus messageBus, IProductService productService , ICouponService couponService)
        {
            _response = new ResponseDto();
            _mapper = mapper;
            _db = db;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert([FromBody]CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeader.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeader.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create cartdetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpDelete("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cartDetailsId)
        {
            try
            {
              CartDetails cartDetails = _db.CartDetails.First(u => u.CartDetailsId == cartDetailsId);
                if (cartDetails == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Cart Details Not Found";
                }
                else
                {
                    int totalCountOfCartIems = _db.CartDetails
                       .Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();

                    _db.CartDetails.Remove(cartDetails);
                   
                    if (totalCountOfCartIems == 1)
                    {
                       var cartHeaderToRemove = await  _db.CartHeader
                           .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                        _db.CartHeader.Remove(cartHeaderToRemove);
                    }

                   
                    await _db.SaveChangesAsync();
                    _response.Result = true; 
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody]CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeader.AsNoTracking()
                    .FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeader.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;

        }

       




        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new CartDto()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(
                        await _db.CartHeader.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId))
                };
                if (cartDto.CartHeader != null)
                {
                    cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(
                         _db.CartDetails.AsNoTracking()
                        .Where(u => u.CartHeaderId == cartDto.CartHeader.CartHeaderId));

                    IEnumerable<ProductDto> productDtos =
                        await _productService.GetProducts();

                    foreach (var item in cartDto.CartDetails)
                    {
                        item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                        cartDto.CartHeader.CartTotal += (item.Count * item.Product.Price);
                    }

                    //apply coupon
                    if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                    {
                        CouponDTO coupon = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                        if (coupon != null && cartDto.CartHeader.CartTotal >= coupon.DiscountAmount)
                        {
                            cartDto.CartHeader.Discount = coupon.DiscountAmount;
                            cartDto.CartHeader.CartTotal -= coupon.DiscountAmount;
                        }
                    }
                }
                else
                {
                    cartDto.CartDetails = new List<CartDetailsDto>();
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;

        }



    }
}