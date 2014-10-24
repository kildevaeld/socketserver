using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Debug;

namespace SocketServer {

	public class Server {

		private static ILog _logger = Debug.Log.Create (typeof(Server));
		public static ILog Log { get { return _logger; } }
		public static void SetLogger(ILog logger) {
			_logger = logger;
		}
			
		// Thread signal.
		public static ManualResetEvent allDone = new ManualResetEvent(false);

		public ISocketServerHandler Handler { get; set; }

		private Socket _server;


		public Server(AddressFamily address, SocketType stype, ProtocolType ptype) : base() {
			_server = new Socket(address,stype,ptype);
		}

		public Server () {
			_server = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp );
		}

		public void Listen(int port=3000) {

			IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Loopback, port);
			this.Listen (localEndPoint, port);
		}

		public void Listen (IPEndPoint endPoint, int port) {

			// Bind the socket to the endpoint and listen for incoming connections.
			try {
				_server.Bind(endPoint);
				_server.Listen(100);

				Log.Info ("Listening on port: {0}", endPoint);

				while (true) {
					// Set the event to nonsignaled state.
					allDone.Reset();

					// Start an asynchronous socket to listen for connections.
					_server.BeginAccept(new AsyncCallback(AcceptCallback), _server );

					// Wait until a connection is made before continuing.
					allDone.WaitOne();
				}

			} catch (Exception e) {
				Log.Error(e.ToString());
			}

		}

		public void Stop () {
			_server.Shutdown (SocketShutdown.Both);
			_server.Close ();
		}

		protected void AcceptCallback(IAsyncResult result) {
			// Signal the main thread to continue.
			allDone.Set();

			// Get the socket that handles the client request.
			Socket listener = (Socket) result.AsyncState;
			Socket socket = listener.EndAccept(result);

			// Create the state object.
			SocketClient client = new SocketClient(socket);

			Log.Info ("Client socket connected: {0}", client.ToString());


			client.Closed += (object sender, SocketEventArgs e) => {
				Log.Info("Client socket disconnected: {0}", client);
			};


			try {
				this.Handler.Initialize (client);
			} catch (SocketException e) {
				Log.Error ("Error {0}", e.ToString ());
			}
		}
	}

}