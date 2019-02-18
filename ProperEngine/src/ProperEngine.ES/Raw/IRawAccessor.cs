namespace ProperEngine.ES.Raw
{
	public interface IRawAccessor<TKey>
			: IAccessor<TKey>
		where TKey : struct, IEntityKey
	{
		new IRawComponentMap<TKey, TComponent> Component<TComponent>()
			where TComponent : struct, IComponent;
	}
}
