using SmartTaskerMini.Core.Domain;


namespace SmartTaskerMini.Core.Application;

/// <summary>
/// Service for managing tasks, providing operations like add, list, update, and delete
/// </summary>
public class TaskService
{
    private readonly ITaskRepository _repo;
    public TaskService(ITaskRepository repo) => _repo = repo;

    /// <summary>
    /// Adds a new task with the specified details
    /// </summary>
    /// <param name="title">The task title</param>
    /// <param name="dueUtc">The due date and time</param>
    /// <param name="priority">The priority level (1-5)</param>
    public virtual async Task<string> AddAsync(string title, DateTime dueUtc, int priority)
    {
        try
        {
            var task = new TaskItem 
            { 
                Title = title, 
                DueUtc = dueUtc, 
                Priority = priority,
                Status = SmartTaskerMini.Core.Domain.TaskStatus.Pending
            };
            await _repo.AddTaskAsync(task);
            return "Task added successfully";
        }
        catch (ArgumentException)
        {
            throw; // Re-throw validation exceptions for tests
        }
        catch (Exception ex)
        {
            return $"Error adding task: {ex.Message}";
        }
    }

    public async Task<List<TaskItem>> ListAsync()
    {
        try 
        {
            var list = await _repo.GetAllTasksAsync();
            return list;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving tasks: {ex.Message}");
            return new List<TaskItem>(); // Return empty list instead of throwing
        }
    }


    public async Task<string> MarkDoneAsync(int id)
    {
        try
        {
            var item = await _repo.GetTaskByIdAsync(id);

            if (item is null)
            {
                return "Invalid guid";
            }
            
            item.MarkDone();
            await _repo.MoveToHistoryAsync(item);
            return "Task completed and moved to history";
        }
        catch (Exception ex)
        {
            return $"Error marking task as done: {ex.Message}";
        }
    }
 
    public async Task<string> DeleteTaskAsync(int id)
    {
        try
        {
            bool status = await _repo.DeleteTaskAsync(id);
            if (status)
            {
                return "Successfully Deleted!";
            }
            return "Invalid Id.";
        }
        catch (Exception ex)
        {
            return $"Error deleting task: {ex.Message}";
        }
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        try
        {
            var item = await _repo.GetTaskByIdAsync(id);
            return item;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving task: {ex.Message}");
            return null;
        }
    }
    
    public async Task<string> UpdateTaskAsync(TaskItem task)
    {
        try
        {
            if (task.DueUtc < DateTime.Now)
                throw new ArgumentException("Due date cannot be in the past");
                
            await _repo.UpdateTaskAsync(task);
            return "Task updated successfully";
        }
        catch (ArgumentException)
        {
            throw; // Re-throw validation exceptions for tests
        }
        catch (Exception ex)
        {
            return $"Error updating task: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Gets all completed tasks from history
    /// </summary>
    /// <returns>A list of completed tasks</returns>
    public async Task<List<HistoryItem>> GetCompletedTasksAsync()
    {
        try
        {
            return await _repo.GetAllCompletedTasksAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving completed tasks: {ex.Message}");
            return new List<HistoryItem>(); // Return empty list instead of throwing
        }
    }
}