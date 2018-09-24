using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Core.Exception;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Database.Entities;
using CompleetKassa.Database.Repositories;
using CompleetKassa.Database.Services.Extensions;
using CompleetKassa.Log.Core;
using CompleetKassa.Models;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Services
{
    public class ProductService : BaseService, IProductService
    {
        protected IProductRepository ProductRepository { get; }

        public ProductService(ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext)
            : base(logger, mapper, userInfo, dbContext)
        {
            ProductRepository = new ProductRepository(userInfo, DbContext);
        }

        public async Task<IListResponse<ProductModel>> GetProductsAsync(int pageSize = 0, int pageNumber = 0)
        {
            Logger?.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new ListResponse<ProductModel>();

            try
            {
                response.Model = await ProductRepository.GetAll(pageSize, pageNumber).Select(o => Mapper.Map<ProductModel>(o)).ToListAsync();
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<IListResponse<ProductModel>> GetProductsWithCategoryAsync(int pageSize = 0, int pageNumber = 0)
        {
            Logger?.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new ListResponse<ProductModel>();

            try
            {
                response.Model = await ProductRepository.GetAllWithCategory(pageSize, pageNumber).Select(o => Mapper.Map<ProductModel>(o)).ToListAsync();
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<ISingleResponse<ProductModel>> GetProductByIDAsync(int productID)
        {
            Logger?.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<ProductModel>();

            try
            {
                response.Model = Mapper.Map<ProductModel>(await ProductRepository.GetByIDAsync(productID));
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<ISingleResponse<ProductModel>> GetProductByIDWithCategoryAsync(int productID)
        {
            Logger?.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<ProductModel>();

            try
            {
                response.Model = Mapper.Map<ProductModel>(await ProductRepository.GetByIDWithCategoryAsync(productID));
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<ISingleResponse<ProductModel>> AddProductAsync(ProductModel details)
        {
            var response = new SingleResponse<ProductModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var product = Mapper.Map<Product>(details);

                    await ProductRepository.AddAsync(product);

                    transaction.Commit();

                    response.Model = Mapper.Map<ProductModel>(product);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.SetError(ex, Logger);
                }
            }

            return response;
        }


        public async Task<IListResponse<ProductModel>> AddProductsAsync(IEnumerable<ProductModel> details)
        {
            var response = new ListResponse<ProductModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    await ProductRepository.AddAsync(details.Select(o => Mapper.Map<Product>(o)).ToAsyncEnumerable());

                    transaction.Commit();

                    response.Model = details;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.SetError(ex, Logger);
                }
            }

            return response;
        }

        public async Task<ISingleResponse<ProductModel>> UpdateProductAsync(ProductModel updates)
        {
            Logger?.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<ProductModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    await ProductRepository.UpdateAsync(Mapper.Map<Product>(updates));

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

        public async Task<ISingleResponse<ProductModel>> RemoveProductAsync(int productID)
        {
            Logger?.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<ProductModel>();

            try
            {
                // Retrieve product by id
                Product product = await ProductRepository.GetByIDAsync(productID);
                if (product == null)
                {
                    throw new DatabaseException("Product record not found.");
                }

                await ProductRepository.DeleteAsync(product);
                response.Model = Mapper.Map<ProductModel>(product);
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }
    }
}
