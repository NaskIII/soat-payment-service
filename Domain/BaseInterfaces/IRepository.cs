namespace Domain.BaseInterfaces
{
    public interface IRepository<T> : IReadonlyRepository<T> where T : class
    {
        Task BeginTransactionAsync();
        public Task CommitTransactionAsync();
        public Task RollbackAsync();
        public Task<T> AddAsync(T entity);
        void AddToSet(T entity);
        Task<bool> SaveChangesAsync();
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        bool TransactionOpened { get; }
    }
}
