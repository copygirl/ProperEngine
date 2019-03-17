using System;
using System.Collections;

namespace ProperEngine.Common.ES
{
	public interface IComponentMap<TEntityKey, TComponent> : IComponentMap
		where TEntityKey : struct
	{
		TComponent Get(TEntityKey key);
		
		TComponent Set(TEntityKey key, TComponent value);
	}
	
	public interface IComponentMap
		: IEnumerable
	{
		Type EntityKeyType { get; }
		
		Type ComponentType { get; }
		
		object Get(object key);
		
		object Set(object key, object value);
	}
}
