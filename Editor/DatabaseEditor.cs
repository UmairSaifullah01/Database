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

    [MenuItem("Tools/THEBADDEST/Database/Database Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<DatabaseEditor>("Database Editor");
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
        GUILayout.Label("Database Editor", EditorStyles.boldLabel);

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

        // Add New Table area at the very top, single line
        EditorGUILayout.BeginHorizontal();
        newTableName = EditorGUILayout.TextField(newTableName, GUILayout.Width(180));
        if (tableTypes.Length == 0)
        {
            EditorGUILayout.LabelField("No TableBase-derived classes found.", GUILayout.Width(220));
        }
        else
        {
            selectedTypeIndex = EditorGUILayout.Popup(selectedTypeIndex, tableTypes.Select(t => t.Name).ToArray(), GUILayout.Width(180));
        }
        if (GUILayout.Button("Create and Add Table", GUILayout.Width(180)))
        {
            CreateAndAddTable();
        }
        if (GUILayout.Button("Refresh", GUILayout.Width(100)))
        {
            DatabaseEditorUtility.AutoRegisterTables();
            OnEnable();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(16);

        // Draw tab bar for tables with wrapping
        float windowWidth = position.width - 20;
        float currentRowWidth = 0;
        float tabWidth = 150;
        float tabSpacing = 4;
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

            // Draw tab as a box with two buttons inside
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(tabWidth));
            EditorGUILayout.BeginHorizontal();
            float mainBtnWidth = tabWidth * 0.8f;
            float xBtnWidth = tabWidth * 0.2f;

            GUIStyle mainTabStyle = new GUIStyle(GUI.skin.button);
            if (i == selectedTableIndex)
            {
                mainTabStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0.4f, 0.8f, 1f)); // blue
                mainTabStyle.normal.textColor = Color.white;
            }
            else
            {
                mainTabStyle.normal.background = MakeTex(2, 2, new Color(0.3f, 0.3f, 0.3f, 1f)); // gray
                mainTabStyle.normal.textColor = Color.white;
            }

            if (GUILayout.Button(tableObj.GetTableName(), mainTabStyle, GUILayout.Width(mainBtnWidth)))
            {
                selectedTableIndex = i;
            }

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("x", GUILayout.Width(xBtnWidth)))
            {
                tablesProperty.DeleteArrayElementAtIndex(i);
                if (selectedTableIndex >= i && selectedTableIndex > 0)
                    selectedTableIndex--;
                serializedDatabase.ApplyModifiedProperties();
                EditorUtility.SetDirty(database);
                GUI.backgroundColor = prevBg;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            GUI.backgroundColor = prevBg;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            currentRowWidth += tabWidth + tabSpacing;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Show inspector for selected table
        if (tablesProperty.arraySize > 0 && selectedTableIndex < tablesProperty.arraySize)
        {
            var tableProp = tablesProperty.GetArrayElementAtIndex(selectedTableIndex);
            var tableObj = tableProp.objectReferenceValue as TableBase;
            if (tableObj != null)
            {
                GUILayout.Label(tableObj.GetTableName() + " Details", EditorStyles.boldLabel);
                inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll, GUILayout.ExpandHeight(true));
                Editor editor = Editor.CreateEditor(tableObj);
                if (editor != null)
                {
                    editor.OnInspectorGUI();
                }
                EditorGUILayout.EndScrollView();
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
} 