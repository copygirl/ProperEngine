using System;

namespace ProperEngine.ES
{
	public readonly struct EntityID<T>
			: IEntityKey
			, IEquatable<EntityID<T>>
			, IComparable<EntityID<T>>
		where T : struct
			, IEquatable<T>
			, IComparable<T>
	{
		private readonly T _value;
		
		public EntityID(T value)
			=> _value = value;
		
		public static explicit operator T(EntityID<T> id)
			=> id._value;
		
		
		public override string ToString()
			=> $"EntityID [{ _value }]";
		
		// IEquatable implementation
		
		public bool Equals(EntityID<T> other)
			=> _value.Equals(other._value);
		public override bool Equals(object other)
			=> (other is EntityID<T> id) && Equals(id);
		
		public static bool operator ==(EntityID<T> left, EntityID<T> right)
			=> left.Equals(right);
		public static bool operator !=(EntityID<T> left, EntityID<T> right)
			=> !left.Equals(right);
		
		public override int GetHashCode()
			=> _value.GetHashCode();
		
		// IComparable implementation
		
		public int CompareTo(EntityID<T> other)
			=> _value.CompareTo(other._value);
	}
}
