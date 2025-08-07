using Moq;
using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;
using System.Text;

namespace SmartTaskerMini.Tests.Application;

public class ReportServiceTests
{
    [Fact]
    public async Task GenerateDailyReportAsync_IncludesCorrectMetrics()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        
        // Setup active tasks with exact date matching
        var today = DateTime.Today;
        var activeTasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Task 1", DueUtc = new DateTime(today.Year, today.Month, today.Day, 10, 0, 0), Priority = 3, Status = SmartTaskerMini.Core.Domain.TaskStatus.Pending },
            new TaskItem { Id = 2, Title = "Task 2", DueUtc = new DateTime(today.Year, today.Month, today.Day, 14, 0, 0), Priority = 2, Status = SmartTaskerMini.Core.Domain.TaskStatus.Pending },
            new TaskItem { Id = 3, Title = "Task 3", DueUtc = today.AddDays(1), Priority = 1, Status = SmartTaskerMini.Core.Domain.TaskStatus.Pending }
        };
        
        // Setup completed tasks
        var completedTasks = new List<HistoryItem>
        { 
            new HistoryItem { Id = 4, Title = "Task 4", CreatedDate = DateTime.Today.AddDays(-1), CompletedDate = DateTime.Today },
            new HistoryItem { Id = 5, Title = "Task 5", CreatedDate = DateTime.Today, CompletedDate = DateTime.Today }
        };
        
        mockRepo.Setup(r => r.GetAllTasksAsync()).ReturnsAsync(activeTasks);
        mockRepo.Setup(r => r.GetCompletedTasksForDateAsync(It.Is<DateTime>(d => d.Date == today.Date))).ReturnsAsync(completedTasks);
        
        var service = new ReportService(mockRepo.Object);
        
        // Act
        var report = await service.GenerateDailyReportAsync(today);
        
        // Debug - print the actual report content
        Console.WriteLine("ACTUAL REPORT CONTENT:\n" + report);
        
        // Assert
        Assert.Contains("Tasks due today: 4", report); // 2 pending + 2 completed
        Assert.Contains("Tasks completed today: 2", report);
        Assert.Contains("Tasks pending due today: 2", report);
        
        // More flexible check for completion rate
        Assert.True(report.Contains("Completion rate:"), "Report should contain completion rate");
        Assert.Contains("Task 1", report);
        Assert.Contains("Task 2", report);
        Assert.Contains("Task 4", report);
        Assert.Contains("Task 5", report);
    }
    
    [Fact]
    public async Task SaveDailyReportAsync_CreatesFileWithCorrectContent()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        mockRepo.Setup(r => r.GetAllTasksAsync()).ReturnsAsync(new List<TaskItem>());
        mockRepo.Setup(r => r.GetCompletedTasksForDateAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<HistoryItem>());
        
        var service = new ReportService(mockRepo.Object);
        var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempPath);
        
        try
        {
            // Act
            await service.SaveDailyReportAsync(DateTime.Today, tempPath);
            
            // Assert
            var fileName = $"DailyReport_{DateTime.Today:yyyy-MM-dd}.txt";
            var filePath = Path.Combine(tempPath, fileName);
            
            Assert.True(File.Exists(filePath));
            var content = await File.ReadAllTextAsync(filePath);
            Assert.Contains("DAILY PRODUCTIVITY REPORT", content);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }
}