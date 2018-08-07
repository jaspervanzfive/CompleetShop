namespace CompleetKassa.Database.Core.Services.ResponseTypes
{
	public interface IResponse
	{
		string Message { get; set; }

		bool DidError { get; set; }

		string ErrorMessage { get; set; }
	}
}
