using System;
using System.Threading.Tasks;

namespace SocketServer
{
	public interface ISocketServerHandler
	{
		void Initialize(ISocketClient client);
		//Task<bool> Send(ISocketClient client, byte[] data, int datalen);
		//Task Read(ISocketClient client);

	
	}
}

