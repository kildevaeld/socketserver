using System;
using SocketServer;
using SocketServer.Handlers;
namespace Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var handler = new SocketChatHandler ();

			var server = new Server { Handler = handler }; 

			server.Listen ();

		}
	}
}
