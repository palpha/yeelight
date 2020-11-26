using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Yeelight.Core
{
	public class ControlConnection : IControlConnection
	{
		private ITcpClient Client { get; }
		private IIncomingMessageDispatcher Dispatcher { get; }
		private ILogger<ControlConnection>? Logger { get; }

		private IPEndPoint? Endpoint { get; set; }
		private CancellationToken CancellationToken { get; set; }
		private INetworkStream? Stream { get; set; }
		private bool IsDisposed { get; set; }

		public bool IsConnected => Client.IsConnected;

		public ControlConnection(
			ITcpClient client,
			IIncomingMessageDispatcher dispatcher,
			ILogger<ControlConnection>? logger = null )
		{
			Client = client;
			Dispatcher = dispatcher;
			Logger = logger ?? NullLogger<ControlConnection>.Instance;
		}

		public async Task ConnectAsync(
			IPEndPoint endpoint,
			CancellationToken cancellationToken = default )
		{
			if ( Client.IsConnected )
			{
				Logger.LogWarning( $"Client already connected" );
				return;
			}

			Endpoint = endpoint;
			CancellationToken = cancellationToken;

			Logger.LogInformation( "Connecting to {0}", Endpoint );
			await Client.ConnectAsync( Endpoint );
			Stream = Client.GetStream();

			//TODO: make safer, handle disconnects, etc
			_ = Task.Run( CreateListener, CancellationToken );
		}

		public async Task SendAsync(
			string payload,
			CancellationToken cancellationToken = default )
		{
			ValidateStream();

			Logger.LogDebug( "Sending {0} to {1}", payload, Endpoint );

			await Stream.WriteAsync(
				Encoding.ASCII.GetBytes( payload.Trim() + "\r\n" ),
				cancellationToken );
		}

		[MemberNotNull(
			nameof( Stream ),
			nameof( Endpoint ) )]
		private void ValidateStream()
		{
			if ( Stream is not null && Endpoint is not null )
			{
				return;
			}

			throw new InvalidOperationException( "Stream null or not ready for writing" );
		}

		private async Task CreateListener()
		{
			ValidateStream();

			Memory<char> outerBuffer = new char[1024 * 8];
			var outerBufferIdx = 0;

			void CheckEndOfMessage( byte current, byte previous )
			{
				if ( current != 10 || previous != 13 )
				{
					return;
				}

				var message = new string( outerBuffer.Slice( 0, outerBufferIdx ).Span );

				//TODO: avoid string alloc by passing Memory
				try
				{
					Dispatcher.Dispatch( message );
				}
				catch ( Exception ex )
				{
					Logger.LogError( ex, $"Error when handling message {message} from {Endpoint}" );
				}

				outerBufferIdx = 0;
			}

			Memory<byte> buffer = new byte[256];

			void AddToBuffer( int length )
			{
				var incomingSpan = buffer.TrimEnd( (byte) 0 ).Span;
				var outerSpan = outerBuffer.Span;
				byte previous = 0;
				for ( var i = 0; i < length; i++ )
				{
					var val = incomingSpan[i];
					outerSpan[outerBufferIdx++] = (char) val;
					CheckEndOfMessage( val, previous );
					previous = val;
				}
			}

			while ( CancellationToken.IsCancellationRequested == false )
			{
				if ( Stream.DataAvailable == false || Stream.CanRead == false )
				{
					await Task.Delay( 100 );
					continue;
				}

				int read;
				while ( (read = await Stream.ReadAsync( buffer )) > 0 )
				{
					AddToBuffer( read );
				}
			}
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( IsDisposed == false )
			{
				if ( disposing )
				{
					Stream?.Dispose();
				}

				IsDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose( disposing: true );
			GC.SuppressFinalize( this );
		}
	}
}
