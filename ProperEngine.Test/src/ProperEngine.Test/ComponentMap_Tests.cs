using System;
using System.Linq;
using Xunit;
using ProperEngine.ES;

namespace ProperEngine.Test
{
	using Entity = Entity<UInt32>;
	
	public class ComponentMap_Tests
	{
		[Fact]
		public void Basic()
		{
			var entityNone = new Entity(0);
			var entitySome = new Entity(1);
			var map = new HashComponentMap<Entity, Position>(null);
			
			bool found;
			
			ref var newlyCreated = ref map.GetOrCreateRef(entitySome, out found);
			Assert.Equal(default(Position), newlyCreated);
			Assert.False(found);
			newlyCreated = new Position(100, 100);
			
			ref var nonExistent = ref map.TryGetRef(entityNone, out found);
			Assert.Equal(default(Position), nonExistent);
			Assert.False(found);
			// This should not have any effect, but be valid nonetheless.
			nonExistent = new Position(100, 100);
			
			ref var testExistent = ref map.GetOrCreateRef(entitySome, out found);
			Assert.Equal(new Position(100, 100), testExistent);
			Assert.True(found);
			testExistent = new Position(200, 200);
			
			var previous = map.TryRemoveRef(entitySome, out found);
			Assert.Equal(new Position(200, 200), previous);
			Assert.True(found);
			
			var removeNonExistent = map.TryRemoveRef(entitySome, out found);
			Assert.Equal(default(Position), removeNonExistent);
			Assert.False(found);
		}
		
		[Fact]
		public void Basic_NonGeneric()
		{
			var entityNone = new Entity(0);
			var entitySome = new Entity(1);
			var map = (IComponentMap)new HashComponentMap<Entity, Position>(null);
			
			var beforeCreated = map.Set(entitySome, new Position(100, 100));
			Assert.Null(beforeCreated);
			
			var nonExistent = map.Get(entityNone);
			Assert.Null(nonExistent);
			
			var testExistent = map.Set(entitySome, new Position(200, 200));
			Assert.Equal(new Position(100, 100), testExistent);
			
			var previous = map.Remove(entitySome);
			Assert.Equal(new Position(200, 200), previous);
			
			var removeNonExistent = map.Remove(entitySome);
			Assert.Null(removeNonExistent);
		}
		
		[Fact]
		public void Enumeration()
		{
			var map = new HashComponentMap<Entity, Position>(null);
			Assert.Empty(map);
			
			for (var i = 1; i <= 10; i++)
				map.Add(new Entity((uint)i), new Position(i * 100, i * -50));
			Assert.Equal(10, map.Count);
			Assert.Equal(55, map.Sum(entry => entry.Entity.ID));
			
			foreach (var entry in map) {
				var i = (int)entry.Entity.ID;
				Assert.Equal(i * 100, entry.Component.X);
				Assert.Equal(i * -50, entry.Component.Y);
				entry.Component = Position.ORIGIN;
			}
			Assert.Equal(0, map.Sum(entry => entry.Component.X));
		}
	}
}
