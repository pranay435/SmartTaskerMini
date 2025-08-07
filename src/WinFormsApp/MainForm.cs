using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Infrastructure;

namespace SmartTaskerMini.WinFormsApp
{
    public partial class MainForm : Form
    {
        private TaskService _taskService;

        public MainForm()
        {
            InitializeComponent();
            _taskService = new TaskService(RepositoryFactory.CreateTaskRepository());
            _ = LoadTasks();
        }

        public async Task RefreshTasks()
        {
            await LoadTasks();
        }

        private async Task LoadTasks()
        {
            try
            {
                var tasks = await _taskService.ListAsync();
                tasksGrid.Rows.Clear();
                tasksGrid.Refresh();
                
                foreach (var task in tasks)
                {
                    tasksGrid.Rows.Add(task.Id, task.Title, task.DueUtc.ToString("yyyy-MM-dd HH:mm"), 
                                       task.Priority, task.Status, task.Score);
                }
                tasksGrid.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void AddTask_Click(object sender, EventArgs e)
        {
            var form = new AddTaskForm(_taskService);
            if (form.ShowDialog() == DialogResult.OK)
                await LoadTasks();
        }

        private async void EditTask_Click(object sender, EventArgs e)
        {
            if (tasksGrid.SelectedRows.Count > 0)
            {
                var taskId = (int)tasksGrid.SelectedRows[0].Cells[0].Value;
                var form = new EditTaskForm(_taskService, taskId, this);
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    await LoadTasks();
                }
            }
        }

        private async void MarkDone_Click(object sender, EventArgs e)
        {
            if (tasksGrid.SelectedRows.Count > 0)
            {
                var taskId = (int)tasksGrid.SelectedRows[0].Cells[0].Value;
                await _taskService.MarkDoneAsync(taskId);
                await LoadTasks();
            }
        }

        private async void DeleteTask_Click(object sender, EventArgs e)
        {
            if (tasksGrid.SelectedRows.Count > 0)
            {
                var taskId = (int)tasksGrid.SelectedRows[0].Cells[0].Value;
                if (MessageBox.Show("Delete this task?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    await _taskService.DeleteTaskAsync(taskId);
                    await LoadTasks();
                }
            }
        }

        private void History_Click(object sender, EventArgs e)
        {
            var form = new HistoryForm(_taskService);
            form.ShowDialog();
        }

        private void DailyReport_Click(object sender, EventArgs e)
        {
            var form = new DailyReportForm();
            form.ShowDialog();
        }

        private async void Settings_Click(object sender, EventArgs e)
        {
            var form = new SettingsForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                Text = $"SmartTasker Mini - {Configuration.StorageType}";
                _taskService = new TaskService(RepositoryFactory.CreateTaskRepository());
                await LoadTasks();
            }
        }
    }
}