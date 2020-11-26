using System.Threading.Tasks;

namespace Yeelight.Core
{
	public record FutureResponse<T>( int Id, Task<Response<T>> Task );
}