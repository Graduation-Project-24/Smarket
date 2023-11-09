namespace RoutelaAPI.DataAccess.Repository
{
    public class ImageRepository : Repository<Image>, IImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ImageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
