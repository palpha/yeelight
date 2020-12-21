using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;

namespace Yeelight.Core
{
	public static class EffectExt
	{
		public static string ToLowerString( this Effect effect ) =>
			effect.ToString().ToLowerInvariant();
	}

	public enum Effect { Sudden, Smooth };

	public record CronGetResult( int Type, int Delay, int Mix );

	public static class Commands
	{
		/// <summary>
		/// Retrieve current property of a smart LED.
		/// </summary>
		public record GetProp( params string[] Props ) : Command<string>()
		{
			public override IEnumerable<object> GetParams() => Props;

			public record PropsRule() : CommandValidationRule<GetProp>(
				x => x.Props.Any(),
				"At least one property must be queried" );
		}

		/// <summary>
		/// Change the color temperature of a smart LED.
		/// </summary>
		public record SetCtAbx( int CtValue, Effect Effect, TimeSpan Duration )
			: Command<string>()
		{
			public override IEnumerable<object> GetParams()
			{
				yield return CtValue;
				yield return Effect.ToLowerString();
				yield return Duration.TotalMilliseconds;
			}

			public record CtValueRule() : CommandValidationRule<SetCtAbx>(
				x => x.CtValue >= 1700 && x.CtValue <= 6500,
				"Temperature must be between 1700 and 6500" );

			//TODO: interface for commands with Effect & Duration instead of functions...
			public record SetCtAbxDurationRule() : DurationRule<SetCtAbx>( x => x.Duration, x => x.Effect );
		}

		/// <summary>
		/// Set the color of a smart LED to an RGB value.
		/// </summary>
		public record SetRgb( Color Color, Effect Effect, TimeSpan Duration )
			: Command<string>()
		{
			public override IEnumerable<object> GetParams()
			{
				yield return Color.ToUInt();
				yield return Effect.ToLowerString();
				yield return Duration.TotalMilliseconds;
			}

			public record SetRgbDurationRule() : DurationRule<SetRgb>( x => x.Duration, x => x.Effect );
		}

		/// <summary>
		/// Retrieves the current cron settings.
		/// </summary>
		public record CronGet() : Command<CronGetResult>()
		{
			public override IEnumerable<object> GetParams()
			{
				yield return 0;
			}
		}

	}

	public record TaggedCommand( int Id, string Method, IEnumerable<object> Params )
	{
		private JsonSerializerOptions Options { get; } =
			new() { PropertyNamingPolicy = new SnakeCaseNamingPolicy() };

		public virtual string Serialize() =>
			JsonSerializer.Serialize( this, Options );
	}

	public abstract record Command
	{
		public string Method => SnakeCaseNamingPolicy.Convert( GetType().Name );
		public abstract IEnumerable<object> GetParams();// { get; }

		public TaggedCommand Tag( int Id ) => new( Id, Method, GetParams() );
	}

	public abstract record Command<TResult> : Command;
}
