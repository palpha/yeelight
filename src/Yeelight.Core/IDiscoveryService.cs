using System.Threading.Tasks;

namespace Yeelight.Core
{
	public interface IDiscoveryService
	{
		Task InitiateAsync();
	}
}