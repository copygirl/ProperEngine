using System;

namespace ProperEngine.ES
{
	public interface IESAccessor<TEntity> : IESAccessor
		where TEntity : struct, IEntity
	{
		IComponentMap<TEntity, TComponent> Component<TComponent>()
			where TComponent : struct, IComponent;
		
		IEntityRef<TEntity> Entity(TEntity entity);
	}
	
	public interface IESAccessor
	{
		IComponentMap Component(Type componentType);
		
		IEntityRef Entity(IEntity entity);
	}
}
