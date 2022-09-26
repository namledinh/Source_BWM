using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using BSS;

namespace BWM.LIB
{
    public partial class SettingsForm : Form
    {
        WriteLogWithoutListenerDelegate WriteLog = Utilities.WriteLog;
        string SettingsFilePath;
        Settings settings = new Settings();

        public SettingsForm(string SettingsFilePath)
        {
            InitializeComponent();

            this.SettingsFilePath = SettingsFilePath;

            Common.CopyObjectPropertyData(Settings.CurrentSettings, settings);
        }
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            int index = 0;
            foreach (var prop in typeof(Settings).GetProperties())
                AddASettingToUI(index++, prop.Name);

            SettingsToUI();
        }
        void AddASettingToUI(int index, string name)
        {
            const int hInterval = 26;

            var controls = panelSettings.Controls.Find("lbl" + name, false);
            if (!controls.Any())
            {
                Label lbl = new Label();
                lbl.AutoSize = true;
                lbl.Location = new System.Drawing.Point(5, 6 + hInterval * index);
                lbl.Name = "lbl" + name;
                lbl.Size = new System.Drawing.Size(93, 13);
                lbl.Text = name;
                panelSettings.Controls.Add(lbl);
            }

            controls = panelSettings.Controls.Find("txt" + name, false);
            if (!controls.Any())
            {
                TextBox txt = new TextBox();
                txt.Location = new System.Drawing.Point(132, 3 + hInterval * index);
                txt.Name = "txt" + name;
                txt.Size = new System.Drawing.Size(300, 20);
                panelSettings.Controls.Add(txt);
            }
        }

        void SettingsToUI()
        {
            foreach (var prop in typeof(Settings).GetProperties())
            {
                var controls = panelSettings.Controls.Find("txt" + prop.Name, false);
                if (controls.Any())
                {
                    TextBox txt = controls[0] as TextBox;
                    txt.Text = prop.GetValue(settings).ToString();
                }
            }
        }

        void UIToSettings()
        {
            foreach (var prop in typeof(Settings).GetProperties())
            {
                var txt = panelSettings.Controls.Find("txt" + prop.Name, false)[0] as TextBox;
                prop.SetValue(settings, GetValue(prop.PropertyType, txt.Text));
            }
        }
        protected object GetValue(Type type, string value)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32: { int.TryParse(value.ToString(), out int val); return val; }
                case TypeCode.String: return value.ToString();
                default: break;
            }
            return "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            UIToSettings();
            var msg = Common.CopyObjectPropertyData(settings, Settings.CurrentSettings);

            msg = Settings.SaveSettingsToFile(SettingsFilePath);
            if (msg.Length > 0)
            {
                MessageBox.Show(this, "Save file error: " + msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            WriteLog("Saved to Setting File [" + SettingsFilePath + "] Ok.");

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnDefault_Click(object sender, EventArgs e)
        {
            settings = new Settings();
            SettingsToUI();
        }

    }
}
