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
        this.GenerateWordsButton = new System.Windows.Forms.Button();
        this.AddPositionsCheckBox = new System.Windows.Forms.CheckBox();
        this.WordCountLabel = new System.Windows.Forms.Label();
        this.NumberTypeLabel = new System.Windows.Forms.Label();
        this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.ValueCombinationDirectionLabel = new System.Windows.Forms.Label();
        this.AddDistancesToPreviousCheckBox = new System.Windows.Forms.CheckBox();
        this.ValueInterlaceLabel = new System.Windows.Forms.Label();
        this.AddVerseAndWordValuesCheckBox = new System.Windows.Forms.CheckBox();
        this.InspectButton = new System.Windows.Forms.Button();
        this.AutoGenerateWordsButton = new System.Windows.Forms.Button();
        this.YaHuseinCheckBox = new System.Windows.Forms.CheckBox();
        this.ProgressBar = new System.Windows.Forms.ProgressBar();
        this.NumerologySystemComboBox = new System.Windows.Forms.ComboBox();
        this.TextModeComboBox = new System.Windows.Forms.ComboBox();
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label4 = new System.Windows.Forms.Label();
        this.label5 = new System.Windows.Forms.Label();
        this.label6 = new System.Windows.Forms.Label();
        this.label7 = new System.Windows.Forms.Label();
        this.label8 = new System.Windows.Forms.Label();
        this.label9 = new System.Windows.Forms.Label();
        this.Line3Label = new System.Windows.Forms.Label();
        this.Line2Label = new System.Windows.Forms.Label();
        this.Line1Label = new System.Windows.Forms.Label();
        this.label13 = new System.Windows.Forms.Label();
        this.label14 = new System.Windows.Forms.Label();
        this.label15 = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // GenerateWordsButton
        // 
        this.GenerateWordsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.GenerateWordsButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.GenerateWordsButton.Image = ((System.Drawing.Image)(resources.GetObject("GenerateWordsButton.Image")));
        this.GenerateWordsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.GenerateWordsButton.Location = new System.Drawing.Point(478, 332);
        this.GenerateWordsButton.Name = "GenerateWordsButton";
        this.GenerateWordsButton.Size = new System.Drawing.Size(88, 21);
        this.GenerateWordsButton.TabIndex = 19;
        this.GenerateWordsButton.Text = "&Generate ";
        this.GenerateWordsButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        this.ToolTip.SetToolTip(this.GenerateWordsButton, resources.GetString("GenerateWordsButton.ToolTip"));
        this.GenerateWordsButton.UseVisualStyleBackColor = true;
        this.GenerateWordsButton.Click += new System.EventHandler(this.GenerateWordsButton_Click);
        // 
        // AddPositionsCheckBox
        // 
        this.AddPositionsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.AddPositionsCheckBox.AutoSize = true;
        this.AddPositionsCheckBox.Location = new System.Drawing.Point(159, 326);
        this.AddPositionsCheckBox.Name = "AddPositionsCheckBox";
        this.AddPositionsCheckBox.Size = new System.Drawing.Size(186, 17);
        this.AddPositionsCheckBox.TabIndex = 4;
        this.AddPositionsCheckBox.Text = "Add positions of letter/word/verse";
        this.ToolTip.SetToolTip(this.AddPositionsCheckBox, "Add letter, word and verse positions to each letter value");
        this.AddPositionsCheckBox.UseVisualStyleBackColor = true;
        this.AddPositionsCheckBox.CheckedChanged += new System.EventHandler(this.AddPositionsCheckBox_CheckedChanged);
        // 
        // WordCountLabel
        // 
        this.WordCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.WordCountLabel.AutoSize = true;
        this.WordCountLabel.ForeColor = System.Drawing.SystemColors.WindowText;
        this.WordCountLabel.Location = new System.Drawing.Point(570, 337);
        this.WordCountLabel.Name = "WordCountLabel";
        this.WordCountLabel.Size = new System.Drawing.Size(65, 13);
        this.WordCountLabel.TabIndex = 23;
        this.WordCountLabel.Text = "00000 Lines";
        this.WordCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.ToolTip.SetToolTip(this.WordCountLabel, "Lines generated");
        // 
        // NumberTypeLabel
        // 
        this.NumberTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.NumberTypeLabel.BackColor = System.Drawing.Color.Silver;
        this.NumberTypeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.NumberTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.NumberTypeLabel.ForeColor = System.Drawing.Color.Green;
        this.NumberTypeLabel.Location = new System.Drawing.Point(447, 334);
        this.NumberTypeLabel.Name = "NumberTypeLabel";
        this.NumberTypeLabel.Size = new System.Drawing.Size(25, 17);
        this.NumberTypeLabel.TabIndex = 12;
        this.NumberTypeLabel.Text = "P";
        this.NumberTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.ToolTip.SetToolTip(this.NumberTypeLabel, "allow prime concatenated letter values only");
        this.NumberTypeLabel.Click += new System.EventHandler(this.NumberTypeLabel_Click);
        // 
        // ValueCombinationDirectionLabel
        // 
        this.ValueCombinationDirectionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.ValueCombinationDirectionLabel.BackColor = System.Drawing.Color.Silver;
        this.ValueCombinationDirectionLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.ValueCombinationDirectionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ValueCombinationDirectionLabel.ForeColor = System.Drawing.SystemColors.WindowText;
        this.ValueCombinationDirectionLabel.Location = new System.Drawing.Point(420, 334);
        this.ValueCombinationDirectionLabel.Name = "ValueCombinationDirectionLabel";
        this.ValueCombinationDirectionLabel.Size = new System.Drawing.Size(25, 17);
        this.ValueCombinationDirectionLabel.TabIndex = 11;
        this.ValueCombinationDirectionLabel.Text = "←";
        this.ValueCombinationDirectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.ToolTip.SetToolTip(this.ValueCombinationDirectionLabel, "combine right to left");
        this.ValueCombinationDirectionLabel.Click += new System.EventHandler(this.ValueCombinationDirectionLabel_Click);
        // 
        // AddDistancesToPreviousCheckBox
        // 
        this.AddDistancesToPreviousCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.AddDistancesToPreviousCheckBox.AutoSize = true;
        this.AddDistancesToPreviousCheckBox.Location = new System.Drawing.Point(159, 341);
        this.AddDistancesToPreviousCheckBox.Name = "AddDistancesToPreviousCheckBox";
        this.AddDistancesToPreviousCheckBox.Size = new System.Drawing.Size(202, 17);
        this.AddDistancesToPreviousCheckBox.TabIndex = 5;
        this.AddDistancesToPreviousCheckBox.Text = "Add distances to previous letter/word";
        this.ToolTip.SetToolTip(this.AddDistancesToPreviousCheckBox, "Add letter and word distances to each letter value\r\nbackword to the previous same" +
    " letter and word");
        this.AddDistancesToPreviousCheckBox.UseVisualStyleBackColor = true;
        this.AddDistancesToPreviousCheckBox.CheckedChanged += new System.EventHandler(this.AddDistancesToPreviousCheckBox_CheckedChanged);
        // 
        // ValueInterlaceLabel
        // 
        this.ValueInterlaceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.ValueInterlaceLabel.BackColor = System.Drawing.Color.Silver;
        this.ValueInterlaceLabel.Cursor = System.Windows.Forms.Cursors.Hand;
        this.ValueInterlaceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ValueInterlaceLabel.ForeColor = System.Drawing.SystemColors.WindowText;
        this.ValueInterlaceLabel.Location = new System.Drawing.Point(391, 334);
        this.ValueInterlaceLabel.Name = "ValueInterlaceLabel";
        this.ValueInterlaceLabel.Size = new System.Drawing.Size(27, 17);
        this.ValueInterlaceLabel.TabIndex = 9;
        this.ValueInterlaceLabel.Text = "- -";
        this.ValueInterlaceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.ToolTip.SetToolTip(this.ValueInterlaceLabel, "concatenate letter values");
        this.ValueInterlaceLabel.Click += new System.EventHandler(this.ValueInterlaceLabel_Click);
        // 
        // AddVerseAndWordValuesCheckBox
        // 
        this.AddVerseAndWordValuesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.AddVerseAndWordValuesCheckBox.AutoSize = true;
        this.AddVerseAndWordValuesCheckBox.Location = new System.Drawing.Point(1, 326);
        this.AddVerseAndWordValuesCheckBox.Name = "AddVerseAndWordValuesCheckBox";
        this.AddVerseAndWordValuesCheckBox.Size = new System.Drawing.Size(155, 17);
        this.AddVerseAndWordValuesCheckBox.TabIndex = 3;
        this.AddVerseAndWordValuesCheckBox.Text = "Add verse and word values";
        this.ToolTip.SetToolTip(this.AddVerseAndWordValuesCheckBox, "Add letter\'s verse and word values to each letter value");
        this.AddVerseAndWordValuesCheckBox.UseVisualStyleBackColor = true;
        this.AddVerseAndWordValuesCheckBox.CheckedChanged += new System.EventHandler(this.AddVerseAndWordValuesCheckBox_CheckedChanged);
        // 
        // InspectButton
        // 
        this.InspectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.InspectButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.InspectButton.Image = ((System.Drawing.Image)(resources.GetObject("InspectButton.Image")));
        this.InspectButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.InspectButton.Location = new System.Drawing.Point(650, 331);
        this.InspectButton.Name = "InspectButton";
        this.InspectButton.Size = new System.Drawing.Size(23, 21);
        this.InspectButton.TabIndex = 91;
        this.InspectButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        this.ToolTip.SetToolTip(this.InspectButton, "View");
        this.InspectButton.UseVisualStyleBackColor = true;
        this.InspectButton.Click += new System.EventHandler(this.InspectButton_Click);
        // 
        // AutoGenerateWordsButton
        // 
        this.AutoGenerateWordsButton.Cursor = System.Windows.Forms.Cursors.Hand;
        this.AutoGenerateWordsButton.Image = ((System.Drawing.Image)(resources.GetObject("AutoGenerateWordsButton.Image")));
        this.AutoGenerateWordsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.AutoGenerateWordsButton.Location = new System.Drawing.Point(457, 0);
        this.AutoGenerateWordsButton.Name = "AutoGenerateWordsButton";
        this.AutoGenerateWordsButton.Size = new System.Drawing.Size(25, 23);
        this.AutoGenerateWordsButton.TabIndex = 89;
        this.AutoGenerateWordsButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        this.ToolTip.SetToolTip(this.AutoGenerateWordsButton, resources.GetString("AutoGenerateWordsButton.ToolTip"));
        this.AutoGenerateWordsButton.UseVisualStyleBackColor = true;
        this.AutoGenerateWordsButton.Click += new System.EventHandler(this.AutoGenerateWordsButton_Click);
        // 
        // YaHuseinCheckBox
        // 
        this.YaHuseinCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.YaHuseinCheckBox.AutoSize = true;
        this.YaHuseinCheckBox.Checked = true;
        this.YaHuseinCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        this.YaHuseinCheckBox.ForeColor = System.Drawing.Color.LightCoral;
        this.YaHuseinCheckBox.Location = new System.Drawing.Point(3, 3);
        this.YaHuseinCheckBox.Name = "YaHuseinCheckBox";
        this.YaHuseinCheckBox.Size = new System.Drawing.Size(135, 17);
        this.YaHuseinCheckBox.TabIndex = 107;
        this.YaHuseinCheckBox.Text = "Ya Husein letter values";
        this.ToolTip.SetToolTip(this.YaHuseinCheckBox, "Add/Subruct Ya Husein letter values from Al-Fatiha and the 14 Infallibles repecti" +
    "vely");
        this.YaHuseinCheckBox.UseVisualStyleBackColor = true;
        this.YaHuseinCheckBox.CheckedChanged += new System.EventHandler(this.YaHuseinCheckBox_CheckedChanged);
        // 
        // ProgressBar
        // 
        this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.ProgressBar.Location = new System.Drawing.Point(-1, 315);
        this.ProgressBar.Name = "ProgressBar";
        this.ProgressBar.Size = new System.Drawing.Size(679, 11);
        this.ProgressBar.TabIndex = 2;
        // 
        // NumerologySystemComboBox
        // 
        this.NumerologySystemComboBox.BackColor = System.Drawing.Color.MistyRose;
        this.NumerologySystemComboBox.DropDownHeight = 1024;
        this.NumerologySystemComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.NumerologySystemComboBox.FormattingEnabled = true;
        this.NumerologySystemComboBox.IntegralHeight = false;
        this.NumerologySystemComboBox.Location = new System.Drawing.Point(283, 1);
        this.NumerologySystemComboBox.Name = "NumerologySystemComboBox";
        this.NumerologySystemComboBox.Size = new System.Drawing.Size(176, 21);
        this.NumerologySystemComboBox.TabIndex = 0;
        this.NumerologySystemComboBox.SelectedIndexChanged += new System.EventHandler(this.NumerologySystemComboBox_SelectedIndexChanged);
        this.NumerologySystemComboBox.MouseHover += new System.EventHandler(this.NumerologySystemComboBox_MouseHover);
        // 
        // TextModeComboBox
        // 
        this.TextModeComboBox.BackColor = System.Drawing.Color.MistyRose;
        this.TextModeComboBox.DropDownHeight = 1024;
        this.TextModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.TextModeComboBox.FormattingEnabled = true;
        this.TextModeComboBox.IntegralHeight = false;
        this.TextModeComboBox.Items.AddRange(new object[] {
            "SimplifiedDots"});
        this.TextModeComboBox.Location = new System.Drawing.Point(201, 1);
        this.TextModeComboBox.Name = "TextModeComboBox";
        this.TextModeComboBox.Size = new System.Drawing.Size(84, 21);
        this.TextModeComboBox.TabIndex = 90;
        this.TextModeComboBox.SelectedIndexChanged += new System.EventHandler(this.TextModeComboBox_SelectedIndexChanged);
        // 
        // label1
        // 
        this.label1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label1.ForeColor = System.Drawing.Color.RoyalBlue;
        this.label1.Location = new System.Drawing.Point(1, 67);
        this.label1.Name = "label1";
        this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label1.Size = new System.Drawing.Size(672, 16);
        this.label1.TabIndex = 92;
        this.label1.Text = "ب س م ا ل ل ه ا ل ر ح م ن ا ل ر ح ي م ا ل ح م د ل ل ه ر ب ا ل ع ل م ي ن ا ل ر ح م" +
