using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class TestingTcp
	{
		public ITestOutputHelper Output { get; }

		public TestingTcp( ITestOutputHelper output )
		{
			Output = output;
		}


		[Fact]
		public async Task Test()
		{
			var endpoint = IPEndPoint.Parse( "192.168.1.76:55443" );
			var cts = new CancellationTokenSource();

			using var tcpClient = new TcpClientFacade();

			Output.WriteLine( "Connecting" );

			await tcpClient.ConnectAsync( endpoint );

			Output.WriteLine( "Connected" );

			using var stream = tcpClient.GetStream();

			Output.WriteLine( "Got stream" );

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

			Task.Run( async () =>
			{
				Output.WriteLine( "Starting read loop" );

				while ( cts.Token.IsCancellationRequested == false )
				{
					if ( stream.DataAvailable == false )
					{
						await Task.Delay( 100 );
						continue;
					}

					var buffer = new Memory<byte>( new byte[1024] );
					if ( await stream.ReadAsync( buffer ) > 0 )
					{
						var str = Encoding.ASCII.GetString( buffer.Span.TrimEnd( (byte) 0 ) );
						Output.WriteLine( "Received: {0}", str );
						cts.Cancel();
					}
					else
					{
						return;
					}
				}
			}, cts.Token );

			await Task.Delay( 300 );

			Task.Run( async () =>
			{
				Output.WriteLine( "Preparing to send" );

				var command = new Commands.GetProp( "power" ).Tag( 1 );
				var commandStr = command.Serialize();
				var bytes = Encoding.ASCII.GetBytes( commandStr + "\r\n" );
				await stream.WriteAsync( bytes.AsMemory( 0, bytes.Length ) );
				Output.WriteLine( "Wrote to client: {0}", commandStr );

				cts.CancelAfter( 5000 );
			}, cts.Token );

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

			while ( cts.Token.IsCancellationRequested == false )
			{
				await Task.Delay( 100 );
			}
		}
	}
}
