namespace SmartTaskerMini.WinFormsApp
{
    partial class HistoryForm
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView historyGrid;

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
            historyGrid = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)historyGrid).BeginInit();
            SuspendLayout();

            // historyGrid
            historyGrid.AutoGenerateColumns = false;
            historyGrid.Dock = DockStyle.Fill;
            historyGrid.Location = new Point(0, 0);
            historyGrid.ReadOnly = true;
            historyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            historyGrid.Size = new Size(600, 400);
            historyGrid.Columns.Add("Id", "ID");
            historyGrid.Columns.Add("Title", "Title");
            historyGrid.Columns.Add("CreatedDate", "Created");
            historyGrid.Columns.Add("CompletedDate", "Completed");
            historyGrid.Columns["CreatedDate"].Width = 200;
            historyGrid.Columns["CompletedDate"].Width = 200;


            // HistoryForm
            AutoScaleDimensions = new SizeF(7F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 400);
            Controls.Add(historyGrid);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Task History";
            ((System.ComponentModel.ISupportInitialize)historyGrid).EndInit();
            ResumeLayout(false);
        }
    }
}