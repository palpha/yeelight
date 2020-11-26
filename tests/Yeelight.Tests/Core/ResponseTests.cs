using System.Text;
using FluentAssertions;
using Xunit;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class ResponseTests
	{
		[Fact]
		public void Can_parse_standard_response()
		{
			var result =
				Response<string>.Deserialize(
					Encoding.ASCII.GetBytes(
						"{\"id\":1,\"result\":[\"ok\"]}" ) );

			result.Id.Should().Be( 1 );
			result.Result.Should().HaveCount( 1 )
				.And.BeEquivalentTo( new[] { "ok" } );
		}

		private record SomeResult( int Foo, int Bar, string BarBaz );

		[Fact]
		public void Can_parse_involved_response()
		{
			var result =
				Response<SomeResult>.Deserialize(
					Encoding.ASCII.GetBytes(
						"{\"id\":1,\"result\":[{\"foo\":1,\"bar\":2,\"bar_baz\":\"3\"}]}" ) );

			result.Id.Should().Be( 1 );
			result.Result.Should().HaveCount( 1 )
				.And.BeEquivalentTo( new[] { new SomeResult( 1, 2, "3" ) } );
		}
	}
}
