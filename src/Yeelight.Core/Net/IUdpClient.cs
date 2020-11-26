using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Yeelight.Core
{
	public interface IUdpClient : IDisposable
	{
		IPEndPoint Endpoint { get; }

		Task<byte[]> ReceiveAsync();
		Task SendAsync( byte[] datagram, int length, IPEndPoint endpoint );
	}
}
