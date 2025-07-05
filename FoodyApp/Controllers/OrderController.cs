using Foody.Web.Models;
using Foody.Web.Models.Dto;
using Foody.Web.Service.IService;
using Foody.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Foody.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        public IActionResult OrderIndex()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeaderDto> orderList = new List<OrderHeaderDto>();
            string? userId = "";

            if (!User.IsInRole(SD.RoleAdmin))
            {

                userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            Console.WriteLine($"User ID: {userId}");

            ResponseDto response = await _orderService.GetAllOrders(userId);

            if (response != null && response.IsSuccess)
            {
                orderList = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(response.Result));

                switch (status?.ToLower())
                {
                    case "approved":
                        orderList = orderList.Where(o => o.Status == SD.Status_Approved).ToList();
                        break;
                    case "pending":
                        orderList = orderList.Where(o => o.Status == SD.Status_Pending).ToList();
                        break;
                    case "readyforpickup":
                        orderList = orderList.Where(o => o.Status == SD.Status_ReadyForPickup).ToList();
                        break;
                    case "completed":
                        orderList = orderList.Where(o => o.Status == SD.Status_Completed).ToList();
                        break;
                    case "cancelled":
                        orderList = orderList.Where(o => o.Status == SD.Status_Cancelled).ToList();
                        break;
                }

                return Json(new { data = orderList.OrderByDescending(o => o.OrderHeaderId) });
            }

            return Json(new { data = new List<OrderHeaderDto>() });
        }


        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDto orderHeader = new OrderHeaderDto();
            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value; // Get the user ID from claims
            ResponseDto response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
                if (orderHeader.UserId != userId && !User.IsInRole(SD.RoleAdmin))
                {
                    return NotFound();
                }
            }
            return View(orderHeader);
        }

        [HttpPost("OrderReadyForPickUp")]
        public async Task<IActionResult> OrderReadyForPickUp(int orderId)
        {
            ResponseDto response = await _orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Order is ready for pickup - Status updated successfully.";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View(nameof(OrderDetail));
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            ResponseDto response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Completed);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Order completed - Status updated successfully.";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            ResponseDto response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Order cancelled - Status updated successfully.";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }
    }
}
