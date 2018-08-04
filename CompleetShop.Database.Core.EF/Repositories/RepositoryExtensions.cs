﻿using System;
using System.Linq;
using CompleetShop.Database.Core.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CompleetShop.Database.Core.EF.Repositories
{
	public static class RepositoryExtensions
	{
		public static IQueryable<TEntity> Paging<TEntity> (this DbContext dbContext, Int32 pageSize = 0, Int32 pageNumber = 0) where TEntity : class, IEntity
		{
			var query = dbContext.Set<TEntity> ().AsQueryable ();

			return pageSize > 0 && pageNumber > 0 ? query
				//.OrderBy(p => p.ID) TODO: Need Order?
				.Skip ((pageNumber - 1) * pageSize)
				.Take (pageSize) : query;
		}

		public static IQueryable<TModel> Paging<TModel> (this IQueryable<TModel> query, Int32 pageSize = 0, Int32 pageNumber = 0) where TModel : class
			=> pageSize > 0 && pageNumber > 0 ? query.Skip ((pageNumber - 1) * pageSize).Take (pageSize) : query;
	}
}
