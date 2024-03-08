using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;
using Smarket.Services.IServices;

namespace Smarket.Controllers
{

	public class BrandController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IImageService _imageService;
		private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<BrandController> _logger;
        public BrandController(IUnitOfWork unitOfWork, IImageService imageService, ApplicationDbContext dbContext,
        ILogger<BrandController> logger)
		{
			_unitOfWork = unitOfWork;
			_imageService = imageService;
			_dbContext = dbContext;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<BrandDtoWithImage>>> GetAllBrandsAsync()
		{
			try
			{
				var brands = await _unitOfWork.Brand.GetAllAsync(null,i => i.Image);

				var brandDtos = brands.Select(b => new BrandDtoWithImage
                {
					Id = b.Id,
					Name = b.Name,
                    Image = new { Url = b.Image.Url }
                });

				return Ok(brandDtos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting brands");
				return StatusCode(500, "Internal server error");
			}
		}
		[HttpGet("Details/{id:int}")]
		public async Task<ActionResult<BrandDtoWithImage>> GetBrandDetailsAsync(int id)
		{
			// Validate input
			if (id <= 0)
				return BadRequest("Invalid brand id");

			try
			{
				// Get brand data
				var brand = await _unitOfWork.Brand.FirstOrDefaultAsync(b => b.Id == id, i => i.Image);

				if (brand == null)
					return NotFound();

				// Map to DTO
				var brandDetails = new BrandDtoWithImage
				{
					Id= brand.Id,
					Name = brand.Name,
                    Image = new { Url = brand.Image.Url }
                };

				return Ok(brandDetails);

			}
			catch (Exception ex)
			{
				// Log error
				_logger.LogError(ex, $"Error getting brand {id}");

				return StatusCode(500, "Internal server error");
			}
		}
		[HttpPost("Create")]
		public async Task<IActionResult> CreateBrandAsync([FromForm] BrandDto brandDto)
		{
			// Validate model state
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				// Upload image
				var imageUploadResult = await _imageService.AddPhotoAsync(brandDto.formFile);

				// Create brand 
				var brand = new Brand
				{
					Name = brandDto.Name,
					Image = new Image
					{
						PublicId = imageUploadResult.PublicId,
						Url = imageUploadResult.Url.ToString(),
					}
				};

				// Save 
				using (var transaction = _dbContext.Database.BeginTransaction())
				{
					_dbContext.Brands.Add(brand);
					await _dbContext.SaveChangesAsync();

					transaction.Commit();
				}

				return Ok(brand);

			}
			catch (Exception ex)
			{
				// Log error
				_logger.LogError(ex, "Error creating brand");
				return StatusCode(500, "Internal server error");
			}
		}
       

        [HttpPut("Edit/{id:int}")]
		public async Task<ActionResult> UpdateBrandAsync(int id, [FromForm] BrandDto updatedBrandDto)
		{
			// Validate input
			if (id <= 0 || !ModelState.IsValid)
				return BadRequest();

			try
			{
				// Get old brand data
				var oldBrand = await _unitOfWork.Brand.FirstOrDefaultAsync(b => b.Id == id, i => i.Image);
				if (oldBrand == null)
					return NotFound();

				// Delete old image
				if (oldBrand.Image?.PublicId != null)
					await _imageService.DeletePhotoAsync(oldBrand.Image.PublicId);

				// Upload new image
				var imageUploadResult = await _imageService.AddPhotoAsync(updatedBrandDto.formFile);

				// Update brand fields
				oldBrand.Name = updatedBrandDto.Name;
				oldBrand.Image.PublicId = imageUploadResult.PublicId;
				oldBrand.Image.Url = imageUploadResult.Url.ToString();


				// Save 
				using (var transaction = _dbContext.Database.BeginTransaction())
				{
					_unitOfWork.Brand.Update(oldBrand);
					await _unitOfWork.Save();
					transaction.Commit();
				}
				return Ok(new
				{
					Name = oldBrand.Name,
					ImageUrl = oldBrand.Image.Url.ToString(),
				});
			}
			catch (Exception ex)
			{
				// Log error
				_logger.LogError(ex, $"Error updating brand {id}");

				return StatusCode(500, "Internal server error");
			}
		}

		[HttpDelete("Delete/{id:int}")]
		public async Task<ActionResult> DeleteBrandAsync(int id)
		{
			try
			{
				// Validate input   
				if (id <= 0)
					return BadRequest("Invalid id");

				// Get brand to delete
				var brand = await _unitOfWork.Brand.FirstOrDefaultAsync(b => b.Id == id);
				if (brand == null)
					return NotFound();

				// Delete image
				if (brand.Image?.PublicId != null)
					await _imageService.DeletePhotoAsync(brand.Image.PublicId);

				var products = await _unitOfWork.Product.GetAllAsync();
				
				foreach (var product in products)
				{
					if (product.BrandId == id)
					{
						product.BrandId = null;
						_unitOfWork.Product.Update(product);
					}
				}
				await _unitOfWork.Save();
				
				// Delete brand
				_unitOfWork.Brand.Delete(brand);
				await _unitOfWork.Save();

				return Ok("Brand Has Deleted");
			}
			catch (Exception ex)
			{
				// Log error
				_logger.LogError(ex, $"Error deleting brand {id}");

				return StatusCode(500, "Internal server error");
			}
		}

      /*  [HttpDelete("DeleteAll")]
        public async Task<ActionResult> DeleteAllBrandsAsync()
        {
            try
            {
                var brandsToDelete = await _unitOfWork.Brand.GetAllAsync();

                if (!brandsToDelete.Any())
                    return NotFound("No brands found to delete");

                foreach (var brand in brandsToDelete)
                {
                    // Delete image
                    if (brand.Image?.PublicId != null)
                        await _imageService.DeletePhotoAsync(brand.Image.PublicId);

					var products = await _unitOfWork.Product
						.GetAllAsync(p => p.BrandId == brand.Id);

                    foreach (var product in products)
                    {
                        product.BrandId = null;
                        _unitOfWork.Product.Update(product);
                    }

                    _unitOfWork.Brand.Delete(brand);
                }

                await _unitOfWork.Save();

                return Ok("All brands have been deleted");
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError(ex, "Error deleting brands");

                return StatusCode(500, "Internal server error");
            }
        }*/


    }
}