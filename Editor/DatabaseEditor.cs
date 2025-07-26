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
    private ListView tabsContainer;
    private VisualElement editorView;
    private VisualElement tableControlsContainer;
    private TextField tableNameField;
    private DropdownField tableTypeDropdown;
    private Button createTableButton;
    private Button createTableClassButton;

    [MenuItem("Tools/THEBADDEST/Database/Database Editor")]
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
        tabsContainer = root.Q<ListView>("Tabs");
        editorView = root.Q<VisualElement>("EditorView");
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
        tableControlsContainer = new VisualElement();
        tableControlsContainer.style.display = DisplayStyle.None;
        tableControlsContainer.style.marginTop = 8;
        tableControlsContainer.style.marginBottom = 8;
        tableControlsContainer.style.paddingTop = 8;
        tableControlsContainer.style.paddingBottom = 8;
        tableControlsContainer.style.borderTopWidth = 1;
        tableControlsContainer.style.borderTopColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        // Table name field
        var nameContainer = new VisualElement();
        nameContainer.style.flexDirection = FlexDirection.Row;
        nameContainer.style.alignItems = Align.Center;
        nameContainer.style.marginBottom = 4;

        var nameLabel = new Label("Table Name:");
        nameLabel.style.width = 70;
        nameContainer.Add(nameLabel);

        tableNameField = new TextField();
        tableNameField.value = newTableName;
        tableNameField.style.width = 180;
        tableNameField.RegisterValueChangedCallback(evt => newTableName = evt.newValue);
        nameContainer.Add(tableNameField);

        // Table type dropdown
        if (tableTypes != null && tableTypes.Length > 0)
        {
            var typeNames = tableTypes.Select(t => t.Name).ToList();
            tableTypeDropdown = new DropdownField();
            tableTypeDropdown.choices = typeNames;
            tableTypeDropdown.style.width = 180;
            tableTypeDropdown.RegisterValueChangedCallback(evt =>
            {
                selectedTypeIndex = typeNames.FindIndex(t => t == evt.newValue);
            });
            nameContainer.Add(tableTypeDropdown);
        }

        // Create table button
        createTableButton = new Button(CreateAndAddTable);
        createTableButton.text = "Create and Add Table";
        createTableButton.style.width = 180;
        nameContainer.Add(createTableButton);

        tableControlsContainer.Add(nameContainer);

        // Create table class button
        var classButtonContainer = new VisualElement();
        classButtonContainer.style.flexDirection = FlexDirection.Row;
        classButtonContainer.style.alignItems = Align.Center;
        classButtonContainer.style.marginTop = 4;

        createTableClassButton = new Button(() =>
        {
            DatabaseEditorUtility.CreateTableDriveClass();
            OnEnable();
        });
        createTableClassButton.text = "Create Table Class";
        createTableClassButton.style.width = 150;
        classButtonContainer.Add(createTableClassButton);

        tableControlsContainer.Add(classButtonContainer);

        // Add the container after the buttons
        var buttonContainer = root.Q<VisualElement>();
        if (buttonContainer != null)
        {
            buttonContainer.parent.Insert(buttonContainer.parent.IndexOf(buttonContainer) + 1, tableControlsContainer);
        }
        else
        {
            root.Insert(2, tableControlsContainer); // Insert after title and buttons
        }
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
        if (tableControlsContainer != null)
        {
            tableControlsContainer.style.display = showTableControls ? DisplayStyle.Flex : DisplayStyle.None;
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

        tabsContainer.itemsSource = tableList;

        tabsContainer.makeItem = () =>
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            var btn = new Button();
            btn.style.width = 110;
            btn.style.height = 22;
            btn.AddToClassList("tab-button");
            var closeBtn = new Button();
            closeBtn.text = "x";
            closeBtn.style.width = 18;
            closeBtn.style.height = 22;
            closeBtn.AddToClassList("close-button");
            container.Add(btn);
            container.Add(closeBtn);
            return container;
        };

        tabsContainer.bindItem = (element, i) =>
        {
            var container = (VisualElement)element;
            var btn = (Button)container.ElementAt(0);
            var closeBtn = (Button)container.ElementAt(1);
            btn.text = tableList[i].GetTableName();
            btn.clicked -= null;
            closeBtn.clicked -= null;
            int index = i;
            btn.clicked += () =>
            {
                selectedTableIndex = index;
                UpdateEditorView();
                tabsContainer.selectedIndex = index;
            };
            closeBtn.clicked += () =>
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
                btn.AddToClassList("tab-button-selected");
            else
                btn.RemoveFromClassList("tab-button-selected");
        };

        tabsContainer.selectionType = SelectionType.Single;
        tabsContainer.selectedIndex = selectedTableIndex;
    }

    private void UpdateTabSelection()
    {
        var tabButtons = tabsContainer.Query<Button>().ToList();
        for (int i = 0; i < tabButtons.Count; i++)
        {
            if (i == selectedTableIndex)
            {
                tabButtons[i].AddToClassList("tab-button-selected");
            }
            else
            {
                tabButtons[i].RemoveFromClassList("tab-button-selected");
            }
        }
    }

    private void UpdateEditorView()
    {
        if (editorView == null) return;

        editorView.Clear();

        if (database == null || serializedDatabase == null || tablesProperty == null)
        {
            var noDbLabel = new Label("No Database found. Click Initialize to create one.");
            noDbLabel.style.color = Color.yellow;
            editorView.Add(noDbLabel);
            return;
        }

        if (tablesProperty.arraySize == 0)
        {
            var noTablesLabel = new Label("No tables found. Create some tables to get started.");
            noTablesLabel.style.color = Color.gray;
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
            invalidTableLabel.style.color = Color.red;
            editorView.Add(invalidTableLabel);
            return;
        }

        // Create a container for the table details
        var detailsContainer = new VisualElement();
        detailsContainer.style.borderTopWidth = 1;
        detailsContainer.style.borderTopColor = new Color(0.18f, 0.22f, 0.32f, 1f);
        detailsContainer.style.paddingTop = 8;

        // Title
        var titleLabel = new Label($"{tableObj.GetTableName()} Details");
        titleLabel.style.fontSize = 16;
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        titleLabel.style.color = new Color(0.18f, 0.22f, 0.32f, 1f);
        titleLabel.style.marginBottom = 8;
        detailsContainer.Add(titleLabel);

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