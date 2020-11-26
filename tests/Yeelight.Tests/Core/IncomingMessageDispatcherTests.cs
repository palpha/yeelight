using Moq;
using Xunit;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class IncomingMessageDispatcherTests
	{
		[Fact]
		public void Can_dispatch_to_right_handler()
		{
			Mock<IIncomingMessageHandler> CreateHandler( string msg )
			{
				var handler = new Mock<IIncomingMessageHandler>();
				handler
					.Setup( x => x.TryHandle( msg ) )
					.Returns( true );

				return handler;
			}

			var deviceHandler = CreateHandler( TestMessages.SEARCH_RESPONSE );
			var cronAddResponseHandler = CreateHandler( TestMessages.CRON_ADD_RESPONSE );
			var handlers = new[] { deviceHandler.Object, cronAddResponseHandler.Object };
			var dispatcher = new IncomingMessageDispatcher( handlers );

			dispatcher.Dispatch( TestMessages.SEARCH_RESPONSE );
			dispatcher.Dispatch( TestMessages.CRON_ADD_RESPONSE );

			deviceHandler.Verify( x => x.TryHandle( TestMessages.SEARCH_RESPONSE ), Times.Once );
			cronAddResponseHandler.Verify( x => x.TryHandle( TestMessages.CRON_ADD_RESPONSE ), Times.Once );
		}
	}
}