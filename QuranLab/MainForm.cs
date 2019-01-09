using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        ShowVersion(false);

        QueryParameterNumericUpDown_ValueChanged(null, null);
        CountOnlyCheckBox_CheckedChanged(null, null);
        SaveMatchesButton.Enabled = false;
        CountOnlyCheckBox.Enabled = false;
        ChapterCountNumericUpDown.Focus();

        PopulateQueryParameterComboBoxes();
        PopulateOutputFormatComboBox();
    }

    private bool m_with_revision = false;
    private void ShowVersion(bool with_revision)
    {
        m_with_revision = with_revision;

        int major = Assembly.GetEntryAssembly().GetName().Version.Major;
        int minor = Assembly.GetEntryAssembly().GetName().Version.Minor;
        int build = Assembly.GetEntryAssembly().GetName().Version.Build;
        int revision = Assembly.GetEntryAssembly().GetName().Version.Revision;
        //int major_revision = Assembly.GetEntryAssembly().GetName().Version.MajorRevision;
        //int minor_revision = Assembly.GetEntryAssembly().GetName().Version.MinorRevision;
        this.Text = Application.ProductName + " v" + major + "." + minor + "." + build + (m_with_revision ? ("." + revision) : "");

        //string version = typeof(MainForm).Assembly.GetName().Version.ToString();
        //int pos = version.LastIndexOf(".");
        //VersionLabel.Text = "v " + version.Substring(0, pos);

    }
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (m_running)
        {
            e.Cancel = true;
        }
    }
    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Finish(true);
        }
        else if (ModifierKeys == Keys.Control)
        {
            m_with_revision = !m_with_revision;
            ShowVersion(m_with_revision);
        }
    }
    private void MainForm_Resize(object sender, EventArgs e)
    {
        //ElapsedTimeValueLabel.Width = ((MainPanel.Width - ElapsedTimeValueLabel.Left - 10) / 2);
        //RemainingTimeValueLabel.Left = ElapsedTimeValueLabel.Left + ElapsedTimeValueLabel.Width + 1;
        //RemainingTimeValueLabel.Width = ElapsedTimeValueLabel.Width;
    }

    private int m_chapter_count = 0;
    private int m_chapter_sum = 0;
    private int m_verse_count = 0;
    private int m_word_count = 0;
    private int m_letter_count = 0;
    private int m_cplusv_sum = 0;
    private int m_cminusv_sum = 0;
    private int m_ctimesv_sum = 0;
    private long m_match_count = 0;
    private void QueryParameterNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        CaptureQueryParameters();
        CaptureNumberTypes();
        UpdateOtherQueryParameters(sender);
        TODO_UpdateOtherNumberTypes(sender);

        bool can_run = (m_chapter_count > 0) ||
                       ((m_chapter_count_number_type != NumberType.None) && (m_chapter_count_number_type != NumberType.Natural)) ||
                       (m_chapter_sum > 0) ||
                       ((m_chapter_sum_number_type != NumberType.None) && (m_chapter_sum_number_type != NumberType.Natural));

        FindButton.Enabled = can_run;
        CountOnlyCheckBox.Enabled = can_run;

        if (m_chapter_count >= 0)
        {
            int count = (int)ChapterCountNumericUpDown.Value;
            BigInteger combinations = Numbers.NChooseK(Book.CHAPTERS, count);
            MatchCountTextBox.Text = combinations.ToString();
        }
    }
    private void QueryParameterNumericUpDown_Enter(object sender, EventArgs e)
    {
        NumericUpDown control = sender as NumericUpDown;
        if (control != null)
        {
            control.Select(0, control.Text.Length);
        }
    }
    private void QueryParameterNumericUpDown_Leave(object sender, EventArgs e)
    {
        NumericUpDown control = sender as NumericUpDown;
        if (control != null)
        {
            if (String.IsNullOrEmpty(control.Text))
            {
                control.Value = 0;
                control.Refresh();
            }
        }
    }
    private void PopulateQueryParameterComboBoxes()
    {
        VerseCountComboBox.Items.Clear();
        VerseCountComboBox.Items.Add("Number");
        VerseCountComboBox.Items.Add("Chapter Sum");
        if (VerseCountComboBox.Items.Count > 0) VerseCountComboBox.SelectedIndex = 0;

        WordCountComboBox.Items.Clear();
        WordCountComboBox.Items.Add("Number");
        WordCountComboBox.Items.Add("Chapter Sum");
        if (WordCountComboBox.Items.Count > 0) WordCountComboBox.SelectedIndex = 0;

        LetterCountComboBox.Items.Clear();
        LetterCountComboBox.Items.Add("Number");
        LetterCountComboBox.Items.Add("Chapter Sum");
        if (LetterCountComboBox.Items.Count > 0) LetterCountComboBox.SelectedIndex = 0;

        CPlusVSumComboBox.Items.Clear();
        CPlusVSumComboBox.Items.Add("Number");
        CPlusVSumComboBox.Items.Add("Word Count");
        CPlusVSumComboBox.Items.Add("Letter Count");
        if (CPlusVSumComboBox.Items.Count > 0) CPlusVSumComboBox.SelectedIndex = 0;

        CMinusVSumComboBox.Items.Clear();
        CMinusVSumComboBox.Items.Add("Number");
        CMinusVSumComboBox.Items.Add("Chapter Sum");
        CMinusVSumComboBox.Items.Add("Verse Count");
        CMinusVSumComboBox.Items.Add("Zero");
        if (CMinusVSumComboBox.Items.Count > 0) CMinusVSumComboBox.SelectedIndex = 0;

        CTimesVSumComboBox.Items.Clear();
        CTimesVSumComboBox.Items.Add("Number");
        CTimesVSumComboBox.Items.Add("Word Count");
        CTimesVSumComboBox.Items.Add("Letter Count");
        if (CTimesVSumComboBox.Items.Count > 0) CTimesVSumComboBox.SelectedIndex = 0;

        MatchCountComboBox.Items.Clear();
        MatchCountComboBox.Items.Add("Number");
        MatchCountComboBox.Items.Add("Chapter Sum");
        MatchCountComboBox.Items.Add("Verse Count");
        MatchCountComboBox.Items.Add("Word Count");
        MatchCountComboBox.Items.Add("Letter Count");
        MatchCountComboBox.Items.Add("V + S  Sum");
        MatchCountComboBox.Items.Add("V - S  Sum");
        MatchCountComboBox.Items.Add("V * S  Sum");
        if (MatchCountComboBox.Items.Count > 0) MatchCountComboBox.SelectedIndex = 0;
    }
    private void QueryParameterComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        CaptureQueryParameters();
        CaptureNumberTypes();

        UpdateSenderQueryParameter(sender);
        TODO_UpdateSenderNumberType(sender);
    }
    private void CaptureQueryParameters()
    {
        m_chapter_count = (int)ChapterCountNumericUpDown.Value;

        m_chapter_sum = (int)ChapterSumNumericUpDown.Value;

        switch (VerseCountComboBox.SelectedIndex)
        {
            case 0: // Number
                m_verse_count = (int)VerseCountNumericUpDown.Value;
                break;
            case 1: // Chapter Sum
                m_verse_count = m_chapter_sum;
                break;
            default:
                break;
        }

        switch (WordCountComboBox.SelectedIndex)
        {
            case 0: // Number
                m_word_count = (int)WordCountNumericUpDown.Value;
                break;
            case 1: // Chapter Sum
                m_word_count = m_chapter_sum;
                break;
            default:
                break;
        }

        switch (LetterCountComboBox.SelectedIndex)
        {
            case 0: // Number
                m_letter_count = (int)LetterCountNumericUpDown.Value;
                break;
            case 1: // Chapter Sum
                m_letter_count = m_chapter_sum;
                break;
            default:
                break;
        }

        switch (CPlusVSumComboBox.SelectedIndex)
        {
            case 0: // Number
                m_cplusv_sum = (int)CPlusVSumNumericUpDown.Value;
                break;
            case 1: // Words
                m_cplusv_sum = m_word_count;
                break;
            case 2: // Letters
                m_cplusv_sum = m_letter_count;
                break;
            default:
                break;
        }

        switch (CMinusVSumComboBox.SelectedIndex)
        {
            case 0: // Number
                m_cminusv_sum = (int)CMinusVSumNumericUpDown.Value;
                break;
            case 1: // Chapter Sum
                m_cminusv_sum = m_chapter_sum;
                break;
            case 2: // Verses
                m_cminusv_sum = m_verse_count;
                break;
            case 3: // Zero
                m_cminusv_sum = int.MaxValue; // Zero subtitute as 0 means ANY
                break;
            default:
                break;
        }

        switch (CTimesVSumComboBox.SelectedIndex)
        {
            case 0: // Number
                m_ctimesv_sum = (int)CTimesVSumNumericUpDown.Value;
                break;
            case 1: // Words
                m_ctimesv_sum = m_word_count;
                break;
            case 2: // Letters
                m_ctimesv_sum = m_letter_count;
                break;
            default:
                break;
        }

        switch (MatchCountComboBox.SelectedIndex)
        {
            case 0: // Number
                m_match_count = (long)MatchCountNumericUpDown.Value;
                break;
            case 1: // Chapter Sum
                m_match_count = m_chapter_sum;
                break;
            case 2: // Verse Count
                m_match_count = m_verse_count;
                break;
            case 3: // Word Count
                m_match_count = m_word_count;
                break;
            case 4: // Letter Count
                m_match_count = m_letter_count;
                break;
            case 5: // C + V  Sum
                m_match_count = m_cplusv_sum;
                break;
            case 6: // C - V  Sum
                m_match_count = (m_cminusv_sum == int.MaxValue) ? 0L : m_cminusv_sum;
                break;
            case 7: // C * V  Sum
                m_match_count = m_ctimesv_sum;
                break;
            default:
                break;
        }
    }
    private void UpdateSenderQueryParameter(object sender)
    {
        if (sender != null)
        {
            try
            {
                VerseCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                WordCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                LetterCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CPlusVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CMinusVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CTimesVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                MatchCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);

                if (sender == VerseCountComboBox)
                {
                    if (VerseCountComboBox.SelectedIndex > 0)
                    {
                        VerseCountNumericUpDown.Value = m_verse_count;
                        VerseCountNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        VerseCountNumericUpDown.Enabled = true;
                    }
                }

                if (sender == WordCountComboBox)
                {
                    if (WordCountComboBox.SelectedIndex > 0)
                    {
                        WordCountNumericUpDown.Value = m_word_count;
                        WordCountNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        WordCountNumericUpDown.Enabled = true;
                    }
                }

                if (sender == LetterCountComboBox)
                {
                    if (LetterCountComboBox.SelectedIndex > 0)
                    {
                        LetterCountNumericUpDown.Value = m_letter_count;
                        LetterCountNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        LetterCountNumericUpDown.Enabled = true;
                    }
                }

                if (sender == CPlusVSumComboBox)
                {
                    if (CPlusVSumComboBox.SelectedIndex > 0)
                    {
                        CPlusVSumNumericUpDown.Value = m_cplusv_sum;
                        CPlusVSumNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        CPlusVSumNumericUpDown.Enabled = true;
                    }
                }

                if (sender == CMinusVSumComboBox)
                {
                    if (CMinusVSumComboBox.SelectedIndex > 0)
                    {
                        CMinusVSumNumericUpDown.Value = (m_cminusv_sum == int.MaxValue) ? 0 : m_cminusv_sum;
                        CMinusVSumNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        CMinusVSumNumericUpDown.Enabled = true;
                    }
                }

                if (sender == CTimesVSumComboBox)
                {
                    if (CTimesVSumComboBox.SelectedIndex > 0)
                    {
                        CTimesVSumNumericUpDown.Value = m_ctimesv_sum;
                        CTimesVSumNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        CTimesVSumNumericUpDown.Enabled = true;
                    }
                }

                if (sender == MatchCountComboBox)
                {
                    if (MatchCountComboBox.SelectedIndex > 0)
                    {
                        MatchCountNumericUpDown.Value = m_match_count;
                        MatchCountNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        MatchCountNumericUpDown.Enabled = true;
                    }
                }
            }
            finally
            {
                VerseCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                WordCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                LetterCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CPlusVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CMinusVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CTimesVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                MatchCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            }
        }
    }
    private void UpdateOtherQueryParameters(object sender)
    {
        try
        {
            VerseCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            WordCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            LetterCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CPlusVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CMinusVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CTimesVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            MatchCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);

            if ((sender != VerseCountComboBox) && (sender != VerseCountNumericUpDown))
            {
                if (VerseCountComboBox.SelectedIndex > 0)
                {
                    VerseCountNumericUpDown.Value = m_verse_count;
                    VerseCountNumericUpDown.Enabled = false;
                }
                else
                {
                    VerseCountNumericUpDown.Enabled = true;
                }
            }

            if ((sender != WordCountComboBox) && (sender != WordCountNumericUpDown))
            {
                if (WordCountComboBox.SelectedIndex > 0)
                {
                    WordCountNumericUpDown.Value = m_word_count;
                    WordCountNumericUpDown.Enabled = false;
                }
                else
                {
                    WordCountNumericUpDown.Enabled = true;
                }
            }

            if ((sender != LetterCountComboBox) && (sender != LetterCountNumericUpDown))
            {
                if (LetterCountComboBox.SelectedIndex > 0)
                {
                    LetterCountNumericUpDown.Value = m_letter_count;
                    LetterCountNumericUpDown.Enabled = false;
                }
                else
                {
                    LetterCountNumericUpDown.Enabled = true;
                }
            }

            if ((sender != CPlusVSumComboBox) && (sender != CPlusVSumNumericUpDown))
            {
                if (CPlusVSumComboBox.SelectedIndex > 0)
                {
                    CPlusVSumNumericUpDown.Value = m_cplusv_sum;
                    CPlusVSumNumericUpDown.Enabled = false;
                }
                else
                {
                    CPlusVSumNumericUpDown.Enabled = true;
                }
            }

            if ((sender != CMinusVSumComboBox) && (sender != CMinusVSumNumericUpDown))
            {
                if (CMinusVSumComboBox.SelectedIndex > 0)
                {
                    CMinusVSumNumericUpDown.Value = (m_cminusv_sum == int.MaxValue) ? 0 : m_cminusv_sum;
                    CMinusVSumNumericUpDown.Enabled = false;
                }
                else
                {
                    CMinusVSumNumericUpDown.Enabled = true;
                }
            }

            if ((sender != CTimesVSumComboBox) && (sender != CTimesVSumNumericUpDown))
            {
                if (CTimesVSumComboBox.SelectedIndex > 0)
                {
                    CTimesVSumNumericUpDown.Value = m_ctimesv_sum;
                    CTimesVSumNumericUpDown.Enabled = false;
                }
                else
                {
                    CTimesVSumNumericUpDown.Enabled = true;
                }
            }

            if ((sender != MatchCountComboBox) && (sender != MatchCountNumericUpDown))
            {
                if (MatchCountComboBox.SelectedIndex > 0)
                {
                    MatchCountNumericUpDown.Value = m_match_count;
                    MatchCountNumericUpDown.Enabled = false;
                }
                else
                {
                    MatchCountNumericUpDown.Enabled = true;
                }
            }
        }
        finally
        {
            VerseCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            WordCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            LetterCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CPlusVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CMinusVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CTimesVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            MatchCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
        }
    }

    private void NumberTypeLabel_Click(object sender, EventArgs e)
    {
        Control control = sender as Control;
        if (control != null)
        {
            UpdateNumberType(control);

            if (control == ChapterCountNumberTypeLabel)
            {
                ChapterCountNumericUpDown.Enabled = (control.Text == "");
                if (control.Text.Length > 0)
                {
                    ChapterCountNumericUpDown.Value = 0;
                }
                else
                {
                    ChapterCountNumericUpDown.Focus();
                }
            }
            else if (control == ChapterSumNumberTypeLabel)
            {
                ChapterSumNumericUpDown.Enabled = (control.Text == "");
                if (control.Text.Length > 0)
                {
                    ChapterSumNumericUpDown.Value = 0;
                }
                else
                {
                    ChapterSumNumericUpDown.Focus();
                }
            }
            else if (control == VerseCountNumberTypeLabel)
            {
                VerseCountNumericUpDown.Enabled = (control.Text == "");
                if (control.Text.Length > 0)
                {
                    VerseCountNumericUpDown.Value = 0;
                }
                else
                {
                    VerseCountNumericUpDown.Focus();
                }
            }
            else if (control == WordCountNumberTypeLabel)
            {
                WordCountNumericUpDown.Enabled = (control.Text == "");
                if (control.Text.Length > 0)
                {
                    WordCountNumericUpDown.Value = 0;
                }
                else
                {
                    WordCountNumericUpDown.Focus();
                }
            }
            else if (control == LetterCountNumberTypeLabel)
            {
                LetterCountNumericUpDown.Enabled = (control.Text == "");
                if (control.Text.Length > 0)
                {
                    LetterCountNumericUpDown.Value = 0;
                }
                else
                {
                    LetterCountNumericUpDown.Focus();
                }
            }
            else if (control == CPlusVSumNumberTypeLabel)
            {
                CPlusVSumNumericUpDown.Enabled = (control.Text == "");
                if (control.Text.Length > 0)
                {
                    CPlusVSumNumericUpDown.Value = 0;
                }
                else
                {
                    CPlusVSumNumericUpDown.Focus();
                }
            }
            else if (control == CMinusVSumNumberTypeLabel)
            {
                CMinusVSumNumericUpDown.Enabled = (control.Text == "");
                if (control.Text.Length > 0)
                {
                    CMinusVSumNumericUpDown.Value = 0;
                }
                else
                {
                    CMinusVSumNumericUpDown.Focus();
                }
            }
            else if (control == CTimesVSumNumberTypeLabel)
            {
                CTimesVSumNumericUpDown.Enabled = (control.Text == "");
                if (control.Text.Length > 0)
                {
                    CTimesVSumNumericUpDown.Value = 0;
                }
                else
                {
                    CTimesVSumNumericUpDown.Focus();
                }
            }
            else if (control == MatchCountNumberTypeLabel)
            {
                MatchCountNumericUpDown.Enabled = (control.Text == "");
                if (control.Text.Length > 0)
                {
                    MatchCountNumericUpDown.Value = 0;
                }
                else
                {
                    MatchCountNumericUpDown.Focus();
                }
            }
            else
            {
                // do nothing
            }
        }

        QueryParameterNumericUpDown_ValueChanged(sender, e);
    }
    private void UpdateNumberType(Control control)
    {
        if (control == null) return;

        if (ModifierKeys != Keys.Shift)
        {
            if (control.Text == "")
            {
                control.Text = "P";
                control.ForeColor = Numbers.GetNumberTypeColor(19L);
                ToolTip.SetToolTip(control, "prime = divisible by itself only");
            }
            else if (control.Text == "P")
            {
                control.Text = "AP";
                control.ForeColor = Numbers.GetNumberTypeColor(47L);
                ToolTip.SetToolTip(control, "additive prime = prime with a prime digit sum");
            }
            else if (control.Text == "AP")
            {
                control.Text = "XP";
                control.ForeColor = Numbers.GetNumberTypeColor(19L);
                ToolTip.SetToolTip(control, "non-additive prime = prime with a composite digit sum");
            }
            else if (control.Text == "XP")
            {
                control.Text = "C";
                control.ForeColor = Numbers.GetNumberTypeColor(14L);
                ToolTip.SetToolTip(control, "composite = divisible by prime(s) below it");
            }
            else if (control.Text == "C")
            {
                control.Text = "AC";
                control.ForeColor = Numbers.GetNumberTypeColor(114L);
                ToolTip.SetToolTip(control, "additive composite = composite with a composite digit sum");
            }
            else if (control.Text == "AC")
            {
                control.Text = "XC";
                control.ForeColor = Numbers.GetNumberTypeColor(25L);
                ToolTip.SetToolTip(control, "non-additive composite = composite with a prime digit sum");
            }
            else if (control.Text == "XC")
            {
                control.Text = "^2";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "square number");
            }
            else if (control.Text == "^2")
            {
                control.Text = "^3";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "cubic number");
            }
            else if (control.Text == "^3")
            {
                control.Text = "^4";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "quartic number");
            }
            else if (control.Text == "^4")
            {
                control.Text = "^5";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "quintic number");
            }
            else if (control.Text == "^5")
            {
                control.Text = "^6";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "sextic number");
            }
            else if (control.Text == "^6")
            {
                control.Text = "^7";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "septic number");
            }
            else if (control.Text == "^7")
            {
                control.Text = "";
                control.ForeColor = control.BackColor;
                ToolTip.SetToolTip(control, "");
            }
        }
        else // if (ModifierKeys == Keys.Shift)
        {
            if (control.Text == "")
            {
                control.Text = "^7";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "septic number");
            }
            else if (control.Text == "^7")
            {
                control.Text = "^6";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "sextic number");
            }
            else if (control.Text == "^6")
            {
                control.Text = "^5";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "quartic number");
            }
            else if (control.Text == "^5")
            {
                control.Text = "^4";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "quartic number");
            }
            else if (control.Text == "^4")
            {
                control.Text = "^3";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "cubic number");
            }
            else if (control.Text == "^3")
            {
                control.Text = "^2";
                control.ForeColor = Numbers.GetNumberTypeColor(0L);
                ToolTip.SetToolTip(control, "square number");
            }
            else if (control.Text == "^2")
            {
                control.Text = "XC";
                control.ForeColor = Numbers.GetNumberTypeColor(25L);
                ToolTip.SetToolTip(control, "non-additive composite = composite with a prime digit sum");
            }
            else if (control.Text == "XC")
            {
                control.Text = "AC";
                control.ForeColor = Numbers.GetNumberTypeColor(114L);
                ToolTip.SetToolTip(control, "additive composite = composite with a composite digit sum");
            }
            else if (control.Text == "AC")
            {
                control.Text = "C";
                control.ForeColor = Numbers.GetNumberTypeColor(14L);
                ToolTip.SetToolTip(control, "composite = divisible by prime(s) below it");
            }
            else if (control.Text == "C")
            {
                control.Text = "XP";
                control.ForeColor = Numbers.GetNumberTypeColor(19L);
                ToolTip.SetToolTip(control, "non-additive prime = prime with a composite digit sum");
            }
            else if (control.Text == "XP")
            {
                control.Text = "AP";
                control.ForeColor = Numbers.GetNumberTypeColor(47L);
                ToolTip.SetToolTip(control, "additive prime = prime with a prime digit sum");
            }
            else if (control.Text == "AP")
            {
                control.Text = "P";
                control.ForeColor = Numbers.GetNumberTypeColor(19L);
                ToolTip.SetToolTip(control, "prime = divisible by itself only");
            }
            else if (control.Text == "P")
            {
                control.Text = "";
                control.ForeColor = control.BackColor;
                ToolTip.SetToolTip(control, "");
            }
        }
    }
    private NumberType m_chapter_sum_number_type;
    private NumberType m_chapter_count_number_type;
    private NumberType m_verse_count_number_type;
    private NumberType m_word_count_number_type;
    private NumberType m_letter_count_number_type;
    private NumberType m_cplusv_sum_number_type;
    private NumberType m_cminusv_sum_number_type;
    private NumberType m_ctimesv_sum_number_type;
    private NumberType m_match_count_number_type;
    private void CaptureNumberTypes()
    {
        string chapter_sum_symbol = ChapterSumNumberTypeLabel.Enabled ? ChapterSumNumberTypeLabel.Text : "";
        m_chapter_sum_number_type =
            (chapter_sum_symbol == "P") ? NumberType.Prime :
            (chapter_sum_symbol == "AP") ? NumberType.AdditivePrime :
            (chapter_sum_symbol == "XP") ? NumberType.NonAdditivePrime :
            (chapter_sum_symbol == "C") ? NumberType.Composite :
            (chapter_sum_symbol == "AC") ? NumberType.AdditiveComposite :
            (chapter_sum_symbol == "XC") ? NumberType.NonAdditiveComposite :
            (chapter_sum_symbol == "^2") ? NumberType.Square :
            (chapter_sum_symbol == "^3") ? NumberType.Cubic :
            (chapter_sum_symbol == "^4") ? NumberType.Quartic :
            (chapter_sum_symbol == "^5") ? NumberType.Quintic :
            (chapter_sum_symbol == "^6") ? NumberType.Sextic :
            (chapter_sum_symbol == "^7") ? NumberType.Septic :
            (chapter_sum_symbol == "") ? NumberType.None :
                                           NumberType.Natural;
        string chapter_count_symbol = ChapterCountNumberTypeLabel.Enabled ? ChapterCountNumberTypeLabel.Text : "";
        m_chapter_count_number_type =
           (chapter_count_symbol == "P") ? NumberType.Prime :
           (chapter_count_symbol == "AP") ? NumberType.AdditivePrime :
           (chapter_count_symbol == "XP") ? NumberType.NonAdditivePrime :
           (chapter_count_symbol == "C") ? NumberType.Composite :
           (chapter_count_symbol == "AC") ? NumberType.AdditiveComposite :
           (chapter_count_symbol == "XC") ? NumberType.NonAdditiveComposite :
           (chapter_count_symbol == "^2") ? NumberType.Square :
           (chapter_count_symbol == "^3") ? NumberType.Cubic :
           (chapter_count_symbol == "^4") ? NumberType.Quartic :
           (chapter_count_symbol == "^5") ? NumberType.Quintic :
           (chapter_count_symbol == "^6") ? NumberType.Sextic :
           (chapter_count_symbol == "^7") ? NumberType.Septic :
           (chapter_count_symbol == "") ? NumberType.None :
                                          NumberType.Natural;
        string verse_count_symbol = VerseCountNumberTypeLabel.Enabled ? VerseCountNumberTypeLabel.Text : "";
        m_verse_count_number_type =
           (verse_count_symbol == "P") ? NumberType.Prime :
           (verse_count_symbol == "AP") ? NumberType.AdditivePrime :
           (verse_count_symbol == "XP") ? NumberType.NonAdditivePrime :
           (verse_count_symbol == "C") ? NumberType.Composite :
           (verse_count_symbol == "AC") ? NumberType.AdditiveComposite :
           (verse_count_symbol == "XC") ? NumberType.NonAdditiveComposite :
           (verse_count_symbol == "^2") ? NumberType.Square :
           (verse_count_symbol == "^3") ? NumberType.Cubic :
           (verse_count_symbol == "^4") ? NumberType.Quartic :
           (verse_count_symbol == "^5") ? NumberType.Quintic :
           (verse_count_symbol == "^6") ? NumberType.Sextic :
           (verse_count_symbol == "^7") ? NumberType.Septic :
           (verse_count_symbol == "") ? NumberType.None :
                                        NumberType.Natural;
        string word_count_symbol = WordCountNumberTypeLabel.Enabled ? WordCountNumberTypeLabel.Text : "";
        m_word_count_number_type =
           (word_count_symbol == "P") ? NumberType.Prime :
           (word_count_symbol == "AP") ? NumberType.AdditivePrime :
           (word_count_symbol == "XP") ? NumberType.NonAdditivePrime :
           (word_count_symbol == "C") ? NumberType.Composite :
           (word_count_symbol == "AC") ? NumberType.AdditiveComposite :
           (word_count_symbol == "XC") ? NumberType.NonAdditiveComposite :
           (word_count_symbol == "^2") ? NumberType.Square :
           (word_count_symbol == "^3") ? NumberType.Cubic :
           (word_count_symbol == "^4") ? NumberType.Quartic :
           (word_count_symbol == "^5") ? NumberType.Quintic :
           (word_count_symbol == "^6") ? NumberType.Sextic :
           (word_count_symbol == "^7") ? NumberType.Septic :
           (word_count_symbol == "") ? NumberType.None :
                                       NumberType.Natural;
        string letter_count_symbol = LetterCountNumberTypeLabel.Enabled ? LetterCountNumberTypeLabel.Text : "";
        m_letter_count_number_type =
           (letter_count_symbol == "P") ? NumberType.Prime :
           (letter_count_symbol == "AP") ? NumberType.AdditivePrime :
           (letter_count_symbol == "XP") ? NumberType.NonAdditivePrime :
           (letter_count_symbol == "C") ? NumberType.Composite :
           (letter_count_symbol == "AC") ? NumberType.AdditiveComposite :
           (letter_count_symbol == "XC") ? NumberType.NonAdditiveComposite :
           (letter_count_symbol == "^2") ? NumberType.Square :
           (letter_count_symbol == "^3") ? NumberType.Cubic :
           (letter_count_symbol == "^4") ? NumberType.Quartic :
           (letter_count_symbol == "^5") ? NumberType.Quintic :
           (letter_count_symbol == "^6") ? NumberType.Sextic :
           (letter_count_symbol == "^7") ? NumberType.Septic :
           (letter_count_symbol == "") ? NumberType.None :
                                         NumberType.Natural;
        string cplusv_sum_symbol = CPlusVSumNumberTypeLabel.Enabled ? CPlusVSumNumberTypeLabel.Text : "";
        m_cplusv_sum_number_type =
           (cplusv_sum_symbol == "P") ? NumberType.Prime :
           (cplusv_sum_symbol == "AP") ? NumberType.AdditivePrime :
           (cplusv_sum_symbol == "XP") ? NumberType.NonAdditivePrime :
           (cplusv_sum_symbol == "C") ? NumberType.Composite :
           (cplusv_sum_symbol == "AC") ? NumberType.AdditiveComposite :
           (cplusv_sum_symbol == "XC") ? NumberType.NonAdditiveComposite :
           (cplusv_sum_symbol == "^2") ? NumberType.Square :
           (cplusv_sum_symbol == "^3") ? NumberType.Cubic :
           (cplusv_sum_symbol == "^4") ? NumberType.Quartic :
           (cplusv_sum_symbol == "^5") ? NumberType.Quintic :
           (cplusv_sum_symbol == "^6") ? NumberType.Sextic :
           (cplusv_sum_symbol == "^7") ? NumberType.Septic :
           (cplusv_sum_symbol == "") ? NumberType.None :
                                          NumberType.Natural;
        string cminusv_sum_symbol = CMinusVSumNumberTypeLabel.Enabled ? CMinusVSumNumberTypeLabel.Text : "";
        m_cminusv_sum_number_type =
           (cminusv_sum_symbol == "P") ? NumberType.Prime :
           (cminusv_sum_symbol == "AP") ? NumberType.AdditivePrime :
           (cminusv_sum_symbol == "XP") ? NumberType.NonAdditivePrime :
           (cminusv_sum_symbol == "C") ? NumberType.Composite :
           (cminusv_sum_symbol == "AC") ? NumberType.AdditiveComposite :
           (cminusv_sum_symbol == "XC") ? NumberType.NonAdditiveComposite :
           (cminusv_sum_symbol == "^2") ? NumberType.Square :
           (cminusv_sum_symbol == "^3") ? NumberType.Cubic :
           (cminusv_sum_symbol == "^4") ? NumberType.Quartic :
           (cminusv_sum_symbol == "^5") ? NumberType.Quintic :
           (cminusv_sum_symbol == "^6") ? NumberType.Sextic :
           (cminusv_sum_symbol == "^7") ? NumberType.Septic :
           (cminusv_sum_symbol == "") ? NumberType.None :
                                          NumberType.Natural;
        string ctimesv_sum_symbol = CTimesVSumNumberTypeLabel.Enabled ? CTimesVSumNumberTypeLabel.Text : "";
        m_ctimesv_sum_number_type =
           (ctimesv_sum_symbol == "P") ? NumberType.Prime :
           (ctimesv_sum_symbol == "AP") ? NumberType.AdditivePrime :
           (ctimesv_sum_symbol == "XP") ? NumberType.NonAdditivePrime :
           (ctimesv_sum_symbol == "C") ? NumberType.Composite :
           (ctimesv_sum_symbol == "AC") ? NumberType.AdditiveComposite :
           (ctimesv_sum_symbol == "XC") ? NumberType.NonAdditiveComposite :
           (ctimesv_sum_symbol == "^2") ? NumberType.Square :
           (ctimesv_sum_symbol == "^3") ? NumberType.Cubic :
           (ctimesv_sum_symbol == "^4") ? NumberType.Quartic :
           (ctimesv_sum_symbol == "^5") ? NumberType.Quintic :
           (ctimesv_sum_symbol == "^6") ? NumberType.Sextic :
           (ctimesv_sum_symbol == "^7") ? NumberType.Septic :
           (ctimesv_sum_symbol == "") ? NumberType.None :
                                          NumberType.Natural;
        string match_count_symbol = MatchCountNumberTypeLabel.Enabled ? MatchCountNumberTypeLabel.Text : "";
        m_match_count_number_type =
           (match_count_symbol == "P") ? NumberType.Prime :
           (match_count_symbol == "AP") ? NumberType.AdditivePrime :
           (match_count_symbol == "XP") ? NumberType.NonAdditivePrime :
           (match_count_symbol == "C") ? NumberType.Composite :
           (match_count_symbol == "AC") ? NumberType.AdditiveComposite :
           (match_count_symbol == "XC") ? NumberType.NonAdditiveComposite :
           (match_count_symbol == "^2") ? NumberType.Square :
           (match_count_symbol == "^3") ? NumberType.Cubic :
           (match_count_symbol == "^4") ? NumberType.Quartic :
           (match_count_symbol == "^5") ? NumberType.Quintic :
           (match_count_symbol == "^6") ? NumberType.Sextic :
           (match_count_symbol == "^7") ? NumberType.Septic :
           (match_count_symbol == "") ? NumberType.None :
                                          NumberType.Natural;
    }
    private void TODO_UpdateSenderNumberType(object sender)
    {
        if (sender != null)
        {
            try
            {
                VerseCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                WordCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                LetterCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CPlusVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CMinusVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CTimesVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                MatchCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);

                if (sender == VerseCountComboBox)
                {
                    if (VerseCountComboBox.SelectedIndex > 0)
                    {
                        if ((m_verse_count_number_type != NumberType.None) && (m_verse_count_number_type != NumberType.Natural))
                        {
                            //??? VerseCountNumericUpDown.Value = 0;
                        }
                        else
                        {
                            VerseCountNumericUpDown.Value = m_verse_count;
                        }
                        //switch (VerseCountComboBox.SelectedIndex)
                        //{
                        //}

                        VerseCountNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        VerseCountNumericUpDown.Enabled = true;
                    }
                }

                if (sender == WordCountComboBox)
                {
                    if (WordCountComboBox.SelectedIndex > 0)
                    {
                        WordCountNumericUpDown.Value = m_word_count;
                        WordCountNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        WordCountNumericUpDown.Enabled = true;
                    }
                }

                if (sender == LetterCountComboBox)
                {
                    if (LetterCountComboBox.SelectedIndex > 0)
                    {
                        LetterCountNumericUpDown.Value = m_letter_count;
                        LetterCountNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        LetterCountNumericUpDown.Enabled = true;
                    }
                }

                if (sender == CPlusVSumComboBox)
                {
                    if (CPlusVSumComboBox.SelectedIndex > 0)
                    {
                        CPlusVSumNumericUpDown.Value = m_cplusv_sum;
                        CPlusVSumNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        CPlusVSumNumericUpDown.Enabled = true;
                    }
                }

                if (sender == CMinusVSumComboBox)
                {
                    if (CMinusVSumComboBox.SelectedIndex > 0)
                    {
                        CMinusVSumNumericUpDown.Value = (m_cminusv_sum == int.MaxValue) ? 0 : m_cminusv_sum;
                        CMinusVSumNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        CMinusVSumNumericUpDown.Enabled = true;
                    }
                }

                if (sender == CTimesVSumComboBox)
                {
                    if (CTimesVSumComboBox.SelectedIndex > 0)
                    {
                        CTimesVSumNumericUpDown.Value = m_ctimesv_sum;
                        CTimesVSumNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        CTimesVSumNumericUpDown.Enabled = true;
                    }
                }

                if (sender == MatchCountComboBox)
                {
                    if (MatchCountComboBox.SelectedIndex > 0)
                    {
                        MatchCountNumericUpDown.Value = m_match_count;
                        MatchCountNumericUpDown.Enabled = false;
                    }
                    else
                    {
                        MatchCountNumericUpDown.Enabled = true;
                    }
                }
            }
            finally
            {
                VerseCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                WordCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                LetterCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CPlusVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CMinusVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                CTimesVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
                MatchCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            }
        }
    }
    private void TODO_UpdateOtherNumberTypes(object sender)
    {
        try
        {
            VerseCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            WordCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            LetterCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CPlusVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CMinusVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CTimesVSumNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            MatchCountNumericUpDown.ValueChanged -= new EventHandler(QueryParameterNumericUpDown_ValueChanged);

            if ((sender != VerseCountComboBox) && (sender != VerseCountNumericUpDown))
            {
                if (VerseCountComboBox.SelectedIndex > 0)
                {
                    VerseCountNumericUpDown.Value = m_verse_count;
                    VerseCountNumericUpDown.Enabled = false;
                }
                else
                {
                    VerseCountNumericUpDown.Enabled = true;
                }
            }

            if ((sender != WordCountComboBox) && (sender != WordCountNumericUpDown))
            {
                if (WordCountComboBox.SelectedIndex > 0)
                {
                    WordCountNumericUpDown.Value = m_word_count;
                    WordCountNumericUpDown.Enabled = false;
                }
                else
                {
                    WordCountNumericUpDown.Enabled = true;
                }
            }

            if ((sender != LetterCountComboBox) && (sender != LetterCountNumericUpDown))
            {
                if (LetterCountComboBox.SelectedIndex > 0)
                {
                    LetterCountNumericUpDown.Value = m_letter_count;
                    LetterCountNumericUpDown.Enabled = false;
                }
                else
                {
                    LetterCountNumericUpDown.Enabled = true;
                }
            }

            if ((sender != CPlusVSumComboBox) && (sender != CPlusVSumNumericUpDown))
            {
                if (CPlusVSumComboBox.SelectedIndex > 0)
                {
                    CPlusVSumNumericUpDown.Value = m_cplusv_sum;
                    CPlusVSumNumericUpDown.Enabled = false;
                }
                else
                {
                    CPlusVSumNumericUpDown.Enabled = true;
                }
            }

            if ((sender != CMinusVSumComboBox) && (sender != CMinusVSumNumericUpDown))
            {
                if (CMinusVSumComboBox.SelectedIndex > 0)
                {
                    CMinusVSumNumericUpDown.Value = (m_cminusv_sum == int.MaxValue) ? 0 : m_cminusv_sum;
                    CMinusVSumNumericUpDown.Enabled = false;
                }
                else
                {
                    CMinusVSumNumericUpDown.Enabled = true;
                }
            }

            if ((sender != CTimesVSumComboBox) && (sender != CTimesVSumNumericUpDown))
            {
                if (CTimesVSumComboBox.SelectedIndex > 0)
                {
                    CTimesVSumNumericUpDown.Value = m_ctimesv_sum;
                    CTimesVSumNumericUpDown.Enabled = false;
                }
                else
                {
                    CTimesVSumNumericUpDown.Enabled = true;
                }
            }

            if ((sender != MatchCountComboBox) && (sender != MatchCountNumericUpDown))
            {
                if (MatchCountComboBox.SelectedIndex > 0)
                {
                    MatchCountNumericUpDown.Value = m_match_count;
                    MatchCountNumericUpDown.Enabled = false;
                }
                else
                {
                    MatchCountNumericUpDown.Enabled = true;
                }
            }
        }
        finally
        {
            VerseCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            WordCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            LetterCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CPlusVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CMinusVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            CTimesVSumNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
            MatchCountNumericUpDown.ValueChanged += new EventHandler(QueryParameterNumericUpDown_ValueChanged);
        }
    }

    private bool m_count_only = false;
    private void CountOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_count_only = CountOnlyCheckBox.Checked;

        ChapterOutputFieldCheckBox.Enabled = !m_count_only;
        VersesOutputFieldCheckBox.Enabled = !m_count_only;
        WordsOutputFieldCheckBox.Enabled = !m_count_only;
        LettersOutputFieldCheckBox.Enabled = !m_count_only;
        OutputFormatZeroPadComboBox.Enabled = !m_count_only;
        OutputFormatFieldSeparatorComboBox.Enabled = !m_count_only;
        OutputFormatChapterSeparatorComboBox.Enabled = !m_count_only;

        FindButton.Text = m_count_only ? "&Count" : "&Find";
        ToolTip.SetToolTip(this.FindButton, (m_count_only ? "عد فقط" : "إبحث"));
    }
    private void FindButton_Click(object sender, EventArgs e)
    {
        Start();
    }
    private SubsetFinder m_dataset_finder = null;
    private long m_subset_count = 0L;
    private DateTime m_start_time;
    private static bool m_running = false;
    public static bool Running
    {
        get { return m_running; }
    }
    private void Start()
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_running)
            {
                if (DialogResult.Yes == MessageBox.Show(
                    "Stop searching?",
                    Application.ProductName,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2))
                {
                    Finish(true);
                }
            }
            else // if not running, then start running
            {
                m_start_time = DateTime.Now;

                m_running = true;
                FindButton.Text = "&Cancel";
                ToolTip.SetToolTip(this.FindButton, "إلغاء");
                FindButton.Refresh();

                CountOnlyCheckBox.Visible = false;
                CountOnlyCheckBox.Enabled = false;
                CountOnlyCheckBox.Refresh();

                ChapterOutputFieldCheckBox.Enabled = false;
                VersesOutputFieldCheckBox.Enabled = false;
                WordsOutputFieldCheckBox.Enabled = false;
                LettersOutputFieldCheckBox.Enabled = false;
                OutputFormatZeroPadComboBox.Enabled = false;
                OutputFormatFieldSeparatorComboBox.Enabled = false;
                OutputFormatChapterSeparatorComboBox.Enabled = false;

                SaveMatchesButton.Enabled = false;

                // go out of all NumericUpDown controls to set their Values
                FindButton.Focus();

                VerseCountComboBox.Enabled = false;
                WordCountComboBox.Enabled = false;
                LetterCountComboBox.Enabled = false;
                CPlusVSumComboBox.Enabled = false;
                CMinusVSumComboBox.Enabled = false;
                CTimesVSumComboBox.Enabled = false;
                MatchCountComboBox.Enabled = false;
                ChapterCountNumericUpDown.Enabled = false;
                ChapterSumNumericUpDown.Enabled = false;
                VerseCountNumericUpDown.Enabled = false;
                WordCountNumericUpDown.Enabled = false;
                LetterCountNumericUpDown.Enabled = false;
                CPlusVSumNumericUpDown.Enabled = false;
                CMinusVSumNumericUpDown.Enabled = false;
                CTimesVSumNumericUpDown.Enabled = false;
                MatchCountNumericUpDown.Enabled = false;

                MatchCountTextBox.Text = "0";
                MatchCountTextBox.Refresh();

                ProgressBar.Value = 0;
                ProcessedPercentageLabel.Text = "0%";
                ElapsedTimeValueLabel.Text = "0d 00h 00m 00s";
                RemainingTimeValueLabel.Text = "0d 00h 00m 00s";

                // Just before Run
                CaptureQueryParameters();
                CaptureNumberTypes();

                NumberQuery query = new NumberQuery();
                query.ChapterCount = m_chapter_count;
                query.ChapterSum = m_chapter_sum;
                query.VerseCount = m_verse_count;
                query.WordCount = m_word_count;
                query.LetterCount = m_letter_count;
                query.CPlusVSum = m_cplusv_sum;
                query.CMinusVSum = m_cminusv_sum;
                query.CTimesVSum = m_ctimesv_sum;
                query.MatchCount = m_match_count;

                query.ChapterCountNumberType = m_chapter_count_number_type;
                query.ChapterSumNumberType = m_chapter_sum_number_type;
                query.VerseCountNumberType = m_verse_count_number_type;
                query.WordCountNumberType = m_word_count_number_type;
                query.LetterCountNumberType = m_letter_count_number_type;
                query.CPlusVSumNumberType = m_cplusv_sum_number_type;
                query.CMinusVSumNumberType = m_cminusv_sum_number_type;
                query.CTimesVSumNumberType = m_ctimesv_sum_number_type;
                query.MatchCountNumberType = m_match_count_number_type;

                // Run
                m_subset_count = 0L;
                m_matches_str = new StringBuilder();
                m_dataset_finder = new SubsetFinder(query);
                if (m_dataset_finder != null)
                {
                    if (m_match_count > 0L)
                    {
                        m_subset_count = m_dataset_finder.Count(m_chapter_count, m_chapter_sum);
                        if (m_match_count == m_subset_count)
                        {
                            m_subset_count = 0L;
                            m_dataset_finder.Find(m_chapter_count, m_chapter_sum, OnFound);
                        }
                    }
                    else
                    {
                        if (m_count_only)
                        {
                            m_subset_count = m_dataset_finder.Count(m_chapter_count, m_chapter_sum);
                        }
                        else
                        {
                            m_dataset_finder.Find(m_chapter_count, m_chapter_sum, OnFound);
                        }
                    }
                }

                if (m_running) // if finished normally
                {
                    if (!m_count_only)
                    {
                        SaveMatchesButton_Click(null, null);
                    }

                    Finish(false);
                }
            }
        }
        catch (Exception ex)
        {
            while (ex != null)
            {
                //Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message, Application.ProductName);
                ex = ex.InnerException;
            }
            Finish(true);
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void Finish(bool cancelled)
    {
        if (m_running)
        {
            m_running = false;
            FindButton.Text = m_count_only ? "&Count" : "&Find";
            ToolTip.SetToolTip(this.FindButton, (m_count_only ? "عد فقط" : "إبحث"));
            FindButton.Refresh();

            CountOnlyCheckBox.Visible = true;
            CountOnlyCheckBox.Enabled = true;
            CountOnlyCheckBox.Refresh();

            ChapterOutputFieldCheckBox.Enabled = !m_count_only;
            VersesOutputFieldCheckBox.Enabled = !m_count_only;
            WordsOutputFieldCheckBox.Enabled = !m_count_only;
            LettersOutputFieldCheckBox.Enabled = !m_count_only;
            OutputFormatZeroPadComboBox.Enabled = !m_count_only;
            OutputFormatFieldSeparatorComboBox.Enabled = !m_count_only;
            OutputFormatChapterSeparatorComboBox.Enabled = !m_count_only;

            VerseCountComboBox.Enabled = true;
            WordCountComboBox.Enabled = true;
            LetterCountComboBox.Enabled = true;
            CPlusVSumComboBox.Enabled = true;
            CMinusVSumComboBox.Enabled = true;
            CTimesVSumComboBox.Enabled = true;
            MatchCountComboBox.Enabled = true;
            ChapterCountNumericUpDown.Enabled = true;
            ChapterSumNumericUpDown.Enabled = true;
            VerseCountNumericUpDown.Enabled = (VerseCountComboBox.SelectedIndex <= 0);
            WordCountNumericUpDown.Enabled = (WordCountComboBox.SelectedIndex <= 0);
            LetterCountNumericUpDown.Enabled = (LetterCountComboBox.SelectedIndex <= 0);
            CPlusVSumNumericUpDown.Enabled = (CPlusVSumComboBox.SelectedIndex <= 0);
            CMinusVSumNumericUpDown.Enabled = (CMinusVSumComboBox.SelectedIndex <= 0);
            CTimesVSumNumericUpDown.Enabled = (CTimesVSumComboBox.SelectedIndex <= 0);
            MatchCountNumericUpDown.Enabled = (MatchCountComboBox.SelectedIndex <= 0);

            ChapterCountNumericUpDown.Focus();

            if (m_count_only)
            {
                MatchCountTextBox.Text = m_subset_count.ToString();
                SaveMatchesButton.Enabled = false;
            }
            else
            {
                if (m_matches_str == null)
                {
                    SaveMatchesButton.Enabled = false;
                }
                else
                {
                    SaveMatchesButton.Enabled = (m_matches_str.Length > 0);
                }
            }

            UpdateElapsedTime(cancelled);
        }
    }
    private void UpdateElapsedTime(bool cancelled)
    {
        if (!cancelled)
        {
            // update the final progress regardless of batch_size
            //ProgressBar.Value = (int)((loops * ProgressBar.Maximum) / m_match_count);
            ProgressBar.Value = 100;
            ProgressBar.Refresh();
            //ProcessedPercentageLabel.Text = Math.Ceiling((float)loops * 100 / (float)m_match_count).ToString() + "%";
            ProcessedPercentageLabel.Text = "100%";
            ProcessedPercentageLabel.Refresh();
        }

        TimeSpan elapsed_time = DateTime.Now - m_start_time;
        ElapsedTimeValueLabel.Text = String.Format("{0:d1}d {1:d2}h {2:d2}m {3:d2}s", elapsed_time.Days, elapsed_time.Hours, elapsed_time.Minutes, elapsed_time.Seconds);
        ElapsedTimeValueLabel.Refresh();
        //long ticks_per_loop = (elapsed_time.Ticks / loops);
        //long remaining_ticks = (m_match_count - loops) * ticks_per_loop;
        //TimeSpan remaining_time = new TimeSpan(remaining_ticks);
        //RemainingTimeValueLabel.Text = String.Format("{0:d1}d {1:d2}h {2:d2}m {3:d2}s", remaining_time.Days, remaining_time.Hours, remaining_time.Minutes, remaining_time.Seconds);
        //RemainingTimeValueLabel.Refresh();
    }
    StringBuilder m_matches_str = null;
    private void OnFound(Chapter[] chapters)
    {
        if (m_dataset_finder != null)
        {
            m_subset_count++;

            if (!m_count_only)
            {
                MatchCountTextBox.Text = m_subset_count.ToString();
                MatchCountTextBox.Refresh();

                StringBuilder str = new StringBuilder();
                for (int i = chapters.Length - 1; i >= 0; i--)
                {
                    str.Append((m_include_chapters_output_field ? (chapters[i].Number.ToString(m_chapter_number_format)) : "") +
                               (m_include_verses_output_field ? ((m_include_chapters_output_field ? m_field_seperator : "") + chapters[i].VerseCount.ToString(m_verses_number_format)) : "") +
                               (m_include_words_output_field ? ((m_include_chapters_output_field || m_include_verses_output_field ? m_field_seperator : "") + chapters[i].WordCount.ToString(m_words_number_format)) : "") +
                               (m_include_letters_output_field ? ((m_include_chapters_output_field || m_include_verses_output_field || m_include_words_output_field ? m_field_seperator : "") + chapters[i].LetterCount.ToString(m_letters_number_format)) : "") +
                               m_chapter_seperator);
                }
                if (str.Length > 0)
                {
                    if (m_chapter_seperator != "\r\n")
                    {
                        str.Remove(str.Length - m_chapter_seperator.Length, m_chapter_seperator.Length);
                    }
                }
                m_matches_str.Insert(0, str.ToString() + "\r\n");
            }
        }
    }

    private bool m_zero_padded_format = true;
    private string m_field_seperator = ".";
    private string m_chapter_seperator = "\t";
    private string m_chapter_number_format;
    private string m_verses_number_format;
    private string m_words_number_format;
    private string m_letters_number_format;
    private void PopulateOutputFormatComboBox()
    {
        OutputFormatZeroPadComboBox.Items.Clear();
        OutputFormatZeroPadComboBox.Items.Add("000");
        OutputFormatZeroPadComboBox.Items.Add("0");
        if (OutputFormatZeroPadComboBox.Items.Count > 0) OutputFormatZeroPadComboBox.SelectedIndex = 0;

        OutputFormatFieldSeparatorComboBox.Items.Clear();
        OutputFormatFieldSeparatorComboBox.Items.Add("Dot");
        OutputFormatFieldSeparatorComboBox.Items.Add("Space");
        OutputFormatFieldSeparatorComboBox.Items.Add("Comma");
        OutputFormatFieldSeparatorComboBox.Items.Add("Dash");
        OutputFormatFieldSeparatorComboBox.Items.Add("Tab");
        if (OutputFormatFieldSeparatorComboBox.Items.Count > 0) OutputFormatFieldSeparatorComboBox.SelectedIndex = 0;

        OutputFormatChapterSeparatorComboBox.Items.Clear();
        OutputFormatChapterSeparatorComboBox.Items.Add("Tab");
        OutputFormatChapterSeparatorComboBox.Items.Add("TabTab");
        OutputFormatChapterSeparatorComboBox.Items.Add("NewLine");
        if (OutputFormatChapterSeparatorComboBox.Items.Count > 0) OutputFormatChapterSeparatorComboBox.SelectedIndex = 0;
    }
    private void OutputFormatZeroPadComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (OutputFormatZeroPadComboBox.SelectedIndex)
        {
            case 0:
                m_zero_padded_format = true;
                break;
            case 1:
                m_zero_padded_format = false;
                break;
            default:
                break;
        }

        m_chapter_number_format = m_zero_padded_format ? "000" : "";
        m_verses_number_format = m_zero_padded_format ? "000" : "";
        m_words_number_format = m_zero_padded_format ? "0000" : "";
        m_letters_number_format = m_zero_padded_format ? "00000" : "";
    }
    private void OutputFormatFieldSeparatorComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (OutputFormatFieldSeparatorComboBox.SelectedIndex)
        {
            case 0:
                m_field_seperator = ".";
                break;
            case 1:
                m_field_seperator = " ";
                break;
            case 2:
                m_field_seperator = ",";
                break;
            case 3:
                m_field_seperator = "-";
                break;
            case 4:
                m_field_seperator = "\t";
                break;
            default:
                break;
        }
    }
    private void OutputFormatChapterSeparatorComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (OutputFormatChapterSeparatorComboBox.SelectedIndex)
        {
            case 0:
                m_chapter_seperator = "\t";
                break;
            case 1:
                m_chapter_seperator = "\t\t";
                break;
            case 2:
                m_chapter_seperator = "\r\n";
                break;
            default:
                break;
        }
    }
    private bool m_include_chapters_output_field = true;
    private bool m_include_verses_output_field = true;
    private bool m_include_words_output_field = true;
    private bool m_include_letters_output_field = true;
    private void ChapterOutputFieldCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_include_chapters_output_field = ChapterOutputFieldCheckBox.Checked;
    }
    private void VersesOutputFieldCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_include_verses_output_field = VersesOutputFieldCheckBox.Checked;
    }
    private void WordsOutputFieldCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_include_words_output_field = WordsOutputFieldCheckBox.Checked;
    }
    private void LettersOutputFieldCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_include_letters_output_field = LettersOutputFieldCheckBox.Checked;
    }
    public const string STATISTIC_FOLDER = "Statistics";
    private void SaveMatchesButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_matches_str != null)
            {
                if ((MatchCountTextBox.Text != "") && (MatchCountTextBox.Text != "0"))
                {
                    string filename = "C" + m_chapter_count.ToString() + "_" +
                                      "s" + m_chapter_sum.ToString() + "_" +
                                      "V" + m_verse_count.ToString() + "_" +
                                      "W" + m_word_count.ToString() + "_" +
                                      "L" + m_letter_count.ToString() + ".txt";

                    if (!Directory.Exists(STATISTIC_FOLDER))
                    {
                        Directory.CreateDirectory(STATISTIC_FOLDER);
                    }
                    string path = STATISTIC_FOLDER + "/" + filename;

                    FileHelper.SaveText(path, m_matches_str.ToString());
                    FileHelper.DisplayFile(path);
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }

    private void WebsiteLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            Control control = (sender as Control);
            if (control != null)
            {
                if (control.Tag != null)
                {
                    if (!String.IsNullOrEmpty(control.Tag.ToString()))
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(control.Tag.ToString());
                        }
                        catch (Exception ex)
                        {
                            while (ex != null)
                            {
                                //Console.WriteLine(ex.Message);
                                MessageBox.Show(ex.Message, Application.ProductName);
                                ex = ex.InnerException;
                            }
                        }
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
