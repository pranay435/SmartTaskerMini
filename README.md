# SmartTaskerMini

A lightweight task management application with prioritization and reporting features.

## Project Structure

The project follows Clean Architecture principles:

```
SmartTaskerMini/
├── src/
│   ├── Core/
│   │   ├── Domain/       # Domain entities and business rules
│   │   ├── Application/  # Application services and interfaces
│   │   └── Infrastructure/ # Implementation of interfaces
│   └── App/              # Console UI application
└── tests/
    └── SmartTaskerMini.Tests/ # Test projects
```

## Features

- Task management (add, list, edit, delete)
- Task prioritization with automatic scoring
- Task completion tracking
- Daily productivity reports
- Data persistence using SQL Server

## Getting Started

1. Ensure you have SQL Server installed and running
2. Update the connection string in `Configuration.cs` if needed
3. Build the solution: `dotnet build`
4. Run the application: `dotnet run --project src/App/App.csproj`

## Commands

- `add` - Add a new task
- `list` - List all active tasks
- `done` - Mark a task as completed
- `delete_task` - Delete a task
- `edittask` - Edit an existing task
- `daily_report` - Generate a productivity report
- `seed` - Seed test data
- `exit` - Exit the application