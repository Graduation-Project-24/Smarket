using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.Dtos;
using Smarket.Models.DTOs;

namespace Smarket.Controllers
{
    public class FavoriteController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<FavoriteController> _logger;
        public FavoriteController(IUnitOfWork unitOfWork, UserManager<User> userManager,
        ILogger<FavoriteController> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("GetFavorites")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetFavorites()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var favorites = await _unitOfWork.UserFav.GetAllAsync(f => f.UserId == user.Id, c=>c.Product.Image,c=>c.Product.Reviews);

                var productIds = favorites.Select(f => f.ProductId).ToList();

                var packages = await _unitOfWork.Package.GetAllAsync(p => productIds.Contains(p.ProductId));

                if (favorites == null)
                    return NotFound();

                var favoriteDto = favorites.Select(c => new FavoriteDto
                {
                    ProductName = c.Product.Name,
                    ImageUrl =c.Product.Image.Url.ToString(),
                    ProductId = c.ProductId,
                    Description = c.Product.Description,
                    Reviews = c.Product.Reviews.Select(p => new ReviewDtoRateOnly
                    {
                        Rate = p.Rate,
                    }).ToList(),
                    Price = packages.FirstOrDefault(p => p.ProductId == c.ProductId)?.Price ?? 0

                });
                return Ok(favoriteDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving favorites");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddFavorite(FavoriteDtoforId obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                var product = await _unitOfWork.Product.FirstOrDefaultAsync(c => c.Id == obj.ProductId);
                var favitem = await _unitOfWork.UserFav.GetAllAsync(c => c.UserId == user.Id);

                if (favitem != null)
                {
                    foreach (var item in favitem)
                    {
                        if (item.ProductId == product.Id)
                        {
                            return Ok(new { Message = "This Product already in Favourite list" });
                        }
                    }
                }

                UserFav fav = new UserFav()
                {
                    ProductId = obj.ProductId,
                    UserId = user.Id,
                };

                await _unitOfWork.UserFav.AddAsync(fav);
                await _unitOfWork.Save();
                return Ok(new { Message = "Favourite added to favourite list successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("Delete")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteFavorite(FavoriteDtoforId obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var favorite = await _unitOfWork.UserFav.FirstOrDefaultAsync(f => f.ProductId == obj.ProductId && f.UserId == user.Id);
                if (favorite == null)
                {
                    return NotFound("Favorite not found for the specified user and product");
                }

                _unitOfWork.UserFav.Delete(favorite);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting favorite");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("CheckFavorite")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CheckFavorite(int productId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var favorite = await _unitOfWork.UserFav.FirstOrDefaultAsync(f => f.ProductId == productId && f.UserId == user.Id);

                return Ok(favorite != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking favorite");
                return StatusCode(500, "Internal server error");
            }
        }
    

}
}
