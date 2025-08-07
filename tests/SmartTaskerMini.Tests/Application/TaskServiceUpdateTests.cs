using Xunit;
using Moq;
using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;

namespace SmartTaskerMini.Tests.Application;

public class TaskServiceUpdateTests
{
    [Fact]
    public async Task UpdateTaskAsync_ValidTask_ReturnsSuccessMessage()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        var service = new TaskService(mockRepo.Object);
        var task = new TaskItem 
        { 
            Id = 1, 
            Title = "Updated Task", 
            DueUtc = DateTime.Now.AddDays(1), 
            Priority = 3 
        };

        // Act
        var result = await service.UpdateTaskAsync(task);

        // Assert
        Assert.Contains("successfully", result);
        mockRepo.Verify(r => r.UpdateTaskAsync(task), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_PastDueDate_ThrowsArgumentException()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        var service = new TaskService(mockRepo.Object);
        var task = new TaskItem 
        { 
            Id = 1, 
            Title = "Task", 
            DueUtc = DateTime.Now.AddDays(-1), 
            Priority = 3 
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateTaskAsync(task));
    }

    [Fact]
    public async Task UpdateTaskAsync_RepositoryThrows_ReturnsErrorMessage()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        mockRepo.Setup(r => r.UpdateTaskAsync(It.IsAny<TaskItem>()))
                .ThrowsAsync(new Exception("Database error"));
        var service = new TaskService(mockRepo.Object);
        var task = new TaskItem 
        { 
            Id = 1, 
            Title = "Task", 
            DueUtc = DateTime.Now.AddDays(1), 
            Priority = 3 
        };

        // Act
        var result = await service.UpdateTaskAsync(task);

        // Assert
        Assert.Contains("Error updating task", result);
    }
}