using System;

namespace ProperEngine.ES
{
	public struct Entity<T> : IEntity
			, IEquatable<Entity<T>>
			, IComparable<Entity<T>>
		where T : struct
			, IEquatable<T>
			, IComparable<T>
	{
		public T ID { get; }
		
		public Entity(T id)
			=> ID = id;
		
		
		public override string ToString()
			=> $"Entity [{ ID }]";
		
		// IEquatable implementation
		
		public bool Equals(Entity<T> other)
			=> ID.Equals(other.ID);
		public override bool Equals(object other)
			=> (other is Entity<T> entity) && Equals(entity);
		
		public static bool operator ==(Entity<T> left, Entity<T> right)
			=> left.Equals(right);
		public static bool operator !=(Entity<T> left, Entity<T> right)
			=> !left.Equals(right);
		
		public override int GetHashCode()
			=> ID.GetHashCode();
		
		// IComparable implementation
		
		public int CompareTo(Entity<T> other)
			=> ID.CompareTo(other.ID);
	}
}
