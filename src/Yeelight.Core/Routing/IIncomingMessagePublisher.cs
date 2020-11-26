namespace Yeelight.Core
{
	public interface IIncomingMessageHandler
	{
		bool TryHandle( string message );
	}
}
