using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;

namespace Smarket.Controllers
{

	public class CategoryController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;

		public CategoryController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var categories = await _unitOfWork.Category.GetAllAsync();
			return Ok(categories);
		}
		[HttpGet("Details")]
		public async Task<IActionResult> Details(int id)
		{
			var Category = await _unitOfWork.Category.FirstOrDefaultAsync(b => b.Id == id);
			return Ok(Category);
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(CategoryDto dto)
		{
			var Category = new Category
			{
				Name = dto.Name,
				ImageId = dto.ImageId,

			};
			if (ModelState.IsValid)
			{
				await _unitOfWork.Category.AddAsync(Category);
				await _unitOfWork.Save();
				return Ok(Category);
			}
			return RedirectToAction("There is an Error while Deleting");
		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(int id, CategoryDto dto)
		{
			if (ModelState.IsValid)
			{
				var oldCategory = await _unitOfWork.Category.FirstOrDefaultAsync(x => x.Id == id);
				oldCategory.Name = dto.Name;
				oldCategory.ImageId = dto.ImageId;
				_unitOfWork.Category.Update(oldCategory);
				await _unitOfWork.Save();
				return Ok(oldCategory);
			}
			return RedirectToAction("There is an Error while Deleting");
		}

		[HttpPost("Delete")]
		public async Task<IActionResult> Delete(int id)
		{
			if (ModelState.IsValid)
			{
				var Category = await _unitOfWork.Category.FirstOrDefaultAsync(x => x.Id == id);
				_unitOfWork.Category.Delete(Category);
				await _unitOfWork.Save();
				return Ok("Category Has Deleted");
			}
			return RedirectToAction("There is an Error while Deleting");
		}
	}
}