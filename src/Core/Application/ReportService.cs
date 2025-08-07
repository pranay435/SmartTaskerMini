using SmartTaskerMini.Core.Domain;
using DomainTaskStatus = SmartTaskerMini.Core.Domain.TaskStatus;
using System.Text;


namespace SmartTaskerMini.Core.Application;

/// <summary>
/// Service for generating productivity reports
/// </summary>
public class ReportService
{
    private readonly ITaskRepository _repo;
    
    public ReportService(ITaskRepository repo)
    {
        _repo = repo;
    }
    
    /// <summary>
    /// Generates a daily productivity report for the specified date
    /// </summary>
    /// <param name="date">The date to generate the report for</param>
    /// <returns>A formatted string containing the report</returns>
    public async Task<string> GenerateDailyReportAsync(DateTime date)
    {
        try
        {
            var tasks = await _repo.GetAllTasksAsync();
      //  Console.WriteLine("Count of All Tasks : " + tasks.Count);
        // Ensure we're working with local dates for comparison
        var today = date.Date;
        var tomorrow = today.AddDays(1);

        // Get tasks due today - using a more reliable approach with explicit conversion
      //  Console.WriteLine($"DEBUG: Today's date: {today:yyyy-MM-dd}, UTC now: {DateTime.UtcNow:yyyy-MM-dd}");
        
        var tasksDueToday = new List<TaskItem>();
        foreach (var task in tasks)
        {
            // Compare just the date part (day, month, year) without time conversion
            bool isToday = task.DueUtc.Day == today.Day && 
                          task.DueUtc.Month == today.Month && 
                          task.DueUtc.Year == today.Year;
            
           // Console.WriteLine($"DEBUG: Task '{task.Title}' due on {task.DueUtc:yyyy-MM-dd HH:mm} UTC" + 
                            //  $" today is {today:yyyy-MM-dd}, is today: {isToday}");
                              
            if (isToday)
            {
                tasksDueToday.Add(task);
            }
        } 
        // Get completed tasks (from history)
        var completedTasks = await _repo.GetCompletedTasksForDateAsync(today);
        
        // Filter completed tasks that were due today
        var completedTasksDueToday = completedTasks.Where(t => 
            t.CompletedDate.Day == today.Day && 
            t.CompletedDate.Month == today.Month && 
            t.CompletedDate.Year == today.Year).ToList();
        
        // Calculate metrics
        int completedTasksToday = completedTasksDueToday.Count;
        int pendingTasksDueToday = tasksDueToday.Count(t => t.Status == DomainTaskStatus.Pending);
        int totalTasksDueToday = pendingTasksDueToday + completedTasksToday;
        
        double completionRate = totalTasksDueToday > 0 
            ? (double)completedTasksToday / totalTasksDueToday * 100 
            : 0;
            
        // Priority distribution of all active tasks
        var priorityDistribution = tasks
            .GroupBy(t => t.Priority)
            .OrderByDescending(g => g.Key)
            .Select(g => new { Priority = g.Key, Count = g.Count() })
            .ToList();
            
        // Generate report
        var sb = new StringBuilder();
        sb.AppendLine($"DAILY PRODUCTIVITY REPORT - {today:yyyy-MM-dd}");
        sb.AppendLine("===========================================");
        sb.AppendLine();
         
        sb.AppendLine($"Tasks due today: {totalTasksDueToday}");
        sb.AppendLine($"Tasks completed today: {completedTasksToday}");
        sb.AppendLine($"Tasks pending due today: {pendingTasksDueToday}");
        sb.AppendLine($"Completion rate: {completionRate:F1}%");
        sb.AppendLine();
        
        sb.AppendLine("PRIORITY DISTRIBUTION (All Active Tasks)");
        sb.AppendLine("---------------------------------------");
        foreach (var p in priorityDistribution)
        {
            sb.AppendLine($"Priority {p.Priority}: {p.Count} tasks");
        }
        sb.AppendLine();
        
        sb.AppendLine("COMPLETED TASKS TODAY");
        sb.AppendLine("--------------------");
        foreach (var task in completedTasks)
        {

                Console.WriteLine(task.Id + "............");
            sb.AppendLine($"- {task.Title}");
        }
        sb.AppendLine();
        
        sb.AppendLine("PENDING TASKS DUE TODAY");
        sb.AppendLine("----------------------");
        var pendingTasks = tasksDueToday
            .Where(t => t.Status == DomainTaskStatus.Pending)
            .OrderByDescending(t => t.Priority)
            .ToList();
        foreach (var task in pendingTasks)
        {
            sb.AppendLine($"- {task.Title} (Priority: {task.Priority})");
        }
        
            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"Error generating report: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Generates and saves a daily report to a file
    /// </summary>
    /// <param name="date">The date to generate the report for</param>
    /// <param name="outputPath">Optional custom output path</param>
    public async Task<string> SaveDailyReportAsync(DateTime date, string? outputPath = null)
    {
        try
        {
            var report = await GenerateDailyReportAsync(date);
            
            // Default path if none provided
            if (string.IsNullOrEmpty(outputPath))
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                outputPath = Path.Combine(documentsPath, "SmartTaskerReports");
                Directory.CreateDirectory(outputPath);
            }
            
            var fileName = $"DailyReport_{date:yyyy-MM-dd}.txt";
            var fullPath = Path.Combine(outputPath, fileName);
            
            await File.WriteAllTextAsync(fullPath, report);
            
            return fullPath;
        }
        catch (Exception ex)
        {
            return $"Error saving report: {ex.Message}";
        }
    }
}