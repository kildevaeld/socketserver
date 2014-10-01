using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Linq;

namespace SocketServer
{
	public class SocketEventArgs : EventArgs {
		public ISocketClient Client { get; set; }

		public SocketEventArgs (ISocketClient client) { Client = client; }

	}

	public class SocketClient : IAsyncSocketClient
	{

		public event EventHandler<SocketEventArgs> Closed;

		public const int BufferSize = 1024;

		public Socket Socket { get; protected set; }

		public byte[] Buffer { get; set; }

		public Guid UUID { get; protected set; }

		public object Data { get; set; }

		public SocketClient (Socket socket) {
			UUID = Guid.NewGuid ();
			Socket = socket;
			Buffer = new byte[SocketClient.BufferSize];
		}

		public void Close () {
			if (Closed != null) {
				var ev = new SocketEventArgs(this);
				Closed (this, ev);
			}

			try {
				Socket.Shutdown(SocketShutdown.Send);
			} catch { }

			Socket.Close (5);

		}
			
		public async Task<byte[]> ReadAsync () {
			var task = Task.Factory.FromAsync<int> (
				Socket.BeginReceive(Buffer,0,SocketClient.BufferSize, SocketFlags.None, null, this),
				Socket.EndReceive
			);
			task.Start();
			int bytesRead = await task;
			byte[] buffer = Buffer;
			Array.Resize (ref buffer, bytesRead);
			return buffer;
		}

		public async Task<int> SendAsync (byte[] data, int len) {
			var task = Task.Factory.FromAsync<int> (
				Socket.BeginSend(data,0,len,SocketFlags.None, null, this),
				Socket.EndSend
			);
			task.Start ();
			int bytesSend = await task;

			return bytesSend;
		}

		public int Send(byte[] data, int len) {
			int ret = Socket.Send (data);
			Server.Log.DebugFormat("Sent {0} bytes to client: {1}",ret,this);
			return ret;
		}

		public int Read() {
			return Socket.Receive (Buffer);
		}

		public NetworkStream GetStream(bool own = false) {
			return new NetworkStream (Socket, own);
		}

		public override string ToString ()
		{
			string soc;
			if (Socket.Connected)
				soc = Socket.RemoteEndPoint.ToString ();
			else
				soc = "Disconnected";
			return string.Format ("[SocketClient: Socket={0}, UUID={1}]", soc, UUID);
		}



	}
}

