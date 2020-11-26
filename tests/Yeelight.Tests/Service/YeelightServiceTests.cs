using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Yeelight.Service;
using Yeelight.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Yeelight.Tests.Service
{
	public class YeelightServiceTests
	{
		[Fact]
		public async Task Host_sets_up_svc()
		{
			//var svc = new Mock<IMessagingService>();
			//var catalog = new Mock<IDeviceCatalog>();
			//var options = new Mock<IOptions<NetworkClientOptions>>();
			//options.Setup( x => x.Value ).Returns( new NetworkClientOptions { PublicEndpoint = "192.168.1.63:50000" } );

			//var host = new YeelightService( svc.Object, catalog.Object, NullLogger<YeelightService>.Instance, options.Object );
			//await host.StartAsync( new CancellationToken( true ) );

			//svc.Verify( x => x.Initialize( new IPEndPoint( IPAddress.Parse( "192.168.1.63" ), 50000 ) ) );
			//svc.Verify( x => x.Listen( It.IsAny<CancellationToken>() ) );
			//svc.Verify( x => x.Search() );
		}
	}
}
