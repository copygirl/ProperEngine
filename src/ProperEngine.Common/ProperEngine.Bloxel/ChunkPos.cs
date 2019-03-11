using System;
using System.Diagnostics;
using ProperEngine.Utility;

namespace ProperEngine.Bloxel
{
	public struct ChunkPos
		: IEquatable<ChunkPos>
	{
		public const int CHUNK_SIZE = 32;
		
		private const int X_MASK = 0b_00000_00000_11111;
		private const int Y_MASK = 0b_00000_11111_00000;
		private const int Z_MASK = 0b_11111_00000_00000;
		
		private const int X_SHIFT =  0;
		private const int Y_SHIFT =  5;
		private const int Z_SHIFT = 10;
		
		
		public ushort Index;
		
		public int X {
			get => (Index & X_MASK) >> X_SHIFT;
			set {
				Debug.Assert((value >= 0) && (value < 32));
				Index = (ushort)(Index & ~X_MASK | (value << X_SHIFT));
			}
		}
		public int Y {
			get => (Index & Y_MASK) >> Y_SHIFT;
			set {
				Debug.Assert((value >= 0) && (value < 32));
				Index = (ushort)(Index & ~Y_MASK | (value << Y_SHIFT));
			}
		}
		public int Z {
			get => (Index & Z_MASK) >> Z_SHIFT;
			set {
				Debug.Assert((value >= 0) && (value < 32));
				Index = (ushort)(Index & ~Z_MASK | (value << Z_SHIFT));
			}
		}
		
		public ChunkPos(ushort index)
			=> Index = index;
		
		public ChunkPos(int x, int y, int z)
		{
			if ((x < 0) || (x >= CHUNK_SIZE)) throw new ArgumentOutOfRangeException(
				nameof(x), x, $"x is not within [0, { CHUNK_SIZE })");
			if ((y < 0) || (y >= CHUNK_SIZE)) throw new ArgumentOutOfRangeException(
				nameof(y), y, $"y is not within [0, { CHUNK_SIZE })");
			if ((z < 0) || (z >= CHUNK_SIZE)) throw new ArgumentOutOfRangeException(
				nameof(z), z, $"z is not within [0, { CHUNK_SIZE })");
			Index = (ushort)((x << X_SHIFT) | (y << Y_SHIFT) | (z << Z_SHIFT));
		}
		
		// IEquatable implementation
		
		public bool Equals(ChunkPos other)
			=> (Index == other.Index);
		public override bool Equals(object other)
			=> (other is ChunkPos pos) && Equals(pos);
		
		public static bool operator ==(ChunkPos left, ChunkPos right)
			=> left.Equals(right);
		public static bool operator !=(ChunkPos left, ChunkPos right)
			=> !left.Equals(right);
		
		public override int GetHashCode()
			=> Index;
	}
}
