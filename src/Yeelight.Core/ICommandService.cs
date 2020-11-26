using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Yeelight.Core
{
	public interface ICommandService
	{
		Task<ICollection<T>> SendAsync<T>(
			Target target,
			Command<T> command,
			CancellationToken cancellationToken = default );
	}
}