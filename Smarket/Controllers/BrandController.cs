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
		public async Task<IActionResult> GetAll()
		{
			var brands = await _unitOfWork.Brand.GetAllAsync();
			return Ok(brands);
		}
		[HttpGet("Details")]
		public async Task<IActionResult> Details(int id)
		{
			var brand = await _unitOfWork.Brand.FirstOrDefaultAsync(b => b.Id == id);
			return Ok(brand);
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(BrandDto dto)
		{
			var brand = new Brand
			{
				Name = dto.Name,
				ImageId = dto.ImageId,

			};
			if (ModelState.IsValid)
			{
				await _unitOfWork.Brand.AddAsync(brand);
				await _unitOfWork.Save();
				return Ok(brand);
			}
			return RedirectToAction("There is an Error while Deleting");
		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(int id, BrandDto dto)
		{
			if (ModelState.IsValid)
			{
				var oldbrand = await _unitOfWork.Brand.FirstOrDefaultAsync(x => x.Id == id);
				oldbrand.Name = dto.Name;
				oldbrand.ImageId = dto.ImageId;
				_unitOfWork.Brand.Update(oldbrand);
				await _unitOfWork.Save();
				return Ok(oldbrand);
			}
			return RedirectToAction("There is an Error while Deleting");
		}

		[HttpPost("Delete")]
		public async Task<IActionResult> Delete(int id)
		{
			if (ModelState.IsValid)
			{
				var brand = await _unitOfWork.Brand.FirstOrDefaultAsync(x => x.Id == id);
				_unitOfWork.Brand.Delete(brand);
				await _unitOfWork.Save();
				return Ok("Brand Has Deleted");
			}
			return RedirectToAction("There is an Error while Deleting");
		}
	}
}