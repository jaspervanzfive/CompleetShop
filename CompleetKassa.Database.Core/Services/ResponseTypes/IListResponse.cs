using System.Collections.Generic;

namespace CompleetKassa.Database.Core.Services.ResponseTypes
{
	public interface IListResponse<TModel> : IResponse
	{
		IEnumerable<TModel> Model { get; set; }
	}
}
