using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.EF.Extensions;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Core.Exception;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Database.Entities;
using CompleetKassa.Database.Repositories;
using CompleetKassa.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompleetKassa.Database.Services
{
	public class CategoryService : BaseService, ICategoryService
	{
		protected ICategoryRepository _categoryRepository;
		protected ICategoryRepository CategoryRepository => _categoryRepository ?? (_categoryRepository = new CategoryRepository(UserInfo, DbContext));

		public CategoryService(ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext) 
			: base(logger, mapper, userInfo, dbContext)
		{
		}
		public async Task<IListResponse<CategoryModel>> GetCategoriesAsync(int pageSize = 0, int pageNumber = 0)
		{
			Logger?.LogInformation(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

			var response = new ListResponse<CategoryModel>();

			try
			{
				response.Model = await CategoryRepository.GetAll(pageSize, pageNumber).Select(o => Mapper.Map<CategoryModel>(o)).ToListAsync();
			}
			catch (Exception ex)
			{
				response.SetError(ex, Logger);
			}

			return response;
		}

		public async Task<ISingleResponse<CategoryModel>> GetCategoryByIDAsync(int CategoryID)
		{
			Logger?.LogInformation(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

			var response = new SingleResponse<CategoryModel>();

			try
			{
				response.Model = Mapper.Map<CategoryModel>(await CategoryRepository.GetByIDAsync(CategoryID));
			}
			catch (Exception ex)
			{
				response.SetError(ex, Logger);
			}

			return response;
		}

		public async Task<ISingleResponse<CategoryModel>> AddCategoryAsync(CategoryModel details)
		{
			var response = new SingleResponse<CategoryModel>();

			using (var transaction = DbContext.Database.BeginTransaction())
			{
				try
				{
					var Category = Mapper.Map<Category>(details);;

					await CategoryRepository.AddAsync(Category);

					transaction.Commit();

					response.Model = Mapper.Map<CategoryModel>(Category);
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					throw ex;
				}
			}

			return response;
		}


		public async Task<IListResponse<CategoryModel>> AddCategoriesAsync(IEnumerable<CategoryModel> details)
		{
			var response = new ListResponse<CategoryModel>();

			using (var transaction = DbContext.Database.BeginTransaction())
			{
				try
				{
					await CategoryRepository.AddAsync(details.Select(o => Mapper.Map<Category>(o)).ToAsyncEnumerable());

					transaction.Commit();

					response.Model = details;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					throw ex;
				}
			}

			return response;
		}

		public async Task<ISingleResponse<CategoryModel>> UpdateCategoryAsync(CategoryModel updates)
		{
			Logger?.LogInformation(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

			var response = new SingleResponse<CategoryModel>();

			using (var transaction = DbContext.Database.BeginTransaction())
			{
				try
				{
					await CategoryRepository.UpdateAsync(Mapper.Map<Category>(updates));

					transaction.Commit();
					response.Model = updates;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					response.SetError(ex, Logger);
				}
			}

			return response;
		}

		public async Task<ISingleResponse<CategoryModel>> RemoveCategoryAsync(int CategoryID)
		{
			Logger?.LogInformation(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

			var response = new SingleResponse<CategoryModel>();

			try
			{
				// Retrieve Category by id
				Category Category = await CategoryRepository.GetByIDAsync(CategoryID);
				if (Category == null)
				{
					throw new DatabaseException("Category record not found.");
				}

				await CategoryRepository.DeleteAsync(Category);
				response.Model = Mapper.Map<CategoryModel>(Category);
			}
			catch (Exception ex)
			{
				response.SetError(ex, Logger);
			}

			return response;
		}
	}
}
