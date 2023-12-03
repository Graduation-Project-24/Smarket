using Microsoft.AspNetCore.Mvc;
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

        public BrandController(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }
        [HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var brands = await _unitOfWork.Brand.GetAllAsync(null, i => i.Image);
			return Ok(brands);
		}
		[HttpGet("Details")]
		public async Task<IActionResult> Details(int id)
		{
			var brand = await _unitOfWork.Brand.FirstOrDefaultAsync(b => b.Id == id, i => i.Image);
			return Ok(brand);
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create([FromForm]BrandDto dto)
		{
            var image = await _imageService.AddPhotoAsync(dto.formFile);

            var cloudimage = new Image
            {
                PublicId = image.PublicId,
                Url = image.Url.ToString(),
            };
            var brand = new Brand
			{
				Name = dto.Name,
				Image = cloudimage

			};
			if (ModelState.IsValid)
			{
				await _unitOfWork.Brand.AddAsync(brand);
				await _unitOfWork.Save();
				return Ok(new
				{
                    Name = dto.Name,
                    Image = cloudimage.Url.ToString(),
                });
			}
			return RedirectToAction("There is an Error while Deleting");
		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(int id, BrandDto dto)
		{
			if (ModelState.IsValid)
			{
				var oldbrand = await _unitOfWork.Brand.FirstOrDefaultAsync(x => x.Id == id, i => i.Image);

                await _imageService.DeletePhotoAsync(oldbrand.Image.PublicId);

                var image = await _imageService.AddPhotoAsync(dto.formFile);

                oldbrand.Name = dto.Name;
                oldbrand.Image.PublicId = image.PublicId;
                oldbrand.Image.Url = image.Url.ToString();

                _unitOfWork.Brand.Update(oldbrand);
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
				var brand = await _unitOfWork.Brand.FirstOrDefaultAsync(x => x.Id == id, i => i.Image);
                await _imageService.DeletePhotoAsync(brand.Image.PublicId);
                _unitOfWork.Brand.Delete(brand);
				await _unitOfWork.Save();
				return Ok("Brand Has Deleted");
			}
			return RedirectToAction("There is an Error while Deleting");
		}
	}
}