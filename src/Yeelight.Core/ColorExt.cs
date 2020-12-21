using System;
using System.Drawing;

namespace Yeelight.Core
{
	public static class ColorExt
	{
		public static Color ToColor( this uint colorValue )
		{
			Span<byte> bytes = stackalloc byte[4];

			if ( BitConverter.TryWriteBytes( bytes, colorValue ) == false )
			{
				throw new InvalidColorValueException( colorValue );
			}

			if ( BitConverter.IsLittleEndian )
			{
				bytes.Reverse();
			}

			return Color.FromArgb( bytes[1], bytes[2], bytes[3] );
		}

		public static uint ToUInt( this Color color )
		{
			Span<byte> bytes = stackalloc byte[] { 0, color.R, color.G, color.B };

			if ( BitConverter.IsLittleEndian )
			{
				bytes.Reverse();
			}

			return BitConverter.ToUInt32( bytes );
		}
	}
}
