using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;

namespace Smarket.Controllers
{

	public class BrandController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;

		public BrandController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var brands =await _unitOfWork.Brand.GetAllAsync();
			return Ok(new BrandDto {Brands=brands});
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(Brand Brand)
		{
			if (ModelState.IsValid)
			{
				await _unitOfWork.Brand.AddAsync(Brand);
				await _unitOfWork.Save();
				return Ok(Brand);
			}
			return RedirectToAction("There is an Error while Deleting");
		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(Brand Brand)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.Brand.Update(Brand);
				await _unitOfWork.Save();
				return Ok(Brand);
			}
            return RedirectToAction("There is an Error while Deleting");
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(Brand Brand)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.Brand.Delete(Brand);
				await _unitOfWork.Save();
				return Ok("Brand Has Deleted");
			}
            return RedirectToAction("There is an Error while Deleting");
        }
	}
}