using System.Xml.Linq;
using SmartTaskerMini.Core.Domain;
using SmartTaskerMini.Core.Application;

namespace SmartTaskerMini.Core.Infrastructure;

public class XmlTaskRepository : ITaskRepository
{
    private readonly string _filePath;
    private readonly string _historyFilePath;
    private List<TaskItem> _tasks;
    private List<HistoryItem> _history;

    public XmlTaskRepository(string filePath = "data/tasks.xml")
    {
        _filePath = filePath;
        _historyFilePath = Path.ChangeExtension(filePath, ".history.xml");
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
            var doc = XDocument.Load(_filePath);
            var tasks = doc.Root?.Elements("Task").Select(x => new TaskItem
            {
                Id = (int)x.Element("Id")!,
                Title = (string?)x.Element("Title") ?? string.Empty,
                DueUtc = (DateTime)x.Element("DueUtc")!,
                Priority = (int)x.Element("Priority")!,
                Status = Enum.Parse<SmartTaskerMini.Core.Domain.TaskStatus>((string)x.Element("Status")!),
                Score = (int)x.Element("Score")!
            }).ToList() ?? new List<TaskItem>();
            
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
            var doc = XDocument.Load(_historyFilePath);
            var history = doc.Root?.Elements("HistoryItem").Select(x => new HistoryItem
            {
                Id = (int)x.Element("Id")!,
                Title = (string?)x.Element("Title") ?? string.Empty,
                CreatedDate = (DateTime)x.Element("CreatedDate")!,
                CompletedDate = (DateTime)x.Element("CompletedDate")!
            }).ToList() ?? new List<HistoryItem>();
            
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
            var doc = new XDocument(
                new XElement("Tasks",
                    _tasks.Select(t => new XElement("Task",
                        new XElement("Id", t.Id),
                        new XElement("Title", t.Title ?? string.Empty),
                        new XElement("DueUtc", t.DueUtc),
                        new XElement("Priority", t.Priority),
                        new XElement("Status", t.Status.ToString()),
                        new XElement("Score", t.Score)
                    ))
                )
            );
            await Task.Run(() => doc.Save(_filePath));
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
            var doc = new XDocument(
                new XElement("History",
                    _history.Select(h => new XElement("HistoryItem",
                        new XElement("Id", h.Id),
                        new XElement("Title", h.Title ?? string.Empty),
                        new XElement("CreatedDate", h.CreatedDate),
                        new XElement("CompletedDate", h.CompletedDate)
                    ))
                )
            );
            await Task.Run(() => doc.Save(_historyFilePath));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving history: {ex.Message}");
        }
    }
}