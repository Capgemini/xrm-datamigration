# Capgemini CRM Data Migration

## Description

This Data Migration project provides a flexible powerful engine based on the XRM SDK which allows CRM Dynamics Configuration, Reference and Seed data to be extracted, stored in version control and loaded to target instances.  This allows data to be managed in the same way as code and a release can be created that can load the required data to support the released functionality.

## Table Of Contents
1. [Description](#Description)  
1. [Installation](#Installation)
1. [Usage](#Usage)
1. [Contributing](#Contributing)
1. [Credits](#Credits)
1. [License](#License)

## Installation

Create a new console app and add Capgemini.Xrm.DataMigration Nuget
![nugetScreen.png](./.attachments/nugetScreen.png)

## Usage

The engine supports two file formats JSON and CSV and has been used for a number of scenarios on a number of projects.  It is extremely flexible and supports the migration of simple reference data entities (e.g. Titles, Countries) to more complex scenarios around Security Roles and Teams.  See wiki for a fuller list of examples (link).

Other features of the engine are the support for many-to-many relationships, application of filters, building relations via composite keys and GUID mappings. 

The engine is controlled by three configuration files, a fuller explanation of the values can be found in the wiki.
**DataSchema.xml** - Defines details of the entities and attributes that are to be extracted.

**DataExport.json** – Holds details of the schema to use, filters to be applied and other run controls.  See wiki for a more details explanation.

**DataImport.json** - Holds details of the location and prefix of the Exported files that are to be loaded.

## Contributing

To contribute to this project, report any bugs, submit new feature requests, submit changes via pull requests or even join in the overall design of the tool.

## Credits

Special thanks to the entire Capgemini community for their support in developing this tool.

## License

The Xrm Solution Audit is released under the [MIT](LICENSE) license.
