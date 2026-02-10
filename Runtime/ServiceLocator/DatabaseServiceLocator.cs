using UnityEngine;


namespace THEBADDEST.DatabaseModule
{

	/// <summary>
	/// A global service locator for database tables.
	/// </summary>
	public static class DatabaseServiceLocator
	{
		// can add more paths if needed
		static string DataBasePath = "GameDatabase";

		static IServiceLocator serviceLocator;
		static bool initialized;

		static DatabaseServiceLocator()
		{
			Initialize();
		}

		static void Initialize()
		{
			if (initialized)
				return;
			serviceLocator = ServiceLocator.Global;
			var database = Resources.Load<Database>(DataBasePath);
			if (database == null)
				return;
			database.Initialize();
			serviceLocator.RegisterService(database);
			initialized = true;
		}

		public static Database DatabaseService()
		{
			Initialize();
			return serviceLocator.GetService<Database>();
		}

		#region Convenient Table Access Methods

		/// <summary>
		/// Get a table directly by type - shortest way to access tables
		/// </summary>
		/// <typeparam name="T">The table type to get</typeparam>
		/// <returns>The table instance</returns>
		public static T GetTable<T>() where T : TableBase
		{
			return DatabaseService().GetTable<T>();
		}

		/// <summary>
		/// Get a table by name - useful when you know the table name
		/// </summary>
		/// <param name="tableName">Name of the table</param>
		/// <returns>The table instance</returns>
		public static TableBase GetTable(string tableName)
		{
			return DatabaseService().GetTableByName(tableName);
		}

		/// <summary>
		/// Get a generic table with its data type
		/// </summary>
		/// <typeparam name="TTable">The table type</typeparam>
		/// <typeparam name="TData">The data type stored in the table</typeparam>
		/// <returns>The typed table</returns>
		public static Table<TData> GetTable<TTable, TData>()
			where TTable : Table<TData>
			where TData : class
		{
			return DatabaseService().GetTable<TTable>();
		}

		#endregion

		#region Database Management

		/// <summary>
		/// Initialize the database (useful for manual initialization)
		/// </summary>
		public static void InitializeDatabase()
		{
			Initialize();
		}

		/// <summary>
		/// Get the database instance directly
		/// </summary>
		/// <returns>The database instance</returns>
		public static Database GetDatabase()
		{
			return DatabaseService();
		}

		#endregion
	}


}