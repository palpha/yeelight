using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Yeelight.Core
{
	public class YeelightException : Exception
	{
		public int Code { get; }

		public YeelightException( Error error ) : base( error.Message )
		{
			Code = error.Code;
		}
	}

	public class ResponseFactory : IResponseFactory, IObserver<Response>
	{
		private record Future( Action<Response> SetCompleted, Action<YeelightException> SetErrored );

		private int counter = 0;

		private ConcurrentDictionary<int, Func<byte[], Response>> Deserializers { get; } = new();
		private ConcurrentDictionary<int, Future> Futures { get; } = new();

		private IDisposable ResponseSubscription { get; }
		private ILogger<ResponseFactory> Logger { get; }

		public ResponseFactory(
			IObservable<Response> responseStream,
			ILogger<ResponseFactory> logger )
		{
			ResponseSubscription = responseStream.Subscribe( this );
			Logger = logger;
		}

		public FutureResponse<T> Create<T>( CancellationToken cancellationToken = default )
		{
			var future = new TaskCompletionSource<Response<T>>();
			var registration = cancellationToken.Register( future.SetCanceled );

			var id = Interlocked.Increment( ref counter );
			Deserializers.TryAdd( id, Response<T>.Deserialize );
			Futures.TryAdd(
				id,
				new(
					x =>
					{
						registration.Unregister();
						future.SetResult( (Response<T>) x );
					},
					x =>
					{
						registration.Unregister();
						future.SetException( x );
					} ) );

			Logger.LogDebug( "Created future #{0} for result of type {1}", id, typeof( T ) );

			return new FutureResponse<T>( id, future.Task );
		}

		public Func<byte[], Response>? TryGetDeserializer( int responseId )
		{
			if ( Deserializers.TryGetValue( responseId, out var deserializer ) == false )
			{
				Logger.LogWarning( "Unable to find deserializer for response #{0}", responseId );
				return null;
			}

			return deserializer;
		}

		public void OnCompleted() =>
			ResponseSubscription?.Dispose();

		public void OnError( Exception ex ) =>
			Logger.LogError( ex, "Error in stream of responses" );

		public void OnNext( Response response )
		{
			if ( Futures.TryGetValue( response.Id, out var future ) == false )
			{
				Logger.LogWarning(
					"Unable to find future for response #{0} of type {1}",
					response.Id,
					response.GetType() );

				return;
			}

			if ( response is ErrorResponse error )
			{
				future.SetErrored( new( error.Error ) );
				return;
			}

			future.SetCompleted( response );
		}
	}

}