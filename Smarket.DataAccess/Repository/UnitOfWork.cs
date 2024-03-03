
using RoutelaAPI.DataAccess.Repository;

namespace Smarket.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository Category { get; set; }

        public IUserRepository User { get; set; }

        public IBrandRepository Brand { get; set; }

        public ICartItemRepository CartItem { get; set; }

        public IImageRepository Image { get; set; }

        public IInventoryRepository Inventory { get; set; }

        public IOrderItemRepository OrderItem { get; set; }

        public IPackageRepository Package { get; set; }

        public IProductRepository Product { get; set; }

        public ISubCategoryRepository SubCategory { get; set; }

        public IUserReviewRepository UserReview { get; set; }
        public IOrderRepository Order { get; set; }
        public IUserFavRepository UserFav { get; set; }


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            Image= new ImageRepository(_context);
            User = new UserRepository(_context);
            Inventory = new InventoryRepository(_context);
            Brand = new BrandRepository(_context);
            CartItem = new CartItemRepository(_context);
            Order = new OrderRepository(_context);
            OrderItem = new OrderItemRepository(_context);
            Package = new PackageRepository(_context);
            Product = new ProductRepository(_context);
            SubCategory = new SubCategoryRepository(_context);
            UserReview = new UserReviewRepository(_context);
            UserFav = new UserFavRepository(_context);
        }



        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }
    }
}