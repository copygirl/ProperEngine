using System;
using System.Collections;
using System.Collections.Generic;
using ProperEngine.Common.Utility;

namespace ProperEngine.Common.ES.Collections
{
	public class ComponentDictionary<TEntityKey, TComponent>
			: IComponentMap<TEntityKey, TComponent>
		where TEntityKey : struct
		where TComponent : class
	{
		private readonly RefDictionary<TEntityKey, TComponent> _dict
			= new RefDictionary<TEntityKey, TComponent>();
		
		// IComponentMap<,> implementation
		
		public TComponent Get(TEntityKey key)
		{
			ref var component = ref _dict.TryGetEntry(GetBehavior.Default, key, out var success);
			return (success ? component : null);
		}
		
		public TComponent Set(TEntityKey key, TComponent value)
		{
			if (value != null) {
				ref var component = ref _dict.TryGetEntry(
					GetBehavior.Create, key, out var exists);
				var previous = (exists ? component : null);
				component = value;
				return previous;
			} else {
				ref var component = ref _dict.TryGetEntry(
					GetBehavior.Remove, key, out var success);
				return (success ? component : null);
			}
		}
		
		// IComponentMap implementation
		
		public Type EntityKeyType => typeof(TEntityKey);
		
		public Type ComponentType => typeof(TComponent);
		
		object IComponentMap.Get(object key)
			=> Get(key.Ensure<TEntityKey>(nameof(key)));
		
		object IComponentMap.Set(object key, object value)
			=> Set(key.Ensure<TEntityKey>(nameof(key)),
			       value.Ensure<TComponent>(nameof(value)));
		
		// IReadOnlyCollection implementation
		
		public int Count => _dict.Count;
		
		public IEnumerator<IComponentRef<TEntityKey, TComponent>> GetEnumerator()
		{
			foreach (var entry in _dict)
				yield return entry;
		}
		
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
	}
}
