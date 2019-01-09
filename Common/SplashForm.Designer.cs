partial class SplashForm
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
        this.InformationLabel = new System.Windows.Forms.Label();
        this.ProgressBar = new System.Windows.Forms.ProgressBar();
        this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.BismAllahLabel = new System.Windows.Forms.Label();
        this.SsalawaaaaaaaaaaaaaatLabel = new System.Windows.Forms.Label();
        this.WordFrequencyLabel = new System.Windows.Forms.Label();
        this.ProductNameLabel = new System.Windows.Forms.Label();
        this.VersionLabel = new System.Windows.Forms.Label();
        this.InitialLettersLabel = new System.Windows.Forms.Label();
        this.CloseLabel = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // InformationLabel
        // 
        this.InformationLabel.BackColor = System.Drawing.Color.MidnightBlue;
        this.InformationLabel.Dock = System.Windows.Forms.DockStyle.Top;
        this.InformationLabel.Font = new System.Drawing.Font("Arial", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.InformationLabel.ForeColor = System.Drawing.Color.Lavender;
        this.InformationLabel.Location = new System.Drawing.Point(0, 132);
        this.InformationLabel.Name = "InformationLabel";
        this.InformationLabel.Size = new System.Drawing.Size(239, 15);
        this.InformationLabel.TabIndex = 7;
        this.InformationLabel.Tag = "http://heliwave.com";
        this.InformationLabel.Text = "help yourself by helping others ... be a guiding light";
        this.InformationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.InformationLabel.Click += new System.EventHandler(this.LinkLabel_Click);
        // 
        // ProgressBar
        // 
        this.ProgressBar.Dock = System.Windows.Forms.DockStyle.Top;
        this.ProgressBar.Location = new System.Drawing.Point(0, 128);
        this.ProgressBar.Name = "ProgressBar";
        this.ProgressBar.Size = new System.Drawing.Size(239, 4);
        this.ProgressBar.TabIndex = 6;
        this.ProgressBar.Value = 100;
        // 
        // ToolTip
        // 
        this.ToolTip.AutoPopDelay = 50000;
        this.ToolTip.InitialDelay = 500;
        this.ToolTip.ReshowDelay = 100;
        // 
        // BismAllahLabel
        // 
        this.BismAllahLabel.BackColor = System.Drawing.Color.Transparent;
        this.BismAllahLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.BismAllahLabel.Dock = System.Windows.Forms.DockStyle.Top;
        this.BismAllahLabel.Font = new System.Drawing.Font("Times New Roman", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.BismAllahLabel.ForeColor = System.Drawing.Color.Lavender;
        this.BismAllahLabel.Location = new System.Drawing.Point(0, 0);
        this.BismAllahLabel.Name = "BismAllahLabel";
        this.BismAllahLabel.Size = new System.Drawing.Size(239, 25);
        this.BismAllahLabel.TabIndex = 0;
        this.BismAllahLabel.Tag = "http://qurancode.com";
        this.BismAllahLabel.Text = "بِسْمِ ٱللَّهِ ٱلرَّحْمَٰنِ ٱلرَّحِيمِ";
        this.BismAllahLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
        this.ToolTip.SetToolTip(this.BismAllahLabel, "In the name of God, the Merciful everywhere, the Merciful everywhen.");
        this.BismAllahLabel.Click += new System.EventHandler(this.LinkLabel_Click);
        // 
        // SsalawaaaaaaaaaaaaaatLabel
        // 
        this.SsalawaaaaaaaaaaaaaatLabel.BackColor = System.Drawing.Color.Transparent;
        this.SsalawaaaaaaaaaaaaaatLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.SsalawaaaaaaaaaaaaaatLabel.Dock = System.Windows.Forms.DockStyle.Top;
        this.SsalawaaaaaaaaaaaaaatLabel.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.SsalawaaaaaaaaaaaaaatLabel.ForeColor = System.Drawing.Color.Lavender;
        this.SsalawaaaaaaaaaaaaaatLabel.Location = new System.Drawing.Point(0, 25);
        this.SsalawaaaaaaaaaaaaaatLabel.Name = "SsalawaaaaaaaaaaaaaatLabel";
        this.SsalawaaaaaaaaaaaaaatLabel.Size = new System.Drawing.Size(239, 19);
        this.SsalawaaaaaaaaaaaaaatLabel.TabIndex = 1;
        this.SsalawaaaaaaaaaaaaaatLabel.Tag = "http://qurancode.com";
        this.SsalawaaaaaaaaaaaaaatLabel.Text = "ٱللَّهُمَّ  صَلِّ  عَلَىٰ  مُحَمَّدٍ  وَءَالِ  مُحَمَّدٍ";
        this.SsalawaaaaaaaaaaaaaatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.ToolTip.SetToolTip(this.SsalawaaaaaaaaaaaaaatLabel, "may Allah draw nearer to Him Muhammed and his progeny");
        this.SsalawaaaaaaaaaaaaaatLabel.Click += new System.EventHandler(this.LinkLabel_Click);
        // 
        // WordFrequencyLabel
        // 
        this.WordFrequencyLabel.BackColor = System.Drawing.Color.Transparent;
        this.WordFrequencyLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.WordFrequencyLabel.Dock = System.Windows.Forms.DockStyle.Top;
        this.WordFrequencyLabel.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.WordFrequencyLabel.ForeColor = System.Drawing.Color.MidnightBlue;
        this.WordFrequencyLabel.Location = new System.Drawing.Point(0, 92);
        this.WordFrequencyLabel.Name = "WordFrequencyLabel";
        this.WordFrequencyLabel.Size = new System.Drawing.Size(239, 19);
        this.WordFrequencyLabel.TabIndex = 14;
        this.WordFrequencyLabel.Tag = "http://qurancode.com";
        this.WordFrequencyLabel.Text = "من   الله   ان   في   ما   لا   الذين   الا   علي";
        this.WordFrequencyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.ToolTip.SetToolTip(this.WordFrequencyLabel, resources.GetString("WordFrequencyLabel.ToolTip"));
        this.WordFrequencyLabel.Click += new System.EventHandler(this.LinkLabel_Click);
        // 
        // ProductNameLabel
        // 
        this.ProductNameLabel.BackColor = System.Drawing.Color.Lavender;
        this.ProductNameLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.ProductNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
        this.ProductNameLabel.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
        this.ProductNameLabel.ForeColor = System.Drawing.Color.MidnightBlue;
        this.ProductNameLabel.Location = new System.Drawing.Point(0, 44);
        this.ProductNameLabel.Name = "ProductNameLabel";
        this.ProductNameLabel.Size = new System.Drawing.Size(239, 29);
        this.ProductNameLabel.TabIndex = 15;
        this.ProductNameLabel.Tag = "http://qurancode.com";
        this.ProductNameLabel.Text = "QuranCode 1433";
        this.ProductNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.ToolTip.SetToolTip(this.ProductNameLabel, resources.GetString("ProductNameLabel.ToolTip"));
        // 
        // VersionLabel
        // 
        this.VersionLabel.BackColor = System.Drawing.Color.Transparent;
        this.VersionLabel.Dock = System.Windows.Forms.DockStyle.Top;
        this.VersionLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.VersionLabel.ForeColor = System.Drawing.Color.MidnightBlue;
        this.VersionLabel.Location = new System.Drawing.Point(0, 111);
        this.VersionLabel.Name = "VersionLabel";
        this.VersionLabel.Size = new System.Drawing.Size(239, 17);
        this.VersionLabel.TabIndex = 16;
        this.VersionLabel.Tag = "http://heliwave.com";
        this.VersionLabel.Text = "©2009-2019  Ali Adams ";
        this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.ToolTip.SetToolTip(this.VersionLabel, "©2009-2019 Ali Adams - علي عبد الرزاق عبد الكريم القره غولي");
        // 
        // InitialLettersLabel
        // 
        this.InitialLettersLabel.BackColor = System.Drawing.Color.LightSteelBlue;
        this.InitialLettersLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.InitialLettersLabel.Dock = System.Windows.Forms.DockStyle.Top;
        this.InitialLettersLabel.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.InitialLettersLabel.ForeColor = System.Drawing.Color.MidnightBlue;
        this.InitialLettersLabel.Location = new System.Drawing.Point(0, 73);
        this.InitialLettersLabel.Name = "InitialLettersLabel";
        this.InitialLettersLabel.Size = new System.Drawing.Size(239, 19);
        this.InitialLettersLabel.TabIndex = 13;
        this.InitialLettersLabel.Tag = "http://qurancode.com";
        this.InitialLettersLabel.Text = "علي صراط حق نمسكه  |  نص حكيم له سر قاطع";
        this.InitialLettersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.ToolTip.SetToolTip(this.InitialLettersLabel, "Ali is a path to the Truth we follow.\r\nPerfect text with a decisive secret.");
        this.InitialLettersLabel.Click += new System.EventHandler(this.LinkLabel_Click);
        // 
        // CloseLabel
        // 
        this.CloseLabel.AutoSize = true;
        this.CloseLabel.BackColor = System.Drawing.Color.Transparent;
        this.CloseLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.CloseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.CloseLabel.ForeColor = System.Drawing.Color.Lavender;
        this.CloseLabel.Location = new System.Drawing.Point(225, -2);
        this.CloseLabel.Name = "CloseLabel";
        this.CloseLabel.Size = new System.Drawing.Size(13, 13);
        this.CloseLabel.TabIndex = 10;
        this.CloseLabel.Text = "x";
        this.CloseLabel.Visible = false;
        this.CloseLabel.Click += new System.EventHandler(this.CloseLabel_Click);
        // 
        // SplashForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(160)))), ((int)(((byte)(200)))));
        this.ClientSize = new System.Drawing.Size(239, 147);
        this.Controls.Add(this.CloseLabel);
        this.Controls.Add(this.InformationLabel);
        this.Controls.Add(this.ProgressBar);
        this.Controls.Add(this.VersionLabel);
        this.Controls.Add(this.WordFrequencyLabel);
        this.Controls.Add(this.InitialLettersLabel);
        this.Controls.Add(this.ProductNameLabel);
        this.Controls.Add(this.SsalawaaaaaaaaaaaaaatLabel);
        this.Controls.Add(this.BismAllahLabel);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        this.Name = "SplashForm";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "SplashForm";
        this.Shown += new System.EventHandler(this.SplashForm_Shown);
        this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SplashForm_KeyDown);
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label InformationLabel;
    private System.Windows.Forms.ProgressBar ProgressBar;
    private System.Windows.Forms.ToolTip ToolTip;
    private System.Windows.Forms.Label BismAllahLabel;
    private System.Windows.Forms.Label SsalawaaaaaaaaaaaaaatLabel;
    private System.Windows.Forms.Label CloseLabel;
    private System.Windows.Forms.Label WordFrequencyLabel;
    private System.Windows.Forms.Label ProductNameLabel;
    private System.Windows.Forms.Label VersionLabel;
    private System.Windows.Forms.Label InitialLettersLabel;
}
