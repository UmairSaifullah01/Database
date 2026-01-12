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

		[MenuItem("Tools/THEBADDEST/Database/Create Table Drive Class")]
		public static void CreateTableDriveClass()
		{
			// Prompt the user to select the name of the derived class
			var tableClassName = EditorUtility.DisplayDialogComplex(
				"Create Table Drive Class",
				"Enter the name of the new table class derived from TableBase:",
				"Create", "Cancel", "");

			if (tableClassName == 1) // Cancel button
				return;

			string className = "NewTable";

			// Create the class file
			var filePath = EditorUtility.SaveFilePanel("Save Table Drive Class", "Assets", className, "cs");
			className = Path.GetFileNameWithoutExtension(filePath).Replace("Table","");
			if (string.IsNullOrWhiteSpace(filePath))
			{
				Debug.LogWarning("Table drive class creation cancelled.");
				return;
			}

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
	}


}