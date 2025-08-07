using Xunit;
using SmartTaskerMini.Core.Domain;
using SmartTaskerMini.Core.Infrastructure;

namespace SmartTaskerMini.Tests.Infrastructure;

public class XmlTaskRepositoryTests : IDisposable
{
    private readonly string _testFilePath = "test_tasks.xml";
    private readonly XmlTaskRepository _repository;

    public XmlTaskRepositoryTests()
    {
        _repository = new XmlTaskRepository(_testFilePath);
    }

    [Fact]
    public async Task AddTaskAsync_ValidTask_AddsSuccessfully()
    {
        // Arrange
        var task = new TaskItem { Title = "XML Test Task", Priority = 3, DueUtc = DateTime.Now.AddDays(1) };

        // Act
        await _repository.AddTaskAsync(task);
        var tasks = await _repository.GetAllTasksAsync();

        // Assert
        Assert.Contains(tasks, t => t.Title == "XML Test Task");
        Assert.True(task.Id > 0);
    }

    [Fact]
    public async Task UpdateTaskAsync_ExistingTask_UpdatesSuccessfully()
    {
        // Arrange
        var task = new TaskItem { Title = "XML Original", Priority = 2, DueUtc = DateTime.Now.AddDays(1) };
        await _repository.AddTaskAsync(task);

        // Act
        task.Title = "XML Updated";
        task.Priority = 4;
        await _repository.UpdateTaskAsync(task);
        var updatedTask = await _repository.GetTaskByIdAsync(task.Id);

        // Assert
        Assert.Equal("XML Updated", updatedTask?.Title);
        Assert.Equal(4, updatedTask?.Priority);
    }

    [Fact]
    public async Task DeleteTaskAsync_ExistingTask_DeletesSuccessfully()
    {
        // Arrange
        var task = new TaskItem { Title = "XML To Delete", Priority = 1, DueUtc = DateTime.Now.AddDays(1) };
        await _repository.AddTaskAsync(task);

        // Act
        var result = await _repository.DeleteTaskAsync(task.Id);
        var deletedTask = await _repository.GetTaskByIdAsync(task.Id);

        // Assert
        Assert.True(result);
        Assert.Null(deletedTask);
    }

    [Fact]
    public async Task MoveToHistoryAsync_ValidTask_MovesToHistory()
    {
        // Arrange
        var task = new TaskItem { Title = "XML Complete Me", Priority = 3, DueUtc = DateTime.Now.AddDays(1) };
        await _repository.AddTaskAsync(task);

        // Act
        await _repository.MoveToHistoryAsync(task);
        var remainingTasks = await _repository.GetAllTasksAsync();
        var history = await _repository.GetAllCompletedTasksAsync();

        // Assert
        Assert.DoesNotContain(remainingTasks, t => t.Id == task.Id);
        Assert.Contains(history, h => h.Id == task.Id && h.Title == "XML Complete Me");
    }

    [Fact]
    public async Task GetCompletedTasksForDateAsync_FiltersByDate()
    {
        // Arrange
        var task = new TaskItem { Title = "XML Daily Task", Priority = 2, DueUtc = DateTime.Now.AddDays(1) };
        await _repository.AddTaskAsync(task);
        await _repository.MoveToHistoryAsync(task);
        var today = DateTime.Today;

        // Act
        var todayTasks = await _repository.GetCompletedTasksForDateAsync(today);

        // Assert
        Assert.Contains(todayTasks, h => h.Title == "XML Daily Task");
    }

    [Fact]
    public async Task XmlFilePersistence_DataSurvivesReload()
    {
        // Arrange
        var task = new TaskItem { Title = "Persistence Test", Priority = 3, DueUtc = DateTime.Now.AddDays(1) };
        await _repository.AddTaskAsync(task);

        // Act - Create new repository instance to test file persistence
        var newRepository = new XmlTaskRepository(_testFilePath);
        var tasks = await newRepository.GetAllTasksAsync();

        // Assert
        Assert.Contains(tasks, t => t.Title == "Persistence Test");
    }

    [Fact]
    public async Task XmlFormat_HandlesSpecialCharacters()
    {
        // Arrange
        var task = new TaskItem { Title = "Task with <special> & \"characters\"", Priority = 2, DueUtc = DateTime.Now.AddDays(1) };

        // Act
        await _repository.AddTaskAsync(task);
        var retrievedTask = await _repository.GetTaskByIdAsync(task.Id);

        // Assert
        Assert.Equal("Task with <special> & \"characters\"", retrievedTask?.Title);
    }

    [Fact]
    public async Task TaskScoring_CalculatesCorrectly()
    {
        // Arrange
        var highPriorityTask = new TaskItem { Title = "XML High Priority", Priority = 5, DueUtc = DateTime.Now.AddHours(1) };
        var lowPriorityTask = new TaskItem { Title = "XML Low Priority", Priority = 1, DueUtc = DateTime.Now.AddHours(1) };

        // Act
        await _repository.AddTaskAsync(highPriorityTask);
        await _repository.AddTaskAsync(lowPriorityTask);
        var tasks = await _repository.GetAllTasksAsync();

        // Assert
        var highTask = tasks.First(t => t.Title == "XML High Priority");
        var lowTask = tasks.First(t => t.Title == "XML Low Priority");
        Assert.True(highTask.Score > lowTask.Score);
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
        if (File.Exists(Path.ChangeExtension(_testFilePath, ".history.xml")))
            File.Delete(Path.ChangeExtension(_testFilePath, ".history.xml"));
    }
}