using System;
using System.Diagnostics;

namespace ProperEngine.Common.Bloxel
{
	public class BlockStorage<T>
		where T : struct
	{
		public readonly T[] Data;
		
		public int Size { get; }
		
		public ref T this[int x, int y, int z] { get {
			Debug.Assert((x >= 0) && (x < Size));
			Debug.Assert((y >= 0) && (y < Size));
			Debug.Assert((z >= 0) && (z < Size));
			return ref Data[x + y * Size + z * Size * Size];
		} }
		
		public BlockStorage(int size)
		{
			if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
			Size = size;
			Data = new T[size * size * size];
		}
	}
}
