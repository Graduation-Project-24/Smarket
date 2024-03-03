namespace RoutelaAPI.DataAccess.Repository
{
    public class UserFavRepository : Repository<UserFav>, IUserFavRepository
    {
        private readonly ApplicationDbContext _context;

        public UserFavRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
