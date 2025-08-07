using Xunit;
using SmartTaskerMini.Core.Domain;
using SmartTaskerMini.Core.Infrastructure;

namespace SmartTaskerMini.Tests.Infrastructure;

public class FileRepositoryComparisonTests : IDisposable
{
    private readonly JsonTaskRepository _jsonRepo;
    private readonly XmlTaskRepository _xmlRepo;
    private readonly string _jsonFile = "comparison_test.json";
    private readonly string _xmlFile = "comparison_test.xml";

    public FileRepositoryComparisonTests()
    {
        _jsonRepo = new JsonTaskRepository(_jsonFile);
        _xmlRepo = new XmlTaskRepository(_xmlFile);
    }

    [Fact]
    public async Task BothRepositories_SameOperations_ProduceSameResults()
    {
        // Arrange
        var jsonTask = new TaskItem { Title = "JSON Task", Priority = 3, DueUtc = DateTime.Now.AddDays(1) };
        var xmlTask = new TaskItem { Title = "XML Task", Priority = 3, DueUtc = DateTime.Now.AddDays(1) };

        // Act - Add tasks
        await _jsonRepo.AddTaskAsync(jsonTask);
        await _xmlRepo.AddTaskAsync(xmlTask);

        // Act - Update tasks
        jsonTask.Priority = 5;
        xmlTask.Priority = 5;
        await _jsonRepo.UpdateTaskAsync(jsonTask);
        await _xmlRepo.UpdateTaskAsync(xmlTask);

        // Act - Get tasks
        var jsonTasks = await _jsonRepo.GetAllTasksAsync();
        var xmlTasks = await _xmlRepo.GetAllTasksAsync();

        // Assert
        Assert.Equal(jsonTasks.Count, xmlTasks.Count);
        Assert.Single(jsonTasks);
        Assert.Single(xmlTasks);
        Assert.Equal(5, jsonTasks[0].Priority);
        Assert.Equal(5, xmlTasks[0].Priority);
        Assert.Equal("JSON Task", jsonTasks[0].Title);
        Assert.Equal("XML Task", xmlTasks[0].Title);
    }

    [Fact]
    public async Task BothRepositories_HistoryOperations_WorkIdentically()
    {
        // Arrange
        var jsonTask = new TaskItem { Title = "JSON History", Priority = 2, DueUtc = DateTime.Now.AddDays(1) };
        var xmlTask = new TaskItem { Title = "XML History", Priority = 2, DueUtc = DateTime.Now.AddDays(1) };

        await _jsonRepo.AddTaskAsync(jsonTask);
        await _xmlRepo.AddTaskAsync(xmlTask);

        // Act
        await _jsonRepo.MoveToHistoryAsync(jsonTask);
        await _xmlRepo.MoveToHistoryAsync(xmlTask);

        var jsonHistory = await _jsonRepo.GetAllCompletedTasksAsync();
        var xmlHistory = await _xmlRepo.GetAllCompletedTasksAsync();

        // Assert
        Assert.Equal(jsonHistory.Count, xmlHistory.Count);
        Assert.Equal("JSON History", jsonHistory[0].Title);
        Assert.Equal("XML History", xmlHistory[0].Title);
    }

    [Fact]
    public async Task BothRepositories_HandleEmptyState_Consistently()
    {
        // Act
        var jsonTasks = await _jsonRepo.GetAllTasksAsync();
        var xmlTasks = await _xmlRepo.GetAllTasksAsync();
        var jsonHistory = await _jsonRepo.GetAllCompletedTasksAsync();
        var xmlHistory = await _xmlRepo.GetAllCompletedTasksAsync();

        // Assert
        Assert.Empty(jsonTasks);
        Assert.Empty(xmlTasks);
        Assert.Empty(jsonHistory);
        Assert.Empty(xmlHistory);
    }

    [Fact]
    public async Task BothRepositories_DeleteOperations_WorkIdentically()
    {
        // Arrange
        var jsonTask = new TaskItem { Title = "JSON Delete", Priority = 1, DueUtc = DateTime.Now.AddDays(1) };
        var xmlTask = new TaskItem { Title = "XML Delete", Priority = 1, DueUtc = DateTime.Now.AddDays(1) };

        await _jsonRepo.AddTaskAsync(jsonTask);
        await _xmlRepo.AddTaskAsync(xmlTask);

        // Act
        var jsonResult = await _jsonRepo.DeleteTaskAsync(jsonTask.Id);
        var xmlResult = await _xmlRepo.DeleteTaskAsync(xmlTask.Id);

        var jsonTasks = await _jsonRepo.GetAllTasksAsync();
        var xmlTasks = await _xmlRepo.GetAllTasksAsync();

        // Assert
        Assert.Equal(jsonResult, xmlResult);
        Assert.True(jsonResult);
        Assert.Empty(jsonTasks);
        Assert.Empty(xmlTasks);
    }

    public void Dispose()
    {
        var filesToDelete = new[]
        {
            _jsonFile,
            Path.ChangeExtension(_jsonFile, ".history.json"),
            _xmlFile,
            Path.ChangeExtension(_xmlFile, ".history.xml")
        };

        foreach (var file in filesToDelete)
        {
            if (File.Exists(file))
                File.Delete(file);
        }
    }
}