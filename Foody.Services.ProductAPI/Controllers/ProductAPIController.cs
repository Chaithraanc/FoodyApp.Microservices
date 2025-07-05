using AutoMapper;
using Foody.Services.ProductAPI.Data;
using Foody.Services.ProductAPI.Models;
using Foody.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Foody.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
   // [Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        public ProductAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> objList = _db.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);


            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product obj = _db.Products.First(u => u.ProductId == id);
                _response.Result = _mapper.Map<ProductDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto PostProduct([FromForm] ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);

                // Temporarily set a placeholder for ImageUrl
                product.ImageUrl = "https://placehold.co/600x400";

                _db.Products.Add(product);
                _db.SaveChanges(); // Save once to get ProductId (if using Identity/Auto-Increment)

                // Handle image after ProductId is assigned
                if (productDto.Image != null)
                {
                    string fileExtension = Path.GetExtension(productDto.Image.FileName);
                    string fileName = product.ProductId + fileExtension;
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages");

                    // Ensure directory exists
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        productDto.Image.CopyTo(fileStream);
                    }

                    string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                    product.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
                    product.ImageLocalPath = filePath;

                    // Update with image info
                    _db.Products.Update(product);
                    _db.SaveChanges();
                }

                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }


        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto PutProduct([FromForm] ProductDto productDto)
        {
            try
            {
                var product = _db.Products.FirstOrDefault(u => u.ProductId == productDto.ProductId);
                if (product == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Product not found";
                    return _response;
                }

                // Delete old image if it exists
                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Map updated values (excluding Image for now)
                _mapper.Map(productDto, product);

                // Upload new image if provided
                if (productDto.Image != null)
                {
                    string fileExtension = Path.GetExtension(productDto.Image.FileName);
                    string fileName = product.ProductId + fileExtension;

                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        productDto.Image.CopyTo(fileStream);
                    }

                    string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                    product.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
                    product.ImageLocalPath = Path.Combine("wwwroot", "ProductImages", fileName); // store relative path
                }

                _db.Products.Update(product);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }


        [HttpDelete]
        [Route("{id:int}")]
       [Authorize(Roles = "ADMIN")]
        public ResponseDto DeleteProduct(int  id)
        {
            try
            {
                Product obj = _db.Products.First(u => u.ProductId == id);
                if(!string.IsNullOrEmpty(obj.ImageLocalPath))
                {
                    var OldfilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);
                    // Check if the file exists before attempting to delete it
                    FileInfo file = new FileInfo(OldfilePathDirectory);
                    if (file.Exists)
                    {
                        file.Delete(); // Delete the file if it exists
                    }
                }
                _db.Products.Remove(obj);
                _db.SaveChanges();

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
