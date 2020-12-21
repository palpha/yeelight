using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Yeelight.Core
{
	public class CommandService : ICommandService
	{
		private ICommandValidator CommandValidator { get; }
		private IControlConnectionPool ConnectionPool { get; }
		private IResponseFactory ResponseFactory { get; }
		private ILogger<CommandService> Logger { get; }

		public CommandService(
			ICommandValidator commandValidator,
			IControlConnectionPool connectionPool,
			IResponseFactory responseFactory,
			ILogger<CommandService>? logger = null )
		{
			CommandValidator = commandValidator;
			ConnectionPool = connectionPool;
			ResponseFactory = responseFactory;
			Logger = logger ?? NullLogger<CommandService>.Instance;
		}

		public async Task<ICollection<T>> SendAsync<T>(
			Target target,
			Command<T> command,
			CancellationToken cancellationToken = default )
		{
			if ( target.Supports( command.Method ) == false )
			{
				throw new CommandNotSupportedException( target, command );
			}

			if ( CommandValidator.Validate( command, out var errorMessages ) == false )
			{
				throw new CommandNotValidException( command, errorMessages );
			}

			var future = ResponseFactory.Create<T>( cancellationToken );
			var connection =
				await ConnectionPool.GetAsync(
					target.Endpoint,
					cancellationToken );
			var tagged = command.Tag( future.Id );

			Logger.LogDebug( "Sending {0} to {1}", tagged, target.Endpoint );

			var serialized = tagged.Serialize();
			await connection.SendAsync( serialized, cancellationToken );
			return (await future.Task).Result;
		}
	}
}
