using Foody.Web.Models;
using Foody.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Foody.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService) 
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO>? list = new();

            ResponseDto? response = await _couponService.GetAllCouponAsync();
            if (response != null && response.IsSuccess )
            {
                list = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCoupon()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCoupon(int couponId)
        {
            ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);
            if (response != null && response.IsSuccess)
            {
                CouponDTO? model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> GetCoupon(string coupon)
        {
            ResponseDto? response = await _couponService.GetCouponAsync(coupon);
            if (response != null && response.IsSuccess)
            {
                CouponDTO? model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDTO model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _couponService.CreateCouponAsync(model);
                if (response != null && response.IsSuccess )
                {
                    TempData["success"] = "Coupon created successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
               
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteCoupon(int couponId)
        {
            ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);
            if (response != null && response.IsSuccess)
            {
                CouponDTO? model =  JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(CouponDTO model)
        {
           
                ResponseDto? response = await _couponService.DeleteCouponAsync(model.CouponId);
                if (response != null && response.IsSuccess)
                {
                TempData["Success"] = "Coupon deleted successfully";
                return RedirectToAction(nameof(CouponIndex));
                }
            else
            {
                TempData["Error"] = response?.Message;
            }

            return View(model);
        }

    }
}
