using System;

namespace Yeelight.Core
{
	public class NetworkClientOptions
	{
		public string PublicEndpoint { get; init; } = string.Empty;
	}

	public class DeviceOptions
	{
		public string[] Disabled { get; init; } = Array.Empty<string>();
	}
}
