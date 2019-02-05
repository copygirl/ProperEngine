using System;

namespace ProperEngine.ES.Raw
{
	public interface IRawComponentMap<TEntity, TComponent>
			: IComponentMap<TEntity, TComponent>
			// TODO: Instead, implement a different method to iterate component references.
			// , IReadOnlyCollection<IRawComponentRef<TEntity, TComponent>>
		where TEntity    : struct, IEntity
		where TComponent : struct, IComponent
	{
		/// <summary>
		/// Returns the <see cref="IRawAccessor{TEntity}"/> this component map
		/// belongs to, if any.
		/// </summary>
		new IRawAccessor<TEntity> Accessor { get; }
		
		/// <summary>
		/// Attempts to get a reference to an already existing component value
		/// associated with the specified entity.
		/// </summary>
		/// <param name="entity"> The entity the component is associated with. </param>
		/// <param name="success"> When this method returns, contains whether the component value existed. </param>
		/// <returns> A reference to the existing component value if <paramref name="success"/> is <c>true</c>
		///     -OR-  a dummy reference with the value <c>default</c> if it's <c>false</c>. </returns>
		/// <exception cref="ArgumentException"> Thrown if the specified entity is invalid. </exception>
		ref TComponent TryGetRef(TEntity entity, out bool success);
		
		/// <summary>
		/// Gets a reference to a component value associated with the specified
		/// entity, creating it if necessary.
		/// </summary>
		/// <param name="entity"> The entity the component is associated with. </param>
		/// <param name="exists"> When this method returns, contains whether the component value existed. </param>
		/// <returns> A reference to the component value. If it has been newly
		///           created, it will have the value <c>default</c>. </returns>
		/// <exception cref="ArgumentException"> Thrown if the specified entity is invalid. </exception>
		ref TComponent GetOrCreateRef(TEntity entity, out bool exists);
		
		/// <summary>
		/// Attempts to remove an existing component value associated with the
		/// specified entity, returning a stale reference to the previous value.
		/// </summary>
		/// <param name="entity"> The entity the component is associated with. </param>
		/// <param name="success"> When this method returns, contains whether the component value existed. </param>
		/// <returns> A reference to stale component value if <paramref name="success"/> is <c>true</c>
		///     -OR-  a dummy reference with the value <c>default</c> if it's <c>false</c>. </returns>
		/// <exception cref="ArgumentException"> Thrown if the specified entity is invalid. </exception>
		ref TComponent TryRemoveRef(TEntity entity, out bool success);
	}
}
