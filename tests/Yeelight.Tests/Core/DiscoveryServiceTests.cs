using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class DiscoveryServiceTests
	{
		[Fact]
		public async Task Sends_the_right_stuff_for_search()
		{
			var networkClient = new MockNetworkClient();
			var service =
				new DiscoveryService(
					networkClient,
					NullLogger<DiscoveryService>.Instance );

			await service.InitiateAsync();

			const string EXPECTED_DATAGRAM =
				"M-SEARCH * HTTP/1.1\r\n"
					+ "HOST: 239.255.255.250:1982\r\n"
					+ "MAN: \"ssdp:discover\"\r\n"
					+ "ST: wifi_bulb";

			var sent = networkClient.Sends.Should().HaveCount( 1 ).And.Subject.First();
			sent.Bytes.Should().Equal( Encoding.ASCII.GetBytes( EXPECTED_DATAGRAM ) );
			sent.Length.Should().Be( EXPECTED_DATAGRAM.Length );
			sent.Endpoint.Address.Should().Be( IPAddress.Parse( "239.255.255.250" ) );
			sent.Endpoint.Port.Should().Be( 1982 );
		}
	}
}