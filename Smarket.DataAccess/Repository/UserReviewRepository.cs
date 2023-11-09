namespace RoutelaAPI.DataAccess.Repository
{
    public class UserReviewRepository : Repository<UserReview>, IUserReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public UserReviewRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
