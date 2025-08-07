using Moq;
using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;

namespace SmartTaskerMini.Tests.Application;

public class TaskServiceHistoryTests
{
    [Fact]  
    public async Task GetCompletedTasksAsync_ReturnsHistoryItems()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        var expectedHistory = new List<HistoryItem>
        {
            new HistoryItem 
            { 
                Id = 1, 
                Title = "Completed Task 1", 
                CreatedDate = DateTime.Today.AddDays(-2),
                CompletedDate = DateTime.Today.AddDays(-1)
            },
            new HistoryItem 
            { 
                Id = 2, 
                Title = "Completed Task 2", 
                CreatedDate = DateTime.Today.AddDays(-1),
                CompletedDate = DateTime.Today
            }
        };
        
        mockRepo.Setup(r => r.GetAllCompletedTasksAsync())
                .ReturnsAsync(expectedHistory);
                
        var service = new TaskService(mockRepo.Object);
        
        // Act
        var result = await service.GetCompletedTasksAsync();
        
        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Completed Task 1", result[0].Title);
        Assert.Equal("Completed Task 2", result[1].Title);
        mockRepo.Verify(r => r.GetAllCompletedTasksAsync(), Times.Once);
    }
    
    [Fact]
    public async Task MarkDoneAsync_MovesTaskToHistory()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        var task = new TaskItem { Id = 1, Title = "Test Task", DueUtc = DateTime.Now.AddDays(1) };
        
        mockRepo.Setup(r => r.GetTaskByIdAsync(1))
                .ReturnsAsync(task);
                
        mockRepo.Setup(r => r.MoveToHistoryAsync(It.IsAny<TaskItem>()))
                .Returns(Task.CompletedTask);
                
        var service = new TaskService(mockRepo.Object);
        
        // Act
        await service.MarkDoneAsync(1);
        
        // Assert
        mockRepo.Verify(r => r.MoveToHistoryAsync(It.Is<TaskItem>(t => t.Id == 1)), Times.Once);
    }
}