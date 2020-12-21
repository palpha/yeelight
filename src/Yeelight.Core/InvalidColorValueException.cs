using System;

namespace Yeelight.Core
{
	public class InvalidColorValueException : Exception
	{
		public InvalidColorValueException( uint colorValue )
			: base( $"Could not convert {colorValue} to Color" )
		{
		}
	}
}
