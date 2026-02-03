# Database

## Overview

The Database Package is a flexible and extensible framework for managing data within Unity. It provides a set of classes and interfaces that allow you to create and manage databases, tables, and data with enterprise-level features like indexing, querying, and sorting.

## ✨ New Features in v2.1

### 🎯 Component System Architecture

- **IDatabaseComponent Interface**: Unified interface for all database components
  - `TableBase` - Tables with multiple records
  - `DatabaseComponent` - Wrapper for any ScriptableObject (e.g., SoundSystem)
  - `SingleEntityComponent<T>` - Single entity/record components (like Table<T> but with one record)
- **Unified Component Management**: All components stored and managed in Database
- **Component-Based Editor**: Unified editor view showing all components (tables, components, single entities)

### 🚀 Service Locator Extensions

- **Unity Object Extensions**: Easy service access from any Unity object
  - `this.GetService<T>()` - Get any registered service
  - `this.RegisterService<T>(service)` - Register a service
  - `this.UnregisterService<T>()` - Unregister a service
- **Global Service Locator**: Simplified to global-only pattern for easier usage
- **Clean API**: Removed category system for straightforward service management

### 🛠️ Enhanced Database Editor

- **Unified Component View**: Left panel shows all components (tables, DatabaseComponents, SingleEntityComponents)
- **Component Details Panel**: Right panel shows inspector for selected component
- **Auto-Register All Components**: Menu option to auto-register all component types
- **Optimized Performance**: Dictionary-based editor caching and TypeCache integration
- **Auto-Refresh**: Automatic component discovery and registration on window open
- **GameDatabase**: Consistent "GameDatabase" naming and management

### 🧰 New Utility Methods

- **Database Extensions**: Extension methods for easy component access
  - `database.GetComponentByName()` - Find component by name
  - `database.GetComponent<T>()` - Get component by type
  - `database.GetComponents<T>()` - Get all components of type
  - `database.GetSingleEntityComponent<T>()` - Get single entity component by type
  - `database.HasComponents()` - Check if database has components
  - `database.GetComponentCount()` - Get total component count
- **Database Management**: Remove and clear methods
  - `database.RemoveTable()` - Remove a table
  - `database.RemoveComponent()` - Remove a DatabaseComponent
  - `database.RemoveSingleEntityComponent()` - Remove a SingleEntityComponent
  - `database.ClearTables()` - Clear all tables
  - `database.ClearComponents()` - Clear all components
  - `database.ClearSingleEntityComponents()` - Clear all single entity components

## ✨ Features from v1.2

### 🚀 Enhanced Table System

- **Indexing System** - Fast lookups by field values
- **LINQ-style Querying** - Filter, sort, and search data efficiently
- **Bulk Operations** - Add, remove, and update multiple records at once
- **Cached Sorting** - Optimized performance for repeated sorting operations
- **Utility Methods** - Random selection, pagination, and data conversion

### 🎮 Improved API

- **Short Global Access** - `Database.Get<T>()` for instant table access
- **Service Locator Shortcuts** - Multiple convenient access patterns
- **Type-Safe Operations** - Compile-time safety for all operations

### 🛠️ Modern UI Toolkit Editor

- **UI Toolkit Integration** - Modern, responsive database editor
- **Visual Table Management** - Drag-and-drop table creation
- **Real-time Updates** - Live preview and instant feedback
- **Keyboard Shortcuts** - Alt+Cmd+D (Mac) / Alt+Ctrl+D (Windows)

## File Descriptions

### 🎯 Core Runtime Files

#### Database.cs

The main database class that manages all components. Features:

- Global access via `Database.Get<T>()`
- Component management (tables, DatabaseComponents, SingleEntityComponents)
- Component initialization and lifecycle management
- Add/Remove/Clear methods for all component types
- Unified component access via `GetAllComponents()`
- Service locator integration

#### Table.cs

Enhanced table implementation with advanced features:

- **Querying**: `Where()`, `FirstOrDefault()`, `Count()`, `Any()`, `All()`
- **Sorting**: `OrderBy()`, `OrderByDescending()`, `Top()`, `Bottom()`
- **Indexing**: `CreateIndex()`, `GetByIndex()`, `RebuildIndexes()`
- **Bulk Operations**: `AddRange()`, `RemoveAll()`, `UpdateAll()`
- **Utilities**: `GetRandom()`, `Contains()`, `ToArray()`, `ToList()`

#### TableBase.cs

Abstract base class for all table types:

- Common table functionality
- Initialization and naming
- Implements `IDatabaseComponent` interface
- Extensible design pattern

#### DatabaseComponent.cs

Component that wraps any ScriptableObject for inspection:

- Holds reference to any ScriptableObject (e.g., SoundSystem)
- Implements `IDatabaseComponent` interface
- Can be created via CreateAssetMenu
- Useful for inspecting non-table ScriptableObjects in Database editor

