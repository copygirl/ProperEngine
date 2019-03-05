using System;
using ProperEngine.Utility;

namespace ProperEngine.Test
{
	public struct Position
		: IEquatable<Position>
	{
		public static readonly Position ORIGIN = new Position(0, 0);
		
		public int X { get; }
		public int Y { get; }
		
		public Position(int x, int y)
			{ X = x; Y = y; }
		
		public override string ToString()
			=> $"Position [ X={ X }, Y={ Y } ]";
		
		// IEquatable implementation
		
		public bool Equals(Position other)
			=> (X == other.X) && (Y == other.Y);
		public override bool Equals(object other)
			=> (other is Position pos) && Equals(pos);
		
		public static bool operator ==(Position left, Position right)
			=> left.Equals(right);
		public static bool operator !=(Position left, Position right)
			=> !left.Equals(right);
		
		public override int GetHashCode()
			=> HashHelpers.Combine(X, Y);
	}
	
	public class Name
		: IEquatable<Name>
	{
		public string Value { get; }
		
		public Name(string value)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			Value = value;
		}
		
		public override string ToString()
			=> $"Name [ { Value.ToLiteral() } ]";
		
		// IEquatable implementation
		
		public bool Equals(Name other)
			=> (other != null) && (Value == other.Value);
		public override bool Equals(object other)
			=> Equals(other as Name);
		
		public static bool operator ==(Name left, Name right)
			=> object.ReferenceEquals(left, right)
			|| (left?.Equals(right) ?? false);
		public static bool operator !=(Name left, Name right)
			=> !(left == right);
		
		public override int GetHashCode()
			=> Value.GetHashCode();
	}
}
