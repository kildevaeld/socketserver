using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using System.Text;

namespace SocketServer.Handlers
{
	public class SocketEchoHandler : SocketServerHandler
	{

	
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
			log.InfoFormat ("Sent {0} bytes to client: {1}",bytesSend, client);
		}
	}
}

