using System;

namespace SocketServer.Logging
{
	public class Logger : ILog
	{
		public Logger ()
		{

		}

		public bool IsDebugEnabled { get;  set; }
		public bool IsInfoEnabled { get;  set; }
		public bool IsWarnEnabled { get;  set; }
		public bool IsErrorEnabled { get;  set; }
		public bool IsFatalEnabled { get;  set; }

		public void Debug(object message) {
			this.Log (LogLevels.Debug,message, null);
		}
		public void DebugFormat(string message, params object[] messages) {
			this.Log (LogLevels.Debug,message, messages);
		}
		public void Info (object message) {
			this.Log (LogLevels.Info,message, null);
		}
		public void InfoFormat (string message, params object[] messages) {
			this.Log (LogLevels.Info,message, messages);
		}
		public void Warn (object message) {
			this.Log (LogLevels.Warn,message, null);
		}
		public void WarnFormat (string message, params object[] messages) {
			this.Log (LogLevels.Warn,message, messages);
		}
		public void Error (object message) {
			this.Log (LogLevels.Error,message, null);
		}
		public void ErrorFormat (string message, params object[] messages) {
			this.Log (LogLevels.Error,message, messages);
		}
		public void Fatal (object message) {
			this.Log (LogLevels.Fatal,message, null);
		}
		public void FatalFormat (string message, params object[] messages) {
			this.Log (LogLevels.Fatal,message, messages);
		}

		protected void Log (LogLevels level, object message, object[] messages) {
			if (ShouldLog(level))
				Console.WriteLine (level + " : " + message, messages);
		}

		protected bool ShouldLog(LogLevels level) {
			switch (level) {
			case LogLevels.Debug:
				return this.IsDebugEnabled;	
			case LogLevels.Info:
				return this.IsInfoEnabled;
			case LogLevels.Warn:
				return this.IsWarnEnabled;
			case LogLevels.Error:
				return this.IsErrorEnabled;
			case LogLevels.Fatal:
				return this.IsFatalEnabled;
			default:
				return false;
			}
		}

	}
}

