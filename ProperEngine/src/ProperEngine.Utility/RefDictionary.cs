using System;
using System.Collections.Generic;
using ProperEngine.ES;

namespace ProperEngine.Utility
{
	internal class RefDictionary<TKey, TValue>
		where TKey : struct
	{
		internal struct Entry
		{
			public int HashCode;
			public int Next;
			public TKey Key;
			public TValue Value;
			
			internal bool HasValue => (HashCode >= 0);
		}
		
		private static TValue EMPTY_COMPONENT
			= default(TValue);
		
		
		private readonly IEqualityComparer<TKey> _comparer;
		
		private int[] _buckets;
		private Entry[] _entries;
		private int _count;
		private int _version;
		private int _freeEntry;
		private int _freeCount;
		
		public int Count => (_count - _freeCount);
		
		public int Capacity {
			get => _entries.Length;
			set => Resize(value);
		}
		
		
		public RefDictionary()
			: this(0, EqualityComparer<TKey>.Default) {  }
		public RefDictionary(int capacity)
			: this(capacity, EqualityComparer<TKey>.Default) {  }
		public RefDictionary(IEqualityComparer<TKey> comparer)
			: this(0, comparer) {  }
		
		public RefDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
			if (comparer == null) throw new ArgumentNullException(nameof(comparer));
			if (capacity > 0) Initialize(capacity);
			_comparer = comparer;
		}
		
		
		private void Initialize(int capacity) {
			int size = HashHelpers.GetPrime(capacity);
			_buckets = new int[size];
			_entries = new Entry[size];
			Array.Fill(_buckets, -1);
			_freeEntry = -1;
		}
		
		public void Clear()
		{
			if (_count == 0) return;
			Array.Fill(_buckets, -1);
			Array.Clear(_entries, 0, _count);
			_count     =  0;
			_freeEntry = -1;
			_freeCount =  0;
			_version++;
		}
		
		private void Resize()
			=> Resize(HashHelpers.ExpandPrime(_count));
		
		private void Resize(int newSize)
		{
			if (newSize < _entries.Length)
				throw new ArgumentOutOfRangeException(nameof(newSize));
			
			var newBuckets = new int[newSize];
			var newEntries = new Entry[newSize];
			Array.Fill(newBuckets, -1);
			Array.Copy(_entries, 0, newEntries, 0, _count);
			
			for (int i = 0; i < _count; i++) {
				ref var entry = ref newEntries[i];
				if (entry.HashCode < 0) continue;
				var bucket = (entry.HashCode % newSize);
				entry.Next = newBuckets[bucket] - 1;
				newBuckets[bucket] = i + 1;
			}
			
			_buckets = newBuckets;
			_entries = newEntries;
			_version++;
		}
		
		
		public ref TValue TryGetEntry(
			GetBehavior behavior, TKey key, out bool found)
		{
			found = false;
			
			ref TValue GetEmptyEntry()
			{
				EMPTY_COMPONENT = default;
				return ref EMPTY_COMPONENT;
			}
			
			if (_buckets == null) {
				if (behavior != GetBehavior.Create)
					return ref GetEmptyEntry();
				Initialize(0);
			}
			
			var hashCode   = _comparer.GetHashCode(key) & 0x7FFFFFFF;
			ref var bucket = ref _buckets[hashCode % _buckets.Length];
			
			var last = -1;
			for (var i = bucket - 1; i >= 0; ) {
				ref var entry = ref _entries[i];
				if ((entry.HashCode == hashCode) && _comparer.Equals(entry.Key, key)) {
					found = true;
					
					if (behavior == GetBehavior.Remove) {
						if (last < 0) bucket = entry.Next + 1;
						else _entries[last].Next = entry.Next;
						
						entry.HashCode = -1;
						entry.Next     = _freeEntry;
						entry.Key      = default(TKey);
						// Not resetting allows us to return previous value.
						// entry.Value    = default(TComponent);
						
						_freeEntry = i;
						_freeCount++;
						_version++;
					}
					
					return ref entry.Value;
				}
				last = i;
				i    = entry.Next;
			}
			
			if (behavior != GetBehavior.Create)
				return ref GetEmptyEntry();
			
			int index;
			if (_freeCount > 0) {
				index      = _freeEntry;
				_freeEntry = _entries[index].Next;
				_freeCount--;
			} else {
				if (_count == _entries.Length) {
					Resize();
					bucket = ref _buckets[hashCode % _buckets.Length];
				}
				index = _count;
				_count++;
			}
			
			{
				ref var entry  = ref _entries[index];
				entry.HashCode = hashCode;
				entry.Next     = bucket - 1;
				entry.Key      = key;
				entry.Value    = default;
				
				bucket = index + 1;
				_version++;
				
				return ref entry.Value;
			}
		}
		
		
		// IEntryEnumerable implementation
		
		public Enumerator GetEnumerator()
			=> new Enumerator(this);
		
		public struct Enumerator
		{
			private readonly RefDictionary<TKey, TValue> _dict;
			private int _index;
			private int _version;
			
			internal Enumerator(RefDictionary<TKey, TValue> dict)
			{
				_dict    = dict;
				_index   = -1;
				_version = _dict._version;
			}
			
			public bool MoveNext()
			{
				while (++_index < _dict._count)
					if (_dict._entries[_index].HasValue)
						return true;
				return false;
			}
			
			public EntryRef Current
				=> new EntryRef(_dict._entries, _index);
		}
		
		public readonly struct EntryRef
			: IComponentRef<TKey, TValue>
		{
			private readonly Entry[] _entries;
			private readonly int _index;
			
			public TKey Key => _entries[_index].Key;
			public ref TValue Value => ref _entries[_index].Value;
			
			internal EntryRef(Entry[] entries, int index)
				{ _entries = entries; _index = index; }
			
			// IComponentRef implementation
			
			TValue IComponentRef<TKey, TValue>.Value {
				get => Value;
				set => Value = value;
			}
			
			object IComponentRef.Key => Key;
			
			object IComponentRef.Value {
				get => Value;
				set => Value = value.Ensure<TValue>(nameof(value));
			}
		}
	}
	
	internal enum GetBehavior
	{
		Default,
		Create,
		Remove,
	}
}
