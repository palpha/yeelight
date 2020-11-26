using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Yeelight.Core
{
	public class DeviceCatalog : IObserver<Device>, IDeviceCatalog
	{
		private IDictionary<uint, Device> DevicesById { get; } =
			new ConcurrentDictionary<uint, Device>();

		public int Count => DevicesById.Count;

		private IDisposable MonitorSubscription { get; }
		private ILogger<DeviceCatalog> Logger { get; }

		public DeviceCatalog(
			IObservable<Device> deviceStream,
			ILogger<DeviceCatalog> logger )
		{
			MonitorSubscription = deviceStream.Subscribe( this );
			Logger = logger;
		}

		public void OnCompleted() =>
			MonitorSubscription.Dispose();

		public void OnError( Exception ex ) =>
			Logger.LogError( ex, "Error in stream of devices" );

		public void OnNext( Device device ) =>
			DevicesById[device.Id] = device;

		public IEnumerator<Device> GetEnumerator() =>
			DevicesById.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();
	}
}
