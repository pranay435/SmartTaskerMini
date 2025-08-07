using SmartTaskerMini.Core.Application;

namespace SmartTaskerMini.WinFormsApp
{
    public partial class HistoryForm : Form
    {
        private TaskService _taskService;

        public HistoryForm(TaskService taskService)
        {
            _taskService = taskService;
            InitializeComponent();
            LoadHistory();
        }

        private async void LoadHistory()
        {
            var history = await _taskService.GetCompletedTasksAsync();
            historyGrid.Rows.Clear();

            foreach (var item in history)
            {
                historyGrid.Rows.Add(item.Id, item.Title,
                                     item.CreatedDate.ToString("yyyy-MM-dd HH:mm"),
                                     item.CompletedDate.ToString("yyyy-MM-dd HH:mm"));
            }
        }

    }
}