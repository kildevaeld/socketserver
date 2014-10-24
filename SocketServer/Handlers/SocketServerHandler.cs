using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Linq;

using Debug;

namespace SocketServer
{

	public abstract class SocketServerHandler : ISocketServerHandler, IDisposable
	{

		protected static readonly ILog log = Log.Create(typeof(SocketServerHandler));

		protected object _lock;
		protected List<ISocketClient> _clients;

		public SocketServerHandler ()
		{
			_clients = new List<ISocketClient> ();
			_lock = new object ();
		}

		public virtual void Initialize(ISocketClient client) {

			AddClient (client);
			client.Closed += (object sender, SocketEventArgs e) => {
				ISocketClient _client = (ISocketClient)sender;
				Console.WriteLine("Client disconnected: {0}", _client.ToString());
				RemoveClient(_client);

			};

			this.Read (client);
		} 



		public Task Read(ISocketClient client) {

			return Task.Factory.FromAsync<int> (

				client.Socket.BeginReceive(client.Buffer,0,SocketClient.BufferSize, SocketFlags.None, null, this),
				client.Socket.EndReceive

			).ContinueWith (x => {

				int bytesRead = x.Result;

				if (bytesRead > 0) {
					bool ret = ReadCallback (client, bytesRead);
					if (ret) {
						this.Read (client);
					} else {

						client.Close();
					}
				// If bytesRead is zero, the socket i probbaly disconnected.
				} else {
					if (client.Socket.Connected)
						return;
					this.RemoveClient(client);
					log.Debug ("Socket is disconnected");
				}

			});
				
		}

		public  Task<bool> Send(ISocketClient client, byte[] data) {
			return Send (client, data, data.Length);
		}

		public  Task<bool> Send(ISocketClient client, byte[] data, int datalen) {
		
			var task = Task.Factory.FromAsync<int> (
				client.Socket.BeginSend(data,0,datalen,SocketFlags.None, null, this),
				client.Socket.EndSend
			);

			return task.ContinueWith (x => {
				if (x.Result != datalen) {
					SendErrorCallback(client);
					return false;
				}
						
				SendCallback(client, data, datalen);
				return true;
			});
		}

		public Task Broadcast (ISocketClient client, byte[] data) {
			return this.Broadcast (client, data, data.Length);
		}

		public Task Broadcast (ISocketClient client, byte[] data, int datalen) {
			var queue = new List<Task<bool>> ();
			this.IterateClients (x => {
				if (x.UUID != client.UUID) {
					queue.Add(this.Send(x, data, datalen));
				}
			});

			return Task.WhenAll (queue);
		}

		public void Close(ISocketClient client) {
			this.RemoveClient (client);
			client.Socket.Shutdown (SocketShutdown.Both);
			client.Socket.Close (5);
		}


		/// <summary>
		/// Reads the callback.
		/// </summary>
		/// <returns><c>true</c>, if callback was  read, <c>false</c> otherwise.</returns>
		/// <param name="state">State.</param>
		/// <param name="bytesRead">Bytes read.</param>
		protected abstract bool ReadCallback (ISocketClient client, int bytesRead);

		/// <summary>
		/// Sends the callback.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="data">Data.</param>
		/// <param name="bytesSend">Bytes send.</param>
		protected abstract void SendCallback (ISocketClient client, byte[] data, int bytesSend);

		protected virtual void SendErrorCallback(ISocketClient client) {
			Console.WriteLine ("Go line");
		}

		protected void AddClient (ISocketClient client) {
			lock (_lock) {
				_clients.Add (
					client);
			}
		}

		protected void RemoveClient (ISocketClient client) {
			lock (_lock) {
				_clients.Remove (client);
			}
		}

		protected void IterateClients(Action<ISocketClient> factory) {
			lock (_lock) {
				foreach (var client in _clients) {
					factory (client);
				}
			}
		}

		public void Dispose() {
			this.IterateClients (x => {
				this.Close(x);
			});
		}
			
	}
}

