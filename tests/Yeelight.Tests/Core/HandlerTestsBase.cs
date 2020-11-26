using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Xunit;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public abstract class HandlerTestsBase<TMessage>
	{
		protected abstract IEnumerable<string> ValidMessages { get; }
		protected abstract IEnumerable<string> InvalidMessages { get; }

		protected abstract IIncomingMessageHandler CreateHandler( IMessagePublisher<TMessage> publisher );

		[Fact]
		public void Can_produce_messages()
		{
			var publisher = new Mock<IMessagePublisher<TMessage>>();
			var handler = CreateHandler( publisher.Object );

			foreach ( var message in ValidMessages )
			{
				publisher.Reset();
				var result = handler.TryHandle( message );
				result.Should().BeTrue( "handler should handle {0}", message );
				publisher.Verify( x => x.Publish( It.IsAny<TMessage>() ), Times.Once );
			}
		}

		[Fact]
		public void Can_ignore_invalid_messages()
		{
			var publisher = new Mock<IMessagePublisher<TMessage>>();
			var handler = CreateHandler( publisher.Object );

			foreach ( var message in InvalidMessages )
			{
				var result = handler.TryHandle( message );
				result.Should().BeFalse( "handler should not handle {0}", message );
			}

			publisher.Verify( x => x.Publish( It.IsAny<TMessage>() ), Times.Never );
		}
	}
}