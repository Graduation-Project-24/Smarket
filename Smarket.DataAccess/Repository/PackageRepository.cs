namespace RoutelaAPI.DataAccess.Repository
{
    public class PackageRepository : Repository<Package>, IPackageRepository
    {
        private readonly ApplicationDbContext _context;

        public PackageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
