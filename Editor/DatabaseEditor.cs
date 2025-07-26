using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using THEBADDEST.DatabaseModule;

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
        // Load the UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Database/Editor/UI.uxml");
        if (visualTree == null)
        {
            Debug.LogError("Could not load UI.uxml file. Make sure it exists at Assets/Database/Editor/UI.uxml");
            return;
        }

        VisualElement root = visualTree.CloneTree();
        rootVisualElement.Add(root);

        // Query UI elements
        var initButton = root.Q<Button>("InitButton");
        var tableButton = root.Q<Button>("TableButton");
        var refreshButton = root.Q<Button>("Refresh");
        tabsContainer = root.Q<VisualElement>("Tabs");
        if (tabsContainer != null)
        {
            tabsContainer.AddToClassList("tabs-container");
        }
        editorView = root.Q<ScrollView>("EditorView");
        // Initialize database
        OnEnable();

        // Wire up button callbacks
        if (initButton != null)
        {
            initButton.clicked += OnInitializeClicked;
        }

        if (tableButton != null)
        {
            tableButton.clicked += OnTableClicked;
        }

        if (refreshButton != null)
        {
            refreshButton.clicked += OnRefreshClicked;
        }

        // Create table controls container
         CreateTableControls(root);

        // Initial population
        PopulateTabs();
        UpdateEditorView();
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
        PopulateTabs();
        UpdateEditorView();
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
        PopulateTabs();
        UpdateEditorView();
    }

    private void PopulateTabs()
    {
        if (tabsContainer == null) return;

        tabsContainer.Clear();

        // Prepare the data source
        var tableList = new List<TableBase>();
        if (tablesProperty != null)
        {
            for (int i = 0; i < tablesProperty.arraySize; i++)
            {
                var tableProp = tablesProperty.GetArrayElementAtIndex(i);
                var tableObj = tableProp.objectReferenceValue as TableBase;
                if (tableObj != null)
                    tableList.Add(tableObj);
            }
        }

        for (int i = 0; i < tableList.Count; i++)
        {
            // Create a new container similar to the template
            var tabContainer = new VisualElement();
            tabContainer.AddToClassList("tab-container");
            
            // Create tab button
            var tabButton = new Button();
            tabButton.name = "TabButton";
            tabButton.AddToClassList("tab-button");
            
            // Create close button
            var closeButton = new Button();
            closeButton.name = "Crossbutton";
            closeButton.text = "x";
            closeButton.AddToClassList("close-button");
            
            // Add buttons to container
            tabContainer.Add(tabButton);
            tabContainer.Add(closeButton);
            
            tabButton.text = tableList[i].GetTableName();
            
            int index = i;
            tabButton.clicked += () =>
            {
                selectedTableIndex = index;
                UpdateEditorView();
                UpdateTabSelection();
            };
            
            closeButton.clicked += () =>
            {
                tablesProperty.DeleteArrayElementAtIndex(index);
                if (selectedTableIndex >= index && selectedTableIndex > 0)
                    selectedTableIndex--;
                serializedDatabase.ApplyModifiedProperties();
                EditorUtility.SetDirty(database);
                PopulateTabs();
                UpdateEditorView();
            };
            
            // Highlight selected
            if (index == selectedTableIndex)
                tabButton.AddToClassList("tab-button-selected");
            else
                tabButton.RemoveFromClassList("tab-button-selected");

            tabsContainer.Add(tabContainer);
        }
    }

    private void UpdateTabSelection()
    {
        for (int i = 0; i < tabsContainer.childCount; i++)
        {
            var container = tabsContainer[i] as VisualElement;
            if (container == null) continue;
            
            var tabButton = container.Q<Button>("TabButton");
            if (tabButton == null) continue;
            
            if (i == selectedTableIndex)
                tabButton.AddToClassList("tab-button-selected");
            else
                tabButton.RemoveFromClassList("tab-button-selected");
        }
    }

    private void UpdateEditorView()
    {
        if (editorView == null) return;

        editorView.Clear();

        if (database == null || serializedDatabase == null || tablesProperty == null)
        {
            var noDbLabel = new Label("No Database found. Click Initialize to create one.");
            noDbLabel.AddToClassList("no-db-label");
            editorView.Add(noDbLabel);
            return;
        }

        if (tablesProperty.arraySize == 0)
        {
            var noTablesLabel = new Label("No tables found. Create some tables to get started.");
            noTablesLabel.AddToClassList("no-tables-label");
            editorView.Add(noTablesLabel);
            return;
        }

        if (selectedTableIndex >= tablesProperty.arraySize)
        {
            selectedTableIndex = 0;
        }

        var tableProp = tablesProperty.GetArrayElementAtIndex(selectedTableIndex);
        var tableObj = tableProp.objectReferenceValue as TableBase;
        if (tableObj == null)
        {
            var invalidTableLabel = new Label("Invalid table selected.");
            invalidTableLabel.AddToClassList("invalid-table-label");
            editorView.Add(invalidTableLabel);
            return;
        }

        // Create a container for the table details
        var detailsContainer = new VisualElement();
        detailsContainer.AddToClassList("details-container");
        // Inspector
        var inspectorContainer = new IMGUIContainer(() =>
        {
            Editor editor = Editor.CreateEditor(tableObj);
            if (editor != null)
            {
                editor.OnInspectorGUI();
            }
        });
        inspectorContainer.style.flexGrow = 1;
        inspectorContainer.style.minHeight = 200;
        detailsContainer.Add(inspectorContainer);

        editorView.Add(detailsContainer);
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
        if (string.IsNullOrWhiteSpace(newTableName))
        {
            Debug.LogError("Table name cannot be empty.");
            return;
        }

        if (tableTypes == null || tableTypes.Length == 0)
        {
            Debug.LogError("No table types found.");
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

        // Update UI
        PopulateTabs();
        UpdateEditorView();
    }
}