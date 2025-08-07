using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Infrastructure;

namespace SmartTaskerMini.WinFormsApp
{
    public partial class DailyReportForm : Form
    {
        public DailyReportForm()
        {
            InitializeComponent();
        }

        private async void generateButton_Click(object sender, EventArgs e)
        {
            try
            {
                var repo = RepositoryFactory.CreateTaskRepository();
                var reportService = new ReportService(repo);
                var report = await reportService.GenerateDailyReportAsync(datePicker.Value);
                reportTextBox.Text = report;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}