using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using System.Text;
using Debug;

namespace SocketServer.Handlers
{
	public class SocketEchoHandler : SocketServerHandler
	{

		private static ILog log = Debug.Log.Create(typeof(SocketEchoHandler));
	
		protected override bool ReadCallback(ISocketClient client, int bytesRead) {
			var ret = true;

			string str = Encoding.ASCII.GetString (client.Buffer, 0, bytesRead);

			if (str.Trim () == "quit") {
				ret = false;
			} else {
				this.Send (client, client.Buffer, bytesRead);
			}

			return ret;
		}
		
		protected override void SendCallback(ISocketClient client, byte[] data, int bytesSend) {
			log.Info("Sent {0} bytes to client: {1}",bytesSend, client);
		}
	}
}

