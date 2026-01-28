using System.Collections.Generic;
using System.Linq;

namespace THEBADDEST.DatabaseModule
{
	/// <summary>
	/// Extension methods for Database class to provide convenient component access and management.
	/// </summary>
	public static class DatabaseExtensions
	{
		/// <summary>
		/// Gets a component by its display name.
		/// </summary>
		/// <param name="database">The database instance</param>
		/// <param name="componentName">The component's display name</param>
		/// <returns>The component if found, null otherwise</returns>
		public static IDatabaseComponent GetComponentByName(this Database database, string componentName)
		{
			if (database == null || string.IsNullOrEmpty(componentName))
				return null;

			return database.GetAllComponents()
				.FirstOrDefault(c => c != null && c.GetComponentName() == componentName);
		}

		/// <summary>
		/// Gets a component by type.
		/// </summary>
		/// <typeparam name="T">The type of component to get</typeparam>
		/// <param name="database">The database instance</param>
		/// <returns>The first component of type T if found, null otherwise</returns>
		public static T GetComponent<T>(this Database database) where T : class, IDatabaseComponent
		{
			if (database == null)
				return null;

			return database.GetAllComponents()
				.OfType<T>()
				.FirstOrDefault();
		}

		/// <summary>
		/// Gets all components of a specific type.
		/// </summary>
		/// <typeparam name="T">The type of components to get</typeparam>
		/// <param name="database">The database instance</param>
		/// <returns>All components of type T</returns>
		public static IEnumerable<T> GetComponents<T>(this Database database) where T : class, IDatabaseComponent
		{
			if (database == null)
				return Enumerable.Empty<T>();

			return database.GetAllComponents().OfType<T>();
		}

		/// <summary>
		/// Gets a DatabaseComponent by the ScriptableObject it references.
		/// </summary>
		/// <param name="database">The database instance</param>
		/// <param name="targetScriptable">The ScriptableObject to find</param>
		/// <returns>The DatabaseComponent if found, null otherwise</returns>
		public static DatabaseComponent GetComponentByTarget(this Database database, UnityEngine.ScriptableObject targetScriptable)
		{
			if (database == null || targetScriptable == null)
				return null;

			return database.GetComponents<DatabaseComponent>()
				.FirstOrDefault(c => c.TargetScriptable == targetScriptable);
		}

		/// <summary>
		/// Gets a SingleEntityComponent by type.
		/// </summary>
		/// <typeparam name="T">The type of entity data</typeparam>
		/// <param name="database">The database instance</param>
		/// <returns>The first SingleEntityComponent&lt;T&gt; if found, null otherwise</returns>
		public static SingleEntityComponent<T> GetSingleEntityComponent<T>(this Database database)
		{
			if (database == null)
				return null;

			return database.GetAllComponents()
				.OfType<SingleEntityComponent<T>>()
				.FirstOrDefault();
		}

		/// <summary>
		/// Checks if the database has any components registered.
		/// </summary>
		/// <param name="database">The database instance</param>
		/// <returns>True if database has any components, false otherwise</returns>
		public static bool HasComponents(this Database database)
		{
			if (database == null)
				return false;

			return database.GetAllComponents().Any();
		}

		/// <summary>
		/// Gets the total count of all components (tables + components + single entity components).
		/// </summary>
		/// <param name="database">The database instance</param>
		/// <returns>The total component count</returns>
		public static int GetComponentCount(this Database database)
		{
			if (database == null)
				return 0;

			return database.GetAllComponents().Count();
		}
	}
}
