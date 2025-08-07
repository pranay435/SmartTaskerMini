using Xunit;
using Moq;
using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;

namespace SmartTaskerMini.Tests.Application;

public class TaskServiceIntegrationTests
{
    [Fact]
    public async Task AddEditDeleteFlow_WorksCorrectly()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        var tasks = new List<TaskItem>();
        
        mockRepo.Setup(r => r.AddTaskAsync(It.IsAny<TaskItem>()))
                .Callback<TaskItem>(t => { t.Id = 1; tasks.Add(t); });
        
        mockRepo.Setup(r => r.GetTaskByIdAsync(1))
                .ReturnsAsync(() => tasks.FirstOrDefault(t => t.Id == 1));
        
        mockRepo.Setup(r => r.UpdateTaskAsync(It.IsAny<TaskItem>()))
                .Callback<TaskItem>(t => 
                {
                    var existing = tasks.FirstOrDefault(x => x.Id == t.Id);
                    if (existing != null)
                    {
                        existing.Title = t.Title;
                        existing.Priority = t.Priority;
                        existing.DueUtc = t.DueUtc;
                    }
                });
        
        mockRepo.Setup(r => r.DeleteTaskAsync(1))
                .ReturnsAsync(() => { tasks.RemoveAll(t => t.Id == 1); return true; });

        var service = new TaskService(mockRepo.Object);

        // Act & Assert - Add
        var addResult = await service.AddAsync("Test Task", DateTime.Now.AddDays(1), 2);
        Assert.Contains("successfully", addResult);
        Assert.Single(tasks);

        // Act & Assert - Edit
        var task = await service.GetTaskByIdAsync(1);
        Assert.NotNull(task);
        task.Title = "Updated Task";
        task.Priority = 4;
        
        var updateResult = await service.UpdateTaskAsync(task);
        Assert.Contains("successfully", updateResult);
        Assert.Equal("Updated Task", tasks[0].Title);
        Assert.Equal(4, tasks[0].Priority);

        // Act & Assert - Delete
        var deleteResult = await service.DeleteTaskAsync(1);
        Assert.Contains("Successfully", deleteResult);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task PriorityUpdate_ReflectsInTaskList()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        var task = new TaskItem { Id = 1, Title = "Task", Priority = 2, DueUtc = DateTime.Now.AddDays(1) };
        
        mockRepo.Setup(r => r.GetTaskByIdAsync(1)).ReturnsAsync(task);
        mockRepo.Setup(r => r.UpdateTaskAsync(It.IsAny<TaskItem>()))
                .Callback<TaskItem>(t => task.Priority = t.Priority);
        mockRepo.Setup(r => r.GetAllTasksAsync()).ReturnsAsync(new List<TaskItem> { task });

        var service = new TaskService(mockRepo.Object);

        // Act
        var retrievedTask = await service.GetTaskByIdAsync(1);
        retrievedTask.Priority = 5;
        await service.UpdateTaskAsync(retrievedTask);
        
        var allTasks = await service.ListAsync();

        // Assert
        Assert.Equal(5, allTasks[0].Priority);
    }
}