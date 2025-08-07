using Moq;
using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;

namespace SmartTaskerMini.Tests.Application;

public class TaskServiceValidationTests
{
    private readonly Mock<ITaskRepository> _mockRepo;
    private readonly TaskService _service;

    public TaskServiceValidationTests()
    {
        _mockRepo = new Mock<ITaskRepository>();
        _service = new TaskService(_mockRepo.Object);
    }

    [Fact]
    public async Task AddAsync_WithPastDueDate_ThrowsArgumentException()
    {
        // Arrange
        var pastDate = DateTime.Now.AddDays(-1);
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.AddAsync("Test Task", pastDate, 3));
        
        Assert.Contains("past", exception.Message);
        _mockRepo.Verify(r => r.AddTaskAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_WithFutureDueDate_Succeeds()
    {
        // Arrange
        var futureDate = DateTime.Now.AddDays(1);
        
        // Act
        await _service.AddAsync("Test Task", futureDate, 3);
        
        // Assert
        _mockRepo.Verify(r => r.AddTaskAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithPastDueDate_ThrowsArgumentException()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "Test Task",
            DueUtc = DateTime.Now.AddDays(-1),
            Priority = 3
        };
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UpdateTaskAsync(task));
        
        Assert.Contains("past", exception.Message);
        _mockRepo.Verify(r => r.UpdateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithFutureDueDate_Succeeds()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "Test Task",
            DueUtc = DateTime.Now.AddDays(1),
            Priority = 3
        };
        
        // Act
        await _service.UpdateTaskAsync(task);
        
        // Assert
        _mockRepo.Verify(r => r.UpdateTaskAsync(task), Times.Once);
    }
}