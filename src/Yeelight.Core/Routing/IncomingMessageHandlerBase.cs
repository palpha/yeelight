using System.Diagnostics.CodeAnalysis;

namespace Yeelight.Core
{
	public abstract class IncomingMessageHandlerBase<T> : IIncomingMessageHandler
	{
		private IMessagePublisher<T> Publisher { get; }

		public IncomingMessageHandlerBase( IMessagePublisher<T> publisher )
		{
			Publisher = publisher;
		}

		protected abstract bool TryDeserialize( string message, [MaybeNull] out T deserialized );

		public bool TryHandle( string message )
		{
			if ( TryDeserialize( message, out var deserialized ) && deserialized is not null )
			{
				Publisher.Publish( deserialized );
				return true;
			}

			return false;
		}
	}
}
