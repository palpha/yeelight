using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Yeelight.Core
{
	public class TcpClientFacade : ITcpClient
	{
		private TcpClient Client { get; } = new();

		private bool IsDisposed { get; set; }

		public bool IsConnected => Client.Connected;

		public Task ConnectAsync( IPEndPoint endpoint ) =>
			Client.ConnectAsync( endpoint.Address, endpoint.Port );

		public INetworkStream GetStream() =>
			new NetworkStreamFacade( Client.GetStream() );

		protected virtual void Dispose( bool disposing )
		{
			if ( IsDisposed == false )
			{
				if ( disposing )
				{
					Client.Dispose();
				}

				IsDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose( disposing: true );
			GC.SuppressFinalize( this );
		}
	}
}
