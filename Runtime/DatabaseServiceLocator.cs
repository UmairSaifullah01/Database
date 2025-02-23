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
		static DatabaseServiceLocator()
		{
			Initialize();
		}

		static void Initialize()
		{
			serviceLocator = ServiceLocator.Global;
			var database = Resources.Load<Database>(DataBasePath);
			database.Initialize();
			serviceLocator.RegisterService(database);
		}

		public static Database DatabaseService()
		{
			Initialize();
			return serviceLocator.GetService<Database>();
		}
		
	}


}