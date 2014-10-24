using System;
using SocketServer;
using SocketServer.Handlers;
using Debug;
namespace Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Debug.Log.Configuation.LogLevels = LogLevels.Info | LogLevels.Error;
			Debug.Log.Configuation.AddTransport<ConsoleTransport> ();

			Log.Configuation.Enable ("*");
			var handler = new SocketChatHandler ();

			var server = new Server { Handler = handler }; 

			server.Listen ();

		}
	}
}
