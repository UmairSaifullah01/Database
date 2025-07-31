using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace THEBADDEST.DatabaseModule
{
	public abstract class Table<T> : TableBase, ITable<T>
	{
		[SerializeField] private string tableName;
		[SerializeField] private List<T> entries = new List<T>();
		[SerializeField] private TableAdapter adapter;

		// Enhanced functionality
		private Dictionary<string, Dictionary<object, List<int>>> fieldIndexes = new Dictionary<string, Dictionary<object, List<int>>>();
		private bool indexesDirty = true;
		private List<T> sortedEntries = null;
		private string currentSortKeyName = null;
		private bool sortAscending = true;
		private bool sortCacheValid = false;

		public IReadOnlyList<T> Entries => entries;

		// Enhanced querying properties
		public int TotalCount => entries.Count;
		public bool IsEmpty => entries.Count == 0;
		public T FirstRecord => entries.Count > 0 ? entries[0] : default;
		public T LastRecord => entries.Count > 0 ? entries[entries.Count - 1] : default;

		public override string GetTableName() => string.IsNullOrEmpty(tableName) ? typeof(T).Name : tableName;

		public override void Initialize()
		{
			adapter?.Deserialize<T>(this);
			RebuildIndexes();
		}

		#region Basic CRUD Operations
		public T GetRecord(int id)
		{
			return id >= 0 && id < entries.Count ? entries[id] : default;
		}

		public void RemoveRecord(int id)
		{
			if (id >= 0 && id < entries.Count)
			{
				entries.RemoveAt(id);
				indexesDirty = true;
				sortCacheValid = false; // Reset sorting
			}
		}

		public void AddRecord(T record)
		{
			if (record != null)
			{
				entries.Add(record);
				indexesDirty = true;
				sortCacheValid = false; // Reset sorting
			}
		}

		public void UpdateRecord(int id, T record)
		{
			if (id >= 0 && id < entries.Count && record != null)
			{
				entries[id] = record;
				indexesDirty = true;
				sortCacheValid = false; // Reset sorting
			}
		}

		public void Clear()
		{
			entries.Clear();
			fieldIndexes.Clear();
			indexesDirty = false;
			sortCacheValid = false;
		}
		#endregion

		#region Indexing System
		/// <summary>
		/// Creates an index on a specific field for faster lookups
		/// </summary>
		/// <typeparam name="TKey">The type of the field to index</typeparam>
		/// <param name="keySelector">Function to extract the key from each record</param>
		/// <param name="indexName">Name of the index (optional)</param>
		public void CreateIndex<TKey>(Func<T, TKey> keySelector, string indexName = null)
		{
			if (keySelector == null) return;

			var indexKey = indexName ?? typeof(TKey).Name;
			var index = new Dictionary<object, List<int>>();

			for (int i = 0; i < entries.Count; i++)
			{
				var key = keySelector(entries[i]);
				if (key != null)
				{
					if (!index.ContainsKey(key))
						index[key] = new List<int>();
					index[key].Add(i);
				}
			}

			fieldIndexes[indexKey] = index;
		}

		/// <summary>
		/// Rebuilds all indexes (call when data changes significantly)
		/// </summary>
		public void RebuildIndexes()
		{
			fieldIndexes.Clear();
			indexesDirty = false;
		}

		/// <summary>
		/// Gets records by indexed field value
		/// </summary>
		public IEnumerable<T> GetByIndex<TKey>(Func<T, TKey> keySelector, TKey key)
		{
			var indexKey = typeof(TKey).Name;

			if (!fieldIndexes.ContainsKey(indexKey))
			{
				CreateIndex(keySelector, indexKey);
			}

			if (fieldIndexes[indexKey].TryGetValue(key, out var indices))
			{
				foreach (var index in indices)
				{
					if (index >= 0 && index < entries.Count)
						yield return entries[index];
				}
			}
		}
		#endregion

		#region Query Methods
		/// <summary>
		/// Filters records based on a predicate
		/// </summary>
		public IEnumerable<T> Where(Func<T, bool> predicate)
		{
			if (predicate == null) return Enumerable.Empty<T>();
			return entries.Where(predicate);
		}

		/// <summary>
		/// Gets the first record that matches the predicate
		/// </summary>
		public T FirstOrDefault(Func<T, bool> predicate)
		{
			if (predicate == null) return default;
			return entries.FirstOrDefault(predicate);
		}

		/// <summary>
		/// Gets the first record that matches the predicate, throws if not found
		/// </summary>
		public T First(Func<T, bool> predicate)
		{
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));
			return entries.First(predicate);
		}

		/// <summary>
		/// Gets the last record that matches the predicate
		/// </summary>
		public T LastOrDefault(Func<T, bool> predicate)
		{
			if (predicate == null) return default;
			return entries.LastOrDefault(predicate);
		}

		/// <summary>
		/// Counts records that match the predicate
		/// </summary>
		public int Count(Func<T, bool> predicate)
		{
			if (predicate == null) return 0;
			return entries.Count(predicate);
		}

		/// <summary>
		/// Checks if any record matches the predicate
		/// </summary>
		public bool Any(Func<T, bool> predicate)
		{
			if (predicate == null) return false;
			return entries.Any(predicate);
		}

		/// <summary>
		/// Checks if all records match the predicate
		/// </summary>
		public bool All(Func<T, bool> predicate)
		{
			if (predicate == null) return true;
			return entries.All(predicate);
		}

		/// <summary>
		/// Projects each record to a new form
		/// </summary>
		public IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			if (selector == null) return Enumerable.Empty<TResult>();
			return entries.Select(selector);
		}
		#endregion

		#region Sorting Methods
		/// <summary>
		/// Sorts records by a key selector
		/// </summary>
		public IEnumerable<T> OrderBy<TKey>(Func<T, TKey> keySelector)
		{
			if (keySelector == null) return entries;

			// Check if we need to rebuild the cache
			bool needRebuild = !sortCacheValid ||
							  currentSortKeyName != keySelector.Method.Name ||
							  !sortAscending;

			if (needRebuild)
			{
				sortedEntries = entries.OrderBy(keySelector).ToList();
				currentSortKeyName = keySelector.Method.Name;
				sortAscending = true;
				sortCacheValid = true;
			}

			return sortedEntries;
		}

		/// <summary>
		/// Sorts records by a key selector in descending order
		/// </summary>
		public IEnumerable<T> OrderByDescending<TKey>(Func<T, TKey> keySelector)
		{
			if (keySelector == null) return entries;

			// Check if we need to rebuild the cache
			bool needRebuild = !sortCacheValid ||
							  currentSortKeyName != keySelector.Method.Name ||
							  sortAscending;

			if (needRebuild)
			{
				sortedEntries = entries.OrderByDescending(keySelector).ToList();
				currentSortKeyName = keySelector.Method.Name;
				sortAscending = false;
				sortCacheValid = true;
			}

			return sortedEntries;
		}

		/// <summary>
		/// Clears the sorting cache
		/// </summary>
		public void ClearSorting()
		{
			sortedEntries = null;
			currentSortKeyName = null;
			sortCacheValid = false;
		}
		#endregion

		#region Bulk Operations
		/// <summary>
		/// Adds multiple records at once
		/// </summary>
		public void AddRange(IEnumerable<T> records)
		{
			if (records == null) return;

			foreach (var record in records)
			{
				if (record != null)
					entries.Add(record);
			}

			indexesDirty = true;
			sortCacheValid = false;
		}

		        /// <summary>
        /// Removes all records that match the predicate
        /// </summary>
        public int RemoveAll(Func<T, bool> predicate)
        {
            if (predicate == null) return 0;
            
            var removedCount = entries.RemoveAll(new Predicate<T>(predicate));
            if (removedCount > 0)
            {
                indexesDirty = true;
                sortCacheValid = false;
            }
            
            return removedCount;
        }

		/// <summary>
		/// Updates all records that match the predicate
		/// </summary>
		public void UpdateAll(Func<T, bool> predicate, Func<T, T> updateFunc)
		{
			if (predicate == null || updateFunc == null) return;

			for (int i = 0; i < entries.Count; i++)
			{
				if (predicate(entries[i]))
				{
					entries[i] = updateFunc(entries[i]);
				                }
            }
            
            indexesDirty = true;
            sortCacheValid = false;
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Gets a random record from the table
		/// </summary>
		public T GetRandom()
		{
			if (entries.Count == 0) return default;
			return entries[UnityEngine.Random.Range(0, entries.Count)];
		}

		/// <summary>
		/// Gets multiple random records
		/// </summary>
		public IEnumerable<T> GetRandom(int count)
		{
			if (count <= 0 || entries.Count == 0) return Enumerable.Empty<T>();

			var shuffled = entries.OrderBy(x => UnityEngine.Random.value).Take(count);
			return shuffled;
		}

		/// <summary>
		/// Finds the index of a record
		/// </summary>
		public int IndexOf(T record)
		{
			return entries.IndexOf(record);
		}

		/// <summary>
		/// Checks if the table contains a specific record
		/// </summary>
		public bool Contains(T record)
		{
			return entries.Contains(record);
		}

		/// <summary>
		/// Converts the table to an array
		/// </summary>
		public T[] ToArray()
		{
			return entries.ToArray();
		}

		/// <summary>
		/// Converts the table to a list
		/// </summary>
		public List<T> ToList()
		{
			return new List<T>(entries);
		}
		#endregion

		#region Serialization
		/// <summary>
		/// Serializes the table data
		/// </summary>
		public void Serialize()
		{
			adapter?.Serialize<T>(this);
		}

		/// <summary>
		/// Deserializes the table data
		/// </summary>
		public void Deserialize()
		{
			adapter?.Deserialize<T>(this);
			RebuildIndexes();
		}
		#endregion
	}
}