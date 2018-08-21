using System;
using System.Linq;
using System.Linq.Expressions;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories.Extensions
{
	public static class ProductRepositoryExtension
	{
		public static IQueryable<Product> EagerWhere(this DbSet<Product> dbSet, Expression<Func<Product, bool>> expr)
		{
			return dbSet
				.Include(m => m.Category)
					.ThenInclude(c => c.ParentCategory)
				.Where(expr);
		}

		public static IQueryable<Product> PagingWithCategory(this DbSet<Product> dbSet, int pageSize = 0, int pageNumber = 0)
		{
			var query = dbSet
				.Include(m => m.Category)
					.ThenInclude(c => c.ParentCategory).AsQueryable();

			return pageSize > 0 && pageNumber > 0 ? query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize) : query;
		}
	}
}
