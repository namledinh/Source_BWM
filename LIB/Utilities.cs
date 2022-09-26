using BSS;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Deployment.Application;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace BWM.LIB
{
    public delegate void WriteLogWithoutListenerDelegate(string s);
    public delegate void WriteLogWithListenerDelegate(string s, string listener);


    public static class Utilities
    {
        public static WriteLogWithoutListenerDelegate WriteLog = null;
        public static string LogRootPath = "";
        public static string SettingsPath = "";
        public static string SettingsFilePath = "";

        public static LogWriter LogWriter = null;

        private const string EncryptionKey = "jE6PPoK941IWX0QUm708";
        private static byte[] Salt = new byte[] { 78, 103, 63, 105, 32, 63, 32, 116, 114, 63, 32, 45, 32, 110, 103, 63, 105, 32, 99, 104, 63, 110, 103, 32, 109, 105, 110, 104 };


        public static string Init(WriteLogWithoutListenerDelegate writeLog, string RootPath)
        {
            string msg = "";

            try
            {
                WriteLog = writeLog;
                LogRootPath = Path.Combine(RootPath, "Log");

                msg = GetLogWriter();
                if (msg.Length > 0) return msg;

                msg = AddShortcutToStartupGroup();
                if (msg.Length > 0) return msg;

                msg = CreateApplicationShortcut_FirstRunOnly();
                if (msg.Length > 0) return msg;

                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                SettingsPath = Path.Combine(localAppData, "Bkav Corporation\\" + Utilities.AssemblyTitle);

                if (!Directory.Exists(SettingsPath)) Directory.CreateDirectory(SettingsPath);
                SettingsFilePath = Path.Combine(SettingsPath, AssemblyTitle + ".dat");
            }
            catch (Exception ex)
            {
                msg = "Init error: " + ex.ToString();
            }
            return msg;
        }

        public static string InitDirectory(string path, string[] SubFolders = null)
        {
            string msg = "";

            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                if (SubFolders == null) return "";
                foreach (string sub in SubFolders)
                {
                    var subPath = path + "\\" + sub;
                    if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);
                }
            }
            catch (Exception ex)
            {
                return $"InitDirectory error: RootPath: {path} - Exception: {ex}";
            }
            return msg;
        }

        static public string InfoMessage = "[INFO] ";
        public static string ToInfoMessage(this string msg)
        {
            return InfoMessage + msg;
        }
        static public bool IsInfoMessage(this string msg)
        {
            return msg.StartsWith(InfoMessage);
        }

        public static DateTime ToDatetime(this string datestr, string culture, DateTime defaultValue)
        {
            var msg = Convertor.StringToDatetime(datestr, culture, out DateTime datetime);
            if (msg.Length > 0) return defaultValue;
            return datetime;
        }

        public static string GetLogWriter()
        {
            string msg = "";

            try
            {
                msg = SetTraceLogPath(LogRootPath);
                if (msg.Length > 0) return msg;

                LogWriter = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();

                WriteLog("Init LogWriter Ok. Log will be saved to folder " + LogRootPath);
            }
            catch (Exception ex)
            {
                msg = "Error: Failed to init LogWriter. No log will be saved to log file: " + ex.ToString();
            }
            return msg;
        }
        public static string SetTraceLogPath(string LogRootPath)
        {
            string msg;
            try
            {
                msg = GetLoggingSettings(out LoggingSettings loggingSettings, out Configuration entLibConfig);
                if (msg.Length > 0) return msg;

                foreach (var traceListenerData in loggingSettings.TraceListeners)
                {
                    RollingFlatFileTraceListenerData data = traceListenerData as RollingFlatFileTraceListenerData;
                    data.FileName = LogRootPath + (LogRootPath.EndsWith("\\") ? "" : "\\") + Path.GetFileName(data.FileName);
                }

                entLibConfig.Save();
            }
            catch (Exception ex)
            {
                msg = "SetTraceLogPath error: " + ex.ToString();
            }
            return msg;
        }
        public static string AddLoggingSettings(string workerName)
        {
            string msg;
            try
            {
                msg = GetLoggingSettings(out LoggingSettings loggingSettings, out Configuration entLibConfig);
                if (msg.Length > 0) return msg;
                if (loggingSettings.TraceListeners.Count == 0) return "loggingSettings.TraceListeners.Count == 0";

                var workerLogFilename = "BWM-" + workerName + ".log";

                foreach (var traceListenerData in loggingSettings.TraceListeners)
                {
                    RollingFlatFileTraceListenerData data = traceListenerData as RollingFlatFileTraceListenerData;

                    var FileName = Path.GetFileName(data.FileName);
                    if (string.Compare(FileName, workerLogFilename, true) == 0) return ""; // Nếu đã tồn tại listeners thì return luôn
                }

                RollingFlatFileTraceListenerData rff = new RollingFlatFileTraceListenerData(workerName + " Listener", workerLogFilename, "", "", 50000, "dd-MM-yy", RollFileExistsBehavior.Overwrite, RollInterval.None, System.Diagnostics.TraceOptions.None, "Text Formatter");
                loggingSettings.TraceListeners.Add(rff);

                TraceSourceData tsd = new TraceSourceData(workerName, System.Diagnostics.SourceLevels.All);
                TraceListenerReferenceData tlrd = new TraceListenerReferenceData(workerName + " Listener");
                tsd.TraceListeners.Add(tlrd);
                loggingSettings.TraceSources.Add(tsd);

                entLibConfig.Save();
            }
            catch (Exception ex)
            {
                msg = "SetTraceLogPath error: " + ex.ToString();
            }
            return msg;
        }
        public static string GetLoggingSettings(out LoggingSettings loggingSettings, out Configuration entLibConfig)
        {
            loggingSettings = null;
            entLibConfig = null;
            try
            {
                ConfigurationFileMap objConfigPath = new ConfigurationFileMap();

                // App config file path.
                string appPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                objConfigPath.MachineConfigFilename = appPath;

                entLibConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                loggingSettings = entLibConfig.GetSection(LoggingSettings.SectionName) as LoggingSettings;
                if (loggingSettings == null) return "loggingSettings is null";
            }
            catch (Exception ex)
            {
                return "GetLoggingSettings error: " + ex.ToString();
            }
            return "";
        }

        public delegate void WriteLogToTextBoxDelegate(TextBox tb, TextBox tbErrors, string s, bool IsAutoScroll, string listener = "ALL");
        public const string LOG_ERROR = "@ERROR@";
        const int LOG_MAXLENGTH = 40000;
        static public long NumOfErrorLogs = 0;
        public static void WriteLogToTextBox(TextBox tb, TextBox tbErrors, string log, bool IsAutoScroll, string listener = "ALL")
        {
            if (tb.InvokeRequired)
            {
                try { tb.Invoke(new WriteLogToTextBoxDelegate(WriteLogToTextBox), tb, tbErrors, log, IsAutoScroll, listener); }
                catch { };

                return;
            }

            bool isErrorLog = false;
            if (log.StartsWith(LOG_ERROR))
            {
                isErrorLog = true;
                NumOfErrorLogs++;

                log = log.Substring(LOG_ERROR.Length);

                if (tbErrors != null) AppendTextToTextBox(tbErrors, DateTime.Now.ToString("dd/MM HH:mm:ss") + ": [" + listener + "]: " + log, IsAutoScroll); // Log error thi hien thi o tab Status Error
            }

            if (tb.Text.Length > LOG_MAXLENGTH) tb.Text = tb.Text.Substring(0, tb.Text.Length / 2);

            var text = DateTime.Now.ToString("dd/MM HH:mm:ss") + ": " + log;
            if (listener == "NoRN") text = log;
            AppendTextToTextBox(tb, text, IsAutoScroll);

            if (LogWriter != null && LogWriter.IsLoggingEnabled()) // write log to log file
            {
                if (listener != "NoRN") LogWriter.Write(log, listener);

                if (isErrorLog) LogWriter.Write("[" + listener + "]: " + log, "Errors"); // write error log to General Catergory 
            }
        }
        private const int SB_VERT = 0x1;
        private const int WM_VSCROLL = 0x115;
        private const int SB_THUMBPOSITION = 0x4;
        private const int SB_BOTTOM = 0x7;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);
        [DllImport("user32.dll")]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        [DllImport("user32.dll")]
        private static extern bool PostMessageA(IntPtr hWnd, int nBar, int wParam, int lParam);
        static void AppendTextToTextBox(TextBox textbox, string text, bool autoscroll)
        {
            int savedVpos = GetScrollPos(textbox.Handle, SB_VERT);
            textbox.AppendText(text + Environment.NewLine);
            if (autoscroll)
            {
                PostMessageA(textbox.Handle, WM_VSCROLL, SB_BOTTOM, 0);
            }
            else
            {
                SetScrollPos(textbox.Handle, SB_VERT, savedVpos, true);
                PostMessageA(textbox.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * savedVpos, 0);
            }
        }

        /// <summary>
        /// Gets the assembly title.
        /// </summary>
        /// <value>The assembly title.</value>
        public static string AssemblyTitle
        {
            get
            {
                // Get all Title attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AddShortcutToStartupGroup()
        {
            try
            {
                if (!ApplicationDeployment.IsNetworkDeployed || !ApplicationDeployment.CurrentDeployment.IsFirstRun) return "";

                string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                startupPath = Path.Combine(startupPath, Utilities.AssemblyTitle) + ".appref-ms";

                if (!File.Exists(startupPath))
                {
                    string shortcutPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "\\Bkav Corporation\\" + Utilities.AssemblyTitle + ".appref-ms");
                    //MessageBox.Show("startupPath: " + startupPath + "; shortcutPath: " + shortcutPath);

                    if (File.Exists(shortcutPath)) File.Copy(shortcutPath, startupPath);
                }
            }
            catch (Exception ex)
            {
                return "AddShortcutToStartupGroup error: " + ex.ToString();
            }
            return "";
        }

        static public string CreateApplicationShortcut_FirstRunOnly()
        {
            try
            {
                if (!ApplicationDeployment.IsNetworkDeployed || !ApplicationDeployment.CurrentDeployment.IsFirstRun) return "";

                CreateApplicationShortcut();
            }
            catch (Exception ex)
            {
                return "CreateApplicationShortcut_FirstRunOnly error: " + ex.ToString();
            }
            return "";
        }
        static public string CreateApplicationShortcut()
        {
            try
            {
                string appName = Utilities.AssemblyTitle;
                string desktopPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "\\" + appName + ".appref-ms");
                string shortcutName = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "\\Bkav Corporation\\" + appName + ".appref-ms");

                System.IO.File.Copy(shortcutName, desktopPath, true);
            }
            catch (Exception ex)
            {
                return "CreateApplicationShortcut error: " + ex.ToString();
            }
            return "";
        }

        #region AutoUpdate ClickOnce
        static AutoUpdate autoUpdate = new AutoUpdate();
        static bool isAddedCheckForUpdate = false;
        static bool UpdadeReady = false;
        static bool isAsking = false;
        static public void UpdateApplication()
        {
            try
            {
                if (isAsking)
                {
                    WriteLog("I am asking you and waiting for your answer... Please answer the question before continuing checking for Update !");
                    return;
                }
                if (UpdadeReady)
                {
                    DialogResult dr = MessageBox.Show("The application has been updated. Restart? (If you do not restart now, the new version will not take effect until after you quit and launch the application again)", "Restart Application", MessageBoxButtons.OKCancel);

                    if (DialogResult.OK == dr)
                    {
                        WriteLog("The application has been updated. Restarting...");
                        Application.Restart();
                    }
                    return;
                }

                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    //hasNewUpdate = false;
                    ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                    if (!isAddedCheckForUpdate)
                    {
                        isAddedCheckForUpdate = true;
                        ad.CheckForUpdateCompleted += new CheckForUpdateCompletedEventHandler(ad_CheckForUpdateCompleted);
                        ad.CheckForUpdateProgressChanged += new DeploymentProgressChangedEventHandler(ad_CheckForUpdateProgressChanged);
                    }

                    autoUpdate.SetStatus("Checking...", 0, 100);
                    //autoUpdate.Show();
                    ad.CheckForUpdateAsync();
                }
                else
                {
                    WriteLog("No new update.");
                }
            }
            catch (Exception ex)
            { WriteLog("Error in UpdateApplication: " + ex.ToString()); }
        }
        static void ad_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            int curr = (int)(e.BytesCompleted / 1024);
            int max = (int)(e.BytesTotal / 1024);
            autoUpdate.SetStatus(String.Format("Downloading: {0}. {1:D}K of {2:D}K downloaded.", GetProgressString(e.State), curr, max), curr, max);
        }
        static string GetProgressString(DeploymentProgressState state)
        {
            if (state == DeploymentProgressState.DownloadingApplicationFiles)
            {
                return "application files";
            }
            else if (state == DeploymentProgressState.DownloadingApplicationInformation)
            {
                return "application manifest";
            }
            else
            {
                return "deployment manifest";
            }
        }
        static void ad_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    //MessageBox.Show("ERROR: Could not retrieve new version of the application. Reason: \n\n" + e.Error.Message + "\n\nPlease report this error to the system administrator.");
                    WriteLog("ERROR: Could not retrieve new version of the application. Reason: \n\n" + e.Error.Message + "\n\nPlease report this error to the system administrator.");
                    autoUpdate.Hide();
                    return;
                }
                else if (e.Cancelled == true)
                {
                    //MessageBox.Show("The update was cancelled.");
                    WriteLog("The update was cancelled.");
                    autoUpdate.Hide();
                }

                // Ask the user if they would like to update the application now. 
                if (e.UpdateAvailable)
                {
                    WriteLog("Has new update.");

                    //BeginUpdate();
                    var sizeOfUpdate = e.UpdateSizeBytes;
                    if (!e.IsUpdateRequired)
                    {
                        isAsking = true;
                        DialogResult dr = MessageBox.Show("An update is available (size: " + string.Format("{0:n0}", sizeOfUpdate) + " bytes). Would you like to update the application now?", "Update Available", MessageBoxButtons.OKCancel);
                        isAsking = false;
                        if (DialogResult.OK == dr)
                        {
                            BeginUpdate();
                        }
                    }
                    else
                    {
                        MessageBox.Show("A mandatory update is available for your application. We will install the update now, after which we will save all of your in-progress data and restart your application.");
                        BeginUpdate();
                    }
                }
                else
                {
                    WriteLog("No new update.");
                    //MessageBox.Show(Utilities.AssemblyTitle + " is up to date.");
                    autoUpdate.Hide();
                }
            }
            catch (Exception ex)
            { WriteLog("Error in ad_CheckForUpdateCompleted: " + ex.ToString()); }
        }

        static bool isStarted = false;
        static bool isAddedUpdate = false;
        static private void BeginUpdate()
        {
            try
            {
                if (isStarted) return;
                isStarted = true;

                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                if (!isAddedUpdate)
                {
                    isAddedUpdate = true;
                    ad.UpdateCompleted += new AsyncCompletedEventHandler(ad_UpdateCompleted);
                    ad.UpdateProgressChanged += new DeploymentProgressChangedEventHandler(ad_UpdateProgressChanged);
                }

                ad.UpdateAsync();
            }
            catch (Exception ex)
            { WriteLog("Error in BeginUpdate: " + ex.ToString()); }
        }
        static void ad_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            int curr = (int)(e.BytesCompleted / 1024);
            int max = (int)(e.BytesTotal / 1024);

            String progressText = String.Format("{0:D}K out of {1:D}K downloaded - {2:D}% complete", e.BytesCompleted / 1024, e.BytesTotal / 1024, e.ProgressPercentage);
            autoUpdate.SetStatus(progressText, curr, max);
        }
        static void ad_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    //MessageBox.Show("The update of the application's latest version was cancelled.");
                    WriteLog("The update of the application's latest version was cancelled.");
                    autoUpdate.Hide();
                    return;
                }
                else if (e.Error != null)
                {
                    //MessageBox.Show("ERROR: Could not install the latest version of the application. Reason: \n" + e.Error.Message + "\nPlease report this error to the system administrator.");
                    WriteLog("ERROR: Could not install the latest version of the application. Reason: \n" + e.Error.Message + "\nPlease report this error to the system administrator.");
                    autoUpdate.Hide();
                    return;
                }

                //DialogResult dr = MessageBox.Show("The application has been updated to new version. Do you want to restart? \r\n\r\n(If you do not restart now, the new version will not take effect until after you quit and launch the application again)", "Restart Application", MessageBoxButtons.OKCancel);

                UpdadeReady = true;
                autoUpdate.Hide();

                //if (DialogResult.OK == dr)
                //{
                WriteLog("The application has been updated to new version. Restarting...");
                Application.Restart();
                //}
            }
            catch (Exception ex)
            { WriteLog("Error in ad_UpdateCompleted: " + ex.ToString()); }
        }
        #endregion

        static public string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        static public string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        static public string ObjectToJson(object obj, out string value)
        {
            value = "";
            try
            {
                value = JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }
        static public string JsonToObject<T>(object data, out T obj) where T : class
        {
            obj = default(T);

            try
            {
                obj = JsonConvert.DeserializeObject<T>(Convert.ToString(data));
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }
        public static string CheckRunningTime(string settingtime, out bool isRunningTime)
        {
            isRunningTime = true;
            //rỗng nghĩa là chạy bất cứ lúc nào
            if (String.IsNullOrEmpty(settingtime)) return "";
            string hourMinute = DateTime.Now.ToString("HH:mm");
            string msg = "";
            try
            {
                string[] times = settingtime.Split(',');
                foreach (var time in times)
                {
                    string[] fromTo = time.Split('-');
                    if (hourMinute.CompareTo(fromTo[0]) >= 0 && hourMinute.CompareTo(fromTo[1]) <= 0) { return msg; };
                }
            }
            catch (Exception)
            {
                msg = $"Config giờ chạy {Settings.CurrentSettings.RunningTimes} chưa đúng định dạng HH:mm-HH:mm,HH:mm-HH:mm,...";
            }
            isRunningTime = false;
            return msg;
        }
    }
}