" ن ا ل ر ح ي م";
        // 
        // label2
        // 
        this.label2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label2.ForeColor = System.Drawing.Color.RoyalBlue;
        this.label2.Location = new System.Drawing.Point(1, 149);
        this.label2.Name = "label2";
        this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label2.Size = new System.Drawing.Size(672, 16);
        this.label2.TabIndex = 93;
        this.label2.Text = "م ل ك ي و م ا ل د ي ن ا ي ا ك ن ع ب د و ا ي ا ك ن س ت ع ي ن ا ه د ن ا ا ل ص ر ط ا" +
" ل م س ت ق ي م";
        // 
        // label3
        // 
        this.label3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label3.ForeColor = System.Drawing.Color.RoyalBlue;
        this.label3.Location = new System.Drawing.Point(1, 232);
        this.label3.Name = "label3";
        this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label3.Size = new System.Drawing.Size(672, 16);
        this.label3.TabIndex = 94;
        this.label3.Text = "ص ر ط ا ل ذ ي ن ا ن ع م ت ع ل ي ه م غ ي ر ا ل م غ ض و ب ع ل ي ه م و ل ا ا ل ض ا ل" +
" ي ن";
        // 
        // label4
        // 
        this.label4.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label4.ForeColor = System.Drawing.Color.SeaGreen;
        this.label4.Location = new System.Drawing.Point(2, 251);
        this.label4.Name = "label4";
        this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label4.Size = new System.Drawing.Size(672, 16);
        this.label4.TabIndex = 97;
        this.label4.Text = "ل ر ض ا م ح م د ا ل ج و ا د ع ل ي ا ل ه ا د ي ح س ن ا ل ع س ك ر ي م ح م د ا ل م ه" +
