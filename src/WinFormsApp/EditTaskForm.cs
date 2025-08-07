using SmartTaskerMini.Core.Application;

namespace SmartTaskerMini.WinFormsApp
{
    public partial class EditTaskForm : Form
    {
        private TaskService _taskService;
        private int _taskId;
        private MainForm _mainForm;

        public EditTaskForm(TaskService taskService, int taskId, MainForm mainForm = null)
        {
            _taskService = taskService;
            _taskId = taskId;
            _mainForm = mainForm;
            InitializeComponent();
            LoadTask();
        }

        private async void LoadTask()
        {
            var task = await _taskService.GetTaskByIdAsync(_taskId);
            if (task != null)
            {
                titleTextBox.Text = task.Title;
                dueDatePicker.Value = task.DueUtc;
                priorityUpDown.Value = task.Priority;
            }
        }

        private async void okButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleTextBox.Text))
            {
                MessageBox.Show("Title is required.");
                return;
            }

            try
            {
                var task = await _taskService.GetTaskByIdAsync(_taskId);
                if (task != null)
                {
                    task.Title = titleTextBox.Text;
                    task.DueUtc = dueDatePicker.Value;
                    task.Priority = (int)priorityUpDown.Value;
                    
                    var result = await _taskService.UpdateTaskAsync(task);
                    if (result.Contains("successfully"))
                    {
                        if (_mainForm != null)
                            await _mainForm.RefreshTasks();
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show(result);
                    }
                }
                else
                {
                    MessageBox.Show("Task not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating task: {ex.Message}");
            }
        }
    }
}