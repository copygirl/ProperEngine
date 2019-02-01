using System;
using System.Collections.Generic;

namespace ProperEngine.ES
{
	public class GenericAccessor<TEntity>
			: IESAccessor<TEntity>
		where TEntity : struct, IEntity
	{
		private readonly Dictionary<Type, IComponentMap> _maps
			= new Dictionary<Type, IComponentMap>();
		
		public void AddComponentMap(IComponentMap map)
		{
			if (map == null) throw new ArgumentNullException(nameof(map));
			if (_maps.ContainsKey(map.ComponentType)) throw new InvalidOperationException(
				$"Already has a map of type '{ map.ComponentType }'");
			_maps.Add(map.ComponentType, map);
		}
		
		// IESAccessor<> implementation
		
		public IComponentMap<TEntity, TComponent> Component<TComponent>()
			where TComponent : IComponent
		{
			if (!_maps.TryGetValue(typeof(TComponent), out var map)) {
				map = new HashComponentMap<TEntity, TComponent>(this);
				_maps.Add(typeof(TComponent), map);
			}
			return (IComponentMap<TEntity, TComponent>)map;
		}
		
		public IEntityRef<TEntity> Entity(TEntity entity)
			=> new EntityRef(this, entity);
		
		// IESAccessor implementation
		
		public IComponentMap Component(Type componentType)
		{
			if (componentType == null) throw new ArgumentNullException(nameof(componentType));
			if (!_maps.TryGetValue(componentType, out var map)) {
				// Assuming this is a semi-expensive check, so it's inside the if statement.
				if (!typeof(IComponent).IsAssignableFrom(componentType)) throw new ArgumentException(
					$"Type '{ componentType }' is not an IComponent", nameof(componentType));
				
				var mapType = typeof(HashComponentMap<,>).MakeGenericType(typeof(TEntity), componentType);
				map = (IComponentMap)Activator.CreateInstance(mapType, this);
				_maps.Add(componentType, map);
			}
			return map;
		}
		
		public IEntityRef Entity(IEntity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));
			if (!(entity is TEntity e)) throw new ArgumentException(
				$"Entity is type '{ entity.GetType() }', but expected '{ typeof(TEntity) }'", nameof(entity));
			return Entity(e);
		}
		
		
		private class EntityRef
			: IEntityRef<TEntity>
		{
			public EntityRef(IESAccessor<TEntity> accessor, TEntity entity)
			{
				if (accessor == null) throw new ArgumentNullException(nameof(accessor));
				Entity   = entity;
				Accessor = accessor;
			}
			
			// IEntityRef<> implementation
			
			public IESAccessor<TEntity> Accessor { get; }
			
			public TEntity Entity { get; }
			
			public ref TComponent TryGetRef<TComponent>(out bool success)
				where TComponent : IComponent
					=> ref Accessor.Component<TComponent>()
					               .TryGetRef(Entity, out success);
			
			public ref TComponent GetOrCreateRef<TComponent>(out bool exists)
				where TComponent : IComponent
					=> ref Accessor.Component<TComponent>()
					               .GetOrCreateRef(Entity, out exists);
			
			public ref TComponent TryRemoveRef<TComponent>(out bool success)
				where TComponent : IComponent
					=> ref Accessor.Component<TComponent>()
					               .TryRemoveRef(Entity, out success);
			
			// IEntityRef implementation
			
			IESAccessor IEntityRef.Accessor => Accessor;
			
			IEntity IEntityRef.Entity => Entity;
			
			public IComponent Get(Type componentType)
				=> Accessor.Component(componentType).Get(Entity);
			
			public IComponent Set(Type componentType, IComponent value)
				=> Accessor.Component(componentType).Set(Entity, value);
			
			public IComponent Remove(Type componentType)
				=> Accessor.Component(componentType).Remove(Entity);
		}
	}
}
