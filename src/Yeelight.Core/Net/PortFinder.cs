using System.Net;
using System.Net.Sockets;

namespace Yeelight.Core
{
	public static class PortFinder
	{
		public static int GetFreeTcpPort()
		{
			var listener = new TcpListener( IPAddress.Loopback, 0 );
			listener.Start();
			int port = ((IPEndPoint) listener.LocalEndpoint).Port;
			listener.Stop();

			return port;
		}
	}
}
