using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Yeelight.Core
{
	public class ControlConnectionPool : IControlConnectionPool
	{
		private ConcurrentDictionary<IPEndPoint, IControlConnection> Connections { get; } = new();

		private Func<IControlConnection> ConnectionFactory { get; }

		private bool IsDisposed { get; set; }

		public ControlConnectionPool( Func<IControlConnection> connectionFactory ) =>
			ConnectionFactory = connectionFactory;

		public async Task<IControlConnection> GetAsync(
			IPEndPoint endpoint,
			CancellationToken cancellationToken = default )
		{
			var connection = Connections.GetOrAdd( endpoint, x => ConnectionFactory() );

			if ( connection.IsConnected == false )
			{
				await connection.ConnectAsync( endpoint, cancellationToken );
			}

			return connection;
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( IsDisposed == false )
			{
				if ( disposing )
				{
					foreach ( var (_, connection) in Connections )
					{
						connection.Dispose();
					}
				}

				IsDisposed = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose( disposing: true );
			GC.SuppressFinalize( this );
		}
	}
}
