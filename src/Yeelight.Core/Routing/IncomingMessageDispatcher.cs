using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Yeelight.Core
{
	public class IncomingMessageDispatcher : IIncomingMessageDispatcher
	{
		private IEnumerable<IIncomingMessageHandler> Handlers { get; }
		private ILogger<IncomingMessageDispatcher> Logger { get; }

		public IncomingMessageDispatcher(
			IEnumerable<IIncomingMessageHandler> handlers,
			ILogger<IncomingMessageDispatcher>? logger = null )
		{
			Handlers = handlers;
			Logger = logger ?? NullLogger<IncomingMessageDispatcher>.Instance;
		}

		public void Dispatch( string message )
		{
			foreach ( var handler in Handlers )
			{
				if ( handler.TryHandle( message ) )
				{
					return;
				}
			}

			Logger.LogError( $"No handler for message {message}" );
		}
	}
}
