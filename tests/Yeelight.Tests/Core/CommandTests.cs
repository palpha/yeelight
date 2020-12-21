using System;
using System.Drawing;
using FluentAssertions;
using Xunit;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class CommandTests
	{
		[Fact]
		public void Names_are_lowercase()
		{
			var result = new Commands.GetProp( "test" ).Tag( 0 ).Serialize();
			result.Should().Contain( "\"id\":" )
				.And.NotContain( "\"Id\":" );
		}

		[Fact]
		public void GetProp_has_the_right_method_and_props()
		{
			var command = new Commands.GetProp( "foo", "bar" );
			command.Method.Should().Be( "get_prop" );
			command.GetParams().Should().BeEquivalentTo( new[] { "bar", "foo" } );
			var serialized = command.Tag( 0 ).Serialize();
			serialized.Replace( " ", "" ).Should().Be(
				@"{""id"":0,""method"":""get_prop"",""params"":[""foo"",""bar""]}" );
		}

		[Theory]
		[InlineData( 1699, false )]
		[InlineData( 6501, false )]
		[InlineData( 1700, true )]
		[InlineData( 6500, true )]
		public void SetCtAbx_CtValueValidator_validates_temperature( int val, bool expected )
		{
			var rule = new Commands.SetCtAbx.CtValueRule();
			var command = new Commands.SetCtAbx( val, Effect.Smooth, TimeSpan.Zero );

			rule.Validate( command ).Should().Be( expected );
		}

		[Theory]
		[InlineData( -1, false )]
		[InlineData( 0, false )]
		[InlineData( 29, false )]
		[InlineData( 30, true )]
		[InlineData( 30000, true )]
		[InlineData( 30001, false )]
		public void SetCtAbx_DurationValidator_validates_duration( int milliseconds, bool expected )
		{
			var rule = new Commands.SetCtAbx.SetCtAbxDurationRule();
			var command = new Commands.SetCtAbx( 0, Effect.Smooth, TimeSpan.FromMilliseconds( milliseconds ) );

			rule.Validate( command ).Should().Be( expected );
		}

		[Fact]
		public void SetCtAbx_DurationValidator_ignores_duration_if_sudden()
		{
			var rule = new Commands.SetCtAbx.SetCtAbxDurationRule();
			var command = new Commands.SetCtAbx( 0, Effect.Sudden, TimeSpan.Zero );

			rule.Validate( command ).Should().BeTrue();
		}

		[Fact]
		public void SetCtAbx_converts_parameters()
		{
			var command1 = new Commands.SetCtAbx( 123, Effect.Sudden, TimeSpan.FromSeconds( 1 ) );
			var command2 = new Commands.SetCtAbx( 123, Effect.Smooth, TimeSpan.FromSeconds( 2 ) );

			command1.Tag( 0 ).Params.Should().BeEquivalentTo( 123, "sudden", 1000 );
			command2.Tag( 0 ).Params.Should().BeEquivalentTo( 123, "smooth", 2000 );
		}

		[Fact]
		public void SetRgb_converts_parameters()
		{
			var command = new Commands.SetRgb( Color.FromArgb( 255, 0, 0 ), Effect.Smooth, TimeSpan.FromSeconds( 1 ) );

			command.Tag( 0 ).Params.Should().BeEquivalentTo( 0xFF0000, "smooth", 1000 );
		}

		[Fact]
		public void SetHsv_uses_color()
		{
			var command = new Commands.SetHsv( Color.FromKnownColor( KnownColor.Azure ), Effect.Smooth, TimeSpan.FromSeconds( 1 ) );

			command.GetParams().Should().BeEquivalentTo( 180, 1, Effect.Smooth, TimeSpan.FromSeconds( 1 ) );
		}

		[Theory]
		[InlineData( 0, 0, 0, 100, true, true, true )]
		[InlineData( 5, 5, 5, 100, true, true, true )]
		[InlineData( 5, 5, 5, 20, false, true, true )]
		[InlineData( 255, 255, 255, 100, true, true, true )]
		// Color-based = can't represent invalid HSV?
		public void SetHsv_validations_work(
			ushort r, ushort g, ushort b, int ms,
			bool expectedRule1, bool expectedRule2, bool expectedRule3 )
		{
			var command =
				new Commands.SetHsv(
					Color.FromArgb( r, g, b ),
					Effect.Smooth,
					TimeSpan.FromMilliseconds( ms ) );

			var rule1 = new Commands.SetHsv.SetHsvDurationRule();
			var rule2 = new Commands.SetHsv.HueRule();
			var rule3 = new Commands.SetHsv.SaturationRule();

			rule1.Validate( command ).Should().Be( expectedRule1, "duration" );
			rule2.Validate( command ).Should().Be( expectedRule2, "hue" );
			rule3.Validate( command ).Should().Be( expectedRule3, "saturation" );
		}
	}
}
