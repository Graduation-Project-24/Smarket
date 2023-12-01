/*using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;

namespace Smarket.Controllers
{

	public class SubSubCategoryController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;

		public SubSubCategoryController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _unitOfWork.SubCategory.GetAllAsync();
            return Ok(categories);
        }
        [HttpGet("GetAllByCategory")]
        public async Task<IActionResult> GetAllByCategory(int categoryId)
        {
            var categories = await _unitOfWork.SubCategory.GetAllAsync(c=>c.CategoryId==categoryId);
            return Ok(categories);
        }
        [HttpGet("Details")]
        public async Task<IActionResult> Details(int id)
        {
            var SubCategory = await _unitOfWork.SubCategory.FirstOrDefaultAsync(b => b.Id == id);
            return Ok(SubCategory);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(SubCategoryDto dto)
        {
            var SubCategory = new SubCategory
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId,

            };
            if (ModelState.IsValid)
            {
                await _unitOfWork.SubCategory.AddAsync(SubCategory);
                await _unitOfWork.Save();
                return Ok(SubCategory);
            }
            return RedirectToAction("There is an Error while Deleting");
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(int id, SubCategoryDto dto)
        {
            if (ModelState.IsValid)
            {
                var oldSubCategory = await _unitOfWork.SubCategory.FirstOrDefaultAsync(x => x.Id == id);
                oldSubCategory.Name = dto.Name;
                oldSubCategory.CategoryId = dto.CategoryId;
                _unitOfWork.SubCategory.Update(oldSubCategory);
                await _unitOfWork.Save();
                return Ok(oldSubCategory);
            }
            return RedirectToAction("There is an Error while Deleting");
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var SubCategory = await _unitOfWork.SubCategory.FirstOrDefaultAsync(x => x.Id == id);
                _unitOfWork.SubCategory.Delete(SubCategory);
                await _unitOfWork.Save();
                return Ok("SubCategory Has Deleted");
            }
            return RedirectToAction("There is an Error while Deleting");
        }
    }
}*/