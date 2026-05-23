// Decompiled with JetBrains decompiler
// Type: EAFRenamer.FormMain
// Assembly: EAFRenamer, Version=2.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: FB825B7B-CEB6-4801-BFE2-EC5935950AB3
// Assembly location: C:\AFONSOFT\EAF\Templates\Renamer.exe

using EAFRenamer.Properties;
using Eaf.Renamer.Lib;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Eaf.Renamer.Win
{
    public partial class FormMain : Form
    {
        private readonly RenameTemplate renameTemplate;

        private void BtnStart_Click(object sender, EventArgs e)
        {
            Console.Text = string.Empty;
            if (btnStart.Text == "Execute")
                StartMethod();
            else
                StopMethod();
        }

        private void StartMethod()
        {
            Arguments arguments = new Arguments
            {
                OldCompanyName = txtOldCompanyName.Text.Trim(),
                OldProjectName = txtOldProjectName.Text.Trim(),
                OldAreaName = txtOldAreaName.Text.Trim(),
                NewCompanyName = txtNewCompanyName.Text.Trim(),
                NewAreaName = txtNewAreaName.Text.Trim(),
                NewProjectName = txtNewProjectName.Text.Trim()
            };
            if (string.IsNullOrEmpty(arguments.NewProjectName))
            {
                MessageBox.Show("Please select the project path!", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Question);
                txtNewProjectName.Focus();
            }
            else
            {
                arguments.RootDir = txtRootDir.Text.Trim();
                if (string.IsNullOrWhiteSpace(arguments.RootDir))
                {
                    if (DialogResult.Yes != MessageBox.Show("Please select the project path!", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Question))
                        return;
                    BtnSelect_Click(this, null);
                }
                else if (!Directory.Exists(arguments.RootDir))
                {
                    MessageBox.Show("Please select the correct project path!");
                }
                else if (chk.Checked && string.IsNullOrWhiteSpace(arguments.NewAreaName))
                {
                    MessageBox.Show("Please type new Area name!", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    progressBar1.Value = 0;
                    arguments.ChangeAreaName = chk.Checked;
                    btnStart.Text = "Processing";
                    progressBar1.Visible = true;

                    renameTemplate.DoWork += BackgroundWorker_DoWork;
                    renameTemplate.WorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                    renameTemplate.ProgressChanged += BackgroundWorker_ProgressChanged;
                    renameTemplate.Log += Template_Log;

                    renameTemplate.RunWorker(arguments);
                }
            }
        }

        private void Template_Log(object sender, string e)
        {
            Log(e);
        }

        private void StopMethod()
        {
            renameTemplate.StopWorker();
            MessageBox.Show("Cancelling..");
        }

        private void Log(string value)
        {
            if (Console.InvokeRequired)
                Console.Invoke(new Action<string>(Log), value);
            else
                Console.AppendText(value);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Log("================= Starting =================\r\n");
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Log(e.UserState.ToString());
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            btnStart.Text = "Execute";
            if (e.Cancelled)
            {
                MessageBox.Show("Task terminated");
            }
            else
            {
                if (e.Error != null)
                {
                    MessageBox.Show("Internal error", e.Error.Message);
                    throw e.Error;
                }

                if (DialogResult.Yes != MessageBox.Show("Processing completed successfully. Terminate EAF Renamer？", "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk))
                {
                    return;
                }

                BtnClose_Click(null, new MyEventArgs());
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "Please select the folder where the EAF project is located."
            };
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            if (string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
            {
                MessageBox.Show(this, "Folder path cannot be empty", "Prompt");
            }
            else
                txtRootDir.Text = folderBrowserDialog.SelectedPath;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtFilter.Text))
                Settings.Default.setFilter = txtFilter.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtOldCompanyName.Text))
                Settings.Default.setOldCompanyName = txtOldCompanyName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtOldProjectName.Text))
                Settings.Default.setOldProjectName = txtOldProjectName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtOldAreaName.Text))
                Settings.Default.setOldAreaName = txtOldAreaName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtRootDir.Text))
                Settings.Default.setRootDir = txtRootDir.Text.Trim();
            Settings.Default.setNewCompanyName = txtNewCompanyName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtNewProjectName.Text))
                Settings.Default.setNewProjectName = txtNewProjectName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtNewAreaName.Text))
                Settings.Default.setNewAreaName = txtNewAreaName.Text.Trim();
            if (e is FormMain.MyEventArgs)
            {
                Settings.Default.setOldCompanyName = txtNewCompanyName.Text.Trim();
                Settings.Default.setOldProjectName = txtNewProjectName.Text.Trim();
            }
            Settings.Default.Save();
            Environment.Exit(0);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Settings.Default.setFilter))
                txtFilter.Text = Settings.Default.setFilter.Trim();
            if (!string.IsNullOrWhiteSpace(Settings.Default.setOldCompanyName))
                txtOldCompanyName.Text = Settings.Default.setOldCompanyName.Trim();
            if (!string.IsNullOrWhiteSpace(Settings.Default.setOldProjectName))
                txtOldProjectName.Text = Settings.Default.setOldProjectName.Trim();
            if (!string.IsNullOrWhiteSpace(Settings.Default.setOldAreaName))
                txtOldAreaName.Text = Settings.Default.setOldAreaName.Trim();
            if (!string.IsNullOrWhiteSpace(Settings.Default.setRootDir))
                txtRootDir.Text = Settings.Default.setRootDir.Trim();
            if (!string.IsNullOrWhiteSpace(Settings.Default.setNewCompanyName))
                txtNewCompanyName.Text = Settings.Default.setNewCompanyName.Trim();
            if (!string.IsNullOrWhiteSpace(Settings.Default.setNewProjectName))
                txtNewProjectName.Text = Settings.Default.setNewProjectName.Trim();
            if (string.IsNullOrWhiteSpace(Settings.Default.setNewAreaName))
                return;
            txtNewAreaName.Text = Settings.Default.setNewAreaName.Trim();
        }

        private void BtnReset_Click(object sender, EventArgs e) => txtFilter.Text = ".cs,.cshtml,.js,.ts,.csproj,.sln,.xml,.config,.DotSettings,.json,.xaml,.txt,.html,.gitignore,.ps1,.md,.plist,.tpl";

        private void lbOriginalName_Click(object sender, EventArgs e) => txtOldCompanyName.Text = "Eaf.ProjectName";

        private void lbOriginalProjectName_Click(object sender, EventArgs e) => txtOldProjectName.Text = "eaf-ProjectName-ui";

        private void lbProjectPath_Click(object sender, EventArgs e) => txtRootDir.Text = "";

        private void lbNewCompanyName_Click(object sender, EventArgs e) => txtNewCompanyName.Text = "Eaf.ProjectName";

        private void lbNewProjectName_Click(object sender, EventArgs e) => txtNewProjectName.Text = "eaf-ProjectName-ui";

        private void lbOriginalAreaName_Click(object sender, EventArgs e) => txtOldAreaName.Text = "AppAreaName";

        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            lbOriginalAreaName.Enabled = chk.Checked;
            txtOldAreaName.Enabled = chk.Checked;
            lbArrow3rd.Enabled = chk.Checked;
            lbNewAreaName.Enabled = chk.Checked;
            txtNewAreaName.Enabled = chk.Checked;
        }

        private void btnChangeLog_Click(object sender, EventArgs e)
        {
            Console.Text = string.Empty;
            Log("Change Log\n");
            Log("==========\n\n");
            Log("V.1.04, 21/02/2024, Fix build\n");
            Log("V.1.03, 20/11/2023, Fix DPI layout\n");
            Log("V.1.02, 16/07/2023, Create a Lib for rename for console\n");
            Log("V.1.01, 12/07/2023, UTF8 Without BOM\n");
            Log("V.1.00, 10/07/2023, Initial Version\n");
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            FormMain.ReleaseCapture();
            FormMain.SendMessage(Handle, 274, 61458, 0);
        }

        public class MyEventArgs : EventArgs
        {
            public bool IsCompleted { get; set; } = true;
        }
    }
}