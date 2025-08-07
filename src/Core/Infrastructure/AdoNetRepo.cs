using Microsoft.Data.SqlClient;
using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;

namespace SmartTaskerMini.Core.Infrastructure;

public class AdoNetRepo : ITaskRepository
{
    private readonly string _connStr;

    public AdoNetRepo(string connStr)
    {
        _connStr = connStr;
    }

    public async Task AddTaskAsync(TaskItem task)
    {
        const string sql = @"
        INSERT INTO Tasks (Title, DueUtc, Priority, Status, Score)
        VALUES (@title, @due, @priority, @status, @score);
        SELECT CAST(SCOPE_IDENTITY() AS int);";

        try
        {
            await using var conn = new SqlConnection(_connStr);
            await conn.OpenAsync().ConfigureAwait(false);
            
            

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@title", System.Data.SqlDbType.NVarChar).Value = task.Title ?? string.Empty;
            cmd.Parameters.Add("@due", System.Data.SqlDbType.DateTime2).Value = task.DueUtc;
            cmd.Parameters.Add("@priority", System.Data.SqlDbType.Int).Value = task.Priority;
            cmd.Parameters.Add("@status", System.Data.SqlDbType.TinyInt).Value = (byte)task.Status;
            cmd.Parameters.Add("@score", System.Data.SqlDbType.Int).Value =
            task.Priority*100 - (int)(task.DueUtc - DateTime.Now).TotalMinutes; // default score

            var result = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
            task.Id = (int)(result ?? throw new InvalidOperationException("Failed to get new task ID"));
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Database error while adding task: {ex.Message}", ex);
        }
    }

    public async Task<List<TaskItem>> GetAllTasksAsync() 
    {
        const string sql = "SELECT Id, Title, DueUtc, Priority, Status, Score FROM Tasks WHERE Status = 0 ORDER BY Score DESC;";

        try
        {
            await using var conn = new SqlConnection(_connStr);
            await conn.OpenAsync().ConfigureAwait(false);

            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

            var list = new List<TaskItem>();
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                list.Add(new TaskItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    DueUtc = reader.GetDateTime(2),
                    Priority = reader.GetInt32(3),
                    Status = (SmartTaskerMini.Core.Domain.TaskStatus)reader.GetByte(4),
                    Score = reader.GetInt32(5)
                });
            }

