using System;

namespace Yeelight.Core
{
	public static class ValidationExt
	{
		public static bool InRange<T>( this T obj, T minInclusive, T maxInclusive ) where T : IComparable<T> =>
			obj.CompareTo( minInclusive ) >= 0 && obj.CompareTo( maxInclusive ) <= 0;
	}

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

	public abstract record DurationRule<T>() : CommandValidationRule<T>(
	x =>
	{
		if ( x.Effect == Effect.Sudden )
		{
			return true;
		}

		return x.Duration.TotalMilliseconds >= 30 && x.Duration.TotalMilliseconds <= 30000;
	},
	"Duration must be between 30 and 30000 milliseconds" ) where T : Command, IDurational;
}
