using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yeelight.Core;

namespace Yeelight.Service
{
	public class YeelightService : BackgroundService
	{
		private IDiscoveryService DiscoveryService { get; }
		private ICommandService CommandService { get; }
		private IDeviceCatalog DeviceCatalog { get; }
		private ILogger<YeelightService> Logger { get; }
		private NetworkClientOptions Options { get; }

		public YeelightService(
			IDiscoveryService discoveryService,
			ICommandService commandService,
			IDeviceCatalog deviceCatalog,
			ILogger<YeelightService> logger,
			IOptions<NetworkClientOptions> options )
		{
			DiscoveryService = discoveryService;
			CommandService = commandService;
			DeviceCatalog = deviceCatalog;
			Logger = logger;
			Options = options.Value;
		}

		protected override async Task ExecuteAsync( CancellationToken stoppingToken )
		{
			await DiscoveryService.InitiateAsync();

			while ( stoppingToken.IsCancellationRequested == false )
			{
				Logger.LogInformation( "Device count: {0}", DeviceCatalog.Count );

				await Task.Delay( 1000, stoppingToken );
			}

			Logger.LogInformation( "Shutting down..." );
		}
	}
}
