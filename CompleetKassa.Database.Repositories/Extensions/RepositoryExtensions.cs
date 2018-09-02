using System;
using System.Linq;
using System.Linq.Expressions;
using CompleetKassa.Database.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{
    public static class RepositoryExtension
    {
        public static IQueryable<TEntity> EagerWhere<TEntity, TProperty>(this DbSet<TEntity> dbSet, Expression<Func<TEntity, TProperty>> navigationPropertyPath, Expression<Func<TEntity, bool>> expr) where TEntity : class, IEntity
        {
            return dbSet
                .Include(navigationPropertyPath)
                .Where(expr);
        }

        public static IQueryable<TEntity> Paging<TEntity>(this DbContext dbContext, Int32 pageSize = 0, Int32 pageNumber = 0) where TEntity : class, IEntity
        {
            var query = dbContext.Set<TEntity>().AsQueryable();

            return pageSize > 0 && pageNumber > 0 ? query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize) : query;
        }

        public static IQueryable<TModel> Paging<TModel>(this IQueryable<TModel> query, Int32 pageSize = 0, Int32 pageNumber = 0) where TModel : class
            => pageSize > 0 && pageNumber > 0 ? query.Skip((pageNumber - 1) * pageSize).Take(pageSize) : query;

        public static IQueryable<TEntity> Paging<TEntity, TProperty>(this DbSet<TEntity> dbSet, Expression<Func<TEntity, TProperty>> navigationPropertyPath, Int32 pageSize = 0, Int32 pageNumber = 0) where TEntity : class
        {
            var query = dbSet.Include(navigationPropertyPath).AsQueryable();

            return pageSize > 0 && pageNumber > 0 ? query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize) : query;
        }
    }
}
