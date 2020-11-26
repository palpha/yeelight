using System.Linq;
using System.Text.Json;

namespace Yeelight.Core
{
	//TODO: fix namespace

	public class SnakeCaseNamingPolicy : JsonNamingPolicy
	{
		public override string ConvertName( string name ) => Convert( name );

		public static string Convert( string name ) =>
			string.Concat(
				name.Select( ( x, i ) =>
					i > 0 && char.IsUpper( x )
						? "_" + x.ToString()
						: x.ToString() ) )
				.ToLower();
	}
}
