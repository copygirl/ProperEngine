using System;

namespace ProperEngine.Common.ES
{
	public interface IAccessor<TEntityKey> : IAccessor
		where TEntityKey : struct
	{
		IComponentMap<TEntityKey, TComponent> Component<TComponent>();
		
		IEntityRef<TEntityKey> Entity(TEntityKey key);
	}
	
	public interface IAccessor
	{
		IComponentMap Component(Type componentType);
		
		IEntityRef Entity(object key);
	}
}
