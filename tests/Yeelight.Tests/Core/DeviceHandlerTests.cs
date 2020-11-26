using System;
using System.Collections.Generic;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class DeviceHandlerTests : HandlerTestsBase<Device>
	{
		protected override IEnumerable<string> ValidMessages =>
			new[]
			{
				TestMessages.SEARCH_RESPONSE
			};

		protected override IEnumerable<string> InvalidMessages =>
			new[]
			{
				TestMessages.CRON_ADD_RESPONSE,
				TestMessages.CRON_GET_RESPONSE,
				TestMessages.NOTIFICATION,
				TestMessages.PROP_GET_RESPONSE
			};

		protected override IIncomingMessageHandler CreateHandler( IMessagePublisher<Device> publisher ) =>
			new DeviceHandler( publisher );
	}
}