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
                var products = await _unitOfWork.Product.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }        

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ProductVM viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.SubCategories = await _unitOfWork.SubCategory.GetAllAsync();
                viewModel.Brands = await _unitOfWork.Brand.GetAllAsync();

                Product product = new()
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    //Price = viewModel.Price,
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
        public async Task<IActionResult> Delete(int? id)
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

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id);
            if (product == null)
                return NotFound();
            else
            {
                ProductVM viewModel = new()
                {
                    Id = product.Id,
                    Name = product.Name,
                    BrandId = product.BrandId,
                    SubCategoryId = product.SubCategoryId,
                    //Price = product.Price,
                    Description = product.Description,
                    ImageId = product.ImageId,
                    SubCategories = await _unitOfWork.SubCategory.GetAllAsync(),
                    Brands = (await _unitOfWork.Brand.GetAllAsync())
                };
                return Ok(viewModel);
            }
        }
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(product);
                await _unitOfWork.Save();
                return Ok(product);
            }
            else
                return NotFound();
        }


        [HttpGet("Details/{productId}")]
        public async Task<IActionResult> Details(int productId)
        {
            Product product = await _unitOfWork.Product.FirstOrDefaultAsync(p => p.Id == productId);
            var RateList = await _unitOfWork.UserReview.GetAllAsync(a => a.ProductId == product.Id);
            ProductVM viewModel = new()
            {
                Id = product.Id,
                Name = product.Name,
                BrandId = product.BrandId,
                SubCategoryId = product.SubCategoryId,
                //Price = product.Price,
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
        public async Task<IActionResult> GetBySubCategory(int? subCategoryId)
        {
            var products = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.SubCategoryId == subCategoryId);
            return Ok(products);
        }

        [HttpGet("GetByBrand")]
        public async Task<IActionResult> GetByBrand(int? brandId)
        {
            var products = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.BrandId == brandId);
            return Ok(products);
        }        
        
    }
}
