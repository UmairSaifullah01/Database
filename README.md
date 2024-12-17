# Database Package Documentation

## Overview

The Database Package is a flexible and extensible framework for managing data within Unity. It provides a set of classes and interfaces that allow you to create and manage databases, tables, and data.

## File Descriptions

### CHANGELOG.md

This file documents all notable changes to the project. It serves as a historical record of modifications, updates, and improvements made over time.

### DatabaseEditorUtility.cs

This utility class provides editor functionalities for managing the database within the Unity Editor. It includes methods to:

- Initialize the database.
- Auto-register tables derived from `TableBase`.
- Create new table drive classes.

### IDatabase.cs

This interface defines the contract for database operations. It includes methods for:

- Initializing the database.
- Retrieving tables by type or name.
- Adding new tables to the database.

### TableBase.cs

This abstract class serves as the base for all table types in the database. It provides:

- An abstract method `GetTableName()` for identifying the table.
- A virtual method `Initialize()` for custom setup logic.

### TableAssetCreatorWindow.cs

This class implements a custom editor window for creating table assets. It allows users to:

- Select the type of table to create.
- Specify the asset name and target folder.
- Create a new table asset based on the selected type.

### IServiceLocator.cs

This interface defines a service locator pattern for managing services within the application. It includes methods for:

- Registering and unregistering services.
- Retrieving registered services.

### DatabaseServiceLocator.cs

This static class implements a global service locator specifically for database tables. It initializes the database and registers it as a service, allowing easy access throughout the application.

## Usage

To use the Database Package, follow these steps:

1. Initialize the database using the `DatabaseEditorUtility`.
2. Create table classes derived from `TableBase`.
3. Use the `TableAssetCreatorWindow` to create table assets.
4. Access the database service using `DatabaseServiceLocator`.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any suggestions or improvements.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.
