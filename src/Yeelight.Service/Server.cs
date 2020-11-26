namespace Yeelight.Service
{
	public class Server
	{
		// incoming request handler for integrations
		/*
				public void Start( IPEndPoint publicEndpoint, Action<string> bodyWriter, CancellationToken token )
		{
			HttpListener.Prefixes.Add( $"http://{publicEndpoint.Address}:{publicEndpoint.Port}/" );
			Logger.LogInformation( $"Listening to {publicEndpoint}" );
			PublicEndpoint = publicEndpoint;
			HttpListener.Start();

			_ = Task.Run( async () =>
			{
				while ( true )
				{
					var ctx = await HttpListener.GetContextAsync();
					bodyWriter( ctx.Request.RemoteEndPoint.ToString() );
					foreach ( var key in ctx.Request.Headers.AllKeys )
					{
						var val = ctx.Request.Headers[key];
						bodyWriter( $"{key}: {val}" );
					}

					var encoding = ctx.Request.ContentEncoding;
					var str = new StreamReader( ctx.Request.InputStream, encoding );
					var body = await str.ReadToEndAsync();
					bodyWriter( body );
					ctx.Request.InputStream.Close();
					str.Close();
					ctx.Response.StatusCode = 200;
					ctx.Response.Close();
				}
			}, token );
		}
		 */
	}
}
