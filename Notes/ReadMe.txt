The architecture for enterprise application should have at least the following levels:
1. Entity Layer: Contains entities (POCOs)
2. Data Layer: Contains all related code to database access
3. Business Layer: Contains definitions and validations related to business
4. External Services Layer (optional): Contains invocations for external services (ASMX, WCF, RESTful)
5. Common: Contains common classes and interfaces for all layers (e.g. Loggers, Mappers, Extensions)
6. Tests (QA): Contains automated tests for back-end code
7. Presentation Layer: This is the UI
8. UI Tests (QA): Contains automated tests for front-end code