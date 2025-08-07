namespace SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;

/// <summary>
/// Repository interface for task persistence operations
/// </summary>
public interface ITaskRepository
{
    /// <summary>Adds a new task to the repository</summary>
    Task AddTaskAsync(TaskItem task);
    
    /// <summary>Gets all active tasks</summary>
    Task<List<TaskItem>> GetAllTasksAsync();
    
    /// <summary>Gets a task by its ID</summary>
    Task<TaskItem?> GetTaskByIdAsync(int id);
    
    /// <summary>Updates an existing task</summary>
    Task UpdateTaskAsync(TaskItem task);
    
    /// <summary>Deletes a task by its ID</summary>
    Task<bool> DeleteTaskAsync(int id);
    
    /// <summary>Moves a task to history (marks as completed)</summary>
    Task MoveToHistoryAsync(TaskItem task);
    
    /// <summary>Gets tasks completed on a specific date</summary>
    Task<List<HistoryItem>> GetCompletedTasksForDateAsync(DateTime date);
    
    /// <summary>Gets all completed tasks</summary>
    Task<List<HistoryItem>> GetAllCompletedTasksAsync();
}
