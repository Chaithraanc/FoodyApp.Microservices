using Foody.Web.Models;
using Foody.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Foody.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService  _productService;
        public ProductController(IProductService productService) 
        {
            _productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? list = new();

            ResponseDto? response = await _productService.GetAllProductsAsync();
            if (response != null && response.IsSuccess )
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.CreateProductAsync(model);
                if (response != null && response.IsSuccess )
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
               
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteProduct(int productId)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {
                ProductDto? model =  JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(ProductDto model)
        {
           
                ResponseDto? response = await _productService.DeleteProductAsync(model.ProductId);
                if (response != null && response.IsSuccess)
                {
                TempData["Success"] = "Product deleted successfully";
                return RedirectToAction(nameof(ProductIndex));
                }
            else
            {
                TempData["Error"] = response?.Message;
            }

            return View(model);
        }


        public async Task<IActionResult> EditProduct(int productId)
        {
            if (ModelState.IsValid)
            {

                ResponseDto? response = await _productService.GetProductByIdAsync(productId);
                if (response != null && response.IsSuccess)
                {
                    ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                    return View(model);
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return NotFound();

        }

       

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.UpdateProductsAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }

            }
            return View(model);
        }

    }
}
