namespace Smarket.DataAccess.Repository.IRepository
{
	public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);

        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        T Update(T entity);

        IEnumerable<T> UpdateRange(IEnumerable<T> entities);

        T Delete(T entity);

        public IEnumerable<T> DeleteRange(IEnumerable<T> entities);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> GetAllNoTrackingAsync(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties);

        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);

        Task<bool> AnyAsync(Expression<Func<T, bool>> filter = null);
    }
}