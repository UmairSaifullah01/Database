using System.IO;
using UnityEditor;
using UnityEngine;


namespace THEBADDEST.DatabaseModule
{
	[CustomEditor(typeof(JsonTableAdapter))]
	public class JsonTableAdapterEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			JsonTableAdapter adapter = (JsonTableAdapter)target;

			// Add button to create and save file
			if (GUILayout.Button("Create and Save JSON File"))
			{
				
				SaveToJsonFile(adapter.json);
			}

			// Add button to load JSON file
			if (GUILayout.Button("Load JSON File"))
			{
				adapter.json = LoadJsonFile();
			}
		}

		public void SaveToJsonFile(string json)
		{
			if(string.IsNullOrEmpty(json)) return;
			string assetPath = AssetDatabase.GetAssetPath(this);
			string filePath  = Path.Combine(Path.GetDirectoryName(assetPath) ?? string.Empty, "JsonTableAdapter.json");
			File.WriteAllText(filePath, json);
			Debug.Log("JSON saved to file: " + filePath);
		}

		public string LoadJsonFile()
		{
			string json      = string.Empty;
			string assetPath = AssetDatabase.GetAssetPath(this);
			string filePath  = Path.Combine(Path.GetDirectoryName(assetPath) ?? string.Empty, "JsonTableAdapter.json");
			if (File.Exists(filePath))
			{
				json = File.ReadAllText(filePath);
				Debug.Log("JSON loaded from file: " + filePath);
			}
			else
			{
				Debug.Log("JSON file not found: " + filePath);
			}

			return json;
		}
	}
}