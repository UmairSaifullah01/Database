# Database Package Documentation v1.2

## Overview

The Database Package is a flexible and extensible framework for managing data within Unity. It provides a set of classes and interfaces that allow you to create and manage databases, tables, and data with enterprise-level features like indexing, querying, and sorting.

## ✨ New Features in v1.2

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

The main database class that manages all tables. Features:

- Global access via `Database.Get<T>()`
- Table management and initialization
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
- Extensible design pattern

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

Extension methods for advanced operations:

- **Range Queries**: `Between()`, `Top()`, `Bottom()`
- **Aggregations**: `Sum()`, `Average()`, `Max()`, `Min()`
- **Text Search**: `ContainsText()`, `StartsWith()`, `EndsWith()`
- **Pagination**: `GetPage()`, `GetPageCount()`
- **Grouping**: `GroupBy()`, `Distinct()`, `Shuffle()`

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

### Creating Tables

1. Click "Table" button to show creation controls
2. Enter table name and select type
3. Click "Create and Add Table"
4. Use "Create Table Class" for custom table types

### Managing Tables

- **Tabs**: Click table tabs to switch between tables
- **Close**: Click "x" to remove tables
- **Refresh**: Click "Refresh" to auto-register new tables

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
