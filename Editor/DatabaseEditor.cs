using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using THEBADDEST.DatabaseModule;

public class DatabaseEditor : EditorWindow
{
    private Database database;
    private SerializedObject serializedDatabase;
    private SerializedProperty tablesProperty;
    private int selectedTableIndex = 0;
    private Vector2 inspectorScroll;

    private Type[] tableTypes;
    private int selectedTypeIndex;
    private string newTableName = "NewTable";
    private bool showTableControls = false;
    // Add this helper for hover effect
    private int hoveredTabIndex = -1;

    [MenuItem("Tools/THEBADDEST/Database/Database Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<DatabaseEditor>("Game Database");
        window.Show();
    }

    private void OnEnable()
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
    }

    private void OnGUI()
    {
        // Main title
        GUILayout.Space(8);
        GUIStyle mainTitleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
        GUILayout.Label("Game Database", mainTitleStyle, GUILayout.ExpandWidth(true));
        GUILayout.Space(8);
        if (database == null)
        {
            EditorGUILayout.HelpBox("No Database asset found. Please create one using the DatabaseEditorUtility.", MessageType.Warning);
            if (GUILayout.Button("Create Database Asset"))
            {
                DatabaseEditorUtility.InitializeDatabase();
                OnEnable();
            }
            return;
        }

        if (serializedDatabase == null || tablesProperty == null)
        {
            EditorGUILayout.HelpBox("Could not load serialized Database.", MessageType.Error);
            return;
        }

        serializedDatabase.Update();

        // Top bar: Initialize, table dropdown, refresh
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Initialize", GUILayout.Width(100)))
        {
            DatabaseEditorUtility.InitializeDatabase();
            OnEnable();
        }
        if (GUILayout.Button("Table",GUILayout.Width(100) ))
        {
            showTableControls = !showTableControls;
        }
        if (GUILayout.Button("Refresh", GUILayout.Width(100)))
        {
            DatabaseEditorUtility.AutoRegisterTables();
            OnEnable();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(8);
        // Show table creation controls and Create Table Class only when toggled
        if (showTableControls)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Table Name :", GUILayout.Width(70));
            newTableName = EditorGUILayout.TextField(newTableName,GUILayout.Width(180));
            if (tableTypes.Length > 0)
            {
                selectedTypeIndex = EditorGUILayout.Popup(selectedTypeIndex, tableTypes.Select(t => t.Name).ToArray(), GUILayout.Width(180));
            }
            
            if (GUILayout.Button("Create and Add Table", GUILayout.Width(180)))
            {
                CreateAndAddTable();
            }
            GUILayout.Space(8);
            Rect vSepRect = GUILayoutUtility.GetRect(2, 24, GUILayout.Width(2), GUILayout.Height(24));
            EditorGUI.DrawRect(vSepRect, new Color(0.3f, 0.3f, 0.3f, 1f));
            GUILayout.Space(8);
            if (GUILayout.Button("Create Table Class", GUILayout.Width(150)))
            {
                DatabaseEditorUtility.CreateTableDriveClass();
                OnEnable();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        // Draw tab bar for tables with wrapping
        float windowWidth = position.width - 20;
        float currentRowWidth = 0;
        float tabWidth = 150;
        float tabSpacing = 2;
        float tabHeight = 22;
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < tablesProperty.arraySize; i++)
        {
            var tableProp = tablesProperty.GetArrayElementAtIndex(i);
            var tableObj = tableProp.objectReferenceValue as TableBase;
            if (tableObj == null) continue;

            if (currentRowWidth + tabWidth > windowWidth)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                currentRowWidth = 0;
            }

            // Native Unity tab style
            GUIStyle tabStyle = EditorStyles.toolbarButton;
            EditorGUILayout.BeginHorizontal(GUILayout.Width(tabWidth), GUILayout.Height(tabHeight));
            if (GUILayout.Button(tableObj.GetTableName(), tabStyle, GUILayout.Width(tabWidth - 20), GUILayout.Height(tabHeight)))
            {
                selectedTableIndex = i;
            }
            // Native mini close button
            if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(18), GUILayout.Height(tabHeight)))
            {
                tablesProperty.DeleteArrayElementAtIndex(i);
                if (selectedTableIndex >= i && selectedTableIndex > 0)
                    selectedTableIndex--;
                serializedDatabase.ApplyModifiedProperties();
                EditorUtility.SetDirty(database);
                break;
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(tabSpacing);
            currentRowWidth += tabWidth + tabSpacing;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(14);

        // Section title for table details
        GUIStyle sectionTitleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleLeft };
        GUILayout.Label("Table Details", sectionTitleStyle);

        // Show inspector for selected table in a boxed area with colored header
        if (tablesProperty.arraySize > 0 && selectedTableIndex < tablesProperty.arraySize)
        {
            var tableProp = tablesProperty.GetArrayElementAtIndex(selectedTableIndex);
            var tableObj = tableProp.objectReferenceValue as TableBase;
            if (tableObj != null)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                Rect headerRect = GUILayoutUtility.GetRect(position.width - 32, 32, GUILayout.ExpandWidth(true));
                EditorGUI.DrawRect(headerRect, new Color(0.18f, 0.22f, 0.32f, 1f));
                GUI.Label(headerRect, tableObj.GetTableName() + " Details", new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleLeft, fontSize = 14, normal = { textColor = Color.white } });
                GUILayout.Space(4);
                inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll, GUILayout.ExpandHeight(true), GUILayout.MinHeight(200));
                Editor editor = Editor.CreateEditor(tableObj);
                if (editor != null)
                {
                    editor.OnInspectorGUI();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No table selected.", MessageType.Info, true);
        }

        GUILayout.Space(10);
        serializedDatabase.ApplyModifiedProperties();
        EditorUtility.SetDirty(database);
    }
    private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
            pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
    private void CreateAndAddTable()
    {
        if (string.IsNullOrWhiteSpace(newTableName))
        {
            Debug.LogError("Table name cannot be empty.");
            return;
        }
        var tableType = tableTypes[selectedTypeIndex];
        var asset = ScriptableObject.CreateInstance(tableType);
        asset.name = newTableName;
        string dbPath = AssetDatabase.GetAssetPath(database);
        string folder = System.IO.Path.GetDirectoryName(dbPath);
        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{newTableName}.asset");
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        tablesProperty.arraySize++;
        tablesProperty.GetArrayElementAtIndex(tablesProperty.arraySize - 1).objectReferenceValue = asset;
        serializedDatabase.ApplyModifiedProperties();
        EditorUtility.SetDirty(database);
        selectedTableIndex = tablesProperty.arraySize - 1;
        Debug.Log($"Created and added table '{newTableName}' to the Database.");
    }
    // Add this helper for a modern tab style
    private GUIStyle GetTabStyle(bool isActive, bool isHover)
    {
        var style = new GUIStyle(GUI.skin.button);
        style.margin = new RectOffset(8, 8, 8, 8);
        style.padding = new RectOffset(18, 18, 6, 6);
        style.border = new RectOffset(16, 16, 16, 16);
        style.fixedHeight = 36;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 14;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.hover.textColor = Color.white;
        style.active.textColor = Color.white;
        Color baseColor = isActive ? new Color(0.18f, 0.42f, 1f, 1f) : (isHover ? new Color(0.25f, 0.25f, 0.4f, 1f) : new Color(0.22f, 0.22f, 0.22f, 1f));
        style.normal.background = MakeTex(2, 2, baseColor);
        style.hover.background = MakeTex(2, 2, baseColor * 1.1f);
        style.active.background = MakeTex(2, 2, baseColor * 0.95f);
        return style;
    }
} 