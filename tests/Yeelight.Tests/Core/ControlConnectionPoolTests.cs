using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class ControlConnectionPoolTests
	{
		[Fact]
		public async Task Creates_connection_when_asked()
		{
			var connection = new Mock<IControlConnection>();
			Func<IControlConnection> factory = () => connection.Object;
			var pool = new ControlConnectionPool( factory );

			var result = await pool.GetAsync( IPEndPoint.Parse( "127.0.0.1:12345" ) );

			result.Should().Be( connection.Object );
		}

		[Fact]
		public async Task Creates_connection_per_endpoint()
		{
			var idx = 0;
			var factory = new Mock<Func<IControlConnection>>();
			factory
				.Setup( x => x.Invoke() )
				.Returns( () => new Mock<IControlConnection> { Name = idx++.ToString() }.Object );
			var pool = new ControlConnectionPool( factory.Object );

			var result1 = await pool.GetAsync( IPEndPoint.Parse( "127.0.0.1:12345" ) );
			var result2 = await pool.GetAsync( IPEndPoint.Parse( "127.0.0.1:23456" ) );
			var result3 = await pool.GetAsync( IPEndPoint.Parse( "127.0.0.1:12345" ) );

			factory.Verify( x => x.Invoke(), Times.Exactly( 2 ) );
			var name1 = Mock.Get( result1 ).Name;
			var name2 = Mock.Get( result2 ).Name;
			var name3 = Mock.Get( result3 ).Name;
			name1.Should().NotBe( name2 );
			name1.Should().Be( name3 );
		}

		[Fact]
		public async Task Connects_connection()
		{
			var connection = new Mock<IControlConnection>();
			var connected = 0;
			connection.Setup( x => x.IsConnected ).Returns( () => connected++ == 0 );
			IControlConnection Factory() => connection.Object;
			var pool = new ControlConnectionPool( Factory );
			var endpoint = IPEndPoint.Parse( "127.0.0.1:12345" );

			var result = await pool.GetAsync( endpoint );
			await pool.GetAsync( endpoint );

			connection.Verify( x => x.ConnectAsync( endpoint, It.IsAny<CancellationToken>() ), Times.Once );
		}
	}
}