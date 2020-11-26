using System;
using System.Collections.Generic;

namespace Yeelight.Core
{
	public class MessagePublisher<T> : IMessagePublisher<T>, IObservable<T>
	{
		private class Unsubscriber : IDisposable
		{
			private IList<IObserver<T>> Observers { get; }

			private IObserver<T>? Observer { get; set; }

			public Unsubscriber(
				IList<IObserver<T>> observers,
				IObserver<T> observer )
			{
				Observers = observers;
				Observer = observer;
			}

			public void Dispose()
			{
				if ( Observer is not null )
				{
					Observers.Remove( Observer );
					Observer = null;
				}
			}
		}

		private IList<IObserver<T>> Observers { get; } = new List<IObserver<T>>();

		public IDisposable Subscribe( IObserver<T> observer )
		{
			if ( Observers.Contains( observer ) == false )
			{
				Observers.Add( observer );
			}

			return new Unsubscriber( Observers, observer );
		}

		public void Publish( T message )
		{
			foreach ( var observer in Observers )
			{
				observer.OnNext( message );
			}
		}
	}
}
