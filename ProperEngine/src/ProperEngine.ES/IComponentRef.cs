namespace ProperEngine.ES
{
	public interface IComponentRef<TEntity, TComponent>
		: IComponentRef
	{
		new TEntity Entity { get; }
		
		new TComponent Component { get; set; }
	}
	
	public interface IComponentRef
	{
		IEntity Entity { get; }
		
		IComponent Component { get; set; }
	}
}
