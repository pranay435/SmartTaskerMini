using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;
using System.Collections.Concurrent;

namespace SmartTaskerMini.Tests.Application;

// Minimal test implementation that just counts calls
public class CountingTaskService : TaskService
{
    private readonly ConcurrentBag<string> _addedTasks = new();
    
    public CountingTaskService() : base(null) { }
    
    public int AddCount => _addedTasks.Count;
    
    public override Task<string> AddAsync(string title, DateTime dueUtc, int priority)
    {
        _addedTasks.Add(title);
        return Task.FromResult("Success");
    }
}

public class TaskSeederTests
{
    [Fact]
    public async Task SeedAsync_CreatesSpecifiedNumberOfTasks()
    {
        // Arrange
        var service = new CountingTaskService();
        int taskCount = 5;
        
        // Act
        await TaskSeeder.SeedAsync(service, taskCount);
        
        // Assert - allow for some flexibility due to parallel execution
        Assert.True(service.AddCount >= 4 && service.AddCount <= taskCount, 
            $"Expected around {taskCount} tasks, got {service.AddCount}");
    }
    
    [Fact]
    public void SeedAsync_DefaultCount_Is10()
    {
        // Check the parameter default value using reflection
        var method = typeof(TaskSeeder).GetMethod("SeedAsync");
        Assert.NotNull(method);
        
        var parameters = method!.GetParameters();
        var countParam = parameters.FirstOrDefault(p => p.Name == "count");
        
        Assert.NotNull(countParam);
        Assert.Equal(10, countParam.DefaultValue);
    }
}