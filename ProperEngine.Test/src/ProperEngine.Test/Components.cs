using System;
using ProperEngine.ES;
using ProperEngine.Utility;

namespace ProperEngine.Test
{
	public struct Position
		: IComponent 
		, IEquatable<Position>
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
		public override int GetHashCode()
			=> HashHelpers.Combine(X, Y);
	}
}
