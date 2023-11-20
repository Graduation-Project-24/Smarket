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
                var products = await _unitOfWork.Product.GetAllAsync(includeProperties: p => new string[] { "SubCategory", "Brand" });
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
            var obj = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id, includeProperties: p => new string[] { "SubCategory", "Brand" });
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
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id, includeProperties: p => new string[] { "SubCategory", "Brand" });
            var package = await _unitOfWork.Package.FirstOrDefaultAsync(p => p.ProductId == product.Id, includeProperties: p => new string[] { "Product", "Inventory" });

            if (product == null)
                return NotFound();
            else
            {
                ProductDto viewModel = new()
                {
                    Id = product.Id,
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
        public async Task<IActionResult> Edit(Product obj)
        {
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == obj.Id, includeProperties: p => new string[] { "SubCategory", "Brand" });
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                await _unitOfWork.Save();
                return Ok(obj);
            }
            else
                return NotFound();
        }

        // to get price for product, search for package to get price from it.
        [HttpGet("Details/{productId}")]
        public async Task<IActionResult> Details(int productId)
        {
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(p => p.Id == productId, includeProperties: p => new string[] { "SubCategory", "Brand" });
            var package = await _unitOfWork.Package.FirstOrDefaultAsync(p => p.ProductId == product.Id, includeProperties: p => new string[] { "Product", "Inventory" });
            var RateList = await _unitOfWork.UserReview.GetAllAsync(a => a.ProductId == product.Id);

            ProductDto viewModel = new()
            {
                Id = product.Id,
                Name = product.Name,
                BrandId = product.BrandId,
                SubCategoryId = product.SubCategoryId,
                Price = package.ListPrice,
                Description = product.Description,
                ImageId = product.ImageId,
                Brand = product.Brand,
                UserReviews = (await _unitOfWork.UserReview.GetAllAsync(a => a.ProductId == product.Id && a.Comment != null))
            };
            double temp = 0;
            foreach (var item in RateList)
            {
                temp += item.Rate;
            }
            viewModel.AvgRate = temp / RateList.Count();
            return Ok(viewModel);
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
