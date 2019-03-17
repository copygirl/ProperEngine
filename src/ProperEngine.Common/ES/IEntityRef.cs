using System;

namespace ProperEngine.Common.ES
{
	public interface IEntityRef<TEntityKey> : IEntityRef
		where TEntityKey : struct
	{
		new TEntityKey EntityKey { get; }
		
		TComponent TryGet<TComponent>(out bool success);
		
		TComponent Set<TComponent>(TComponent value, out bool exists);
		
		TComponent TryRemove<TComponent>(out bool success);
	}
	
	public interface IEntityRef
	{
		object EntityKey { get; }
		
		object Get(Type componentType);
		
		object Set(Type componentType, object value);
		
		object Remove(Type componentType);
	}
}
