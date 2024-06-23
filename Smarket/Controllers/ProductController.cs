﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.Dtos;
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
                var products = (await _unitOfWork.Product.GetAllAsync(null, p => p.SubCategory, p => p.Brand, p => p.Image)).Take(100).ToList();

                var productDtos = products.Select(p => new ProductDtoWithCBNames
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    SubCategoryName = p.SubCategory?.Name,
                    BrandName = p.Brand?.Name,
                    BrandId = p.Brand?.Id ?? 0, 
                    SubCategoryId = p.SubCategory?.Id ?? 0, 
                    ImageUrl = p.Image?.Url
                });

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, "Internal server error");
            }
        }

        
        [HttpGet("ProductRecomm")]
        public async Task<ActionResult<IEnumerable<ProductDtoWebCard>>> ProductRecomm()
        {
            try
            {
                var packages = (await _unitOfWork.Package.GetAllAsync(null, p => p.Product.Image, p => p.Product.SubCategory
                , p => p.Product.Brand, p => p.Product.Reviews)).ToList();


                var productDtos = packages.Select(p => new ProductDtoWebCard
                {
                    Id = p.Product.Id,
                    Name = p.Product.Name,
                    Description = p.Product.Description,
                    SubCategoryName = p.Product.SubCategory?.Name,
                    BrandName = p.Product.Brand?.Name,
                    ImageUrl = p.Product.Image?.Url,
                    Price = p.Price,
                    Reviews = p.Product.Reviews.Select(p => new ReviewDtoRateOnly
                    {
                        Rate = p.Rate,
                    }).ToList()
                });

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetProductsWeb")]
        public async Task<ActionResult<IEnumerable<ProductDtoWebCard>>> GetProductsWeb()
        {
            try
            {
                var packages = (await _unitOfWork.Package.GetAllAsync(null, p => p.Product.Image, p => p.Product.SubCategory
                , p => p.Product.Brand, p => p.Product.Reviews)).Take(50).ToList();


                var productDtos = packages.Select(p => new ProductDtoWebCard
                {
                    Id = p.Product.Id,
                    Name = p.Product.Name,
                    Description = p.Product.Description,
                    SubCategoryName = p.Product.SubCategory?.Name,
                    BrandName = p.Product.Brand?.Name,
                    ImageUrl = p.Product.Image?.Url,
                    Price= p.Price,
                    Reviews = p.Product.Reviews.Select(p => new ReviewDtoRateOnly
                    {
                        Rate = p.Rate,
                    }).ToList()
                });

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("GetProductsBySearch")]
        public async Task<ActionResult<IEnumerable<ProductDtoWithCBNames>>> GetProductsBySearch(string searchTerm = null)
        {
            try
            {
                var productsQuery = _unitOfWork.Product.GetAllAsync(
                    searchTerm != null ? p => p.Name.Contains(searchTerm) : null,
                    p => p.SubCategory,
                    p => p.Brand,
                    p => p.Image);

                var products = (await productsQuery).Take(10).ToList();

                var productDtos = products.Select(p => new ProductDtoWithCBNames
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    SubCategoryName = p.SubCategory?.Name,
                    BrandName = p.Brand?.Name,
                    BrandId = p.Brand?.Id ?? 0, 
                    SubCategoryId = p.SubCategory?.Id ?? 0, 
                    ImageUrl = p.Image?.Url
                });

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, "Internal server error");
            }
        }




        [HttpGet("GetProductAI")]
        public async Task<ActionResult<IEnumerable<ProductDtoAxies>>> GetProductAI(int id)
        {
            try
            {
                var product = await _unitOfWork.Product.FirstOrDefaultAsync(p=>p.Id==id);
                if (product == null)
                {
                    return NotFound();
                }

                var productDto = new ProductDtoAxies
                {
                    Id = product.Id,
                    Name = product.Name,
                    XAxies = product.XAxies,
                    YAxies = product.YAxies,
                };

                return Ok(productDto);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("GetProductsWithReview")]
        public async Task<ActionResult<IEnumerable<ProductDtoReview>>> GetProductsWithReview()
        {
            try
            {
                var products = await _unitOfWork.Product.GetAllAsync(null, p => p.SubCategory, p => p.Brand, p => p.Image,p=>p.Reviews);

                var productDtos = products.Select(p => new ProductDtoReview
                {
                    Name = p.Name,
                    Description = p.Description,
                    SubCategoryName = p.SubCategory != null ? p.SubCategory.Name : null,
                    BrandName = p.Brand.Name != null ? p.Brand.Name : null,
                    ImageUrl = p.Image.Url,
                    Reviews = p.Reviews.Select(p => new ReviewDto
                    {
                        Comment= p.Comment,
                        ProductId = p.Id,
                        Rate = p.Rate,
                    }).ToList()
                });

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetProductsWithAxies")]
        public async Task<ActionResult<IEnumerable<ProductDtoAllProductAxies>>> GetProductsWithAxies()
        {
            try
            {
                var products = (await _unitOfWork.Product.GetAllAsync()).Take(100).ToList();

                var productDtos = products.Select(p => new ProductDtoAllProductAxies
                {
                    Id = p.Id,
                    Name = p.Name,
                    XAxies = p.XAxies,
                    YAxies = p.YAxies,

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
/*                var product = await _unitOfWork.Product.FirstOrDefaultAsync(p => p.Id == id, p => p.SubCategory, p => p.Brand, p => p.Image, r => r.Reviews);
*/
                var package = await _unitOfWork.Package.FirstOrDefaultAsync(p => p.ProductId == id, p=>p.Product.Image, p => p.Product.Brand,
                    p => p.Product.SubCategory, p => p.Product.Reviews);

                var randomProducts = (await _unitOfWork.Product.GetAllAsync(null, p => p.Image)).ToList();
                
                Random random = new Random();
                List<int> randomIndices = new List<int>();

                while (randomIndices.Count < 3)
                {
                    int randomIndex = random.Next(0, randomProducts.Count - 1);
                    if (!randomIndices.Contains(randomIndex))
                    {
                        randomIndices.Add(randomIndex);
                    }
                }

                List<Product> threeProducts = randomIndices.Select(index => randomProducts[index]).ToList();


                var ReviewWithUsers = await _unitOfWork.UserReview.GetAllAsync(p=>p.ProductId==id,i=>i.User.Image);

                if (package == null)
                {
                    return NotFound();
                }

                var productDto = new ProductDetailsPageMobile
                {
                    Id= package.Product.Id,
                    Name = package.Product.Name,
                    Description = package.Product.Description,
                    SubCategoryName = package.Product.SubCategory.Name,
                    BrandName = package.Product.Brand.Name,
                    price = package.Price,
                    Reviews = ReviewWithUsers.Select(p => new ReviewDtoProductDetails
                    {
                        Comment = p.Comment,
                        ProductId = p.Id,
                        Rate = p.Rate,
                        UserId = p.UserId,
                        ImageUrl = p.User.Image.Url,
                        UserName = p.User.UserName
                    }).ToList(),
                    productDtoFilters = threeProducts.Select(p=> new ProductDtoFilter
                    {
                        ProductId= p.Id,
                        Name = p.Name,
                        ImageUrl=p.Image.Url,
                    }),
                    ImageUrl = package.Product.Image.Url,
                    BrandId = package.Product.Brand.Id,
                    SubCategoryId = package.Product.SubCategory.Id,
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product {id} details");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("GetProductDetailsForWeb/{id}")]
        public async Task<IActionResult> GetProductDetailsForWeb(int id)
        {
            try
            {
                var package = await _unitOfWork.Package.FirstOrDefaultAsync(p => p.ProductId == id, p => p.Product.Image, p => p.Product.Brand,
                    p => p.Product.SubCategory, p => p.Product.Reviews);

                var threeProducts = (await _unitOfWork.Package.GetAllAsync(null, p => p.Product.Image, p => p.Product.Brand,p=>p.Product.SubCategory)).Take(4).ToList();

                var ReviewWithUsers = await _unitOfWork.UserReview.GetAllAsync(p => p.ProductId == id, i => i.User.Image);

                if (package == null)
                {
                    return NotFound();
                }

                var productDto = new ProductDetailsDtoForWeb
                {
                    Id = package.Product.Id,
                    Name = package.Product.Name,
                    Left = package.left,
                    Description = package.Product.Description,
                    SubCategoryName = package.Product.SubCategory.Name,
                    BrandName = package.Product.Brand.Name,
                    price = package.Price,
                    Reviews = ReviewWithUsers.Select(p => new ReviewDtoProductDetails
                    {
                        Comment = p.Comment,
                        ProductId = p.ProductId,
                        Rate = p.Rate,
                        UserId = p.UserId,
                        ImageUrl = p.User.Image.Url,
                        UserName = p.User.UserName
                    }).ToList(),
                    productDtoFilters = threeProducts.Select(p => new ProductDtoFilter
                    {
                        
                        ProductId = p.Product.Id,
                        Price =p.Price,
                        Description= p.Product.Description,
                        SubCategoryName = p.Product.SubCategory.Name,
                        BrandName= p.Product.Brand.Name,
                        Name = p.Product.Name,
                        ImageUrl = p.Product.Image.Url,
                    }),
                    ImageUrl = package.Product.Image.Url,
                    BrandId = package.Product.Brand.Id,                
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

                var product = new Models.Product
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    SubCategoryId = productDto.SubCategoryId,
                    BrandId = productDto.BrandId,
                    Image = new Image
                    {
                        PublicId = imageUploadResult.PublicId,
                        Url = imageUploadResult.Url.ToString(),
                    },
                    YAxies = productDto.YAxies,
                    XAxies = productDto.XAxies,
                    
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

/*        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteAllProducts()
        {
            try
            {
                // Retrieve all products from the database
                var products = await _unitOfWork.Product.GetAllAsync();

                _unitOfWork.Product.DeleteRange(products);
                await _unitOfWork.Save();

                return Ok("All products have been deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting products");
                return StatusCode(500, "Internal server error");
            }
        }*/
      /*  [HttpPut("Edit")]
        public async Task<IActionResult> UpdateProducts(List<ProductDtoWithCBNames> productDtos)
        {
            if (!ModelState.IsValid || productDtos == null || productDtos.Count == 0)
            {
                return BadRequest(ModelState);
            }

            try
            {
                foreach (var productDto in productDtos)
                {

                    var id = productDto.Id;
                    var brand = await _unitOfWork.Brand.FirstOrDefaultAsync(b => b.Name == productDto.BrandName);
                    var product = await _unitOfWork.Product.FirstOrDefaultAsync(p => p.Id == id);

                    if (product == null)
                    {
                        return NotFound($"Product with ID {id} not found");
                    }
                    product.BrandId = brand.Id;
                    

                    _unitOfWork.Product.Update(product);
                }

                await _unitOfWork.Save();

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating products");
                return StatusCode(500, "Internal server error");
            }
        }*/



        /*        [HttpPut("Update")]
                public async Task<IActionResult> UpdateProducts()
                {
                    try
                    {
                        var products = await _unitOfWork.Product.GetAllAsync(null, i => i.Image);
                        string substringToRemove = "images/W/WEBP_402378-T1/";
                        string substringToRemove2 = "images/W/WEBP_402378-T2/";

                        foreach (var product in products)
                        {
                            product.Image.Url = product.Image.Url.Replace(substringToRemove, "");
                            product.Image.PublicId = product.Image.PublicId.Replace(substringToRemove, "");
                            product.Image.Url = product.Image.Url.Replace(substringToRemove2, "");
                            product.Image.PublicId = product.Image.PublicId.Replace(substringToRemove2, "");


                        }

                        await _unitOfWork.Save();

                        return Ok(products.Select(product => new
                        {
                            id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            SubCategoryId = product.SubCategoryId,
                            BrandId = product.BrandId,
                            // You may include other properties here if needed
                        }));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating products");
                        return StatusCode(500, "Internal server error");
                    }
                }
        */

        /*
                [HttpPost("CreateProducts")]
                public async Task<IActionResult> CreateProducts(List<ProductDtoWithoutForm> productDtos)
                {
                    if (!ModelState.IsValid || productDtos == null || productDtos.Count == 0)
                    {
                        return BadRequest("Invalid request or empty product list");
                    }

                    var products = new List<Models.Product>();
                    var productIds = new List<int>();

                    foreach (var productDto in productDtos)
                    {
                        try
                        {
                            var product = new Models.Product
                            {
                                Name = productDto.Name,
                                Description = productDto.Description,
                                SubCategoryId = productDto.SubCategoryId,
                                BrandId = productDto.BrandId,
                                Image = new Image
                                {
                                    PublicId = productDto.ImageUrl,
                                    Url = productDto.ImageUrl,
                                },
                                YAxies = productDto.YAxies,
                                XAxies = productDto.XAxies,
                            };

                            products.Add(product);
                            productIds.Add(product.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error creating product");
                        }
                    }

                    if (products.Count == 0)
                    {
                        return BadRequest("No valid products found in the request");
                    }

                    try
                    {
                        await _unitOfWork.Product.AddRangeAsync(products);
                        await _unitOfWork.Save();

                        return Ok(products.Select(product => new
                        {
                            id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            SubCategoryId = product.SubCategoryId,
                            BrandId = product.BrandId,
                            ImageUrl = product.Image.Url,
                        }));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating products");
                        return StatusCode(500, "Internal server error");
                    }
                }*/





    }
}
