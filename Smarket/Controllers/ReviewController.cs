using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.Dtos;


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
        public async Task<IActionResult> AddReview(ReviewDto obj)
        {
            UserReview review = new UserReview()
            {
                ProductId = obj.Id,
                Rate = obj.Rate,
                Comment = obj.Comment,
                UserId = (await _userManager.GetUserAsync(User)).Id
            };
            await _unitOfWork.UserReview.AddAsync(review);
            await _unitOfWork.Save();
            return Ok();
        }

    }
}
