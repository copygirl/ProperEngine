using System;
using System.Linq;
using Xunit;
using ProperEngine.ES;
using ProperEngine.ES.Collections;

namespace ProperEngine.Tests
{
	using EntityID = EntityID<UInt32>;

	public class GeneralAccessor_Tests
	{
		public GeneralAccessor<EntityID> Accessor { get; }
			= new GeneralAccessor<EntityID>();
		
		public EntityID EntityNone { get; } = new EntityID(0);
		public EntityID EntitySome { get; } = new EntityID(1);
		
		[Fact]
		public void Basic()
		{
			var names     = Accessor.Component<Name>();
			var positions = Accessor.Component<Position?>();
			
			Assert.Throws<ArgumentException>(() => Accessor.Component<Position>());
		}
		
		[Fact]
		public void Raw()
		{
			var rawPositions = Accessor.RawComponent<Position>();
			var positions    = Accessor.Component<Position?>();
			
			rawPositions.GetOrCreateRef(EntitySome, out _) = new Position(100, 100);
			
			Assert.Null(positions.Get(EntityNone));
			Assert.Equal(new Position(100, 100), rawPositions.GetOrCreateRef(EntitySome, out _));
			Assert.Equal(new Position(100, 100), positions.Get(EntitySome));
		}
		
		[Fact]
		public void NonGeneric()
		{
			var names     = Accessor.Component(typeof(Name));
			var positions = Accessor.Component(typeof(Position));
			
			names    .Set(EntitySome, new Name("Friend?"));
			positions.Set(EntitySome, new Position(100, 100));
			
			Assert.Null(names    .Get(EntityNone));
			Assert.Null(positions.Get(EntityNone));
			
			Assert.Equal(new Name("Friend?")   , names    .Get(EntitySome));
			Assert.Equal(new Position(100, 100), positions.Get(EntitySome));
			
			var pos    = Accessor.Component<Position?>().Get(EntitySome);
			var rawPos = Accessor.RawComponent<Position>().TryGetRef(EntitySome, out _);
			Assert.Equal(new Position(100, 100), pos   );
			Assert.Equal(new Position(100, 100), rawPos);
			
			Assert.Throws<ArgumentException>(() => Accessor.Component(typeof(Position?)));
		}
	}
}
