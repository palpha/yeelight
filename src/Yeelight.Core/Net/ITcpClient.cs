using System;
using System.Net;
using System.Threading.Tasks;

namespace Yeelight.Core
{
	public interface ITcpClient : IDisposable
	{
		bool IsConnected { get; }
		Task ConnectAsync( IPEndPoint endpoint );
		INetworkStream GetStream();
	}
}
