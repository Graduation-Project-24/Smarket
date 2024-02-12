using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;
using Smarket.Models.ViewModels;
using Smarket.Services.IServices;

namespace Smarket.Controllers
{
    public class ProductController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IUnitOfWork unitOfWork, IImageService imageService,
        ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _logger = logger;
        }
        // Smarket\Controllers\ProductController.cs

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDtoWithCBNames>>> GetProducts()
        {
            try
            {
                var products = await _unitOfWork.Product.GetAllAsync(null, p => p.SubCategory, p => p.Brand, p => p.Image);

                var productDtos = products.Select(p => new ProductDtoWithCBNames
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    SubCategoryName = p.SubCategory != null ? p.SubCategory.Name : null,
                    BrandName = p.Brand.Name != null ? p.Brand.Name : null,
                    BrandId = p.Brand.Id,
                    SubCategoryId = p.SubCategory.Id,
                    ImageUrl = p.Image.Url
                });

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, "Internal server error");
            }
        }


        // Smarket\Controllers\ProductController.cs

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            try
            {
                var product = await _unitOfWork.Product.FirstOrDefaultAsync(p => p.Id == id, p => p.SubCategory, p => p.Brand, p => p.Image);

                if (product == null)
                {
                    return NotFound();
                }

                var productDto = new ProductDtoWithCBNames
                {
                    Id= product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    SubCategoryName = product.SubCategory.Name,
                    BrandName = product.Brand.Name,
                    ImageUrl = product.Image.Url,
                    BrandId = product.Brand.Id,
                    SubCategoryId = product.SubCategory.Id,
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product {id} details");
                return StatusCode(500, "Internal server error");
            }
        }

        // Smarket\Controllers\ProductController.cs

        [HttpPost("Create")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var imageUploadResult = await _imageService.AddPhotoAsync(productDto.formFile);

                var product = new Product
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    SubCategoryId = productDto.SubCategoryId,
                    BrandId = productDto.BrandId,
                    Image = new Image
                    {
                        PublicId = imageUploadResult.PublicId,
                        Url = imageUploadResult.Url.ToString(),
                    }
                };

                await _unitOfWork.Product.AddAsync(product);
                await _unitOfWork.Save();

                return Ok(new 
                {
                    id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    SubCategoryId = product.SubCategoryId,
                    BrandId = product.BrandId,
                    ImageUrl= product.Image.Url,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Internal server error");
            }
        }
        // Smarket\Controllers\ProductController.cs

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = await _unitOfWork.Product.FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                var imageUploadResult = await _imageService.AddPhotoAsync(productDto.formFile);

                product.Name = productDto.Name;
                product.Description = productDto.Description;
                product.SubCategoryId = productDto.SubCategoryId;
                product.BrandId = productDto.BrandId;
                product.Image.PublicId = imageUploadResult.PublicId;
                product.Image.Url = imageUploadResult.Url.ToString();

                _unitOfWork.Product.Update(product);
                await _unitOfWork.Save();

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Smarket\Controllers\ProductController.cs

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _unitOfWork.Product.FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound();
                }

                _unitOfWork.Product.Delete(product);
                await _unitOfWork.Save();

                return Ok("The Product Have been Deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product {id}");
                return StatusCode(500, "Internal server error");
            }
        }
        // Smarket\Controllers\ProductController.cs

        [HttpGet("BySubCategory/{subCategoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDtoFilter>>> GetProductsBySubCategory(int subCategoryId)
        {
            try
            {
                var products = await _unitOfWork.Product.GetAllAsync(p => p.SubCategoryId == subCategoryId,
                p=>p.SubCategory, p => p.Brand, p => p.Image);

                if (!products.Any())
                {
                    return NotFound();
                }

                var productDtos = products.Select(p => new ProductDtoFilter
                {
                    Name = p.Name,
                    Description = p.Description,
                    SubCategoryName = p.SubCategory.Name,
                    BrandName = p.Brand.Name,
                    ImageUrl = p.Image.Url
                });

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting products for subcategory {subCategoryId}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Smarket\Controllers\ProductController.cs

        [HttpGet("ByBrand/{brandId}")]
        public async Task<ActionResult<IEnumerable<ProductDtoFilter>>> GetProductsByBrand(int brandId)
        {
            try
            {
                var products = await _unitOfWork.Product.GetAllAsync(p => p.BrandId == brandId,
                    p => p.SubCategory, p => p.Brand, p => p.Image);

                if (!products.Any())
                {
                    return NotFound();
                }

                var productDtos = products.Select(p => new ProductDtoFilter
                {
                    Name = p.Name,
                    Description = p.Description,
                    SubCategoryName = p.SubCategory.Name,
                    BrandName = p.Brand.Name,
                    ImageUrl = p.Image.Url
                });

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting products for brand {brandId}");
                return StatusCode(500, "Internal server error");
            }
        }




    }
}
