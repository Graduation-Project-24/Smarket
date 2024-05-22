using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;
using Smarket.Services.IServices;

namespace Smarket.Controllers
{

	public class HomeController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        public HomeController(IUnitOfWork unitOfWork,ILogger<HomeController> logger)
		{
			_unitOfWork = unitOfWork;
            _logger = logger;
		}

        [HttpGet("MobileHome")]
        public async Task<ActionResult<IEnumerable<HomeDto>>> GetDataHome()
        {
            try
            {
                var categories = (await _unitOfWork.Category.GetAllAsync(null, c => c.Image)).Take(5).ToList();

                var categoriesDto = categories.Select(category => new CategoryDtoImageUrl
                {
                    Name = category.Name,
                    Image = new { Url = category.Image.Url }
                });

                var randomPackages = (await _unitOfWork.Package.GetAllAsync(null, p => p.Product.Image)).Take(10).ToList();

                Random random = new Random();
                List<int> randomIndices = new List<int>();

                while (randomIndices.Count < 10 && randomPackages.Count > 0)
                {
                    int randomIndex = random.Next(0, randomPackages.Count);
                    if (!randomIndices.Contains(randomIndex))
                    {
                        randomIndices.Add(randomIndex);
                    }
                }

                List<Package> tenProducts = randomIndices.Select(index => randomPackages[index]).ToList();


                var productDtos = tenProducts.Select(product => new FlatProductDto
                {
                    Id = product.Product.Id,
                    Name = product.Product.Name,
                    ImageUrl = product.Product.Image.Url,
                    Price = product.Price
                });

                var homeDto = new HomeDto
                {
                    Categories = categoriesDto,
                    Products = productDtos
                };


                return Ok(homeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Data");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}