# Application Layer

This directory contains application services and interfaces that orchestrate domain objects to perform use cases.

## Contents

- **ITaskRepository.cs**: Repository interface for task persistence
- **TaskService.cs**: Main service for task management operations
- **TaskSeeder.cs**: Utility for seeding test data
- **ReportService.cs**: Service for generating productivity reports
- **Configuration.cs**: Application configuration settings

## Design Principles

The Application layer:
- Implements use cases using domain objects
- Defines interfaces that will be implemented by the infrastructure layer
- Contains no UI or database access logic
- Depends on the Domain layer but not on Infrastructure