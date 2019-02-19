using System;
using System.Collections;
using System.Collections.Generic;
using ProperEngine.Utility;

namespace ProperEngine.ES.Raw.Collections
{
	public class RawComponentMap<TEntityKey, TComponent>
			: IRawComponentMap<TEntityKey, TComponent>
			, IComponentMap<TEntityKey, TComponent?>
			, IReadOnlyCollection<IComponentRef<TEntityKey, TComponent>>
		where TEntityKey : struct
		where TComponent : struct
	{
		private readonly RefDictionary<TEntityKey, TComponent> _dict
			= new RefDictionary<TEntityKey, TComponent>();
		
		
		// IRawComponentMap<,> implementation
		
		public ref TComponent TryGetRef(TEntityKey key, out bool success)
			=> ref _dict.TryGetEntry(GetBehavior.Default, key, out success);
		
		public ref TComponent GetOrCreateRef(TEntityKey key, out bool exists)
			=> ref _dict.TryGetEntry(GetBehavior.Create, key, out exists);
		
		public ref TComponent TryRemoveRef(TEntityKey key, out bool success)
			=> ref _dict.TryGetEntry(GetBehavior.Remove, key, out success);
		
		// IComponentMap<,> implementation
		
		public TComponent? Get(TEntityKey key)
		{
			ref var component = ref TryGetRef(key, out var success);
			return (success ? component : (TComponent?)null);
		}
		
		public TComponent? Set(TEntityKey key, TComponent? value)
		{
			if (value != null) {
				ref var component = ref GetOrCreateRef(key, out var exists);
				var previous = (exists ? component : (TComponent?)null);
				component = value.Value;
				return previous;
			} else {
				ref var component = ref TryRemoveRef(key, out var success);
				return (success ? component : (TComponent?)null);
			}
		}
		
		// IComponentMap implementation
		
		public Type EntityKeyType => typeof(TEntityKey);
		
		public Type ComponentType => typeof(TComponent);
		
		public object Get(object key)
		{
			var k = key.Ensure<TEntityKey>(nameof(key));
			ref var component = ref TryGetRef(k, out var success);
			return (success ? component : (TComponent?)null);
		}
		
		public object Set(object key, object value)
		{
			var k = key.Ensure<TEntityKey>(nameof(key));
			if (value != null) {
				var v = value.Ensure<TComponent>(nameof(value));
				ref var component = ref GetOrCreateRef(k, out var exists);
				var previous = (exists ? component : (object)null);
				component = v;
				return previous;
			} else {
				ref var component = ref TryRemoveRef(k, out var success);
				return (success ? component : (object)null);
			}
		}
		
		// IReadOnlyCollection implementation
		
		public int Count => _dict.Count;
		
		public IEnumerator<IComponentRef<TEntityKey, TComponent>> GetEnumerator()
		{
			foreach (var entry in _dict)
				yield return new ComponentRef(entry);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
		
		
		public readonly struct ComponentRef
			: IComponentRef<TEntityKey, TComponent>
		{
			internal readonly RefDictionary<TEntityKey, TComponent>.EntryRef _entry;
			
			internal ComponentRef(RefDictionary<TEntityKey, TComponent>.EntryRef entry)
				=> _entry = entry;
			
			public TEntityKey Key => _entry.Key;
			
			object IComponentRef.Key => Key;
			
			public TComponent Value {
				get => _entry.Value;
				set => _entry.Value = value;
			}
			
			object IComponentRef.Value {
				get => _entry.Value;
				set => _entry.Value = value.Ensure<TComponent>(nameof(value));
			}
		}
	}
}
