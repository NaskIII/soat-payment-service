using Domain.BaseInterfaces;
using Infraestructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Infraestructure.BaseRepository
{
    public class ReadonlyRepository<T> : IReadonlyRepository<T> where T : class
    {
        protected readonly ApplicationDatabaseContext Context;
        protected readonly DbSet<T> DbSet;

        public ReadonlyRepository(ApplicationDatabaseContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        public IQueryable<T> AsQueryable()
        {
            return DbSet.AsQueryable();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> query, params string[] joins)
        {
            var dbQuery = joins.Aggregate(DbSet.AsQueryable(), (current, include) => current.Include(include));
            return await dbQuery.CountAsync(query);
        }

        public async Task<int> CountAsync()
        {
            return await DbSet.CountAsync();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> query)
        {
            return await DbSet.AnyAsync(query);
        }

        public async Task<bool> ExistsByIdAsync(params object[] ids)
        {
            return await DbSet.FindAsync(ids) != null;
        }

        public async Task<T?> GetByIdAsync(params object[] ids)
        {
            return await DbSet.FindAsync(ids);
        }

        public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> query)
        {
            return await DbSet.FirstOrDefaultAsync(query);
        }

        public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> query, params string[] joins)
        {
            var dbQuery = joins.Aggregate(DbSet.AsQueryable(), (current, include) => current.Include(include));
            return await dbQuery.FirstOrDefaultAsync(query);
        }

        public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> query, params Expression<Func<T, object>>[] includes)
        {
            var dbQuery = includes.Aggregate(DbSet.AsQueryable(), (current, include) => current.Include(include));
            return await dbQuery.FirstOrDefaultAsync(query);
        }

        public async Task<List<T>> GetManyAsync(Expression<Func<T, bool>> query)
        {
            return await DbSet.Where(query).ToListAsync();
        }

        public async Task<List<T>> GetManyAsync(Expression<Func<T, bool>> query, params string[] joins)
        {
            var dbQuery = joins.Aggregate(DbSet.AsQueryable(), (current, include) => current.Include(include));
            return await dbQuery.Where(query).ToListAsync();
        }

        public async Task<List<T>> GetManyAsync(Expression<Func<T, bool>> query, params Expression<Func<T, object>>[] includes)
        {
            var dbQuery = includes.Aggregate(DbSet.AsQueryable(), (current, include) => current.Include(include));
            return await dbQuery.Where(query).ToListAsync();
        }

        public async Task<List<T>> GetManyByIdAsync<TId>(List<TId> ids)
        {
            var entityType = Context.Model.FindEntityType(typeof(T));

            if (entityType == null)
                throw new InvalidOperationException($"Entidade {typeof(T).Name} não encontrada no modelo.");

            var keyProperty = entityType.FindPrimaryKey()?.Properties.FirstOrDefault();

            if (keyProperty == null)
                throw new InvalidOperationException($"Chave primária não encontrada para a entidade {typeof(T).Name}.");

            var keyName = keyProperty.Name;

            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, keyName);
            var containsMethod = typeof(List<TId>).GetMethod("Contains", new[] { typeof(TId) });
            var containsCall = Expression.Call(Expression.Constant(ids), containsMethod!, property);
            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

            return await DbSet.Where(lambda).ToListAsync();
        }

        public async Task<List<T>> AsNoTrackingListAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public async Task<bool> IsActiveAsync(params object[] ids)
        {
            var entity = await DbSet.FindAsync(ids);

            if (entity == null) return false;

            var method = typeof(T).GetMethod("IsActive", BindingFlags.Public | BindingFlags.Instance);

            if (method == null)
                throw new InvalidOperationException($"O método IsActive não foi encontrado em {typeof(T).Name}");

            var result = method.Invoke(entity, null);
            return result is bool boolResult && boolResult;
        }

        public async Task<List<T>> GetPagedDataAsync(
            Expression<Func<T, bool>> query,
            int pageNumber,
            int pageSize,
            string orderByProperty,
            bool orderByAscending,
            params string[] joins)
        {
            var dbQuery = joins.Aggregate(DbSet.AsQueryable(), (current, include) => current.Include(include));

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, orderByProperty);
            var lambda = Expression.Lambda(property, parameter);

            var orderedQuery = orderByAscending
                ? Queryable.OrderBy(dbQuery, (dynamic)lambda)
                : Queryable.OrderByDescending(dbQuery, (dynamic)lambda);

            return await orderedQuery
                .Where(query)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

    }
}
