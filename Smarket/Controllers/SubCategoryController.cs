using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;

namespace Smarket.Controllers
{

	public class SubCategoryController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;

		public SubCategoryController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var Subcategories =await _unitOfWork.SubCategory.GetAllAsync();
			return Ok(new SubCategoryDto {SubCategories= Subcategories });
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(SubCategory SubCategory)
		{
			if (ModelState.IsValid)
			{
				await _unitOfWork.SubCategory.AddAsync(SubCategory);
				await _unitOfWork.Save();
				return Ok(SubCategory);
			}
			return RedirectToAction("There is an Error while Deleting");
		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(SubCategory SubCategory)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.SubCategory.Update(SubCategory);
				await _unitOfWork.Save();
				return Ok(SubCategory);
			}
            return RedirectToAction("There is an Error while Deleting");
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(SubCategory SubCategory)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.SubCategory.Delete(SubCategory);
				await _unitOfWork.Save();
				return Ok("SubCategory Has Deleted");
			}
            return RedirectToAction("There is an Error while Deleting");
        }
	}
}