namespace CompleetShop.Database.Core.Contracts
{
	public interface ISingleResponse<TModel> : IResponse
	{
		TModel Model { get; set; }
	}
}
