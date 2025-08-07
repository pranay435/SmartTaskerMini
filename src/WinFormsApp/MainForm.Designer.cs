namespace SmartTaskerMini.WinFormsApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private ToolStrip toolStrip1;
        private ToolStripButton addTaskButton;
        private ToolStripButton editTaskButton;
        private ToolStripButton markDoneButton;
        private ToolStripButton deleteTaskButton;
        private ToolStripButton historyButton;
        private ToolStripButton dailyReportButton;
        private ToolStripButton settingsButton;
        private DataGridView tasksGrid;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            toolStrip1 = new ToolStrip();
            addTaskButton = new ToolStripButton();
            editTaskButton = new ToolStripButton();
            markDoneButton = new ToolStripButton();
            deleteTaskButton = new ToolStripButton();
            historyButton = new ToolStripButton();
            dailyReportButton = new ToolStripButton();
            settingsButton = new ToolStripButton();
            tasksGrid = new DataGridView();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tasksGrid).BeginInit();
            SuspendLayout();
            
            // toolStrip1
            toolStrip1.Items.AddRange(new ToolStripItem[] {
                addTaskButton,
                editTaskButton,
                markDoneButton,
                deleteTaskButton,
                historyButton,
                dailyReportButton,
                settingsButton});
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            
            // addTaskButton
            addTaskButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            addTaskButton.Name = "addTaskButton";
            addTaskButton.Size = new Size(60, 22);
            addTaskButton.Text = "Add Task";
            addTaskButton.Click += AddTask_Click;
            
            // editTaskButton
            editTaskButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            editTaskButton.Name = "editTaskButton";
            editTaskButton.Size = new Size(59, 22);
            editTaskButton.Text = "Edit Task";
            editTaskButton.Click += EditTask_Click;
            
            // markDoneButton
            markDoneButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            markDoneButton.Name = "markDoneButton";
            markDoneButton.Size = new Size(70, 22);
            markDoneButton.Text = "Mark Done";
            markDoneButton.Click += MarkDone_Click;
            
            // deleteTaskButton
            deleteTaskButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            deleteTaskButton.Name = "deleteTaskButton";
            deleteTaskButton.Size = new Size(73, 22);
            deleteTaskButton.Text = "Delete Task";
            deleteTaskButton.Click += DeleteTask_Click;
            
            // historyButton
            historyButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            historyButton.Name = "historyButton";
            historyButton.Size = new Size(50, 22);
            historyButton.Text = "History";
            historyButton.Click += History_Click;
            
            // dailyReportButton
            dailyReportButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            dailyReportButton.Name = "dailyReportButton";
            dailyReportButton.Size = new Size(78, 22);
            dailyReportButton.Text = "Daily Report";
            dailyReportButton.Click += DailyReport_Click;
            
            // settingsButton
            settingsButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(53, 22);
            settingsButton.Text = "Settings";
            settingsButton.Click += Settings_Click;
            
            // tasksGrid
            tasksGrid.AutoGenerateColumns = false;
            tasksGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tasksGrid.Dock = DockStyle.Fill;
            tasksGrid.Location = new Point(0, 25);
            tasksGrid.MultiSelect = false;
            tasksGrid.Name = "tasksGrid";
            tasksGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tasksGrid.Size = new Size(800, 425);
            tasksGrid.TabIndex = 1;
            tasksGrid.Columns.Add("Id", "ID");
            tasksGrid.Columns.Add("Title", "Title");
            tasksGrid.Columns.Add("DueUtc", "Due Date");
            tasksGrid.Columns.Add("Priority", "Priority");
            tasksGrid.Columns.Add("Status", "Status");
            tasksGrid.Columns.Add("Score", "Score");
            tasksGrid.Columns["DueUtc"].Width = 200;
            tasksGrid.Columns["Title"].Width = 220;
            
            // MainForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tasksGrid);
            Controls.Add(toolStrip1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SmartTasker Mini";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tasksGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}