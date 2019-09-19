﻿partial class MainForm
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
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        this.MainMenu = new System.Windows.Forms.MainMenu(this.components);
        this.ProgressBar = new System.Windows.Forms.ProgressBar();
        this.WebsiteLabel = new System.Windows.Forms.Label();
        this.NotifyIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
        this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
        this.VersionLabel = new System.Windows.Forms.Label();
        this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.MainPanel = new System.Windows.Forms.Panel();
        this.NotifyIconContextMenuStrip.SuspendLayout();
        this.SuspendLayout();
        // 
        // ProgressBar
        // 
        this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.ProgressBar.Location = new System.Drawing.Point(0, 625);
        this.ProgressBar.Name = "ProgressBar";
        this.ProgressBar.Size = new System.Drawing.Size(584, 6);
        this.ProgressBar.TabIndex = 0;
        this.ProgressBar.Visible = false;
        // 
        // WebsiteLabel
        // 
        this.WebsiteLabel.BackColor = System.Drawing.Color.DarkGray;
        this.WebsiteLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.WebsiteLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.WebsiteLabel.Font = new System.Drawing.Font("Tahoma", 8F);
        this.WebsiteLabel.ForeColor = System.Drawing.Color.Purple;
        this.WebsiteLabel.Location = new System.Drawing.Point(0, 369);
        this.WebsiteLabel.Name = "WebsiteLabel";
        this.WebsiteLabel.Size = new System.Drawing.Size(624, 14);
        this.WebsiteLabel.TabIndex = 999;
        this.WebsiteLabel.Tag = "http://qurancode.com";
        this.WebsiteLabel.Text = "©2009-2019 Ali Adams      www.qurancode.com";
        this.WebsiteLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        this.WebsiteLabel.Click += new System.EventHandler(this.LinkLabel_Click);
        // 
        // NotifyIconContextMenuStrip
        // 
        this.NotifyIconContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutToolStripMenuItem,
            this.ExitToolStripMenuItem});
        this.NotifyIconContextMenuStrip.Name = "NotifyIconContextMenuStrip";
        this.NotifyIconContextMenuStrip.Size = new System.Drawing.Size(108, 48);
        // 
        // AboutToolStripMenuItem
        // 
        this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
        this.AboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
        this.AboutToolStripMenuItem.Text = "About";
        this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
        // 
        // ExitToolStripMenuItem
        // 
        this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
        this.ExitToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
        this.ExitToolStripMenuItem.Text = "Exit";
        this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
        // 
        // NotifyIcon
        // 
        this.NotifyIcon.ContextMenuStrip = this.NotifyIconContextMenuStrip;
        this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
        this.NotifyIcon.Text = "Numbers";
        this.NotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseClick);
        this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
        // 
        // VersionLabel
        // 
        this.VersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.VersionLabel.BackColor = System.Drawing.Color.DarkGray;
        this.VersionLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.VersionLabel.Font = new System.Drawing.Font("Tahoma", 8F);
        this.VersionLabel.ForeColor = System.Drawing.Color.Purple;
        this.VersionLabel.Location = new System.Drawing.Point(598, 634);
        this.VersionLabel.Name = "VersionLabel";
        this.VersionLabel.Size = new System.Drawing.Size(63, 14);
        this.VersionLabel.TabIndex = 32;
        this.VersionLabel.Tag = "http://heliwave.com/114.txt";
        this.VersionLabel.Text = "7.29.139.8317";
        this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        this.ToolTip.SetToolTip(this.VersionLabel, "114 Amazing Numbers");
        this.VersionLabel.Click += new System.EventHandler(this.LinkLabel_Click);
        // 
        // ToolTip
        // 
        this.ToolTip.AutomaticDelay = 100;
        this.ToolTip.AutoPopDelay = 10000;
        this.ToolTip.InitialDelay = 40;
        this.ToolTip.ReshowDelay = 20;
        // 
        // MainPanel
        // 
        this.MainPanel.BackColor = System.Drawing.SystemColors.Control;
        this.MainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.MainPanel.Location = new System.Drawing.Point(0, 0);
        this.MainPanel.Name = "MainPanel";
        this.MainPanel.Size = new System.Drawing.Size(624, 369);
        this.MainPanel.TabIndex = 0;
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(624, 383);
        this.Controls.Add(this.MainPanel);
        this.Controls.Add(this.VersionLabel);
        this.Controls.Add(this.WebsiteLabel);
        this.Controls.Add(this.ProgressBar);
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.KeyPreview = true;
        this.Menu = this.MainMenu;
        this.Name = "MainForm";
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        this.Text = "Numbers are the DNA that controls the materialization of light to form our Univer" +
"se.";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
        this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
        this.Load += new System.EventHandler(this.MainForm_Load);
        this.Shown += new System.EventHandler(this.MainForm_Shown);
        this.NotifyIconContextMenuStrip.ResumeLayout(false);
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.MainMenu MainMenu;
    private System.Windows.Forms.ProgressBar ProgressBar;
    private System.Windows.Forms.Label WebsiteLabel;
    private System.Windows.Forms.ContextMenuStrip NotifyIconContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
    private System.Windows.Forms.NotifyIcon NotifyIcon;
    private System.Windows.Forms.Label VersionLabel;
    private System.Windows.Forms.ToolTip ToolTip;
    private System.Windows.Forms.Panel MainPanel;
}
