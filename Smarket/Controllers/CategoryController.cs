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
		public async Task<IActionResult> Index()
		{
			var categories =await _unitOfWork.Category.GetAllAsync();
			return Ok(new CategoryDto {Categories=categories});
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(Category category)
		{
			if (ModelState.IsValid)
			{
				await _unitOfWork.Category.AddAsync(category);
				await _unitOfWork.Save();
				return Ok(category);
			}
			return RedirectToAction("There is an Error while Deleting");
		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(Category category)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.Category.Update(category);
				await _unitOfWork.Save();
				return Ok(category);
			}
            return RedirectToAction("There is an Error while Deleting");
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(Category category)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.Category.Delete(category);
				await _unitOfWork.Save();
				return Ok("Category Has Deleted");
			}
            return RedirectToAction("There is an Error while Deleting");
        }
	}
}