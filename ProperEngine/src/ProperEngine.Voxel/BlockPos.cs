using System;
using ProperEngine.ES;
using ProperEngine.Utility;

namespace ProperEngine.Voxel
{
	public struct BlockPos
		: IEntity
		, IEquatable<BlockPos>
	{
		public static readonly BlockPos ORIGIN
			= new BlockPos(0, 0, 0);
		
		public int X;
		public int Y;
		public int Z;
		
		public BlockPos(int x, int y, int z)
			{ X = x; Y = y; Z = z; }
		
		// IEquatable implementation
		
		public bool Equals(BlockPos other)
			=> (X == other.X) && (Y == other.Y) && (Z == other.Z);
		public override bool Equals(object other)
			=> (other is BlockPos pos) && Equals(pos);
		public override int GetHashCode()
			=> HashHelpers.Combine(X, Y, Z);
	}
}
