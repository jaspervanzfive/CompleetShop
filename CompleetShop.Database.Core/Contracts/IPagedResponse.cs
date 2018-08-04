namespace CompleetShop.Database.Core.Contracts
{
	public interface IPagedResponse<TModel> : IListResponse<TModel>
	{
		int ItemsCount { get; set; }

		int PageCount { get; }
	}
}
