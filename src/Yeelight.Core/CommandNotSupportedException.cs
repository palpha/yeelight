using System;
using System.Collections.Generic;

namespace Yeelight.Core
{
	public class CommandNotSupportedException : Exception
	{
		public CommandNotSupportedException( Target target, Command command )
			: base( $"Device at {target.Endpoint} does not support {command.Method}." )
		{
		}
	}

	public class CommandNotValidException : Exception
	{
		public IEnumerable<string> ErrorMessages { get; }

		public CommandNotValidException( Command command, IEnumerable<string> errorMessages )
			: base( $"Invalid {command.GetType()}: {string.Join( ";", errorMessages )} ({command})" )
		{
			ErrorMessages = errorMessages;
		}

	}
}
