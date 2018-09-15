using System;

namespace CompleetKassa.Log.Core
{
	public delegate void OnExceptionHandler(Exception exception, string exceptionText);

	public delegate void OnErrorHandler(string errorMessage);

	public interface ILogger
	{
		event OnExceptionHandler OnException;

		event OnErrorHandler OnError;
		void LogException(Exception exception, Func<Exception, string> exceptionToTextConverter = null, bool invokeOnException = true);
		void Error(string errorMessage, bool invokeOnError = true);
		void Trace(string message);
		void Debug(string message);
		void Warn(string message);
		void Info(string message);
		void Fatal(string message);
	}
}