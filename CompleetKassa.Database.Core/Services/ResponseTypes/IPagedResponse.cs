namespace CompleetKassa.Database.Core.Services.ResponseTypes
{
	public interface IPagedResponse<TModel> : IListResponse<TModel>
	{
		int ItemsCount { get; set; }

		int PageCount { get; }
	}
}
