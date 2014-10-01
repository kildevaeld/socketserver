using System;

namespace SocketServer
{

	public enum LogLevels
	{
		Info,
		Debug,
		Warn,
		Error,
		Fatal
	}

	public interface ILog
	{
		bool IsDebugEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsWarnEnabled { get; }
		bool IsErrorEnabled { get; }
		bool IsFatalEnabled { get; }

		void Debug (object message);
		void DebugFormat (string message, params object[] messages);
		void Info (object message);
		void InfoFormat (string message, params object[] messages);
		void Warn (object message);
		void WarnFormat (string message, params object[] messages);
		void Error (object message);
		void ErrorFormat (string message, params object[] messages);
		void Fatal (object message);
		void FatalFormat (string message, params object[] messages);
	}
}

