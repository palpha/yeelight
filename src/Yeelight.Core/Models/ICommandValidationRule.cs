using System;

namespace Yeelight.Core
{
	public interface ICommandValidationRule
	{
		string ErrorMessage { get; }
		Type CommandType { get; }
		bool Validate( Command command );
	}
}
