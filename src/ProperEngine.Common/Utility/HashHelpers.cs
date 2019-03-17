// The following license applies to parts of this file.
// Some changes (additions, cosmetic, formatting) have been made.

/*

Copyright (c) .NET Foundation and Contributors

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using System;
using System.Collections;
using System.Diagnostics;

namespace ProperEngine.Common.Utility
{
	public static class HashHelpers
	{
		public static int Combine(int hash1, int hash2, int hash3)
			=> Combine(Combine(hash1, hash2), hash3);
		public static int Combine(int hash1, int hash2, int hash3, int hash4)
			=> Combine(Combine(Combine(hash1, hash2), hash3), hash4);
		public static int Combine(params int[] hashCodes)
		{
			if (hashCodes == null) throw new ArgumentNullException(nameof(hashCodes));
			if (hashCodes.Length == 0) throw new ArgumentException(
				"Argument 'hashCodes' is empty", nameof(hashCodes));
			var hash = hashCodes[0];
			for (var i = 1; i < hashCodes.Length; i++)
				hash = Combine(hash, hashCodes[i]);
			return hash;
		}
		
		public static int Combine(IEnumerable objects)
		{
			if (objects == null) throw new ArgumentNullException(nameof(objects));
			var enumerator = objects.GetEnumerator();
			// Get the initial hash code.
			if (!enumerator.MoveNext()) throw new ArgumentException(
				"Argument 'objects' is empty", nameof(objects));
			var hash = enumerator.Current?.GetHashCode() ?? 0;
			// Combine it with all other objects' hash codes.
			while (enumerator.MoveNext())
				hash = Combine(hash, enumerator.Current?.GetHashCode() ?? 0);
			// And return the result.
			return hash;
		}
		public static int Combine(params object[] objects)
			=> Combine((IEnumerable)objects);
		
		
		// CoreLib => System.Numerics.Hashing.HashHelpers
		// https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Numerics/Hashing/HashHelpers.cs
		
		public static int Combine(int hash1, int hash2)
		{
			uint rol5 = ((uint)hash1 << 5) | ((uint)hash1 >> 27);
			return ((int)rol5 + hash1) ^ hash2;
		}
		
		
		// CoreLib => System.Collections.HashHelpers
		// https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Collections/HashHelpers.cs
		
		public const int MaxPrimeArrayLength = 0x7FEFFFFD;
		
		public const int HashPrime = 101;
		
		private static readonly int[] _primes = {
			3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
			1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519,
			21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751, 225307,
			270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263, 1674319, 2009191,
			2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
		};
		
		public static bool IsPrime(int candidate)
		{
			if ((candidate & 1) != 0) {
				int limit = (int)Math.Sqrt(candidate);
				for (int divisor = 3; divisor <= limit; divisor += 2)
					if ((candidate % divisor) == 0)
						return false;
				return true;
			}
			return (candidate == 2);
		}
		
		public static int GetPrime(int min)
		{
			if (min < 0) throw new ArgumentOutOfRangeException(nameof(min));
			
			for (int i = 0; i < _primes.Length; i++) {
				int prime = _primes[i];
				if (prime >= min)
					return prime;
			}
			
			for (int i = (min | 1); i < int.MaxValue; i += 2)
				if (IsPrime(i) && ((i - 1) % HashPrime != 0))
					return i;
			
			return min;
		}
		
		public static int ExpandPrime(int oldSize)
		{
			int newSize = 2 * oldSize;
			
			if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize) {
				Debug.Assert(MaxPrimeArrayLength == GetPrime(MaxPrimeArrayLength), "Invalid MaxPrimeArrayLength");
				return MaxPrimeArrayLength;
			}
			
			return GetPrime(newSize);
		}
	}
}
