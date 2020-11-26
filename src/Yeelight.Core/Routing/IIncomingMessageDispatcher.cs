using System.Net;

namespace Yeelight.Core
{
	public interface IIncomingMessageDispatcher
	{
		void Dispatch( string message );
	}
}
