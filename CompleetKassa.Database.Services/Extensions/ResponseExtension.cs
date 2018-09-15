using CompleetKassa.Database.Core.Exception;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Log.Core;

namespace CompleetKassa.Database.Services.Extensions
{
	internal static class ResponseExtension
	{
		public static void SetError (this IResponse response, System.Exception ex, ILogger logger)
		{
			response.DidError = true;

			var cast = ex as DatabaseException;

			if (cast == null) {
				logger?.Fatal (ex.ToString ());
				response.ErrorMessage = "There was an internal error, please contact to technical support.";
			}
			else {
				logger?.Error (ex.Message);
				response.ErrorMessage = ex.Message;
			}
		}
	}
}
