using System;
using System.Linq;
using System.Linq.Expressions;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories.Extensions
{
	public static class CategoryRepositoryExtension
	{
		public static IQueryable<Category> EagerWhere(this DbSet<Category> dbSet, Expression<Func<Category, bool>> expr)
		{
			return dbSet
				.Include(m => m.ParentCategory)
				.Where(expr);
		}

		public static IQueryable<Category> PagingWithParentCategory(this DbSet<Category> dbSet, int pageSize = 0, int pageNumber = 0)
		{
			var query = dbSet
					.Include(c => c.ParentCategory).AsQueryable();

			return pageSize > 0 && pageNumber > 0 ? query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize) : query;
		}
	}
}
