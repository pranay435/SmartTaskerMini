namespace SmartTaskerMini.WinFormsApp
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label label1;
        private RadioButton sqlRadio;
        private RadioButton jsonRadio;
        private RadioButton xmlRadio;
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
            sqlRadio = new RadioButton();
            jsonRadio = new RadioButton();
            xmlRadio = new RadioButton();
            okButton = new Button();
            cancelButton = new Button();
            SuspendLayout();
            
            // label1
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(78, 15);
            label1.TabIndex = 0;
            label1.Text = "Storage Type:";
            
            // sqlRadio
            sqlRadio.AutoSize = true;
            sqlRadio.Location = new Point(20, 45);
            sqlRadio.Name = "sqlRadio";
            sqlRadio.Size = new Size(82, 19);
            sqlRadio.TabIndex = 1;
            sqlRadio.TabStop = true;
            sqlRadio.Text = "SQL Server";
            sqlRadio.UseVisualStyleBackColor = true;
            
            // jsonRadio
            jsonRadio.AutoSize = true;
            jsonRadio.Location = new Point(20, 70);
            jsonRadio.Name = "jsonRadio";
            jsonRadio.Size = new Size(73, 19);
            jsonRadio.TabIndex = 2;
            jsonRadio.TabStop = true;
            jsonRadio.Text = "JSON File";
            jsonRadio.UseVisualStyleBackColor = true;
            
            // xmlRadio
            xmlRadio.AutoSize = true;
            xmlRadio.Location = new Point(20, 95);
            xmlRadio.Name = "xmlRadio";
            xmlRadio.Size = new Size(68, 19);
            xmlRadio.TabIndex = 3;
            xmlRadio.TabStop = true;
            xmlRadio.Text = "XML File";
            xmlRadio.UseVisualStyleBackColor = true;
            
            // okButton
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(180, 130);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 4;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            
            // cancelButton
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(260, 130);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            
            // SettingsForm
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(350, 170);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(xmlRadio);
            Controls.Add(jsonRadio);
            Controls.Add(sqlRadio);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}