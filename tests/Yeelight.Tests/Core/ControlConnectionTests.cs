using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class ControlConnectionTests
	{
		private ITestOutputHelper Output { get; }

		private record Bits(
			Mock<ITcpClient> Client,
			IPEndPoint Endpoint,
			ControlConnection Connection,
			ICollection<string> Dispatched );

		private class StreamWrapper : INetworkStream
		{
			public bool DataAvailable => Stream is not null;
			public bool CanRead => Stream is not null;

			public MemoryStream Stream { get; }

			public StreamWrapper( MemoryStream? stream = null ) =>
				Stream = stream ?? new MemoryStream();

			public ValueTask<int> ReadAsync(
				Memory<byte> buffer,
				CancellationToken cancellationToken = default ) =>
				Stream.ReadAsync( buffer, cancellationToken );

			public ValueTask WriteAsync(
				ReadOnlyMemory<byte> buffer,
				CancellationToken cancellationToken = default ) =>
				Stream.WriteAsync( buffer, cancellationToken );

			public void Dispose()
			{
			}
		}

		public ControlConnectionTests( ITestOutputHelper output ) => Output = output;

		private Bits SetupSystem(
			MemoryStream? stream = null,
			Action<string>? dispatch = null )
		{
			var streamWrapper = new StreamWrapper( stream );
			var client = new Mock<ITcpClient>();
			client
				.Setup( x => x.GetStream() )
				.Returns( streamWrapper );

			var dispatcher = new Mock<IIncomingMessageDispatcher>();
			var dispatched = new List<string>();
			dispatcher
				.Setup( x => x.Dispatch( It.IsAny<string>() ) )
				.Callback( dispatch ?? dispatched.Add );

			var logger = Output.BuildLoggerFor<ControlConnection>();
			var connection = new ControlConnection( client.Object, dispatcher.Object, logger );
			var endpoint = new IPEndPoint( IPAddress.Parse( "127.0.0.1" ), 12345 );

			return new( client, endpoint, connection, dispatched );
		}

		[Fact]
		public async Task Connects_to_endpoint()
		{
			var bits = SetupSystem();

			await bits.Connection.ConnectAsync( bits.Endpoint );

			bits.Client.Verify( x => x.ConnectAsync( bits.Endpoint ) );
		}

		[Fact]
		public async Task Dies_if_not_connected_before_send()
		{
			var bits = SetupSystem();

			await Assert.ThrowsAnyAsync<Exception>( async () =>
				await bits.Connection.SendAsync( "foobar" ) );
		}

		[Fact]
		public async Task Writes_to_tcp_client()
		{
			var stream = new MemoryStream();
			var bits = SetupSystem( stream );
			await bits.Connection.ConnectAsync( bits.Endpoint );

			await bits.Connection.SendAsync( "foobar" );

			stream.Seek( 0, SeekOrigin.Begin );
			var sent = Encoding.ASCII.GetString( stream.ToArray() );
			sent.Should().Be( "foobar\r\n" );
		}

		[Fact]
		public async Task Connect_hooks_up_listener()
		{
			var stream = new MemoryStream( Encoding.ASCII.GetBytes( "foobar\r\n" ) );
			stream.Seek( 0, SeekOrigin.Begin );
			var bits = SetupSystem( stream );

			await bits.Connection.ConnectAsync( bits.Endpoint );

			await Task.Delay( 100 );

			bits.Client.Verify( x => x.GetStream(), Times.Once );
			bits.Dispatched.Should().BeEquivalentTo( "foobar\r\n" );
		}

		[Fact]
		public async Task Can_handle_long_stream_of_data()
		{
			var input = string.Join( "", Enumerable.Repeat( "foobar\r\n", 10000 ) );
			var stream = new MemoryStream( Encoding.ASCII.GetBytes( input ) );
			stream.Seek( 0, SeekOrigin.Begin );
			var bits = SetupSystem( stream );

			await bits.Connection.ConnectAsync( bits.Endpoint );

			while ( bits.Dispatched.Count < 10000 )
			{
				await Task.Delay( 100 );
			}

			bits.Client.Verify( x => x.GetStream(), Times.Once );
			bits.Dispatched.Should().HaveCount( 10000 )
				.And.OnlyContain( x => x == "foobar\r\n" );
		}

		[Fact]
		public async Task Can_deal_with_errors_in_dispatch()
		{
			var input = "foobar1\r\nfoobar2\r\n";
			var stream = new MemoryStream( Encoding.ASCII.GetBytes( input ) );
			var foobar2 = false;
			stream.Seek( 0, SeekOrigin.Begin );
			var bits =
				SetupSystem(
					stream,
					x =>
					{
						if ( x.StartsWith( "foobar1" ) )
						{
							throw new InvalidOperationException( "Some error" );
						}

						if ( x.StartsWith( "foobar2" ) )
						{
							foobar2 = true;
						}
					} );

			await bits.Connection.ConnectAsync( bits.Endpoint );

			await Task.Delay( 100 );
			foobar2.Should().BeTrue();
		}
	}
}