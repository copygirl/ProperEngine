using System;

namespace ProperEngine.ES
{
	/// <summary>
	/// Represents a key, uniquely identifying an entity in the context of an
	/// <see cref="IAccessor"/> and associate <see cref="IComponent"/>s with it.
	/// </summary>
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
