using System.Collections.Generic;
using Moq;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class ResponseHandlerTests : HandlerTestsBase<Response>
	{
		protected override IEnumerable<string> ValidMessages =>
			new[]
			{
				TestMessages.CRON_ADD_RESPONSE,
				TestMessages.PROP_GET_RESPONSE
				// We ignore CRON_GET, which has its own payload type.
			};

		protected override IEnumerable<string> InvalidMessages =>
			new[]
			{
				TestMessages.SEARCH_RESPONSE,
				TestMessages.ADVERTISEMENT,
				TestMessages.NOTIFICATION,
				TestMessages.ERROR_RESPONSE
			};

		protected override IIncomingMessageHandler CreateHandler( IMessagePublisher<Response> publisher )
		{
			var responseFactory = new Mock<IResponseFactory>();
			responseFactory
				.Setup( x => x.TryGetDeserializer( It.IsIn( 2, 3 ) ) )
				.Returns( Response<string>.Deserialize );

			return new ResponseHandler( publisher, responseFactory.Object );
		}
	}
}