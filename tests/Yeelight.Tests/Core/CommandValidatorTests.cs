using System;
using FluentAssertions;
using Xunit;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class CommandValidatorTests
	{
		public record SomeCommand( string Foo, string Bar ) : Command<string>();

		public class FoobarValidator : ICommandValidationRule
		{
			public string ErrorMessage => "Is not foobar";
			public Type CommandType => typeof( SomeCommand );

			public bool Validate( Command command ) =>
				command is SomeCommand someCommand && someCommand.Foo == "foobar";
		}

		public class BarbazValidator : ICommandValidationRule
		{
			public string ErrorMessage => "Is not barbaz";
			public Type CommandType => typeof( SomeCommand );

			public bool Validate( Command command ) =>
				command is SomeCommand someCommand && someCommand.Bar == "barbaz";
		}

		[Fact]
		public void Can_validate_according_to_rules()
		{
			var validator = new CommandValidator( new[] { new FoobarValidator() } );

			var result1 = validator.Validate( new SomeCommand( "foobar", "" ), out var messages1 );
			var result2 = validator.Validate( new SomeCommand( "barbaz", "" ), out var messages2 );

			result1.Should().BeTrue();
			messages1.Should().BeEmpty();
			result2.Should().BeFalse();
			messages2.Should().ContainSingle( "Is not foobar" );
		}

		[Fact]
		public void Can_validate_against_multiple_rules()
		{
			var validator =
				new CommandValidator(
					new ICommandValidationRule[]
					{
						new FoobarValidator(),
						new BarbazValidator()
					} );


			var result1 = validator.Validate( new SomeCommand( "foobar", "" ), out var messages1 );
			var result2 = validator.Validate( new SomeCommand( "foobar", "barbaz" ), out var messages2 );

			result1.Should().BeFalse();
			messages1.Should().ContainSingle( "Is not barbaz" );
			result2.Should().BeTrue();
			messages2.Should().BeEmpty();
		}
	}
}