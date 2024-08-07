﻿using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;
using Smarket.Services;
using Smarket.Services.IServices;

namespace Smarket.Controllers
{

	public class CategoryController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IImageService _imageService;
		private readonly ApplicationDbContext _dbContext;
		private readonly ILogger<CategoryController> _logger;

		public CategoryController(IUnitOfWork unitOfWork, IImageService imageService,
		ApplicationDbContext dbContext,
		ILogger<CategoryController> logger)
		{
			_unitOfWork = unitOfWork;
			_imageService = imageService;
			_logger = logger;
			_dbContext = dbContext;


		}
		[HttpGet]
		public async Task<ActionResult<IEnumerable<CategoryDtoImageUrl>>> GetCategoriesAsync()
		{
			try
			{
				var categories = await _unitOfWork.Category.GetAllAsync(null, i => i.Image);

				var categoryDtos = categories.Select(c => new CategoryDtoImageUrl
                {
					Name = c.Name,
					Image = new { Url = c.Image.Url }
                });

				return Ok(categoryDtos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting categories");
				return StatusCode(500, "Internal server error");
			}
		}

        [HttpGet("GetAllWithSubCategories")]
        public async Task<IActionResult> GetAllWithSubCategories()
        {
            var categories = await _unitOfWork.Category.GetAllAsync(null, i => i.Image, i => i.SubCategories);
            return Ok(categories);
        }


        [HttpGet("Details/{id:int}")]
		public async Task<ActionResult<CategoryDtoImageUrl>> GetCategoryDetailsAsync(int id)
		{
			// Validate input
			if (id <= 0)
				return BadRequest("Invalid category id");

			try
			{
				// Get category data
				var category = await _unitOfWork.Category.FirstOrDefaultAsync(c => c.Id == id, i => i.Image);

				if (category == null)
					return NotFound();

				// Map to DTO
				var categoryDetails = new CategoryDtoImageUrl
                {
					Name = category.Name,
					Image = new { Url = category.Image.Url }
				};

				return Ok(categoryDetails);

			}
			catch (Exception ex)
			{
				// Log error
				_logger.LogError(ex, $"Error getting category {id}");

				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPost("CreateCategoryAsync")]
		public async Task<IActionResult> CreateCategoryAsync([FromForm] CategoryDto categoryDto)
		{
			// Validate model
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				// Upload image
				var imageUploadResult = await _imageService.AddPhotoAsync(categoryDto.formFile);

				// Create category
				var category = new Category
				{
					Name = categoryDto.Name,
					Image = new Image
					{
						PublicId = imageUploadResult.PublicId,
						Url = imageUploadResult.Url.ToString()
					}
				};

				// Save
				using (var transaction = _dbContext.Database.BeginTransaction())
				{
					_dbContext.Categories.Add(category);
					await _dbContext.SaveChangesAsync();

					transaction.Commit();
				}

				return Ok(category);

			}
			catch (Exception ex)
			{
				// Log error
				_logger.LogError(ex, "Error creating category");

				return StatusCode(500, "Internal server error");
			}
		}


		[HttpPut("Edit/{id:int}")]
		public async Task<ActionResult> UpdateCategoryAsync(int id, [FromForm] CategoryDto updatedCategoryDto)
		{

			// Validate 
			if (id <= 0 || !ModelState.IsValid)
				return BadRequest();

			try
			{
				// Get old category data
				var oldCategory = await _unitOfWork.Category.FirstOrDefaultAsync(c => c.Id == id, i => i.Image);
				if (oldCategory == null)
					return NotFound();

				// Delete old image
				if (oldCategory.Image?.PublicId != null)
					await _imageService.DeletePhotoAsync(oldCategory.Image.PublicId);

				// Upload new image
				var imageUploadResult = await _imageService.AddPhotoAsync(updatedCategoryDto.formFile);

				// Update category fields
				oldCategory.Name = updatedCategoryDto.Name;
				oldCategory.Image.PublicId = imageUploadResult.PublicId;
				oldCategory.Image.Url = imageUploadResult.Url.ToString();

				// Save
				using (var transaction = _dbContext.Database.BeginTransaction())
				{
					_unitOfWork.Category.Update(oldCategory);
					await _unitOfWork.Save();

					transaction.Commit();
				}

				// Return response
				return Ok(new
				{
					Name = oldCategory.Name,
					ImageUrl = oldCategory.Image.Url.ToString()
				});

			}
			catch (Exception ex)
			{
				// Log error
				_logger.LogError(ex, $"Error updating category {id}");

				return StatusCode(500, "Internal server error");
			}

		}
		[HttpDelete("Delete/{id:int}")]
		public async Task<ActionResult> DeleteCategoryAsync(int id)
		{
			try
			{
				if (id <= 0)
					return BadRequest("Invalid id");

				// Get category
				var category = await _unitOfWork.Category.FirstOrDefaultAsync(c => c.Id == id);
				if (category == null)
					return NotFound();

				// Delete image
				if (category.Image?.PublicId != null)
					await _imageService.DeletePhotoAsync(category.Image.PublicId);

				// Delete subcategories
				var subCategories = await _unitOfWork.SubCategory.GetAllAsync(sc => sc.CategoryId == id);
				foreach (var subCat in subCategories)
				{
					_unitOfWork.SubCategory.Delete(subCat);
				}

				// Delete category
				_unitOfWork.Category.Delete(category);
				await _unitOfWork.Save();

				return Ok("Category deleted");

			}
			catch (Exception ex)
			{
				// Log error
				_logger.LogError(ex, $"Error deleting category {id}");

				return StatusCode(500, "Internal server error");
			}
		}

	}
}