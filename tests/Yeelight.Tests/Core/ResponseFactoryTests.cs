using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Yeelight.Core;

namespace Yeelight.Tests.Core
{
	public class ResponseFactoryTests
	{
		[Fact]
		public void Ids_are_allocated_quickly()
		{
			var stream = new Subject<Response>();
			var factory = new ResponseFactory( stream, NullLogger<ResponseFactory>.Instance );
			var sw = Stopwatch.StartNew();

			for ( var i = 0; i < 10000; i++ )
			{
				factory.Create<Response<int>>();
			}

			var ms = sw.Elapsed.TotalMilliseconds;
			(ms / 10000).Should().BeLessThan( 0.1 );
		}

		[Fact]
		public void Ids_are_created_without_apparent_collisions()
		{
			var stream = new Subject<Response>();
			var factory = new ResponseFactory( stream, NullLogger<ResponseFactory>.Instance );
			var ids = new HashSet<int>();
			bool result = true;

			for ( var i = 0;
				i < 100000 && result;
				i++, result = ids.Add( factory.Create<Response<int>>().Id ) ) ;

			result.Should().BeTrue( "collision if already in set" );
		}

		[Fact]
		public void Promises_are_kept()
		{
			var stream = new Subject<Response>();
			var factory = new ResponseFactory( stream, NullLogger<ResponseFactory>.Instance );

			var future1 = factory.Create<int>();
			var future2 = factory.Create<int>();
			var future3 = factory.Create<int>();

			stream.OnNext( new Response<int>( future1.Id, new[] { 123 } ) );
			stream.OnNext( new Response<int>( future2.Id, new[] { 234, 345 } ) );

			future1.Task.IsCompleted.Should().BeTrue();
			future1.Task.Result.Result.Should().BeEquivalentTo( new[] { 123 } );

			future2.Task.IsCompleted.Should().BeTrue();
			future2.Task.Result.Result.Should().BeEquivalentTo( new[] { 234, 345 } );

			future3.Task.IsCompleted.Should().BeFalse();
		}

		[Fact]
		public void Receiving_errors_should_trigger_task_failure()
		{
			var stream = new Subject<Response>();
			var factory = new ResponseFactory( stream, NullLogger<ResponseFactory>.Instance );

			var future = factory.Create<int>();

			stream.OnNext( new ErrorResponse( future.Id, new( 234, "Foobar" ) ) );

			future.Task.IsFaulted.Should().BeTrue();
			var exception = future.Task.Exception?.InnerException;
			exception.Should().NotBeNull()
				.And.BeOfType<YeelightException>()
				.Which.Message.Should().Be( "Foobar" );
		}

		[Fact]
		public void Returns_null_deserializer_when_unknown_id()
		{
			var factory =
				new ResponseFactory(
					Observable.Empty<Response>(),
					NullLogger<ResponseFactory>.Instance );

			var result = factory.TryGetDeserializer( 0 );

			result.Should().BeNull();
		}
	}
}
