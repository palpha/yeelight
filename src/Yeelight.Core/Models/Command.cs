using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Yeelight.Core
{
	public enum Effect { Sudden, Smooth };

	public record CronGetResult( int Type, int Delay, int Mix );

	public static class Commands
	{
		/// <summary>
		/// Retrieve current property of a smart LED.
		/// </summary>
		public record GetProp( params string[] Props ) : Command<string>( Props );

		/// <summary>
		/// Change the color temperature of a smart LED.
		/// </summary>
		public record SetCtAbx( int CtValue, Effect Effect, TimeSpan Duration )
			: Command<string>( CtValue, Effect, Duration )
		{
			public record CtValueRule() : CommandValidationRule<SetCtAbx>(
				x => x.CtValue >= 1700 && x.CtValue <= 6500,
				"Temperature must be between 1700 and 6500" );

			public record DurationRule() : CommandValidationRule<SetCtAbx>(
				x => x.Duration.TotalMilliseconds >= 30 && x.Duration.TotalMilliseconds <= 30000,
				"Duration must be between 30 and 30000 milliseconds" );
		}

		/// <summary>
		/// Retrieves the current cron settings.
		/// </summary>
		public record CronGet() : Command<CronGetResult>( 0 );
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
		public IEnumerable<object> Params { get; }

		public Command( params object[] paramArr ) => Params = paramArr;

		public TaggedCommand Tag( int Id ) => new( Id, Method, Params );
	}

	public abstract record Command<TResult> : Command
	{
		public Command( params object[] paramArr ) : base( paramArr ) { }
	}
}
