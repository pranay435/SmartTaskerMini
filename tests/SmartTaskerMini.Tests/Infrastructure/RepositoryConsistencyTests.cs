using Xunit;
using SmartTaskerMini.Core.Domain;
using SmartTaskerMini.Core.Infrastructure;
using SmartTaskerMini.Core.Application;

namespace SmartTaskerMini.Tests.Infrastructure;

public class RepositoryConsistencyTests
{
    [Theory]
    [InlineData("SQL")]
    [InlineData("JSON")]
    [InlineData("XML")]
    public async Task UpdateTask_AllStorageTypes_ReflectChangesImmediately(string storageType)
    {
        // Arrange
        ConfigurationManager.SetStorageType(storageType);
        var repo = RepositoryFactory.CreateTaskRepository();
        
        var task = new TaskItem 
        { 
            Title = "Original Task", 
            Priority = 2, 
            DueUtc = DateTime.Now.AddDays(1) 
        };

        // Act - Add task
        await repo.AddTaskAsync(task);
        var allTasks = await repo.GetAllTasksAsync();
        var addedTask = allTasks.First(t => t.Title == "Original Task");

        // Act - Update task
        addedTask.Title = "Updated Task";
        addedTask.Priority = 4;
        await repo.UpdateTaskAsync(addedTask);

        // Act - Retrieve updated task
        var updatedTask = await repo.GetTaskByIdAsync(addedTask.Id);
        var allTasksAfterUpdate = await repo.GetAllTasksAsync();
        var taskInList = allTasksAfterUpdate.First(t => t.Id == addedTask.Id);

        // Assert
        Assert.Equal("Updated Task", updatedTask.Title);
        Assert.Equal(4, updatedTask.Priority);
        Assert.Equal("Updated Task", taskInList.Title);
        Assert.Equal(4, taskInList.Priority);

        // Cleanup
        await repo.DeleteTaskAsync(addedTask.Id);
    }

    [Theory]
    [InlineData("SQL")]
    [InlineData("JSON")]
    [InlineData("XML")]
    public async Task AddDeleteTask_AllStorageTypes_ConsistentBehavior(string storageType)
    {
        // Arrange
        ConfigurationManager.SetStorageType(storageType);
        var repo = RepositoryFactory.CreateTaskRepository();
        
        var initialCount = (await repo.GetAllTasksAsync()).Count;
        var task = new TaskItem 
        { 
            Title = "Test Task", 
            Priority = 3, 
            DueUtc = DateTime.Now.AddDays(1) 
        };

        // Act - Add
        await repo.AddTaskAsync(task);
        var afterAdd = await repo.GetAllTasksAsync();
        var addedTask = afterAdd.First(t => t.Title == "Test Task");

        // Act - Delete
        await repo.DeleteTaskAsync(addedTask.Id);
        var afterDelete = await repo.GetAllTasksAsync();

        // Assert
        Assert.Equal(initialCount + 1, afterAdd.Count);
        Assert.Equal(initialCount, afterDelete.Count);
        Assert.DoesNotContain(afterDelete, t => t.Id == addedTask.Id);
    }
}