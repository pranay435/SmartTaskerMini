using SmartTaskerMini.Core.Domain;

namespace SmartTaskerMini.Core.Application;

public static class TaskSeeder
{
    private const int DefaultCount = 10;
    
    public static async Task<string> SeedAsync(TaskService svc, int count = DefaultCount)
    {
        try
        {
            var rnd = new Random();
            var now = DateTime.Now;

            var tasks = Enumerable.Range(0, count).Select(i =>
              
                new TaskItem 
                {
                    Title = $"Seed Task #{i}",
                    DueUtc = now.AddHours(rnd.Next(0, 72)).AddMinutes(rnd.Next(1, 60)), // Always future dates
                    Priority = rnd.Next(1, 5)
                }).ToList();

            await Parallel.ForEachAsync(tasks, async (t, _) => await svc.AddAsync(t.Title, t.DueUtc, t.Priority));
            return $"Successfully seeded {count} tasks";
        }
        catch (Exception ex) 
        {
            return $"Error seeding tasks: {ex.Message}";
        }
    }
}

