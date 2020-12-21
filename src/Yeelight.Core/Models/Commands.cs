using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Yeelight.Core
{
	public record CronGetResult( int Type, int Delay, int Mix );

	public static class Commands
	{
		/// <summary>
		/// Retrieves current property of a smart LED.
		/// </summary>
		public record GetProp( params string[] Props ) : Command<string>
		{
			public override IEnumerable<object> GetParams() => Props;

			public record PropsRule() : CommandValidationRule<GetProp>(
				x => x.Props.Any(),
				"At least one property must be queried" );
		}

		/// <summary>
		/// Changes the color temperature of a smart LED.
		/// </summary>
		public record SetCtAbx( int CtValue, Effect Effect, TimeSpan Duration )
			: Command<string>, IDurational
		{
			public override IEnumerable<object> GetParams()
			{
				yield return CtValue;
				yield return Effect;
				yield return Duration;
			}

			public record CtValueRule() : CommandValidationRule<SetCtAbx>(
				x => x.CtValue >= 1700 && x.CtValue <= 6500,
				"Temperature must be between 1700 and 6500" );

			public record SetCtAbxDurationRule : DurationRule<SetCtAbx>;
		}

		/// <summary>
		/// Sets the color of a smart LED to an RGB value.
		/// </summary>
		public record SetRgb( Color Color, Effect Effect, TimeSpan Duration )
			: Command<string>, IDurational
		{
			public override IEnumerable<object> GetParams()
			{
				yield return Color.ToUInt();
				yield return Effect;
				yield return Duration;
			}

			public record SetRgbDurationRule : DurationRule<SetRgb>;
		}

		/// <summary>
		/// Retrieves the current cron settings.
		/// </summary>
		public record CronGet : Command<CronGetResult>
		{
			public override IEnumerable<object> GetParams()
			{
				yield return 0;
			}
		}

		/// <summary>
		/// Sets the color of a smart LED to an HSV value.
		/// </summary>
		public record SetHsv( Color Color, Effect Effect, TimeSpan Duration )
			: Command<CronGetResult>, IDurational
		{
			public override IEnumerable<object> GetParams()
			{
				yield return Color.GetHue();
				yield return Color.GetSaturation();
				yield return Effect;
				yield return Duration;
			}

			public record SetHsvDurationRule : DurationRule<SetHsv>;
			public record HueRule() : CommandValidationRule<SetHsv>(
				x => x.Color.GetHue().InRange( 0, 359 ),
				"Hue must be between 0 and 359" );
			public record SaturationRule() : CommandValidationRule<SetHsv>(
				x => x.Color.GetSaturation().InRange( 0, 100 ),
				"Saturation must be between 0 and 100" );
		}
	}
}
