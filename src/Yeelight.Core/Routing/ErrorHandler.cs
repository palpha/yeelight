using System.Text;

namespace Yeelight.Core
{
	public class ErrorHandler : IncomingMessageHandlerBase<Response>
	{
		public ErrorHandler( IMessagePublisher<Response> publisher ) : base( publisher )
		{
		}

		protected override bool TryDeserialize( string message, out Response? deserialized )
		{
			if ( message.Contains( "\"error\":" ) == false )
			{
				deserialized = null;
				return false;
			}

			var bytes = Encoding.ASCII.GetBytes( message );
			deserialized = ErrorResponse.Deserialize( bytes );

			return true;
		}
	}
}
