using System.ComponentModel.DataAnnotations;

namespace SmartTaskerMini.WebApp.Models;

public class TaskViewModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = "";

    [Required]
    public DateTime DueUtc { get; set; } = DateTime.Now.AddDays(1);

    [Range(1, 5)]
    public int Priority { get; set; } = 3;
}