# Domain Layer

This directory contains the core domain models and business rules for SmartTaskerMini.

## Contents

- **TaskItem.cs**: Core entity representing a task
- **TaskStatus.cs**: Enumeration of possible task statuses
- **HistoryItem.cs**: Entity representing a completed task in history

## Design Principles

The Domain layer contains:
- Business entities
- Value objects
- Domain events
- Domain exceptions
- Business rules

This layer has no dependencies on other layers or external libraries.