﻿using System;
using Xunit;
using Yeelight.Core;
using FluentAssertions;
using System.Net;
using System.Drawing;

namespace Yeelight.Tests.Core
{
	public class ColorExtTests
	{
		[Theory]
		[InlineData( 0x0000FF, 0, 0, 255 )]
		[InlineData( 0x00FFFF, 0, 255, 255 )]
		[InlineData( 0xFFFFFF, 255, 255, 255 )]
		public void CanConvertFromUInt( uint incoming, byte r, byte g, byte b )
		{
			var color = ColorExt.ToColor( incoming );
			(color.R, color.G, color.B).Should().Be( (r, g, b) );
		}

		[Theory]
		[InlineData( 0x0000FF, 0, 0, 255 )]
		[InlineData( 0x00FFFF, 0, 255, 255 )]
		[InlineData( 0xFFFFFF, 255, 255, 255 )]
		public void CanConvertToUInt( uint expected, byte r, byte g, byte b )
		{
			var color = Color.FromArgb( r, g, b );
			ColorExt.ToUInt( color ).Should().Be( expected );
		}
	}

	public class DeviceTests
	{
		[Fact]
		public void Can_parse_device_payloads()
		{
			static void Test( Device device )
			{
				device.Location.Should().Be( "yeelight://192.168.1.76:55443" );
				device.Endpoint.Should().Be( IPEndPoint.Parse( "192.168.1.76:55443" ) );
				device.Id.Should().Be( Device.ParseId( "0x0000000011748cfe" ) );
				device.Model.Should().Be( "color4" );
				device.Support.Should().StartWith( "get_prop set_default " );
				device.Supports( "set_default" ).Should().BeTrue();
				device.PoweredOn.Should().BeFalse();
				device.Brightness.Should().Be( 100 );
				device.ColorMode.Should().Be( ColorMode.ColorTemperature );
				device.Temperature.Should().Be( 3907 );
				device.Color.Should().Be( ((uint) 6399).ToColor() );
				((int) device.Color.GetHue()).Should().Be( 234 );
				((int) device.Color.GetSaturation() * 100).Should().Be( 100 );
				device.Hue.Should().Be( 234 );
				device.Saturation.Should().Be( 100 );
			}

			Test( Device.Parse( TestMessages.SEARCH_RESPONSE ) );
			Test( Device.Parse( TestMessages.ADVERTISEMENT ) );
		}

		[Fact]
		public void Can_parse_rgb_value()
		{
			var value = ((uint) 16777215).ToColor();
			value.R.Should().Be( 255 );
			value.G.Should().Be( 255 );
			value.B.Should().Be( 255 );
		}

		[Fact]
		public void Uri_can_handle_scheme()
		{
			const string TEST_URI = "yeelight://192.168.1.76:55443";
			var uri = new Uri( TEST_URI );
			uri.Scheme.Should().Be( "yeelight" );
			uri.Host.Should().Be( "192.168.1.76" );
			uri.Port.Should().Be( 55443 );
		}

		[Fact]
		public void Can_parse_id()
		{
			var value = Device.ParseId( "0x0000000011748cfe" );
			value.Should().Be( 0x0000000011748cfe );
		}
	}
}
