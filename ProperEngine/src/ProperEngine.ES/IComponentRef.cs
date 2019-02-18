namespace ProperEngine.ES
{
	public interface IComponentRef<TKey, TComponent> : IComponentRef
		where TKey : struct, IEntityKey
	{
		new TKey Key { get; }
		
		new TComponent Value { get; set; }
	}
	
	public interface IComponentRef
	{
		IEntityKey Key { get; }
		
		IComponent Value { get; set; }
	}
}
