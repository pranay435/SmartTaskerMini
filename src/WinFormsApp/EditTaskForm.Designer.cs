namespace SmartTaskerMini.WinFormsApp
{
    partial class EditTaskForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label label1;
        private TextBox titleTextBox;
        private Label label2;
        private DateTimePicker dueDatePicker;
        private Label label3;
        private NumericUpDown priorityUpDown;
        private Button okButton;
        private Button cancelButton;

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
            titleTextBox = new TextBox();
            label2 = new Label();
            dueDatePicker = new DateTimePicker();
            label3 = new Label();
            priorityUpDown = new NumericUpDown();
            okButton = new Button();
            cancelButton = new Button();
            ((System.ComponentModel.ISupportInitialize)priorityUpDown).BeginInit();
            SuspendLayout();
            
            // label1
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(32, 15);
            label1.TabIndex = 0;
            label1.Text = "Title:";
            
            // titleTextBox
            titleTextBox.Location = new Point(80, 12);
            titleTextBox.Name = "titleTextBox";
            titleTextBox.Size = new Size(300, 23);
            titleTextBox.TabIndex = 1;
            
            // label2
            label2.AutoSize = true;
            label2.Location = new Point(12, 44);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 2;
            label2.Text = "Due Date:";
            
            // dueDatePicker
            dueDatePicker.CustomFormat = "yyyy-MM-dd HH:mm";
            dueDatePicker.Format = DateTimePickerFormat.Custom;
            dueDatePicker.Location = new Point(80, 41);
            dueDatePicker.Name = "dueDatePicker";
            dueDatePicker.Size = new Size(200, 23);
            dueDatePicker.TabIndex = 3;
            
            // label3
            label3.AutoSize = true;
            label3.Location = new Point(12, 73);
            label3.Name = "label3";
            label3.Size = new Size(48, 15);
            label3.TabIndex = 4;
            label3.Text = "Priority:";
            
            // priorityUpDown
            priorityUpDown.Location = new Point(80, 70);
            priorityUpDown.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            priorityUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            priorityUpDown.Name = "priorityUpDown";
            priorityUpDown.Size = new Size(60, 23);
            priorityUpDown.TabIndex = 5;
            
            // okButton
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(225, 110);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 6;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            
            // cancelButton
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(305, 110);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 7;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            
            // EditTaskForm
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(400, 150);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(priorityUpDown);
            Controls.Add(label3);
            Controls.Add(dueDatePicker);
            Controls.Add(label2);
            Controls.Add(titleTextBox);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "EditTaskForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Edit Task";
            ((System.ComponentModel.ISupportInitialize)priorityUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}