namespace Smarket.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IUserRepository User { get; }
        IBrandRepository Brand { get; }
        ICartItemRepository CartItem { get; }
        IImageRepository Image { get; }
        IInventoryRepository Inventory { get; }
        IOrderItemRepository OrderItem { get; }
        IPackageRepository Package { get; }
        IProductRepository Product { get; }
        ISubCategoryRepository SubCategory { get; }
        IUserReviewRepository UserReview { get; }
        IOrderRepository Order { get; }
        IUserFavRepository UserFav { get; }
        Task<int> Save();
    }
}