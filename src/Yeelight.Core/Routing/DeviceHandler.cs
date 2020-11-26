namespace Yeelight.Core
{
	public class DeviceHandler : IncomingMessageHandlerBase<Device>
	{
		public DeviceHandler( IMessagePublisher<Device> publisher ) : base( publisher )
		{
		}

		protected override bool TryDeserialize( string message, out Device? deserialized )
		{
			var trimmed = message.TrimStart();

			if ( (trimmed.StartsWith( "HTTP/1.1 200 OK" )
				|| trimmed.StartsWith( "NOTIFY * HTTP/1.1" )) == false )
			{
				deserialized = null;
				return false;
			}

			deserialized = Device.Parse( message );
			return true;
		}
	}
}
