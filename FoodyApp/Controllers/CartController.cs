using Foody.Web.Models.Dto;
using Foody.Web.Models;
using Foody.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Foody.Web.Utility;

namespace Foody.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IcartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(IcartService cartService , IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

    

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }


        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
           CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Name = cartDto.CartHeader.Name;

            var response = await _orderService.CreateOrder(cart);
            OrderHeaderDto orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                //get stripe session and resdirect to payment page
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDto stripeRequestDto = new StripeRequestDto
                {
                    ApprovedUrl = domain + "Cart/Confirmation?OrderId=" + orderHeader.OrderHeaderId,
                    CancelUrl = domain + "Cart/Checkout",
                    OrderHeader = orderHeader
                };
                 var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDto);

                StripeRequestDto stripeResponseResult = JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));
                Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl);
                return new StatusCodeResult(303); // Redirect to the Stripe payment page

            }
            return View();
        }

     
        public async Task<IActionResult> Confirmation(int OrderId)
        {
            ResponseDto? response = await _orderService.ValidateStripeSession(OrderId);
            if (response == null || !response.IsSuccess)
            {
                TempData["error"] = response?.Message ?? "Error validating order.";
                return RedirectToAction(nameof(CartIndex));
            }
            OrderHeaderDto orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
            if (orderHeader.Status == SD.Status_Approved)
            {
                return View(OrderId);
            }
           

            return View(OrderId);
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null & response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto();
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null & response.IsSuccess)
            {
               TempData["success"] = "Item removed from cart successfully.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error removing item from cart.";
            }
                return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            if (cartDto.CartHeader.UserId != userId)
            {
                return Unauthorized();
            }
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon applied successfully.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error applying coupon.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            if (cartDto.CartHeader.UserId != userId)
            {
                return Unauthorized();
            }
            cartDto.CartHeader.CouponCode = string.Empty; // Clear the coupon code
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon removed successfully.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error removing coupon.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;


            ResponseDto? response = await _cartService.EmailCart(cart);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error emailing cart.";
                return View();
            }
        }
    }
}
