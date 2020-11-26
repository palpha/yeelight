using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Yeelight.Core
{
	public class DiscoveryService : IDiscoveryService
	{
		private IUdpClient NetworkClient { get; }
		private ILogger<DiscoveryService> Logger { get; }

		public DiscoveryService(
			IUdpClient networkClient,
			ILogger<DiscoveryService> logger )
		{
			NetworkClient = networkClient;
			Logger = logger;
		}

		public async Task InitiateAsync()
		{
			var address = IPAddress.Parse( "239.255.255.250" );
			var endpoint = new IPEndPoint( address, 1982 );
			var bytes =
				Encoding.ASCII.GetBytes(
					string.Join( "\r\n",
						"M-SEARCH * HTTP/1.1",
						$"HOST: {endpoint}",
						"MAN: \"ssdp:discover\"",
						"ST: wifi_bulb" ) );

			Logger.LogInformation( "Sending search request." );

			await NetworkClient.SendAsync( bytes, bytes.Length, endpoint );

			Logger.LogDebug( "Search request sent." );
		}
	}
}
