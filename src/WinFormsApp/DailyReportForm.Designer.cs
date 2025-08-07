namespace SmartTaskerMini.WinFormsApp
{
    partial class DailyReportForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label label1;
        private DateTimePicker datePicker;
        private Button generateButton;
        private RichTextBox reportTextBox;

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
            label1 = new Label();
            datePicker = new DateTimePicker();
            generateButton = new Button();
            reportTextBox = new RichTextBox();
            SuspendLayout();
            
            // label1
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 0;
            label1.Text = "Report Date:";
            
            // datePicker
            datePicker.Format = DateTimePickerFormat.Short;
            datePicker.Location = new Point(100, 12);
            datePicker.Name = "datePicker";
            datePicker.Size = new Size(150, 23);
            datePicker.TabIndex = 1;
            datePicker.Value = DateTime.Today;
            
            // generateButton
            generateButton.Location = new Point(260, 12);
            generateButton.Name = "generateButton";
            generateButton.Size = new Size(120, 23);
            generateButton.TabIndex = 2;
            generateButton.Text = "Generate Report";
            generateButton.UseVisualStyleBackColor = true;
            generateButton.Click += generateButton_Click;
            
            // reportTextBox
            reportTextBox.Font = new Font("Consolas", 9F);
            reportTextBox.Location = new Point(12, 45);
            reportTextBox.Name = "reportTextBox";
            reportTextBox.ReadOnly = true;
            reportTextBox.Size = new Size(560, 400);
            reportTextBox.TabIndex = 3;
            reportTextBox.Text = "";
            
            // DailyReportForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 461);
            Controls.Add(reportTextBox);
            Controls.Add(generateButton);
            Controls.Add(datePicker);
            Controls.Add(label1);
            Name = "DailyReportForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Daily Report";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}