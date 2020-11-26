namespace Yeelight.Core
{
	public interface IMessagePublisher<T>
	{
		void Publish( T message );
	}
}
