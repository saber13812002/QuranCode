using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Threading;
using Model;

public partial class MainForm : Form
{
    private Client m_client = null;
    private string m_numerology_system_name = null;
    private List<Letter> m_fatiha_letters = null;
    private string m_infallible_letters = null;
    private bool m_use_ya_husein = true;
    private string m_ya_husein_letters = null;
    private long[] m_ya_husein_letter_values = null;
    private List<string> m_generated_lines = null;

    public MainForm()
    {
        InitializeComponent();

        using (Graphics graphics = this.CreateGraphics())
        {
            // 100% = 96.0F,   125% = 120.0F,   150% = 144.0F
            if (graphics.DpiX == 96.0F)
            {
                this.AutoGenerateWordsButton.Size = new System.Drawing.Size(25, 23);
            }
            else if (graphics.DpiX == 120.0F)
            {
                this.AutoGenerateWordsButton.Size = new System.Drawing.Size(27, 25);
            }
            else if (graphics.DpiX == 144.0F)
            {
                //this.AutoGenerateWordsButton.Size = new System.Drawing.Size(27, 25);
            }
        }
    }
    private void MainForm_Load(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            m_client = new Client(NumerologySystem.DEFAULT_NAME);
            if (m_client != null)
            {
                if (m_client.NumerologySystem != null)
                {
                    m_numerology_system_name = m_client.NumerologySystem.Name;

                    string text_mode = m_client.NumerologySystem.TextMode;
                    PopulateTextModeComboBox();
                    if (TextModeComboBox.Items.Count > 0)
                    {
                        if (TextModeComboBox.Items.Contains(text_mode))
                        {
                            TextModeComboBox.SelectedItem = text_mode;
                        }
                        else
                        {
                            TextModeComboBox.SelectedIndex = 0;
                        }
                    }

                    m_client.BuildSimplifiedBook(text_mode, true, false, false, false);
                    if (m_client.Book != null)
                    {
                        if (m_client.Book.Chapters != null)
                        {
                            if (m_client.Book.Chapters.Count > 0)
                            {
                                List<Verse> fatiha_verses = m_client.Book.Chapters[0].Verses;
                                if (fatiha_verses != null)
                                {
                                    List<Word> fatiha_words = new List<Word>();
                                    foreach (Verse fatiha_verse in fatiha_verses)
                                    {
                                        fatiha_words.AddRange(fatiha_verse.Words);
                                    }

                                    if (fatiha_words != null)
                                    {
                                        m_fatiha_letters = new List<Letter>();
                                        foreach (Word fatiha_word in fatiha_words)
                                        {
                                            m_fatiha_letters.AddRange(fatiha_word.Letters);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    m_infallible_letters = "محمدالمصطفىعليالمرتضىفاطمةالزهراءحسنالمجتبىحسينالشهيدعليالسجادمحمدالباقرجعفرالصادقموسىالكاظمعليالرضامحمدالجوادعليالهاديحسنالعسكريمحمدالمهدي";

                    m_ya_husein_letters = "يييييييييياححححححححسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسسيييييييييينننننننننننننننننننننننننننننننننننننننننننننننننن";
                    m_ya_husein_letter_values = new long[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 1, 8, 8, 8, 8, 8, 8, 8, 8, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60, 60 };

                    m_generated_lines = new List<string>();
                }
            }

            m_number_type = NumberType.Prime;
            NumberTypeLabel.Text = "P";
            NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
            ToolTip.SetToolTip(ValueInterlaceLabel, "concatenate letter values");
            ToolTip.SetToolTip(ValueCombinationDirectionLabel, "combine letter values right to left");
            ToolTip.SetToolTip(NumberTypeLabel, "allow prime combined letter values only");
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Application.Exit(); // doesn't close WinForm application immediately
        try
        {
            Environment.Exit(0); // close Console and WinForms applications immediately
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
    }
    private void PopulateTextModeComboBox()
    {
        try
        {
            TextModeComboBox.SelectedIndexChanged -= new EventHandler(TextModeComboBox_SelectedIndexChanged);

            if (m_client != null)
            {
                if (m_client.LoadedNumerologySystems != null)
                {
                    TextModeComboBox.BeginUpdate();

                    TextModeComboBox.Items.Clear();
                    foreach (NumerologySystem numerology_system in m_client.LoadedNumerologySystems.Values)
                    {
                        string[] parts = numerology_system.Name.Split('_');
                        if (parts != null)
                        {
                            if (parts.Length == 3)
                            {
                                string text_mode = parts[0];
                                if (text_mode == "Original") continue;

                                if (!TextModeComboBox.Items.Contains(text_mode))
                                {
                                    TextModeComboBox.Items.Add(text_mode);
                                }
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            TextModeComboBox.EndUpdate();
            TextModeComboBox.SelectedIndexChanged += new EventHandler(TextModeComboBox_SelectedIndexChanged);
        }
    }
    private void PopulateNumerologySystemComboBox()
    {
        try
        {
            NumerologySystemComboBox.SelectedIndexChanged -= new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);

            if (m_client != null)
            {
                if (m_client.LoadedNumerologySystems != null)
                {
                    NumerologySystemComboBox.BeginUpdate();

                    if (TextModeComboBox.SelectedItem != null)
                    {
                        string text_mode = TextModeComboBox.SelectedItem.ToString();

                        NumerologySystemComboBox.Items.Clear();
                        foreach (NumerologySystem numerology_system in m_client.LoadedNumerologySystems.Values)
                        {
                            if (numerology_system.Name.Contains("Zero")) continue;

                            string[] parts = numerology_system.Name.Split('_');
                            if (parts != null)
                            {
                                if (parts.Length == 3)
                                {
                                    if (parts[0] == text_mode)
                                    {
                                        string valuation_system = parts[1] + "_" + parts[2];
                                        if (!NumerologySystemComboBox.Items.Contains(valuation_system))
                                        {
                                            NumerologySystemComboBox.Items.Add(valuation_system);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            NumerologySystemComboBox.EndUpdate();
            NumerologySystemComboBox.SelectedIndexChanged += new EventHandler(NumerologySystemComboBox_SelectedIndexChanged);
        }
    }
    private void TextModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.NumerologySystem != null)
            {
                PopulateNumerologySystemComboBox();
                if (NumerologySystemComboBox.Items.Count > 0)
                {
                    int pos = m_client.NumerologySystem.Name.IndexOf("_");
                    string default_letter_valuation = m_client.NumerologySystem.Name.Substring(pos + 1);
                    if (NumerologySystemComboBox.Items.Contains(default_letter_valuation))
                    {
                        NumerologySystemComboBox.SelectedItem = default_letter_valuation;
                    }
                    else
                    {
                        NumerologySystemComboBox.SelectedIndex = 0;
                    }
                }
                else
                {
                    NumerologySystemComboBox.SelectedIndex = -1;
                }
            }
        }
    }
    private void NumerologySystemComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        m_numerology_system_name = TextModeComboBox.SelectedItem.ToString() + "_" + NumerologySystemComboBox.SelectedItem.ToString();
        if (m_client != null)
        {
            m_client.LoadNumerologySystem(m_numerology_system_name);
            UpdateNumerologySystem();
        }
    }
    private void NumerologySystemComboBox_MouseHover(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.NumerologySystem != null)
            {
                StringBuilder str = new StringBuilder();
                foreach (char c in m_client.NumerologySystem.Keys)
                {
                    str.AppendLine(c.ToString() + "\t" + m_client.NumerologySystem[c].ToString());
                }
                ToolTip.SetToolTip(NumerologySystemComboBox, str.ToString());
            }
        }
    }
    private void UpdateNumerologySystem()
    {
        if (m_client.NumerologySystem != null)
        {
            m_client.NumerologySystem.AddToLetterLNumber = m_add_positions_to_letter_value;
            m_client.NumerologySystem.AddToLetterWNumber = m_add_positions_to_letter_value;
            m_client.NumerologySystem.AddToLetterVNumber = m_add_positions_to_letter_value;
            m_client.NumerologySystem.AddToLetterCNumber = false;
            m_client.NumerologySystem.AddToLetterLDistance = true;
            m_client.NumerologySystem.AddToLetterWDistance = true;
            m_client.NumerologySystem.AddToLetterVDistance = true;
            m_client.NumerologySystem.AddToLetterCDistance = false;
            m_client.NumerologySystem.AddToWordWNumber = m_add_positions_to_letter_value;
            m_client.NumerologySystem.AddToWordVNumber = m_add_positions_to_letter_value;
            m_client.NumerologySystem.AddToWordCNumber = false;
            m_client.NumerologySystem.AddToWordWDistance = true;
            m_client.NumerologySystem.AddToWordVDistance = true;
            m_client.NumerologySystem.AddToWordCDistance = false;
            m_client.NumerologySystem.AddToVerseVNumber = m_add_positions_to_letter_value;
            m_client.NumerologySystem.AddToVerseCNumber = false;
            m_client.NumerologySystem.AddToVerseVDistance = true;
            m_client.NumerologySystem.AddToVerseCDistance = false;
            m_client.NumerologySystem.AddToChapterCNumber = false;

            m_client.NumerologySystem.AddDistancesWithinChapters = true;
            if (m_client.Book != null)
            {
                m_client.Book.SetupDistances(m_client.NumerologySystem.AddDistancesWithinChapters);
            }
        }
    }
    private bool m_add_verse_and_word_values_to_letter_value = false;
    private bool m_add_positions_to_letter_value = false;
    private bool m_add_distances_to_previous_to_letter_value = false;
    private void AddVerseAndWordValuesCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_add_verse_and_word_values_to_letter_value = AddVerseAndWordValuesCheckBox.Checked;
    }
    private void AddPositionsCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_add_positions_to_letter_value = AddPositionsCheckBox.Checked;
        UpdateNumerologySystem();
    }
    private void AddDistancesToPreviousCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_add_distances_to_previous_to_letter_value = AddDistancesToPreviousCheckBox.Checked;
        UpdateNumerologySystem();
    }
    private void YaHuseinCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_use_ya_husein = YaHuseinCheckBox.Checked;
        label7.Visible = m_use_ya_husein;
        label8.Visible = m_use_ya_husein;
        label9.Visible = m_use_ya_husein;
    }

    private enum CombinationMethod { Concatenate, InterlaceAB, InterlaceBA, CrossOverAB, CrossOverBA };
    private CombinationMethod m_combination_method = CombinationMethod.Concatenate;
    private Direction m_value_combination_direction = Direction.RightToLeft;
    private NumberType m_number_type = NumberType.Prime;
    private void ValueInterlaceLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            GoToPreviousValueCombinationType();
        }
        else
        {
            GoToNextValueCombinationType();
        }
    }
    private void GoToPreviousValueCombinationType()
    {
        switch (m_combination_method)
        {
            case CombinationMethod.CrossOverBA:
                {
                    m_combination_method = CombinationMethod.CrossOverAB;
                    ValueInterlaceLabel.Text = "aXb";
                    ToolTip.SetToolTip(ValueInterlaceLabel, "corssover digits of letter values, aabbaaabb");
                }
                break;
            case CombinationMethod.CrossOverAB:
                {
                    m_combination_method = CombinationMethod.InterlaceBA;
                    ValueInterlaceLabel.Text = "b§a";
                    ToolTip.SetToolTip(ValueInterlaceLabel, "interlace digits of letter values, babababaa");
                }
                break;
            case CombinationMethod.InterlaceBA:
                {
                    m_combination_method = CombinationMethod.InterlaceAB;
                    ValueInterlaceLabel.Text = "a§b";
                    ToolTip.SetToolTip(ValueInterlaceLabel, "interlace digits of letter values, ababababa");
                }
                break;
            case CombinationMethod.InterlaceAB:
                {
                    m_combination_method = CombinationMethod.Concatenate;
                    ValueInterlaceLabel.Text = "- -";
                    ToolTip.SetToolTip(ValueInterlaceLabel, "concatenate letter values, aaaaabbbb");
                }
                break;
            case CombinationMethod.Concatenate:
                {
                    m_combination_method = CombinationMethod.CrossOverBA;
                    ValueInterlaceLabel.Text = "bXa";
                    ToolTip.SetToolTip(ValueInterlaceLabel, "crossover digits of letter values, bbaabbaaa");
                }
                break;
        }
        ValueInterlaceLabel.Refresh();
    }
    private void GoToNextValueCombinationType()
    {
        switch (m_combination_method)
        {
            case CombinationMethod.Concatenate:
                {
                    m_combination_method = CombinationMethod.InterlaceAB;
                    ValueInterlaceLabel.Text = "a§b";
                    ToolTip.SetToolTip(ValueInterlaceLabel, "interlace digits of letter values, ababababa");
                }
                break;
            case CombinationMethod.InterlaceAB:
                {
                    m_combination_method = CombinationMethod.InterlaceBA;
                    ValueInterlaceLabel.Text = "b§a";
                    ToolTip.SetToolTip(ValueInterlaceLabel, "interlace digits of letter values, babababaa");
                }
                break;
            case CombinationMethod.InterlaceBA:
                {
                    m_combination_method = CombinationMethod.CrossOverAB;
                    ValueInterlaceLabel.Text = "aXb";
                    ToolTip.SetToolTip(ValueInterlaceLabel, "corssover digits of letter values, aabbaaabb");
                }
                break;
            case CombinationMethod.CrossOverAB:
                {
                    m_combination_method = CombinationMethod.CrossOverBA;
                    ValueInterlaceLabel.Text = "bXa";
                    ToolTip.SetToolTip(ValueInterlaceLabel, "crossover digits of letter values, bbaabbaaa");
                }
                break;
            case CombinationMethod.CrossOverBA:
                {
                    m_combination_method = CombinationMethod.Concatenate;
                    ValueInterlaceLabel.Text = "- -";
                    ToolTip.SetToolTip(ValueInterlaceLabel, "concatenate letter values, aaaaabbbb");
                }
                break;
        }
        ValueInterlaceLabel.Refresh();
    }
    private void ValueCombinationDirectionLabel_Click(object sender, EventArgs e)
    {
        if (m_value_combination_direction == Direction.RightToLeft)
        {
            m_value_combination_direction = Direction.LeftToRight;
            ValueCombinationDirectionLabel.Text = "→";
            ToolTip.SetToolTip(ValueCombinationDirectionLabel, "combine letter values left to right");
        }
        else
        {
            m_value_combination_direction = Direction.RightToLeft;
            ValueCombinationDirectionLabel.Text = "←";
            ToolTip.SetToolTip(ValueCombinationDirectionLabel, "combine letter values right to left");
        }
        ValueCombinationDirectionLabel.Refresh();
    }
    private void NumberTypeLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            GotoPreviousNumberType();
        }
        else
        {
            GotoNextNumberType();
        }
    }
    private void GotoPreviousNumberType()
    {
        switch (m_number_type)
        {
            case NumberType.NonAdditiveComposite:
                {
                    m_number_type = NumberType.AdditiveComposite;
                    NumberTypeLabel.Text = "AC";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(114L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow additive composite combined letter values only");
                }
                break;
            case NumberType.AdditiveComposite:
                {
                    m_number_type = NumberType.Composite;
                    NumberTypeLabel.Text = "C";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(14L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow composite combined letter values only");
                }
                break;
            case NumberType.Composite:
                {
                    m_number_type = NumberType.NonAdditivePrime;
                    NumberTypeLabel.Text = "XP";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow non-additive prime combined letter values only");
                }
                break;
            case NumberType.NonAdditivePrime:
                {
                    m_number_type = NumberType.AdditivePrime;
                    NumberTypeLabel.Text = "AP";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(47L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow additive prime combined letter values only");
                }
                break;
            case NumberType.AdditivePrime:
                {
                    m_number_type = NumberType.Prime;
                    NumberTypeLabel.Text = "P";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow prime combined letter values only");
                }
                break;
            case NumberType.Prime:
                {
                    m_number_type = NumberType.Natural;
                    NumberTypeLabel.Text = "";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(0L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow all combined letter values only");
                }
                break;
            case NumberType.Natural:
                {
                    m_number_type = NumberType.NonAdditiveComposite;
                    NumberTypeLabel.Text = "XC";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(12L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow non-additive composite combined letter values only");
                }
                break;
        }
        NumberTypeLabel.Refresh();
    }
    private void GotoNextNumberType()
    {
        switch (m_number_type)
        {
            case NumberType.Natural:
                {
                    m_number_type = NumberType.Prime;
                    NumberTypeLabel.Text = "P";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow prime combined letter values only");
                }
                break;
            case NumberType.Prime:
                {
                    m_number_type = NumberType.AdditivePrime;
                    NumberTypeLabel.Text = "AP";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(47L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow additive prime combined letter values only");
                }
                break;
            case NumberType.AdditivePrime:
                {
                    m_number_type = NumberType.NonAdditivePrime;
                    NumberTypeLabel.Text = "XP";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow non-additive prime combined letter values only");
                }
                break;
            case NumberType.NonAdditivePrime:
                {
                    m_number_type = NumberType.Composite;
                    NumberTypeLabel.Text = "C";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(14L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow composite combined letter values only");
                }
                break;
            case NumberType.Composite:
                {
                    m_number_type = NumberType.AdditiveComposite;
                    NumberTypeLabel.Text = "AC";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(114L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow additive composite combined letter values only");
                }
                break;
            case NumberType.AdditiveComposite:
                {
                    m_number_type = NumberType.NonAdditiveComposite;
                    NumberTypeLabel.Text = "XC";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(25L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow non-additive composite combined letter values only");
                }
                break;
            case NumberType.NonAdditiveComposite:
                {
                    m_number_type = NumberType.Natural;
                    NumberTypeLabel.Text = "";
                    NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(0L);
                    ToolTip.SetToolTip(NumberTypeLabel, "allow all combined letter values only");
                }
                break;
        }
        NumberTypeLabel.Refresh();
    }

    private void GenerateWordsButton_Click(object sender, EventArgs e)
    {
        TextModeComboBox.Enabled = false;
        NumerologySystemComboBox.Enabled = false;
        AddVerseAndWordValuesCheckBox.Enabled = false;
        AddPositionsCheckBox.Enabled = false;
        AddDistancesToPreviousCheckBox.Enabled = false;
        ValueCombinationDirectionLabel.Enabled = false;
        NumberTypeLabel.Enabled = false;
        AutoGenerateWordsButton.Enabled = false;
        GenerateWordsButton.Enabled = false;
        InspectButton.Enabled = false;
        TextModeComboBox.Refresh();
        NumerologySystemComboBox.Refresh();
        AddVerseAndWordValuesCheckBox.Refresh();
        AddPositionsCheckBox.Refresh();
        AddDistancesToPreviousCheckBox.Refresh();
        ValueCombinationDirectionLabel.Refresh();
        NumberTypeLabel.Refresh();
        AutoGenerateWordsButton.Refresh();
        GenerateWordsButton.Refresh();
        InspectButton.Refresh();

        this.Cursor = Cursors.WaitCursor;
        try
        {
            // at the start of running
            ProgressBar.Value = 0;
            ProgressBar.Refresh();
            WordCountLabel.Text = "0 Lines";
            WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(0L);
            WordCountLabel.Refresh();

            if (m_generated_lines == null)
            {
                m_generated_lines = new List<string>();
            }
            if (m_generated_lines != null)
            {
                m_generated_lines.Clear();
            }

            if (ModifierKeys == Keys.Shift)
            {
                int loops = 0;
                foreach (string text_mode in TextModeComboBox.Items)
                {
                    TextModeComboBox.SelectedItem = text_mode;
                    TextModeComboBox.Refresh();

                    loops += NumerologySystemComboBox.Items.Count;
                }

                int i = 0;
                foreach (string text_mode in TextModeComboBox.Items)
                {
                    TextModeComboBox.SelectedItem = text_mode;
                    TextModeComboBox.Refresh();

                    foreach (string numerology_system in NumerologySystemComboBox.Items)
                    {
                        NumerologySystemComboBox.SelectedItem = numerology_system;
                        NumerologySystemComboBox.Refresh();

                        GenerateLine();

                        // display progress
                        i++;
                        ProgressBar.Value = (i * 100) / loops;
                        if (m_generated_lines != null)
                        {
                            WordCountLabel.Text = m_generated_lines.Count + " Line" + (m_generated_lines.Count == 1 ? "" : "s");
                            WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(m_generated_lines.Count);
                            WordCountLabel.Refresh();
                        }
                        Application.DoEvents();

                    } // foreach NumerologySystem

                } // foreach TextMode
            }
            else
            {
                GenerateLine();
            }

            // at the end of running
            ProgressBar.Value = 100;
            ProgressBar.Refresh();
            if (m_generated_lines != null)
            {
                WordCountLabel.Text = m_generated_lines.Count + " Line" + (m_generated_lines.Count == 1 ? "" : "s");
                WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(m_generated_lines.Count);
                WordCountLabel.Refresh();
            }
        }
        finally
        {
            TextModeComboBox.Enabled = true;
            NumerologySystemComboBox.Enabled = true;
            AddVerseAndWordValuesCheckBox.Enabled = true;
            AddPositionsCheckBox.Enabled = true;
            AddDistancesToPreviousCheckBox.Enabled = true;
            ValueCombinationDirectionLabel.Enabled = true;
            NumberTypeLabel.Enabled = true;
            AutoGenerateWordsButton.Enabled = true;
            GenerateWordsButton.Enabled = true;
            InspectButton.Enabled = true;

            this.Cursor = Cursors.Default;
        }
    }
    private void AutoGenerateWordsButton_Click(object sender, EventArgs e)
    {
        TextModeComboBox.Enabled = false;
        NumerologySystemComboBox.Enabled = false;
        AddVerseAndWordValuesCheckBox.Enabled = false;
        AddPositionsCheckBox.Enabled = false;
        AddDistancesToPreviousCheckBox.Enabled = false;
        ValueCombinationDirectionLabel.Enabled = false;
        NumberTypeLabel.Enabled = false;
        AutoGenerateWordsButton.Enabled = false;
        GenerateWordsButton.Enabled = false;
        InspectButton.Enabled = false;
        TextModeComboBox.Refresh();
        NumerologySystemComboBox.Refresh();
        AddVerseAndWordValuesCheckBox.Refresh();
        AddPositionsCheckBox.Refresh();
        AddDistancesToPreviousCheckBox.Refresh();
        ValueCombinationDirectionLabel.Refresh();
        NumberTypeLabel.Refresh();
        AutoGenerateWordsButton.Refresh();
        GenerateWordsButton.Refresh();
        InspectButton.Refresh();

        this.Cursor = Cursors.WaitCursor;
        try
        {
            // at the start of running
            ProgressBar.Value = 0;
            ProgressBar.Refresh();
            WordCountLabel.Text = "0 Lines";
            WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(0L);
            WordCountLabel.Refresh();

            if (m_generated_lines == null)
            {
                m_generated_lines = new List<string>();
            }
            if (m_generated_lines != null)
            {
                m_generated_lines.Clear();
            }

            if (ModifierKeys == Keys.Shift)
            {
                int loops = 0;
                foreach (string text_mode in TextModeComboBox.Items)
                {
                    TextModeComboBox.SelectedItem = text_mode;
                    TextModeComboBox.Refresh();

                    loops += NumerologySystemComboBox.Items.Count;
                }

                int i = 0;
                foreach (string text_mode in TextModeComboBox.Items)
                {
                    TextModeComboBox.SelectedItem = text_mode;
                    TextModeComboBox.Refresh();

                    foreach (string numerology_system in NumerologySystemComboBox.Items)
                    {
                        NumerologySystemComboBox.SelectedItem = numerology_system;
                        NumerologySystemComboBox.Refresh();

                        ProcessNumerologySystem();

                        // display progress
                        i++;
                        ProgressBar.Value = ((i + 1) * 100) / loops;
                        if (m_generated_lines != null)
                        {
                            WordCountLabel.Text = m_generated_lines.Count + " Line" + (m_generated_lines.Count == 1 ? "" : "s");
                            WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(m_generated_lines.Count);
                            WordCountLabel.Refresh();
                        }
                        Application.DoEvents();

                    } // foreach NumerologySystem

                } // foreach TextMode
            }
            else
            {
                ProcessNumerologySystem();
            }

            // at the end of running
            ProgressBar.Value = 100;
            ProgressBar.Refresh();
            if (m_generated_lines != null)
            {
                WordCountLabel.Text = m_generated_lines.Count + " Line" + (m_generated_lines.Count == 1 ? "" : "s");
                WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(m_generated_lines.Count);
                WordCountLabel.Refresh();
            }
        }
        finally
        {
            TextModeComboBox.Enabled = true;
            NumerologySystemComboBox.Enabled = true;
            AddVerseAndWordValuesCheckBox.Enabled = true;
            AddPositionsCheckBox.Enabled = true;
            AddDistancesToPreviousCheckBox.Enabled = true;
            ValueCombinationDirectionLabel.Enabled = true;
            NumberTypeLabel.Enabled = true;
            AutoGenerateWordsButton.Enabled = true;
            GenerateWordsButton.Enabled = true;
            InspectButton.Enabled = true;

            this.Cursor = Cursors.Default;
        }
    }
    private void ProcessNumerologySystem()
    {
        if (m_generated_lines != null)
        {
            if (m_client != null)
            {
                if (m_client.NumerologySystem != null)
                {
                    // prepare for next state
                    m_combination_method = CombinationMethod.CrossOverBA;
                    m_value_combination_direction = Direction.LeftToRight;
                    m_number_type = NumberType.NonAdditiveComposite;

                    for (int h = 0; h < 2; h++)
                    {
                        AddVerseAndWordValuesCheckBox.Checked = (h == 1);
                        for (int k = 0; k < 2; k++)
                        {
                            AddPositionsCheckBox.Checked = (k == 1);
                            for (int l = 0; l < 2; l++)
                            {
                                AddDistancesToPreviousCheckBox.Checked = (l == 1);
                                for (int n = 0; n < 5; n++)
                                {
                                    ValueInterlaceLabel_Click(null, null);
                                    for (int o = 0; o < 2; o++)
                                    {
                                        ValueCombinationDirectionLabel_Click(null, null);
                                        for (int p = 0; p < 6; p++)
                                        {
                                            GotoNextNumberType();
                                            // skip Natural type
                                            if (m_number_type == NumberType.Natural) // skip natural type
                                            {
                                                GotoNextNumberType();
                                            }

                                            GenerateLine();

                                        } // for NumberType
                                    } // for Direction
                                } // for Combination
                            } // for AddDistancesToPrevious
                        } // for AddPositions
                    } // for AddVerseAndWordValues
                }
            }
        }
    }
    private void GenerateLine()
    {
        if (m_generated_lines != null)
        {
            if (m_fatiha_letters != null)
            {
                List<long> fatiha_letter_values = new List<long>();
                List<long> infallible_letter_values = new List<long>();
                for (int i = 0; i < m_fatiha_letters.Count; i++)
                {
                    long value = m_client.CalculateValue(m_fatiha_letters[i]);
                    if (m_add_verse_and_word_values_to_letter_value)
                    {
                        value += m_client.CalculateValue(m_fatiha_letters[i].Word);
                        value += m_client.CalculateValue(m_fatiha_letters[i].Word.Verse);
                    }
                    if (m_use_ya_husein) value += m_ya_husein_letter_values[i];
                    fatiha_letter_values.Add(value);

                    value = m_client.CalculateValueUserText(m_infallible_letters[i]);
                    if (m_use_ya_husein) value -= m_ya_husein_letter_values[i];
                    infallible_letter_values.Add(Math.Abs(value));
                }

                StringBuilder str = new StringBuilder();
                if (m_client.NumerologySystem != null)
                {
                    string generated_line = "";
                    if (fatiha_letter_values != null)
                    {
                        for (int j = 0; j < fatiha_letter_values.Count; j++)
                        {
                            long number = 0L;
                            long AAA = fatiha_letter_values[j];
                            long BBB = infallible_letter_values[j];
                            switch (m_combination_method)
                            {
                                case CombinationMethod.Concatenate:
                                    number = Numbers.Concatenate(AAA, BBB, m_value_combination_direction);
                                    break;
                                case CombinationMethod.InterlaceAB:
                                    number = Numbers.Interlace(AAA, BBB, true, m_value_combination_direction);
                                    break;
                                case CombinationMethod.InterlaceBA:
                                    number = Numbers.Interlace(AAA, BBB, false, m_value_combination_direction);
                                    break;
                                case CombinationMethod.CrossOverAB:
                                    number = Numbers.CrossOver(AAA, BBB, true, m_value_combination_direction);
                                    break;
                                case CombinationMethod.CrossOverBA:
                                    number = Numbers.CrossOver(AAA, BBB, false, m_value_combination_direction);
                                    break;
                            }

                            if (number != -1)
                            {
                                if (Numbers.IsNumberType(number, m_number_type))
                                {
                                    // mod 29 to select letter
                                    int i = (int)((long)number % (long)m_client.NumerologySystem.Count);
                                    char[] letters = new char[m_client.NumerologySystem.LetterValues.Count];
                                    m_client.NumerologySystem.LetterValues.Keys.CopyTo(letters, 0);
                                    generated_line += letters[i] + " ";
                                }
                                else
                                {
                                    generated_line += "  ";
                                }
                            }
                            else
                            {
                                generated_line += "  ";
                            }
                            generated_line.Remove(generated_line.Length - 1, 1);
                        }

                        Line1Label.Text = generated_line.Substring(0, 96);
                        Line2Label.Text = generated_line.Substring(96, 96);
                        Line3Label.Text = generated_line.Substring(192);

                        string parameters =
                        m_numerology_system_name + "_"
                        + (m_add_verse_and_word_values_to_letter_value ? "vw" : "__")
                        + (m_add_positions_to_letter_value ? "_n" : "__")
                        + (m_add_distances_to_previous_to_letter_value ? "_d" : "_-_")
                        + ("_" + m_combination_method.ToString().ToLower())
                        + ((m_value_combination_direction == Direction.RightToLeft) ? "_r" : "_l")
                        + ((m_number_type != NumberType.None) ? "_" : "")
                        + (
                            (m_number_type == NumberType.Prime) ? "P" :
                            (m_number_type == NumberType.AdditivePrime) ? "AP" :
                            (m_number_type == NumberType.NonAdditivePrime) ? "XP" :
                            (m_number_type == NumberType.Composite) ? "C" :
                            (m_number_type == NumberType.AdditiveComposite) ? "AC" :
                            (m_number_type == NumberType.NonAdditiveComposite) ? "XC" : ""
                            )
                        ;
                        m_generated_lines.Add(generated_line + "\t" + parameters);
                    }
                }
            }
        }
    }
    private void InspectButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            StringBuilder str = new StringBuilder();
            if (m_generated_lines != null)
            {
                if (m_generated_lines.Count > 0)
                {
                    for (int i = 0; i < m_generated_lines.Count; i++)
                    {
                        string xxx = m_generated_lines[i];
                        if (xxx != null)
                        {
                            while (xxx.Contains("  "))
                            {
                                xxx = xxx.Replace("  ", " ");
                            }
                            str.AppendLine(xxx);
                        }
                    }
                    str.AppendLine();
                }
            }

            string filename = "AlFatiha_AhlulBayt" + (m_use_ya_husein ? "_YaHusein" : "") + ".txt";
            if (!Directory.Exists(Globals.STATISTICS_FOLDER))
            {
                Directory.CreateDirectory(Globals.STATISTICS_FOLDER);
            }
            string folder = Globals.STATISTICS_FOLDER + "/" + m_numerology_system_name;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (Directory.Exists(Globals.STATISTICS_FOLDER))
            {
                string path = folder + "/" + filename;
                FileHelper.SaveText(path, str.ToString());

                if (sender == InspectButton)
                {
                    FileHelper.DisplayFile(path);
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
}