" د ي";
        // 
        // label5
        // 
        this.label5.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label5.ForeColor = System.Drawing.Color.SeaGreen;
        this.label5.Location = new System.Drawing.Point(2, 168);
        this.label5.Name = "label5";
        this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label5.Size = new System.Drawing.Size(672, 16);
        this.label5.TabIndex = 96;
        this.label5.Text = "ل ش ه ي د ع ل ي ا ل س ج ا د م ح م د ا ل ب ا ق ر ج ع ف ر ا ل ص ا د ق م و س ى ا ل ك" +
" ا ظ م ع ل ي ا";
        // 
        // label6
        // 
        this.label6.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label6.ForeColor = System.Drawing.Color.SeaGreen;
        this.label6.Location = new System.Drawing.Point(2, 86);
        this.label6.Name = "label6";
        this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label6.Size = new System.Drawing.Size(672, 16);
        this.label6.TabIndex = 95;
        this.label6.Text = "م ح م د ا ل م ص ط ف ى ع ل ي ا ل م ر ت ض ى ف ا ط م ة ا ل ز ه ر ا ء ح س ن ا ل م ج ت" +
" ب ى ح س ي ن ا";
        // 
        // label7
        // 
        this.label7.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label7.ForeColor = System.Drawing.Color.LightCoral;
        this.label7.Location = new System.Drawing.Point(2, 105);
        this.label7.Name = "label7";
        this.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label7.Size = new System.Drawing.Size(672, 16);
        this.label7.TabIndex = 98;
        this.label7.Text = "ي ي ي ي ي ي ي ي ي ي ا ح ح ح ح ح ح ح ح س س س س س س س س س س س س س س س س س س س س س س" +
