using System;

namespace Yeelight.Core
{
	public abstract record CommandValidationRule<T>( Func<T, bool> ValidateFn, string ErrorMessage )
		: ICommandValidationRule where T : Command
	{
		public Type CommandType => typeof( T );

		public bool Validate( Command command )
		{
			if ( command is not T commandOfT )
			{
				return true;
			}

			return ValidateFn( commandOfT );
		}
	}
}
