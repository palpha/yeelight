using System.Collections.Generic;

namespace Yeelight.Core
{
	public interface ICommandValidator
	{
		bool Validate( Command command, out IEnumerable<string> errorMessages );
	}
}