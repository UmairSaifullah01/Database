using System;
using System.Collections.Generic;


namespace THEBADDEST.DatabaseModule
{


	public static class TableExtensions
	{
		/// <summary>
		/// Converts a Table<T> to a Dictionary using the provided key selector function.
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <typeparam name="K">The type of the key.</typeparam>
		/// <param name="table">The table to convert.</param>
		/// <param name="keySelector">The function to extract a key from a record.</param>
		/// <returns>A dictionary mapping keys to records.</returns>
		public static Dictionary<K, T> ToDictionary<T, K>(this Table<T> table, Func<T, K> keySelector)
		{
			var dictionary = new Dictionary<K, T>();

			foreach (var entry in table.Entries)
			{
				var key = keySelector(entry);
				if (!dictionary.TryAdd(key, entry))
				{
					UnityEngine.Debug.LogWarning($"Duplicate key '{key}' encountered in table '{table.GetTableName()}'. Skipping entry.");
				}
			}

			return dictionary;
		}

		/// <summary>
		/// Retrieves a record from a Table<T> based on a key using the provided key selector.
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <typeparam name="K">The type of the key.</typeparam>
		/// <param name="table">The table to search.</param>
		/// <param name="keySelector">The function to extract a key from a record.</param>
		/// <param name="key">The key to search for.</param>
		/// <returns>The matching record, or default if not found.</returns>
		public static T GetByKey<T, K>(this Table<T> table, Func<T, K> keySelector, K key)
		{
			foreach (var entry in table.Entries)
			{
				if (EqualityComparer<K>.Default.Equals(keySelector(entry), key))
				{
					return entry;
				}
			}

			UnityEngine.Debug.LogWarning($"Key '{key}' not found in table '{table.GetTableName()}'.");
			return default;
		}
	}


}