using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using THEBADDEST.DatabaseModule;

public static class EditorAssetLoader
{
    public static VisualTreeAsset LoadUXML(string relativePath)
    {
        string packagePath = $"Packages/com.thebaddest.databasemodule/{relativePath}";
        var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(packagePath);
        if (asset != null)
            return asset;
        string assetsPath = $"Assets/Database/{relativePath}";
        asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetsPath);
        return asset;
    }
    public static StyleSheet LoadUSS(string relativePath)
    {
        string packagePath = $"Packages/com.thebaddest.databasemodule/{relativePath}";
        var asset = AssetDatabase.LoadAssetAtPath<StyleSheet>(packagePath);
        if (asset != null)
            return asset;
        string assetsPath = $"Assets/Database/{relativePath}";
        asset = AssetDatabase.LoadAssetAtPath<StyleSheet>(assetsPath);
        return asset;
    }
}

public class DatabaseEditor : EditorWindow
{
    private Database database;
    private SerializedObject serializedDatabase;
    private SerializedProperty tablesProperty;
    private int selectedTableIndex = 0;

    private Type[] tableTypes;
    private int selectedTypeIndex;
    private string newTableName = "NewTable";
    private bool showTableControls = false;

    // UI Toolkit elements
    private VisualElement tabsContainer;
    private ScrollView editorView;
    private VisualElement tableControlsContainer;
    private TextField tableNameField;
    private DropdownField tableTypeDropdown;
    private Button createTableButton;
    private Button createTableClassButton;
    private VisualElement tablesBox;
    private VisualElement inspectorContent;
    private Button addTableButton;

    [MenuItem("Tools/THEBADDEST/Database/Database Editor")]
#if UNITY_2021_2_OR_NEWER
    [UnityEditor.ShortcutManagement.Shortcut("THEBADDEST/Database Editor", KeyCode.D, UnityEditor.ShortcutManagement.ShortcutModifiers.Action | UnityEditor.ShortcutManagement.ShortcutModifiers.Alt)]
#endif
    public static void ShowWindow()
    {
        var window = GetWindow<DatabaseEditor>("Game Database");
        window.Show();
    }

    public void CreateGUI()
    {
        var visualTree = EditorAssetLoader.LoadUXML("Editor/UI.uxml");
        if (visualTree == null)
        {
            Debug.LogError("Could not load UI.uxml file. Make sure it exists at Assets/Database/Editor/UI.uxml");
            return;
        }
        VisualElement root = visualTree.CloneTree();
        rootVisualElement.Add(root);

        // Query new UI elements
        tablesBox = root.Q<VisualElement>("tablesBox");
        inspectorContent = root.Q<VisualElement>("inspectorContent");
        addTableButton = root.Q<Button>("AddTableButton");

        // Setup add table button
        if (addTableButton != null)
            addTableButton.clicked += CreateAndAddTable;

        OnEnable();
        PopulateTableList();
        ShowTableInspector();

        var styleSheet = EditorAssetLoader.LoadUSS("Editor/DatabaseEditor.uss");
        if (styleSheet != null)
            rootVisualElement.styleSheets.Add(styleSheet);
    }

    private void CreateTableControls(VisualElement root)
    {
        // Create a container for table creation controls
        tableControlsContainer = rootVisualElement.Q<VisualElement>("TableCreationContainer");

        // Table name field

        tableNameField = tableControlsContainer.Q<TextField>("TableTextField");
        tableNameField.value = newTableName;
        tableNameField.RegisterValueChangedCallback(evt => newTableName = evt.newValue);

        // Table type dropdown
        if (tableTypes != null && tableTypes.Length > 0)
        {
            var typeNames = tableTypes.Select(t => t.Name).ToList();
            tableTypeDropdown = tableControlsContainer.Q<DropdownField>("TableTypeDropdown");
            tableTypeDropdown.choices = typeNames;
            tableTypeDropdown.RegisterValueChangedCallback(evt =>
            {
                selectedTypeIndex = typeNames.FindIndex(t => t == evt.newValue);
            });
        }

        // Create table button
        createTableButton = tableControlsContainer.Q<Button>("CreateTable");
        createTableButton.clicked += CreateAndAddTable;

        // Create table class button
        createTableClassButton = tableControlsContainer.Q<Button>("CreateTableClass");
        createTableClassButton.clicked += () =>
        {
            DatabaseEditorUtility.CreateTableDriveClass();
            OnEnable();
        };
    }

