namespace RoutelaAPI.DataAccess.Repository
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