" س س س س س س س";
        // 
        // label8
        // 
        this.label8.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label8.ForeColor = System.Drawing.Color.LightCoral;
        this.label8.Location = new System.Drawing.Point(2, 187);
        this.label8.Name = "label8";
        this.label8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label8.Size = new System.Drawing.Size(672, 16);
        this.label8.TabIndex = 99;
        this.label8.Text = "س س س س س س س س س س س س س س س س س س س س س س س س س س س س س س س ي ي ي ي ي ي ي ي ي ي" +
" ن ن ن ن ن ن ن";
        // 
        // label9
        // 
        this.label9.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label9.ForeColor = System.Drawing.Color.LightCoral;
        this.label9.Location = new System.Drawing.Point(2, 270);
        this.label9.Name = "label9";
        this.label9.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label9.Size = new System.Drawing.Size(672, 16);
        this.label9.TabIndex = 100;
        this.label9.Text = "ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن ن" +
" ن ن";
        // 
        // Line3Label
        // 
        this.Line3Label.BackColor = System.Drawing.Color.Silver;
        this.Line3Label.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.Line3Label.ForeColor = System.Drawing.Color.Black;
        this.Line3Label.Location = new System.Drawing.Point(2, 289);
        this.Line3Label.Name = "Line3Label";
        this.Line3Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.Line3Label.Size = new System.Drawing.Size(672, 16);
        this.Line3Label.TabIndex = 103;
        // 
        // Line2Label
        // 
        this.Line2Label.BackColor = System.Drawing.Color.Silver;
        this.Line2Label.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.Line2Label.ForeColor = System.Drawing.Color.Black;
        this.Line2Label.Location = new System.Drawing.Point(2, 206);
        this.Line2Label.Name = "Line2Label";
        this.Line2Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.Line2Label.Size = new System.Drawing.Size(672, 16);
        this.Line2Label.TabIndex = 102;
        // 
        // Line1Label
        // 
        this.Line1Label.BackColor = System.Drawing.Color.Silver;
        this.Line1Label.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.Line1Label.ForeColor = System.Drawing.Color.Black;
        this.Line1Label.Location = new System.Drawing.Point(2, 124);
        this.Line1Label.Name = "Line1Label";
        this.Line1Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.Line1Label.Size = new System.Drawing.Size(672, 16);
        this.Line1Label.TabIndex = 101;
        // 
        // label13
        // 
        this.label13.BackColor = System.Drawing.Color.Black;
        this.label13.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label13.ForeColor = System.Drawing.Color.LightSteelBlue;
        this.label13.Location = new System.Drawing.Point(462, 36);
        this.label13.Name = "label13";
        this.label13.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label13.Size = new System.Drawing.Size(212, 16);
        this.label13.TabIndex = 104;
        this.label13.Text = "سورة الفاتحة = 139 حرف ";
        this.label13.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // label14
        // 
        this.label14.BackColor = System.Drawing.Color.Black;
        this.label14.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label14.ForeColor = System.Drawing.Color.SpringGreen;
        this.label14.Location = new System.Drawing.Point(240, 36);
        this.label14.Name = "label14";
        this.label14.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label14.Size = new System.Drawing.Size(223, 16);
        this.label14.TabIndex = 105;
        this.label14.Text = " أسماء أهل البيت ع = 139 حرف ";
        this.label14.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // label15
        // 
        this.label15.BackColor = System.Drawing.Color.Black;
        this.label15.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label15.ForeColor = System.Drawing.Color.MistyRose;
        this.label15.Location = new System.Drawing.Point(2, 36);
        this.label15.Name = "label15";
        this.label15.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
        this.label15.Size = new System.Drawing.Size(239, 16);
        this.label15.TabIndex = 106;
        this.label15.Text = "يا حسين = 139 بنظام الجُمّل";
        this.label15.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // MainForm
        // 
        this.AcceptButton = this.GenerateWordsButton;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.SystemColors.Control;
        this.ClientSize = new System.Drawing.Size(677, 356);
        this.Controls.Add(this.ValueInterlaceLabel);
        this.Controls.Add(this.NumberTypeLabel);
        this.Controls.Add(this.ValueCombinationDirectionLabel);
        this.Controls.Add(this.WordCountLabel);
        this.Controls.Add(this.label15);
        this.Controls.Add(this.label14);
        this.Controls.Add(this.label13);
        this.Controls.Add(this.Line3Label);
        this.Controls.Add(this.Line2Label);
        this.Controls.Add(this.Line1Label);
        this.Controls.Add(this.label9);
        this.Controls.Add(this.label8);
        this.Controls.Add(this.label7);
        this.Controls.Add(this.label4);
        this.Controls.Add(this.label5);
        this.Controls.Add(this.label6);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.InspectButton);
        this.Controls.Add(this.AutoGenerateWordsButton);
        this.Controls.Add(this.NumerologySystemComboBox);
        this.Controls.Add(this.TextModeComboBox);
        this.Controls.Add(this.ProgressBar);
        this.Controls.Add(this.GenerateWordsButton);
        this.Controls.Add(this.YaHuseinCheckBox);
        this.Controls.Add(this.AddDistancesToPreviousCheckBox);
        this.Controls.Add(this.AddPositionsCheckBox);
        this.Controls.Add(this.AddVerseAndWordValuesCheckBox);
        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        this.Name = "MainForm";
        this.Text = "Ahlul-Bayt";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
        this.Load += new System.EventHandler(this.MainForm_Load);
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button GenerateWordsButton;
    private System.Windows.Forms.CheckBox AddPositionsCheckBox;
    private System.Windows.Forms.Label WordCountLabel;
    private System.Windows.Forms.Label NumberTypeLabel;
    private System.Windows.Forms.ToolTip ToolTip;
    private System.Windows.Forms.Label ValueCombinationDirectionLabel;
    private System.Windows.Forms.CheckBox AddDistancesToPreviousCheckBox;
    private System.Windows.Forms.ProgressBar ProgressBar;
    private System.Windows.Forms.ComboBox NumerologySystemComboBox;
    private System.Windows.Forms.Label ValueInterlaceLabel;
    private System.Windows.Forms.Button AutoGenerateWordsButton;
    private System.Windows.Forms.CheckBox AddVerseAndWordValuesCheckBox;
    private System.Windows.Forms.ComboBox TextModeComboBox;
    private System.Windows.Forms.Button InspectButton;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label Line3Label;
    private System.Windows.Forms.Label Line2Label;
    private System.Windows.Forms.Label Line1Label;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.CheckBox YaHuseinCheckBox;
}
