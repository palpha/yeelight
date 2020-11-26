using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Yeelight.Core
{
	public static class ServiceCollectionExt
	{
		public static IServiceCollection AddYeelight(
			this IServiceCollection services,
			IConfiguration configuration ) =>
			services
				.Configure<NetworkClientOptions>( configuration.GetSection( "Service" ) )
				.Configure<DeviceOptions>( configuration.GetSection( "Devices" ) )
				.AddSingleton<IUdpClient, UdpNetworkClient>()
				.AddIncomingMessageHandler<Device, DeviceHandler>()
				.AddIncomingMessageHandler<Response, ResponseHandler>()
				.AddSingleton<IIncomingMessageHandler, ErrorHandler>()
				.AddSingleton<IDeviceCatalog, DeviceCatalog>()
				.AddSingleton<IDiscoveryService, DiscoveryService>()
				.AddSingleton<ICommandService, CommandService>()
				.AddSingleton<ICommandValidator, CommandValidator>()
				.AddValidationRules()
				.AddSingleton<IResponseFactory, ResponseFactory>()
				.AddSingleton<IIncomingMessageDispatcher, IncomingMessageDispatcher>()
				.AddSingleton<IControlConnectionPool, ControlConnectionPool>()
				.AddTransient<IControlConnection, ControlConnection>()
				.AddSingleton<Func<IControlConnection>>( x =>
					() => x.GetRequiredService<IControlConnection>() )
				.AddTransient<ITcpClient, TcpClientFacade>()
				.AddTransient<INetworkStream, NetworkStreamFacade>()
				.AddHostedService<DiscoveryMessageListener>();

		private static IServiceCollection AddIncomingMessageHandler<TMessage, TService>(
			this IServiceCollection services ) where TService : class, IIncomingMessageHandler =>
			services
				.AddSingleton<IIncomingMessageHandler, TService>()
				.AddSingleton<IMessagePublisher<TMessage>, MessagePublisher<TMessage>>()
				.AddSingleton<IObservable<TMessage>>( x =>
					(MessagePublisher<TMessage>) x.GetRequiredService<IMessagePublisher<TMessage>>() );

		private static IServiceCollection AddValidationRules( this IServiceCollection services )
		{
			foreach ( var ruleType in
				from commandType in typeof( Commands ).GetNestedTypes()
				from ruleType in commandType.GetNestedTypes()
				where ruleType.IsAssignableTo( typeof( ICommandValidationRule ) )
				select ruleType )
			{
				services.AddTransient( typeof( ICommandValidationRule ), ruleType );
			}

			return services;
		}
	}
}
