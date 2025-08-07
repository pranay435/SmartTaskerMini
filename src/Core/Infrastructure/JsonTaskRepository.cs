using System.Text.Json;
using SmartTaskerMini.Core.Domain;
using SmartTaskerMini.Core.Application;

namespace SmartTaskerMini.Core.Infrastructure;

public class JsonTaskRepository : ITaskRepository
{
    private readonly string _filePath;
    private readonly string _historyFilePath;
    private List<TaskItem> _tasks;
    private List<HistoryItem> _history;

    public JsonTaskRepository(string filePath = "data/tasks.json")
    {
        _filePath = filePath;
        _historyFilePath = Path.ChangeExtension(filePath, ".history.json");
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        _tasks = LoadTasks();
        _history = LoadHistory();
    }

    public async Task AddTaskAsync(TaskItem task)
    {
        if (task.Id == 0)
            task.Id = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
        task.Score = task.Priority * 100 - (int)(task.DueUtc - DateTime.Now).TotalMinutes;
        _tasks.Add(task);
        await SaveTasksAsync();
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var removed = _tasks.RemoveAll(t => t.Id == id) > 0;
        if (removed) await SaveTasksAsync();
        return removed;
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        return await Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        return await Task.FromResult(_tasks.OrderByDescending(t => t.Score).ToList());
    }

    public async Task UpdateTaskAsync(TaskItem task)
    {
        var index = _tasks.FindIndex(t => t.Id == task.Id);
        if (index >= 0)
        {
            task.Score = task.Priority * 100 - (int)(task.DueUtc - DateTime.Now).TotalMinutes;
            _tasks[index] = task;
            await SaveTasksAsync();
        }
    }

    public async Task MoveToHistoryAsync(TaskItem task)
    {
        var historyItem = new HistoryItem
        {
            Id = task.Id,
            Title = task.Title ?? string.Empty,
            CreatedDate = task.DueUtc,
            CompletedDate = DateTime.Now
        };
        _history.Add(historyItem);
        _tasks.RemoveAll(t => t.Id == task.Id);
        await SaveTasksAsync();
        await SaveHistoryAsync();
        _history = LoadHistory(); // Reload to ensure consistency
    }

    public async Task<List<HistoryItem>> GetCompletedTasksForDateAsync(DateTime date)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);
        return await Task.FromResult(_history.Where(h => h.CompletedDate >= startDate && h.CompletedDate < endDate).ToList());
    }

    public async Task<List<HistoryItem>> GetAllCompletedTasksAsync()
    {
        return await Task.FromResult(_history.OrderByDescending(h => h.CompletedDate).ToList());
    }

    private List<TaskItem> LoadTasks()
    {
        if (!File.Exists(_filePath))
            return new List<TaskItem>();

        try
        {
            var json = File.ReadAllText(_filePath);
            var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            
            // Ensure all tasks have valid titles
            foreach (var task in tasks)
            {
                task.Title ??= string.Empty;
            }
            
            return tasks;
        }
        catch
        {
            return new List<TaskItem>();
        }
    }

    private List<HistoryItem> LoadHistory()
    {
        if (!File.Exists(_historyFilePath))
            return new List<HistoryItem>();

        try
        {
            var json = File.ReadAllText(_historyFilePath);
            var history = JsonSerializer.Deserialize<List<HistoryItem>>(json) ?? new List<HistoryItem>();
            
            // Ensure all history items have valid titles
            foreach (var item in history)
            {
                item.Title ??= string.Empty;
            }
            
            return history;
        }
        catch
        {
            return new List<HistoryItem>();
        }
    }

    private async Task SaveTasksAsync()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_tasks, options);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving tasks: {ex.Message}");
        }
    }

    private async Task SaveHistoryAsync()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_history, options);
            await File.WriteAllTextAsync(_historyFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving history: {ex.Message}");
        }
    }
}