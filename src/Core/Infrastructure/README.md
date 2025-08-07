# Infrastructure Layer

This directory contains implementations of interfaces defined in the Application layer.

## Contents

- **AdoNetRepo.cs**: SQL Server implementation of ITaskRepository using ADO.NET

## Design Principles

The Infrastructure layer:
- Implements interfaces defined in the Application layer
- Contains database access code, external API clients, etc.
- Depends on the Application and Domain layers
- Contains adapters for external services and frameworks