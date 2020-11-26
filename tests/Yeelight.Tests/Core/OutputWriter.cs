using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Yeelight.Tests.Core
{
	public class OutputWriter
	{
		private IMessageSink Output { get; }

		public OutputWriter( IMessageSink output ) => Output = output;

		public void WriteLine( string message, params object[] param )
		{
			Console.WriteLine( message, param );

			var sinkMessage = new DiagnosticMessage( message, param );
			Output.OnMessage( sinkMessage );
		}
	}
}
