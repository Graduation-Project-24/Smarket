using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.Dtos;
using Smarket.Models.DTOs;
using Stripe;



namespace Smarket.Controllers
{

    public class ReviewController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ReviewController> _logger;
        public ReviewController(IUnitOfWork unitOfWork, UserManager<User> userManager,
        ILogger<ReviewController> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var reviews = await _unitOfWork.UserReview.GetAllAsync(null,p=>p.Product,u=>u.User);

                var reviewDtos = reviews.Select(p => new ReviewDto
                {
                    ProductId = p.ProductId,
                    Rate = p.Rate,
                    Comment = p.Comment,
                    UserId = p.UserId
                });
                return Ok(reviewDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews");
                return StatusCode(500, "Internal server error");
            }
        }
        // Smarket\Controllers\ReviewController.cs

        [HttpPost("Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDto obj)
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

                UserReview review = new UserReview()
                {
                    ProductId = obj.ProductId,
                    Rate = obj.Rate,
                    Comment = obj.Comment,
                    UserId = user.Id
                };

                await _unitOfWork.UserReview.AddAsync(review);
                await _unitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
