using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Infrastructure;

namespace SmartTaskerMini.App;

/// <summary>
/// Handles user interface logic for the console application
/// </summary>
public static class ConsoleUI
{
    /// <summary>
    /// Runs the console application's main loop
    /// </summary>
    public static async Task RunAsync()
    {
        var service = new TaskService(RepositoryFactory.CreateTaskRepository());
 
        while (true)
        {
            Console.WriteLine($"\n\nSmartTasker Mini ({Configuration.StorageType}) – commands: add_task | list_tasks | mark_done | delete_task | edit_task | daily_report | history | seed | use_json | use_xml | use_sql | exit\n");
            Console.Write("> ");
            
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(input)) continue;

            var cmd = input.Split(' ', 2)[0].ToLowerInvariant();
            
            try
            {
                switch (cmd)
                {
                    case "add_task":
                        await HandleAddCommand(service);
                        break;
                    case "list_tasks":
                        await HandleListCommand(service);
                        break;
                    case "mark_done":
                        await HandleDoneCommand(service);
                        break;
                    case "delete_task":
                        await HandleDeleteCommand(service);
                        break;
                    case "edit_task":
                        await HandleEditCommand(service);
                        break;
                    case "daily_report":
                        await HandleDailyReportCommand(service);
                        break;
                    case "history":
                        await HandleHistoryCommand(service);
                        break;
                    case "seed":
                        await HandleSeedCommand(input);
                        break;
                    case "use_json":
                        ConfigurationManager.SetStorageType("JSON");
                        Console.WriteLine("Switched to JSON storage. Restart to take effect.");
                        break;
                    case "use_xml":
                        ConfigurationManager.SetStorageType("XML");
                        Console.WriteLine("Switched to XML storage. Restart to take effect.");
                        break;
                    case "use_sql":
                        ConfigurationManager.SetStorageType("SQL");
                        Console.WriteLine("Switched to SQL storage. Restart to take effect.");
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static async Task HandleAddCommand(TaskService service)
    {
        Console.Write("Title: ");
        var title = Console.ReadLine()!;

        Console.Write("Due (yyyy-MM-dd HH:mm): ");
        var dueString = Console.ReadLine()!;
        if (!DateTime.TryParse(dueString, out var due))
        {
            Console.WriteLine("Invalid date format. Please use yyyy-MM-dd HH:mm format.");
            return;
        }

        if (due < DateTime.Now)
        {
            Console.WriteLine("Error: Due date cannot be in the past. Please enter a future date.");
            return;
        }

        Console.Write("Priority (1–5): ");
        if (!int.TryParse(Console.ReadLine(), out var pri) || pri < 1 || pri > 5)
        {
            Console.WriteLine("Invalid priority. Please enter a number between 1 and 5.");
            return;
        }

        var result = await service.AddAsync(title, due, pri);
        Console.WriteLine(result);
    }

    private static async Task HandleListCommand(TaskService service)
    {
        var tasks = await service.ListAsync();
        Console.WriteLine("\n");
        
        foreach (var t in tasks)
            Console.WriteLine($"{t.Id,-4} | {t.Title,-25} | Due {t.DueUtc:yyyy-MM-dd HH:mm} | P{t.Priority} | {t.Status,-8} | {t.Score}");
        
       // Console.WriteLine("\n");
    } 

    private static async Task HandleDoneCommand(TaskService service)
    { 
        Console.Write("Task ID: ");
        if (int.TryParse(Console.ReadLine(), out var id))
        { 
            var result = await service.MarkDoneAsync(id);
            Console.WriteLine(result);
        }
        else
            Console.WriteLine("Invalid ID.");
    }

    private static async Task HandleDeleteCommand(TaskService service)
    {
        Console.Write("Task ID: ");
        if (int.TryParse(Console.ReadLine(), out var id))
        {
            var result = await service.DeleteTaskAsync(id);
            Console.WriteLine(result);
        }
        else
            Console.WriteLine("Invalid ID.");
    }

    private static async Task HandleEditCommand(TaskService service)
    {
        Console.Write("Task ID: ");
        if (!int.TryParse(Console.ReadLine(), out var editId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var task = await service.GetTaskByIdAsync(editId);
        if (task == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }

        Console.Write("New Title (press Enter to keep current): ");
        var newTitle = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newTitle)) task.Title = newTitle;

        Console.Write("New Due Date (yyyy-MM-dd HH:mm, press Enter to keep current): ");
        var newDue = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newDue))
        {
            if (!DateTime.TryParse(newDue, out var parsedDue))
            {
                Console.WriteLine("Invalid date format. Task not updated.");
                return;
            }
            
            if (parsedDue < DateTime.Now)
            {
                Console.WriteLine("Error: Due date cannot be in the past. Task not updated.");
                return;
            }
            
            task.DueUtc = parsedDue;
        }

        Console.Write("New Priority (1-5, press Enter to keep current): ");
        var newPri = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newPri) && int.TryParse(newPri, out var parsedPri))
            task.Priority = parsedPri;

