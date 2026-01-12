# Changelog

All notable changes to this project will be documented in this file.

## [2.0.0] - 2024-12-XX

### 🚀 Added - Service Locator Enhancements

- **Global Service Locator**: Simplified to global-only service locator pattern
- **Unity Object Extensions**: New extension methods for `UnityEngine.Object`:
  - `GetService<T>(bool global = true)` - Easy service access from any Unity object
  - `RegisterService<T>(T service, bool global = true)` - Simple service registration
  - `UnregisterService<T>(bool global = true)` - Service unregistration
- **Simplified API**: Removed category system for cleaner, more straightforward usage

### 🛠️ Improved - Database Editor

- **Optimized Editor Caching**: Dictionary-based editor caching system for better performance
- **TypeCache Integration**: Replaced expensive `AppDomain` scanning with Unity's optimized `TypeCache`
- **Auto-Refresh Tables**: Automatic table refresh when Database Editor window opens
- **GameDatabase Naming**: Consistent "GameDatabase" naming throughout the system
- **Better Editor Management**: Improved editor lifecycle management without unnecessary destruction

### 🔧 Improved - Database Utilities

- **FindOrCreateDatabase**: Enhanced database discovery with multiple fallback strategies
- **RefreshDatabaseTables**: New utility method to automatically refresh and register all TableBase assets
- **Better Path Management**: Improved database path resolution and creation

### 📚 Improved - Code Quality

- **Performance Optimizations**: Reduced overhead in editor operations
- **Cleaner Code**: Removed unnecessary destruction logic
- **Better Resource Management**: Improved memory management for editor windows

### 🔄 Migration Notes

- **Service Locator**: Extension methods provide easier access - `this.GetService<T>()` instead of `ServiceLocator.Global.GetService<T>()`
- **Database Editor**: No breaking changes, but improved performance and auto-refresh functionality
- **Backward Compatible**: All existing code continues to work without changes

## [1.2.0] - 2024-01-XX

### 🚀 Added - Enhanced Table System

- **Indexing System**: Fast lookups by field values with `CreateIndex()` and `GetByIndex()`
- **LINQ-style Querying**: `Where()`, `FirstOrDefault()`, `Count()`, `Any()`, `All()`, `Select()`
- **Advanced Sorting**: `OrderBy()`, `OrderByDescending()` with cached results
- **Bulk Operations**: `AddRange()`, `RemoveAll()`, `UpdateAll()` for efficient batch processing
- **Utility Methods**: `GetRandom()`, `Contains()`, `ToArray()`, `ToList()`, `Clear()`
- **Performance Properties**: `TotalCount`, `IsEmpty`, `FirstRecord`, `LastRecord`

### 🎯 Added - Extension Methods

- **Range Queries**: `Between()`, `Top()`, `Bottom()` for value-based filtering
- **Aggregations**: `Sum()`, `Average()`, `Max()`, `Min()` for numeric calculations
- **Text Search**: `ContainsText()`, `StartsWith()`, `EndsWith()` with case-insensitive options
- **Pagination**: `GetPage()`, `GetPageCount()` for large dataset management
- **Grouping**: `GroupBy()`, `Distinct()`, `Shuffle()` for data analysis
- **Dictionary Conversion**: `ToDictionary()`, `GetByKey()` for key-based access

### 🛠️ Added - Modern UI Toolkit Editor

- **UI Toolkit Integration**: Complete rewrite of database editor using modern UI Toolkit
- **Responsive Design**: Clean, professional interface with proper styling
- **Visual Table Management**: Tab-based interface for managing multiple tables
- **Real-time Updates**: Live preview and instant feedback for all operations
- **Keyboard Shortcuts**: Alt+Cmd+D (Mac) / Alt+Ctrl+D (Windows) for quick access
- **Table Creation Tools**: Integrated controls for creating and managing tables

### 🎮 Added - Improved API

- **Global Access**: `Database.Get<T>()` for shortest possible table access
- **Service Locator Shortcuts**: `DatabaseServiceLocator.GetTable<T>()` for convenient access
- **Multiple Access Patterns**: Support for type-based, name-based, and generic access
- **Type Safety**: Compile-time safety for all database operations

### 📚 Added - Documentation & Examples

- **Comprehensive Examples**: `TableExamples.cs` with practical usage demonstrations
- **Performance Tips**: Guidelines for optimal database usage
- **Migration Guide**: Clear instructions for upgrading from v1.1
- **API Documentation**: Complete documentation of all new features

### 🔧 Improved - Core Functionality

- **Better Error Handling**: Improved validation and error messages
- **Memory Management**: Proper cleanup of cached data and indexes
- **Performance Optimization**: Cached sorting and efficient indexing
- **Code Organization**: Better structured code with regions and documentation

### 🐛 Fixed - Issues

- **Naming Conflicts**: Resolved conflicts between properties and methods
- **Type Conversions**: Fixed delegate comparison issues in sorting cache
- **Compilation Errors**: Resolved all build issues and warnings
- **UI Responsiveness**: Fixed UI Toolkit integration issues

### 📦 Changed - Breaking Changes

- **Property Renames**:
  - `table.Count` → `table.TotalCount` (property)
  - `table.First` → `table.FirstRecord` (property)
  - `table.Last` → `table.LastRecord` (property)
- **Method Signatures**: All new methods are additive and backward compatible

### 🔄 Migration Notes

- **Backward Compatibility**: All existing code continues to work without changes
- **Optional Upgrades**: New features can be adopted gradually
- **Performance Benefits**: Existing code automatically benefits from new optimizations

## [1.1.0] - Previous Version

### Added

- Basic database functionality
- Table management system
- Service locator pattern
- Editor utilities for database management
- Asset creation windows
- Basic CRUD operations

### Features

- Database initialization and management
- Table creation and registration
- Basic data storage and retrieval
- Unity Editor integration
- ScriptableObject-based storage

---

## Version History

- **v2.0.0**: Service Locator Extensions, Optimized Editor, Auto-Refresh Tables
- **v1.2.0**: Enhanced Table System, UI Toolkit Editor, Improved API
- **v1.1.0**: Basic Database Functionality, Service Locator, Editor Tools
- **v1.0.0**: Initial Release
