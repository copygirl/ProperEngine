using System;

namespace ProperEngine.ES
{
	public interface IAccessor<TKey> : IAccessor
		where TKey : struct, IEntityKey
	{
		IComponentMap<TKey, TComponent> Component<TComponent>()
			where TComponent : IComponent;
		
		IEntityRef<TKey> Entity(TKey key);
	}
	
	public interface IAccessor
	{
		IComponentMap Component(Type componentType);
		
		IEntityRef Entity(IEntityKey key);
	}
}
