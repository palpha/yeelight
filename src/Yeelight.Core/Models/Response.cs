using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

//TODO: fix namespace
namespace Yeelight.Core
{
	public abstract record Response( int Id )
	{
		//TODO: take Memory
		protected static T Deserialize<T>( byte[] payload ) =>
			JsonSerializer.Deserialize<T>(
				payload.AsSpan().TrimEnd( new byte[] { 13, 10 } ),
				new JsonSerializerOptions
				{
					PropertyNamingPolicy = new SnakeCaseNamingPolicy()
				} ) ?? throw new ArgumentException(
					"Payload was null",
					nameof( payload ) );
	}

	public record Error( int Code, string Message );
	public record ErrorResponse( int Id, Error Error )
		: Response( Id )
	{
		public static ErrorResponse Deserialize( byte[] payload ) =>
			Deserialize<ErrorResponse>( payload );
	}

	public record Response<TResult>( int Id, ICollection<TResult> Result )
		: Response( Id )
	{
		public static Response<TResult> Deserialize( byte[] payload ) =>
			Deserialize<Response<TResult>>( payload );
	}
}
