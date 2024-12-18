using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace THEBADDEST.DatabaseModule
{


	public class TableAssetCreatorWindow : EditorWindow
	{
		private Type[] tableTypes;
		private int    selectedTypeIndex;
		private string assetName        = "NewTable";
		private string targetFolderPath = "Assets";

		[MenuItem("Tools/THEBADDEST/Database/Table Asset Creator")]
		public static void ShowWindow()
		{
			var window = GetWindow<TableAssetCreatorWindow>("Table Asset Creator");
			window.Show();
		}

		private void OnEnable()
		{
			// Fetch all types derived from TableBase
			tableTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(TableBase)))
				.ToArray();

			if (tableTypes.Length == 0)
			{
				Debug.LogWarning("No derived TableBase types found.");
			}
		}

		private void OnGUI()
		{
			GUILayout.Label("Table Asset Creator", EditorStyles.boldLabel);

			if (tableTypes == null || tableTypes.Length == 0)
			{
				EditorGUILayout.HelpBox("No TableBase-derived classes found. Please create one first.", MessageType.Warning);
				return;
			}

			// Dropdown for selecting table type
			selectedTypeIndex = EditorGUILayout.Popup("Table Type", selectedTypeIndex, tableTypes.Select(t => t.Name).ToArray());

			// Text field for asset name
			assetName = EditorGUILayout.TextField("Asset Name", assetName);

			// Folder selection
			EditorGUILayout.BeginHorizontal();
			targetFolderPath = EditorGUILayout.TextField("Target Folder", targetFolderPath);
			if (GUILayout.Button("Select Folder", GUILayout.Width(120)))
			{
				var selectedFolder = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
				if (!string.IsNullOrEmpty(selectedFolder))
				{
					targetFolderPath = selectedFolder.Replace(Application.dataPath, "Assets");
				}
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(10);

			if (GUILayout.Button("Create Table Asset"))
			{
				CreateTableAsset();
			}
		}

		private void CreateTableAsset()
		{
			if (string.IsNullOrWhiteSpace(assetName))
			{
				Debug.LogError("Asset name cannot be empty.");
				return;
			}

			if (string.IsNullOrWhiteSpace(targetFolderPath) || !AssetDatabase.IsValidFolder(targetFolderPath))
			{
				Debug.LogError("Invalid target folder path.");
				return;
			}

			var tableType = tableTypes[selectedTypeIndex];
			var asset     = ScriptableObject.CreateInstance(tableType);

			var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{targetFolderPath}/{assetName}.asset");
			AssetDatabase.CreateAsset(asset, assetPath);
			AssetDatabase.SaveAssets();

			Debug.Log($"Created table asset '{assetName}' at '{assetPath}'.");
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
	}


}