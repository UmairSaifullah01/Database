using System.Collections.Generic;
using UnityEngine;


namespace THEBADDEST.DatabaseModule
{


	[CreateAssetMenu(menuName = "Database/Database", fileName = "Database")]
	public class Database : ScriptableObject,IDatabase
	{
		private Dictionary<string, TableBase> tableDictionary = new Dictionary<string, TableBase>();

		[SerializeField]
		private List<TableBase> tables = new List<TableBase>();

		
		
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