        var result = await service.UpdateTaskAsync(task);
        Console.WriteLine(result);
    }

    private static async Task HandleDailyReportCommand(TaskService service)
    {
        Console.Write("Report date (yyyy-MM-dd, press Enter for today): ");
        var dateInput = Console.ReadLine();
        
        DateTime reportDate = DateTime.Today;
        if (!string.IsNullOrWhiteSpace(dateInput) && !DateTime.TryParse(dateInput, out reportDate))
        {
            Console.WriteLine("Invalid date format. Using today's date.");
            reportDate = DateTime.Today;
        }
        
        Console.Write("Output folder (press Enter for default): ");
        var outputPath = Console.ReadLine();
        
        // Create a new repository instance for the report service
        var repo = RepositoryFactory.CreateTaskRepository();
        var reportService = new ReportService(repo);
        
        string? finalOutputPath = string.IsNullOrWhiteSpace(outputPath) ? null : outputPath;
        var savedPath = await reportService.SaveDailyReportAsync(reportDate, finalOutputPath);
        
        // Check if the result is a path or an error message
        if (File.Exists(savedPath))
        {
            Console.WriteLine($"\nDaily report generated successfully!\nSaved to: {savedPath}\n");
            
            // Preview the report
            Console.WriteLine("Report Preview:\n");
            var report = await reportService.GenerateDailyReportAsync(reportDate);
            Console.WriteLine(report);
        }
        else
        {
            Console.WriteLine(savedPath); // Display error message
        }
    }
    
    private static async Task HandleHistoryCommand(TaskService service)
    {
        try
        {
            var completedTasks = await service.GetCompletedTasksAsync();
            
            if (completedTasks.Count == 0)
            {
                Console.WriteLine("\nNo completed tasks found in history.\n");
                return;
            }
            
            Console.WriteLine("\nCOMPLETED TASKS HISTORY");
            Console.WriteLine("======================\n");
            Console.WriteLine("{0,-5} {1,-30} {2,-20} {3,-20}", "ID", "TITLE", "CREATED", "COMPLETED");
            Console.WriteLine(new string('-', 75));
            
            foreach (var task in completedTasks)
            {
                Console.WriteLine("{0,-5} {1,-30} {2,-20:yyyy-MM-dd HH:mm} {3,-20:yyyy-MM-dd HH:mm}", 
                    task.Id, 
                    task.Title.Length > 27 ? task.Title.Substring(0, 24) + "..." : task.Title, 
                    task.CreatedDate, 
                    task.CompletedDate);
            }
            
            Console.WriteLine("\nTotal: {0} completed tasks\n", completedTasks.Count);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving history: {ex.Message}");
        }
    }

    private static async Task HandleSeedCommand(string input)
    {
        // Use test database for seeding to avoid polluting production
        var service = new TaskService(new AdoNetRepo(Configuration.ProductionConnectionString));
        var parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        
        int count = 5000;
        if (parts.Length > 1 && !int.TryParse(parts[1], out count))
        {
            Console.WriteLine("Invalid number provided. Seeding 5000 fake tasks by default.");
            count = 5000;
        }

        Console.WriteLine($"Seeding {count} sample tasks to database...");
        var result = await TaskSeeder.SeedAsync(service, count);
        Console.WriteLine(result);
        
        var tasks = await service.ListAsync();
        Console.WriteLine($"\nTotal tasks in database: {tasks.Count}\n");
    }
} 