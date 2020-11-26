using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Yeelight.Core
{
	public interface IControlConnectionPool : IDisposable
	{
		Task<IControlConnection> GetAsync(
			IPEndPoint endpoint,
			CancellationToken cancellationToken = default );
	}
}
