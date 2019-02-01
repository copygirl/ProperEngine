using System;
using System.Collections;
using System.Collections.Generic;

namespace ProperEngine.ES
{
	public class HashComponentMap<TEntity, TComponent>
		: IComponentMap<TEntity, TComponent>
		where TEntity    : struct, IEntity
		where TComponent : struct, IComponent
	{
		private readonly RefDictionary<TEntity, TComponent> _dict
			= new RefDictionary<TEntity, TComponent>();
		
		public TComponent? this[TEntity entity] {
			// Get and Set are available as extension methods.
			get => this.Get(entity);
			set => this.Set(entity, value);
		}
		
		public HashComponentMap(IESAccessor<TEntity> accessor)
			=> Accessor = accessor;
		
		
		// IComponentMap<,> implementation

		public IESAccessor<TEntity> Accessor { get; }
		
		public ref TComponent TryGetRef(TEntity entity, out bool success)
			=> ref _dict.TryGetEntry(GetBehavior.Default, entity, out success);
		
		public ref TComponent GetOrCreateRef(TEntity entity, out bool exists)
			=> ref _dict.TryGetEntry(GetBehavior.Create, entity, out exists);
		
		public ref TComponent TryRemoveRef(TEntity entity, out bool success)
			=> ref _dict.TryGetEntry(GetBehavior.Remove, entity, out success);
		
		
		// IComponentMap implementation
		
		IESAccessor IComponentMap.Accessor => Accessor;
		
		Type IComponentMap.EntityType => typeof(TEntity);
		Type IComponentMap.ComponentType => typeof(TComponent);
		
		public int Count => _dict.Count;

		IComponent IComponentMap.Get(IEntity entity)
		{
			var e = CheckEntity(entity, nameof(entity));
			ref TComponent component = ref TryGetRef(e, out var exists);
			return (exists ? (IComponent)component : null);
		}
		
		IComponent IComponentMap.Set(IEntity entity, IComponent value)
		{
			var e = CheckEntity(entity, nameof(entity));
			var v = CheckComponent(value, nameof(value));
			ref TComponent component = ref GetOrCreateRef(e, out var exists);
			var old = (exists ? (IComponent)component : null);
			component = v;
			return old;
		}
		
		IComponent IComponentMap.Remove(IEntity entity)
		{
			var e = CheckEntity(entity, nameof(entity));
			ref TComponent component = ref TryRemoveRef(e, out var exists);
			return (exists ? (IComponent)component : null);
		}
		
		
		private static TEntity CheckEntity(IEntity entity, string paramName)
		{
			if (entity == null) throw new ArgumentNullException(paramName);
			if (!(entity is TEntity e)) throw new ArgumentException(
				$"Entity is type '{ entity.GetType() }' but expected '{ typeof(TEntity) }'", paramName);
			return e;
		}
		
		private static TComponent CheckComponent(IComponent component, string paramName)
		{
			if (component == null) throw new ArgumentNullException(paramName);
			if (!(component is TComponent c)) throw new ArgumentException(
				$"Component is type '{ component.GetType() }' but expected '{ typeof(TComponent) }'", paramName);
			return c;
		}
		
		
		// IEntryEnumerable implemenation
		
		public IEnumerator<IComponentRef<TEntity, TComponent>> GetEnumerator()
		{
			var enumerator = _dict.GetEntryEnumerator();
			while (enumerator.MoveNext())
				using (var componentRef = new EnumerableComponentRef(enumerator))
					yield return componentRef;
		}
		
		IEnumerator IEnumerable.GetEnumerator()
			=> _dict.GetEnumerator();
		
		
		private class EnumerableComponentRef
			: IComponentRef<TEntity, TComponent>
			, IDisposable
		{
			private RefDictionary<TEntity, TComponent>.EntryEnumerator _enumerator;
			
			public ref readonly TEntity Entity => ref GetEnumerator().CurrentKey;
			public ref TComponent Component => ref GetEnumerator().CurrentValue;
			
			public EnumerableComponentRef(
					RefDictionary<TEntity, TComponent>.EntryEnumerator enumerator)
				=> _enumerator = enumerator;
			
			public void Dispose()
				=> _enumerator = null;
			
			private RefDictionary<TEntity, TComponent>.EntryEnumerator GetEnumerator()
			{
				if (_enumerator == null) throw new InvalidOperationException(
					"EnumerableComponentRef was disposed (used outside of foreach?)");
				return _enumerator;
			}
		}
	}
}
