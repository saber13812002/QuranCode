partial class MainForm
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
        this.StatusPanel = new System.Windows.Forms.Panel();
        this.ProgressBar = new System.Windows.Forms.ProgressBar();
        this.MainPanel = new System.Windows.Forms.Panel();
        this.QueryPanel = new System.Windows.Forms.Panel();
        this.MatchCountNumberTypeLabel = new System.Windows.Forms.Label();
        this.CTimesVSumNumberTypeLabel = new System.Windows.Forms.Label();
        this.CMinusVSumNumberTypeLabel = new System.Windows.Forms.Label();
        this.CPlusVSumNumberTypeLabel = new System.Windows.Forms.Label();
        this.LetterCountNumberTypeLabel = new System.Windows.Forms.Label();
        this.WordCountNumberTypeLabel = new System.Windows.Forms.Label();
        this.VerseCountNumberTypeLabel = new System.Windows.Forms.Label();
        this.ChapterSumNumberTypeLabel = new System.Windows.Forms.Label();
        this.ChapterCountNumberTypeLabel = new System.Windows.Forms.Label();
        this.ChapterCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.ChapterCountLabel = new System.Windows.Forms.Label();
        this.SaveMatchesButton = new System.Windows.Forms.Button();
        this.CTimesVSumComboBox = new System.Windows.Forms.ComboBox();
        this.ChapterSumLabel = new System.Windows.Forms.Label();
        this.CTimesVSumNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.ChapterSumNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.CountOnlyCheckBox = new System.Windows.Forms.CheckBox();
        this.CTimesVSumLabel = new System.Windows.Forms.Label();
        this.VerseCountComboBox = new System.Windows.Forms.ComboBox();
        this.LetterCountComboBox = new System.Windows.Forms.ComboBox();
        this.VerseCountLabel = new System.Windows.Forms.Label();
        this.WordCountComboBox = new System.Windows.Forms.ComboBox();
        this.VerseCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.FindButton = new System.Windows.Forms.Button();
        this.WordCountLabel = new System.Windows.Forms.Label();
        this.CMinusVSumComboBox = new System.Windows.Forms.ComboBox();
        this.WordCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.CPlusVSumComboBox = new System.Windows.Forms.ComboBox();
        this.LetterCountLabel = new System.Windows.Forms.Label();
        this.CMinusVSumNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.LetterCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.CMinusVSumLabel = new System.Windows.Forms.Label();
        this.CPlusVSumLabel = new System.Windows.Forms.Label();
        this.CPlusVSumNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.MatchCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.MatchCountComboBox = new System.Windows.Forms.ComboBox();
        this.MatchCountLabel = new System.Windows.Forms.Label();
        this.ResultsPanel = new System.Windows.Forms.Panel();
        this.OutputFormatChapterSeparatorComboBox = new System.Windows.Forms.ComboBox();
        this.OutputFormatFieldSeparatorComboBox = new System.Windows.Forms.ComboBox();
        this.MatchCountTextBox = new System.Windows.Forms.TextBox();
        this.OutputFormatLabel = new System.Windows.Forms.Label();
        this.OutputFormatZeroPadComboBox = new System.Windows.Forms.ComboBox();
        this.LettersOutputFieldCheckBox = new System.Windows.Forms.CheckBox();
        this.FoundMatchCountLabel = new System.Windows.Forms.Label();
        this.ElapsedTimeLabel = new System.Windows.Forms.Label();
        this.ElapsedTimeValueLabel = new System.Windows.Forms.Label();
        this.RemainingTimeValueLabel = new System.Windows.Forms.Label();
        this.VersesOutputFieldCheckBox = new System.Windows.Forms.CheckBox();
        this.ChapterOutputFieldCheckBox = new System.Windows.Forms.CheckBox();
        this.WordsOutputFieldCheckBox = new System.Windows.Forms.CheckBox();
        this.OutputFieldsLabel = new System.Windows.Forms.Label();
        this.ProcessedPercentageLabel = new System.Windows.Forms.Label();
        this.SalawaatEnglishLabel = new System.Windows.Forms.Label();
        this.SalawaatArabicLabel = new System.Windows.Forms.Label();
        this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.StatusPanel.SuspendLayout();
        this.MainPanel.SuspendLayout();
        this.QueryPanel.SuspendLayout();
        this.ResultsPanel.SuspendLayout();
        this.SuspendLayout();
        // 
        // StatusPanel
        // 
        this.StatusPanel.Controls.Add(this.ProgressBar);
        this.StatusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.StatusPanel.Location = new System.Drawing.Point(0, 364);
        this.StatusPanel.Name = "StatusPanel";
        this.StatusPanel.Size = new System.Drawing.Size(294, 8);
        this.StatusPanel.TabIndex = 0;
        // 
        // ProgressBar
        // 
        this.ProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
        this.ProgressBar.Location = new System.Drawing.Point(0, 0);
        this.ProgressBar.Name = "ProgressBar";
        this.ProgressBar.Size = new System.Drawing.Size(294, 8);
        this.ProgressBar.TabIndex = 34;
        // 
        // MainPanel
        // 
        this.MainPanel.BackColor = System.Drawing.SystemColors.Control;
        this.MainPanel.Controls.Add(this.QueryPanel);
        this.MainPanel.Controls.Add(this.ResultsPanel);
        this.MainPanel.Controls.Add(this.SalawaatEnglishLabel);
        this.MainPanel.Controls.Add(this.SalawaatArabicLabel);
        this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.MainPanel.Location = new System.Drawing.Point(0, 0);
        this.MainPanel.Name = "MainPanel";
        this.MainPanel.Size = new System.Drawing.Size(294, 364);
        this.MainPanel.TabIndex = 0;
        // 
        // QueryPanel
        // 
        this.QueryPanel.BackColor = System.Drawing.SystemColors.Control;
        this.QueryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.QueryPanel.Controls.Add(this.MatchCountNumberTypeLabel);
        this.QueryPanel.Controls.Add(this.CTimesVSumNumberTypeLabel);
        this.QueryPanel.Controls.Add(this.CMinusVSumNumberTypeLabel);
        this.QueryPanel.Controls.Add(this.CPlusVSumNumberTypeLabel);
        this.QueryPanel.Controls.Add(this.LetterCountNumberTypeLabel);
        this.QueryPanel.Controls.Add(this.WordCountNumberTypeLabel);
        this.QueryPanel.Controls.Add(this.VerseCountNumberTypeLabel);
        this.QueryPanel.Controls.Add(this.ChapterSumNumberTypeLabel);
        this.QueryPanel.Controls.Add(this.ChapterCountNumberTypeLabel);
        this.QueryPanel.Controls.Add(this.ChapterCountNumericUpDown);
        this.QueryPanel.Controls.Add(this.ChapterCountLabel);
        this.QueryPanel.Controls.Add(this.SaveMatchesButton);
        this.QueryPanel.Controls.Add(this.CTimesVSumComboBox);
        this.QueryPanel.Controls.Add(this.ChapterSumLabel);
        this.QueryPanel.Controls.Add(this.CTimesVSumNumericUpDown);
        this.QueryPanel.Controls.Add(this.ChapterSumNumericUpDown);
        this.QueryPanel.Controls.Add(this.CountOnlyCheckBox);
        this.QueryPanel.Controls.Add(this.CTimesVSumLabel);
        this.QueryPanel.Controls.Add(this.VerseCountComboBox);
        this.QueryPanel.Controls.Add(this.LetterCountComboBox);
        this.QueryPanel.Controls.Add(this.VerseCountLabel);
        this.QueryPanel.Controls.Add(this.WordCountComboBox);
        this.QueryPanel.Controls.Add(this.VerseCountNumericUpDown);
        this.QueryPanel.Controls.Add(this.FindButton);
        this.QueryPanel.Controls.Add(this.WordCountLabel);
        this.QueryPanel.Controls.Add(this.CMinusVSumComboBox);
        this.QueryPanel.Controls.Add(this.WordCountNumericUpDown);
        this.QueryPanel.Controls.Add(this.CPlusVSumComboBox);
        this.QueryPanel.Controls.Add(this.LetterCountLabel);
        this.QueryPanel.Controls.Add(this.CMinusVSumNumericUpDown);
        this.QueryPanel.Controls.Add(this.LetterCountNumericUpDown);
        this.QueryPanel.Controls.Add(this.CMinusVSumLabel);
        this.QueryPanel.Controls.Add(this.CPlusVSumLabel);
        this.QueryPanel.Controls.Add(this.CPlusVSumNumericUpDown);
        this.QueryPanel.Controls.Add(this.MatchCountNumericUpDown);
        this.QueryPanel.Controls.Add(this.MatchCountComboBox);
        this.QueryPanel.Controls.Add(this.MatchCountLabel);
        this.QueryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.QueryPanel.Location = new System.Drawing.Point(0, 46);
        this.QueryPanel.Name = "QueryPanel";
        this.QueryPanel.Size = new System.Drawing.Size(294, 228);
        this.QueryPanel.TabIndex = 0;
        // 
        // MatchCountNumberTypeLabel
        // 
        this.MatchCountNumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.MatchCountNumberTypeLabel.BackColor = System.Drawing.SystemColors.Window;
        this.MatchCountNumberTypeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.MatchCountNumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.MatchCountNumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.MatchCountNumberTypeLabel.ForeColor = System.Drawing.Color.Black;
        this.MatchCountNumberTypeLabel.Location = new System.Drawing.Point(259, 177);
        this.MatchCountNumberTypeLabel.Name = "MatchCountNumberTypeLabel";
        this.MatchCountNumberTypeLabel.Size = new System.Drawing.Size(24, 19);
        this.MatchCountNumberTypeLabel.TabIndex = 73;
        this.MatchCountNumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.MatchCountNumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
        // 
        // CTimesVSumNumberTypeLabel
        // 
        this.CTimesVSumNumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.CTimesVSumNumberTypeLabel.BackColor = System.Drawing.SystemColors.Window;
        this.CTimesVSumNumberTypeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.CTimesVSumNumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.CTimesVSumNumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.CTimesVSumNumberTypeLabel.ForeColor = System.Drawing.Color.Black;
        this.CTimesVSumNumberTypeLabel.Location = new System.Drawing.Point(259, 157);
        this.CTimesVSumNumberTypeLabel.Name = "CTimesVSumNumberTypeLabel";
        this.CTimesVSumNumberTypeLabel.Size = new System.Drawing.Size(24, 19);
        this.CTimesVSumNumberTypeLabel.TabIndex = 72;
        this.CTimesVSumNumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.CTimesVSumNumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
        // 
        // CMinusVSumNumberTypeLabel
        // 
        this.CMinusVSumNumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.CMinusVSumNumberTypeLabel.BackColor = System.Drawing.SystemColors.Window;
        this.CMinusVSumNumberTypeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.CMinusVSumNumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.CMinusVSumNumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.CMinusVSumNumberTypeLabel.ForeColor = System.Drawing.Color.Black;
        this.CMinusVSumNumberTypeLabel.Location = new System.Drawing.Point(259, 136);
        this.CMinusVSumNumberTypeLabel.Name = "CMinusVSumNumberTypeLabel";
        this.CMinusVSumNumberTypeLabel.Size = new System.Drawing.Size(24, 19);
        this.CMinusVSumNumberTypeLabel.TabIndex = 71;
        this.CMinusVSumNumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.CMinusVSumNumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
        // 
        // CPlusVSumNumberTypeLabel
        // 
        this.CPlusVSumNumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.CPlusVSumNumberTypeLabel.BackColor = System.Drawing.SystemColors.Window;
        this.CPlusVSumNumberTypeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.CPlusVSumNumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.CPlusVSumNumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.CPlusVSumNumberTypeLabel.ForeColor = System.Drawing.Color.Black;
        this.CPlusVSumNumberTypeLabel.Location = new System.Drawing.Point(259, 115);
        this.CPlusVSumNumberTypeLabel.Name = "CPlusVSumNumberTypeLabel";
        this.CPlusVSumNumberTypeLabel.Size = new System.Drawing.Size(24, 19);
        this.CPlusVSumNumberTypeLabel.TabIndex = 70;
        this.CPlusVSumNumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.CPlusVSumNumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
        // 
        // LetterCountNumberTypeLabel
        // 
        this.LetterCountNumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.LetterCountNumberTypeLabel.BackColor = System.Drawing.SystemColors.Window;
        this.LetterCountNumberTypeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.LetterCountNumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.LetterCountNumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.LetterCountNumberTypeLabel.ForeColor = System.Drawing.Color.Black;
        this.LetterCountNumberTypeLabel.Location = new System.Drawing.Point(259, 94);
        this.LetterCountNumberTypeLabel.Name = "LetterCountNumberTypeLabel";
        this.LetterCountNumberTypeLabel.Size = new System.Drawing.Size(24, 19);
        this.LetterCountNumberTypeLabel.TabIndex = 69;
        this.LetterCountNumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.LetterCountNumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
        // 
        // WordCountNumberTypeLabel
        // 
        this.WordCountNumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.WordCountNumberTypeLabel.BackColor = System.Drawing.SystemColors.Window;
        this.WordCountNumberTypeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.WordCountNumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.WordCountNumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.WordCountNumberTypeLabel.ForeColor = System.Drawing.Color.Black;
        this.WordCountNumberTypeLabel.Location = new System.Drawing.Point(259, 72);
        this.WordCountNumberTypeLabel.Name = "WordCountNumberTypeLabel";
        this.WordCountNumberTypeLabel.Size = new System.Drawing.Size(24, 19);
        this.WordCountNumberTypeLabel.TabIndex = 68;
        this.WordCountNumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.WordCountNumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
        // 
        // VerseCountNumberTypeLabel
        // 
        this.VerseCountNumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.VerseCountNumberTypeLabel.BackColor = System.Drawing.SystemColors.Window;
        this.VerseCountNumberTypeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.VerseCountNumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.VerseCountNumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.VerseCountNumberTypeLabel.ForeColor = System.Drawing.Color.Black;
        this.VerseCountNumberTypeLabel.Location = new System.Drawing.Point(259, 51);
        this.VerseCountNumberTypeLabel.Name = "VerseCountNumberTypeLabel";
        this.VerseCountNumberTypeLabel.Size = new System.Drawing.Size(24, 19);
        this.VerseCountNumberTypeLabel.TabIndex = 67;
        this.VerseCountNumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.VerseCountNumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
        // 
        // ChapterSumNumberTypeLabel
        // 
        this.ChapterSumNumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.ChapterSumNumberTypeLabel.BackColor = System.Drawing.SystemColors.Window;
        this.ChapterSumNumberTypeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.ChapterSumNumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.ChapterSumNumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ChapterSumNumberTypeLabel.ForeColor = System.Drawing.Color.Black;
        this.ChapterSumNumberTypeLabel.Location = new System.Drawing.Point(259, 26);
        this.ChapterSumNumberTypeLabel.Name = "ChapterSumNumberTypeLabel";
        this.ChapterSumNumberTypeLabel.Size = new System.Drawing.Size(24, 19);
        this.ChapterSumNumberTypeLabel.TabIndex = 66;
        this.ChapterSumNumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.ChapterSumNumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
        // 
        // ChapterCountNumberTypeLabel
        // 
        this.ChapterCountNumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.ChapterCountNumberTypeLabel.BackColor = System.Drawing.SystemColors.Window;
        this.ChapterCountNumberTypeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.ChapterCountNumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.ChapterCountNumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ChapterCountNumberTypeLabel.ForeColor = System.Drawing.Color.Black;
        this.ChapterCountNumberTypeLabel.Location = new System.Drawing.Point(259, 5);
        this.ChapterCountNumberTypeLabel.Name = "ChapterCountNumberTypeLabel";
        this.ChapterCountNumberTypeLabel.Size = new System.Drawing.Size(24, 19);
        this.ChapterCountNumberTypeLabel.TabIndex = 65;
        this.ChapterCountNumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.ChapterCountNumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
        // 
        // ChapterCountNumericUpDown
        // 
        this.ChapterCountNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.ChapterCountNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ChapterCountNumericUpDown.Location = new System.Drawing.Point(91, 5);
        this.ChapterCountNumericUpDown.Maximum = new decimal(new int[] {
            114,
            0,
            0,
            0});
        this.ChapterCountNumericUpDown.Name = "ChapterCountNumericUpDown";
        this.ChapterCountNumericUpDown.Size = new System.Drawing.Size(166, 20);
        this.ChapterCountNumericUpDown.TabIndex = 3;
        this.ChapterCountNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        this.ToolTip.SetToolTip(this.ChapterCountNumericUpDown, "عدد السور");
        this.ChapterCountNumericUpDown.ValueChanged += new System.EventHandler(this.QueryParameterNumericUpDown_ValueChanged);
        this.ChapterCountNumericUpDown.Leave += new System.EventHandler(this.QueryParameterNumericUpDown_Leave);
        this.ChapterCountNumericUpDown.Enter += new System.EventHandler(this.QueryParameterNumericUpDown_Enter);
        // 
        // ChapterCountLabel
        // 
        this.ChapterCountLabel.Location = new System.Drawing.Point(6, 8);
        this.ChapterCountLabel.Name = "ChapterCountLabel";
        this.ChapterCountLabel.Size = new System.Drawing.Size(83, 13);
        this.ChapterCountLabel.TabIndex = 0;
        this.ChapterCountLabel.Text = "Chapter Count";
        this.ToolTip.SetToolTip(this.ChapterCountLabel, "عدد السور");
        // 
        // SaveMatchesButton
        // 
        this.SaveMatchesButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.SaveMatchesButton.Location = new System.Drawing.Point(4, 201);
        this.SaveMatchesButton.Name = "SaveMatchesButton";
        this.SaveMatchesButton.Size = new System.Drawing.Size(86, 23);
        this.SaveMatchesButton.TabIndex = 19;
        this.SaveMatchesButton.Tag = "";
        this.SaveMatchesButton.Text = "&View";
        this.ToolTip.SetToolTip(this.SaveMatchesButton, "إعرض النتائج");
        this.SaveMatchesButton.UseVisualStyleBackColor = true;
        this.SaveMatchesButton.Click += new System.EventHandler(this.SaveMatchesButton_Click);
        // 
        // CTimesVSumComboBox
        // 
        this.CTimesVSumComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.CTimesVSumComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.CTimesVSumComboBox.FormattingEnabled = true;
        this.CTimesVSumComboBox.Location = new System.Drawing.Point(91, 156);
        this.CTimesVSumComboBox.Name = "CTimesVSumComboBox";
        this.CTimesVSumComboBox.Size = new System.Drawing.Size(90, 20);
        this.CTimesVSumComboBox.TabIndex = 15;
        this.CTimesVSumComboBox.SelectedIndexChanged += new System.EventHandler(this.QueryParameterComboBox_SelectedIndexChanged);
        // 
        // ChapterSumLabel
        // 
        this.ChapterSumLabel.Location = new System.Drawing.Point(6, 28);
        this.ChapterSumLabel.Name = "ChapterSumLabel";
        this.ChapterSumLabel.Size = new System.Drawing.Size(83, 13);
        this.ChapterSumLabel.TabIndex = 0;
        this.ChapterSumLabel.Text = "Chapter Sum";
        this.ToolTip.SetToolTip(this.ChapterSumLabel, "مجموع ارقام السور");
        // 
        // CTimesVSumNumericUpDown
        // 
        this.CTimesVSumNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.CTimesVSumNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.CTimesVSumNumericUpDown.Location = new System.Drawing.Point(183, 156);
        this.CTimesVSumNumericUpDown.Maximum = new decimal(new int[] {
            -2146290736,
            0,
            0,
            0});
        this.CTimesVSumNumericUpDown.Name = "CTimesVSumNumericUpDown";
        this.CTimesVSumNumericUpDown.Size = new System.Drawing.Size(74, 20);
        this.CTimesVSumNumericUpDown.TabIndex = 16;
        this.CTimesVSumNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        // 
        // ChapterSumNumericUpDown
        // 
        this.ChapterSumNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.ChapterSumNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ChapterSumNumericUpDown.Location = new System.Drawing.Point(91, 26);
        this.ChapterSumNumericUpDown.Maximum = new decimal(new int[] {
            6555,
            0,
            0,
            0});
        this.ChapterSumNumericUpDown.Name = "ChapterSumNumericUpDown";
        this.ChapterSumNumericUpDown.Size = new System.Drawing.Size(166, 20);
        this.ChapterSumNumericUpDown.TabIndex = 4;
        this.ChapterSumNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        this.ToolTip.SetToolTip(this.ChapterSumNumericUpDown, "مجموع ارقام السور");
        this.ChapterSumNumericUpDown.ValueChanged += new System.EventHandler(this.QueryParameterNumericUpDown_ValueChanged);
        this.ChapterSumNumericUpDown.Leave += new System.EventHandler(this.QueryParameterNumericUpDown_Leave);
        this.ChapterSumNumericUpDown.Enter += new System.EventHandler(this.QueryParameterNumericUpDown_Enter);
        // 
        // CountOnlyCheckBox
        // 
        this.CountOnlyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.CountOnlyCheckBox.AutoSize = true;
        this.CountOnlyCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
        this.CountOnlyCheckBox.Location = new System.Drawing.Point(263, 206);
        this.CountOnlyCheckBox.Name = "CountOnlyCheckBox";
        this.CountOnlyCheckBox.Size = new System.Drawing.Size(15, 14);
        this.CountOnlyCheckBox.TabIndex = 21;
        this.ToolTip.SetToolTip(this.CountOnlyCheckBox, "Fast count only\r\nتعداد فقط للسرعة");
        this.CountOnlyCheckBox.UseVisualStyleBackColor = true;
        this.CountOnlyCheckBox.CheckedChanged += new System.EventHandler(this.CountOnlyCheckBox_CheckedChanged);
        // 
        // CTimesVSumLabel
        // 
        this.CTimesVSumLabel.Location = new System.Drawing.Point(6, 159);
        this.CTimesVSumLabel.Name = "CTimesVSumLabel";
        this.CTimesVSumLabel.Size = new System.Drawing.Size(78, 13);
        this.CTimesVSumLabel.TabIndex = 0;
        this.CTimesVSumLabel.Text = "C × V  Sum";
        // 
        // VerseCountComboBox
        // 
        this.VerseCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.VerseCountComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.VerseCountComboBox.FormattingEnabled = true;
        this.VerseCountComboBox.Location = new System.Drawing.Point(91, 51);
        this.VerseCountComboBox.Name = "VerseCountComboBox";
        this.VerseCountComboBox.Size = new System.Drawing.Size(90, 20);
        this.VerseCountComboBox.TabIndex = 5;
        this.VerseCountComboBox.SelectedIndexChanged += new System.EventHandler(this.QueryParameterComboBox_SelectedIndexChanged);
        // 
        // LetterCountComboBox
        // 
        this.LetterCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.LetterCountComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.LetterCountComboBox.FormattingEnabled = true;
        this.LetterCountComboBox.Location = new System.Drawing.Point(91, 93);
        this.LetterCountComboBox.Name = "LetterCountComboBox";
        this.LetterCountComboBox.Size = new System.Drawing.Size(90, 20);
        this.LetterCountComboBox.TabIndex = 9;
        this.LetterCountComboBox.SelectedIndexChanged += new System.EventHandler(this.QueryParameterComboBox_SelectedIndexChanged);
        // 
        // VerseCountLabel
        // 
        this.VerseCountLabel.Location = new System.Drawing.Point(6, 54);
        this.VerseCountLabel.Name = "VerseCountLabel";
        this.VerseCountLabel.Size = new System.Drawing.Size(83, 13);
        this.VerseCountLabel.TabIndex = 0;
        this.VerseCountLabel.Text = "Verse Count";
        this.ToolTip.SetToolTip(this.VerseCountLabel, "مجموع ءايات السور");
        // 
        // WordCountComboBox
        // 
        this.WordCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.WordCountComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.WordCountComboBox.FormattingEnabled = true;
        this.WordCountComboBox.Location = new System.Drawing.Point(91, 72);
        this.WordCountComboBox.Name = "WordCountComboBox";
        this.WordCountComboBox.Size = new System.Drawing.Size(90, 20);
        this.WordCountComboBox.TabIndex = 7;
        this.WordCountComboBox.SelectedIndexChanged += new System.EventHandler(this.QueryParameterComboBox_SelectedIndexChanged);
        // 
        // VerseCountNumericUpDown
        // 
        this.VerseCountNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.VerseCountNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.VerseCountNumericUpDown.Location = new System.Drawing.Point(183, 51);
        this.VerseCountNumericUpDown.Maximum = new decimal(new int[] {
            6236,
            0,
            0,
            0});
        this.VerseCountNumericUpDown.Name = "VerseCountNumericUpDown";
        this.VerseCountNumericUpDown.Size = new System.Drawing.Size(74, 20);
        this.VerseCountNumericUpDown.TabIndex = 6;
        this.VerseCountNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        this.ToolTip.SetToolTip(this.VerseCountNumericUpDown, "مجموع ءايات السور");
        this.VerseCountNumericUpDown.ValueChanged += new System.EventHandler(this.QueryParameterNumericUpDown_ValueChanged);
        this.VerseCountNumericUpDown.Leave += new System.EventHandler(this.QueryParameterNumericUpDown_Leave);
        this.VerseCountNumericUpDown.Enter += new System.EventHandler(this.QueryParameterNumericUpDown_Enter);
        // 
        // FindButton
        // 
        this.FindButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.FindButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.FindButton.Location = new System.Drawing.Point(91, 201);
        this.FindButton.Name = "FindButton";
        this.FindButton.Size = new System.Drawing.Size(194, 23);
        this.FindButton.TabIndex = 20;
        this.FindButton.Tag = "";
        this.FindButton.Text = "&Find";
        this.ToolTip.SetToolTip(this.FindButton, "إبحث");
        this.FindButton.UseVisualStyleBackColor = true;
        this.FindButton.Click += new System.EventHandler(this.FindButton_Click);
        // 
        // WordCountLabel
        // 
        this.WordCountLabel.Location = new System.Drawing.Point(6, 75);
        this.WordCountLabel.Name = "WordCountLabel";
        this.WordCountLabel.Size = new System.Drawing.Size(83, 13);
        this.WordCountLabel.TabIndex = 0;
        this.WordCountLabel.Text = "Word Count";
        this.ToolTip.SetToolTip(this.WordCountLabel, "مجموع كلمات السور");
        // 
        // CMinusVSumComboBox
        // 
        this.CMinusVSumComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.CMinusVSumComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.CMinusVSumComboBox.FormattingEnabled = true;
        this.CMinusVSumComboBox.Location = new System.Drawing.Point(91, 135);
        this.CMinusVSumComboBox.Name = "CMinusVSumComboBox";
        this.CMinusVSumComboBox.Size = new System.Drawing.Size(90, 20);
        this.CMinusVSumComboBox.TabIndex = 13;
        this.CMinusVSumComboBox.SelectedIndexChanged += new System.EventHandler(this.QueryParameterComboBox_SelectedIndexChanged);
        // 
        // WordCountNumericUpDown
        // 
        this.WordCountNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.WordCountNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.WordCountNumericUpDown.Location = new System.Drawing.Point(183, 72);
        this.WordCountNumericUpDown.Maximum = new decimal(new int[] {
            77878,
            0,
            0,
            0});
        this.WordCountNumericUpDown.Name = "WordCountNumericUpDown";
        this.WordCountNumericUpDown.Size = new System.Drawing.Size(74, 20);
        this.WordCountNumericUpDown.TabIndex = 8;
        this.WordCountNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        this.ToolTip.SetToolTip(this.WordCountNumericUpDown, "مجموع كلمات السور");
        this.WordCountNumericUpDown.ValueChanged += new System.EventHandler(this.QueryParameterNumericUpDown_ValueChanged);
        this.WordCountNumericUpDown.Leave += new System.EventHandler(this.QueryParameterNumericUpDown_Leave);
        this.WordCountNumericUpDown.Enter += new System.EventHandler(this.QueryParameterNumericUpDown_Enter);
        // 
        // CPlusVSumComboBox
        // 
        this.CPlusVSumComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.CPlusVSumComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.CPlusVSumComboBox.FormattingEnabled = true;
        this.CPlusVSumComboBox.Location = new System.Drawing.Point(91, 114);
        this.CPlusVSumComboBox.Name = "CPlusVSumComboBox";
        this.CPlusVSumComboBox.Size = new System.Drawing.Size(90, 20);
        this.CPlusVSumComboBox.TabIndex = 11;
        this.CPlusVSumComboBox.SelectedIndexChanged += new System.EventHandler(this.QueryParameterComboBox_SelectedIndexChanged);
        // 
        // LetterCountLabel
        // 
        this.LetterCountLabel.Location = new System.Drawing.Point(6, 96);
        this.LetterCountLabel.Name = "LetterCountLabel";
        this.LetterCountLabel.Size = new System.Drawing.Size(83, 13);
        this.LetterCountLabel.TabIndex = 0;
        this.LetterCountLabel.Text = "Letter Count";
        this.ToolTip.SetToolTip(this.LetterCountLabel, "مجموع حروف السور");
        // 
        // CMinusVSumNumericUpDown
        // 
        this.CMinusVSumNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.CMinusVSumNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.CMinusVSumNumericUpDown.Location = new System.Drawing.Point(183, 135);
        this.CMinusVSumNumericUpDown.Maximum = new decimal(new int[] {
            321237,
            0,
            0,
            0});
        this.CMinusVSumNumericUpDown.Name = "CMinusVSumNumericUpDown";
        this.CMinusVSumNumericUpDown.Size = new System.Drawing.Size(74, 20);
        this.CMinusVSumNumericUpDown.TabIndex = 14;
        this.CMinusVSumNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        this.CMinusVSumNumericUpDown.ValueChanged += new System.EventHandler(this.QueryParameterNumericUpDown_ValueChanged);
        this.CMinusVSumNumericUpDown.Leave += new System.EventHandler(this.QueryParameterNumericUpDown_Leave);
        this.CMinusVSumNumericUpDown.Enter += new System.EventHandler(this.QueryParameterNumericUpDown_Enter);
        // 
        // LetterCountNumericUpDown
        // 
        this.LetterCountNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.LetterCountNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.LetterCountNumericUpDown.Location = new System.Drawing.Point(183, 93);
        this.LetterCountNumericUpDown.Maximum = new decimal(new int[] {
            327792,
            0,
            0,
            0});
        this.LetterCountNumericUpDown.Name = "LetterCountNumericUpDown";
        this.LetterCountNumericUpDown.Size = new System.Drawing.Size(74, 20);
        this.LetterCountNumericUpDown.TabIndex = 10;
        this.LetterCountNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        this.ToolTip.SetToolTip(this.LetterCountNumericUpDown, "مجموع حروف السور");
        this.LetterCountNumericUpDown.ValueChanged += new System.EventHandler(this.QueryParameterNumericUpDown_ValueChanged);
        this.LetterCountNumericUpDown.Leave += new System.EventHandler(this.QueryParameterNumericUpDown_Leave);
        this.LetterCountNumericUpDown.Enter += new System.EventHandler(this.QueryParameterNumericUpDown_Enter);
        // 
        // CMinusVSumLabel
        // 
        this.CMinusVSumLabel.Location = new System.Drawing.Point(6, 138);
        this.CMinusVSumLabel.Name = "CMinusVSumLabel";
        this.CMinusVSumLabel.Size = new System.Drawing.Size(78, 13);
        this.CMinusVSumLabel.TabIndex = 0;
        this.CMinusVSumLabel.Text = "C − V  Sum";
        // 
        // CPlusVSumLabel
        // 
        this.CPlusVSumLabel.Location = new System.Drawing.Point(6, 118);
        this.CPlusVSumLabel.Name = "CPlusVSumLabel";
        this.CPlusVSumLabel.Size = new System.Drawing.Size(78, 13);
        this.CPlusVSumLabel.TabIndex = 0;
        this.CPlusVSumLabel.Text = "C + V  Sum";
        // 
        // CPlusVSumNumericUpDown
        // 
        this.CPlusVSumNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.CPlusVSumNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.CPlusVSumNumericUpDown.Location = new System.Drawing.Point(183, 114);
        this.CPlusVSumNumericUpDown.Maximum = new decimal(new int[] {
            334347,
            0,
            0,
            0});
        this.CPlusVSumNumericUpDown.Name = "CPlusVSumNumericUpDown";
        this.CPlusVSumNumericUpDown.Size = new System.Drawing.Size(74, 20);
        this.CPlusVSumNumericUpDown.TabIndex = 12;
        this.CPlusVSumNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        this.CPlusVSumNumericUpDown.ValueChanged += new System.EventHandler(this.QueryParameterNumericUpDown_ValueChanged);
        this.CPlusVSumNumericUpDown.Leave += new System.EventHandler(this.QueryParameterNumericUpDown_Leave);
        this.CPlusVSumNumericUpDown.Enter += new System.EventHandler(this.QueryParameterNumericUpDown_Enter);
        // 
        // MatchCountNumericUpDown
        // 
        this.MatchCountNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.MatchCountNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.MatchCountNumericUpDown.Location = new System.Drawing.Point(183, 177);
        this.MatchCountNumericUpDown.Maximum = new decimal(new int[] {
            -2146290736,
            0,
            0,
            0});
        this.MatchCountNumericUpDown.Name = "MatchCountNumericUpDown";
        this.MatchCountNumericUpDown.Size = new System.Drawing.Size(74, 20);
        this.MatchCountNumericUpDown.TabIndex = 18;
        this.MatchCountNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        // 
        // MatchCountComboBox
        // 
        this.MatchCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.MatchCountComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.MatchCountComboBox.FormattingEnabled = true;
        this.MatchCountComboBox.Location = new System.Drawing.Point(91, 177);
        this.MatchCountComboBox.Name = "MatchCountComboBox";
        this.MatchCountComboBox.Size = new System.Drawing.Size(90, 20);
        this.MatchCountComboBox.TabIndex = 17;
        this.MatchCountComboBox.SelectedIndexChanged += new System.EventHandler(this.QueryParameterComboBox_SelectedIndexChanged);
        // 
        // MatchCountLabel
        // 
        this.MatchCountLabel.Location = new System.Drawing.Point(6, 180);
        this.MatchCountLabel.Name = "MatchCountLabel";
        this.MatchCountLabel.Size = new System.Drawing.Size(78, 13);
        this.MatchCountLabel.TabIndex = 20;
        this.MatchCountLabel.Text = "Match Count";
        this.ToolTip.SetToolTip(this.MatchCountLabel, "عدد النتائج");
        // 
        // ResultsPanel
        // 
        this.ResultsPanel.BackColor = System.Drawing.Color.LightSteelBlue;
        this.ResultsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.ResultsPanel.Controls.Add(this.OutputFormatChapterSeparatorComboBox);
        this.ResultsPanel.Controls.Add(this.OutputFormatFieldSeparatorComboBox);
        this.ResultsPanel.Controls.Add(this.MatchCountTextBox);
        this.ResultsPanel.Controls.Add(this.OutputFormatLabel);
        this.ResultsPanel.Controls.Add(this.OutputFormatZeroPadComboBox);
        this.ResultsPanel.Controls.Add(this.LettersOutputFieldCheckBox);
        this.ResultsPanel.Controls.Add(this.FoundMatchCountLabel);
        this.ResultsPanel.Controls.Add(this.ElapsedTimeLabel);
        this.ResultsPanel.Controls.Add(this.ElapsedTimeValueLabel);
        this.ResultsPanel.Controls.Add(this.RemainingTimeValueLabel);
        this.ResultsPanel.Controls.Add(this.VersesOutputFieldCheckBox);
        this.ResultsPanel.Controls.Add(this.ChapterOutputFieldCheckBox);
        this.ResultsPanel.Controls.Add(this.WordsOutputFieldCheckBox);
        this.ResultsPanel.Controls.Add(this.OutputFieldsLabel);
        this.ResultsPanel.Controls.Add(this.ProcessedPercentageLabel);
        this.ResultsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.ResultsPanel.Location = new System.Drawing.Point(0, 274);
        this.ResultsPanel.Name = "ResultsPanel";
        this.ResultsPanel.Size = new System.Drawing.Size(294, 90);
        this.ResultsPanel.TabIndex = 0;
        // 
        // OutputFormatChapterSeparatorComboBox
        // 
        this.OutputFormatChapterSeparatorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.OutputFormatChapterSeparatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.OutputFormatChapterSeparatorComboBox.FormattingEnabled = true;
        this.OutputFormatChapterSeparatorComboBox.Items.AddRange(new object[] {
            "CCC.VVV.WWWW.LLLLL"});
        this.OutputFormatChapterSeparatorComboBox.Location = new System.Drawing.Point(206, 23);
        this.OutputFormatChapterSeparatorComboBox.Name = "OutputFormatChapterSeparatorComboBox";
        this.OutputFormatChapterSeparatorComboBox.Size = new System.Drawing.Size(79, 21);
        this.OutputFormatChapterSeparatorComboBox.TabIndex = 29;
        this.ToolTip.SetToolTip(this.OutputFormatChapterSeparatorComboBox, "Chapter separator");
        this.OutputFormatChapterSeparatorComboBox.SelectedIndexChanged += new System.EventHandler(this.OutputFormatChapterSeparatorComboBox_SelectedIndexChanged);
        // 
        // OutputFormatFieldSeparatorComboBox
        // 
        this.OutputFormatFieldSeparatorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.OutputFormatFieldSeparatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.OutputFormatFieldSeparatorComboBox.FormattingEnabled = true;
        this.OutputFormatFieldSeparatorComboBox.Items.AddRange(new object[] {
            "CCC.VVV.WWWW.LLLLL"});
        this.OutputFormatFieldSeparatorComboBox.Location = new System.Drawing.Point(143, 23);
        this.OutputFormatFieldSeparatorComboBox.Name = "OutputFormatFieldSeparatorComboBox";
        this.OutputFormatFieldSeparatorComboBox.Size = new System.Drawing.Size(61, 21);
        this.OutputFormatFieldSeparatorComboBox.TabIndex = 28;
        this.ToolTip.SetToolTip(this.OutputFormatFieldSeparatorComboBox, "Field separator");
        this.OutputFormatFieldSeparatorComboBox.SelectedIndexChanged += new System.EventHandler(this.OutputFormatFieldSeparatorComboBox_SelectedIndexChanged);
        // 
        // MatchCountTextBox
        // 
        this.MatchCountTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.MatchCountTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.MatchCountTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.MatchCountTextBox.Location = new System.Drawing.Point(91, 46);
        this.MatchCountTextBox.Name = "MatchCountTextBox";
        this.MatchCountTextBox.ReadOnly = true;
        this.MatchCountTextBox.Size = new System.Drawing.Size(194, 20);
        this.MatchCountTextBox.TabIndex = 30;
        this.MatchCountTextBox.Text = "0";
        this.MatchCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        // 
        // OutputFormatLabel
        // 
        this.OutputFormatLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.OutputFormatLabel.Location = new System.Drawing.Point(4, 27);
        this.OutputFormatLabel.Name = "OutputFormatLabel";
        this.OutputFormatLabel.Size = new System.Drawing.Size(83, 13);
        this.OutputFormatLabel.TabIndex = 31;
        this.OutputFormatLabel.Text = "Output Format";
        this.ToolTip.SetToolTip(this.OutputFormatLabel, "حقول النتائج");
        // 
        // OutputFormatZeroPadComboBox
        // 
        this.OutputFormatZeroPadComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.OutputFormatZeroPadComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.OutputFormatZeroPadComboBox.FormattingEnabled = true;
        this.OutputFormatZeroPadComboBox.Items.AddRange(new object[] {
            "CCC.VVV.WWWW.LLLLL"});
        this.OutputFormatZeroPadComboBox.Location = new System.Drawing.Point(91, 23);
        this.OutputFormatZeroPadComboBox.Name = "OutputFormatZeroPadComboBox";
        this.OutputFormatZeroPadComboBox.Size = new System.Drawing.Size(50, 21);
        this.OutputFormatZeroPadComboBox.TabIndex = 27;
        this.ToolTip.SetToolTip(this.OutputFormatZeroPadComboBox, "Zero padding");
        this.OutputFormatZeroPadComboBox.SelectedIndexChanged += new System.EventHandler(this.OutputFormatZeroPadComboBox_SelectedIndexChanged);
        // 
        // LettersOutputFieldCheckBox
        // 
        this.LettersOutputFieldCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.LettersOutputFieldCheckBox.AutoSize = true;
        this.LettersOutputFieldCheckBox.Checked = true;
        this.LettersOutputFieldCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        this.LettersOutputFieldCheckBox.Location = new System.Drawing.Point(254, 5);
        this.LettersOutputFieldCheckBox.Name = "LettersOutputFieldCheckBox";
        this.LettersOutputFieldCheckBox.Size = new System.Drawing.Size(32, 17);
        this.LettersOutputFieldCheckBox.TabIndex = 26;
        this.LettersOutputFieldCheckBox.Text = "L";
        this.ToolTip.SetToolTip(this.LettersOutputFieldCheckBox, "Include letters in output");
        this.LettersOutputFieldCheckBox.UseVisualStyleBackColor = true;
        this.LettersOutputFieldCheckBox.CheckedChanged += new System.EventHandler(this.LettersOutputFieldCheckBox_CheckedChanged);
        // 
        // FoundMatchCountLabel
        // 
        this.FoundMatchCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.FoundMatchCountLabel.Location = new System.Drawing.Point(4, 50);
        this.FoundMatchCountLabel.Name = "FoundMatchCountLabel";
        this.FoundMatchCountLabel.Size = new System.Drawing.Size(83, 16);
        this.FoundMatchCountLabel.TabIndex = 0;
        this.FoundMatchCountLabel.Text = "Match Count";
        this.ToolTip.SetToolTip(this.FoundMatchCountLabel, "عدد النتائج");
        // 
        // ElapsedTimeLabel
        // 
        this.ElapsedTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ElapsedTimeLabel.Location = new System.Drawing.Point(4, 70);
        this.ElapsedTimeLabel.Name = "ElapsedTimeLabel";
        this.ElapsedTimeLabel.Size = new System.Drawing.Size(83, 13);
        this.ElapsedTimeLabel.TabIndex = 0;
        this.ElapsedTimeLabel.Text = "Elapsed Time";
        this.ToolTip.SetToolTip(this.ElapsedTimeLabel, "الوقت المستغرق");
        // 
        // ElapsedTimeValueLabel
        // 
        this.ElapsedTimeValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.ElapsedTimeValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.ElapsedTimeValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ElapsedTimeValueLabel.Location = new System.Drawing.Point(91, 68);
        this.ElapsedTimeValueLabel.Name = "ElapsedTimeValueLabel";
        this.ElapsedTimeValueLabel.Size = new System.Drawing.Size(194, 16);
        this.ElapsedTimeValueLabel.TabIndex = 32;
        this.ElapsedTimeValueLabel.Text = "0d 00h 00m 00s";
        this.ElapsedTimeValueLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // RemainingTimeValueLabel
        // 
        this.RemainingTimeValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.RemainingTimeValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.RemainingTimeValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.RemainingTimeValueLabel.Location = new System.Drawing.Point(179, 68);
        this.RemainingTimeValueLabel.Name = "RemainingTimeValueLabel";
        this.RemainingTimeValueLabel.Size = new System.Drawing.Size(91, 16);
        this.RemainingTimeValueLabel.TabIndex = 33;
        this.RemainingTimeValueLabel.Text = "0d 00h 00m 00s";
        this.RemainingTimeValueLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        this.ToolTip.SetToolTip(this.RemainingTimeValueLabel, "الوقت المتبقي");
        // 
        // VersesOutputFieldCheckBox
        // 
        this.VersesOutputFieldCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.VersesOutputFieldCheckBox.AutoSize = true;
        this.VersesOutputFieldCheckBox.Checked = true;
        this.VersesOutputFieldCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        this.VersesOutputFieldCheckBox.Location = new System.Drawing.Point(144, 5);
        this.VersesOutputFieldCheckBox.Name = "VersesOutputFieldCheckBox";
        this.VersesOutputFieldCheckBox.Size = new System.Drawing.Size(33, 17);
        this.VersesOutputFieldCheckBox.TabIndex = 24;
        this.VersesOutputFieldCheckBox.Text = "V";
        this.ToolTip.SetToolTip(this.VersesOutputFieldCheckBox, "Include verses in output");
        this.VersesOutputFieldCheckBox.UseVisualStyleBackColor = true;
        this.VersesOutputFieldCheckBox.CheckedChanged += new System.EventHandler(this.VersesOutputFieldCheckBox_CheckedChanged);
        // 
        // ChapterOutputFieldCheckBox
        // 
        this.ChapterOutputFieldCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ChapterOutputFieldCheckBox.AutoSize = true;
        this.ChapterOutputFieldCheckBox.Checked = true;
        this.ChapterOutputFieldCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        this.ChapterOutputFieldCheckBox.Location = new System.Drawing.Point(91, 5);
        this.ChapterOutputFieldCheckBox.Name = "ChapterOutputFieldCheckBox";
        this.ChapterOutputFieldCheckBox.Size = new System.Drawing.Size(33, 17);
        this.ChapterOutputFieldCheckBox.TabIndex = 23;
        this.ChapterOutputFieldCheckBox.Text = "C";
        this.ToolTip.SetToolTip(this.ChapterOutputFieldCheckBox, "Include chapters");
        this.ChapterOutputFieldCheckBox.UseVisualStyleBackColor = true;
        this.ChapterOutputFieldCheckBox.CheckedChanged += new System.EventHandler(this.ChapterOutputFieldCheckBox_CheckedChanged);
        // 
        // WordsOutputFieldCheckBox
        // 
        this.WordsOutputFieldCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.WordsOutputFieldCheckBox.AutoSize = true;
        this.WordsOutputFieldCheckBox.Checked = true;
        this.WordsOutputFieldCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        this.WordsOutputFieldCheckBox.Location = new System.Drawing.Point(197, 5);
        this.WordsOutputFieldCheckBox.Name = "WordsOutputFieldCheckBox";
        this.WordsOutputFieldCheckBox.Size = new System.Drawing.Size(37, 17);
        this.WordsOutputFieldCheckBox.TabIndex = 25;
        this.WordsOutputFieldCheckBox.Text = "W";
        this.ToolTip.SetToolTip(this.WordsOutputFieldCheckBox, "Include words in output");
        this.WordsOutputFieldCheckBox.UseVisualStyleBackColor = true;
        this.WordsOutputFieldCheckBox.CheckedChanged += new System.EventHandler(this.WordsOutputFieldCheckBox_CheckedChanged);
        // 
        // OutputFieldsLabel
        // 
        this.OutputFieldsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.OutputFieldsLabel.Location = new System.Drawing.Point(4, 8);
        this.OutputFieldsLabel.Name = "OutputFieldsLabel";
        this.OutputFieldsLabel.Size = new System.Drawing.Size(83, 13);
        this.OutputFieldsLabel.TabIndex = 0;
        this.OutputFieldsLabel.Text = "Output Fields";
        this.ToolTip.SetToolTip(this.OutputFieldsLabel, "حقول النتائج");
        // 
        // ProcessedPercentageLabel
        // 
        this.ProcessedPercentageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.ProcessedPercentageLabel.BackColor = System.Drawing.SystemColors.ActiveCaption;
        this.ProcessedPercentageLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.ProcessedPercentageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ProcessedPercentageLabel.Location = new System.Drawing.Point(182, 49);
        this.ProcessedPercentageLabel.Name = "ProcessedPercentageLabel";
        this.ProcessedPercentageLabel.Size = new System.Drawing.Size(38, 16);
        this.ProcessedPercentageLabel.TabIndex = 31;
        this.ProcessedPercentageLabel.Text = "100%";
        this.ProcessedPercentageLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // SalawaatEnglishLabel
        // 
        this.SalawaatEnglishLabel.BackColor = System.Drawing.Color.LightSlateGray;
        this.SalawaatEnglishLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.SalawaatEnglishLabel.Dock = System.Windows.Forms.DockStyle.Top;
        this.SalawaatEnglishLabel.ForeColor = System.Drawing.Color.Transparent;
        this.SalawaatEnglishLabel.Location = new System.Drawing.Point(0, 29);
        this.SalawaatEnglishLabel.Name = "SalawaatEnglishLabel";
        this.SalawaatEnglishLabel.Size = new System.Drawing.Size(294, 17);
        this.SalawaatEnglishLabel.TabIndex = 2;
        this.SalawaatEnglishLabel.Tag = "http://heliwave.com/main.html";
        this.SalawaatEnglishLabel.Text = "may Allah draw nearer to Him muhammed and his progeny";
        this.SalawaatEnglishLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        this.SalawaatEnglishLabel.Click += new System.EventHandler(this.WebsiteLabel_Click);
        // 
        // SalawaatArabicLabel
        // 
        this.SalawaatArabicLabel.BackColor = System.Drawing.Color.LightSlateGray;
        this.SalawaatArabicLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.SalawaatArabicLabel.Dock = System.Windows.Forms.DockStyle.Top;
        this.SalawaatArabicLabel.Font = new System.Drawing.Font("Andalus", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.SalawaatArabicLabel.ForeColor = System.Drawing.Color.Transparent;
        this.SalawaatArabicLabel.Location = new System.Drawing.Point(0, 0);
        this.SalawaatArabicLabel.Name = "SalawaatArabicLabel";
        this.SalawaatArabicLabel.Size = new System.Drawing.Size(294, 29);
        this.SalawaatArabicLabel.TabIndex = 1;
        this.SalawaatArabicLabel.Tag = "http://qurancode.com";
        this.SalawaatArabicLabel.Text = "ٱللَّهُمَّ صَلِّ عَلَىٰ مُحَمَّدٍ وَءَالِ مُحَمَّدٍ";
        this.SalawaatArabicLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
        this.SalawaatArabicLabel.Click += new System.EventHandler(this.WebsiteLabel_Click);
        // 
        // MainForm
        // 
        this.AcceptButton = this.FindButton;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.SystemColors.Control;
        this.ClientSize = new System.Drawing.Size(294, 372);
        this.Controls.Add(this.MainPanel);
        this.Controls.Add(this.StatusPanel);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.KeyPreview = true;
        this.MaximizeBox = false;
        this.Name = "MainForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "QuranLab";
        this.Load += new System.EventHandler(this.MainForm_Load);
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
        this.Resize += new System.EventHandler(this.MainForm_Resize);
        this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
        this.StatusPanel.ResumeLayout(false);
        this.MainPanel.ResumeLayout(false);
        this.QueryPanel.ResumeLayout(false);
        this.QueryPanel.PerformLayout();
        this.ResultsPanel.ResumeLayout(false);
        this.ResultsPanel.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel StatusPanel;
    private System.Windows.Forms.Panel MainPanel;
    private System.Windows.Forms.NumericUpDown ChapterCountNumericUpDown;
    private System.Windows.Forms.Label ChapterCountLabel;
    private System.Windows.Forms.NumericUpDown ChapterSumNumericUpDown;
    private System.Windows.Forms.Label ChapterSumLabel;
    private System.Windows.Forms.NumericUpDown VerseCountNumericUpDown;
    private System.Windows.Forms.Label VerseCountLabel;
    private System.Windows.Forms.Button FindButton;
    private System.Windows.Forms.TextBox MatchCountTextBox;
    private System.Windows.Forms.Label FoundMatchCountLabel;
    private System.Windows.Forms.ProgressBar ProgressBar;
    private System.Windows.Forms.NumericUpDown LetterCountNumericUpDown;
    private System.Windows.Forms.Label LetterCountLabel;
    private System.Windows.Forms.NumericUpDown WordCountNumericUpDown;
    private System.Windows.Forms.Label WordCountLabel;
    private System.Windows.Forms.Button SaveMatchesButton;
    private System.Windows.Forms.Label ProcessedPercentageLabel;
    private System.Windows.Forms.Label SalawaatArabicLabel;
    private System.Windows.Forms.ToolTip ToolTip;
    private System.Windows.Forms.Label ElapsedTimeLabel;
    private System.Windows.Forms.Label ElapsedTimeValueLabel;
    private System.Windows.Forms.Label RemainingTimeValueLabel;
    private System.Windows.Forms.CheckBox CountOnlyCheckBox;
    private System.Windows.Forms.CheckBox LettersOutputFieldCheckBox;
    private System.Windows.Forms.CheckBox WordsOutputFieldCheckBox;
    private System.Windows.Forms.CheckBox VersesOutputFieldCheckBox;
    private System.Windows.Forms.NumericUpDown CMinusVSumNumericUpDown;
    private System.Windows.Forms.Label CMinusVSumLabel;
    private System.Windows.Forms.NumericUpDown CPlusVSumNumericUpDown;
    private System.Windows.Forms.Label CPlusVSumLabel;
    private System.Windows.Forms.Label SalawaatEnglishLabel;
    private System.Windows.Forms.ComboBox CPlusVSumComboBox;
    private System.Windows.Forms.ComboBox CMinusVSumComboBox;
    private System.Windows.Forms.CheckBox ChapterOutputFieldCheckBox;
    private System.Windows.Forms.ComboBox LetterCountComboBox;
    private System.Windows.Forms.ComboBox WordCountComboBox;
    private System.Windows.Forms.ComboBox VerseCountComboBox;
    private System.Windows.Forms.ComboBox CTimesVSumComboBox;
    private System.Windows.Forms.NumericUpDown CTimesVSumNumericUpDown;
    private System.Windows.Forms.Label CTimesVSumLabel;
    private System.Windows.Forms.Panel ResultsPanel;
    private System.Windows.Forms.Panel QueryPanel;
    private System.Windows.Forms.ComboBox MatchCountComboBox;
    private System.Windows.Forms.NumericUpDown MatchCountNumericUpDown;
    private System.Windows.Forms.Label MatchCountLabel;
    private System.Windows.Forms.Label OutputFieldsLabel;
    private System.Windows.Forms.ComboBox OutputFormatZeroPadComboBox;
    private System.Windows.Forms.Label OutputFormatLabel;
    private System.Windows.Forms.ComboBox OutputFormatChapterSeparatorComboBox;
    private System.Windows.Forms.ComboBox OutputFormatFieldSeparatorComboBox;
    private System.Windows.Forms.Label MatchCountNumberTypeLabel;
    private System.Windows.Forms.Label CTimesVSumNumberTypeLabel;
    private System.Windows.Forms.Label CMinusVSumNumberTypeLabel;
    private System.Windows.Forms.Label CPlusVSumNumberTypeLabel;
    private System.Windows.Forms.Label LetterCountNumberTypeLabel;
    private System.Windows.Forms.Label WordCountNumberTypeLabel;
    private System.Windows.Forms.Label VerseCountNumberTypeLabel;
    private System.Windows.Forms.Label ChapterSumNumberTypeLabel;
    private System.Windows.Forms.Label ChapterCountNumberTypeLabel;

}
