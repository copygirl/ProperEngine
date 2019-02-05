namespace ProperEngine.ES.Raw
{
	public interface IRawAccessor<TEntity>
			: IAccessor<TEntity>
		where TEntity : struct, IEntity
	{
		new IRawComponentMap<TEntity, TComponent> Component<TComponent>()
			where TComponent : struct, IComponent;
	}
}
