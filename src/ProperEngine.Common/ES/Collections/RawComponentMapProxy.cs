using System;
using System.Collections;
using ProperEngine.Common.Utility;

namespace ProperEngine.Common.ES.Raw.Collections
{
	internal class RawComponentMapProxy<TEntityKey, TComponent>
			: IComponentMap<TEntityKey, TComponent?>
		where TEntityKey : struct
		where TComponent : struct
	{
		private readonly IRawComponentMap<TEntityKey, TComponent> _map;
		
		public RawComponentMapProxy(IRawComponentMap<TEntityKey, TComponent> map)
		{
			if (map == null) throw new ArgumentNullException(nameof(map));
			_map = map;
		}
		
		// IComponentMap<,> implementation
		
		public TComponent? Get(TEntityKey key)
		{
			ref var component = ref _map.TryGetRef(key, out var success);
			return (success ? component : (TComponent?)null);
		}
		
		public TComponent? Set(TEntityKey key, TComponent? value)
		{
			if (value != null) {
				ref var component = ref _map.GetOrCreateRef(key, out var exists);
				var previous = (exists ? component : (TComponent?)null);
				component = value.Value;
				return previous;
			} else {
				ref var component = ref _map.TryRemoveRef(key, out var success);
				return (success ? component : (TComponent?)null);
			}
		}
		
		// IComponentMap implementation
		
		public Type EntityKeyType => typeof(TEntityKey);
		
		public Type ComponentType => typeof(TComponent);
		
		public object Get(object key)
		{
			var k = key.Ensure<TEntityKey>(nameof(key));
			ref var component = ref _map.TryGetRef(k, out var success);
			return (success ? component : (TComponent?)null);
		}
		
		public object Set(object key, object value)
		{
			var k = key.Ensure<TEntityKey>(nameof(key));
			if (value != null) {
				var v = value.Ensure<TComponent>(nameof(value));
				ref var component = ref _map.GetOrCreateRef(k, out var exists);
				var previous = (exists ? component : (object)null);
				component = v;
				return previous;
			} else {
				ref var component = ref _map.TryRemoveRef(k, out var success);
				return (success ? component : (object)null);
			}
		}
		
		// IEnumerable implementation
		
		public IEnumerator GetEnumerator()
			=> throw new NotSupportedException();
	}
}