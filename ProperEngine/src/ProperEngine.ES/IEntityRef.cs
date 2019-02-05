using System;

namespace ProperEngine.ES
{
	public interface IEntityRef<TEntity>
			: IEntityRef
		where TEntity : struct, IEntity
	{
		new TEntity Entity { get; }
		
		TComponent TryGet<TComponent>(out bool success)
			where TComponent : IComponent;
		
		TComponent Set<TComponent>(TComponent value, out bool exists)
			where TComponent : IComponent;
		
		TComponent TryRemove<TComponent>(out bool success)
			where TComponent : IComponent;
	}
	
	public interface IEntityRef
	{
		IEntity Entity { get; }
		
		IComponent Get(Type componentType);
		
		IComponent Set(Type componentType, IComponent value);
		
		IComponent Remove(Type componentType);
	}
}
