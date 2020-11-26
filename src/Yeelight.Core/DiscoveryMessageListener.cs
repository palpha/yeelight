using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Yeelight.Core
{
	public class DiscoveryMessageListener : BackgroundService
	{
		private IUdpClient UdpClient { get; }
		private IIncomingMessageDispatcher Dispatcher { get; }
		private ILogger<DiscoveryMessageListener> Logger { get; }

		private bool IsDisposed { get; set; }

		public DiscoveryMessageListener(
			IUdpClient client,
			IIncomingMessageDispatcher dispatcher,
			ILogger<DiscoveryMessageListener> logger )
		{
			UdpClient = client;
			Dispatcher = dispatcher;
			Logger = logger;
		}

		protected override Task ExecuteAsync( CancellationToken stoppingToken ) =>
			Listen( stoppingToken );

		public async Task Listen( CancellationToken stoppingToken )
		{
			Logger.LogInformation( $"Listening to incoming messages at {UdpClient.Endpoint}" );

			do
			{
				try
				{
					var result = await UdpClient.ReceiveAsync();
					var resultStr = Encoding.ASCII.GetString( result );
					Dispatcher.Dispatch( resultStr );
				}
				catch ( Exception ) when ( IsDisposed || stoppingToken.IsCancellationRequested )
				{
					return;
				}
				catch ( Exception ex )
				{
					Logger.LogError( ex, "Unable to receive message" );
				}
			} while ( stoppingToken.IsCancellationRequested == false && IsDisposed == false );

			Logger.LogInformation( "Stopped listening" );
		}

		public override void Dispose()
		{
			IsDisposed = true;
		}
	}
}
