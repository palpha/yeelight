using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class MockNetworkClient : IUdpClient
	{
		public record Sent( byte[] Bytes, int Length, IPEndPoint Endpoint );

		public IPEndPoint Endpoint { get; } = new( IPAddress.Loopback, 12345 );
		public ICollection<Sent> Sends { get; } = new List<Sent>();

		public Task<byte[]> ReceiveAsync()
		{
			throw new System.NotImplementedException();
		}

		public Task SendAsync( byte[] datagram, int length, IPEndPoint endpoint )
		{
			Sends.Add( new( datagram, length, endpoint ) );
			return Task.CompletedTask;
		}

		public void Dispose()
		{
		}
	}
}