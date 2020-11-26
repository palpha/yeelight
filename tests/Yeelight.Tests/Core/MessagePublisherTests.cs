using System;
using System.Collections.Generic;
using Xunit;
using Yeelight.Core;
using FluentAssertions;

namespace Yeelight.Tests.Core
{
	public class MessagePublisherTests
	{
		[Fact]
		public void Monitor_publishes_devices_to_observers()
		{
			var observed = new List<Device>();
			var monitor = new MessagePublisher<Device>();

			monitor.Subscribe( observed.Add );
			monitor.Publish( TestMessages.NewSearchResponse() );

			observed.Count.Should().Be( 1 );
		}

		[Fact]
		public void Monitor_subscription_disposal_stops_subscription()
		{
			var observed = new List<Device>();
			var monitor = new MessagePublisher<Device>();

			using ( var _ = monitor.Subscribe( observed.Add ) )
			{
				monitor.Publish( TestMessages.NewSearchResponse() );
			}

			monitor.Publish( TestMessages.NewSearchResponse() );

			observed.Count.Should().Be( 1 );
		}
	}
}
