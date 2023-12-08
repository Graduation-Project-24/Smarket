using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.Dtos;
using Smarket.Models.DTOs;


namespace Smarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public ReviewController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpPost("AddReview")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddReview(ReviewDto obj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(c => c.Id == obj.ProductId);
            if (user == null)
            {
                return NotFound("User not found");
            }
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

    }
}
