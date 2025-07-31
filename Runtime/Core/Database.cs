using System.Collections.Generic;
using UnityEngine;


namespace THEBADDEST.DatabaseModule
{


	[CreateAssetMenu(menuName = "THEBADDEST/Database/Database", fileName = "Database")]
	public class Database : ScriptableObject, IDatabase
	{
		private Dictionary<string, TableBase> tableDictionary = new Dictionary<string, TableBase>();

		[SerializeField]
		private List<TableBase> tables = new List<TableBase>();

		#region Global Access
		/// <summary>
		/// Global database instance for easy access
		/// </summary>
		public static Database Instance => DatabaseServiceLocator.DatabaseService();

		/// <summary>
		/// Get a table globally - shortest possible way
		/// </summary>
		/// <typeparam name="T">The table type</typeparam>
		/// <returns>The table instance</returns>
		public static T Get<T>() where T : TableBase
		{
			return Instance.GetTable<T>();
		}

		/// <summary>
		/// Get a table by name globally
		/// </summary>
		/// <param name="tableName">Name of the table</param>
		/// <returns>The table instance</returns>
		public static TableBase Get(string tableName)
		{
			return Instance.GetTableByName(tableName);
		}
		#endregion

		public void Initialize()
		{
			tableDictionary.Clear();

			foreach (var table in tables)
			{
				if (table == null) continue;

				var tableName = table.GetTableName();
				if (tableDictionary.TryAdd(tableName, table))
				{
					table.Initialize();
				}
				else
				{
					Debug.LogWarning($"Duplicate table with name '{tableName}' found: {table.name}");
				}
			}
		}

		public T GetTable<T>() where T : TableBase
		{
			foreach (var table in tables)
			{
				if (table is T tTable)
					return tTable;
			}

			Debug.LogWarning($"Table of type {typeof(T).Name} not found.");
			return null;
		}

		public TableBase GetTableByName(string tableName)
		{
			if (tableDictionary.TryGetValue(tableName, out var table))
			{
				return table;
			}

			Debug.LogWarning($"Table with name '{tableName}' not found.");
			return null;
		}

		public void AddTable(TableBase table)
		{
			if (table == null) return;

			var tableName = table.GetTableName();

			if (tableDictionary.TryAdd(tableName, table))
			{
				tables.Add(table);
				Debug.Log($"Table '{tableName}' added successfully.");
			}
			else
			{
				Debug.LogWarning($"Table with name '{tableName}' is already registered.");
			}
		}
	}


}