using Moq;
using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;
using DomainTaskStatus = SmartTaskerMini.Core.Domain.TaskStatus;


namespace SmartTaskerMini.Tests.Application;

public class TaskServiceUnitTests
{
    private readonly Mock<ITaskRepository> _mockRepo;
    private readonly TaskService _service;
 
    public TaskServiceUnitTests()
    {
        _mockRepo = new Mock<ITaskRepository>();
        _service = new TaskService(_mockRepo.Object);
    }

    [Fact]
    public async Task AddAsync_CallsRepositoryAddTaskAsync()
    {
        var title = "Test Task";
        var dueDate = DateTime.UtcNow.AddDays(1);
        var priority = 3;

        await _service.AddAsync(title, dueDate, priority);

        _mockRepo.Verify(r => r.AddTaskAsync(It.Is<TaskItem>(t => 
            t.Title == title && 
            t.DueUtc == dueDate && 
            t.Priority == priority)), Times.Once);
    }

    [Fact]
    public async Task ListAsync_ReturnsTasksFromRepository()
    {
        var expectedTasks = new List<TaskItem>
        {
            new() { Id = 1, Title = "Task 1" },
            new() { Id = 2, Title = "Task 2" }
        };
        _mockRepo.Setup(r => r.GetAllTasksAsync()).ReturnsAsync(expectedTasks);

        var result = await _service.ListAsync();

        Assert.Equal(expectedTasks, result);
    }

    [Fact]
    public async Task MarkDoneAsync_TaskExists_MovesToHistory()
    {
        var task = new TaskItem { Id = 1, Title = "Test", Status = DomainTaskStatus.Pending };
        _mockRepo.Setup(r => r.GetTaskByIdAsync(1)).ReturnsAsync(task);

        var result = await _service.MarkDoneAsync(1);

        Assert.Equal("Task completed and moved to history", result);
        _mockRepo.Verify(r => r.MoveToHistoryAsync(It.Is<TaskItem>(t => 
            t.Id == 1 && t.Status == DomainTaskStatus.Done)), Times.Once);
    }

    [Fact]
    public async Task MarkDoneAsync_TaskNotFound_ReturnsInvalidGuid()
    {
        _mockRepo.Setup(r => r.GetTaskByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        var result = await _service.MarkDoneAsync(999);

        Assert.Equal("Invalid guid", result);
        _mockRepo.Verify(r => r.MoveToHistoryAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTaskAsync_TaskExists_ReturnsSuccessMessage()
    {
        _mockRepo.Setup(r => r.DeleteTaskAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteTaskAsync(1);

        Assert.Equal("Successfully Deleted!", result);
    }

    [Fact]
    public async Task DeleteTaskAsync_TaskNotFound_ReturnsInvalidId()
    {
        _mockRepo.Setup(r => r.DeleteTaskAsync(999)).ReturnsAsync(false);

        var result = await _service.DeleteTaskAsync(999);

        Assert.Equal("Invalid Id.", result);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsTaskFromRepository()
    {
        var expectedTask = new TaskItem { Id = 1, Title = "Test Task" };
        _mockRepo.Setup(r => r.GetTaskByIdAsync(1)).ReturnsAsync(expectedTask);

        var result = await _service.GetTaskByIdAsync(1);

        Assert.Equal(expectedTask, result);
    }

    [Fact]
    public async Task UpdateTaskAsync_CallsRepositoryUpdateTaskAsync()
    {
        var task = new TaskItem { 
            Id = 1, 
            Title = "Updated Task",
            DueUtc = DateTime.Now.AddDays(1) // Future date
        };

        await _service.UpdateTaskAsync(task);

        _mockRepo.Verify(r => r.UpdateTaskAsync(task), Times.Once);
    }
}