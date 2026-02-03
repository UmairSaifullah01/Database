using System.Collections.Generic;
using THEBADDEST.EditorTools;
using UnityEditor;
using UnityEngine;


namespace THEBADDEST.DatabaseModule
{


	public class DatabaseEditor : EditorWindow
	{
		private Database database;
		private SerializedObject serializedDatabase;

		private List<IDatabaseComponent> allComponents = new List<IDatabaseComponent>();
		private int selectedComponentIndex = -1;
		private Dictionary<Object, Editor> componentEditors = new Dictionary<Object, Editor>();
		private Vector2 scrollPosition;

		private Texture2D selectedColorTexture;
		private Texture2D normalColorTexture;

		[MenuItem("Tools/THEBADDEST/Database/Database Editor %&d")]
		public static void ShowWindow()
		{
			var window = GetWindow<DatabaseEditor>("Game Database");
			window.Show();
		}

		void OnEnable()
		{
			// Find or create the GameDatabase
			database = DatabaseEditorUtility.FindOrCreateDatabase();

			if (database != null)
			{
				// Refresh database tables
				DatabaseEditorUtility.RefreshDatabaseTables(database);

				serializedDatabase = new SerializedObject(database);
			}

			selectedColorTexture = EditorUtils.ColorToTexture2D(new Color(1f, 1f, 1f, 0.2f));
			normalColorTexture = EditorUtils.ColorToTexture2D(new Color(0.1f, 0.1f, 0.1f, 0.2f));

			RefreshComponents();
		}

		void OnDisable()
		{
			// Clean up cached editors
			foreach (var kvp in componentEditors)
			{
				if (kvp.Value != null)
				{
					DestroyImmediate(kvp.Value);
				}
			}
			componentEditors.Clear();
			allComponents.Clear();
		}

		void RefreshComponents()
		{
			allComponents.Clear();
			if (database != null)
			{
				allComponents.AddRange(database.GetAllComponents());
			}

			// Reset selection if out of bounds
			if (selectedComponentIndex >= allComponents.Count)
			{
				selectedComponentIndex = allComponents.Count > 0 ? 0 : -1;
			}
		}

		void OnGUI()
		{
			if (serializedDatabase != null)
			{
				serializedDatabase.Update();
			}

			// Refresh components if database changed
			if (database != null)
			{
				var currentCount = allComponents.Count;
				RefreshComponents();
				if (currentCount != allComponents.Count)
				{
					// Components changed, reset selection
					selectedComponentIndex = allComponents.Count > 0 ? 0 : -1;
				}
			}

			DrawTitle();
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical(EditorUtils.Window, GUILayout.Width(200), GUILayout.ExpandHeight(true));
			DrawComponentsList();
			GUILayout.EndVertical();
			GUILayout.BeginVertical(EditorUtils.Window);
			DrawComponentDetails();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			if (serializedDatabase != null)
			{
				serializedDatabase.ApplyModifiedProperties();
			}
		}
		void DrawTitle()
		{
			EditorGUILayout.Space();
			GUILayout.BeginVertical(EditorUtils.Window, GUILayout.ExpandWidth(true), GUILayout.Height(100));
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
			GUILayout.Label("Version 2.0 - Developed by Umair Saifullah", new GUIStyle() { alignment = TextAnchor.LowerRight, fontStyle = FontStyle.Italic, normal = { textColor = Color.gray } });
			EditorGUILayout.Space();
			GUILayout.EndVertical();
			EditorGUILayout.Space(10);
		}
		void DrawComponentsList()
		{
			EditorUtils.DrawHeader("Components");

			if (allComponents.Count == 0)
			{
				EditorGUILayout.HelpBox("No database components registered.\n\nAdd tables, components, or single entity components to the Database asset.", MessageType.Info);
				return;
			}

			var selectedStyle = new GUIStyle(GUI.skin.button)
			{
				normal =
				{
					background = normalColorTexture,
				},
				border = new RectOffset(-1, -1, -1, -1),
				active = { background = selectedColorTexture, },
			};
			var pressedStyle = new GUIStyle(GUI.skin.button)
			{
				normal =
				{
					background = selectedColorTexture,
				},
				border = new RectOffset(-1, -1, -1, -1),
			};

			for (int i = 0; i < allComponents.Count; i++)
			{
				var component = allComponents[i];
				if (component == null) continue;

				string displayName = component.GetComponentName();
				if (string.IsNullOrEmpty(displayName))
				{
					displayName = component.GetType().Name;
				}

				if (GUILayout.Button(displayName, selectedComponentIndex == i ? pressedStyle : selectedStyle, GUILayout.Height(30)))
				{
					selectedComponentIndex = i;
				}
			}
		}

		void DrawComponentDetails()
		{
			EditorUtils.DrawHeader("Details");

			if (allComponents.Count == 0 || selectedComponentIndex < 0 || selectedComponentIndex >= allComponents.Count)
			{
				EditorGUILayout.HelpBox("Select a component from the list on the left.", MessageType.Info);
				return;
			}

			var component = allComponents[selectedComponentIndex];
			if (component == null)
			{
				EditorGUILayout.HelpBox("Selected component is not available.", MessageType.Warning);
				return;
			}

			Object targetObject = null;
			Editor editor = null;

			// Determine the target object based on component type
			if (component is TableBase tableBase)
			{
				targetObject = tableBase;
			}
			else if (component is DatabaseComponent databaseComponent)
			{
				targetObject = databaseComponent.TargetScriptable;
				if (targetObject == null)
				{
					EditorGUILayout.HelpBox("This DatabaseComponent has no ScriptableObject assigned.", MessageType.Info);
					EditorGUILayout.Space();
					EditorGUILayout.ObjectField("Target Scriptable", databaseComponent.TargetScriptable, typeof(ScriptableObject), false);
					return;
				}
			}
			else if (component is SingleEntityComponentBase singleEntityComponentBase)
			{
				// For SingleEntityComponent, show the component itself (which contains the serialized data field)
				targetObject = singleEntityComponentBase;
			}

			if (targetObject == null)
			{
				EditorGUILayout.HelpBox($"Unknown component type: {component.GetType().Name}", MessageType.Warning);
				return;
			}

			// Get or create editor for the target object
			if (!componentEditors.TryGetValue(targetObject, out editor) || editor == null || editor.target != targetObject)
			{
				if (editor != null)
				{
					DestroyImmediate(editor);
				}
				editor = Editor.CreateEditor(targetObject);
				componentEditors[targetObject] = editor;
			}

			if (editor != null)
			{
				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
				editor.OnInspectorGUI();
				EditorGUILayout.EndScrollView();
			}
		}
	}

}

