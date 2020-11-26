using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class IncomingMessageListenerTests
	{
		[Fact]
		public async Task Listens_to_udp_client()
		{
			var onePublished = false;
			var client = new Mock<IUdpClient>();
			client
				.Setup( x => x.ReceiveAsync() )
				.Returns(
					async () =>
					{
						if ( onePublished )
						{
							await Task.Delay( -1 );
						}

						onePublished = true;

						return Encoding.ASCII.GetBytes(
							TestMessages.SEARCH_RESPONSE );
					} );

			var dispatcher = new Mock<IIncomingMessageDispatcher>();
			var logger = NullLogger<DiscoveryMessageListener>.Instance;
			var token = new CancellationToken( true );
			var listener = new DiscoveryMessageListener( client.Object, dispatcher.Object, logger );

			await listener.Listen( token );

			dispatcher.Verify( x => x.Dispatch( TestMessages.SEARCH_RESPONSE ) );
		}
	}
}