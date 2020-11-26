using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Yeelight.Core
{
	public class UdpNetworkClient : IUdpClient
	{
		private UdpClient UdpClient { get; }

		private bool Disposed { get; set; }

		public IPEndPoint Endpoint { get; }
		public ILogger<UdpNetworkClient>? Logger { get; }

		public UdpNetworkClient(
			IOptions<NetworkClientOptions> options,
			ILogger<UdpNetworkClient>? logger = null )
		{
			Logger = logger ?? NullLogger<UdpNetworkClient>.Instance;

			if ( IPEndPoint.TryParse( options.Value.PublicEndpoint, out var endpoint ) == false )
			{
				var message = $"Unable to parse endpoint configuration ({options.Value.PublicEndpoint}).";
				Logger.LogError( message );
				throw new ConfigurationException( message );
			}

			Endpoint = endpoint;
			UdpClient = new UdpClient( Endpoint );
		}

		public async Task<byte[]> ReceiveAsync() =>
			(await UdpClient.ReceiveAsync()).Buffer;

		public async Task SendAsync( byte[] datagram, int length, IPEndPoint endpoint ) =>
			await UdpClient.SendAsync( datagram, length, endpoint );

		protected virtual void Dispose( bool disposing )
		{
			if ( Disposed == false )
			{
				if ( disposing )
				{
					UdpClient.Dispose();
				}

				Disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}
	}
}
