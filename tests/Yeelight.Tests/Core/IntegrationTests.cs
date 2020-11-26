﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class IntegrationTests : IClassFixture<IntegrationTestFixture>
	{
		private ISet<IPAddress> Disabled { get; }
		private bool IsConfigured { get; }
		private IDeviceCatalog DeviceCatalog { get; }
		private ICommandService CommandService { get; }
		private ITestOutputHelper Output { get; }

		public IntegrationTests(
			IntegrationTestFixture fixture,
			ITestOutputHelper output )
		{
			Disabled =
				new HashSet<IPAddress>(
					fixture.GetService<IOptions<DeviceOptions>>().Value.Disabled
						.Select( IPAddress.Parse ) );

			var x = fixture.GetService<IOptions<NetworkClientOptions>>();

			IsConfigured = fixture.IsConfigured;
			DeviceCatalog = fixture.GetService<IDeviceCatalog>();
			CommandService = fixture.GetService<ICommandService>();
			Output = output;
		}

		[Fact]
		public void Can_find_devices()
		{
			if ( IsConfigured == false )
			{
				return;
			}

			DeviceCatalog.Count.Should().BeGreaterThan( 0 );

			foreach ( var device in DeviceCatalog )
			{
				Output.WriteLine( device.Endpoint.ToString() );
			}
		}

		private async Task Test<T>(
			Command<T> command,
			Action<ICollection<T>> test )
		{
			if ( IsConfigured == false )
			{
				return;
			}

			static CancellationToken GetToken() =>
				Debugger.IsAttached
					? CancellationToken.None
					: new CancellationTokenSource( 1000 ).Token;

			var success = false;
			foreach ( var device in DeviceCatalog
				.Where( x => Disabled.Contains( x.Endpoint.Address ) == false ) )
			{
				try
				{
					var result =
						await CommandService.SendAsync(
							device,
							command,
							GetToken() );

					test( result );
					success = true;
					Output.WriteLine( $"{command} sent successfully to {device.Endpoint} ({device.Model})" );
					break;
				}
				catch ( TaskCanceledException )
				{
					Output.WriteLine(
						$"{command.GetType().Name} timed out -- {device.Endpoint} ({device.Model}) "
						+ $"probably needs to be restarted." );
				}
				catch ( CommandNotSupportedException )
				{
					Output.WriteLine(
						$"{command.GetType().Name} not supported by {device.Endpoint} ({device.Model}) " );
				}
			}

			success.Should().BeTrue( "otherwise all found devices failed to respond" );
		}

		[Fact]
		public async Task Can_get_prop() =>
			await Test(
				new Commands.GetProp( "power" ),
				x => x.Should().ContainSingle( x => x == "on" || x == "off" ) );

		[Fact]
		public async Task Can_get_cron() =>
			await Test(
				new Commands.CronGet(),
				x => x.Should().NotBeNull() );

		[Fact]
		public async Task Set_ct_abx_is_validated() =>
			await Assert.ThrowsAsync<CommandNotValidException>(
				() => Test(
					new Commands.SetCtAbx( 0, Effect.Smooth, TimeSpan.FromMilliseconds( 100 ) ),
					x => { } ) );

		[Fact]
		public async Task Can_set_ct_abx() =>
			await Test(
				new Commands.SetCtAbx( 2000, Effect.Smooth, TimeSpan.FromMilliseconds( 30 ) ),
				x => { } );
	}
}