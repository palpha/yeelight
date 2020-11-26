using System;
using System.Threading;

namespace Yeelight.Core
{
	public interface IResponseFactory
	{
		FutureResponse<T> Create<T>( CancellationToken cancellationToken = default );
		Func<byte[], Response>? TryGetDeserializer( int responseId );
	}
}