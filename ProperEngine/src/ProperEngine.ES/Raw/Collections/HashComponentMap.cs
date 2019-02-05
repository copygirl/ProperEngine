using System;
using System.Collections;
using System.Collections.Generic;
using ProperEngine.Utility;

namespace ProperEngine.ES.Raw.Collections
{
	public class HashComponentMap<TEntity, TComponent>
			: IRawComponentMap<TEntity, TComponent>
		where TEntity    : struct, IEntity
		where TComponent : struct, IComponent
	{
		private readonly RefDictionary<TEntity, TComponent> _dict
			= new RefDictionary<TEntity, TComponent>();
		
		public HashComponentMap(IRawAccessor<TEntity> accessor)
			=> Accessor = accessor;
		
		
		// IRawComponentMap<,> implementation
		
		public IRawAccessor<TEntity> Accessor { get; }
		
		public ref TComponent TryGetRef(TEntity entity, out bool success)
			=> ref _dict.TryGetEntry(GetBehavior.Default, entity, out success);
		
		public ref TComponent GetOrCreateRef(TEntity entity, out bool exists)
			=> ref _dict.TryGetEntry(GetBehavior.Create, entity, out exists);
		
		public ref TComponent TryRemoveRef(TEntity entity, out bool success)
			=> ref _dict.TryGetEntry(GetBehavior.Remove, entity, out success);
		
		
		// IComponentMap<,> implementation
		
		IAccessor<TEntity> IComponentMap<TEntity, TComponent>.Accessor => Accessor;
		
		public TComponent TryGet(TEntity entity, out bool success)
		{
			ref TComponent component = ref TryGetRef(entity, out success);
			return (success ? component : default);
		}
		
		public TComponent Set(TEntity entity, TComponent value, out bool exists)
		{
			ref TComponent component = ref GetOrCreateRef(entity, out exists);
			var old = (exists ? component : default);
			component = value;
			return old;
		}
		
		public TComponent TryRemove(TEntity entity, out bool success)
		{
			ref TComponent component = ref TryRemoveRef(entity, out success);
			return (success ? component : default);
		}
		
		
		// IComponentMap implementation
		
		IAccessor IComponentMap.Accessor => Accessor;
		
		Type IComponentMap.EntityType => typeof(TEntity);
		
		Type IComponentMap.ComponentType => typeof(TComponent);
		
		IComponent IComponentMap.Get(IEntity entity)
		{
			var e = entity.Ensure<TEntity>(nameof(entity));
			ref TComponent component = ref TryGetRef(e, out var success);
			return (success ? (IComponent)component : null);
		}
		
		IComponent IComponentMap.Set(IEntity entity, IComponent value)
		{
			var e = entity.Ensure<TEntity>(nameof(entity));
			var v = value.Ensure<TComponent>(nameof(value));
			ref TComponent component = ref GetOrCreateRef(e, out var exists);
			var old = (exists ? (IComponent)component : null);
			component = v;
			return old;
		}
		
		IComponent IComponentMap.Remove(IEntity entity)
		{
			var e = entity.Ensure<TEntity>(nameof(entity));
			ref TComponent component = ref TryRemoveRef(e, out var success);
			return (success ? (IComponent)component : null);
		}
		
		
		// IReadOnlyCollection / IEnumerable implementation
		
		public int Count => _dict.Count;
		
		public IEnumerator<IComponentRef<TEntity, TComponent>> GetEnumerator()
		{
			foreach (var entry in _dict)
				yield return new ComponentRef(entry);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
		
		
		public struct ComponentRef
			: IComponentRef<TEntity, TComponent>
		{
			internal readonly RefDictionary<TEntity, TComponent>.EntryRef _entry;
			
			internal ComponentRef(RefDictionary<TEntity, TComponent>.EntryRef entry)
				=> _entry = entry;
			
			public TEntity Entity => _entry.Key;
			
			IEntity IComponentRef.Entity => Entity;
			
			public TComponent Component {
				get => _entry.Value;
				set => _entry.Value = value;
			}
			
			IComponent IComponentRef.Component {
				get => Component;
				set => Component = value.Ensure<TComponent>(nameof(value));
			}
		}
	}
}
