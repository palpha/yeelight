using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;

namespace Yeelight.Core
{
	public record Target( Uri Location, string Support )
	{
		private ISet<string> SupportedCommands { get; } =
			new HashSet<string>( Support.Split( " " ) );

		public IPEndPoint Endpoint { get; } =
			IPEndPoint.Parse( Location.Authority );

		public bool Supports( string command ) =>
			SupportedCommands.Contains( command );

		public Target( IPEndPoint endpoint, string support )
			: this( new Uri( $"yeelight://{endpoint}" ), support )
		{
		}
	}

	public record Device(
		Uri Location,
		uint Id,
		string Model,
		string Support,
		bool PoweredOn,
		ushort Brightness,
		ColorMode ColorMode,
		ushort Temperature,
		Color Color,
		ushort Hue,
		ushort Saturation,
		string Name,
		ushort MaxAge ) : Target( Location, Support )
	{
		public static Color ParseColor( uint colorInt ) =>
			ColorTranslator.FromHtml( $"#{colorInt:X2}" );

		public static uint ParseId( string idStr ) =>
			uint.Parse(
				idStr.Replace( "0x", string.Empty ),
				System.Globalization.NumberStyles.HexNumber );

		public static Device Parse( string deviceMessage )
		{
			var matches =
				Regex.Match(
					deviceMessage,
					@"
						^(NOTIFY\s\*\s)?HTTP/1\.1.+
						Cache-Control:\smax-age=(?<MaxAge>\d+).+
						Location:\s(?<Location>yeelight://.+?:.+?)\r?\n.+
						id:\s(?<Id>0x[\da-f]+).+
						model:\s(?<Model>.+?)\r?\n.+
						support:\s(?<Support>.+)\r?\n
						power:\s(?<PoweredOn>on|off).+
						bright:\s(?<Brightness>\d+).+
						color_mode:\s(?<ColorMode>\d).+
						ct:\s(?<Temperature>\d+).+
						rgb:\s(?<Color>\d+).+
						hue:\s(?<Hue>\d+).+
						sat:\s(?<Saturation>\d+).+
						name:\s(?<Name>.*?)(?:\r?\n|$)
					",
					RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace );

			if ( matches.Success == false )
			{
				throw new ArgumentException( "Could not parse message", nameof( deviceMessage ) );
			}

			string GetMatch<T>( Expression<Func<Device, T>> expr )
			{
				var name = expr.Body is MemberExpression memberExpr ? memberExpr.Member.Name : throw new InvalidOperationException();
				if ( matches.Groups.ContainsKey( name ) )
				{
					return matches.Groups[name].Value;
				}
				else
				{
					throw new InvalidOperationException( $"Unable to parse value for {name}" );
				}
			}

			return new Device(
				Location: new Uri( GetMatch( x => x.Location ) ),
				Id: ParseId( GetMatch( x => x.Id ) ),
				Model: GetMatch( x => x.Model ),
				Support: GetMatch( x => x.Support ),
				PoweredOn: GetMatch( x => x.PoweredOn ) == "on",
				Brightness: ushort.Parse( GetMatch( x => x.Brightness ) ),
				ColorMode: Enum.Parse<ColorMode>( GetMatch( x => x.ColorMode ) ),
				Temperature: ushort.Parse( GetMatch( x => x.Temperature ) ),
				Color: ParseColor( uint.Parse( GetMatch( x => x.Color ) ) ),
				Hue: ushort.Parse( GetMatch( x => x.Hue ) ),
				Saturation: ushort.Parse( GetMatch( x => x.Saturation ) ),
				Name: GetMatch( x => x.Name ),
				MaxAge: ushort.Parse( GetMatch( x => x.MaxAge ) ) );
		}
	}
}
