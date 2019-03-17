using System;
using System.Collections;
using System.Collections.Generic;
using ProperEngine.Common.Utility;

namespace ProperEngine.Common.ES.Raw.Collections
{
	public class RawComponentMap<TEntityKey, TComponent>
			: IRawComponentMap<TEntityKey, TComponent>
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
