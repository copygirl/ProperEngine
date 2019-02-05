using System;
using System.Collections;
using System.Collections.Generic;

namespace ProperEngine.ES
{
	public interface IComponentMap<TEntity, TComponent>
			: IComponentMap
			, IReadOnlyCollection<IComponentRef<TEntity, TComponent>>
		where TEntity    : struct, IEntity
		where TComponent : IComponent
	{
		new IAccessor<TEntity> Accessor { get; }
		
		TComponent TryGet(TEntity entity, out bool success);
		
		TComponent Set(TEntity entity, TComponent value, out bool exists);
		
		TComponent TryRemove(TEntity entity, out bool success);
	}
	
	public interface IComponentMap
		: IEnumerable
	{
		IAccessor Accessor { get; }
		
		Type EntityType { get; }
		
		Type ComponentType { get; }
		
		IComponent Get(IEntity entity);
		
		IComponent Set(IEntity entity, IComponent value);
		
		IComponent Remove(IEntity entity);
	}
}
