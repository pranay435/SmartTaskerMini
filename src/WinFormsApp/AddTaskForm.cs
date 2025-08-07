using SmartTaskerMini.Core.Application;

namespace SmartTaskerMini.WinFormsApp
{
    public partial class AddTaskForm : Form
    {
        private TaskService _taskService;

        public AddTaskForm(TaskService taskService)
        {
            _taskService = taskService;
            InitializeComponent();
        }

        private async void okButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleTextBox.Text))
            {
                MessageBox.Show("Title is required.");
                return;
            }

            if (dueDatePicker.Value < DateTime.Now)
            {
                MessageBox.Show("Due date cannot be in the past.");
                return;
            }

            try
            {
                var result = await _taskService.AddAsync(titleTextBox.Text, dueDatePicker.Value, (int)priorityUpDown.Value);
                if (result.Contains("successfully"))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding task: {ex.Message}");
            }
        }
    }
}