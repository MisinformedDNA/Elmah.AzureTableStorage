Elmah.AzureTableStorage
=======================

Elmah plugin for Azure Table Storage

Setup 
-----

If you use NuGet to add this project, option 1 will be used, by default.

### Option 1

By connection string name:

    <errorLog type="Elmah.AzureTableStorage.AzureTableStorageErrorLog, Elmah.AzureTableStorage"
        connectionStringName="ElmahAzureTableStorage" />
        
Make sure you add an element under the `connectionStrings` element.

### Option 2

By application setting:

    <errorLog type="Elmah.AzureTableStorage.AzureTableStorageErrorLog, Elmah.AzureTableStorage"
        connectionStringAppKey="ElmahAzureTableStorage" />
        
Make sure you add an element under the `appSettings` element.

### Option 3

By connection string:

    <errorLog type="Elmah.AzureTableStorage.AzureTableStorageErrorLog, Elmah.AzureTableStorage" 
        connectionString="UseDevelopmentStorage=true" />


Configuration Parameters
-----

### Application Name

Isolate per application in a single Elmah store

    <errorLog type="Elmah.AzureTableStorage.AzureTableStorageErrorLog, Elmah.AzureTableStorage" 
        connectionString="UseDevelopmentStorage=true" applicationName="MyApp" />

### Table Name

Set a custom table name (defaults to "Elmah"):

    <errorLog type="Elmah.AzureTableStorage.AzureTableStorageErrorLog, Elmah.AzureTableStorage" 
        connectionString="UseDevelopmentStorage=true" tableName="MyCustomElmahTable" />