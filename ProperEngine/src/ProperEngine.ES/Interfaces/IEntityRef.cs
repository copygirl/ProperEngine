using System;
using System.Collections.Generic;

namespace ProperEngine.ES
{
	public interface IEntityRef<TEntity>
			: IEntityRef
		where TEntity : struct, IEntity
	{
		new IESAccessor<TEntity> Accessor { get; }
		
		new TEntity Entity { get; }
		
		ref TComponent TryGetRef<TComponent>(out bool success)
			where TComponent : struct, IComponent;
		
		ref TComponent GetOrCreateRef<TComponent>(out bool exists)
			where TComponent : struct, IComponent;
		
		ref TComponent TryRemoveRef<TComponent>(out bool success)
			where TComponent : struct, IComponent;
	}
	
	public interface IEntityRef
	{
		IESAccessor Accessor { get; }
		
		IEntity Entity { get; }
		
		IComponent Get(Type componentType);
		
		IComponent Set(Type componentType, IComponent value);
		
		IComponent Remove(Type componentType);
	}
}
