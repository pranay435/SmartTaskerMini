namespace SmartTaskerMini.Core.Domain;

/// <summary>
/// Represents a completed task that has been moved to history
/// </summary>
public class HistoryItem
{
    /// <summary>
    /// The unique identifier of the task
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The title of the task
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// The date when the task was created
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// The date when the task was completed
    /// </summary>
    public DateTime CompletedDate { get; set; }
}