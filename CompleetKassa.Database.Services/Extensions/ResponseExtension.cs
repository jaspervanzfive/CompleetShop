using CompleetKassa.Database.Core.Exception;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using Microsoft.Extensions.Logging;

namespace CompleetKassa.Database.Core.EF.Extensions
{
	internal static class ResponseExtension
	{
		public static void SetError (this IResponse response, System.Exception ex, ILogger logger)
		{
			response.DidError = true;

			var cast = ex as DatabaseException;

			if (cast == null) {
				logger?.LogCritical (ex.ToString ());
				response.ErrorMessage = "There was an internal error, please contact to technical support.";
			}
			else {
				logger?.LogError (ex.Message);
				response.ErrorMessage = ex.Message;
			}
		}
	}
}
