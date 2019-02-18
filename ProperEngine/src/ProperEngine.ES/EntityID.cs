using System;

namespace ProperEngine.ES
{
	public struct EntityID<T> : IEntityKey
			, IEquatable<EntityID<T>>
			, IComparable<EntityID<T>>
		where T : struct
			, IEquatable<T>
			, IComparable<T>
	{
		public T Value { get; }
		
		public EntityID(T value)
			=> Value = value;
		
		
		public override string ToString()
			=> $"EntityID [{ Value }]";
		
		// IEquatable implementation
		
		public bool Equals(EntityID<T> other)
			=> Value.Equals(other.Value);
		public override bool Equals(object other)
			=> (other is EntityID<T> entity) && Equals(entity);
		
		public static bool operator ==(EntityID<T> left, EntityID<T> right)
			=> left.Equals(right);
		public static bool operator !=(EntityID<T> left, EntityID<T> right)
			=> !left.Equals(right);
		
		public override int GetHashCode()
			=> Value.GetHashCode();
		
		// IComparable implementation
		
		public int CompareTo(EntityID<T> other)
			=> Value.CompareTo(other.Value);
	}
}
