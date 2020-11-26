using System.Collections.Generic;

namespace Yeelight.Core
{
	public interface IDeviceCatalog : IEnumerable<Device>
	{
		int Count { get; }
	}
}