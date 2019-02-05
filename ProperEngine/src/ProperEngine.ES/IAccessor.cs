using System;

namespace ProperEngine.ES
{
	public interface IAccessor<TEntity>
			: IAccessor
		where TEntity : struct, IEntity
	{
		IComponentMap<TEntity, TComponent> Component<TComponent>()
			where TComponent : IComponent;
		
		IEntityRef<TEntity> Entity(TEntity entity);
	}
	
	public interface IAccessor
	{
		IComponentMap Component(Type componentType);
		
		IEntityRef Entity(IEntity entity);
	}
}
