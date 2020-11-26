using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class CommandServiceTests
	{
		private record Bits<T>(
			IPEndPoint Endpoint,
			Mock<IControlConnection> Connection,
			CommandService Service,
			Mock<ICommandValidator> Validator );

		private static Bits<T> SetupService<T>( params T[] result )
		{
			var commandValidator = new Mock<ICommandValidator>();
			commandValidator
				.Setup( x => x.Validate( It.IsAny<Command>(), out It.Ref<IEnumerable<string>>.IsAny ) )
				.Returns( true );

			var endpoint = IPEndPoint.Parse( "127.0.0.1:12345" )!;
			var connection = new Mock<IControlConnection>();
			var connectionPool = new Mock<IControlConnectionPool>();
			connectionPool
				.Setup( x => x.GetAsync( endpoint, It.IsAny<CancellationToken>() ) )
				.ReturnsAsync( connection.Object );

			var responseFactory = new Mock<IResponseFactory>();
			responseFactory
				.Setup( x => x.Create<T>( It.IsAny<CancellationToken>() ) )
				.Returns(
					new FutureResponse<T>(
						123, Task.FromResult( new Response<T>( 123, result ) ) ) );

			var service =
				new CommandService(
					commandValidator.Object,
					connectionPool.Object,
					responseFactory.Object );

			return new( endpoint, connection, service, commandValidator );
		}

		[Fact]
		public async Task Can_send_message()
		{
			var bits = SetupService( Array.Empty<string>() );
			var command = new Commands.GetProp( "foobar" );

			await bits.Service.SendAsync( new( bits.Endpoint, "get_prop" ), command );

			var expected = command.Tag( 123 ).Serialize();
			bits.Connection.Verify( x => x.SendAsync( expected, It.IsAny<CancellationToken>() ) );
		}

		[Fact]
		public async Task Can_receive_answer()
		{
			var bits = SetupService( "barbaz" );
			var command = new Commands.GetProp( "foobar" );

			var answer = await bits.Service.SendAsync( new( bits.Endpoint, "get_prop" ), command );

			answer.Should().BeEquivalentTo( "barbaz" );
		}

		[Fact]
		public async Task Unsupported_command_throws()
		{
			var bits = SetupService<string>();
			var command = new Commands.GetProp( "" );

			await Assert.ThrowsAsync<CommandNotSupportedException>( () =>
				bits.Service.SendAsync( new( bits.Endpoint, "foo" ), command ) );
		}

		[Fact]
		public async Task Validates_commands()
		{
			var bits = SetupService<string>();
			var command = new Commands.GetProp();

			await bits.Service.SendAsync( new( bits.Endpoint, "get_prop" ), command );

			bits.Validator.Verify(
				x => x.Validate( It.IsAny<Command>(), out It.Ref<IEnumerable<string>>.IsAny ),
				Times.Once );
		}
	}
}