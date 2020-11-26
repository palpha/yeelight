using System.Collections.Generic;
using Moq;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class CronGetResponseHandlerTests : HandlerTestsBase<Response>
	{
		protected override IEnumerable<string> ValidMessages =>
			new[] { TestMessages.CRON_GET_RESPONSE };

		protected override IEnumerable<string> InvalidMessages =>
			new[]
			{
				TestMessages.SEARCH_RESPONSE,
				TestMessages.CRON_ADD_RESPONSE,
				TestMessages.NOTIFICATION,
				TestMessages.PROP_GET_RESPONSE
			};

		protected override IIncomingMessageHandler CreateHandler( IMessagePublisher<Response> publisher )
		{
			var responseFactory = new Mock<IResponseFactory>();
			responseFactory
				.Setup( x => x.TryGetDeserializer( 1 ) )
				.Returns( Response<CronGetResult>.Deserialize );

			return new ResponseHandler( publisher, responseFactory.Object );
		}
	}
}