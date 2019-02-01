using System;
using System.Collections.Generic;

namespace ProperEngine.ES
{
	public interface IComponentMap<TEntity, TComponent>
			: IEnumerable<IComponentRef<TEntity, TComponent>>
			, IComponentMap
		where TEntity    : struct, IEntity
		where TComponent : IComponent
	{
		/// <summary>
		/// Returns the <see cref="IESAccessor{TEntity}"/> this component map
		/// belongs to, if any.
		/// </summary>
		new IESAccessor<TEntity> Accessor { get; }
		
		/// <summary>
		/// Attempts to get a reference to an already existing component value
		/// associated with the specified entity.
		/// </summary>
		/// <param name="entity"> The entity the component is associated with. </param>
		/// <param name="success"> When this method returns, contains whether the component value existed. </param>
		/// <returns> A reference to the existing component value if <paramref name="success"/> is <c>true</c>
		///     -OR-  a dummy reference with the value <c>default(TComponent)</c> if it's <c>false</c>. </returns>
		/// <exception cref="ArgumentException"> Thrown if the specified entity is invalid. </exception>
		ref TComponent TryGetRef(TEntity entity, out bool success);
		
		/// <summary>
		/// Gets a reference to a component value associated with the specified
		/// entity, creating it if necessary.
		/// </summary>
		/// <param name="entity"> The entity the component is associated with. </param>
		/// <param name="exists"> When this method returns, contains whether the component value existed. </param>
		/// <returns> A reference to the component value. If it has been newly
		///           created, it will have the value <c>default(TComponent)</c>. </returns>
		/// <exception cref="ArgumentException"> Thrown if the specified entity is invalid. </exception>
		ref TComponent GetOrCreateRef(TEntity entity, out bool exists);
		
		/// <summary>
		/// Attempts to remove an existing component value associated with the
		/// specified entity, returning a stale reference to the previous value.
		/// </summary>
		/// <param name="entity"> The entity the component is associated with. </param>
		/// <param name="success"> When this method returns, contains whether the component value existed. </param>
		/// <returns> A reference to stale component value if <paramref name="success"/> is <c>true</c>
		///     -OR-  a dummy reference with the value <c>default(TComponent)</c> if it's <c>false</c>. </returns>
		/// <exception cref="ArgumentException"> Thrown if the specified entity is invalid. </exception>
		ref TComponent TryRemoveRef(TEntity entity, out bool success);
	}
	
	public interface IComponentMap
	{
		IESAccessor Accessor { get; }
		
		Type EntityType { get; }
		Type ComponentType { get; }
		
		int Count { get; }
		
		IComponent Get(IEntity entity);
		
		IComponent Set(IEntity entity, IComponent value);
		
		IComponent Remove(IEntity entity);
	}
	
	public interface IComponentRef<TEntity, TComponent>
		where TEntity    : struct, IEntity
		where TComponent : IComponent
	{
		ref readonly TEntity Entity { get; }
		
		ref TComponent Component { get; }
	}
}