            return list;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Database error while retrieving tasks: {ex.Message}", ex);
        }
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        const string sql = "SELECT Id, Title, DueUtc, Priority, Status FROM Tasks WHERE Id = @id;";

        try
        {
            await using var conn = new SqlConnection(_connStr);
            await conn.OpenAsync().ConfigureAwait(false);

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;

            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

            if (await reader.ReadAsync().ConfigureAwait(false))
            {
                return new TaskItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    DueUtc = reader.GetDateTime(2),
                    Priority = reader.GetInt32(3),
                    Status = (SmartTaskerMini.Core.Domain.TaskStatus)reader.GetByte(4)
                };
            }

            return null;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Database error while retrieving task #{id}: {ex.Message}", ex);
        }
    }

    public async Task UpdateTaskAsync(TaskItem task)
    {
        var setParts = new List<string>();
        var parameters = new List<(string name, System.Data.SqlDbType type, object value)>();

        if (!string.IsNullOrEmpty(task.Title))
        {
            setParts.Add("Title = @title");
            parameters.Add(("@title", System.Data.SqlDbType.NVarChar, task.Title));
        }
        
        if (task.DueUtc != default)
        {
            setParts.Add("DueUtc = @due");
            parameters.Add(("@due", System.Data.SqlDbType.DateTime2, task.DueUtc));
        }
        
        if (task.Priority > 0)
        {
            setParts.Add("Priority = @priority");
            parameters.Add(("@priority", System.Data.SqlDbType.Int, task.Priority));
        }
        
        setParts.Add("Status = @status");
        parameters.Add(("@status", System.Data.SqlDbType.TinyInt, (byte)task.Status));

        if (setParts.Count == 0) return;
        int score = task.Priority*100 - (int)(task.DueUtc - DateTime.Now).TotalMinutes;
        setParts.Add("Score = @score");
        parameters.Add(("@score", System.Data.SqlDbType.Int, score));
        var sql = $"UPDATE Tasks SET {string.Join(", ", setParts)} WHERE Id = @id;";
        parameters.Add(("@id", System.Data.SqlDbType.Int, task.Id));

        try
        {
            await using var conn = new SqlConnection(_connStr);
            await conn.OpenAsync().ConfigureAwait(false);

            await using var cmd = new SqlCommand(sql, conn);
            foreach (var (name, type, value) in parameters)
                cmd.Parameters.Add(name, type).Value = value;

            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Database error while updating task #{task.Id}: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        const string sql = "DELETE FROM Tasks WHERE Id = @id;";

        try
        {
            await using var conn = new SqlConnection(_connStr);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;

            await conn.OpenAsync().ConfigureAwait(false);
            int rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

            return rows == 1;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Database error while deleting task #{id}: {ex.Message}", ex);
        }
    }

    public async Task MoveToHistoryAsync(TaskItem task)
    {
        const string insertSql = "INSERT INTO History (Id, Title, CreatedDate, CompletedDate) VALUES (@id, @title, @created, @completed);";
        const string deleteSql = "DELETE FROM Tasks WHERE Id = @id;";

        try
        {
            await using var conn = new SqlConnection(_connStr);
            await conn.OpenAsync().ConfigureAwait(false);
            
            await using var transaction = conn.BeginTransaction();
            
            // Insert into History
            await using var insertCmd = new SqlCommand(insertSql, conn, transaction);
            insertCmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = task.Id;
            insertCmd.Parameters.Add("@title", System.Data.SqlDbType.NVarChar).Value = task.Title;
            insertCmd.Parameters.Add("@created", System.Data.SqlDbType.DateTime2).Value = task.DueUtc;
            insertCmd.Parameters.Add("@completed", System.Data.SqlDbType.DateTime2).Value = DateTime.Now;
            await insertCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            
            // Delete from Tasks
            await using var deleteCmd = new SqlCommand(deleteSql, conn, transaction);
            deleteCmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = task.Id;
            await deleteCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            
            await transaction.CommitAsync().ConfigureAwait(false);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Database error while moving task #{task.Id} to history: {ex.Message}", ex);
        }
    }
    
    public async Task<List<HistoryItem>> GetCompletedTasksForDateAsync(DateTime date)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);
        
        const string sql = @"
            SELECT Id, Title, CreatedDate, CompletedDate 
            FROM History 
            WHERE CompletedDate >= @startDate AND CompletedDate < @endDate
            ORDER BY CompletedDate;
        ";

        try
        {
            await using var conn = new SqlConnection(_connStr);
            await conn.OpenAsync().ConfigureAwait(false);

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@startDate", System.Data.SqlDbType.DateTime2).Value = startDate;
            cmd.Parameters.Add("@endDate", System.Data.SqlDbType.DateTime2).Value = endDate;

            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

            var list = new List<HistoryItem>();
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                list.Add(new HistoryItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    CreatedDate = reader.GetDateTime(2),
                    CompletedDate = reader.GetDateTime(3)
                });
            }

            return list;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Database error while retrieving completed tasks: {ex.Message}", ex);
        }
    }
    
    public async Task<List<HistoryItem>> GetAllCompletedTasksAsync()
    {
        const string sql = @"
            SELECT Id, Title, CreatedDate, CompletedDate 
            FROM History 
            ORDER BY CompletedDate DESC;
        ";

        try
        {
            await using var conn = new SqlConnection(_connStr);
            await conn.OpenAsync().ConfigureAwait(false);

            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

            var list = new List<HistoryItem>();
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                list.Add(new HistoryItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    CreatedDate = reader.GetDateTime(2),
                    CompletedDate = reader.GetDateTime(3)
                });
            }

            return list;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Database error while retrieving completed tasks: {ex.Message}", ex);
        }
    }
}