#### SingleEntityComponentBase.cs & SingleEntityComponent&lt;T&gt;

Components for single entity/record management:

- `SingleEntityComponentBase` - Abstract base class
- `SingleEntityComponent<T>` - Generic implementation (like Table&lt;T&gt; but with one record)
- Holds a single `T` record in serialized `data` field
- Implements `IDatabaseComponent` interface
- Create concrete classes like: `MyEntityComponent : SingleEntityComponent<MyEntityData>`

### 🔌 Adapters

#### TableAdapter.cs

Base adapter class for data serialization:

- Abstract serialization interface
- Extensible for different formats

#### JsonTableAdapter.cs

JSON-based data serialization:

- Human-readable data format
- Easy debugging and editing
- Cross-platform compatibility

### 🧰 Extensions

#### TableExtensions.cs

Extension methods for advanced table operations:

- **Range Queries**: `Between()`, `Top()`, `Bottom()`
- **Aggregations**: `Sum()`, `Average()`, `Max()`, `Min()`
- **Text Search**: `ContainsText()`, `StartsWith()`, `EndsWith()`
- **Pagination**: `GetPage()`, `GetPageCount()`
- **Grouping**: `GroupBy()`, `Distinct()`, `Shuffle()`

#### DatabaseExtensions.cs

Extension methods for Database component management:

- **Component Access**: `GetComponentByName()`, `GetComponent<T>()`, `GetComponents<T>()`
- **Single Entity Access**: `GetSingleEntityComponent<T>()`
- **Component Queries**: `GetComponentByTarget()`, `HasComponents()`, `GetComponentCount()`

#### TableExamples.cs

Comprehensive examples showing:

- Basic CRUD operations
- Advanced querying and filtering
- Performance optimization techniques
- Complex data operations

### 🛠️ Utilities

#### DatabaseServiceLocator.cs

Global service locator with convenient access methods:

- `GetTable<T>()` - Direct table access
- `GetTable(string)` - Access by name
- `InitializeDatabase()` - Manual initialization

#### ObjectServiceExtensions.cs

Extension methods for `UnityEngine.Object` to easily access services:

- `GetService<T>()` - Get any registered service
- `RegisterService<T>(service)` - Register a service
- `UnregisterService<T>()` - Unregister a service

### 🎨 Editor Files

#### DatabaseEditor.cs

Modern UI Toolkit-based database editor featuring:

- Clean, responsive interface
- Real-time table management
- Visual tab system
- Integrated table creation tools

#### DatabaseEditorUtility.cs

Editor utilities for database management:

- Database initialization
- Auto-registration of tables
- **Auto-registration of all components** (tables, DatabaseComponents, SingleEntityComponents)
- Table class generation

#### TableAssetCreatorWindow.cs

Asset creation window for tables:

- Type selection
- Asset naming and placement
- Validation and error handling

### 🎨 Shared UI

#### UI.uxml

UI Toolkit layout file:

- Modular design accessible from all folders
- Clean, professional interface
- Responsive layout system

#### DatabaseEditor.uss

UI Toolkit styles:

- Consistent visual design
- Dark theme optimized
- Professional appearance

## 🚀 Quick Start

### 1. Create a Data Class

```csharp
[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
    public int level;
    public bool isActive;
}
```

### 2. Create a Table Class

```csharp
[CreateAssetMenu(menuName = "Database/Tables/PlayerTable", fileName = "PlayerTable")]
public class PlayerTable : Table<PlayerData>
{
    // Custom table logic can be added here
}
```

### 3. Access Your Data

```csharp
// Shortest way to get a table
var playerTable = Database.Get<PlayerTable>();

// Or using extension methods (v2.0)
var playerTable = this.GetService<PlayerTable>();

// Add data
playerTable.AddRecord(new PlayerData { playerName = "Alice", score = 100 });

// Query data
var activePlayers = playerTable.Where(p => p.isActive);
var topPlayers = playerTable.Top(p => p.score, 5);
var alice = playerTable.FirstOrDefault(p => p.playerName == "Alice");

// Bulk operations
playerTable.AddRange(newPlayers);
playerTable.RemoveAll(p => !p.isActive);
```

### 4. Working with Components (v2.1)

```csharp
var database = Database.Instance;

// Create and add a DatabaseComponent
var soundComponent = ScriptableObject.CreateInstance<DatabaseComponent>();
soundComponent.TargetScriptable = mySoundSystem;
database.AddComponent(soundComponent);

// Create and add a SingleEntityComponent
[CreateAssetMenu(menuName = "Database/PlayerEntity", fileName = "PlayerEntity")]
public class PlayerEntityComponent : SingleEntityComponent<PlayerData> { }

var playerEntity = ScriptableObject.CreateInstance<PlayerEntityComponent>();
playerEntity.Data = new PlayerData { playerName = "Bob", score = 200 };
database.AddSingleEntityComponent(playerEntity);

// Access components using extensions
var component = database.GetComponentByName("My Component");
var soundComp = database.GetComponent<DatabaseComponent>();
var playerComp = database.GetSingleEntityComponent<PlayerData>();

// Get all components
var allComponents = database.GetAllComponents();
int totalCount = database.GetComponentCount();
```

