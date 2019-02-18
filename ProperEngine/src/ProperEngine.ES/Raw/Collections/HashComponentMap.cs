using System;
using System.Collections;
using System.Collections.Generic;
using ProperEngine.Utility;

namespace ProperEngine.ES.Raw.Collections
{
	public class HashComponentMap<TKey, TComponent>
			: IRawComponentMap<TKey, TComponent>
		where TKey       : struct, IEntityKey
		where TComponent : struct, IComponent
	{
		private readonly RefDictionary<TKey, TComponent> _dict
			= new RefDictionary<TKey, TComponent>();
		
		public HashComponentMap(IRawAccessor<TKey> accessor)
			=> Accessor = accessor;
		
		
		// IRawComponentMap<,> implementation
		
		public IRawAccessor<TKey> Accessor { get; }
		
		public ref TComponent TryGetRef(TKey key, out bool success)
			=> ref _dict.TryGetEntry(GetBehavior.Default, key, out success);
		
		public ref TComponent GetOrCreateRef(TKey key, out bool exists)
			=> ref _dict.TryGetEntry(GetBehavior.Create, key, out exists);
		
		public ref TComponent TryRemoveRef(TKey key, out bool success)
			=> ref _dict.TryGetEntry(GetBehavior.Remove, key, out success);
		
		
		// IComponentMap<,> implementation
		
		IAccessor<TKey> IComponentMap<TKey, TComponent>.Accessor => Accessor;
		
		public TComponent TryGet(TKey key, out bool success)
		{
			ref TComponent component = ref TryGetRef(key, out success);
			return (success ? component : default);
		}
		
		public TComponent Set(TKey key, TComponent value, out bool exists)
		{
			ref TComponent component = ref GetOrCreateRef(key, out exists);
			var old = (exists ? component : default);
			component = value;
			return old;
		}
		
		public TComponent TryRemove(TKey key, out bool success)
		{
			ref TComponent component = ref TryRemoveRef(key, out success);
			return (success ? component : default);
		}
		
		
		// IComponentMap implementation
		
		IAccessor IComponentMap.Accessor => Accessor;
		
		Type IComponentMap.KeyType => typeof(TKey);
		
		Type IComponentMap.ComponentType => typeof(TComponent);
		
		IComponent IComponentMap.Get(IEntityKey key)
		{
			var e = key.Ensure<TKey>(nameof(key));
			ref TComponent component = ref TryGetRef(e, out var success);
			return (success ? (IComponent)component : null);
		}
		
		IComponent IComponentMap.Set(IEntityKey key, IComponent value)
		{
			var e = key.Ensure<TKey>(nameof(key));
			var v = value.Ensure<TComponent>(nameof(value));
			ref TComponent component = ref GetOrCreateRef(e, out var exists);
			var old = (exists ? (IComponent)component : null);
			component = v;
			return old;
		}
		
		IComponent IComponentMap.Remove(IEntityKey key)
		{
			var e = key.Ensure<TKey>(nameof(key));
			ref TComponent component = ref TryRemoveRef(e, out var success);
			return (success ? (IComponent)component : null);
		}
		
		
		// IReadOnlyCollection / IEnumerable implementation
		
		public int Count => _dict.Count;
		
		public IEnumerator<IComponentRef<TKey, TComponent>> GetEnumerator()
		{
			foreach (var entry in _dict)
				yield return new ComponentRef(entry);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
		
		
		public struct ComponentRef
			: IComponentRef<TKey, TComponent>
		{
			internal readonly RefDictionary<TKey, TComponent>.EntryRef _entry;
			
			internal ComponentRef(RefDictionary<TKey, TComponent>.EntryRef entry)
				=> _entry = entry;
			
			public TKey Key => _entry.Key;
			
			IEntityKey IComponentRef.Key => Key;
			
			public TComponent Value {
				get => _entry.Value;
				set => _entry.Value = value;
			}
			
			IComponent IComponentRef.Value {
				get => Value;
				set => Value = value.Ensure<TComponent>(nameof(value));
			}
		}
	}
}
