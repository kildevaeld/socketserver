using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketServer
{
	public interface ISocketClient
	{
		event EventHandler<SocketEventArgs> Closed;

		byte[] Buffer { get; set; }
		Guid UUID { get; }
		Socket Socket { get; }
		object Data { get; set; }

		void Close();
		Task<int> SendAsync (byte[] data, int len);
		Task<byte[]> ReadAsync();
		int Send(byte[] data, int len);
		int Read();
		NetworkStream GetStream (bool own);
	}

	public interface IAsyncSocketClient : ISocketClient {

	}
}

