using System;
using System.Text;
using System.Threading.Tasks;
namespace SocketServer.Handlers
{
	public class SocketChatHandler : SocketServerHandler
	{


		public override void Initialize(ISocketClient client) {

			var message = Encoding.ASCII.GetBytes ("New client connected!");

			var b = Encoding.ASCII.GetBytes ("Please enter your name: ");
			var task = client.
				SendAsync (b, b.Length);

			task.ContinueWith (x => {
				return client.ReadAsync().Result;

			}).ContinueWith( x => {
				client.Data = Encoding.ASCII.GetString(x.Result);

				var welcome_message = Encoding.ASCII.GetBytes("Welcome " + client.Data);

				return client.SendAsync(welcome_message,welcome_message.Length).Result;

			}).ContinueWith( r => {
				var welcome_message = Encoding.ASCII.GetBytes("Welcome " + client.Data);
				this.Broadcast(client,welcome_message).Wait();
				base.Initialize(client);
			});

		}

		protected override bool ReadCallback (ISocketClient client, int bytesRead)
		{
			var ret = true;

			string str = Encoding.ASCII.GetString (client.Buffer, 0, bytesRead);

			if (str.Trim () == "quit") {
				ret = false;
			} else {
				base.Initialize (client);
				this.Broadcast (client, client.Buffer, bytesRead);
			}

			return ret;
		}

		protected override void SendCallback (ISocketClient client, byte[] data, int bytesSend)
		{

		}
	}
}

