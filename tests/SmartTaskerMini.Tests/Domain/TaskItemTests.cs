using SmartTaskerMini.Core.Domain;

namespace SmartTaskerMini.Tests.Domain;

using DomainTaskStatus = SmartTaskerMini.Core.Domain.TaskStatus;


public class TaskItemTests
{
    [Fact]
    public void MarkDone_SetsStatusToDone()
    {
        var task = new TaskItem { Status = DomainTaskStatus.Pending };

        task.MarkDone();

        Assert.Equal(DomainTaskStatus.Done, task.Status);
    }

    [Fact]
    public void TaskItem_DefaultValues_AreCorrect()
    {
        var task = new TaskItem();

        Assert.Equal(0, task.Id);
        Assert.Equal("", task.Title);
        Assert.Equal(DomainTaskStatus.Pending, task.Status);
        Assert.Equal(0, task.Priority);
        Assert.Equal(0, task.Score);
    }
} 