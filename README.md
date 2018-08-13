# CompleetShop

## Architecture
The architecture includes the following levels:
1. Entity Layer: Contains entities (POCOs)
2. Data Layer: Contains all related code to database access
3. Business Layer: Contains definitions and validations related to business
4. Presentation Layer: Contains User Interfaces

| Layer | Content | Has Interface | Prefix | Remarks |
 ----------- | ----------- | ----------- | ----------- | ----------- |
| Entity Layer | DB Models | No | (none) | Properties are mapped to DB fields |
| Data Layer | DB Context and | No | (none) |  |
|  |            DB Configurations | No | Configuration | Configuration: Object property to DB field mapping |
|  | DB Repositories | Yes | Repository |  |
| Business Layer | DB Service | Yes | Service | DB Models are converted to UI Models and vice-versa |
| Presentation Layer | UI Models | No | Model |  |
|  | View Models | No | ViewModel |  |
|  | View | No | (none) |  |

## Adding new tables
| No. | Content | Target Project | Remarks |
 ----------- | ----------- | ----------- | ----------- |
| 1. | Add new class for the table | [CompleetShop.Database.Entities][1] | Class name is mapped as table name and properties as field names. |
| 2. | Configure table mappings | [CompleetShop.Database.Context][2] | Can specifify primary key, rename fields, etc. |
| 3. | Create a repository: class and interface | [CompleetShop.Database.Repositories][3] |  |
| 4. | Create business logic | [CompleetShop.Database.Services][4] | This is called by UI. Entities(DB) are mapped with Models(UI). |


[1]: CompleetShop.Database.Entities/
[2]: CompleetShop.Database.Context/
[3]: CompleetShop.Database.Repositories/
[4]: CompleetShop.Database.Services/
