using Xunit;
using Yeelight.Core;
using FluentAssertions;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging.Abstractions;

namespace Yeelight.Tests.Core
{
	public class DeviceCatalogTests
	{
		[Fact]
		public void Catalog_can_turn_stream_of_messages_into_devices()
		{
			var subject = new Subject<Device>();
			var catalog = new DeviceCatalog( subject, NullLogger<DeviceCatalog>.Instance );

			subject.OnNext( TestMessages.NewSearchResponse() );
			subject.OnNext( TestMessages.NewSearchResponse() );
			var device = TestMessages.NewSearchResponse();
			subject.OnNext( device );
			subject.OnNext( device );

			catalog.Count.Should().Be( 3 );
		}

		[Fact]
		public void Can_enumerate_catalog()
		{
			var subject = new Subject<Device>();
			var catalog = new DeviceCatalog( subject, NullLogger<DeviceCatalog>.Instance );
			subject.OnNext( TestMessages.NewSearchResponse() with { Id = 7 } );

			var i = 0;
			foreach ( var device in catalog )
			{
				i++;
				device.Id.Should().Be( 7 );
			}
		}
	}
}
