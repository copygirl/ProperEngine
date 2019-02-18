using System;

namespace ProperEngine.ES
{
	public interface IEntityRef<TKey> : IEntityRef
		where TKey : struct, IEntityKey
	{
		new TKey Key { get; }
		
		TComponent TryGet<TComponent>(out bool success)
			where TComponent : IComponent;
		
		TComponent Set<TComponent>(TComponent value, out bool exists)
			where TComponent : IComponent;
		
		TComponent TryRemove<TComponent>(out bool success)
			where TComponent : IComponent;
	}
	
	public interface IEntityRef
	{
		IEntityKey Key { get; }
		
		IComponent Get(Type componentType);
		
		IComponent Set(Type componentType, IComponent value);
		
		IComponent Remove(Type componentType);
	}
}
