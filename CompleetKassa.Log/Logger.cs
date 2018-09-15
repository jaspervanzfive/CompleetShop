using System;
using System.Diagnostics;
using System.Text;
using CompleetKassa.Log.Core;
using NLog;

namespace CompleetKassa.Log
{
	public class Logger : Core.ILogger
	{
		#region Constructor
		public Logger()
		{
		}

		public Logger(string internalLogFilePath)
		{
			NLog.Common.InternalLogger.LogFile = System.IO.Path.Combine(internalLogFilePath);
		}
		#endregion Constructor

		#region Events
		public event OnExceptionHandler OnException;
		public event OnErrorHandler OnError;
		#endregion Events

		#region Properties
		private NLog.Logger NLogger { get; } = LogManager.GetCurrentClassLogger();
		#endregion Properties

		#region Methods
		public void LogException(Exception exception, Func<Exception, string> exceptionToTextConverter = null, bool invokeOnException = true)
		{
			string exceptionText = exceptionToTextConverter != null ? exceptionToTextConverter(exception) : GetExceptionText(exception);

			NLogger.Fatal(exceptionText);

			if (invokeOnException == true)
			{
				OnException?.Invoke(exception, exceptionText);
			}
		}

		public void Error(string errorMessage, bool invokeOnError = true)
		{
			NLogger.Error(errorMessage);

			if (invokeOnError == true)
			{
				OnError?.Invoke(errorMessage);
			}
		}

		public void Trace(string message) => NLogger.Trace(message);

		public void Debug(string message) => NLogger.Debug(message);

		public void Warn(string message) => NLogger.Warn(message);

		public void Info(string message) => NLogger.Info(message);

		public void Fatal(string message) => NLogger.Fatal(message);

		private string GetExceptionText(Exception exception, bool innerException = false, string intend = null)
		{
			StringBuilder stringBuilder = new StringBuilder();

			intend = intend ?? string.Empty;

			if (innerException == true)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append($"{intend}InnerException:");
			}
			else
			{
				string systemType = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";

				stringBuilder.AppendLine();
				stringBuilder.Append($"OS Version: {Environment.OSVersion.VersionString}");
				stringBuilder.AppendLine();
				stringBuilder.Append($"System Type: {systemType}");
			}

			stringBuilder.AppendLine();
			stringBuilder.Append($"{intend}Source: {exception.Source}");
			stringBuilder.AppendLine();
			stringBuilder.Append($"{intend}Message: {exception.Message}");
			stringBuilder.AppendLine();
			stringBuilder.Append($"{intend}TargetSite: {exception.TargetSite}");
			stringBuilder.AppendLine();
			stringBuilder.Append($"{intend}Type: {exception.GetType()}");
			stringBuilder.AppendLine();

			if (!string.IsNullOrEmpty(exception.StackTrace))
			{
				StackTrace exceptionStackTrace = new StackTrace(exception, true);

				StackFrame[] frames = exceptionStackTrace.GetFrames();

				if (frames != null)
				{
					stringBuilder.Append($"{intend}StackTrace:");
					stringBuilder.AppendLine();

					foreach (StackFrame stackFrame in frames)
					{
						string newIntend = new string(' ', intend.Length + 4);

						string fileName = stackFrame.GetFileName();

						fileName = !string.IsNullOrEmpty(fileName)
							? fileName.Substring(fileName.LastIndexOf(@"\", StringComparison.InvariantCultureIgnoreCase) + 1)
							: string.Empty;

						stringBuilder.Append(
							$"{newIntend}File: {fileName} | Line: {stackFrame.GetFileLineNumber()} | Col: {stackFrame.GetFileColumnNumber()} | Offset: {stackFrame.GetILOffset()} | Method: {stackFrame.GetMethod()}");

						stringBuilder.AppendLine();
					}
				}
			}

			if (exception.InnerException != null)
			{
				stringBuilder.Append(GetExceptionText(exception.InnerException, innerException: true,
					intend: new string(' ', intend.Length + 4)));
			}
			else
			{
				stringBuilder.AppendLine();
			}

			return stringBuilder.ToString();
		}

		#endregion Methods
	}
}