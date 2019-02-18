using System;

namespace ProperEngine.ES
{
	public interface IEntityKey {  }
	
	
	public static class EntityKeyExtensions
	{
		public static TKey Ensure<TKey>(
				this IEntityKey key, string paramName)
			where TKey : IEntityKey
		{
			if (key == null) throw new ArgumentNullException(paramName);
			if (!(key is TKey k)) throw new ArgumentException(
				$"Entity key is type '{ key.GetType() }' but expected '{ typeof(TKey) }'", paramName);
			return k;
		}
	}
}
