using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Yeelight.Core
{
	public interface IControlConnection : IDisposable
	{
		bool IsConnected { get; }

		Task ConnectAsync(
			IPEndPoint endpoint,
			CancellationToken cancellationToken = default );

		Task SendAsync(
			string payload,
			CancellationToken cancellationToken = default );
	}
}
