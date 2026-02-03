using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace THEBADDEST.DatabaseModule
{


	[CreateAssetMenu(menuName = "THEBADDEST/Database/Database", fileName = "Database")]
	public class Database : ScriptableObject, IDatabase
	{
		private Dictionary<string, TableBase> tableDictionary = new Dictionary<string, TableBase>();

		[SerializeField]
		private List<TableBase> tables = new List<TableBase>();

		[SerializeField]
		private List<DatabaseComponent> components = new List<DatabaseComponent>();

		[SerializeField]
		private List<SingleEntityComponentBase> singleEntityComponents = new List<SingleEntityComponentBase>();

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

			// Initialize tables
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

			// Initialize components
			foreach (var component in components)
			{
				if (component != null)
				{
					component.Initialize();
				}
			}

			// Initialize single entity components
			foreach (var singleEntityComponent in singleEntityComponents)
			{
				if (singleEntityComponent != null)
				{
					singleEntityComponent.Initialize();
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

		/// <summary>
		/// Adds a DatabaseComponent to the database.
		/// </summary>
		/// <param name="component">The component to add</param>
		public void AddComponent(DatabaseComponent component)
		{
			if (component == null) return;

			if (!components.Contains(component))
			{
				components.Add(component);
				Debug.Log($"Component '{component.GetComponentName()}' added successfully.");
			}
			else
			{
				Debug.LogWarning($"Component '{component.GetComponentName()}' is already registered.");
			}
		}

		/// <summary>
		/// Adds a SingleEntityComponent to the database.
		/// </summary>
		/// <param name="singleEntityComponent">The single entity component to add</param>
		public void AddSingleEntityComponent(SingleEntityComponentBase singleEntityComponent)
		{
			if (singleEntityComponent == null) return;

			if (!singleEntityComponents.Contains(singleEntityComponent))
			{
				singleEntityComponents.Add(singleEntityComponent);
				Debug.Log($"Single Entity Component '{singleEntityComponent.GetComponentName()}' added successfully.");
			}
			else
			{
				Debug.LogWarning($"Single Entity Component '{singleEntityComponent.GetComponentName()}' is already registered.");
			}
		}

		/// <summary>
		/// Gets all components (tables, components, and single entity components) as a unified list.
		/// </summary>
		/// <returns>An enumerable of all IDatabaseComponent instances</returns>
		public IEnumerable<IDatabaseComponent> GetAllComponents()
		{
			foreach (var table in tables)
			{
				if (table != null)
					yield return table;
			}

			foreach (var component in components)
			{
				if (component != null)
					yield return component;
			}

			foreach (var singleEntityComponent in singleEntityComponents)
			{
				if (singleEntityComponent != null)
					yield return singleEntityComponent;
			}
		}

		/// <summary>
		/// Removes a table from the database.
		/// </summary>
		/// <param name="table">The table to remove</param>
		/// <returns>True if removed successfully, false otherwise</returns>
		public bool RemoveTable(TableBase table)
		{
			if (table == null) return false;

			var removed = tables.Remove(table);
			if (removed)
			{
				tableDictionary.Remove(table.GetTableName());
				Debug.Log($"Table '{table.GetTableName()}' removed successfully.");
			}
			return removed;
		}

		/// <summary>
		/// Removes a DatabaseComponent from the database.
		/// </summary>
		/// <param name="component">The component to remove</param>
		/// <returns>True if removed successfully, false otherwise</returns>
		public bool RemoveComponent(DatabaseComponent component)
		{
			if (component == null) return false;

			var removed = components.Remove(component);
			if (removed)
			{
				Debug.Log($"Component '{component.GetComponentName()}' removed successfully.");
			}
			return removed;
		}

		/// <summary>
		/// Removes a SingleEntityComponent from the database.
		/// </summary>
		/// <param name="singleEntityComponent">The single entity component to remove</param>
		/// <returns>True if removed successfully, false otherwise</returns>
		public bool RemoveSingleEntityComponent(SingleEntityComponentBase singleEntityComponent)
		{
			if (singleEntityComponent == null) return false;

			var removed = singleEntityComponents.Remove(singleEntityComponent);
			if (removed)
			{
				Debug.Log($"Single Entity Component '{singleEntityComponent.GetComponentName()}' removed successfully.");
			}
			return removed;
		}

		/// <summary>
		/// Clears all tables from the database.
		/// </summary>
		public void ClearTables()
		{
			tables.Clear();
			tableDictionary.Clear();
			Debug.Log("All tables cleared from database.");
		}

		/// <summary>
		/// Clears all DatabaseComponents from the database.
		/// </summary>
		public void ClearComponents()
		{
			components.Clear();
			Debug.Log("All components cleared from database.");
		}

		/// <summary>
		/// Clears all SingleEntityComponents from the database.
		/// </summary>
		public void ClearSingleEntityComponents()
		{
			singleEntityComponents.Clear();
			Debug.Log("All single entity components cleared from database.");
		}

		/// <summary>
		/// Gets a component by its display name.
		/// </summary>
		/// <param name="componentName">The component's display name</param>
		/// <returns>The component if found, null otherwise</returns>
		public IDatabaseComponent GetComponentByName(string componentName)
		{
			if (string.IsNullOrEmpty(componentName))
				return null;

			return GetAllComponents()
				.FirstOrDefault(c => c != null && c.GetComponentName() == componentName);
		}
	}


}