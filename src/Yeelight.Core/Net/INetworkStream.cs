using System;
using System.Threading;
using System.Threading.Tasks;

namespace Yeelight.Core
{
	public interface INetworkStream : IDisposable
	{
		bool DataAvailable { get; }
		bool CanRead { get; }

		ValueTask<int> ReadAsync(
			Memory<byte> buffer,
			CancellationToken cancellationToken = default );

		ValueTask WriteAsync(
			ReadOnlyMemory<byte> buffer,
			CancellationToken cancellationToken = default );
	}
}
