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

	public abstract record DurationRule<T>(
		Func<T, TimeSpan> DurationFn,
		Func<T, Effect> EffectFn ) : CommandValidationRule<T>(
	x =>
	{
		if ( EffectFn( x ) == Effect.Sudden )
		{
			return true;
		}

		var duration = DurationFn( x );
		return duration.TotalMilliseconds >= 30 && duration.TotalMilliseconds <= 30000;
	},
	"Duration must be between 30 and 30000 milliseconds" ) where T : Command;
}
