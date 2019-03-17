namespace ProperEngine.Common.ES
{
	public interface IComponentRef<TEntityKey, TComponent> : IComponentRef
		where TEntityKey : struct
	{
		new TEntityKey Key { get; }
		
		new TComponent Value { get; set; }
	}
	
	public interface IComponentRef
	{
		object Key { get; }
		
		object Value { get; set; }
	}
}
