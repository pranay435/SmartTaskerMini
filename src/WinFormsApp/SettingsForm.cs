using SmartTaskerMini.Core.Application;

namespace SmartTaskerMini.WinFormsApp
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            switch (Configuration.StorageType.ToUpper())
            {
                case "SQL":
                    sqlRadio.Checked = true;
                    break;
                case "JSON":
                    jsonRadio.Checked = true;
                    break;
                case "XML":
                    xmlRadio.Checked = true;
                    break;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string storageType = "SQL";
            if (jsonRadio.Checked) storageType = "JSON";
            else if (xmlRadio.Checked) storageType = "XML";

            ConfigurationManager.SetStorageType(storageType);
        }
    }
}