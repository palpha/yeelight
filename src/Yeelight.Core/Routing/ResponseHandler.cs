using System;
using System.Text;
using System.Text.Json;

namespace Yeelight.Core
{
	public class ResponseHandler : IncomingMessageHandlerBase<Response>
	{
		private IResponseFactory ResponseFactory { get; }

		public ResponseHandler(
			IMessagePublisher<Response> publisher,
			IResponseFactory responseFactory ) : base( publisher )
		{
			ResponseFactory = responseFactory;
		}

		protected override bool TryDeserialize( string message, out Response? deserialized )
		{
			deserialized = null;
			if ( message.Contains( "\"result\":" ) == false )
			{
				return false;
			}

			var bytes = Encoding.ASCII.GetBytes( message );
			Func<byte[], Response>? deserializer;

			if ( TryGetMessageId( bytes, out var messageId ) == false
				|| (deserializer = ResponseFactory.TryGetDeserializer( messageId )) is null )
			{
				return false;
			}

			deserialized = deserializer( bytes );
			return true;
		}

		protected static bool TryGetMessageId( byte[] bytes, out int messageId )
		{
			var reader = new Utf8JsonReader( bytes );

			if ( (reader.Read() && reader.TokenType == JsonTokenType.StartObject
				&& reader.Read() && reader.TokenType == JsonTokenType.PropertyName
				&& reader.Read() && reader.TokenType == JsonTokenType.Number) == false )
			{
				messageId = 0;
				return false;
			}

			messageId = reader.GetInt32();
			return true;
		}
	}
}
