using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Yeelight.Core
{
	public class NetworkStreamFacade : INetworkStream
	{
		private NetworkStream Stream { get; }

		private bool IsDisposed { get; set; }

		public bool DataAvailable => Stream.DataAvailable;
		public bool CanRead => Stream.CanRead;

		public NetworkStreamFacade( NetworkStream stream ) =>
			Stream = stream;

		public ValueTask<int> ReadAsync(
			Memory<byte> buffer,
			CancellationToken cancellationToken = default ) =>
			Stream.ReadAsync( buffer, cancellationToken );

		public ValueTask WriteAsync(
			ReadOnlyMemory<byte> buffer,
			CancellationToken cancellationToken = default ) =>
			Stream.WriteAsync( buffer, cancellationToken );

		protected virtual void Dispose( bool disposing )
		{
			if ( IsDisposed == false )
			{
				if ( disposing )
				{
					Stream.Dispose();
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