    private void OnInitializeClicked()
    {
        DatabaseEditorUtility.InitializeDatabase();
        OnEnable();
        PopulateTableList();
        ShowTableInspector();
    }

    private void OnTableClicked()
    {
        showTableControls = !showTableControls;
        var tableCreationContainer = rootVisualElement.Q<VisualElement>("TableCreationContainer");
        if (tableCreationContainer != null)
        {
            if (showTableControls)
                tableCreationContainer.RemoveFromClassList("hidden");
            else
                tableCreationContainer.AddToClassList("hidden");
        }
    }

    private void OnRefreshClicked()
    {
        DatabaseEditorUtility.AutoRegisterTables();
        OnEnable();
        PopulateTableList();
        ShowTableInspector();
    }

    private void PopulateTableList()
    {
        if (tablesBox == null) return;
        tablesBox.Clear();
        if (tablesProperty == null || tablesProperty.arraySize == 0)
            return;
        for (int i = 0; i < tablesProperty.arraySize; i++)
        {
            var tableProp = tablesProperty.GetArrayElementAtIndex(i);
            var tableObj = tableProp.objectReferenceValue as TableBase;
            if (tableObj == null) continue;
            var row = new VisualElement();
            row.AddToClassList("table-list-item");
            // Icon
            var icon = new VisualElement();
            icon.AddToClassList("table-icon");
            row.Add(icon);
            // Name label
            var label = new Label(tableObj.GetTableName());
            label.AddToClassList("table-list-label");
            row.Add(label);
            // Remove button
            var removeBtn = new Button();
            removeBtn.AddToClassList("table-remove-button");
            removeBtn.clicked += () => {
                tablesProperty.DeleteArrayElementAtIndex(i);
                serializedDatabase.ApplyModifiedProperties();
                EditorUtility.SetDirty(database);
                if (selectedTableIndex >= i && selectedTableIndex > 0)
                    selectedTableIndex--;
                PopulateTableList();
                ShowTableInspector();
            };
            row.Add(removeBtn);
            // Selection logic
            int index = i;
            row.RegisterCallback<MouseDownEvent>((evt) => {
                selectedTableIndex = index;
                ShowTableInspector();
                PopulateTableList();
            });
            if (i == selectedTableIndex)
                row.AddToClassList("selected");
            tablesBox.Add(row);
        }
    }

    private void ShowTableInspector()
    {
        if (inspectorContent == null)
            return;
        inspectorContent.Clear();
        if (tablesProperty == null || tablesProperty.arraySize == 0)
            return;
        if (selectedTableIndex >= tablesProperty.arraySize)
            selectedTableIndex = 0;
        var tableProp = tablesProperty.GetArrayElementAtIndex(selectedTableIndex);
        var tableObj = tableProp.objectReferenceValue as TableBase;
        if (tableObj == null)
            return;
        var inspector = new IMGUIContainer(() => {
            Editor editor = Editor.CreateEditor(tableObj);
            if (editor != null)
                editor.OnInspectorGUI();
        });
        inspectorContent.Add(inspector);
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

    private void CreateAndAddTable()
    {
        if (tableTypes == null || tableTypes.Length == 0)
        {
            Debug.LogError("No table types found.");
            return;
        }
        var tableType = tableTypes[0]; // Use first type for simplicity
        var asset = ScriptableObject.CreateInstance(tableType);
        asset.name = "NewTable";
        string dbPath = AssetDatabase.GetAssetPath(database);
        string folder = System.IO.Path.GetDirectoryName(dbPath);
        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folder}/NewTable.asset");
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        tablesProperty.arraySize++;
        tablesProperty.GetArrayElementAtIndex(tablesProperty.arraySize - 1).objectReferenceValue = asset;
        serializedDatabase.ApplyModifiedProperties();
        EditorUtility.SetDirty(database);
        selectedTableIndex = tablesProperty.arraySize - 1;
        PopulateTableList();
        ShowTableInspector();
    }
}