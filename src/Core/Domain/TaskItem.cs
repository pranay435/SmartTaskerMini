namespace SmartTaskerMini.Core.Domain;

/// <summary>
/// Represents a task in the system with its properties and behaviors
/// </summary>
public class TaskItem
{
	public int Id { get; set; }  
	public string Title { get; set; } = "";
	public DateTime DueUtc { get; set; }
	public int Priority { get; set; }
	public TaskStatus Status { get; set; }  

	public int Score { get; set; }


	public void MarkDone() => Status = TaskStatus.Done;
}
