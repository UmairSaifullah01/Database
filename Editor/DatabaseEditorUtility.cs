using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace THEBADDEST.DatabaseModule
{


	public static class DatabaseEditorUtility
	{
		private const string DatabaseName = "GameDatabase";
		private const string DefaultDatabasePath   = "Assets/Resources/GameDatabase.asset";
		private const string ResourcesDatabasePath = "GameDatabase";
		[MenuItem("Tools/THEBADDEST/Database/Initialize Database")]
		public static void InitializeDatabase()
		{
			var database = FindOrCreateDatabase();
			database.Initialize();
			Debug.Log("Database initialized.");
		}

		[MenuItem("Tools/THEBADDEST/Database/Auto-Register Tables")]
		public static void AutoRegisterTables()
		{
			var database = FindOrCreateDatabase();

			var tableAssets = AssetDatabase.FindAssets("t:ScriptableObject")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
				.Where(table => table is TableBase)
				.Cast<TableBase>()
				.ToList();

			foreach (var table in tableAssets)
			{
				database.AddTable(table);
			}
			EditorUtility.SetDirty(database);
			AssetDatabase.SaveAssetIfDirty(database);
			Debug.Log($"Auto-registered {tableAssets.Count} tables to the Database.");
		}

		[MenuItem("Tools/THEBADDEST/Database/Auto-Register All Components")]
		public static void AutoRegisterAllComponents()
		{
			var database = FindOrCreateDatabase();
			int registeredCount = 0;

			// Register tables
			var tableAssets = AssetDatabase.FindAssets("t:ScriptableObject")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
				.Where(obj => obj is TableBase)
				.Cast<TableBase>()
				.ToList();

			foreach (var table in tableAssets)
			{
				database.AddTable(table);
				registeredCount++;
			}

			// Register DatabaseComponents
			var componentAssets = AssetDatabase.FindAssets("t:ScriptableObject")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
				.Where(obj => obj is DatabaseComponent)
				.Cast<DatabaseComponent>()
				.ToList();

			foreach (var component in componentAssets)
			{
				database.AddComponent(component);
				registeredCount++;
			}

			// Register SingleEntityComponents
			var singleEntityAssets = AssetDatabase.FindAssets("t:ScriptableObject")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
				.Where(obj => obj is SingleEntityComponentBase)
				.Cast<SingleEntityComponentBase>()
				.ToList();

			foreach (var singleEntity in singleEntityAssets)
			{
				database.AddSingleEntityComponent(singleEntity);
				registeredCount++;
			}

			EditorUtility.SetDirty(database);
			AssetDatabase.SaveAssetIfDirty(database);
			Debug.Log($"Auto-registered {registeredCount} components (tables: {tableAssets.Count}, components: {componentAssets.Count}, single entities: {singleEntityAssets.Count}) to the Database.");
		}

		[MenuItem("Tools/THEBADDEST/Database/Create Table Drive Class")]
		public static void CreateTableDriveClass()
		{
			var filePath = EditorUtility.SaveFilePanel("Save Table Drive Class", "Assets", "NewTable.cs", "cs");
			if (string.IsNullOrWhiteSpace(filePath))
			{
				Debug.LogWarning("Table drive class creation cancelled.");
				return;
			}
			var className = Path.GetFileNameWithoutExtension(filePath).Replace("Table", "");

			var directory = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var content = GenerateTableClass(className);
			File.WriteAllText(filePath, content);

			AssetDatabase.Refresh();
			Debug.Log($"Table drive class '{className}' created at '{filePath}'.");
		}

		private static string GenerateTableClass(string className)
		{
			return $@"
namespace THEBADDEST.DatabaseModule
{{
    public class {className}Table : Table<{className}>
    {{
        public {className}Table() : base()
        {{
        }}

        // Add your custom table logic here.
    }}

    [System.Serializable]
    public class {className}
    {{
        // Add your custom fields here.
    }}
}}";
		}
		private static string GenerateTableClassContent(string className)
		{
			return $@"using UnityEngine;
using THEBADDEST.DatabaseModule;

[CreateAssetMenu(menuName = ""Database/Tables/{className}"", fileName = ""{className}"")]
public class {className} : TableBase
{{
    [SerializeField] private string tableName = ""{className}"";

    public override string GetTableName() => tableName;

    // Add your custom table logic here.
}}";
		}

		public static Database FindOrCreateDatabase()
		{
			// First try to find by exact path
			var database = AssetDatabase.LoadAssetAtPath<Database>(DefaultDatabasePath);
			if (database != null)
				return database;
			
			// Try to find by name "GameDatabase" in all Database assets
			var databaseByName = AssetDatabase.FindAssets("t:Database")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Where(path => Path.GetFileNameWithoutExtension(path) == DatabaseName)
				.Select(AssetDatabase.LoadAssetAtPath<Database>)
				.FirstOrDefault();
			
			if (databaseByName != null)
				return databaseByName;
			
			// If not found by name, try to find any Database asset
			var anyDatabase = AssetDatabase.FindAssets("t:Database")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<Database>)
				.FirstOrDefault();
			
			if (anyDatabase != null)
				return anyDatabase;
			
			// If still not found, create a new one
			var databasePath = Path.GetDirectoryName(DefaultDatabasePath);
			if (!Directory.Exists(databasePath))
			{
				Directory.CreateDirectory(databasePath);
			}
			var newDatabase = ScriptableObject.CreateInstance<Database>();
			newDatabase.name = DatabaseName;
			AssetDatabase.CreateAsset(newDatabase, DefaultDatabasePath);
			AssetDatabase.SaveAssets();
			Debug.Log($"Created a new database '{DatabaseName}' at '{DefaultDatabasePath}'.");
			
			return newDatabase;
		}
		
		public static void RefreshDatabaseTables(Database database)
		{
			if (database == null) return;
			
			var tableAssets = AssetDatabase.FindAssets("t:ScriptableObject")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
				.Where(table => table is TableBase)
				.Cast<TableBase>()
				.ToList();

			// Clear existing tables and re-add all found tables
			var serializedObject = new SerializedObject(database);
			var tablesProperty = serializedObject.FindProperty("tables");
			tablesProperty.ClearArray();
			
			foreach (var table in tableAssets)
			{
				tablesProperty.arraySize++;
				tablesProperty.GetArrayElementAtIndex(tablesProperty.arraySize - 1).objectReferenceValue = table;
			}
			
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(database);
			AssetDatabase.SaveAssetIfDirty(database);
		}

		/// <summary>
		/// Creates a DatabaseComponent for the selected ScriptableObject.
		/// Appears in the context menu when right-clicking a ScriptableObject.
		/// </summary>
		[MenuItem("CONTEXT/ScriptableObject/Create Database Component", false, 1000)]
		public static void CreateDatabaseComponent(MenuCommand command)
		{
			var targetScriptable = command.context as ScriptableObject;
			if (targetScriptable == null)
			{
				Debug.LogWarning("Selected object is not a ScriptableObject.");
				return;
			}

			// Skip if it's already a DatabaseComponent or TableBase to avoid recursion
			if (targetScriptable is DatabaseComponent || targetScriptable is TableBase || targetScriptable is SingleEntityComponentBase)
			{
				Debug.LogWarning("Cannot create DatabaseComponent for DatabaseComponent, TableBase, or SingleEntityComponent.");
				return;
			}

			// Get the path of the target ScriptableObject
			string targetPath = AssetDatabase.GetAssetPath(targetScriptable);
			if (string.IsNullOrEmpty(targetPath))
			{
				Debug.LogWarning("Could not get asset path for the selected ScriptableObject.");
				return;
			}

			// Get directory and create new path for DatabaseComponent
			string directory = Path.GetDirectoryName(targetPath);
			string fileName = Path.GetFileNameWithoutExtension(targetPath);
			string newPath = Path.Combine(directory, $"{fileName}DBComponent.asset");

			// Ensure unique filename
			newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);

			// Create DatabaseComponent instance
			var dbComponent = ScriptableObject.CreateInstance<DatabaseComponent>();
			dbComponent.name = $"{fileName}DBComponent";
			
			// Set the target scriptable using public property
			dbComponent.TargetScriptable = targetScriptable;
			
			// Set the component name using SerializedObject (private field)
			var serializedObject = new SerializedObject(dbComponent);
			var componentNameProperty = serializedObject.FindProperty("componentName");
			
			if (componentNameProperty != null)
			{
				componentNameProperty.stringValue = fileName;
				serializedObject.ApplyModifiedProperties();
			}

			// Create asset
			AssetDatabase.CreateAsset(dbComponent, newPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			// Select the newly created asset
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = dbComponent;
			EditorGUIUtility.PingObject(dbComponent);

			Debug.Log($"Created DatabaseComponent '{dbComponent.name}' for '{targetScriptable.name}' at '{newPath}'.");

			// Optionally add to Database
			var database = FindOrCreateDatabase();
			if (database != null)
			{
				database.AddComponent(dbComponent);
				EditorUtility.SetDirty(database);
				AssetDatabase.SaveAssetIfDirty(database);
				Debug.Log($"Added DatabaseComponent '{dbComponent.name}' to Database.");
			}
		}

		/// <summary>
		/// Validates if the context menu item should be enabled.
		/// </summary>
		[MenuItem("CONTEXT/ScriptableObject/Create Database Component", true)]
		public static bool ValidateCreateDatabaseComponent(MenuCommand command)
		{
			var targetScriptable = command.context as ScriptableObject;
			if (targetScriptable == null)
				return false;

			// Disable for DatabaseComponent, TableBase, and SingleEntityComponentBase
			if (targetScriptable is DatabaseComponent || targetScriptable is TableBase || targetScriptable is SingleEntityComponentBase)
				return false;

			return true;
		}
	}


}