### 5. Using Service Locator (v2.0)

```csharp
// Register a service
this.RegisterService<MyService>(myService);

// Get a service
var myService = this.GetService<MyService>();

// Unregister a service
this.UnregisterService<MyService>();
```

## 📊 Advanced Usage

### Indexing for Performance

```csharp
// Create indexes for fast lookups
playerTable.CreateIndex(p => p.playerName, "NameIndex");
playerTable.CreateIndex(p => p.email, "EmailIndex");

// Fast indexed lookups
var alicePlayers = playerTable.GetByIndex(p => p.playerName, "Alice");
```

### Complex Queries

```csharp
// Multi-condition filtering
var activeHighLevelPlayers = playerTable
    .Where(p => p.isActive && p.level >= 3 && p.score > 200);

// Aggregation with filtering
double averageScoreOfActivePlayers = playerTable
    .Where(p => p.isActive)
    .Average(p => p.score);

// Grouping and analysis
var averageScoreByLevel = playerTable
    .GroupBy(p => p.level)
    .Select(g => new { Level = g.Key, AverageScore = g.Average(p => p.score) });
```

### Text Search

```csharp
// Find players with names containing "a"
var playersWithA = playerTable.ContainsText(p => p.playerName, "a");

// Find players starting with "A"
var playersStartingWithA = playerTable.StartsWith(p => p.playerName, "A");
```

### Pagination

```csharp
// Get first 10 players
var page1Players = playerTable.GetPage(1, 10);

// Get total page count
int totalPages = playerTable.GetPageCount(10);
```

## 🛠️ Editor Usage

### Opening the Database Editor

1. **Menu**: Tools → THEBADDEST → Database → Database Editor
2. **Shortcut**: Alt+Cmd+D (Mac) / Alt+Ctrl+D (Windows)

### Component Management

The Database Editor now shows a unified view of all components:

- **Left Panel**: Lists all components (tables, DatabaseComponents, SingleEntityComponents)
- **Right Panel**: Shows inspector/details for the selected component
- **Component Types**:
  - **Tables**: Full table inspector with entries list
  - **DatabaseComponents**: Inspector of the referenced ScriptableObject
  - **SingleEntityComponents**: Inspector showing the single entity data

### Creating Components

**Tables**:
1. Create a table class inheriting from `Table<T>`
2. Create asset via CreateAssetMenu
3. Use "Auto-Register Tables" or manually add to Database

**DatabaseComponents**:
1. Create asset via CreateAssetMenu: "THEBADDEST/Database/Database Component"
2. Assign target ScriptableObject
3. Use "Auto-Register All Components" or manually add to Database

**SingleEntityComponents**:
1. Create a class: `MyEntityComponent : SingleEntityComponent<MyData>`
2. Create asset via CreateAssetMenu
3. Assign data in inspector
4. Use "Auto-Register All Components" or manually add to Database

### Managing Components

- **Auto-Register**: Use menu "Tools → THEBADDEST → Database → Auto-Register All Components"
- **Select Component**: Click component name in left panel to view details
- **Remove**: Use `database.RemoveTable()`, `RemoveComponent()`, or `RemoveSingleEntityComponent()`
- **Clear All**: Use `database.ClearTables()`, `ClearComponents()`, or `ClearSingleEntityComponents()`

## 🔧 Performance Tips

1. **Use Indexes** for frequently queried fields
2. **Clear Sorting Cache** when not needed: `table.ClearSorting()`
3. **Rebuild Indexes** after bulk operations: `table.RebuildIndexes()`
4. **Use Bulk Operations** instead of individual operations
5. **Leverage Cached Sorting** for repeated sort operations

## 📝 Migration from v1.1

### API Changes

- `table.Count` → `table.TotalCount` (property)
- `table.Count(predicate)` → `table.Count(predicate)` (method)
- `table.First` → `table.FirstRecord` (property)
- `table.First(predicate)` → `table.First(predicate)` (method)

### New Features

- All new features are additive and backward compatible
- Existing code continues to work without changes
- New methods provide enhanced functionality

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any suggestions or improvements.

## 📄 License

This project is licensed under the MIT License. See the LICENSE file for more details.

## 🔗 Links

- **Documentation**: [GitHub Repository](https://github.com/UmairSaifullah01/Database)
- **Issues**: [GitHub Issues](https://github.com/UmairSaifullah01/Database/issues)
- **Author**: [Umair Saifullah](https://www.umairsaifullah.com)
