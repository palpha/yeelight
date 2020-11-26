using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Yeelight.Service;
using Yeelight.Core;

try
{
	var hostBuilder =
		Host.CreateDefaultBuilder()
			.ConfigureAppConfiguration( x => x
				.AddCommandLine(
					args,
					new Dictionary<string, string>
					{
						{ "-e", "Service:PublicEndpoint" },
						{ "--public-endpoint", "Service:PublicEndpoint" },
						{ "-d", "Devices:Disabled" },
						{ "--disabled", "Devices:Disabled" }
					} ) )
			.ConfigureLogging( x => x.AddConsole().SetMinimumLevel( LogLevel.Debug ) )
			//.UseSystemd()
			//.UseWindowsService( x => x.ServiceName = "YeelightSvc" )
			.ConfigureServices( ( ctx, x ) => x
				.AddYeelight( ctx.Configuration )
				.AddHostedService<YeelightService>() );

	await hostBuilder.RunConsoleAsync();
}
catch ( ConfigurationException ex )
{
	Console.WriteLine( ex.Message );
}