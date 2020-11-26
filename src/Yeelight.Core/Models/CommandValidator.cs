using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Yeelight.Core
{
	//TODO: fix namespace
	public class CommandValidator : ICommandValidator
	{
		private ConcurrentDictionary<Type, ICollection<ICommandValidationRule>> ValidationRules { get; } = new();

		public CommandValidator( IEnumerable<ICommandValidationRule> validationRules )
		{
			foreach ( var rule in validationRules )
			{
				ValidationRules.GetOrAdd( rule.CommandType, new List<ICommandValidationRule>() ).Add( rule );
			}
		}

		public bool Validate( Command command, out IEnumerable<string> errorMessages )
		{
			if ( ValidationRules.TryGetValue( command.GetType(), out var rules ) == false )
			{
				errorMessages = Enumerable.Empty<string>();
				return true;
			}

			var messages = new List<string>();
			foreach ( var rule in rules )
			{
				if ( rule.Validate( command ) == false )
				{
					messages.Add( rule.ErrorMessage );
				}
			}

			errorMessages = messages;
			return messages.Any() == false;
		}
	}
}
