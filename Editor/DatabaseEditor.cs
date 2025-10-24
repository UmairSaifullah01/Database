using System;
using System.Linq;
using THEBADDEST.EditorTools;
using UnityEditor;
using UnityEngine;


namespace THEBADDEST.DatabaseModule
{


	public class DatabaseEditor : EditorWindow
	{
		private Database database;
		private Type[] tableTypes;
		private SerializedObject serializedDatabase;
		private SerializedProperty tablesProperty;
		private int selectedIndex = 0;
		Vector2 scrollVector;
		Texture2D selectedColorTexture;
		Texture2D normalColorTexture;
		[MenuItem("Tools/THEBADDEST/Database/Database Editor %&d")]
		public static void ShowWindow()
		{
			var window = GetWindow<DatabaseEditor>("Game Database");
			window.Show();
		}

		void OnEnable()
		{
			string[] guids = AssetDatabase.FindAssets("t:Database");
			if (guids.Length > 0)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[0]);
				database = AssetDatabase.LoadAssetAtPath<Database>(path);
				if (database != null)
				{
					serializedDatabase = new SerializedObject(database);
					tablesProperty = serializedDatabase.FindProperty("tables");
				}
			}
			else
			{
				database = null;
			}
			tableTypes = AppDomain.CurrentDomain.GetAssemblies()
								  .SelectMany(assembly => assembly.GetTypes())
								  .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(TableBase)))
								  .ToArray();
			 // selectedColorTexture = EditorUtils.ColorToTexture2D(new Color(0f, 0f, 1f, 0.2f));
			 selectedColorTexture = EditorUtils.ColorToTexture2D(new Color(1f, 1f, 1f, 0.2f));
			 normalColorTexture = EditorUtils.ColorToTexture2D(new Color(0.1f, 0.1f, 0.1f, 0.2f));
		}

		void OnGUI()
		{
			DrawTitle();
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical(EditorUtils.Window,GUILayout.Width(150), GUILayout.ExpandHeight(true));
			DrawLeftSide();
			GUILayout.EndVertical();
			GUILayout.BeginVertical(EditorUtils.Window);
			DrawRightSide();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
	
			
		}
		void DrawTitle()
		{
			EditorGUILayout.Space();
			GUILayout.BeginVertical(EditorUtils.Window,GUILayout.ExpandWidth(true), GUILayout.Height(100));
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var titleStyle = new GUIStyle(GUI.skin.label)
			{ 
				fontSize = 28,
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleLeft,
				normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
			};
			GUILayout.Label("Game Database", titleStyle, GUILayout.Height(50));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			GUILayout.Label("Version 4.0b - Developed by Umair Saifullah", new GUIStyle() { alignment = TextAnchor.LowerRight, fontStyle = FontStyle.Italic, normal = { textColor = Color.gray } });
			EditorGUILayout.Space();
			GUILayout.EndVertical();
			EditorGUILayout.Space(10);
		}
		void DrawLeftSide()
		{

			EditorUtils.DrawHeader("Table Names");
			if (tablesProperty == null || tablesProperty.arraySize == 0)
				return;
			
			var selectedStyle = new GUIStyle(GUI.skin.button) { 
				normal =
				{
					background = normalColorTexture,
				},
				border = new RectOffset(-1, -1, -1, -1),
				active = { background = selectedColorTexture, },
			};
			var pressedStyle = new GUIStyle(GUI.skin.button) { 
				normal =
				{
					background = selectedColorTexture,
				},
				border = new RectOffset(-1, -1, -1, -1),
			};
			for (int i = 0; i < tablesProperty.arraySize; i++)
			{
				var tableProp = tablesProperty.GetArrayElementAtIndex(i);
				var tableObj = tableProp.objectReferenceValue as TableBase;
				if (tableObj == null) continue;
				if (GUILayout.Button(tableObj.name,selectedIndex==i?pressedStyle:selectedStyle,GUILayout.Height(30)))
				{
					selectedIndex = i;
				}
			}
		}
		
		void DrawRightSide()
		{
			EditorUtils.DrawHeader("Table details");
			if (tablesProperty == null || tablesProperty.arraySize == 0)
				return;
			var tableProp = tablesProperty.GetArrayElementAtIndex(selectedIndex);
			var tableObj = tableProp.objectReferenceValue as TableBase;
			if (tableObj == null) return;
			Editor editor = Editor.CreateEditor(tableObj);
			if (editor != null)
			{
				scrollVector=EditorGUILayout.BeginScrollView(scrollVector);
				editor.OnInspectorGUI();
				EditorGUILayout.EndScrollView();
			}
		}
	}
	
}

