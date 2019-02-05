using System;

namespace ProperEngine.ES
{
	public interface IEntity {  }
	
	
	public static class EntityExtensions
	{
		public static TEntity Ensure<TEntity>(
				this IEntity entity, string paramName)
			where TEntity : IEntity
		{
			if (entity == null) throw new ArgumentNullException(paramName);
			if (!(entity is TEntity e)) throw new ArgumentException(
				$"Entity is type '{ entity.GetType() }' but expected '{ typeof(TEntity) }'", paramName);
			return e;
		}
	}
}
