namespace RoutelaAPI.DataAccess.Repository
{
    public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public SubCategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
