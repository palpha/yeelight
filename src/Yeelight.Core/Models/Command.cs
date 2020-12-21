using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Yeelight.Core
{
	public static class EffectExt
	{
		public static string ToLowerString( this Effect effect ) =>
			effect.ToString().ToLowerInvariant();
	}

	public interface IDurational
	{
		Effect Effect { get; }
		TimeSpan Duration { get; }
	}

	public enum Effect { Sudden, Smooth };

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
		public abstract IEnumerable<object> GetParams();

		public TaggedCommand Tag( int Id ) =>
			new(
				Id,
				Method,
				GetParams().Select( x => x switch
				{
					Effect effect => effect.ToLowerString(),
					TimeSpan timeSpan => timeSpan.TotalMilliseconds,
					_ => x
				} ) );
	}

	public abstract record Command<TResult> : Command;
}
