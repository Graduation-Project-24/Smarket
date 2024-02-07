using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
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

        public CategoryController(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }
        [HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var categories = await _unitOfWork.Category.GetAllAsync(null,i => i.Image,i=>i.SubCategories);
            return Ok(categories);
		}
		[HttpGet("Details")]
		public async Task<IActionResult> Details(int id)
		{
			var Category = await _unitOfWork.Category.FirstOrDefaultAsync(b => b.Id == id, i => i.Image);
			return Ok(Category);
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create([FromForm]CategoryDto dto)
		{

			var image = await _imageService.AddPhotoAsync(dto.formFile);

            var cloudimage = new Image
            {
                PublicId = image.PublicId,
                Url = image.Url.ToString(),
            };


            var Category = new Category
			{
				Name = dto.Name,
				Image = cloudimage
			};
			if (ModelState.IsValid)
			{
				await _unitOfWork.Category.AddAsync(Category);
				await _unitOfWork.Save();
				return Ok(new 
				{
					Name= dto.Name,
					Image = cloudimage.Url.ToString(),
				});
			}
			return RedirectToAction("There is an Error while Deleting");
		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(int id,[FromForm] CategoryDto dto)
		{
			if (ModelState.IsValid)
			{
				var oldCategory = await _unitOfWork.Category.FirstOrDefaultAsync(x => x.Id == id, i => i.Image);

				await _imageService.DeletePhotoAsync(oldCategory.Image.PublicId);

				var image = await _imageService.AddPhotoAsync(dto.formFile);

				oldCategory.Name = dto.Name;
				oldCategory.Image.PublicId = image.PublicId;
				oldCategory.Image.Url = image.Url.ToString();

				_unitOfWork.Category.Update(oldCategory);
				await _unitOfWork.Save();
				return Ok(new
				{
					Name = dto.Name,
					ImageUrl = image.Url.ToString(),
				});
			}
			return RedirectToAction("There is an Error while Deleting");
		}

		[HttpPost("Delete")]
		public async Task<IActionResult> Delete(int id)
		{
			if (ModelState.IsValid)
			{
				var Category = await _unitOfWork.Category.FirstOrDefaultAsync(x => x.Id == id, i => i.Image);

                await _imageService.DeletePhotoAsync(Category.Image.PublicId);

                _unitOfWork.Category.Delete(Category);
				await _unitOfWork.Save();
				return Ok("Category Has Deleted");
			}
			return RedirectToAction("There is an Error while Deleting");
		}
	}
}