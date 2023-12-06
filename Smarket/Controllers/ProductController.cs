using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.ViewModels;
using Smarket.Services.IServices;

namespace Smarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        public ProductController(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Include SubCategory and Brand
                var products = await _unitOfWork.Product.GetAllAsync(null, i => i.Image);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] ProductDto dto)
        {

            var image = await _imageService.AddPhotoAsync(dto.formFile);

            var cloudimage = new Image
            {
                PublicId = image.PublicId,
                Url = image.Url.ToString(),
            };

            if (ModelState.IsValid)
            {
                Product product = new()
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    BrandId = dto.BrandId,
                    SubCategoryId = dto.SubCategoryId,
                    Image = cloudimage
                };

                await _unitOfWork.Product.AddAsync(product);
                await _unitOfWork.Save();
                return Ok(new
                {
                    Name=dto.Name,
                    Description=dto.Description,
                    Image = cloudimage.Url.ToString(),
                });
            }
            else
                return NotFound();
        }


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id, i => i.Image);
            if (obj == null)
                return NotFound();
            else
            {
                _unitOfWork.Product.Delete(obj);
                await _unitOfWork.Save();
                return Ok();
            }
        }
        // You should here make include, not GetAllAsync subcategories and brands
        [HttpGet("GetoneProduct/{id}")]
        public async Task<IActionResult> GetoneProduct(int id)
        {
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id, i => i.Image);
            var package = await _unitOfWork.Package.FirstOrDefaultAsync(p => p.ProductId == product.Id);

            if (product == null)
                return NotFound();
            else
            {
                return Ok(new
                {
                    Name=product.Name,
                    Description=product.Description,
                    Image=product.Image.Url.ToString(),
                    Reviews=product.Reviews,
                });
            }
        }
        // you making routing with id and paramter is Product?,first search for product then update
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(int id, ProductDto obj)
        {
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id, i => i.Image);
            await _imageService.DeletePhotoAsync(product.Image.PublicId);

            var image = await _imageService.AddPhotoAsync(obj.formFile);


            product.Name = obj.Name;
            product.BrandId = obj.BrandId;
            product.SubCategoryId = obj.SubCategoryId;
            product.Description = obj.Description;
            product.Image.PublicId = image.PublicId;
            product.Image.Url = image.Url.ToString();

            _unitOfWork.Product.Update(product);
            await _unitOfWork.Save();
            return Ok(obj);

        }

        // to get price for product, search for package to get price from it.
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var obj = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id, i => i.Image);
            if (obj == null)
                return NotFound();
            else
                return Ok(obj);

        }


        [HttpGet("GetBySubCategory")]
        public async Task<IActionResult> GetBySubCategory(int subCategoryId)
        {
            var products = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.SubCategoryId == subCategoryId, i => i.Image);
            return Ok(products);
        }

        [HttpGet("GetByBrand")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            var products = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.BrandId == brandId, i => i.Image);
            return Ok(products);
        }

    }
}
