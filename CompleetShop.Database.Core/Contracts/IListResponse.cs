using System.Collections.Generic;

namespace CompleetShop.Database.Core.Contracts
{
	public interface IListResponse<TModel> : IResponse
	{
		IEnumerable<TModel> Model { get; set; }
	}
}
