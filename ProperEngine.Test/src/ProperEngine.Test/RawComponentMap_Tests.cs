using System;
using System.Linq;
using Xunit;
using ProperEngine.ES;
using ProperEngine.ES.Raw.Collections;
using ProperEngine.ES.Raw;

namespace ProperEngine.Test
{
	using EntityID = EntityID<UInt32>;
	
	public class RawComponentMap_Tests
	{
		public IRawComponentMap<EntityID, Position> Map { get; }
			= new HashComponentMap<EntityID, Position>(null);
		
		public EntityID EntityNone { get; } = new EntityID(0);
		public EntityID EntitySome { get; } = new EntityID(1);
		
		[Fact]
		public void Basic()
		{
			Assert.Equal(0, Map.Count);
			bool found;
			
			ref var newlyCreated = ref Map.GetOrCreateRef(EntitySome, out found);
			Assert.Equal(1, Map.Count);
			Assert.Equal(default(Position), newlyCreated);
			Assert.False(found);
			newlyCreated = new Position(100, 100);
			
			ref var nonExistent = ref Map.TryGetRef(EntityNone, out found);
			Assert.Equal(1, Map.Count);
			Assert.Equal(default(Position), nonExistent);
			Assert.False(found);
			// This should not have any effect, but it's valid nonetheless.
			nonExistent = new Position(100, 100);
			
			ref var testExistent = ref Map.GetOrCreateRef(EntitySome, out found);
			Assert.Equal(1, Map.Count);
			Assert.Equal(new Position(100, 100), testExistent);
			Assert.True(found);
			testExistent = new Position(200, 200);
			
			var previous = Map.TryRemoveRef(EntitySome, out found);
			Assert.Equal(0, Map.Count);
			Assert.Equal(new Position(200, 200), previous);
			Assert.True(found);
			
			var removeNonExistent = Map.TryRemoveRef(EntitySome, out found);
			Assert.Equal(0, Map.Count);
			Assert.Equal(default(Position), removeNonExistent);
			Assert.False(found);
		}
		
		[Fact]
		public void Basic_As_IComponentMap()
		{
			var map = (IComponentMap)Map;
			
			Assert.Equal(typeof(EntityID), map.KeyType);
			Assert.Equal(typeof(Position), map.ComponentType);
			
			var beforeCreated = map.Set(EntitySome, new Position(100, 100));
			Assert.Null(beforeCreated);
			
			var nonExistent = map.Get(EntityNone);
			Assert.Null(nonExistent);
			
			var beforeChanged = map.Set(EntitySome, new Position(200, 200));
			Assert.Equal(new Position(100, 100), beforeChanged);
			
			var beforeRemoved = map.Remove(EntitySome);
			Assert.Equal(new Position(200, 200), beforeRemoved);
			
			var removeNonExistent = map.Remove(EntitySome);
			Assert.Null(removeNonExistent);
		}
		
		[Fact]
		public void Exceptions_As_IComponentMap()
		{
			var map = (IComponentMap)Map;
			
			Assert.Throws<ArgumentNullException>(() => map.Get(null));
			Assert.Throws<ArgumentNullException>(() => map.Set(null, default(Position)));
			Assert.Throws<ArgumentNullException>(() => map.Set(EntitySome, null));
			Assert.Throws<ArgumentNullException>(() => map.Remove(null));
			
			Assert.Throws<ArgumentException>(() => map.Get(new EntityID<byte>()));
			Assert.Throws<ArgumentException>(() => map.Set(EntitySome, new Name("Error")));
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
			Assert.Equal(55, Map.Sum(entry => entry.Key.Value));
			Enumerable.Sum(Map, entry => entry.Key.Value);
			
			foreach (var entry in Map) {
				var i = (int)entry.Key.Value;
				Assert.Equal(i * 100, entry.Value.X);
				Assert.Equal(i * -50, entry.Value.Y);
				entry.Value = Position.ORIGIN;
			}
			Assert.Equal(0, Map.Sum(entry => entry.Value.X));
		}
	}
}
