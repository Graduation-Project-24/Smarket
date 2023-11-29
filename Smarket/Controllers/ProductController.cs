using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.ViewModels;

namespace Smarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Include SubCategory and Brand
                var products = await _unitOfWork.Product.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        // No need for All SubCategory and Brand in Creating, and you should do include in get method
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ProductDto viewModel)
        {
            if (ModelState.IsValid)
            {
                Product product = new()
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    BrandId = viewModel.BrandId,
                    SubCategoryId = viewModel.SubCategoryId,
                    ImageId = viewModel.ImageId
                };

                await _unitOfWork.Product.AddAsync(product);
                await _unitOfWork.Save();
                return Ok(viewModel);
            }
            else
                return NotFound();
        }


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id);
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
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id);
            var package = await _unitOfWork.Package.FirstOrDefaultAsync(p => p.ProductId == product.Id);

            if (product == null)
                return NotFound();
            else
            {
                ProductDto viewModel = new()
                {
                    Name = product.Name,
                    BrandId = product.BrandId,
                    Price = package.ListPrice,
                    SubCategoryId = product.SubCategoryId,
                    Description = product.Description,
                    ImageId = product.ImageId,
                };
                return Ok(viewModel);
            }
        }
        // you making routing with id and paramter is Product?,first search for product then update
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(int id, ProductDto obj)
        {
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id);

            product.Name = obj.Name;
            product.BrandId = obj.BrandId;
            product.SubCategoryId = obj.SubCategoryId;
            product.Description = obj.Description;
            product.ImageId = obj.ImageId;

            _unitOfWork.Product.Update(product);
            await _unitOfWork.Save();
            return Ok(obj);

        }

        // to get price for product, search for package to get price from it.
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var obj = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id);
            if (obj == null)
                return NotFound();
            else
                return Ok(obj);

        }


        [HttpGet("GetBySubCategory")]
        public async Task<IActionResult> GetBySubCategory(int subCategoryId)
        {
            var products = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.SubCategoryId == subCategoryId);
            return Ok(products);
        }

        [HttpGet("GetByBrand")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            var products = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.BrandId == brandId);
            return Ok(products);
        }        
        
    }
}
