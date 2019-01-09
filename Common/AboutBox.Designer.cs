partial class AboutBox
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.AboutPictureBox = new System.Windows.Forms.PictureBox();
            this.ProductNameLabel = new System.Windows.Forms.Label();
            this.CopyrightLabel = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AboutPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TableLayoutPanel.ColumnCount = 2;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 211F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.Controls.Add(this.AboutPictureBox, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.ProductNameLabel, 1, 0);
            this.TableLayoutPanel.Controls.Add(this.CopyrightLabel, 1, 1);
            this.TableLayoutPanel.Controls.Add(this.DescriptionTextBox, 1, 2);
            this.TableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 3;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.Size = new System.Drawing.Size(549, 410);
            this.TableLayoutPanel.TabIndex = 0;
            // 
            // AboutPictureBox
            // 
            this.AboutPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AboutPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AboutPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("AboutPictureBox.Image")));
            this.AboutPictureBox.Location = new System.Drawing.Point(3, 3);
            this.AboutPictureBox.Name = "AboutPictureBox";
            this.TableLayoutPanel.SetRowSpan(this.AboutPictureBox, 3);
            this.AboutPictureBox.Size = new System.Drawing.Size(205, 408);
            this.AboutPictureBox.TabIndex = 12;
            this.AboutPictureBox.TabStop = false;
            this.AboutPictureBox.Tag = "http://heliwave.com/Soul.and.Spirit.pdf";
            this.ToolTip.SetToolTip(this.AboutPictureBox, "Open splash screen ...");
            this.AboutPictureBox.Click += new System.EventHandler(this.AboutPictureBox_Click);
            // 
            // ProductNameLabel
            // 
            this.ProductNameLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ProductNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProductNameLabel.Location = new System.Drawing.Point(217, 0);
            this.ProductNameLabel.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.ProductNameLabel.MaximumSize = new System.Drawing.Size(0, 17);
            this.ProductNameLabel.Name = "ProductNameLabel";
            this.ProductNameLabel.Size = new System.Drawing.Size(329, 17);
            this.ProductNameLabel.TabIndex = 19;
            this.ProductNameLabel.Tag = "http://qurancode.com/";
            this.ProductNameLabel.Text = "Product Name";
            this.ProductNameLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.ToolTip.SetToolTip(this.ProductNameLabel, "QuranCode");
            this.ProductNameLabel.Click += new System.EventHandler(this.LinkLabel_Click);
            // 
            // CopyrightLabel
            // 
            this.CopyrightLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CopyrightLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CopyrightLabel.Location = new System.Drawing.Point(217, 19);
            this.CopyrightLabel.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.CopyrightLabel.MaximumSize = new System.Drawing.Size(0, 17);
            this.CopyrightLabel.Name = "CopyrightLabel";
            this.CopyrightLabel.Size = new System.Drawing.Size(329, 17);
            this.CopyrightLabel.TabIndex = 21;
            this.CopyrightLabel.Tag = "http://qurancode.com/";
            this.CopyrightLabel.Text = "Copyright";
            this.CopyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ToolTip.SetToolTip(this.CopyrightLabel, "©2009-2019 Ali Adams");
            this.CopyrightLabel.Click += new System.EventHandler(this.LinkLabel_Click);
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DescriptionTextBox.Location = new System.Drawing.Point(217, 45);
            this.DescriptionTextBox.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.DescriptionTextBox.Multiline = true;
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.ReadOnly = true;
            this.DescriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.DescriptionTextBox.Size = new System.Drawing.Size(329, 366);
            this.DescriptionTextBox.TabIndex = 23;
            this.DescriptionTextBox.TabStop = false;
            this.DescriptionTextBox.Tag = "";
            this.DescriptionTextBox.Text = "Description";
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 417);
            this.Controls.Add(this.TableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AboutBox_KeyDown);
            this.TableLayoutPanel.ResumeLayout(false);
            this.TableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AboutPictureBox)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
    private System.Windows.Forms.Label ProductNameLabel;
    private System.Windows.Forms.Label CopyrightLabel;
    private System.Windows.Forms.TextBox DescriptionTextBox;
    private System.Windows.Forms.ToolTip ToolTip;
    private System.Windows.Forms.PictureBox AboutPictureBox;
}
