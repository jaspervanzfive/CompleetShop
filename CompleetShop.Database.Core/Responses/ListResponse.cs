using System.Collections.Generic;
using CompleetShop.Database.Core.Contracts;

namespace CompleetShop.Database.Core.Responses
{
    public class ListResponse<TModel> : IListResponse<TModel>
    {
        public string Message { get; set; }

        public bool DidError { get; set; }

        public string ErrorMessage { get; set; }

        public IEnumerable<TModel> Model { get; set; }
    }
}
