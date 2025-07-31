using System;
using System.Collections.Generic;
using System.Linq;

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

		/// <summary>
		/// Gets the top N records sorted by a key selector
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <typeparam name="TKey">The type of the key to sort by.</typeparam>
		/// <param name="table">The table to get records from.</param>
		/// <param name="keySelector">The function to extract the key for sorting.</param>
		/// <param name="count">The number of records to return.</param>
		/// <returns>The top N records.</returns>
		public static IEnumerable<T> Top<T, TKey>(this Table<T> table, Func<T, TKey> keySelector, int count)
		{
			return table.OrderByDescending(keySelector).Take(count);
		}

		/// <summary>
		/// Gets the bottom N records sorted by a key selector
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <typeparam name="TKey">The type of the key to sort by.</typeparam>
		/// <param name="table">The table to get records from.</param>
		/// <param name="keySelector">The function to extract the key for sorting.</param>
		/// <param name="count">The number of records to return.</param>
		/// <returns>The bottom N records.</returns>
		public static IEnumerable<T> Bottom<T, TKey>(this Table<T> table, Func<T, TKey> keySelector, int count)
		{
			return table.OrderBy(keySelector).Take(count);
		}

		/// <summary>
		/// Gets records within a range of values
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <typeparam name="TKey">The type of the key to filter by.</typeparam>
		/// <param name="table">The table to get records from.</param>
		/// <param name="keySelector">The function to extract the key for filtering.</param>
		/// <param name="minValue">The minimum value (inclusive).</param>
		/// <param name="maxValue">The maximum value (inclusive).</param>
		/// <returns>Records within the specified range.</returns>
		public static IEnumerable<T> Between<T, TKey>(this Table<T> table, Func<T, TKey> keySelector, TKey minValue, TKey maxValue)
			where TKey : IComparable<TKey>
		{
			return table.Where(record =>
			{
				var value = keySelector(record);
				return value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0;
			});
		}

		/// <summary>
		/// Groups records by a key selector
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <typeparam name="TKey">The type of the key to group by.</typeparam>
		/// <param name="table">The table to group.</param>
		/// <param name="keySelector">The function to extract the key for grouping.</param>
		/// <returns>Groups of records.</returns>
		public static IEnumerable<IGrouping<TKey, T>> GroupBy<T, TKey>(this Table<T> table, Func<T, TKey> keySelector)
		{
			return table.Entries.GroupBy(keySelector);
		}

		/// <summary>
		/// Calculates the sum of a numeric field
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <param name="table">The table to calculate the sum from.</param>
		/// <param name="selector">The function to extract the numeric value.</param>
		/// <returns>The sum of all values.</returns>
		public static double Sum<T>(this Table<T> table, Func<T, double> selector)
		{
			return table.Entries.Sum(selector);
		}

		/// <summary>
		/// Calculates the average of a numeric field
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <param name="table">The table to calculate the average from.</param>
		/// <param name="selector">The function to extract the numeric value.</param>
		/// <returns>The average of all values.</returns>
		public static double Average<T>(this Table<T> table, Func<T, double> selector)
		{
			return table.Entries.Average(selector);
		}

		/// <summary>
		/// Finds the maximum value of a field
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="table">The table to find the maximum from.</param>
		/// <param name="selector">The function to extract the value.</param>
		/// <returns>The maximum value.</returns>
		public static TResult Max<T, TResult>(this Table<T> table, Func<T, TResult> selector)
			where TResult : IComparable<TResult>
		{
			return table.Entries.Max(selector);
		}

		/// <summary>
		/// Finds the minimum value of a field
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="table">The table to find the minimum from.</param>
		/// <param name="selector">The function to extract the value.</param>
		/// <returns>The minimum value.</returns>
		public static TResult Min<T, TResult>(this Table<T> table, Func<T, TResult> selector)
			where TResult : IComparable<TResult>
		{
			return table.Entries.Min(selector);
		}

		/// <summary>
		/// Gets distinct values from a field
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="table">The table to get distinct values from.</param>
		/// <param name="selector">The function to extract the value.</param>
		/// <returns>Distinct values.</returns>
		public static IEnumerable<TResult> Distinct<T, TResult>(this Table<T> table, Func<T, TResult> selector)
		{
			return table.Entries.Select(selector).Distinct();
		}

		/// <summary>
		/// Shuffles the records in the table
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <param name="table">The table to shuffle.</param>
		/// <returns>Shuffled records.</returns>
		public static IEnumerable<T> Shuffle<T>(this Table<T> table)
		{
			var random = new System.Random();
			return table.Entries.OrderBy(x => random.Next());
		}

		/// <summary>
		/// Gets a page of records
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <param name="table">The table to get records from.</param>
		/// <param name="pageNumber">The page number (1-based).</param>
		/// <param name="pageSize">The number of records per page.</param>
		/// <returns>Records for the specified page.</returns>
		public static IEnumerable<T> GetPage<T>(this Table<T> table, int pageNumber, int pageSize)
		{
			var skip = (pageNumber - 1) * pageSize;
			return table.Entries.Skip(skip).Take(pageSize);
		}

		/// <summary>
		/// Gets the total number of pages
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <param name="table">The table to calculate pages for.</param>
		/// <param name="pageSize">The number of records per page.</param>
		/// <returns>The total number of pages.</returns>
		        public static int GetPageCount<T>(this Table<T> table, int pageSize)
        {
            return (int)Math.Ceiling((double)table.TotalCount / pageSize);
        }

		/// <summary>
		/// Finds records that contain a specific text in a string field
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <param name="table">The table to search.</param>
		/// <param name="selector">The function to extract the string field.</param>
		/// <param name="searchText">The text to search for.</param>
		/// <param name="ignoreCase">Whether to ignore case in the search.</param>
		/// <returns>Records containing the search text.</returns>
		public static IEnumerable<T> ContainsText<T>(this Table<T> table, Func<T, string> selector, string searchText, bool ignoreCase = true)
		{
			if (string.IsNullOrEmpty(searchText))
				return Enumerable.Empty<T>();

			var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			return table.Where(record =>
			{
				var text = selector(record);
				return !string.IsNullOrEmpty(text) && text.IndexOf(searchText, comparison) >= 0;
			});
		}

		/// <summary>
		/// Finds records that start with a specific text
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <param name="table">The table to search.</param>
		/// <param name="selector">The function to extract the string field.</param>
		/// <param name="searchText">The text to search for.</param>
		/// <param name="ignoreCase">Whether to ignore case in the search.</param>
		/// <returns>Records starting with the search text.</returns>
		public static IEnumerable<T> StartsWith<T>(this Table<T> table, Func<T, string> selector, string searchText, bool ignoreCase = true)
		{
			if (string.IsNullOrEmpty(searchText))
				return Enumerable.Empty<T>();

			var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			return table.Where(record =>
			{
				var text = selector(record);
				return !string.IsNullOrEmpty(text) && text.StartsWith(searchText, comparison);
			});
		}

		/// <summary>
		/// Finds records that end with a specific text
		/// </summary>
		/// <typeparam name="T">The type of the records in the table.</typeparam>
		/// <param name="table">The table to search.</param>
		/// <param name="selector">The function to extract the string field.</param>
		/// <param name="searchText">The text to search for.</param>
		/// <param name="ignoreCase">Whether to ignore case in the search.</param>
		/// <returns>Records ending with the search text.</returns>
		public static IEnumerable<T> EndsWith<T>(this Table<T> table, Func<T, string> selector, string searchText, bool ignoreCase = true)
		{
			if (string.IsNullOrEmpty(searchText))
				return Enumerable.Empty<T>();

			var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			return table.Where(record =>
			{
				var text = selector(record);
				return !string.IsNullOrEmpty(text) && text.EndsWith(searchText, comparison);
			});
		}
	}
}