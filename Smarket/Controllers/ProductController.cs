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
            if (!ModelState.IsValid)
            {
                viewModel.SubCategories = await _unitOfWork.SubCategory.GetAllAsync();
                viewModel.Brands = await _unitOfWork.Brand.GetAllAsync();
                return Ok(viewModel);
            }

            Product product = new()
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                Price = viewModel.Price,
                BrandId = (int)viewModel.BrandId,
                SubCategoryId = (int)viewModel.SubCategoryId,
                ImageId = (int)viewModel.ImageId
            };

            await _unitOfWork.Product.AddAsync(product);
            await _unitOfWork.Save();
            return Ok();
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            var obj = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id);
            if (obj == null)
            {
                return Ok();
            }
            _unitOfWork.Product.Delete(obj);
            await _unitOfWork.Save();
            return Ok();
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(u => u.Id == id);
            if (product == null)
                return NotFound();

            ProductVM viewModel = new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                BrandId = product.BrandId,
                SubCategoryId = product.SubCategoryId,
                Price = product.Price,
                Description = product.Description,
                ImageId = product.ImageId,
                SubCategories = await _unitOfWork.SubCategory.GetAllAsync(),
                Brands = (await _unitOfWork.Brand.GetAllAsync())
            };

            return Ok(viewModel);
        }

        [HttpGet("Details")]
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
                Price = product.Price,
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


    }
}
