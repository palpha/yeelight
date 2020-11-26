using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class IntegrationTestFixture : IDisposable, IAsyncLifetime
	{
		private IHost MyHost { get; }

		public OutputWriter Output { get; }
		public bool IsConfigured { get; }

		public IntegrationTestFixture( IMessageSink output )
		{
			Output = new( output );
			MyHost = CreateHost();
			IsConfigured =
				GetService<IOptions<NetworkClientOptions>>().Value.PublicEndpoint
					.StartsWith( "127." ) == false;
		}

		public T GetService<T>() where T : class =>
			MyHost.Services.GetRequiredService<T>();

		private IHost CreateHost()
		{
			Output.WriteLine( "Configuring host..." );

			var hostBuilder =
				Host.CreateDefaultBuilder()
					.ConfigureAppConfiguration( x => x
						.AddInMemoryCollection( new KeyValuePair<string, string>[] {
							new( "Service:PublicEndpoint", "127.0.0.1:50000" ) } )
						.AddJsonFile( "appsettings.json" ) )
					.ConfigureServices( ( ctx, x ) => x
						.AddLogging()
						.AddYeelight( ctx.Configuration ) );

			Output.WriteLine( "Starting host..." );

			return hostBuilder.Start();
		}

		public void Dispose()
		{
			Output.WriteLine( "Disposing..." );
			GC.SuppressFinalize( this );
			MyHost.Dispose();
			Output.WriteLine( "Disposed." );
		}

		public async Task InitializeAsync()
		{
			if ( IsConfigured == false )
			{
				return;
			}

			await GetService<IDiscoveryService>().InitiateAsync();
			var deviceCatalog = GetService<IDeviceCatalog>();
			var cts = new CancellationTokenSource( 1000 );
			while ( deviceCatalog.Count == 0 && cts.IsCancellationRequested == false )
			{
				await Task.Delay( 100, cts.Token );
			}

		}

		public Task DisposeAsync()
		{
			return Task.CompletedTask;
		}
	}
}
