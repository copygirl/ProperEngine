using System;
using System.Linq;
using Xunit;
using ProperEngine.ES;
using ProperEngine.ES.Raw.Collections;

namespace ProperEngine.Tests
{
	using EntityID = EntityID<UInt32>;

	public class RawComponentMap_Tests
	{
		public RawComponentMap<EntityID, Position> Map { get; }
			= new RawComponentMap<EntityID, Position>();
		
		public EntityID EntityNone { get; } = new EntityID(0);
		public EntityID EntitySome { get; } = new EntityID(1);
		
		[Fact]
		public void Basic()
		{
			Assert.Empty(Map);
			bool found;
			
			ref var newlyCreated = ref Map.GetOrCreateRef(EntitySome, out found);
			Assert.Single(Map);
			Assert.Equal(default(Position), newlyCreated);
			Assert.False(found);
			newlyCreated = new Position(100, 100);
			
			ref var nonExistent = ref Map.TryGetRef(EntityNone, out found);
			Assert.Single(Map);
			Assert.Equal(default(Position), nonExistent);
			Assert.False(found);
			// This should not have any effect, but it's valid nonetheless.
			nonExistent = new Position(100, 100);
			
			ref var testExistent = ref Map.GetOrCreateRef(EntitySome, out found);
			Assert.Single(Map);
			Assert.Equal(new Position(100, 100), testExistent);
			Assert.True(found);
			testExistent = new Position(200, 200);
			
			var previous = Map.TryRemoveRef(EntitySome, out found);
			Assert.Empty(Map);
			Assert.Equal(new Position(200, 200), previous);
			Assert.True(found);
			
			var removeNonExistent = Map.TryRemoveRef(EntitySome, out found);
			Assert.Empty(Map);
			Assert.Equal(default(Position), removeNonExistent);
			Assert.False(found);
		}
		
		
		[Fact]
		public void Enumeration()
		{
			Assert.Empty(Map);
			
			for (var i = 1; i <= 10; i++) {
				ref var component = ref Map.GetOrCreateRef(new EntityID((uint)i), out _);
				component = new Position(i * 100, i * -50);
			}
			
			Assert.Equal(10, Map.Count());
			Assert.Equal(55, Map.Sum(entry => (uint)entry.Key));
			Enumerable.Sum(Map, entry => (uint)entry.Key);
			
			foreach (var entry in Map) {
				var i = (int)(uint)entry.Key;
				Assert.Equal(i * 100, entry.Value.X);
				Assert.Equal(i * -50, entry.Value.Y);
				entry.Value = Position.ORIGIN;
			}
			Assert.Equal(0, Map.Sum(entry => entry.Value.X));
		}
	}
}
