using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace THEBADDEST.DatabaseModule
{


	public static class DatabaseEditorUtility
	{
		private const string DefaultDatabasePath   = "Assets/Database.asset";
		private const string ResourcesDatabasePath = "GameDatabase";
		[MenuItem("Database/Initialize Database")]
		public static void InitializeDatabase()
		{
			var database = FindOrCreateDatabase();
			database.Initialize();
			Debug.Log("Database initialized.");
		}

		[MenuItem("Database/Auto-Register Tables")]
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

			Debug.Log($"Auto-registered {tableAssets.Count} tables to the Database.");
		}

		[MenuItem("Database/Create Table Drive Class")]
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
			className = Path.GetFileNameWithoutExtension(filePath);
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

			var content = GenerateTableClassContent(className);
			File.WriteAllText(filePath, content);

			AssetDatabase.Refresh();
			Debug.Log($"Table drive class '{className}' created at '{filePath}'.");
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

		private static Database FindOrCreateDatabase()
		{
			var database = AssetDatabase.FindAssets("t:RefinedDatabase")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<Database>)
				.FirstOrDefault();
			if (database == null)
			{
				database = Resources.Load<Database>(ResourcesDatabasePath);;
			}
			
			if (database == null)
			{
				database = ScriptableObject.CreateInstance<Database>();
				AssetDatabase.CreateAsset(database, DefaultDatabasePath);
				AssetDatabase.SaveAssets();
				Debug.Log($"Created a new database at '{DefaultDatabasePath}'.");
			}

			return database;
		}
	}


}