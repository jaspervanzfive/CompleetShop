namespace CompleetShop.Database.Core.Contracts
{
	public interface IResponse
	{
		string Message { get; set; }

		bool DidError { get; set; }

		string ErrorMessage { get; set; }
	}
}
