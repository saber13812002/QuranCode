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
    // Use Simplified29 ONLY because Surat Al-Fatiha is built upon pattern 7-29
    private string m_numerology_system_name = "Simplified29_Alphabet_Primes1";

    private Client m_client = null;
    private List<List<Word>> m_word_subsets = null;
    private List<Line> m_lines = null;

    public MainForm()
    {
        InitializeComponent();

        using (Graphics graphics = this.CreateGraphics())
        {
            // 100% = 96.0F,   125% = 120.0F,   150% = 144.0F
            if (graphics.DpiX == 96.0F)
            {
                this.IdColumnHeader.Width = 55;
                this.SentenceColumnHeader.Width = 385;
                this.ValueColumnHeader.Width = 94;
                this.WordColumnHeader.Width = 110;
                this.AutoGenerateWordsButton.Size = new System.Drawing.Size(25, 23);
            }
            else if (graphics.DpiX == 120.0F)
            {
                this.IdColumnHeader.Width = 70;
                this.SentenceColumnHeader.Width = 510;
                this.ValueColumnHeader.Width = 114;
                this.WordColumnHeader.Width = 165;
                this.AutoGenerateWordsButton.Size = new System.Drawing.Size(27, 25);
            }
            else if (graphics.DpiX == 144.0F)
            {
                //this.IdColumnHeader.Width = 70;
                //this.SentenceColumnHeader.Width = 510;
                //this.ValueColumnHeader.Width = 114;
                //this.WordColumnHeader.Width = 165;
                //this.AutoGenerateWordsButton.Size = new System.Drawing.Size(27, 25);
            }
        }
    }
    private void MainForm_Load(object sender, EventArgs e)
    {
        if (m_client == null)
        {
            m_client = new Client(m_numerology_system_name);
            if (m_client != null)
            {
                string default_text_mode = m_client.NumerologySystem.TextMode;
                m_client.BuildSimplifiedBook(default_text_mode, true, false, false, false);

                PopulateTextModeComboBox();
                if (TextModeComboBox.Items.Count > 0)
                {
                    if (TextModeComboBox.Items.Contains(default_text_mode))
                    {
                        TextModeComboBox.SelectedItem = default_text_mode;
                    }
                    else
                    {
                        TextModeComboBox.SelectedIndex = 0;
                    }
                }
            }
        }

        m_lines = new List<Line>();
        m_generated_words = new SortedDictionary<string, int>();

        m_number_type = NumberType.Prime;
        NumberTypeLabel.Text = "P";
        NumberTypeLabel.ForeColor = Numbers.GetNumberTypeColor(19L);
        ToolTip.SetToolTip(ValueInterlaceLabel, "concatenate letter values");
        ToolTip.SetToolTip(ValueCombinationDirectionLabel, "combine letter values right to left");
        ToolTip.SetToolTip(NumberTypeLabel, "allow prime combined letter values only");
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
                        string default_text_mode = m_client.NumerologySystem.TextMode;
                        if (!numerology_system.Name.StartsWith(default_text_mode)) continue;

                        string[] parts = numerology_system.Name.Split('_');
                        if (parts != null)
                        {
                            if (parts.Length == 3)
                            {
                                string text_mode = parts[0];
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

    public enum SortMethod { ById, BySentence, ByValue, ByWord }
    public enum SortOrder { Ascending, Descending }
    public class Line : IComparable<Line>
    {
        public int Id;
        public string Sentence;
        public long Value;
        public string Word;

        public static SortMethod SortMethod;
        public static SortOrder SortOrder;
        public int CompareTo(Line obj)
        {
            if (SortOrder == SortOrder.Ascending)
            {
                if (SortMethod == SortMethod.ById)
                {
                    return this.Id.CompareTo(obj.Id);
                }
                else if (SortMethod == SortMethod.BySentence)
                {
                    return this.Sentence.CompareTo(obj.Sentence);
                }
                else if (SortMethod == SortMethod.ByValue)
                {
                    return this.Value.CompareTo(obj.Value);
                }
                else if (SortMethod == SortMethod.ByWord)
                {
                    return this.Word.CompareTo(obj.Word);
                }
                else
                {
                    return this.Id.CompareTo(obj.Id);
                }
            }
            else
            {
                if (SortMethod == SortMethod.ById)
                {
                    return obj.Id.CompareTo(this.Id);
                }
                else if (SortMethod == SortMethod.BySentence)
                {
                    return obj.Sentence.CompareTo(this.Sentence);
                }
                else if (SortMethod == SortMethod.ByValue)
                {
                    return obj.Value.CompareTo(this.Value);
                }
                else if (SortMethod == SortMethod.ByWord)
                {
                    return obj.Word.CompareTo(this.Word);
                }
                else
                {
                    return obj.Id.CompareTo(this.Id);
                }
            }
        }
    }
    private void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        if (ListView != null)
        {
            if (ListView.Columns != null)
            {
                // sort method
                Line.SortMethod = (SortMethod)e.Column;

                // sort order
                if (Line.SortOrder == SortOrder.Ascending)
                {
                    Line.SortOrder = SortOrder.Descending;
                }
                else
                {
                    Line.SortOrder = SortOrder.Ascending;
                }

                // sort marker
                string sort_marker = (Line.SortOrder == SortOrder.Ascending) ? "▲" : "▼";
                foreach (ColumnHeader column in ListView.Columns)
                {
                    if (column.Text.EndsWith("▲"))
                    {
                        column.Text = column.Text.Replace("▲", " ");
                    }
                    else if (column.Text.EndsWith("▼"))
                    {
                        column.Text = column.Text.Replace("▼", " ");
                    }
                }
                ListView.Columns[e.Column].Text = ListView.Columns[e.Column].Text.Replace("  ", " " + sort_marker);
                ListView.Refresh();

                // sort items
                m_lines.Sort();

                // display items
                UpdateListView();
            }
        }
    }
    private void ClearListView()
    {
        if (ListView != null)
        {
            if (ListView.Items != null)
            {
                ListView.Items.Clear();
                Line.SortMethod = (SortMethod)0;
                Line.SortOrder = SortOrder.Ascending;
                foreach (ColumnHeader column in ListView.Columns)
                {
                    if (column.Text.EndsWith("▲"))
                    {
                        column.Text = column.Text.Replace("▲", " ");
                    }
                    else if (column.Text.EndsWith("▼"))
                    {
                        column.Text = column.Text.Replace("▼", " ");
                    }
                }
                ListView.Columns[0].Text = ListView.Columns[0].Text = "# ▲";
                ListView.Refresh();
            }
        }
    }
    private void UpdateListView()
    {
        if (ListView != null)
        {
            if (ListView.Items != null)
            {
                ListView.Items.Clear();
                for (int i = 0; i < m_lines.Count; i++)
                {
                    string[] parts = new string[4];
                    parts[0] = m_lines[i].Id.ToString();
                    parts[1] = m_lines[i].Sentence.ToString();
                    parts[2] = m_lines[i].Value.ToString();
                    parts[3] = m_lines[i].Word;
                    ListView.Items.Add(new ListViewItem(parts, i));
                }
                ListView.Refresh();
            }
        }
    }

    private SortedDictionary<string, int> m_generated_words = null;
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
            ClearListView();

            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.Chapters[0] != null)
                    {
                        List<Verse> fatiha_verses = m_client.Book.Chapters[0].Verses;
                        if (fatiha_verses != null)
                        {
                            List<Word> fatiha_words = new List<Word>();
                            foreach (Verse fatiha_verse in fatiha_verses)
                            {
                                fatiha_words.AddRange(fatiha_verse.Words);
                            }


                            // find all 7-word 29-letter word subsets
                            WordSubsetFinder word_subset_finder = new WordSubsetFinder(fatiha_words);
                            m_word_subsets = word_subset_finder.Find(fatiha_verses.Count, fatiha_words.Count);
                            if (m_word_subsets != null)
                            {
                                if (ModifierKeys == Keys.Shift)
                                {
                                    foreach (string numerology_system_name in NumerologySystemComboBox.Items)
                                    {
                                        NumerologySystemComboBox.SelectedItem = numerology_system_name;
                                        NumerologySystemComboBox.Refresh();

                                        DoGenerateWords(false);
                                    } // foreach NumerologySystem
                                }
                                else
                                {
                                    DoGenerateWords(true);
                                }
                            }
                        }
                    }
                }
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
            ClearListView();

            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_client.Book.Chapters[0] != null)
                    {
                        List<Verse> fatiha_verses = m_client.Book.Chapters[0].Verses;
                        if (fatiha_verses != null)
                        {
                            // setup all quran words from quran verses
                            List<Word> fatiha_words = new List<Word>();
                            foreach (Verse fatiha_verse in fatiha_verses)
                            {
                                fatiha_words.AddRange(fatiha_verse.Words);
                            }

                            // find all 7-word 29-letter word subsets
                            WordSubsetFinder word_subset_finder = new WordSubsetFinder(fatiha_words);
                            m_word_subsets = word_subset_finder.Find(fatiha_verses.Count, fatiha_words.Count);
                            if (m_word_subsets != null)
                            {
                                if (ModifierKeys == Keys.Shift)
                                {
                                    foreach (string numerology_system_name in NumerologySystemComboBox.Items)
                                    {
                                        NumerologySystemComboBox.SelectedItem = numerology_system_name;
                                        NumerologySystemComboBox.Refresh();

                                        ProcessNumerologySystem();
                                    } // foreach NumerologySystem
                                }
                                else
                                {
                                    ProcessNumerologySystem();
                                }
                            }
                        }
                    }
                }
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
        if (m_generated_words != null)
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

                                            DoGenerateWords(false);

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
    private void DoGenerateWords(bool display_progress)
    {
        if (m_lines != null)
        {
            m_lines.Clear();
            if (m_generated_words != null)
            {
                m_generated_words.Clear();

                // get unique quran words
                List<string> quran_word_texts = m_client.GetSimplifiedWords();
                if (quran_word_texts != null)
                {

                    if (m_word_subsets != null)
                    {
                        for (int i = 0; i < m_word_subsets.Count; i++)
                        {
                            // calculate word values
                            long sentence_word_value = 0L;
                            foreach (Word word in m_word_subsets[i])
                            {
                                sentence_word_value += m_client.CalculateValue(word);
                            }

                            // calculate letter values
                            List<long> sentence_letter_values = new List<long>();
                            foreach (Word word in m_word_subsets[i])
                            {
                                foreach (Letter letter in word.Letters)
                                {
                                    long letter_value = m_client.CalculateValue(letter);
                                    if (m_add_verse_and_word_values_to_letter_value)
                                    {
                                        letter_value += m_client.CalculateValue(letter.Word);
                                        letter_value += m_client.CalculateValue(letter.Word.Verse);
                                    }
                                    sentence_letter_values.Add(letter_value);
                                }
                            }

                            // build sentence from word subset
                            StringBuilder str = new StringBuilder();
                            foreach (Word word in m_word_subsets[i])
                            {
                                str.Append(word.Text + " ");
                            }
                            if (str.Length > 1)
                            {
                                str.Remove(str.Length - 1, 1);
                            }

                            // generate Quran words
                            string generated_word = "";
                            if (m_client.NumerologySystem != null)
                            {
                                Dictionary<char, long> letter_dictionary = m_client.NumerologySystem.LetterValues;
                                if (letter_dictionary != null)
                                {
                                    List<char> numerology_letters = new List<char>(letter_dictionary.Keys);
                                    List<long> numerology_letter_values = new List<long>(letter_dictionary.Values);

                                    // interlace or concatenate values of numerology letters with sentence letters
                                    for (int j = 0; j < numerology_letters.Count; j++)
                                    {
                                        long number = 0L;
                                        long AAA = numerology_letter_values[j];
                                        long BBB = sentence_letter_values[j];
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
                                            // generate word from letter value combinations matching the number type
                                            if (Numbers.IsNumberType(number, m_number_type))
                                            {
                                                // mod 29 to select letter
                                                int index = (int)((long)number % (long)numerology_letters.Count);
                                                generated_word += numerology_letters[index];
                                            }
                                        }
                                    }
                                }
                            }

                            // add sentence if it generates a valid quran word
                            if (quran_word_texts.Contains(generated_word))
                            {
                                Line line = new Line();
                                line.Id = m_lines.Count + 1;
                                line.Sentence = str.ToString();
                                line.Value = sentence_word_value;
                                line.Word = generated_word;
                                m_lines.Add(line);

                                if (m_generated_words.ContainsKey(generated_word))
                                {
                                    m_generated_words[generated_word]++;
                                }
                                else
                                {
                                    m_generated_words.Add(generated_word, 1);
                                }
                            }

                            if (display_progress)
                            {
                                // display progress
                                this.Text = "WordGenerator | Primalogy value of أُمُّ ٱلْكِتَٰبِ = letters+diacritics of سورة الفاتحة | Sentence " + (i + 1) + "/" + m_word_subsets.Count;
                                ProgressBar.Value = ((i + 1) * 100) / m_word_subsets.Count;
                                WordCountLabel.Text = m_lines.Count + " (" + m_generated_words.Count + ") words";
                                WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(m_lines.Count);
                                WordCountLabel.Refresh();

                                Application.DoEvents();
                            }
                        } // for m_word_subsets
                    }

                    // at the end of running
                    this.Text = "WordGenerator | Primalogy value of أُمُّ ٱلْكِتَٰبِ = letters+diacritics of سورة الفاتحة | Sentences = " + m_word_subsets.Count;
                    ProgressBar.Value = 100;
                    ProgressBar.Refresh();
                    WordCountLabel.Text = m_lines.Count + " (" + m_generated_words.Count + ") words";
                    WordCountLabel.ForeColor = Numbers.GetNumberTypeColor(m_lines.Count);
                    WordCountLabel.Refresh();

                    UpdateListView();
                    InspectButton_Click(null, null);

                    Application.DoEvents();
                }
            }
        }
    }
    private void InspectButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_lines != null)
            {
                StringBuilder str = new StringBuilder();

                if (sender == InspectButton)
                {
                    if (m_lines.Count > 0)
                    {
                        for (int i = 0; i < m_lines.Count; i++)
                        {
                            if (m_lines[i] != null)
                            {
                                str.AppendLine(m_lines[i].Id + "\t" + m_lines[i].Sentence + "\t" + m_lines[i].Value + "\t" + m_lines[i].Word);
                            }
                        }
                        str.AppendLine();
                    }
                }

                if (m_generated_words != null)
                {
                    str.AppendLine("#" + "\t" + "Word" + "\t" + "Freq" + "\t" + "Length" + "\t" + "Value");

                    int count = 0;
                    int frequency_sum = 0;
                    int length_sum = 0;
                    long value_sum = 0L;
                    foreach (string key in m_generated_words.Keys)
                    {
                        count++;

                        int frequency = m_generated_words[key];
                        frequency_sum += frequency;

                        int length = key.Length;
                        length_sum += length;

                        long value = m_client.CalculateValueUserText(key);
                        value_sum += value;

                        str.AppendLine(count + "\t" + key + "\t" + frequency + "\t" + length + "\t" + value);
                    }

                    str.AppendLine();
                    str.AppendLine(count + "\t" + "Sum" + "\t" + frequency_sum + "\t" + length_sum + "\t" + value_sum);
                }

                string filename = null;
                if (sender == InspectButton)
                {
                    string sort_method = Line.SortMethod.ToString().Substring(2);
                    string sort_order = (Line.SortOrder == SortOrder.Ascending) ? "asc" : "desc";

                    filename =
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
                        + "_" + m_generated_words.Count.ToString()
                        + "_" + sort_method + "_" + sort_order
                        + ".txt";
                }
                else
                {
                    filename =
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
                        + "_" + m_generated_words.Count.ToString()
                        + ".txt";
                }

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
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
}
