using System.Collections.Generic;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class ErrorHandlerTests : HandlerTestsBase<Response>
	{
		protected override IEnumerable<string> ValidMessages =>
			new[] { TestMessages.ERROR_RESPONSE };

		protected override IEnumerable<string> InvalidMessages =>
			new[]
			{
				TestMessages.CRON_ADD_RESPONSE,
				TestMessages.PROP_GET_RESPONSE,
				TestMessages.SEARCH_RESPONSE,
				TestMessages.ADVERTISEMENT,
				TestMessages.NOTIFICATION
			};

		protected override IIncomingMessageHandler CreateHandler( IMessagePublisher<Response> publisher ) =>
			new ErrorHandler( publisher );
	}
}