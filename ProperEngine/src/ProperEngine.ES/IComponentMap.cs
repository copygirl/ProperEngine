using System;
using System.Collections;
using System.Collections.Generic;

namespace ProperEngine.ES
{
	public interface IComponentMap<TKey, TComponent>
			: IComponentMap
			, IReadOnlyCollection<IComponentRef<TKey, TComponent>>
		where TKey       : struct, IEntityKey
		where TComponent : IComponent
	{
		new IAccessor<TKey> Accessor { get; }
		
		TComponent TryGet(TKey key, out bool success);
		
		TComponent Set(TKey key, TComponent value, out bool exists);
		
		TComponent TryRemove(TKey key, out bool success);
	}
	
	public interface IComponentMap
		: IEnumerable
	{
		IAccessor Accessor { get; }
		
		Type KeyType { get; }
		
		Type ComponentType { get; }
		
		IComponent Get(IEntityKey key);
		
		IComponent Set(IEntityKey key, IComponent value);
		
		IComponent Remove(IEntityKey key);
	}
}
