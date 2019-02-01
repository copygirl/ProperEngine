using System;

namespace ProperEngine.ES
{
	public static class ComponentMapExtensions
	{
		public static ref TComponent GetOrCreateRef<TEntity, TComponent>(
				this IComponentMap<TEntity, TComponent> map, TEntity entity)
			where TEntity    : struct, IEntity
			where TComponent : IComponent
				=> ref map.GetOrCreateRef(entity, out _);
		// Without this we get errors CS8156 and CS8347.
		private static bool _;
		
		
		public static void Add<TEntity, TComponent>(
				this IComponentMap<TEntity, TComponent> map,
				TEntity entity, TComponent value)
			where TEntity    : struct, IEntity
			where TComponent : IComponent
		{
			ref var component = ref map.GetOrCreateRef(entity, out var exists);
			if (exists) throw new ArgumentException(
				$"Entity '{ entity }' already component '{ typeof(TComponent) }'");
			component = value;
		}
		
		public static TComponent Remove<TEntity, TComponent>(
				this IComponentMap<TEntity, TComponent> map, TEntity entity)
			where TEntity    : struct, IEntity
			where TComponent : IComponent
		{
			ref var component = ref map.TryRemoveRef(entity, out var success);
			if (!success) throw new ArgumentException(
				$"Entity '{ entity }' doesn't have a component '{ typeof(TComponent) }'");
			return component;
		}
		
		
		/*
		public static TComponent? Get<TEntity, TComponent>(
				this IComponentMap<TEntity, TComponent> map, TEntity entity)
			where TEntity    : struct, IEntity
			where TComponent : struct, IComponent
		{
			ref var component = ref map.TryGetRef(entity, out var success);
			return (success ? component : (TComponent?)null);
		}
		
		public static TComponent? Set<TEntity, TComponent>(
				this IComponentMap<TEntity, TComponent> map,
				TEntity entity, TComponent value)
			where TEntity    : struct, IEntity
			where TComponent : struct, IComponent
		{
			ref var component = ref map.GetOrCreateRef(entity, out var exists);
			var result = (exists ? component : (TComponent?)null);
			component = value;
			return result;
		}
		
		public static TComponent? Set<TEntity, TComponent>(
				this IComponentMap<TEntity, TComponent> map,
				TEntity entity, TComponent? value)
			where TEntity    : struct, IEntity
			where TComponent : struct, IComponent
				=> (value.HasValue ? Set(map, entity, value.Value)
				                   : Remove(map, entity));
		*/
	}
}
