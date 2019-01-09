using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using Model;

public partial class MainForm : Form
{
    #region Languages
    ///////////////////////////////////////////////////////////////////////////////
    private string l = DEFAULT_LANGUAGE;
    private List<string> m_language_names = null;
    private void LanguageLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            Control control = sender as Control;
            if (control != null)
            {
                int pos = control.Name.IndexOf("LanguageLabel");
                if (pos > -1)
                {
                    l = control.Name.Remove(pos);
                    LoadLanguage(l);
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void LoadLanguageNames()
    {
        if (Directory.Exists(Globals.LANGUAGES_FOLDER))
        {
            if (m_language_names == null)
            {
                m_language_names = new List<string>();
            }
            if (m_language_names != null)
            {
                m_language_names.Clear();

                DirectoryInfo folder = new DirectoryInfo(Globals.LANGUAGES_FOLDER);
                if (folder != null)
                {
                    FileInfo[] files = folder.GetFiles("*.txt");
                    if ((files != null) && (files.Length > 0))
                    {
                        foreach (FileInfo file in files)
                        {
                            try
                            {
                                if (!String.IsNullOrEmpty(file.Name))
                                {
                                    int pos = file.Name.IndexOf(file.Extension);
                                    if (pos > -1)
                                    {
                                        m_language_names.Add(file.Name.Remove(pos));
                                    }
                                }
                            }
                            catch
                            {
                                // skip non-conformant language
                            }
                        }
                    }
                }
            }
        }
    }
    private void ApplyLanguage(string name)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_language_names != null)
            {
                for (int i = 0; i < m_language_names.Count; i++)
                {
                    if (name == m_language_names[i])
                    {
                        l = name;
                        LoadLanguage(l);
                    }
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private List<Control> GetDescendentControls(Control control)
    {
        List<Control> result = new List<Control>();
        foreach (Control c in control.Controls)
        {
            result.Add(c);
            result.AddRange(GetDescendentControls(c));
        }
        return result;
    }
    private Dictionary<string, Dictionary<string, string>> L = new Dictionary<string, Dictionary<string, string>>();
    private void LoadLanguage(string language_name)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (Directory.Exists(Globals.LANGUAGES_FOLDER))
            {
                Dictionary<string, string> language = new Dictionary<string, string>();
                List<Control> controls = GetDescendentControls(this);

                string filename = Globals.LANGUAGES_FOLDER + "/" + language_name + ".txt";
                List<string> lines = FileHelper.LoadLines(filename);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('\t');
                    if (parts.Length >= 2)
                    {
                        if (parts[0] == "Dictionary")
                        {
                            if (parts.Length > 2)
                            {
                                if ((parts[1].Length > 0) && (parts[2].Length > 0))
                                {
                                    language.Add(parts[1], parts[2]);
                                }
                            }
                        }
                        else
                        {
                            foreach (Control control in controls)
                            {
                                if (parts[0] == control.Name)
                                {
                                    // set Text
                                    if (parts.Length > 2)
                                    {
                                        if (parts[2].Length > 0)
                                        {
                                            control.Text = parts[2];
                                        }
                                    }

                                    // set ToolTip
                                    if (parts.Length > 4)
                                    {
                                        if (parts[4].Length > 0)
                                        {
                                            if (control is TabPage)
                                            {
                                                (control as TabPage).ToolTipText = parts[4];
                                            }
                                            else
                                            {
                                                if (control.Name.Contains("PositionLabel")) continue;
                                                ToolTip.SetToolTip(control, parts[4]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (L.ContainsKey(l))
                {
                    L[l] = language;
                }
                else
                {
                    L.Add(language_name, language);
                }

                UpdateHeaderLabel();
                PopulateChaptersListBox();
                DisplayChapterRevelationInfo();

                if (m_found_verses_displayed)
                {
                    if (m_word_wrap_search_textbox)
                    {
                        ToolTip.SetToolTip(WordWrapLabel, L[l]["Wrap"]);
                    }
                    else
                    {
                        ToolTip.SetToolTip(WordWrapLabel, L[l]["Unwrap"]);
                    }
                }
                else
                {
                    if (m_word_wrap_main_textbox)
                    {
                        ToolTip.SetToolTip(WordWrapLabel, L[l]["Wrap"]);
                    }
                    else
                    {
                        ToolTip.SetToolTip(WordWrapLabel, L[l]["Unwrap"]);
                    }
                }

                LetterFrequencyColumnHeader.Text = L[l]["Freq"] + "  "; // + 2 spaces for sort marker after them

                SetToolTips();
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void SetToolTips()
    {
        try
        {
            this.ToolTip.SetToolTip(this.InspectChaptersLabel, L[l]["Inspect chapters"]);
            this.ToolTip.SetToolTip(this.InspectVersesLabel, L[l]["Inspect verses"]);
            this.ToolTip.SetToolTip(this.ChaptersTextBox, L[l]["Chapters in selection"]);
            this.ToolTip.SetToolTip(this.VersesTextBox, L[l]["Verses in selection"]);
            this.ToolTip.SetToolTip(this.WordsTextBox, L[l]["Words in selection"]);
            this.ToolTip.SetToolTip(this.LettersTextBox, L[l]["Letters in selection"]);
            this.ToolTip.SetToolTip(this.ValueTextBox, L[l]["Value of selection"]);
            this.ToolTip.SetToolTip(this.SearchScopeBookLabel, L[l]["Search in entire book"]);
            this.ToolTip.SetToolTip(this.SearchScopeSelectionLabel, L[l]["Search in current selection"]);
            this.ToolTip.SetToolTip(this.SearchScopeResultLabel, L[l]["Search in current search result"]);
            this.ToolTip.SetToolTip(this.FindByTextTextBox, L[l]["text to search for in Arabic or any installed language"]);
            this.ToolTip.SetToolTip(this.FindByTextWordnessCheckBox, L[l]["find verses with whole word only"]);
            this.ToolTip.SetToolTip(this.FindByTextProximitySearchTypeLabel, L[l]["find any/all given words"]);
            this.ToolTip.SetToolTip(this.FindByTextRootSearchTypeLabel, L[l]["find words of given roots"]);
            this.ToolTip.SetToolTip(this.FindByTextAtChapterAnyRadioButton, L[l]["find anywhere in chapters"]);
            this.ToolTip.SetToolTip(this.FindByTextAtChapterStartRadioButton, L[l]["find in first verses"]);
            this.ToolTip.SetToolTip(this.FindByTextAtChapterMiddleRadioButton, L[l]["find in middle verses"]);
            this.ToolTip.SetToolTip(this.FindByTextAtChapterEndRadioButton, L[l]["find in last verses"]);
            this.ToolTip.SetToolTip(this.FindByTextAtVerseAnyRadioButton, L[l]["find anywhere in verses"]);
            this.ToolTip.SetToolTip(this.FindByTextAtVerseStartRadioButton, L[l]["find in first words"]);
            this.ToolTip.SetToolTip(this.FindByTextAtVerseMiddleRadioButton, L[l]["find in middle words"]);
            this.ToolTip.SetToolTip(this.FindByTextAtVerseEndRadioButton, L[l]["find in last words"]);
            this.ToolTip.SetToolTip(this.FindByTextAtWordAnyRadioButton, L[l]["find anywhere in words"]);
            this.ToolTip.SetToolTip(this.FindByTextAtWordStartRadioButton, L[l]["find at the beginning of words"]);
            this.ToolTip.SetToolTip(this.FindByTextAtWordMiddleRadioButton, L[l]["find in the middle of words"]);
            this.ToolTip.SetToolTip(this.FindByTextAtWordEndRadioButton, L[l]["find at the end of words"]);
            this.ToolTip.SetToolTip(this.FindByTextAllWordsRadioButton, L[l]["find verses with all words in any order"]);
            this.ToolTip.SetToolTip(this.FindByTextAnyWordRadioButton, L[l]["find verses with at least one word"]);
            this.ToolTip.SetToolTip(this.ChapterComboBox, "C, C-C, C:V, C:V-C, C-C:V, C:V-C:V, ..." + "\r\n" + "36  40-46  15:87  18:9-25  1-2:5  24:35-27:62  2:29,41:9-12");
            this.ToolTip.SetToolTip(this.ChapterVerseNumericUpDown, "V, V-V, ...");
            this.ToolTip.SetToolTip(this.ChapterWordNumericUpDown, "W, W-W, ...");
            this.ToolTip.SetToolTip(this.ChapterLetterNumericUpDown, "L, L-L, ...");
            this.ToolTip.SetToolTip(this.PartNumericUpDown, "P, P-P, ...");
            this.ToolTip.SetToolTip(this.PageNumericUpDown, "#, #-#, ...");
            this.ToolTip.SetToolTip(this.StationNumericUpDown, "S, S-S, ...");
            this.ToolTip.SetToolTip(this.GroupNumericUpDown, "G, G-G, ...");
            this.ToolTip.SetToolTip(this.HalfNumericUpDown, "H, H-H, ...");
            this.ToolTip.SetToolTip(this.QuarterNumericUpDown, "Q, Q-Q, ...");
        }
        catch
        {
            // ignore
        }
    }
    /////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Framework
    ///////////////////////////////////////////////////////////////////////////////
    // TextBox has no Ctrl+A by default, so add it
    private void FixMicrosoft(object sender, KeyPressEventArgs e)
    {
        // stop annoying beep due to parent not having an AcceptButton
        if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Escape))
        {
            e.Handled = true;
        }
        // enable Ctrl+A to SelectAll
        if ((ModifierKeys == Keys.Control) && (e.KeyChar == (char)1))
        {
            TextBoxBase control = (sender as TextBoxBase);
            if (control != null)
            {
                control.SelectAll();
                e.Handled = true;
            }
        }
    }
    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (sender is TextBoxBase)
        {
            TextBoxBase control = (sender as TextBoxBase);
            if (control != null)
            {
                if (e.KeyCode == Keys.Tab)
                {
                    control.Text.Insert(control.SelectionStart, "\t");
                    //e.Handled = true;
                }
                else
                {
                    if (ModifierKeys == Keys.Control)
                    {
                        if (e.KeyCode == Keys.A)
                        {
                            control.SelectAll();
                        }
                        else if (e.KeyCode == Keys.F)
                        {
                            // Find dialog
                        }
                        else if (e.KeyCode == Keys.H)
                        {
                            // Replace dialog
                        }
                        else if (e.KeyCode == Keys.S)
                        {
                            // Save As dialog
                        }
                    }
                }
            }
        }
    }
    private FontConverter font_converter = new FontConverter();
    private ColorConverter color_converter = new ColorConverter();
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Constants
    ///////////////////////////////////////////////////////////////////////////////
    private static int c = 140;
    private static Color[] CHAPTER_INITIALIZATION_COLORS =
    { 
        /* InitializationType.Key */                  Color.Black,
        /* InitializationType.PartiallyInitialized */ Color.FromArgb(c+32, c+0, 0),
        /* InitializationType.FullyInitialized */     Color.FromArgb(c+64, c+32, 0),
        /* InitializationType.DoublyInitialized */    Color.FromArgb(c+96, c+64, 0),
        /* InitializationType.NonInitialized */       Color.FromArgb(64, 64, 64),
    };

    private const int DEFAULT_WINDOW_WIDTH = 800;
    private const int DEFAULT_WINDOW_HEIGHT = 614;
    private const string DEFAULT_LANGUAGE = "Arabic";
    private const string SUM_SYMBOL = "Ʃ";
    private const string SPACE_GAP = "     ";
    private const string CAPTION_SEPARATOR = " ► ";
    private const string DEFAULT_QURAN_FONT_NAME = "me_quran";
    private const float DEFAULT_QURAN_FONT_SIZE = 14.0F;
    private const int DEFAULT_TRANSLATION_BOX_WIDTH = 409;
    private const string DEFAULT_TRANSALTION_FONT_NAME = "Microsoft Sans Serif";
    private const float DEFAULT_TRANSALTION_FONT_SIZE = 11.0F;
    private static Color DEFAULT_TRANSALTION_FONT_COLOR = Color.Navy;
    private const int SELECTON_SCOPE_TEXT_MAX_LENGTH = 32;  // for longer text, use elipses (...)
    private const float DEFAULT_TEXT_ZOOM_FACTOR = 1.0F;
    private const int DEFAULT_RADIX = 10;                   // base for current number system. Decimal by default.
    private const int DEFAULT_DIVISOR = 19;                 // 19 for OverItNineteen.
    private static Color DIVISOR_COLOR = Color.FromArgb(204, 255, 204); // background color if number is divisible by DEFAULT_DIVISOR or DISTANCES_DIVISOR.
    private const float DEFAULT_DPI_X = 96.0F;              // 100% = 96.0F,   125% = 120.0F,   150% = 144.0F
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region MainForm
    ///////////////////////////////////////////////////////////////////////////////
    private float m_dpi_x = DEFAULT_DPI_X;
    private string m_ini_filename = null;
    private Client m_client = null;
    private string m_current_text = null;
    public MainForm()
    {
        InitializeComponent();
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

        LoadLanguageNames();
        ApplyLanguage(DEFAULT_LANGUAGE);

        InstallFonts();

        using (Graphics graphics = this.CreateGraphics())
        {
            m_dpi_x = graphics.DpiX;
            //if (m_dpi_x == 120.0F)
            //{
            //    // adjust GUI to fit into 125%
            //    MainSplitContainer.Height = (int)(MainSplitContainer.Height / (m_dpi_x / DEFAULT_DPI_X)) + 96;
            //    MainSplitContainer.SplitterDistance = 215;
            //}
        }

        FindByTextButton.Enabled = true;

        m_ini_filename = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", ".ini");

        // must initialize here as it is null
        m_active_textbox = MainTextBox;

        this.MainTextBox.HideSelection = false; // this won't shift the text to the left
        //this.MainTextBox.HideSelection = true; // this WILL shift the text to the left
        this.SearchResultTextBox.HideSelection = false; // this won't shift the text to the left
        //this.SearchResultTextBox.HideSelection = true; // this WILL shift the text to the left

        this.MainTextBox.MouseWheel += new MouseEventHandler(MainTextBox_MouseWheel);
        this.SearchResultTextBox.MouseWheel += new MouseEventHandler(MainTextBox_MouseWheel);
    }
    private void MainForm_Load(object sender, EventArgs e)
    {
        bool splash_screen_done = false;
        try
        {
            SplashForm splash_form = new SplashForm();
            if (splash_form != null)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    using (splash_form)
                    {
                        splash_form.Show();
                        while (!splash_screen_done)
                        {
                            Application.DoEvents();
                        }
                        splash_form.Close();
                    }
                }, null);

                splash_form.Version += " - " + Globals.SHORT_VERSION;
                splash_form.Progress = 30;
                Thread.Sleep(100);

                string numerology_system_name = LoadNumerologySystemName();
                m_client = new Client(numerology_system_name);
                if (m_client != null)
                {
                    if (m_client.NumerologySystem != null)
                    {
                        LoadApplicationFolders();

                        LoadTextModeSettings();
                        splash_form.Information = "Building book ...";
                        string text_mode = m_client.NumerologySystem.TextMode;
                        m_client.BuildSimplifiedBook(text_mode, m_with_bism_Allah, m_waw_as_word, m_shadda_as_letter);
                        EnableFindByTextControls();
                        splash_form.Progress = 60;
                        Thread.Sleep(100);

                        if (m_client.Book != null)
                        {
                            UpdateNumericMinMax();

                            splash_form.Information = "Loading chapter names ...";
                            PopulateChapterComboBox();
                            PopulateChaptersListBox();
                            splash_form.Progress = 70;
                            Thread.Sleep(100);

                            splash_form.Information = "Loading user settings ...";
                            LoadApplicationSettings();
                            splash_form.Progress = 90;
                            Thread.Sleep(100);

                            // must be before DisplaySelection for Verse.IncludeNumber to take effect
                            ApplyWordWrapSettings();

                            if (m_client.Selection == null)
                            {
                                m_client.Selection = new Selection(m_client.Book, SelectionScope.Chapter, new List<int>() { 0 });
                            }
                            if (m_client.Selection != null)
                            {
                                DisplaySelection(false);

                                splash_form.Progress = 100;
                                Thread.Sleep(100);
                            }
                            UpdateSearchScope();
                        }

                        this.Activate(); // bring to foreground
                    }
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
        }
        finally
        {
            splash_screen_done = true;
            Thread.Sleep(100);  // prevent race-condition to allow splashform.Close()
        }
    }
    private void MainForm_Shown(object sender, EventArgs e)
    {
        // setup C V W L start for distance caluclations
        MainTextBox.AlignToStart();
        Verse verse = GetCurrentVerse();
        if (verse != null)
        {
            if (verse.Chapter != null)
            {
                m_clicked_chapter_number = verse.Chapter.SortedNumber;
            }

            m_clicked_verse_number = verse.Number;

            if (verse.Words.Count > 0)
            {
                Word word = verse.Words[0];
                if (word != null)
                {
                    m_clicked_word_number = word.Number;
                    if (word.Letters.Count > 0)
                    {
                        Letter letter = word.Letters[0];
                        if (letter != null)
                        {
                            m_clicked_letter_number = letter.Number;
                        }
                    }
                }
            }
        }

        ToolTip.SetToolTip(InspectChaptersLabel, L[l]["Inspect chapters"]);
        WordsListBoxLabel.Visible = false;
        WordsListBox.Visible = false;
        WordsListBox.SendToBack();

        // start user at chapter list box
        ChaptersListBox.Focus();
    }
    private void MainForm_Resize(object sender, EventArgs e)
    {
        Verse verse = GetCurrentVerse();
        if (verse != null)
        {
            if (m_active_textbox != null)
            {
                int start = m_active_textbox.SelectionStart;
                int length = m_active_textbox.SelectionLength;
                m_active_textbox.AlignToLineStart();
                m_active_textbox.Select(start, length);
            }
        }
    }
    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            if (WordsListBox.Focused)
            {
                WordsListBox_DoubleClick(null, null);
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        if (e.KeyCode == Keys.Tab)
        {
            e.Handled = false;
        }
        else if (e.KeyCode == Keys.Escape)
        {
            HandleEscapeKeyPress(null, null);
            e.Handled = true; // stop annoying beep
        }
        else if (e.Control && (e.KeyCode == Keys.A)) // SelectAll chapters
        {
            if (ChaptersListBox.Focused)
            {
                for (int i = 0; i < 3; i++) ChaptersListBox.SelectedIndexChanged -= new EventHandler(ChaptersListBox_SelectedIndexChanged);
                for (int i = 0; i < ChaptersListBox.Items.Count - 1; i++)
                {
                    ChaptersListBox.SelectedIndices.Add(i);
                }
                ChaptersListBox.SelectedIndexChanged += new EventHandler(ChaptersListBox_SelectedIndexChanged);
                ChaptersListBox.SelectedIndices.Add(ChaptersListBox.Items.Count - 1);
            }
            else if (WordsListBox.Focused)
            {
                for (int i = 0; i < 3; i++) WordsListBox.SelectedIndexChanged -= new EventHandler(WordsListBox_SelectedIndexChanged);
                for (int i = 0; i < WordsListBox.Items.Count - 1; i++)
                {
                    WordsListBox.SelectedIndices.Add(i);
                }
                WordsListBox.SelectedIndexChanged += new EventHandler(WordsListBox_SelectedIndexChanged);
                WordsListBox.SelectedIndices.Add(WordsListBox.Items.Count - 1);
            }
            else
            {
                e.Handled = false;
            }
        }
        else if (e.Control && (e.KeyCode == Keys.G))
        {
            GLabel_Click(null, null);
        }
        else if (e.Control && (e.KeyCode == Keys.P))
        {
            PLabel_Click(null, null);
        }
        else if (e.Control && (e.KeyCode == Keys.F))
        {
            FLabel_Click(null, null);
        }
        else
        {
            if (!e.Alt && !e.Control && !e.Shift)
            {
                if (e.KeyCode == Keys.F3)
                {
                    if (m_found_verses_displayed)
                    {
                        SelectNextFindMatch();
                    }
                }
                else if (e.KeyCode == Keys.F10)
                {
                }
                else if (e.KeyCode == Keys.F11)
                {
                    ToggleWordWrap();
                }
                else if (e.KeyCode == Keys.F12)
                {
                    if (this.WindowState != FormWindowState.Maximized)
                    {
                        this.WindowState = FormWindowState.Maximized;
                        this.FormBorderStyle = FormBorderStyle.None;
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.FormBorderStyle = FormBorderStyle.Sizable;
                    }
                }
                else
                {
                    // let editor process key
                }
            }
            else if (!e.Alt && !e.Control && e.Shift)
            {
                if (e.KeyCode == Keys.F1)
                {
                }
                else if (e.KeyCode == Keys.F2)
                {
                }
                else if (e.KeyCode == Keys.F3)
                {
                    if (m_found_verses_displayed)
                    {
                        SelectPreviousFindMatch();
                    }
                }
                else if (e.KeyCode == Keys.F4)
                {
                }
                else if (e.KeyCode == Keys.F5)
                {
                }
                else if (e.KeyCode == Keys.F6)
                {
                }
                else if (e.KeyCode == Keys.F7)
                {
                }
                else if (e.KeyCode == Keys.F8)
                {
                }
                else if (e.KeyCode == Keys.F9)
                {
                }
                else if (e.KeyCode == Keys.F10)
                {
                }
                else if (e.KeyCode == Keys.F11)
                {
                }
                else if (e.KeyCode == Keys.F12)
                {
                }
                else
                {
                    // let editor process key
                }
            }
        }
    }
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        //// prevent user from closing from the X close button
        //if (e.CloseReason == CloseReason.UserClosing)
        //{
        //    e.Cancel = true;
        //    this.Visible = false;
        //}
    }
    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        CloseApplication();
    }
    private void CloseApplication()
    {
        try
        {
            // save application options
            SaveApplicationOptions();
        }
        catch
        {
            // silence IO error in case running from read-only media (CD/DVD)
        }
    }
    private void HandleEscapeKeyPress(object sender, KeyEventArgs e)
    {
        if (m_found_verses_displayed)
        {
            if (ChaptersListBox.SelectedIndices.Count > 0)
            {
                DisplaySearchResults();
            }
            else
            {
                SwitchActiveTextBox();
            }
        }
        else
        {
            SwitchActiveTextBox();
        }
    }
    private void EscapeButton_Click(object sender, EventArgs e)
    {
        HandleEscapeKeyPress(null, null);
    }
    private void DisplaySearchResults()
    {
        // must clear to go back to main results not main text
        ChaptersListBox.SelectedIndices.Clear();

        int pos = m_find_result_header.IndexOf(" of ");
        if (pos > -1)
        {
            m_find_result_header = m_find_result_header.Substring(pos + 4);
        }

        m_client.FilterChapters = null;
        ClearFindMatches(); // clear m_find_matches for F3 to work correctly in filtered result
        DisplayFoundVerses(false, false);

        SearchResultTextBox.Focus();
        SearchResultTextBox.Refresh();
    }
    private void SwitchActiveTextBox()
    {
        if (m_active_textbox != null)
        {
            if (m_found_verses_displayed)
            {
                SwitchToMainTextBox();
            }
            else
            {
                SwitchToSearchResultTextBox();
            }

            // this code has been moved out of SelectionChanged and brought to MouseClick and KeyUp
            // to keep all verse translations visible until the user clicks a verse then show one verse translation
            if (m_active_textbox.SelectionLength == 0)
            {
                ToolTip.SetToolTip(InspectChaptersLabel, L[l]["Inspect chapters"]);
                WordsListBoxLabel.Visible = false;
                WordsListBox.Visible = false;
                WordsListBox.SendToBack();
            }
            else
            {
                // selected text is dealt with by CalculateAndDisplayCounts 
                DisplayWordFrequencies();
            }

            UpdateHeaderLabel();

            m_active_textbox.Focus();
            MainTextBox_SelectionChanged(m_active_textbox, null);
        }
    }
    private void UpdateNumericMinMax()
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                PageNumericUpDown.Minimum = 1;
                PageNumericUpDown.Maximum = m_client.Book.Pages.Count;
                StationNumericUpDown.Minimum = 1;
                StationNumericUpDown.Maximum = m_client.Book.Stations.Count;
                PartNumericUpDown.Minimum = 1;
                PartNumericUpDown.Maximum = m_client.Book.Parts.Count;
                GroupNumericUpDown.Minimum = 1;
                GroupNumericUpDown.Maximum = m_client.Book.Groups.Count;
                HalfNumericUpDown.Minimum = 1;
                HalfNumericUpDown.Maximum = m_client.Book.Halfs.Count;
                QuarterNumericUpDown.Minimum = 1;
                QuarterNumericUpDown.Maximum = m_client.Book.Quarters.Count;
                PageNumericUpDown.Minimum = 1;
                PageNumericUpDown.Maximum = m_client.Book.Pages.Count;
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Initialization
    ///////////////////////////////////////////////////////////////////////////////
    private void LoadApplicationFolders()
    {
        if (File.Exists(m_ini_filename))
        {
            try
            {
                using (StreamReader reader = File.OpenText(m_ini_filename))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!String.IsNullOrEmpty(line))
                        {
                            string[] parts = line.Split('=');
                            if (parts.Length >= 2)
                            {
                                switch (parts[0])
                                {
                                    // [Folders]
                                    case "LanguagesFolder":
                                        {
                                            Globals.LANGUAGES_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "FontsFolder":
                                        {
                                            Globals.FONTS_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "ImagesFolder":
                                        {
                                            Globals.IMAGES_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "DataFolder":
                                        {
                                            Globals.DATA_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "RulesFolder":
                                        {
                                            Globals.RULES_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "ValuesFolder":
                                        {
                                            Globals.VALUES_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                    case "StatisticsFolder":
                                        {
                                            Globals.STATISTICS_FOLDER = parts[1].Replace("\\", "/").Trim();
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // silence Parse exceptions
                // continue with next INI entry
            }
        }
    }
    private string LoadNumerologySystemName()
    {
        if (File.Exists(m_ini_filename))
        {
            using (StreamReader reader = File.OpenText(m_ini_filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length >= 2)
                        {
                            if (parts[0] == "NumerologySystem")
                            {
                                try
                                {
                                    return parts[1].Trim();
                                }
                                catch
                                {
                                    return NumerologySystem.DEFAULT_NAME;
                                }
                            }
                        }
                    }
                }
            }
        }
        return NumerologySystem.DEFAULT_NAME;
    }
    private string LoadTextModeSettings()
    {
        if (File.Exists(m_ini_filename))
        {
            using (StreamReader reader = File.OpenText(m_ini_filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length >= 2)
                        {
                            switch (parts[0])
                            {
                                case "WithBismAllah":
                                    {
                                        try
                                        {
                                            m_with_bism_Allah = bool.Parse(parts[1].Trim());
                                        }
                                        catch
                                        {
                                            m_with_bism_Allah = true;
                                        }
                                    }
                                    break;
                                case "WawAsWord":
                                    {
                                        try
                                        {
                                            m_waw_as_word = bool.Parse(parts[1].Trim());
                                        }
                                        catch
                                        {
                                            m_waw_as_word = false;
                                        }
                                    }
                                    break;
                                case "ShaddaAsLetter":
                                    {
                                        try
                                        {
                                            m_shadda_as_letter = bool.Parse(parts[1].Trim());
                                        }
                                        catch
                                        {
                                            m_shadda_as_letter = false;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
        return NumerologySystem.DEFAULT_NAME;
    }
    private void LoadApplicationSettings()
    {
        try
        {
            // must be after the populates...
            LoadApplicationOptions();

            // WARNING: updates size BUT loses the font face in right-to-left RichTextBox
            //SetFontSize(m_font_size);
            // so use ZoomFactor instead
            MainTextBox.ZoomFactor = m_text_zoom_factor;
            SearchResultTextBox.ZoomFactor = m_text_zoom_factor;
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
    private void LoadApplicationOptions()
    {
        try
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (File.Exists(m_ini_filename))
                    {
                        // Selection.Scope and Selection.Indexes are immutable/readonly so create a new Selection to replace m_client.Selection 
                        SelectionScope selection_scope = SelectionScope.Book;
                        List<int> selection_indexes = new List<int>();

                        using (StreamReader reader = File.OpenText(m_ini_filename))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                if (!String.IsNullOrEmpty(line))
                                {
                                    if (line.StartsWith("#")) continue;

                                    string[] parts = line.Split('=');
                                    if (parts.Length >= 2)
                                    {
                                        switch (parts[0])
                                        {
                                            // [Window]
                                            case "Top":
                                                {
                                                    try
                                                    {
                                                        this.Top = int.Parse(parts[1].Trim());
                                                    }
                                                    catch
                                                    {
                                                        this.Top = 100;
                                                    }
                                                }
                                                break;
                                            case "Left":
                                                {
                                                    try
                                                    {
                                                        this.Left = int.Parse(parts[1].Trim());
                                                    }
                                                    catch
                                                    {
                                                        this.Left = 100;
                                                    }
                                                }
                                                break;
                                            case "Width":
                                                {
                                                    try
                                                    {
                                                        this.Width = int.Parse(parts[1].Trim());
                                                    }
                                                    catch
                                                    {
                                                        this.Width = DEFAULT_WINDOW_WIDTH;
                                                    }
                                                }
                                                break;
                                            case "Height":
                                                {
                                                    try
                                                    {
                                                        this.Height = int.Parse(parts[1].Trim());
                                                    }
                                                    catch
                                                    {
                                                        this.Height = DEFAULT_WINDOW_HEIGHT;
                                                    }
                                                }
                                                break;
                                            case "Language":
                                                {
                                                    string name = parts[1].Trim();
                                                    ApplyLanguage(name);
                                                }
                                                break;
                                            // [Display]
                                            case "MainTextWordWrap":
                                                {
                                                    try
                                                    {
                                                        m_word_wrap_main_textbox = bool.Parse(parts[1].Trim());
                                                    }
                                                    catch
                                                    {
                                                        m_word_wrap_main_textbox = false;
                                                    }
                                                }
                                                break;
                                            case "SearchResultWordWrap":
                                                {
                                                    try
                                                    {
                                                        m_word_wrap_search_textbox = bool.Parse(parts[1].Trim());
                                                    }
                                                    catch
                                                    {
                                                        m_word_wrap_search_textbox = false;
                                                    }
                                                }
                                                break;
                                            case "SelectionScope":
                                                {
                                                    try
                                                    {
                                                        selection_scope = (SelectionScope)Enum.Parse(typeof(SelectionScope), parts[1].Trim());
                                                    }
                                                    catch
                                                    {
                                                        selection_scope = SelectionScope.Chapter;
                                                    }
                                                }
                                                break;
                                            case "SelectionIndexes":
                                                {
                                                    try
                                                    {
                                                        string part = parts[1].Trim();
                                                        string[] sub_parts = part.Split('+');
                                                        selection_indexes.Clear();
                                                        for (int i = 0; i < sub_parts.Length; i++)
                                                        {
                                                            int index = int.Parse(sub_parts[i].Trim()) - 1;
                                                            selection_indexes.Add(index);
                                                        }
                                                        m_client.Selection = new Selection(m_client.Book, selection_scope, selection_indexes);
                                                    }
                                                    catch
                                                    {
                                                        selection_indexes.Add(0);
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else // first Application launch
                    {
                        this.StartPosition = FormStartPosition.CenterScreen;
                        this.Width = DEFAULT_WINDOW_WIDTH;
                        this.Height = DEFAULT_WINDOW_HEIGHT;

                        ApplyLanguage(DEFAULT_LANGUAGE);

                        if (this.ChapterComboBox.Items.Count > 0)
                        {
                            this.ChapterComboBox.SelectedIndex = 0;
                        }

                        // select chapter Al-Fatiha as default
                        m_client.Selection = new Selection(m_client.Book, SelectionScope.Chapter, new List<int>() { 0 });
                    }

                    ApplyFont(DEFAULT_QURAN_FONT_NAME, DEFAULT_QURAN_FONT_SIZE);
                }
            }
        }
        catch
        {
            // silence Parse exceptions
            // continue with next INI entry
        }
    }
    private void SaveApplicationOptions()
    {
        try
        {
            if (m_client != null)
            {
                using (StreamWriter writer = new StreamWriter(m_ini_filename, false, Encoding.Unicode))
                {
                    if (this.WindowState == FormWindowState.Maximized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }

                    writer.WriteLine("[Window]");
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        // restore or width/height will be saved as 0
                        writer.WriteLine("Top" + "=" + (Screen.PrimaryScreen.WorkingArea.Height - DEFAULT_WINDOW_HEIGHT) / 2);
                        writer.WriteLine("Left" + "=" + (Screen.PrimaryScreen.WorkingArea.Width - DEFAULT_WINDOW_WIDTH) / 2);
                        writer.WriteLine("Width" + "=" + DEFAULT_WINDOW_WIDTH);
                        writer.WriteLine("Height" + "=" + DEFAULT_WINDOW_HEIGHT);
                    }
                    else
                    {
                        writer.WriteLine("Top" + "=" + this.Top);
                        writer.WriteLine("Left" + "=" + this.Left);
                        writer.WriteLine("Width" + "=" + this.Width);
                        writer.WriteLine("Height" + "=" + this.Height);
                    }
                    writer.WriteLine("Language" + "=" + l);
                    writer.WriteLine();

                    writer.WriteLine("[Display]");
                    writer.WriteLine("MainTextWordWrap" + "=" + m_word_wrap_main_textbox);
                    writer.WriteLine("SearchResultWordWrap" + "=" + m_word_wrap_search_textbox);
                    if (m_client != null)
                    {
                        if (m_client.Selection != null)
                        {
                            writer.WriteLine("SelectionScope" + "=" + (int)m_client.Selection.Scope);
                            StringBuilder str = new StringBuilder("SelectionIndexes=");
                            if (m_client.Selection.Indexes.Count > 0)
                            {
                                foreach (int index in m_client.Selection.Indexes)
                                {
                                    str.Append((index + 1).ToString() + "+");
                                }
                                if (str.Length > 1)
                                {
                                    str.Remove(str.Length - 1, 1);
                                }
                            }
                            writer.WriteLine(str);
                        }
                    }

                    writer.WriteLine("QuranFont" + "=" + font_converter.ConvertToString(m_quran_font));
                    writer.WriteLine("TextZoomFactor" + "=" + m_text_zoom_factor);
                    writer.WriteLine();

                    writer.WriteLine("[Folders]");
                    writer.WriteLine("LanguagesFolder=" + Globals.LANGUAGES_FOLDER);
                    writer.WriteLine("FontsFolder=" + Globals.FONTS_FOLDER);
                    writer.WriteLine("ImagesFolder=" + Globals.IMAGES_FOLDER);
                    writer.WriteLine("DataFolder=" + Globals.DATA_FOLDER);
                    writer.WriteLine("RulesFolder=" + Globals.RULES_FOLDER);
                    writer.WriteLine("ValuesFolder=" + Globals.VALUES_FOLDER);
                    writer.WriteLine("StatisticsFolder=" + Globals.STATISTICS_FOLDER);
                }
            }
        }
        catch
        {
            // silence IO errors in case running from read-only media (CD/DVD)
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region MainTextBox
    ///////////////////////////////////////////////////////////////////////////////
    private float m_text_zoom_factor = DEFAULT_TEXT_ZOOM_FACTOR;
    private Point m_previous_location = new Point(0, 0);
    private int m_clicked_chapter_number = 1;
    private int m_clicked_verse_number = 1;
    private int m_clicked_word_number = 1;
    private int m_clicked_letter_number = 1;
    private float m_min_zoom_factor = 0.1F;
    private float m_max_zoom_factor = 2.0F;
    private float m_zoom_factor_increment = 0.1F;
    private float m_error_margin = 0.001F;
    private Font m_quran_font = null;
    private List<Font> m_quran_fonts = null;
    private void InstallFonts()
    {
        if (Directory.Exists(Globals.FONTS_FOLDER))
        {
            BuildFonts(Globals.FONTS_FOLDER);
        }
    }
    //private void BuildFonts(Assembly resources_assembly)
    //{
    //    if (m_fonts == null)
    //    {
    //        m_fonts = new List<Font>();
    //    }

    //    if (m_fonts != null)
    //    {
    //        m_fonts.Clear();

    //        if (resources_assembly != null)
    //        {
    //            string[] resource_names = resources_assembly.GetManifestResourceNames();
    //            foreach (string resource_name in resource_names)
    //            {
    //                try
    //                {
    //                    Stream font_stream = resources_assembly.GetManifestResourceStream(resource_name);
    //                    Thread.Sleep(100); // time to refresh Windows resources
    //                    if (font_stream != null)
    //                    {
    //                        //string font_name = resource_name.Remove(resource_name.Length - 4, 4);
    //                        //int start = font_name.LastIndexOf(".");
    //                        //font_name = font_name.Substring(start + 1);
    //                        //Font font = FontBuilder.Build(font_stream, font_name, m_main_font.Size * ((font_name.Contains("Mushaf")) ? 1.33F : 1));
    //                        Font font = FontBuilder.Build(font_stream, m_main_font.Name, m_main_font.Size * ((m_main_font.Name.Contains("Mushaf")) ? 1.33F : 1));
    //                        if (font != null)
    //                        {
    //                            m_fonts.Add(font);
    //                        }
    //                    }
    //                }
    //                catch
    //                {
    //                    // skip non-conformant font
    //                }
    //            }
    //        }
    //    }
    //}
    private void BuildFonts(string fonts_folder)
    {
        if (m_quran_fonts == null)
        {
            m_quran_fonts = new List<Font>();
        }

        if (m_quran_fonts != null)
        {
            m_quran_fonts.Clear();

            DirectoryInfo folder = new DirectoryInfo(fonts_folder);
            if (folder != null)
            {
                FileInfo[] files = folder.GetFiles("*.ttf");
                if ((files != null) && (files.Length > 0))
                {
                    foreach (FileInfo file in files)
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(file.FullName))
                            {
                                Font font = FontBuilder.Build(file.FullName, DEFAULT_QURAN_FONT_SIZE);
                                Thread.Sleep(100); // time to refresh Windows resources
                                if (font != null)
                                {
                                    m_quran_fonts.Add(font);
                                }
                                else
                                {
                                    font = new Font("Courier New", DEFAULT_QURAN_FONT_SIZE);
                                    if (font != null)
                                    {
                                        m_quran_fonts.Add(font);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // skip non-conformant font
                        }
                    }
                }
            }
        }
    }
    private void ApplyFont(string font_name, float font_size)
    {
        if (m_quran_fonts != null)
        {
            for (int i = 0; i < m_quran_fonts.Count; i++)
            {
                m_quran_font = m_quran_fonts[i];
                if (m_quran_font != null)
                {
                    if (m_quran_font.Name == font_name)
                    {
                        ApplyFont(i);
                        break;
                    }
                }
            }
        }
    }
    private void ApplyFont(int index)
    {
        if (m_quran_fonts != null)
        {
            if ((index >= 0) && (index < m_quran_fonts.Count))
            {
                m_quran_font = m_quran_fonts[index];
                if (m_quran_font != null)
                {
                    try
                    {
                        MainTextBox.BeginUpdate();
                        SearchResultTextBox.BeginUpdate();

                        MainTextBox.Font = m_quran_font;
                        SearchResultTextBox.Font = m_quran_font;
                        // MUST DO IT AGAIN for it to work !!!
                        MainTextBox.Font = m_quran_font;
                        SearchResultTextBox.Font = m_quran_font;
                        // MUST DO IT AGAIN for it to work !!!
                        MainTextBox.Font = m_quran_font;
                        SearchResultTextBox.Font = m_quran_font;
                        // MUST DO IT AGAIN for it to work !!!
                        MainTextBox.Font = m_quran_font;
                        SearchResultTextBox.Font = m_quran_font;
                        // MUST DO IT AGAIN for it to work !!!
                        MainTextBox.Font = m_quran_font;
                        SearchResultTextBox.Font = m_quran_font;

                        MainTextBox.AlignToStart();
                        SearchResultTextBox.AlignToStart();
                        MainTextBox.Refresh();
                        SearchResultTextBox.Refresh();
                    }
                    finally
                    {
                        MainTextBox.EndUpdate();
                        SearchResultTextBox.EndUpdate();
                    }
                }
            }
        }
    }
    private void MainTextBox_TextChanged(object sender, EventArgs e)
    {
        if (
             ((sender != null) && (sender == m_active_textbox)) &&
             (
               (m_active_textbox.Focused) ||
               (ChaptersListBox.Focused) ||
               (ChapterComboBox.Focused) ||
               (ChapterVerseNumericUpDown.Focused) ||
               (ChapterWordNumericUpDown.Focused) ||
               (ChapterLetterNumericUpDown.Focused) ||
               (PageNumericUpDown.Focused) ||
               (StationNumericUpDown.Focused) ||
               (PartNumericUpDown.Focused) ||
               (GroupNumericUpDown.Focused) ||
               (HalfNumericUpDown.Focused) ||
               (QuarterNumericUpDown.Focused)
             )
           )
        {
            if (m_client != null)
            {
                CalculateCurrentValue();

                BuildLetterFrequencies();
                DisplayLetterFrequencies();
            }
        }
    }
    private void MainTextBox_SelectionChanged(object sender, EventArgs e)
    {
        if (
             ((sender != null) && (sender == m_active_textbox)) &&
             (
               (m_active_textbox.Focused) ||
               (ChapterWordNumericUpDown.Focused) ||
               (ChapterLetterNumericUpDown.Focused)
             )
           )
        {
            if (m_client != null)
            {
                m_selection_mode = false;

                Verse previous_verse = GetCurrentVerse();
                Verse verse = GetVerseAtCursor();
                if (verse != null)
                {
                    if (verse != previous_verse)
                    {
                        CurrentVerseIndex = GetVerseIndex(verse);
                        UpdateHeaderLabel();
                    }

                    CalculateCurrentValue();

                    BuildLetterFrequencies();
                    DisplayLetterFrequencies();

                    DisplayCurrentPositions();

                    if (m_active_textbox.SelectionLength > 0)
                    {
                        DisplayWordFrequencies();
                    }
                    else
                    {
                        ToolTip.SetToolTip(InspectChaptersLabel, L[l]["Inspect chapters"]);
                        WordsListBoxLabel.Visible = false;
                        WordsListBox.Visible = false;
                        WordsListBox.SendToBack();
                    }
                }
            }
        }
    }
    private void MainTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if ((e.Control) && (e.KeyCode == Keys.C))
        {
            if (m_active_textbox != null)
            {
                if (m_active_textbox.SelectionLength == 0)
                {
                    List<Verse> selected_verses = GetCurrentVerses();
                    if (selected_verses != null)
                    {
                        StringBuilder str = new StringBuilder();
                        foreach (Verse verse in selected_verses)
                        {
                            str.AppendLine(verse.Chapter.Name + "\t" + verse.Address + "\t" + verse.Text);
                        }
                        Clipboard.SetText(str.ToString());
                        Thread.Sleep(100); // must give chance for Clipboard to refresh its content before Paste
                        e.Handled = true;
                    }
                }
                else
                {
                    m_active_textbox.Copy();
                }
            }
        }
    }
    private void MainTextBox_KeyUp(object sender, KeyEventArgs e)
    {
        UpdateMouseCursor();
    }
    private void MainTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == ' ')
        {
            if (FindByTextTextBox.Focused) return;
            if (ChapterComboBox.Focused) return;
        }

        e.Handled = true; // stop annoying beep
    }
    private void MainTextBox_Enter(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);
        this.AcceptButton = null;
        UpdateMouseCursor();
    }
    private void MainTextBox_MouseEnter(object sender, EventArgs e)
    {
    }
    private void MainTextBox_MouseLeave(object sender, EventArgs e)
    {
        // stop cursor flicker
        if (m_active_textbox != null)
        {
            if (m_active_textbox.Cursor != Cursors.Default)
            {
                m_active_textbox.Cursor = Cursors.Default;
            }
        }
    }
    private void MainTextBox_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            // in case we come from UserTextTextBox
            if (m_active_textbox != null)
            {
                m_active_textbox.Focus();
                MainTextBox_SelectionChanged(m_active_textbox, null);

                // set cursor at mouse RIGHT-click location so we know which word to get related words for
                int start = m_active_textbox.GetCharIndexFromPosition(e.Location);
                if (
                     (start <= m_active_textbox.SelectionStart)
                     ||
                     (start > (m_active_textbox.SelectionStart + m_active_textbox.SelectionLength))
                   )
                {
                    m_active_textbox.Select(start, 0);
                }
            }
        }
    }
    private void MainTextBox_MouseMove(object sender, MouseEventArgs e)
    {
        // stop flickering
        if (
            (Math.Abs(m_previous_location.X - e.X) < 8)
            &&
            (Math.Abs(m_previous_location.Y - e.Y) < 8)
           )
        {
            return;
        }
        m_previous_location = e.Location;

        UpdateMouseCursor();

        Word word = GetWordAtPointer(e);
        if (word != null)
        {
            m_current_word = word;

            // in all cases
            //this.Text = Application.ProductName + " | " + GetSelectionSummary();
            UpdateFindMatchCaption();

            string word_info = GetWordInformation(word);
            if (ModifierKeys == Keys.Control)
            {
                word_info += "\r\n\r\n";
                word_info += GetRelatedWordsInformation(word);
            }
            ToolTip.SetToolTip(m_active_textbox, word_info);

            //// display word info at application caption
            //this.Text += SPACE_GAP +
            //(
            //    word.Verse.Chapter.Name + SPACE_GAP +
            //    L[l]["verse"] + " " + word.Verse.NumberInChapter + "-" + word.Verse.Number + SPACE_GAP +
            //    L[l]["word"] + " " + word.NumberInVerse + "-" + word.NumberInChapter + "-" + word.Number + SPACE_GAP +
            //    word.Transliteration + SPACE_GAP +
            //    word.Text + SPACE_GAP +
            //    word.Meaning + SPACE_GAP +
            //    word.Occurrence.ToString() + "/" + word.Frequency.ToString()
            //);
        }
    }
    private void MainTextBox_MouseUp(object sender, MouseEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            // go to related words to word under mouse pointer
            FindRelatedWords(m_current_word);
        }
    }
    private void MainTextBox_Click(object sender, EventArgs e)
    {
    }
    private void MainTextBox_DoubleClick(object sender, EventArgs e)
    {
        try
        {
            for (int i = 0; i < 3; i++) MainTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
            for (int i = 0; i < 3; i++) MainTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.BeginUpdate();

            if (ModifierKeys == Keys.None)
            {
                // double clicking search result, takes us back to Main text displaying the whole chapter of the double-clicked verse
                if (m_found_verses_displayed)
                {
                    Verse verse = GetCurrentVerse();
                    if (verse != null)
                    {
                        GotoVerse(verse);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            MainTextBox.EndUpdate();
            MainTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
        }
    }
    private void GotoVerse(Verse verse)
    {
        if (verse != null)
        {
            if (verse.Chapter != null)
            {
                if (m_client != null)
                {
                    // select chapter and display it and colorize target verse
                    m_client.Selection = new Selection(m_client.Book, SelectionScope.Chapter, new List<int>() { verse.Chapter.Number - 1 });
                    if (m_client.Selection != null)
                    {
                        SwitchToMainTextBox();

                        m_selection_mode = false;

                        ToolTip.SetToolTip(InspectChaptersLabel, L[l]["Inspect chapters"]);
                        WordsListBoxLabel.Visible = false;
                        WordsListBox.Visible = false;
                        WordsListBox.SendToBack();

                        UpdateSearchScope();

                        DisplaySelectionText();

                        MainTextBox.ClearHighlight();
                        MainTextBox.AlignToStart();
                        HighlightVerse(verse);
                        UpdateHeaderLabel();

                        CalculateCurrentValue();

                        UpdateVersePositions(verse);

                        BuildLetterFrequencies();
                        DisplayLetterFrequencies();

                        // change focus to MainTextBox control insead of SearchTextBox
                        MainTextBox.Focus();

                        ToolTip.SetToolTip(InspectChaptersLabel, L[l]["Inspect chapters"]);
                        WordsListBoxLabel.Visible = false;
                        WordsListBox.Visible = false;
                        WordsListBox.SendToBack();
                    }
                }
            }
        }
    }
    private void MainTextBox_MouseWheel(object sender, MouseEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (m_active_textbox != null)
            {
                ZoomInLabel.Enabled = true;
                ZoomOutLabel.Enabled = true;

                m_text_zoom_factor = m_active_textbox.ZoomFactor;
                if (m_active_textbox.ZoomFactor <= (m_min_zoom_factor + m_error_margin))
                {
                    MainTextBox.ZoomFactor = m_min_zoom_factor;
                    SearchResultTextBox.ZoomFactor = m_min_zoom_factor;
                    ZoomInLabel.Enabled = true;
                    ZoomOutLabel.Enabled = false;
                }
                else if (m_active_textbox.ZoomFactor >= (m_max_zoom_factor - m_error_margin))
                {
                    MainTextBox.ZoomFactor = m_max_zoom_factor;
                    SearchResultTextBox.ZoomFactor = m_max_zoom_factor;
                    ZoomInLabel.Enabled = false;
                    ZoomOutLabel.Enabled = true;
                }

                MainTextBox.ZoomFactor = m_text_zoom_factor;
                SearchResultTextBox.ZoomFactor = m_text_zoom_factor;
            }
        }
    }
    private void ZoomInLabel_Click(object sender, EventArgs e)
    {
        if (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin))
        {
            m_text_zoom_factor += m_zoom_factor_increment;

            MainTextBox.ZoomFactor = m_text_zoom_factor;
            SearchResultTextBox.ZoomFactor = m_text_zoom_factor;
        }
        // re-check same condition after zoom_factor update
        ZoomInLabel.Enabled = (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin));
        ZoomOutLabel.Enabled = true;
    }
    private void ZoomOutLabel_Click(object sender, EventArgs e)
    {
        if (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin))
        {
            m_text_zoom_factor -= m_zoom_factor_increment;

            MainTextBox.ZoomFactor = m_text_zoom_factor;
            SearchResultTextBox.ZoomFactor = m_text_zoom_factor;
        }
        // re-check same condition after zoom_factor update
        ZoomOutLabel.Enabled = (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin));
        ZoomInLabel.Enabled = true;
    }
    // wordwrap mode
    private bool m_word_wrap_main_textbox = false;
    private bool m_word_wrap_search_textbox = false;
    private void ApplyWordWrapSettings()
    {
        try
        {
            MainTextBox.BeginUpdate();
            SearchResultTextBox.BeginUpdate();

            UpdateWordWrapLabel(m_word_wrap_main_textbox);
            MainTextBox.WordWrap = m_word_wrap_main_textbox;
            SearchResultTextBox.WordWrap = m_word_wrap_search_textbox;

            Verse.IncludeNumber = m_word_wrap_main_textbox;
        }
        finally
        {
            MainTextBox.EndUpdate();
            SearchResultTextBox.EndUpdate();
        }
    }
    private void UpdateWordWrapLabel(bool word_wrap)
    {
        if (word_wrap)
        {
            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "text_wrap.png"))
            {
                WordWrapLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "text_wrap.png");
            }
            ToolTip.SetToolTip(WordWrapLabel, L[l]["Wrap"]);
        }
        else
        {
            if (File.Exists(Globals.IMAGES_FOLDER + "/" + "text_unwrap.png"))
            {
                WordWrapLabel.Image = new Bitmap(Globals.IMAGES_FOLDER + "/" + "text_unwrap.png");
            }
            ToolTip.SetToolTip(WordWrapLabel, L[l]["Unwrap"]);
        }
        WordWrapLabel.Refresh();
    }
    private void WordWrapLabel_Click(object sender, EventArgs e)
    {
        ToggleWordWrap();
    }
    // add/remove Verse.EndMark, wrap/unwrap and redisplay
    private void ToggleWordWrap() // F11
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_active_textbox != null)
            {
                m_active_textbox.BeginUpdate();

                Verse current_verse = null;
                if (m_selection_mode == false)
                {
                    current_verse = GetCurrentVerse();
                }

                m_active_textbox.WordWrap = !m_active_textbox.WordWrap;
                if (m_found_verses_displayed)
                {
                    m_word_wrap_search_textbox = m_active_textbox.WordWrap;
                    Verse.IncludeNumber = false;

                    UpdateWordWrapLabel(m_word_wrap_search_textbox);

                    // no text is changed so no need to redisplay and recolorize
                    //DisplayFoundVerses(false);
                }
                else
                {
                    m_word_wrap_main_textbox = m_active_textbox.WordWrap;
                    Verse.IncludeNumber = m_word_wrap_main_textbox;

                    UpdateWordWrapLabel(m_word_wrap_main_textbox);

                    // re-display as verse changed IncludeNumber
                    DisplaySelection(false);
                }

                if (current_verse != null)
                {
                    HighlightVerse(current_verse);
                }
            }
        }
        finally
        {
            m_active_textbox.EndUpdate();
            this.Cursor = Cursors.Default;
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Verses
    ///////////////////////////////////////////////////////////////////////////////
    // navigation
    private int m_current_selection_verse_index = 0;
    private int m_current_found_verse_index = 0;
    private int CurrentVerseIndex
    {
        get
        {
            if (m_found_verses_displayed)
            {
                return m_current_found_verse_index;
            }
            else
            {
                return m_current_selection_verse_index;
            }
        }
        set
        {
            if (m_client != null)
            {
                if (m_found_verses_displayed)
                {
                    if (m_client.FoundVerses != null)
                    {
                        if ((value >= 0) && (value < m_client.FoundVerses.Count))
                        {
                            m_current_found_verse_index = value;
                        }
                    }
                }
                else
                {
                    if (m_client.Selection != null)
                    {
                        if (m_client.Selection.Verses != null)
                        {
                            if ((value >= 0) && (value < m_client.Selection.Verses.Count))
                            {
                                m_current_selection_verse_index = value;
                            }
                        }
                    }
                }
            }
        }
    }
    private Verse GetCurrentVerse()
    {
        return GetVerse(CurrentVerseIndex);
    }
    private List<Verse> GetCurrentVerses()
    {
        List<Verse> result = new List<Verse>();
        char[] separators = { '\n', Constants.OPEN_BRACKET[0] };
        string[] lines = m_current_text.Split(separators);
        int current_verse_index = CurrentVerseIndex;
        for (int i = current_verse_index; i < current_verse_index + lines.Length; i++)
        {
            result.Add(GetVerse(i));
        }
        return result;
    }
    private Verse GetVerse(int verse_index)
    {
        if (m_client != null)
        {
            List<Verse> verses = null;
            if (m_found_verses_displayed)
            {
                verses = m_client.FoundVerses;
            }
            else // m_curent_verses displayed
            {
                if (m_client.Selection != null)
                {
                    verses = m_client.Selection.Verses;
                }
            }

            if (verses != null)
            {
                if ((verse_index >= 0) && (verse_index < verses.Count))
                {
                    return verses[verse_index];
                }
            }
        }
        return null;
    }
    private int GetVerseDisplayStart(Verse verse)
    {
        int start = 0;
        if (m_client != null)
        {
            if (verse != null)
            {
                List<Verse> verses = null;
                if (m_found_verses_displayed)
                {
                    verses = m_client.FoundVerses;
                }
                else
                {
                    if (m_client.Selection != null)
                    {
                        verses = m_client.Selection.Verses;
                    }
                }

                if (verses != null)
                {
                    foreach (Verse v in verses)
                    {
                        if (v == verse) break;

                        if (m_found_verses_displayed)
                        {//                            \t                  \n
                            start += v.Address.Length + 1 + v.Text.Length + 1;
                        }
                        else
                        {
                            start += v.Text.Length + v.Endmark.Length;
                        }
                    }
                }
            }
        }
        return start;
    }
    private int GetVerseDisplayLength(Verse verse)
    {
        int length = 0;
        if (verse != null)
        {
            if (m_found_verses_displayed)
            {//                                \t                       \n
                length = verse.Address.Length + 1 + verse.Text.Length + 1;
            }
            else
            {//                                 { # }  or  \n
                length = verse.Text.Length + verse.Endmark.Length;
            }
        }
        return length;
    }
    private int GetWordDisplayStart(Word word) //??? should be int word_index in RichTextBox
    {
        int start = 0;
        if (word != null)
        {
            if (m_client != null)
            {
                List<Verse> verses = null;
                if (m_found_verses_displayed)
                {
                    verses = m_client.FoundVerses;
                }
                else
                {
                    if (m_client.Selection != null)
                    {
                        verses = m_client.Selection.Verses;
                    }
                }

                foreach (Verse verse in verses)
                {
                    if (verse == word.Verse)  //??? this will bring first matching word only
                    {
                        start += word.Position;
                        break;
                    }
                    start += GetVerseDisplayLength(verse);
                }
            }
        }
        return start;
    }
    private int GetWordDisplayLength(Word word)
    {
        if (word != null)
        {
            if (word.Text != null)
            {
                return word.Text.Length + 1;
            }
        }
        return 0;
    }
    // highlighting verse/word
    private Verse m_previous_highlighted_verse = null;
    private void HighlightVerse(Verse verse)
    {
        if (m_active_textbox != null)
        {
            try
            {
                for (int i = 0; i < 3; i++) m_active_textbox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
                for (int i = 0; i < 3; i++) m_active_textbox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
                m_active_textbox.BeginUpdate();

                // de-highlight previous verse
                if (m_previous_highlighted_verse != null)
                {
                    int start = GetVerseDisplayStart(m_previous_highlighted_verse);
                    int length = GetVerseDisplayLength(m_previous_highlighted_verse);
                    if (m_found_verses_displayed)
                    {
                        if (m_found_verse_backcolors.ContainsKey(m_previous_highlighted_verse))
                        {
                            m_active_textbox.Highlight(start, length - 1, m_found_verse_backcolors[m_previous_highlighted_verse]);
                        }
                        else
                        {
                            m_active_textbox.ClearHighlight(start, length - 1);
                        }
                    }
                    else
                    {
                        m_active_textbox.ClearHighlight(start, length - 1);
                    }
                }

                // highlight this verse
                if (verse != null)
                {
                    int start = GetVerseDisplayStart(verse);
                    int length = GetVerseDisplayLength(verse);
                    m_active_textbox.Highlight(start, length - 1, Color.Lavender);

                    // ####### re-wire MainTextBox_SelectionChanged event
                    m_active_textbox.EndUpdate();
                    m_active_textbox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
                    m_active_textbox.TextChanged += new EventHandler(MainTextBox_TextChanged);
                    CalculateCurrentValue(); // will update translation too !!!

                    // move cursor to verse start
                    m_active_textbox.Select(start, 0);

                    // updates verse position and value when cursor goes to start of verse
                    CurrentVerseIndex = GetVerseIndex(verse);
                    UpdateVersePositions(verse);

                    // backup highlighted verse
                    m_previous_highlighted_verse = verse;
                }
                else
                {
                    m_active_textbox.EndUpdate();
                    m_active_textbox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
                    m_active_textbox.TextChanged += new EventHandler(MainTextBox_TextChanged);
                }
            }
            finally
            {
                //// ####### already re-wired above
                //m_active_textbox.EndUpdate();
                //m_active_textbox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
                //m_active_textbox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            }
        }
    }
    private Word m_previous_highlighted_word = null;
    private void HighlightWord(Word word)
    {
        if (m_active_textbox != null)
        {
            int backup_selection_start = m_active_textbox.SelectionStart;
            int backup_selection_length = m_active_textbox.SelectionLength;

            // de-highlight previous word
            if (m_previous_highlighted_word != null)
            {
                int start = GetWordDisplayStart(m_previous_highlighted_word);
                int length = GetWordDisplayLength(m_previous_highlighted_word);
                m_active_textbox.ClearHighlight(start, length);
            }

            // highlight this word
            if (word != null)
            {
                int start = GetWordDisplayStart(word);
                int length = GetWordDisplayLength(word);
                m_active_textbox.Highlight(start, length, Color.Lavender);

                // backup highlighted word
                m_previous_highlighted_word = word;
            }

            //??? BAD DESIGN: if backup_selection is outside visible area, then this line will scroll to it and loses highlight above
            m_active_textbox.Select(backup_selection_start, backup_selection_length);
        }
    }
    // helpers
    private Verse GetVerseAtCursor()
    {
        if (m_active_textbox != null)
        {
            int start = m_active_textbox.SelectionStart;
            return GetVerseAtChar(start);
        }
        return null;
    }
    private Word GetWordAtCursor()
    {
        if (m_active_textbox != null)
        {
            int char_index = m_active_textbox.SelectionStart;
            if (char_index > 0)
            {
                return GetWordAtChar(char_index);
            }
        }
        return null;
    }
    private Letter GetLetterAtCursor()
    {
        if (m_active_textbox != null)
        {
            int char_index = m_active_textbox.SelectionStart;
            if (char_index > 0)
            {
                return GetLetterAtChar(char_index);
            }
        }
        return null;
    }
    private Verse GetVerseAtPointer(MouseEventArgs e)
    {
        return GetVerseAtLocation(e.Location);
    }
    private Word GetWordAtPointer(MouseEventArgs e)
    {
        return GetWordAtLocation(e.Location);
    }
    private Letter GetLetterAtPointer(MouseEventArgs e)
    {
        return GetLetterAtLocation(e.Location);
    }
    private Verse GetVerseAtLocation(Point mouse_location)
    {
        if (m_active_textbox != null)
        {
            int char_index = m_active_textbox.GetCharIndexFromPosition(mouse_location);
            if (char_index > 0)
            {
                return GetVerseAtChar(char_index);
            }
        }
        return null;
    }
    private Word GetWordAtLocation(Point mouse_location)
    {
        if (m_active_textbox != null)
        {
            int char_index = m_active_textbox.GetCharIndexFromPosition(mouse_location);
            if (char_index > 0)
            {
                return GetWordAtChar(char_index);
            }
        }
        return null;
    }
    private Letter GetLetterAtLocation(Point mouse_location)
    {
        if (m_active_textbox != null)
        {
            int char_index = m_active_textbox.GetCharIndexFromPosition(mouse_location);
            if (char_index > 0)
            {
                return GetLetterAtChar(char_index);
            }
        }
        return null;
    }
    // helper helpers
    private Verse GetVerseAtChar(int char_index)
    {
        if (m_client != null)
        {
            List<Verse> verses = null;
            if (m_found_verses_displayed)
            {
                verses = m_client.FoundVerses;
            }
            else
            {
                if (m_client.Selection != null)
                {
                    verses = m_client.Selection.Verses;
                }
            }

            if (verses != null)
            {
                Verse scanned_verse = null;
                foreach (Verse verse in verses)
                {
                    int start = GetVerseDisplayStart(verse);
                    if (char_index < start)
                    {
                        return scanned_verse;
                    }
                    scanned_verse = verse;
                }
                return scanned_verse;
            }
        }
        return null;
    }
    private Word GetWordAtChar(int char_index)
    {
        Word word = null;
        if (m_client != null)
        {
            if (m_found_verses_displayed)
            {
                List<Verse> verses = m_client.FoundVerses;
                if (verses != null)
                {
                    foreach (Verse verse in verses)
                    {
                        int length = GetVerseDisplayLength(verse);
                        if (char_index >= length)
                        {
                            char_index -= length;
                        }
                        else
                        {
                            // verse found, remove verse address
                            char_index -= verse.Address.Length + 1; // \t

                            int word_index = CalculateWordIndex(verse, char_index);
                            if ((word_index >= 0) && (word_index < verse.Words.Count))
                            {
                                word = verse.Words[word_index];
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (m_client.Selection != null)
                {
                    List<Verse> verses = m_client.Selection.Verses;
                    if (verses != null)
                    {
                        foreach (Verse verse in verses)
                        {
                            if ((char_index >= verse.Text.Length) && (char_index < (verse.Text.Length + verse.Endmark.Length - 1)))
                            {
                                return null; // don't return a word at verse Endmark
                            }

                            int length = GetVerseDisplayLength(verse);
                            if (char_index >= length)
                            {
                                char_index -= length;
                            }
                            else
                            {
                                int word_index = CalculateWordIndex(verse, char_index);
                                if ((word_index >= 0) && (word_index < verse.Words.Count))
                                {
                                    word = verse.Words[word_index];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        return word;
    }
    private Letter GetLetterAtChar(int char_index)
    {
        if (m_client != null)
        {
            if (m_found_verses_displayed)
            {
                List<Verse> verses = m_client.FoundVerses;
                if (verses != null)
                {
                    foreach (Verse verse in verses)
                    {
                        int length = GetVerseDisplayLength(verse);
                        if (char_index >= length)
                        {
                            char_index -= length;
                        }
                        else
                        {
                            // remove verse address
                            char_index -= verse.Address.Length + 1; // \t

                            int letter_index = CalculateLetterIndex(verse, char_index);
                            if ((letter_index >= 0) && (letter_index < verse.LetterCount))
                            {
                                return verse.GetLetter(letter_index);
                            }
                        }
                    }
                }
            }
            else
            {
                if (m_client.Selection != null)
                {
                    List<Verse> verses = m_client.Selection.Verses;
                    if (verses != null)
                    {
                        foreach (Verse verse in verses)
                        {
                            int length = GetVerseDisplayLength(verse);
                            if (char_index >= length)
                            {
                                char_index -= length;
                            }
                            else
                            {
                                int letter_index = CalculateLetterIndex(verse, char_index);
                                if ((letter_index >= 0) && (letter_index < verse.LetterCount))
                                {
                                    return verse.GetLetter(letter_index);
                                }
                            }
                        }
                    }
                }
            }
        }
        return null;
    }
    // helper helper helpers
    /// <summary>
    /// Use only when no duplicate verses are displayed like with VerseRanges or ChapterRanges
    /// </summary>
    /// <param name="verse"></param>
    /// <returns>index of first matching verse</returns>
    private int GetVerseIndex(Verse verse)
    {
        if (m_client != null)
        {
            List<Verse> verses = null;
            if (m_found_verses_displayed)
            {
                verses = m_client.FoundVerses;
            }
            else
            {
                if (m_client.Selection != null)
                {
                    verses = m_client.Selection.Verses;
                }
            }

            if (verses != null)
            {
                int verse_index = -1;
                foreach (Verse v in verses)
                {
                    verse_index++;
                    if (v == verse)
                    {
                        return verse_index;
                    }
                }
            }
        }
        return -1;
    }
    private int CalculateWordIndex(Verse verse, int char_index)
    {
        int word_index = -1;
        if (verse != null)
        {
            string[] word_texts = verse.Text.Split();
            foreach (string word_text in word_texts)
            {
                // skip stopmarks (1-letter words), except real Quranic 1-letter words
                if (
                     (word_text.Length == 1)
                     &&
                     !((word_text == "_") || (word_text == "ص") || (word_text == "ق") || (word_text == "ن") || (word_text == "و"))
                   )
                {
                    // skip stopmark words
                    char_index -= word_text.Length + 1; // 1 for stopmark
                }
                else
                {
                    word_index++;

                    if (char_index <= word_text.Length)
                    {
                        break;
                    }
                    char_index -= word_text.Length + 1; // 1 for space
                }
            }
        }
        return word_index;
    }
    private int CalculateLetterIndex(Verse verse, int char_index)
    {
        int letter_index = -1;
        if (verse != null)
        {
            // before verse start
            if (char_index < 0)
            {
                char_index = 0;
            }
            // after verse end
            else if (char_index >= verse.Text.Length)
            {
                char_index = verse.Text.Length - 1;
            }

            for (int i = 0; i <= char_index; i++)
            {
                if (Constants.ARABIC_LETTERS.Contains(verse.Text[i]))
                {
                    letter_index++;
                }
            }
        }
        return letter_index;
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Chapters
    ///////////////////////////////////////////////////////////////////////////////
    private void PopulateChapterComboBox()
    {
        try
        {
            for (int i = 0; i < 3; i++) ChapterComboBox.SelectedIndexChanged -= new EventHandler(ChapterComboBox_SelectedIndexChanged);
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    ChapterComboBox.BeginUpdate();
                    ChapterComboBox.Items.Clear();
                    if (m_client.Book.Chapters != null)
                    {
                        foreach (Chapter chapter in m_client.Book.Chapters)
                        {
                            ChapterComboBox.Items.Add(chapter.SortedNumber + " - " + chapter.Name);
                        }
                    }
                }
            }
        }
        finally
        {
            ChapterComboBox.EndUpdate();
            ChapterComboBox.SelectedIndexChanged += new EventHandler(ChapterComboBox_SelectedIndexChanged);
        }
    }
    private void PopulateChaptersListBox()
    {
        try
        {
            for (int i = 0; i < 3; i++) ChaptersListBox.SelectedIndexChanged -= new EventHandler(ChaptersListBox_SelectedIndexChanged);
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    ChaptersListBox.BeginUpdate();

                    ChaptersListBox.Items.Clear();
                    ChaptersListBox.ClearItemColors(); // cannot override Items.Clear cos not virtual so use this wrapper method
                    if (m_client.Book.Chapters != null)
                    {
                        if (m_found_verses_displayed)
                        {
                            foreach (Chapter chapter in m_client.Book.Chapters)
                            {
                                ChaptersListBox.Items.Add(String.Format("{0,-3} {2,-3}  {1}", chapter.SortedNumber, chapter.Name, chapter.Verses.Count));

                                int match_count = 0;
                                if (m_matches_per_chapter != null)
                                {
                                    match_count = m_matches_per_chapter[chapter.SortedNumber - 1];
                                }

                                // use color shading to represent match_count visually
                                Color color = ChaptersListBox.BackColor;
                                if (match_count > 0)
                                {
                                    int red = 224;
                                    int green = 224;
                                    int blue = 255;
                                    green -= (match_count * 16);
                                    if (green < 0)
                                    {
                                        red += green;
                                        green = 0;
                                    }
                                    if (red < 0)
                                    {
                                        blue += red;
                                        red = 0;
                                    }
                                    if (blue < 0)
                                    {
                                        blue = 0;
                                    }
                                    color = Color.FromArgb(red, green, blue);
                                }
                                ChaptersListBox.SetItemColor(chapter.SortedNumber - 1, color);

                                int matching_chapters = 0;
                                if (m_matches_per_chapter != null)
                                {
                                    foreach (int chapter_match_count in m_matches_per_chapter)
                                    {
                                        if (chapter_match_count > 0)
                                        {
                                            matching_chapters++;
                                        }
                                    }
                                }
                                ChapterGroupBox.ForeColor = Color.Black;
                                ChapterGroupBox.Text = ((matching_chapters > 99) ? "" : ((matching_chapters > 9) ? " " : "  ")) + matching_chapters + " " + L[l]["Chapters"] + "   ";
                                this.ToolTip.SetToolTip(this.ChapterGroupBox, L[l]["Found chapters"]);
                            }
                        }
                        else // selection displayed
                        {
                            foreach (Chapter chapter in m_client.Book.Chapters)
                            {
                                ChaptersListBox.Items.Add(String.Format("{0,-3} {2,-3}  {1}", chapter.SortedNumber, chapter.Name, chapter.Verses.Count));
                                ChaptersListBox.SetItemColor(chapter.SortedNumber - 1, CHAPTER_INITIALIZATION_COLORS[(int)chapter.InitializationType]);
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            ChaptersListBox.EndUpdate();
            ChaptersListBox.SelectedIndexChanged += new EventHandler(ChaptersListBox_SelectedIndexChanged);
        }
    }
    private Chapter GetCurrentChapter()
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                Verse verse = GetCurrentVerse();
                if (verse != null)
                {
                    return verse.Chapter;
                }
            }
        }
        return null;
    }
    private List<Chapter> GetCurrentChapters()
    {
        List<Chapter> result = new List<Chapter>();
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                List<Verse> verses = GetCurrentVerses();
                if (verses != null)
                {
                    result = m_client.Book.GetChapters(verses);
                }
            }
        }
        return result;
    }

    private void DisplayChapterRevelationInfo()
    {
        if (m_found_verses_displayed) return;

        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (ChapterComboBox.SelectedIndex > -1)
                {
                    int index = ChapterComboBox.SelectedIndex;
                    if (m_client.Book.Chapters != null)
                    {
                        Chapter chapter = m_client.Book.Chapters[index];
                        if (chapter != null)
                        {
                            string arabic_revelation_place = null;
                            switch (chapter.RevelationPlace)
                            {
                                case RevelationPlace.Makkah:
                                    arabic_revelation_place = L[l]["Makkah"];
                                    break;
                                case RevelationPlace.Medina:
                                    arabic_revelation_place = L[l]["Medina"];
                                    break;
                                default:
                                    arabic_revelation_place = "";
                                    break;
                            }
                            ChapterGroupBox.Text = arabic_revelation_place + " - " + chapter.RevelationOrder.ToString() + "        ";
                        }
                    }
                }
                else
                {
                    ChapterGroupBox.Text = "";
                }
                this.ToolTip.SetToolTip(this.ChapterGroupBox, L[l]["Revelation order"]);

                UpdateChapterGroupBoxTextColor();
            }
        }
    }
    private void UpdateSelection()
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (ChaptersListBox.SelectedIndices.Count > 0)
                {
                    SelectionScope scope = SelectionScope.Chapter;
                    List<int> indexes = new List<int>();
                    for (int i = 0; i < ChaptersListBox.SelectedIndices.Count; i++)
                    {
                        int selected_index = ChaptersListBox.SelectedIndices[i];
                        if (m_client.Book.Chapters != null)
                        {
                            if ((selected_index >= 0) && (selected_index < m_client.Book.Chapters.Count))
                            {
                                Chapter chapter = m_client.Book.Chapters[selected_index];
                                if (chapter != null)
                                {
                                    indexes.Add(chapter.Number - 1);
                                }
                            }
                        }
                    }
                    m_client.Selection = new Selection(m_client.Book, scope, indexes);
                }
            }
        }
    }
    private void UpdateChaptersListBox()
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (m_client.Selection != null)
                {
                    try
                    {
                        for (int i = 0; i < 3; i++) ChaptersListBox.SelectedIndexChanged -= new EventHandler(ChaptersListBox_SelectedIndexChanged);
                        if (m_found_verses_displayed)
                        {
                            //??? wrongly removes selections of FindChapters result
                            //??? selects found chapters losing all color-shade information
                            //if (m_client.FoundVerses != null)
                            //{
                            //    List<Chapter> chapters = m_client.Book.GetChapters(m_client.FoundVerses);
                            //    ChaptersListBox.SelectedIndices.Clear();
                            //    foreach (Chapter chapter in chapters)
                            //    {
                            //        ChaptersListBox.SelectedIndices.Add(chapter.Number - 1);
                            //    }
                            //}
                        }
                        else
                        {
                            if (m_client.Selection.Chapters != null)
                            {
                                ChaptersListBox.SelectedIndices.Clear();
                                foreach (Chapter chapter in m_client.Selection.Chapters)
                                {
                                    ChaptersListBox.SelectedIndices.Add(chapter.SortedNumber - 1);
                                }
                            }
                        }
                    }
                    finally
                    {
                        ChaptersListBox.SelectedIndexChanged += new EventHandler(ChaptersListBox_SelectedIndexChanged);
                    }
                }
            }
        }
    }
    private void UpdateMinMaxChapterVerseWordLetter(int chapter_index)
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (m_client.Book.Chapters != null)
                {
                    if ((chapter_index >= 0) && (chapter_index < m_client.Book.Chapters.Count))
                    {
                        Chapter chapter = m_client.Book.Chapters[chapter_index];
                        if (chapter != null)
                        {
                            try
                            {
                                for (int i = 0; i < 3; i++) ChapterVerseNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                                for (int i = 0; i < 3; i++) ChapterWordNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                                for (int i = 0; i < 3; i++) ChapterLetterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);

                                ChapterVerseNumericUpDown.Minimum = 1;
                                ChapterVerseNumericUpDown.Maximum = chapter.Verses.Count;

                                ChapterWordNumericUpDown.Minimum = 1;
                                ChapterWordNumericUpDown.Maximum = chapter.WordCount;

                                ChapterLetterNumericUpDown.Minimum = 1;
                                ChapterLetterNumericUpDown.Maximum = chapter.LetterCount;
                            }
                            finally
                            {
                                ChapterVerseNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                                ChapterWordNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                                ChapterLetterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                            }
                        }
                    }
                }
            }
        }
    }
    private void UpdateChapterGroupBoxTextColor()
    {
        Verse verse = GetCurrentVerse();
        if (verse != null)
        {
            ChapterGroupBox.ForeColor = CHAPTER_INITIALIZATION_COLORS[(int)verse.Chapter.InitializationType];
            ChapterGroupBox.Refresh();
        }
    }

    private void ChapterComboBox_KeyDown(object sender, KeyEventArgs e)
    {
        bool SeparatorKeys = (
            ((e.KeyCode == Keys.Subtract) && (e.Modifiers != Keys.Shift))           // HYPHEN
            || ((e.KeyCode == Keys.OemMinus) && (e.Modifiers != Keys.Shift))        // HYPHEN
            || ((e.KeyCode == Keys.Oemcomma) && (e.Modifiers != Keys.Shift))        // COMMA
            || ((e.KeyCode == Keys.OemSemicolon) && (e.Modifiers == Keys.Shift))    // COLON
            );

        bool NumericKeys = (
            ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            && e.Modifiers != Keys.Shift);

        bool EditKeys = (
            (e.KeyCode == Keys.A && e.Modifiers == Keys.Control) ||
            (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control) ||
            (e.KeyCode == Keys.X && e.Modifiers == Keys.Control) ||
            (e.KeyCode == Keys.C && e.Modifiers == Keys.Control) ||
            (e.KeyCode == Keys.V && e.Modifiers == Keys.Control) ||
            e.KeyCode == Keys.Delete ||
            e.KeyCode == Keys.Back);

        bool NavigationKeys = (
            e.KeyCode == Keys.Up ||
            e.KeyCode == Keys.Right ||
            e.KeyCode == Keys.Down ||
            e.KeyCode == Keys.Left ||
            e.KeyCode == Keys.Home ||
            e.KeyCode == Keys.End);

        bool ExecuteKeys = (e.KeyCode == Keys.Enter);

        if (ExecuteKeys)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    try
                    {
                        string text = ChapterComboBox.Text;
                        if (!String.IsNullOrEmpty(text))
                        {
                            // 1, 3-4, 5:55, 3-4:19, 6:19-23, 24:35-27:62
                            SelectionScope scope = SelectionScope.Verse;
                            List<int> indexes = new List<int>();

                            foreach (string part in text.Split(','))
                            {
                                string[] range_parts = part.Split('-');
                                if (range_parts.Length == 1) // 1 | 5:55
                                {
                                    string[] sub_range_parts = part.Split(':');
                                    if (sub_range_parts.Length == 1) // 1
                                    {
                                        int chapter_number;
                                        if (int.TryParse(sub_range_parts[0], out chapter_number))
                                        {
                                            Chapter chapter = null;
                                            if (m_client.Book.Chapters != null)
                                            {
                                                foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                {
                                                    if (book_chapter.SortedNumber == chapter_number)
                                                    {
                                                        chapter = book_chapter;
                                                        break;
                                                    }
                                                }

                                                if (chapter != null)
                                                {
                                                    foreach (Verse verse in chapter.Verses)
                                                    {
                                                        if (!indexes.Contains(verse.Number - 1))
                                                        {
                                                            indexes.Add(verse.Number - 1);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (sub_range_parts.Length == 2) // 5:55
                                    {
                                        int chapter_number;
                                        if (int.TryParse(sub_range_parts[0], out chapter_number)) // 5:55
                                        {
                                            int verse_number_in_chapter;
                                            if (int.TryParse(sub_range_parts[1], out verse_number_in_chapter))
                                            {
                                                Chapter chapter = null;
                                                if (m_client.Book.Chapters != null)
                                                {
                                                    foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                    {
                                                        if (book_chapter.SortedNumber == chapter_number)
                                                        {
                                                            chapter = book_chapter;
                                                            break;
                                                        }
                                                    }

                                                    if (chapter != null)
                                                    {
                                                        if (((verse_number_in_chapter - 1 >= 0) && ((verse_number_in_chapter - 1) < chapter.Verses.Count)))
                                                        {
                                                            int from_verse_index = chapter.Verses[verse_number_in_chapter - 1].Number - 1;
                                                            if (!indexes.Contains(from_verse_index))
                                                            {
                                                                indexes.Add(from_verse_index);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (range_parts.Length == 2) // 3-4, 3-4:19, 6:19-23, 24:35-27:62
                                {
                                    int from_chapter_number;
                                    int to_chapter_number;
                                    if (int.TryParse(range_parts[0], out from_chapter_number))
                                    {
                                        if (int.TryParse(range_parts[1], out to_chapter_number)) // 3-4
                                        {
                                            if (from_chapter_number <= to_chapter_number)
                                            {
                                                for (int number = from_chapter_number; number <= to_chapter_number; number++)
                                                {
                                                    Chapter chapter = null;
                                                    if (m_client.Book.Chapters != null)
                                                    {
                                                        foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                        {
                                                            if (book_chapter.SortedNumber == number)
                                                            {
                                                                chapter = book_chapter;
                                                                break;
                                                            }
                                                        }

                                                        if (chapter != null)
                                                        {
                                                            foreach (Verse verse in chapter.Verses)
                                                            {
                                                                if (!indexes.Contains(verse.Number - 1))
                                                                {
                                                                    indexes.Add(verse.Number - 1);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else // backward 4-3
                                            {
                                                for (int number = from_chapter_number; number >= to_chapter_number; number--)
                                                {
                                                    Chapter chapter = null;
                                                    if (m_client.Book.Chapters != null)
                                                    {
                                                        foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                        {
                                                            if (book_chapter.SortedNumber == number)
                                                            {
                                                                chapter = book_chapter;
                                                                break;
                                                            }
                                                        }

                                                        if (chapter != null)
                                                        {
                                                            foreach (Verse verse in chapter.Verses)
                                                            {
                                                                if (!indexes.Contains(verse.Number - 1))
                                                                {
                                                                    indexes.Add(verse.Number - 1);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else // 3-4:19
                                        {
                                            // range_parts[0] == 3
                                            // range_parts[1] == 4:19
                                            string[] to_range_parts = range_parts[1].Split(':'); // 4:19
                                            if (to_range_parts.Length == 2)
                                            {
                                                if (int.TryParse(to_range_parts[0], out to_chapter_number))  // 4
                                                {
                                                    int from_verse_number_in_chapter;
                                                    int to_verse_number_in_chapter;
                                                    if (int.TryParse(to_range_parts[1], out to_verse_number_in_chapter)) // 19
                                                    {
                                                        Chapter from_chapter = null;
                                                        if (m_client.Book.Chapters != null)
                                                        {
                                                            foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                            {
                                                                if (book_chapter.SortedNumber == from_chapter_number)
                                                                {
                                                                    from_chapter = book_chapter;
                                                                    break;
                                                                }
                                                            }

                                                            if (from_chapter != null)
                                                            {
                                                                if (from_chapter_number <= to_chapter_number)
                                                                {
                                                                    from_verse_number_in_chapter = 1; // start from first verse in chapter
                                                                }
                                                                else
                                                                {
                                                                    from_verse_number_in_chapter = from_chapter.Verses.Count; // start from last verse in chapter
                                                                }

                                                                if (((from_verse_number_in_chapter - 1 >= 0) && ((from_verse_number_in_chapter - 1) < from_chapter.Verses.Count)))
                                                                {
                                                                    int from_verse_index = from_chapter.Verses[from_verse_number_in_chapter - 1].Number - 1;

                                                                    Chapter to_chapter = null;
                                                                    foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                    {
                                                                        if (book_chapter.SortedNumber == to_chapter_number)
                                                                        {
                                                                            to_chapter = book_chapter;
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (to_chapter != null)
                                                                    {
                                                                        if (((to_verse_number_in_chapter - 1 >= 0) && ((to_verse_number_in_chapter - 1) < to_chapter.Verses.Count)))
                                                                        {
                                                                            int to_verse_index = to_chapter.Verses[to_verse_number_in_chapter - 1].Number - 1;

                                                                            if (from_verse_index <= to_verse_index)  // XX:19-23
                                                                            {
                                                                                for (int i = from_verse_index; i <= to_verse_index; i++)
                                                                                {
                                                                                    if (!indexes.Contains(i))
                                                                                    {
                                                                                        indexes.Add(i);
                                                                                    }
                                                                                }
                                                                            }
                                                                            else // backward XX:32-19
                                                                            {
                                                                                for (int i = from_verse_index; i >= to_verse_index; i--)
                                                                                {
                                                                                    if (!indexes.Contains(i))
                                                                                    {
                                                                                        indexes.Add(i);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else // "range_parts[0]" contains a colon ':'  // "6:19"-23, "24:35"-27:62
                                    {
                                        //int from_chapter_number;
                                        //int to_chapter_number;
                                        string[] from_parts = range_parts[0].Split(':');
                                        if (from_parts.Length == 2)
                                        {
                                            int from_verse_number_in_chapter;
                                            if (int.TryParse(from_parts[0], out from_chapter_number))
                                            {
                                                if (int.TryParse(from_parts[1], out from_verse_number_in_chapter))
                                                {
                                                    string[] to_parts = range_parts[1].Split(':'); // "range_parts[1]" may or may not contain a colon ':'  // 6:19-"23", 24:35-"27:62"
                                                    if (to_parts.Length == 1) // 6:19-"23"
                                                    {
                                                        int to_verse_number_in_chapter;
                                                        if (int.TryParse(to_parts[0], out to_verse_number_in_chapter))
                                                        {
                                                            if (from_verse_number_in_chapter <= to_verse_number_in_chapter)  // XX:19-23
                                                            {
                                                                Chapter from_chapter = null;
                                                                if (m_client.Book.Chapters != null)
                                                                {
                                                                    foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                    {
                                                                        if (book_chapter.SortedNumber == from_chapter_number)
                                                                        {
                                                                            from_chapter = book_chapter;
                                                                            break;
                                                                        }
                                                                    }

                                                                    if (from_chapter != null)
                                                                    {
                                                                        if (((from_verse_number_in_chapter - 1 >= 0) && ((from_verse_number_in_chapter - 1) < from_chapter.Verses.Count)))
                                                                        {
                                                                            if (((to_verse_number_in_chapter - 1 >= 0) && ((to_verse_number_in_chapter - 1) < from_chapter.Verses.Count)))
                                                                            {
                                                                                int from_verse_index = from_chapter.Verses[from_verse_number_in_chapter - 1].Number - 1;
                                                                                int to_verse_index = from_chapter.Verses[to_verse_number_in_chapter - 1].Number - 1;
                                                                                for (int i = from_verse_index; i <= to_verse_index; i++)
                                                                                {
                                                                                    if (!indexes.Contains(i))
                                                                                    {
                                                                                        indexes.Add(i);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else // backward 1:7-5
                                                            {
                                                                Chapter from_chapter = null;
                                                                if (m_client.Book.Chapters != null)
                                                                {
                                                                    foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                    {
                                                                        if (book_chapter.SortedNumber == from_chapter_number)
                                                                        {
                                                                            from_chapter = book_chapter;
                                                                            break;
                                                                        }
                                                                    }

                                                                    if (from_chapter != null)
                                                                    {
                                                                        if (((from_verse_number_in_chapter - 1 >= 0) && ((from_verse_number_in_chapter - 1) < from_chapter.Verses.Count)))
                                                                        {
                                                                            if (((to_verse_number_in_chapter - 1 >= 0) && ((to_verse_number_in_chapter - 1) < from_chapter.Verses.Count)))
                                                                            {
                                                                                int from_verse_index = from_chapter.Verses[from_verse_number_in_chapter - 1].Number - 1;
                                                                                int to_verse_index = from_chapter.Verses[to_verse_number_in_chapter - 1].Number - 1;
                                                                                for (int i = from_verse_index; i >= to_verse_index; i--)
                                                                                {
                                                                                    if (!indexes.Contains(i))
                                                                                    {
                                                                                        indexes.Add(i);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (to_parts.Length == 2) // 24:35-"27:62"
                                                    {
                                                        int to_verse_number_in_chapter;
                                                        if (int.TryParse(to_parts[0], out to_chapter_number))
                                                        {
                                                            if (int.TryParse(to_parts[1], out to_verse_number_in_chapter))
                                                            {
                                                                if (from_chapter_number <= to_chapter_number)  // 24:XX-27:XX // only worry about chapters
                                                                {
                                                                    Chapter from_chapter = null;
                                                                    if (m_client.Book.Chapters != null)
                                                                    {
                                                                        foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                        {
                                                                            if (book_chapter.SortedNumber == from_chapter_number)
                                                                            {
                                                                                from_chapter = book_chapter;
                                                                                break;
                                                                            }
                                                                        }

                                                                        if (from_chapter != null)
                                                                        {
                                                                            if (((from_verse_number_in_chapter - 1 >= 0) && ((from_verse_number_in_chapter - 1) < from_chapter.Verses.Count)))
                                                                            {
                                                                                int from_verse_index = from_chapter.Verses[from_verse_number_in_chapter - 1].Number - 1;
                                                                                Chapter to_chapter = null;
                                                                                foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                                {
                                                                                    if (book_chapter.SortedNumber == to_chapter_number)
                                                                                    {
                                                                                        to_chapter = book_chapter;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                if (to_chapter != null)
                                                                                {
                                                                                    if (((to_verse_number_in_chapter - 1 >= 0) && ((to_verse_number_in_chapter - 1) < to_chapter.Verses.Count)))
                                                                                    {
                                                                                        int to_verse_index = to_chapter.Verses[to_verse_number_in_chapter - 1].Number - 1;
                                                                                        for (int i = from_verse_index; i <= to_verse_index; i++)
                                                                                        {
                                                                                            if (!indexes.Contains(i))
                                                                                            {
                                                                                                indexes.Add(i);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else // backward   // 27:XX-24:XX // only worry about chapters
                                                                {
                                                                    Chapter from_chapter = null;
                                                                    if (m_client.Book.Chapters != null)
                                                                    {
                                                                        foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                        {
                                                                            if (book_chapter.SortedNumber == from_chapter_number)
                                                                            {
                                                                                from_chapter = book_chapter;
                                                                                break;
                                                                            }
                                                                        }

                                                                        if (from_chapter != null)
                                                                        {
                                                                            if (((from_verse_number_in_chapter - 1 >= 0) && ((from_verse_number_in_chapter - 1) < from_chapter.Verses.Count)))
                                                                            {
                                                                                int from_verse_index = from_chapter.Verses[from_verse_number_in_chapter - 1].Number - 1;
                                                                                Chapter to_chapter = null;
                                                                                foreach (Chapter book_chapter in m_client.Book.Chapters)
                                                                                {
                                                                                    if (book_chapter.SortedNumber == to_chapter_number)
                                                                                    {
                                                                                        to_chapter = book_chapter;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                if (to_chapter != null)
                                                                                {
                                                                                    if (((to_verse_number_in_chapter - 1 >= 0) && ((to_verse_number_in_chapter - 1) < to_chapter.Verses.Count)))
                                                                                    {
                                                                                        int to_verse_index = to_chapter.Verses[to_verse_number_in_chapter - 1].Number - 1;
                                                                                        for (int i = from_verse_index; i >= to_verse_index; i--)
                                                                                        {
                                                                                            if (!indexes.Contains(i))
                                                                                            {
                                                                                                indexes.Add(i);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (indexes.Count > 0)
                            {
                                m_client.Selection = new Selection(m_client.Book, scope, indexes);
                                DisplaySelection(true);
                            }
                            else
                            {
                                e.Handled = false;
                            }
                        }
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }

        // reject all other keys
        if (!(SeparatorKeys || NumericKeys || EditKeys || NavigationKeys))
        {
            e.SuppressKeyPress = true;
            e.Handled = true;
        }
    }
    private void ChapterComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                List<Chapter> chapters = m_client.Book.Chapters;
                int index = ChapterComboBox.SelectedIndex;
                if ((index >= 0) && (index < chapters.Count))
                {
                    int chapter_index = chapters[index].Number - 1;

                    if (
                         ChapterComboBox.Focused ||
                         ChapterVerseNumericUpDown.Focused ||
                         ChapterWordNumericUpDown.Focused ||
                         ChapterLetterNumericUpDown.Focused ||
                         PageNumericUpDown.Focused ||
                         StationNumericUpDown.Focused ||
                         PartNumericUpDown.Focused ||
                         GroupNumericUpDown.Focused ||
                         HalfNumericUpDown.Focused ||
                         QuarterNumericUpDown.Focused
                     )
                    {
                        UpdateSelection();
                    }
                    else
                    {
                    }

                    DisplaySelection(false);
                }
            }
        }
    }

    private int m_previous_index = -1;
    private void ChaptersListBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                int index = ChaptersListBox.IndexFromPoint(e.Location);
                if (index != m_previous_index)
                {
                    m_previous_index = index;
                    if ((index >= 0) && (index < m_client.Book.Chapters.Count))
                    {
                        Chapter chapter = m_client.Book.Chapters[index];
                        if (chapter != null)
                        {
                            int match_count = 0;
                            if (m_matches_per_chapter != null)
                            {
                                if ((index >= 0) && (index < m_matches_per_chapter.Length))
                                {
                                    match_count = m_matches_per_chapter[index];
                                }
                            }

                            string revelation_place = "";
                            switch (chapter.RevelationPlace)
                            {
                                case RevelationPlace.Makkah:
                                    revelation_place = L[l]["Makkah"];
                                    break;
                                case RevelationPlace.Medina:
                                    revelation_place = L[l]["Medina"];
                                    break;
                                default:
                                    revelation_place = "";
                                    break;
                            }

                            if (chapter.Verses != null)
                            {
                                if (chapter.Verses.Count > 2)
                                {
                                    ToolTip.SetToolTip(ChaptersListBox,
                                        chapter.SortedNumber.ToString() + " - " + chapter.TransliteratedName + " - " + chapter.EnglishName + "\r\n" +
                                        L[l]["Number"] + "\t\t" + chapter.Number.ToString() + "\r\n" +
                                        L[l]["Revelation"] + "\t" + ((revelation_place.IsArabic()) ? "\t" : "") + chapter.RevelationOrder.ToString() + " - " + revelation_place + "\r\n" +
                                        L[l]["Verses"] + "\t\t" + chapter.Verses.Count.ToString() + "\r\n" +
                                        L[l]["Words"] + "\t\t" + chapter.WordCount.ToString() + "\r\n" +
                                        L[l]["Letters"] + "\t\t" + chapter.LetterCount.ToString() + "\r\n" +
                                        L[l]["Unique letters"] + "\t" + chapter.UniqueLetters.Count.ToString() + "\r\n" +
                                        (m_found_verses_displayed ? (L[l]["Matches"] + "\t\t" + match_count.ToString() + "\r\n") : "") + "\r\n" +
                                        chapter.Verses[0].Text + ((index == 41) ? ("\r\n" + chapter.Verses[1].Text) : "")
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void ChaptersListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender == ChaptersListBox)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    if (m_found_verses_displayed)
                    {
                        // set chapter filter
                        List<Chapter> chapters = new List<Chapter>();
                        foreach (int index in ChaptersListBox.SelectedIndices)
                        {
                            chapters.Add(m_client.Book.Chapters[index]);
                        }
                        m_client.FilterChapters = chapters;

                        int pos = m_find_result_header.IndexOf(" of ");
                        if (pos > -1)
                        {
                            m_find_result_header = m_find_result_header.Substring(pos + 4);
                        }
                        int selected_chapters_match_count = 0;
                        foreach (int index in ChaptersListBox.SelectedIndices)
                        {
                            if (m_matches_per_chapter != null)
                            {
                                if ((index >= 0) && (index < m_matches_per_chapter.Length))
                                {
                                    selected_chapters_match_count += m_matches_per_chapter[index];
                                }
                            }
                        }
                        m_find_result_header = selected_chapters_match_count + " of " + m_find_result_header;

                        ClearFindMatches(); // clear m_find_matches for F3 to work correctly in filtered result
                        DisplayFoundVerses(false, false);

                        SearchResultTextBox.Focus();
                        SearchResultTextBox.Refresh();
                    }
                    else
                    {
                        UpdateSelection();
                        DisplaySelection(true);
                    }

                    ChaptersListBox.Focus();
                }
            }
        }
    }

    private void InspectChaptersLabel_Click(object sender, EventArgs e)
    {
        if (m_client == null) return;
        if (m_client.Book == null) return;
        if (m_client.Selection == null) return;

        if (WordsListBox.Visible)
        {
            InspectWordFrequencies();
        }
        else
        {
            List<Chapter> chapters = null;
            if (m_found_verses_displayed)
            {
                chapters = m_client.Book.GetChapters(m_client.FoundVerses);
            }
            else
            {
                chapters = m_client.Selection.Chapters;
            }
            string result = null;
            if (chapters != null)
            {
                result = DisplayChapterInformation(chapters);
            }

            StringBuilder str = new StringBuilder();
            foreach (Chapter chapter in chapters)
            {
                str.Append("." + chapter.SortedNumber);
            }
            if (str.Length > 100)
            {
                str.Remove(100, str.Length - 100);
                int pos = str.ToString().LastIndexOf('.');
                if (pos > -1)
                {
                    str.Remove(pos, str.Length - pos);
                }

                if (str[str.Length - 1] == '.')
                {
                    str.Append("..");
                }
                else
                {
                    str.Append("...");
                }
            }

            string filename = m_client.NumerologySystem.Name + "_" + "Chapters" + str.ToString() + Globals.OUTPUT_FILE_EXT;
            if (Directory.Exists(Globals.STATISTICS_FOLDER))
            {
                string path = Globals.STATISTICS_FOLDER + "/" + filename;
                FileHelper.SaveText(path, result);
                FileHelper.DisplayFile(path);
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Selection
    ///////////////////////////////////////////////////////////////////////////////
    private bool m_selection_mode = false;
    private int m_word_number_in_verse = -1;
    private int m_letter_number_in_verse = -1;
    private int m_word_number_in_chapter = -1;
    private int m_letter_number_in_chapter = -1;
    private void NumericUpDown_Enter(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);
        this.AcceptButton = null;

        if (sender == HeaderLabel)
        {
            m_active_textbox.Focus();
            CalculateCurrentValue();
        }

        // Ctrl+Click factorizes number
        if (ModifierKeys == Keys.Control)
        {
            long value = 0L;
            if (sender == ChapterComboBox)
            {
                if (ChapterComboBox.SelectedIndex != -1)
                {
                    string[] parts = ChapterComboBox.Text.Split('-');
                    if (parts.Length > 0)
                    {
                        value = long.Parse(parts[0]);
                    }
                }
            }
            else if (sender is NumericUpDown)
            {
                try
                {
                    value = (int)(sender as NumericUpDown).Value;
                }
                catch
                {
                    value = -1L; // error
                }
            }
            else
            {
                value = -1L; // error
            }

            FactorizeValue(value, false);
        }
    }
    private void NumericUpDown_Leave(object sender, EventArgs e)
    {
        this.AcceptButton = null;
    }
    private void NumericUpDown_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            if (sender is NumericUpDown)
            {
                Control control = (sender as NumericUpDown);
                if (control != null)
                {
                    try
                    {
                        for (int i = 0; i < 3; i++) ChapterVerseNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        for (int i = 0; i < 3; i++) ChapterWordNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        for (int i = 0; i < 3; i++) ChapterLetterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        for (int i = 0; i < 3; i++) PageNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        for (int i = 0; i < 3; i++) StationNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        for (int i = 0; i < 3; i++) PartNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        for (int i = 0; i < 3; i++) GroupNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        for (int i = 0; i < 3; i++) HalfNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                        for (int i = 0; i < 3; i++) QuarterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);

                        SelectionScope scope = SelectionScope.Book;
                        if (control == ChapterVerseNumericUpDown)
                        {
                            scope = SelectionScope.Verse;
                        }
                        else if (control == ChapterWordNumericUpDown)
                        {
                            scope = SelectionScope.Word;
                        }
                        else if (control == ChapterLetterNumericUpDown)
                        {
                            scope = SelectionScope.Letter;
                        }
                        else if (control == PageNumericUpDown)
                        {
                            scope = SelectionScope.Page;
                        }
                        else if (control == StationNumericUpDown)
                        {
                            scope = SelectionScope.Station;
                        }
                        else if (control == PartNumericUpDown)
                        {
                            scope = SelectionScope.Part;
                        }
                        else if (control == GroupNumericUpDown)
                        {
                            scope = SelectionScope.Group;
                        }
                        else if (control == HalfNumericUpDown)
                        {
                            scope = SelectionScope.Half;
                        }
                        else if (control == QuarterNumericUpDown)
                        {
                            scope = SelectionScope.Quarter;
                        }
                        else
                        {
                            scope = SelectionScope.Book;
                        }

                        if (m_client != null)
                        {
                            if (m_client.Book != null)
                            {
                                if (m_client.Selection != null)
                                {
                                    // varaibles for word/letter highlight
                                    int highlight_word_number = 0;
                                    int highlight_letter_number = 0;

                                    // XXXs before chapter for ChapterXXXNumericUpDown
                                    Chapter chapter = null;
                                    int verses_before_chapter = 0;
                                    int words_before_chapter = 0;
                                    int letters_before_chapter = 0;
                                    if (m_client.Selection.Chapters.Count > 0)
                                    {
                                        chapter = m_client.Selection.Chapters[0];
                                        if (chapter != null)
                                        {
                                            if (chapter.Verses.Count > 0)
                                            {
                                                verses_before_chapter += chapter.Verses[0].Number - 1;
                                                if (chapter.Verses[0].Words.Count > 0)
                                                {
                                                    words_before_chapter += chapter.Verses[0].Words[0].Number - 1;
                                                    if (chapter.Verses[0].Words[0].Letters.Count > 0)
                                                    {
                                                        letters_before_chapter += chapter.Verses[0].Words[0].Letters[0].Number - 1;
                                                    }
                                                }
                                            }

                                            // split by , then by -
                                            List<int> indexes = new List<int>();
                                            string text = (sender as NumericUpDown).Text;
                                            string[] parts = text.Split(',');
                                            foreach (string part in parts)
                                            {
                                                string[] sub_parts = part.Split('-');
                                                if (sub_parts.Length == 1)
                                                {
                                                    int number;
                                                    if (int.TryParse(sub_parts[0], out number))
                                                    {
                                                        if (scope == SelectionScope.Verse)
                                                        {
                                                            if (control == ChapterVerseNumericUpDown)
                                                            {
                                                                if (chapter.Verses != null)
                                                                {
                                                                    if (number > chapter.Verses.Count)
                                                                    {
                                                                        number = chapter.Verses.Count;
                                                                    }
                                                                    number += verses_before_chapter;
                                                                }
                                                            }
                                                        }
                                                        else if (scope == SelectionScope.Word)
                                                        {
                                                            if (control == ChapterWordNumericUpDown)
                                                            {
                                                                if (number > chapter.WordCount)
                                                                {
                                                                    number = chapter.WordCount;
                                                                }
                                                                number += words_before_chapter;
                                                            }

                                                            // number = number of verse containing the word
                                                            if (m_client.Book != null)
                                                            {
                                                                Word word = m_client.Book.GetWord(number - 1);
                                                                if (word != null)
                                                                {
                                                                    if (highlight_word_number == 0)
                                                                    {
                                                                        highlight_word_number = word.Number;
                                                                    }

                                                                    if (word.Verse != null)
                                                                    {
                                                                        number = word.Verse.Number;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (scope == SelectionScope.Letter)
                                                        {
                                                            if (control == ChapterLetterNumericUpDown)
                                                            {
                                                                if (number > chapter.LetterCount)
                                                                {
                                                                    number = chapter.LetterCount;
                                                                }
                                                                number += letters_before_chapter;
                                                            }

                                                            // number = number of verse containing the letter
                                                            Letter letter = m_client.Book.GetLetter(number - 1);
                                                            if (letter != null)
                                                            {
                                                                if (highlight_letter_number == 0)
                                                                {
                                                                    highlight_letter_number = letter.Number;
                                                                }

                                                                if (letter.Word != null)
                                                                {
                                                                    if (letter.Word.Verse != null)
                                                                    {
                                                                        number = letter.Word.Verse.Number;
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        // add number to indexes
                                                        if (!indexes.Contains(number - 1))
                                                        {
                                                            indexes.Add(number - 1);
                                                        }
                                                    }
                                                }
                                                else if (sub_parts.Length == 2)
                                                {
                                                    int number;
                                                    int min, max;
                                                    if (int.TryParse(sub_parts[0], out min))
                                                    {
                                                        if (int.TryParse(sub_parts[1], out max))
                                                        {
                                                            int temp = -1;
                                                            if (min > max) // reverse range, e.g. min-max: 100-90
                                                            {
                                                                temp = max;
                                                                max = min;
                                                                min = temp;
                                                            }
                                                            for (int i = min; i <= max; i++)
                                                            {
                                                                if (temp == -1)
                                                                {
                                                                    number = i;
                                                                }
                                                                else // reversed min-max: 90-100
                                                                {
                                                                    // from 100 to 90 i--
                                                                    number = max - (i - min);
                                                                }

                                                                if (scope == SelectionScope.Verse)
                                                                {
                                                                    if (control == ChapterVerseNumericUpDown)
                                                                    {
                                                                        if (chapter.Verses != null)
                                                                        {
                                                                            if (number > chapter.Verses.Count)
                                                                            {
                                                                                number = chapter.Verses.Count;
                                                                            }
                                                                            number += verses_before_chapter;
                                                                        }
                                                                    }
                                                                }
                                                                else if (scope == SelectionScope.Word)
                                                                {
                                                                    if (control == ChapterWordNumericUpDown)
                                                                    {
                                                                        if (number > chapter.WordCount)
                                                                        {
                                                                            number = chapter.WordCount;
                                                                        }
                                                                        number += words_before_chapter;
                                                                    }

                                                                    // number = number of verse containing the word
                                                                    if (m_client.Book != null)
                                                                    {
                                                                        Word word = m_client.Book.GetWord(number - 1);
                                                                        if (word != null)
                                                                        {
                                                                            if (highlight_word_number == 0)
                                                                            {
                                                                                highlight_word_number = word.Number;
                                                                            }

                                                                            if (word.Verse != null)
                                                                            {
                                                                                number = word.Verse.Number;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else if (scope == SelectionScope.Letter)
                                                                {
                                                                    if (control == ChapterLetterNumericUpDown)
                                                                    {
                                                                        if (number > chapter.LetterCount)
                                                                        {
                                                                            number = chapter.LetterCount;
                                                                        }
                                                                        number += letters_before_chapter;
                                                                    }

                                                                    // number = number of verse containing the letter
                                                                    Letter letter = m_client.Book.GetLetter(number - 1);
                                                                    if (letter != null)
                                                                    {
                                                                        if (highlight_letter_number == 0)
                                                                        {
                                                                            highlight_letter_number = letter.Number;
                                                                        }

                                                                        if (letter.Word != null)
                                                                        {
                                                                            if (letter.Word.Verse != null)
                                                                            {
                                                                                number = letter.Word.Verse.Number;
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                if (!indexes.Contains(number - 1))
                                                                {
                                                                    indexes.Add(number - 1);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // do nothing
                                                }
                                            }

                                            // always display selection
                                            if (indexes.Count > 0)
                                            {
                                                if ((scope == SelectionScope.Word) || (scope == SelectionScope.Letter))
                                                {
                                                    scope = SelectionScope.Verse;
                                                }
                                                m_client.Selection = new Selection(m_client.Book, scope, indexes);

                                                DisplaySelection(true);

                                                // highlight first word/letter only
                                                if (control == ChapterWordNumericUpDown)
                                                {
                                                    HighlightWord(highlight_word_number);
                                                }
                                                else if (control == ChapterLetterNumericUpDown)
                                                {
                                                    HighlightLetter(highlight_letter_number);
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
                        ChapterVerseNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                        ChapterWordNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                        ChapterLetterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                        PageNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                        StationNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                        PartNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                        GroupNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                        HalfNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                        QuarterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    }
                }
            }
        }
    }
    private void NumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        Control control = sender as NumericUpDown;
        if (control != null)
        {
            if (control.Focused)
            {
                DisplayNumericSelection(control);
            }
        }
    }
    private void DisplayNumericSelection(Control control)
    {
        if (control is NumericUpDown)
        {
            if (control.Focused)
            {
                try
                {
                    for (int i = 0; i < 3; i++) ChapterVerseNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) ChapterWordNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) ChapterLetterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) PageNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) StationNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) PartNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) GroupNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) HalfNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) QuarterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);

                    int number = (int)((control as NumericUpDown).Value);

                    // backup number before as it will be overwritten with verse.Number
                    // if control is WordNumericUpDown OR LetterNumericUpDown or
                    // if control is ChapterWordNumericUpDown OR ChapterLetterNumericUpDown 
                    int word_number = 0;
                    int letter_number = 0;
                    if (control == ChapterLetterNumericUpDown)
                    {
                        word_number = number;
                    }
                    else if (control == ChapterLetterNumericUpDown)
                    {
                        letter_number = number;
                    }

                    if (m_client != null)
                    {
                        if (m_client.Book != null)
                        {
                            if (m_client.Book.Verses != null)
                            {
                                SelectionScope scope = SelectionScope.Book;

                                if (control == ChapterVerseNumericUpDown)
                                {
                                    scope = SelectionScope.Verse;

                                    if (m_client.Book.Chapters != null)
                                    {
                                        int verse_number_in_chapter = (int)ChapterVerseNumericUpDown.Value;

                                        int selected_index = ChapterComboBox.SelectedIndex;
                                        if ((selected_index >= 0) && (selected_index < m_client.Book.Chapters.Count))
                                        {
                                            Chapter chapter = m_client.Book.Chapters[selected_index];
                                            if (chapter != null)
                                            {
                                                if (chapter.Verses != null)
                                                {
                                                    if (chapter.Verses != null)
                                                    {
                                                        if (chapter.Verses.Count > verse_number_in_chapter - 1)
                                                        {
                                                            Verse verse = chapter.Verses[verse_number_in_chapter - 1];
                                                            if (verse != null)
                                                            {
                                                                number = verse.Number;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if ((control == ChapterWordNumericUpDown) || (control == ChapterLetterNumericUpDown))
                                {
                                    scope = SelectionScope.Verse;

                                    if (m_client.Book.Chapters != null)
                                    {
                                        int selected_index = ChapterComboBox.SelectedIndex;
                                        if ((selected_index >= 0) && (selected_index < m_client.Book.Chapters.Count))
                                        {
                                            Chapter chapter = m_client.Book.Chapters[selected_index];
                                            if (chapter != null)
                                            {
                                                if (chapter.Verses != null)
                                                {
                                                    Verse verse = null;
                                                    if (control == ChapterWordNumericUpDown)
                                                    {
                                                        word_number = number + chapter.Verses[0].Words[0].Number - 1;
                                                        verse = m_client.Book.GetVerseByWordNumber(word_number);
                                                    }
                                                    else if (control == ChapterLetterNumericUpDown)
                                                    {
                                                        letter_number = number + chapter.Verses[0].Words[0].Letters[0].Number - 1;
                                                        verse = m_client.Book.GetVerseByLetterNumber(letter_number);
                                                    }

                                                    if (verse != null)
                                                    {
                                                        number = verse.Number;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (control == PageNumericUpDown)
                                {
                                    if (m_client.Book.Pages != null)
                                    {
                                        scope = SelectionScope.Page;
                                    }
                                }
                                else if (control == StationNumericUpDown)
                                {
                                    if (m_client.Book.Stations != null)
                                    {
                                        scope = SelectionScope.Station;
                                    }
                                }
                                else if (control == PartNumericUpDown)
                                {
                                    if (m_client.Book.Parts != null)
                                    {
                                        scope = SelectionScope.Part;
                                    }
                                }
                                else if (control == GroupNumericUpDown)
                                {
                                    if (m_client.Book.Groups != null)
                                    {
                                        scope = SelectionScope.Group;
                                    }
                                }
                                else if (control == HalfNumericUpDown)
                                {
                                    if (m_client.Book.Halfs != null)
                                    {
                                        scope = SelectionScope.Half;
                                    }
                                }
                                else if (control == QuarterNumericUpDown)
                                {
                                    if (m_client.Book.Quarters != null)
                                    {
                                        scope = SelectionScope.Quarter;
                                    }
                                }
                                else
                                {
                                    // do nothing
                                }

                                if (m_client.Selection != null)
                                {
                                    // if selection has changed
                                    if (
                                        (m_client.Selection.Scope != scope)
                                        ||
                                        ((m_client.Selection.Indexes.Count > 0) && (m_client.Selection.Indexes[0] != (number - 1)))
                                       )
                                    {
                                        List<int> indexes = new List<int>() { number - 1 };
                                        m_client.Selection = new Selection(m_client.Book, scope, indexes);

                                        DisplaySelection(true);
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    ChapterVerseNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    ChapterWordNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    ChapterLetterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    PageNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    StationNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    PartNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    GroupNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    HalfNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    QuarterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                }
            }
        }
    }
    private void DisplaySelection(bool add_to_history)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            SwitchToMainTextBox();

            for (int i = 0; i < 3; i++) MainTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
            for (int i = 0; i < 3; i++) MainTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.BeginUpdate();

            m_selection_mode = true;

            ToolTip.SetToolTip(InspectChaptersLabel, L[l]["Inspect chapters"]);
            WordsListBoxLabel.Visible = false;
            WordsListBox.Visible = false;
            WordsListBox.SendToBack();

            UpdateSearchScope();

            DisplaySelectionText();

            CalculateCurrentValue();

            DisplaySelectionPositions();

            BuildLetterFrequencies();
            DisplayLetterFrequencies();

            MainTextBox.ClearHighlight();
            MainTextBox.AlignToStart();

            m_current_selection_verse_index = 0;
            CurrentVerseIndex = 0;
            UpdateHeaderLabel();

            if (m_client != null)
            {
                if (m_client.Selection != null)
                {
                    if (m_client.Selection.Verses.Count > 0)
                    {
                        Verse verse = m_client.Selection.Verses[0];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            MainTextBox.EndUpdate();
            MainTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            MainTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void DisplaySelectionText()
    {
        if (m_client != null)
        {
            if (m_client.Selection != null)
            {
                List<Verse> verses = m_client.Selection.Verses;
                if (verses != null)
                {
                    if (verses.Count > 0)
                    {
                        StringBuilder str = new StringBuilder();
                        foreach (Verse verse in verses)
                        {
                            if (verse != null)
                            {
                                str.Append(verse.Text + verse.Endmark);
                            }
                        }
                        if (str.Length > 1)
                        {
                            str.Remove(str.Length - 1, 1); // last space in " {###} "   OR  \n
                        }
                        m_current_text = str.ToString();
                    }
                }

                MainTextBox.Text = m_current_text;
                MainTextBox.Refresh();
            }
        }
    }
    private string GetSelectionSummary()
    {
        string result = null;
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if (m_client.Selection != null)
                {
                    if (m_client.Book.Verses != null)
                    {
                        if (m_client.Selection.Indexes != null)
                        {
                            StringBuilder str = new StringBuilder();

                            List<int> selection_indexes = new List<int>();
                            if (m_client.Selection.Scope == SelectionScope.Chapter)
                            {
                                foreach (Chapter chapter in m_client.Selection.Chapters)
                                {
                                    selection_indexes.Add(chapter.SortedNumber - 1);
                                }
                            }
                            else
                            {
                                selection_indexes = m_client.Selection.Indexes;
                            }

                            if (Numbers.AreConsecutive(selection_indexes))
                            {
                                if (m_client.Selection.Indexes.Count > 1)
                                {
                                    int first_index = m_client.Selection.Indexes[0];
                                    int last_index = m_client.Selection.Indexes[m_client.Selection.Indexes.Count - 1];

                                    if (m_client.Selection.Scope == SelectionScope.Verse)
                                    {
                                        str.Append(m_client.Book.Verses[first_index].Address + " - ");
                                        str.Append(m_client.Book.Verses[last_index].Address);
                                    }
                                    else if (m_client.Selection.Scope == SelectionScope.Chapter)
                                    {
                                        int from_chapter_sorted_number = -1;
                                        int to_chapter_sorted_number = -1;
                                        int from_chapter_number = first_index + 1;
                                        int to_chapter_number = last_index + 1;
                                        if (m_client.Book.Chapters != null)
                                        {
                                            foreach (Chapter chapter in m_client.Book.Chapters)
                                            {
                                                if (chapter.Number == from_chapter_number)
                                                {
                                                    from_chapter_sorted_number = chapter.SortedNumber;
                                                    break;
                                                }
                                            }
                                            foreach (Chapter chapter in m_client.Book.Chapters)
                                            {
                                                if (chapter.Number == to_chapter_number)
                                                {
                                                    to_chapter_sorted_number = chapter.SortedNumber;
                                                    break;
                                                }
                                            }
                                            str.Append(from_chapter_sorted_number.ToString() + " - ");
                                            str.Append(to_chapter_sorted_number.ToString());
                                        }
                                    }
                                    else
                                    {
                                        str.Append((first_index + 1).ToString() + "-");
                                        str.Append((last_index + 1).ToString());
                                    }
                                }
                                else if (m_client.Selection.Indexes.Count == 1)
                                {
                                    int index = m_client.Selection.Indexes[0];
                                    if (m_client.Selection.Scope == SelectionScope.Verse)
                                    {
                                        str.Append(m_client.Book.Verses[index].Address);
                                    }
                                    else if (m_client.Selection.Scope == SelectionScope.Chapter)
                                    {
                                        int chapter_sorted_number = 0;
                                        int chapter_number = index + 1;
                                        if (m_client.Book.Chapters != null)
                                        {
                                            foreach (Chapter chapter in m_client.Book.Chapters)
                                            {
                                                if (chapter.Number == chapter_number)
                                                {
                                                    chapter_sorted_number = chapter.SortedNumber;
                                                    break;
                                                }
                                            }
                                            str.Append(chapter_sorted_number.ToString());
                                        }
                                    }
                                    else
                                    {
                                        str.Append((index + 1).ToString());
                                    }
                                }
                                else
                                {
                                    // do nothing
                                }
                            }
                            else
                            {
                                if (m_client.Selection.Indexes.Count > 0)
                                {
                                    foreach (int index in m_client.Selection.Indexes)
                                    {
                                        if (m_client.Selection.Scope == SelectionScope.Verse)
                                        {
                                            str.Append(m_client.Book.Verses[index].Address + " ");
                                        }
                                        else if (m_client.Selection.Scope == SelectionScope.Chapter)
                                        {
                                            int chapter_sorted_number = 0;
                                            int chapter_number = index + 1;
                                            if (m_client.Book.Chapters != null)
                                            {
                                                foreach (Chapter chapter in m_client.Book.Chapters)
                                                {
                                                    if (chapter.Number == chapter_number)
                                                    {
                                                        chapter_sorted_number = chapter.SortedNumber;
                                                        break;
                                                    }
                                                }
                                                str.Append(chapter_sorted_number.ToString() + " ");
                                            }
                                        }
                                        else
                                        {
                                            str.Append((index + 1).ToString() + " ");
                                        }
                                    }
                                    if (str.Length > 1)
                                    {
                                        str.Remove(str.Length - 1, 1);
                                    }
                                }

                                if (m_client.Selection.Scope == SelectionScope.Verse)
                                {
                                }
                                else if (m_client.Selection.Scope == SelectionScope.Chapter)
                                {
                                }
                                else
                                {
                                }
                            }

                            if (m_client.Selection.Indexes.Count == 1)
                            {
                                result = m_client.Selection.Scope.ToString() + " " + str.ToString();
                            }
                            else if (m_client.Selection.Indexes.Count > 1)
                            {
                                result = m_client.Selection.Scope.ToString() + "s" + " " + str.ToString();
                            }
                        }
                    }
                }
            }

            if (result != null)
            {
                // trim if too long
                if (result.Length > SELECTON_SCOPE_TEXT_MAX_LENGTH)
                {
                    result = result.Substring(0, SELECTON_SCOPE_TEXT_MAX_LENGTH) + " ...";
                }
            }
        }
        return result;
    }
    private void UpdateLanguageType(string text)
    {
        if (text.IsArabic())
        {
            SetLanguageType(LanguageType.RightToLeft);
        }
        else
        {
            SetLanguageType(LanguageType.LeftToRight);
        }
        EnableFindByTextControls();
    }
    private void DisplaySelectionPositions()
    {
        if (m_client != null)
        {
            if (m_client.Selection != null)
            {
                List<Verse> verses = m_client.Selection.Verses;
                if (verses != null)
                {
                    if (verses.Count > 0)
                    {
                        Verse verse = verses[0];
                        if (verse != null)
                        {
                            // show postion of selection in the Quran visually
                            UpdateProgressBar(verse);

                            if (verse.Chapter != null)
                            {
                                UpdateMinMaxChapterVerseWordLetter(verse.Chapter.SortedNumber - 1);
                            }

                            if (ChapterComboBox.Items.Count > 0)
                            {
                                // without this guard, we cannot select more than 1 chapter in ChaptersListBox and
                                // we cannot move backward/forward inside the ChaptersListBox using Backspace
                                if (!ChaptersListBox.Focused)
                                {
                                    UpdateChaptersListBox();
                                }
                            }
                            UpdateVersePositions(verse);
                        }
                    }
                }
            }
        }
    }
    private Chapter m_old_chapter = null;
    private Verse m_old_verse = null;
    private void DisplayCurrentPositions()
    {
        if (m_active_textbox != null)
        {
            if (m_active_textbox.Lines != null)
            {
                if (m_active_textbox.Lines.Length > 0)
                {
                    Verse verse = GetCurrentVerse();
                    if (verse != null)
                    {
                        if (m_old_verse != verse)
                        {
                            m_old_verse = verse;

                            // show postion of verse in the Quran visually
                            ProgressBar.Minimum = 1;
                            ProgressBar.Maximum = verse.Book.Verses.Count;
                            ProgressBar.Value = verse.Number;
                            ProgressBar.Refresh();

                            if (verse.Chapter != null)
                            {
                                if (m_old_chapter != verse.Chapter)
                                {
                                    m_old_chapter = verse.Chapter;
                                    UpdateMinMaxChapterVerseWordLetter(verse.Chapter.SortedNumber - 1);
                                }
                            }

                            if (ChapterComboBox.Items.Count > 0)
                            {
                                // without this guard, we cannot select more than 1 chapter in ChaptersListBox and
                                // we cannot move backward/forward inside the ChaptersListBox using Backspace
                                if (!ChaptersListBox.Focused)
                                {
                                    UpdateChaptersListBox();
                                }
                            }
                        }
                        UpdateVersePositions(verse);
                    }
                }
            }
        }
    }
    private void UpdateVersePositions(Verse verse)
    {
        if (m_active_textbox != null)
        {
            if (verse != null)
            {
                try
                {
                    for (int i = 0; i < 3; i++) ChapterComboBox.SelectedIndexChanged -= new EventHandler(ChapterComboBox_SelectedIndexChanged);
                    for (int i = 0; i < 3; i++) ChapterVerseNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) ChapterWordNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) ChapterLetterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) PageNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) StationNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) PartNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) GroupNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) HalfNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
                    for (int i = 0; i < 3; i++) QuarterNumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);

                    if (verse.Chapter != null)
                    {
                        if (ChapterComboBox.SelectedIndex != verse.Chapter.SortedNumber - 1)
                        {
                            ChapterComboBox.SelectedIndex = verse.Chapter.SortedNumber - 1;
                            DisplayChapterRevelationInfo();
                        }
                    }

                    if ((verse.NumberInChapter >= 1) && (verse.NumberInChapter <= verse.Chapter.Verses.Count))
                    {
                        if (verse.Chapter != null)
                        {
                            if (ChapterVerseNumericUpDown.Value != verse.NumberInChapter)
                            {
                                ChapterVerseNumericUpDown.Value = (verse.NumberInChapter > ChapterVerseNumericUpDown.Maximum) ? ChapterVerseNumericUpDown.Maximum : verse.NumberInChapter;
                            }
                        }
                    }

                    if (verse.Page != null)
                    {
                        if (PageNumericUpDown.Value != verse.Page.Number)
                        {
                            PageNumericUpDown.Value = verse.Page.Number;
                        }
                    }
                    if (verse.Station != null)
                    {
                        if (StationNumericUpDown.Value != verse.Station.Number)
                        {
                            StationNumericUpDown.Value = verse.Station.Number;
                        }
                    }
                    if (verse.Part != null)
                    {
                        if (PartNumericUpDown.Value != verse.Part.Number)
                        {
                            PartNumericUpDown.Value = verse.Part.Number;
                        }
                    }
                    if (verse.Group != null)
                    {
                        if (GroupNumericUpDown.Value != verse.Group.Number)
                        {
                            GroupNumericUpDown.Value = verse.Group.Number;
                        }
                    }
                    if (verse.Half != null)
                    {
                        if (HalfNumericUpDown.Value != verse.Half.Number)
                        {
                            HalfNumericUpDown.Value = verse.Half.Number;
                        }
                    }
                    if (verse.Quarter != null)
                    {
                        if (QuarterNumericUpDown.Value != verse.Quarter.Number)
                        {
                            QuarterNumericUpDown.Value = verse.Quarter.Number;
                        }
                    }

                    int char_index = m_active_textbox.SelectionStart;
                    int line_index = m_active_textbox.GetLineFromCharIndex(char_index);

                    Word word = GetWordAtChar(char_index);
                    if (word != null)
                    {
                        int word_index_in_verse = word.NumberInVerse - 1;
                        Letter letter = GetLetterAtChar(char_index);
                        if (letter == null) letter = GetLetterAtChar(char_index - 1); // (Ctrl_End)
                        if (letter != null)
                        {
                            int letter_index_in_verse = letter.NumberInVerse - 1;
                            int word_number = verse.Words[0].Number + word_index_in_verse;
                            m_word_number_in_verse = word_index_in_verse + 1;
                            m_letter_number_in_verse = letter_index_in_verse + 1;
                            int word_count = 0;
                            int letter_count = 0;
                            if (verse.Chapter != null)
                            {
                                foreach (Verse chapter_verse in verse.Chapter.Verses)
                                {
                                    if (chapter_verse.NumberInChapter < verse.NumberInChapter)
                                    {
                                        word_count += chapter_verse.Words.Count;
                                        letter_count += chapter_verse.LetterCount;
                                    }
                                }
                            }
                            m_word_number_in_chapter = word_count + m_word_number_in_verse;
                            m_letter_number_in_chapter = letter_count + m_letter_number_in_verse;

                            if (m_word_number_in_chapter > ChapterWordNumericUpDown.Maximum)
                            {
                                ChapterWordNumericUpDown.Value = ChapterWordNumericUpDown.Maximum;
                            }
                            else if (m_word_number_in_chapter < ChapterWordNumericUpDown.Minimum)
                            {
                                ChapterWordNumericUpDown.Value = ChapterWordNumericUpDown.Minimum;
                            }
                            else
                            {
                                if (ChapterWordNumericUpDown.Value != m_word_number_in_chapter)
                                {
                                    ChapterWordNumericUpDown.Value = m_word_number_in_chapter;
                                }
                            }

                            if (m_letter_number_in_chapter > ChapterLetterNumericUpDown.Maximum)
                            {
                                ChapterLetterNumericUpDown.Value = ChapterLetterNumericUpDown.Maximum;
                            }
                            else if (m_letter_number_in_chapter < ChapterLetterNumericUpDown.Minimum)
                            {
                                ChapterLetterNumericUpDown.Value = ChapterLetterNumericUpDown.Minimum;
                            }
                            else
                            {
                                if (ChapterLetterNumericUpDown.Value != m_letter_number_in_chapter)
                                {
                                    ChapterLetterNumericUpDown.Value = m_letter_number_in_chapter;
                                }
                            }

                            ColorizePositionNumbers();
                            ColorizePositionControls();
                        }
                    }
                }
                catch
                {
                    // ignore poosible error due to non-Arabic search result
                    // showing verses with more words than the words in the Arabic verse
                    // and throwing exception when assigned to WordNumericUpDown.Value or LetterNumericUpDown.Value
                }
                finally
                {
                    ChapterComboBox.SelectedIndexChanged += new EventHandler(ChapterComboBox_SelectedIndexChanged);
                    ChapterVerseNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    ChapterWordNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    ChapterLetterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    PageNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    StationNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    PartNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    GroupNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    HalfNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                    QuarterNumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
                }
            }
        }
    }
    private void ColorizePositionNumbers()
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                if ((ChapterComboBox.SelectedIndex >= 0) && (ChapterComboBox.SelectedIndex < m_client.Book.Chapters.Count))
                {
                    int chapter_number = m_client.Book.Chapters[ChapterComboBox.SelectedIndex].SortedNumber;
                    //ChapterComboBox.ForeColor = ChaptersListBox.GetItemColor(chapter_number - 1);
                    ChapterComboBox.ForeColor = Numbers.GetNumberTypeColor(chapter_number);
                    ChapterComboBox.BackColor = (Numbers.Compare(chapter_number, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.Window;
                }
            }
        }

        ChapterVerseNumericUpDown.ForeColor = Numbers.GetNumberTypeColor((int)ChapterVerseNumericUpDown.Value);
        ChapterWordNumericUpDown.ForeColor = Numbers.GetNumberTypeColor((int)ChapterWordNumericUpDown.Value);
        ChapterLetterNumericUpDown.ForeColor = Numbers.GetNumberTypeColor((int)ChapterLetterNumericUpDown.Value);
        PageNumericUpDown.ForeColor = Numbers.GetNumberTypeColor((int)PageNumericUpDown.Value);
        StationNumericUpDown.ForeColor = Numbers.GetNumberTypeColor((int)StationNumericUpDown.Value);
        PartNumericUpDown.ForeColor = Numbers.GetNumberTypeColor((int)PartNumericUpDown.Value);
        GroupNumericUpDown.ForeColor = Numbers.GetNumberTypeColor((int)GroupNumericUpDown.Value);
        HalfNumericUpDown.ForeColor = Numbers.GetNumberTypeColor((int)HalfNumericUpDown.Value);
        QuarterNumericUpDown.ForeColor = Numbers.GetNumberTypeColor((int)QuarterNumericUpDown.Value);

        ChapterVerseNumericUpDown.BackColor = (Numbers.Compare((int)ChapterVerseNumericUpDown.Value, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.Window;
        ChapterWordNumericUpDown.BackColor = (Numbers.Compare((int)ChapterWordNumericUpDown.Value, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.Window;
        ChapterLetterNumericUpDown.BackColor = (Numbers.Compare((int)ChapterLetterNumericUpDown.Value, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.Window;
        PageNumericUpDown.BackColor = (Numbers.Compare((int)PageNumericUpDown.Value, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.Window;
        StationNumericUpDown.BackColor = (Numbers.Compare((int)StationNumericUpDown.Value, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.Window;
        PartNumericUpDown.BackColor = (Numbers.Compare((int)PartNumericUpDown.Value, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.Window;
        GroupNumericUpDown.BackColor = (Numbers.Compare((int)GroupNumericUpDown.Value, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.Window;
        HalfNumericUpDown.BackColor = (Numbers.Compare((int)HalfNumericUpDown.Value, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.Window;
        QuarterNumericUpDown.BackColor = (Numbers.Compare((int)QuarterNumericUpDown.Value, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.Window;

        ChapterComboBox.Refresh();
        ChapterVerseNumericUpDown.Refresh();
        ChapterWordNumericUpDown.Refresh();
        ChapterLetterNumericUpDown.Refresh();
        PageNumericUpDown.Refresh();
        StationNumericUpDown.Refresh();
        PartNumericUpDown.Refresh();
        GroupNumericUpDown.Refresh();
        HalfNumericUpDown.Refresh();
        QuarterNumericUpDown.Refresh();
    }
    private void ColorizePositionControls()
    {
        if (m_client != null)
        {
            // reset colors
            ChapterPositionLabel.ForeColor = Color.CornflowerBlue;
            ChapterVerseWordLetterPositionLabel.ForeColor = Color.CornflowerBlue;
            PagePositionLabel.ForeColor = Color.LightSkyBlue;
            StationPositionLabel.ForeColor = Color.LightSteelBlue;
            PartPositionLabel.ForeColor = Color.LightSteelBlue;
            GroupPositionLabel.ForeColor = Color.LightSteelBlue;
            HalfPositionLabel.ForeColor = Color.LightSteelBlue;
            QuarterPositionLabel.ForeColor = Color.LightSteelBlue;

            // set selected color
            if (m_client.Selection != null)
            {
                switch (m_client.Selection.Scope)
                {
                    case SelectionScope.Book:
                        {
                        }
                        break;
                    case SelectionScope.Station:
                        {
                            StationPositionLabel.ForeColor = Color.LightCoral;
                        }
                        break;
                    case SelectionScope.Part:
                        {
                            PartPositionLabel.ForeColor = Color.LightCoral;
                        }
                        break;
                    case SelectionScope.Group:
                        {
                            GroupPositionLabel.ForeColor = Color.LightCoral;
                        }
                        break;
                    case SelectionScope.Half:
                        {
                            HalfPositionLabel.ForeColor = Color.LightCoral;
                        }
                        break;
                    case SelectionScope.Quarter:
                        {
                            QuarterPositionLabel.ForeColor = Color.LightCoral;
                        }
                        break;
                    case SelectionScope.Chapter:
                        {
                            ChapterPositionLabel.ForeColor = Color.LightCoral;
                        }
                        break;
                    case SelectionScope.Verse:
                    case SelectionScope.Word:
                    case SelectionScope.Letter:
                        {
                            ChapterVerseWordLetterPositionLabel.ForeColor = Color.LightCoral;
                        }
                        break;
                    case SelectionScope.Page:
                        {
                            PagePositionLabel.ForeColor = Color.LightCoral;
                        }
                        break;
                    default: // Unknown
                        break;
                }
            }
        }
    }
    private void HighlightWord(int word_number)
    {
        if (m_active_textbox != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    Verse verse = m_client.Book.GetVerseByWordNumber(word_number);
                    if (verse != null)
                    {
                        word_number -= verse.Words[0].Number;
                        if ((word_number >= 0) && (word_number < verse.Words.Count))
                        {
                            Word word = verse.Words[word_number];
                            m_active_textbox.Select(word.Position, word.Text.Length);
                        }
                    }
                }
            }
        }
    }
    private void HighlightLetter(int letter_number)
    {
        if (m_active_textbox != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    Word word = m_client.Book.GetWordByLetterNumber(letter_number);
                    if (word != null)
                    {
                        letter_number -= word.Letters[0].Number;
                        if ((letter_number >= 0) && (letter_number < word.Letters.Count))
                        {
                            int letter_position = word.Position + letter_number;
                            int letter_length = 1;
                            m_active_textbox.Select(letter_position, letter_length);
                        }
                    }
                }
            }
        }
    }

    private int m_progressbar_X = 0;
    private Verse m_progressbar_verse = null;
    private void ProgressBar_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.X == m_progressbar_X) return;
        m_progressbar_X = e.X;

        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                int delta = (int)((double)ProgressBar.Maximum / (double)ProgressBar.Width);
                int index = (int)(((double)m_progressbar_X / (double)ProgressBar.Width) * (ProgressBar.Maximum + delta));
                if ((index >= 0) && (index < m_client.Book.Verses.Count))
                {
                    m_progressbar_verse = m_client.Book.Verses[index];
                    string info_text = "Page " + m_progressbar_verse.Page.Number + "     " + m_progressbar_verse.Address + "\t" + m_progressbar_verse.Text;
                    ToolTip.SetToolTip(ProgressBar, info_text);
                }
            }
        }
    }
    private void ProgressBar_Click(object sender, EventArgs e)
    {
        if (m_progressbar_verse != null)
        {
            GotoVerse(m_progressbar_verse);
            UpdateProgressBar(m_progressbar_verse);
        }
    }
    private void UpdateProgressBar(Verse verse)
    {
        if (m_client != null)
        {
            if (m_client.Selection != null)
            {
                switch (m_client.Selection.Scope)
                {
                    case SelectionScope.Book:
                        {
                        }
                        break;
                    case SelectionScope.Station:
                        {
                        }
                        break;
                    case SelectionScope.Part:
                        {
                        }
                        break;
                    case SelectionScope.Group:
                        {
                        }
                        break;
                    case SelectionScope.Half:
                        {
                        }
                        break;
                    case SelectionScope.Quarter:
                        {
                        }
                        break;
                    case SelectionScope.Bowing:
                        {
                        }
                        break;
                    case SelectionScope.Chapter:
                        {
                        }
                        break;
                    case SelectionScope.Verse:
                    case SelectionScope.Word:
                    case SelectionScope.Letter:
                        {
                        }
                        break;
                    case SelectionScope.Page:
                        {
                        }
                        break;
                }

                // show postion of verse in the Quran visually
                ProgressBar.Minimum = 1;
                ProgressBar.Maximum = verse.Book.Verses.Count;
                ProgressBar.Value = verse.Number;
                ProgressBar.Refresh();
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Grammar/RelatedWords
    ///////////////////////////////////////////////////////////////////////////////
    private Word m_current_word = null;
    private Word m_info_word = null;
    private string GetWordInformation(Word word)
    {
        if (word != null)
        {
            StringBuilder roots = new StringBuilder();
            if (word.Roots != null)
            {
                if (word.Roots.Count > 0)
                {
                    foreach (string root in word.Roots)
                    {
                        roots.Append(root + " | ");
                    }
                    roots.Remove(roots.Length - 3, 3);
                }
            }

            return
                word.Transliteration + SPACE_GAP +
                word.Text + SPACE_GAP +
                word.Meaning + SPACE_GAP +
                roots + "\r\n" +
                L[l]["chapter"] + "  " + word.Verse.Chapter.SortedNumber /*+ " " + word.Verse.Chapter.Name*/ + SPACE_GAP +
                L[l]["verse"] + "  " + word.Verse.NumberInChapter /*+ "-" + word.Verse.Number*/ + SPACE_GAP +
                L[l]["word"] + "  " + word.NumberInVerse /*+ "-" + word.NumberInChapter + "-" + word.Number*/ + SPACE_GAP + SPACE_GAP + SPACE_GAP +
                word.Occurrence.ToString() + "/" + word.Frequency.ToString();
        }
        return null;
    }
    private string GetRelatedWordsInformation(Word word)
    {
        if (word != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    string result = null;
                    int words_per_line = 0;
                    int max_words_per_line = 10;
                    List<Word> related_words = m_client.Book.GetRelatedWords(word);
                    int word_count = related_words.Count;
                    related_words = related_words.RemoveDuplicates();
                    int unique_word_count = related_words.Count;
                    if (related_words != null)
                    {
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(word_count.ToString() + " (" + unique_word_count.ToString() + ")" + "  " + L[l]["related words"]);

                        if (related_words.Count > 0)
                        {
                            foreach (Word related_word in related_words)
                            {
                                words_per_line++;
                                str.Append(related_word.Text + (((words_per_line % max_words_per_line) == 0) ? "\r\n" : "\t"));
                            }
                            if (str.Length > 1)
                            {
                                str.Remove(str.Length - 1, 1); // \t
                            }
                            str.AppendLine();

                            result = str.ToString();
                        }
                    }
                    return result;
                }
            }
        }
        return null;
    }
    private string GetRelatedVersesInformation(Word word)
    {
        if (word != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    string result = null;
                    List<Verse> related_verses = m_client.Book.GetRelatedWordVerses(word);
                    if (related_verses != null)
                    {
                        StringBuilder str = new StringBuilder();
                        str.AppendLine(related_verses.Count.ToString() + "  " + L[l]["related verses"]);

                        foreach (Verse related_verse in related_verses)
                        {
                            str.AppendLine(related_verse.Text);
                        }
                        str.AppendLine();

                        result = str.ToString();
                    }
                    return result;
                }
            }
        }
        return null;
    }
    private void FindRelatedWords(Word word)
    {
        if (word != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    string text = word.Text;

                    FindByTextTextBox.Text = text;
                    FindByTextTextBox.Refresh();
                    text = word.BestRoot;
                    FindByRoot(text);
                }
            }
        }
    }
    private void UpdateMouseCursor()
    {
        if (m_active_textbox != null)
        {
            if (ModifierKeys == Keys.Control)
            {
                // stop cursor flicker
                if (m_active_textbox.Cursor != Cursors.Hand)
                {
                    m_active_textbox.Cursor = Cursors.Hand;
                }
            }
            else
            {
                // stop cursor flicker
                if (m_active_textbox.Cursor != Cursors.IBeam)
                {
                    m_active_textbox.Cursor = Cursors.IBeam;
                }
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Autocomplete/WordFrequency
    ///////////////////////////////////////////////////////////////////////////////
    private bool m_auto_complete_mode = false;
    private bool m_word_double_click = false;
    private bool m_sort_by_word_frequency = true;
    private Dictionary<string, int> m_word_frequency_dictionary = null;
    private void WordsListBox_Enter(object sender, EventArgs e)
    {
        this.AcceptButton = FindByTextButton;
    }
    private void WordsListBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            if (WordsListBox.SelectedIndices.Count > 1)
            {
                FindSelectedWordsMenuItem_Click(null, null);
            }
            else
            {
                WordsListBox_DoubleClick(sender, e);
            }
        }
        else if (e.KeyCode == Keys.Space)
        {
            FindByTextTextBox.Text += " ";
            FindByTextTextBox.Focus();
        }
        else if ((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right))
        {
            FindByTextTextBox.Focus();
        }
        FindByTextTextBox.SelectionStart = FindByTextTextBox.Text.Length;
    }
    private void WordsListBox_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            // set cursor at mouse RIGHT-click location so we know which word to Find
            if (WordsListBox.SelectedIndices.Count == 1)
            {
                WordsListBox.SelectedIndex = -1;
            }
            WordsListBox.SelectedIndex = WordsListBox.IndexFromPoint(e.X, e.Y);
        }
    }
    private void WordsListBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (m_client != null)
            {
                int index = WordsListBox.IndexFromPoint(e.Location);
                if (index != m_previous_index)
                {
                    m_previous_index = index;
                    if ((index >= 0) && (index < WordsListBox.Items.Count))
                    {
                        char[] separators = { ' ' };
                        string[] parts = WordsListBox.Items[index].ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 1)
                        {
                            DisplayWordVerses(parts[1]);
                        }
                    }
                }
            }
        }
        else
        {
            ToolTip.SetToolTip(WordsListBox, null);
        }
    }
    private void WordsListBox_Click(object sender, EventArgs e)
    {
        // do nothing
    }
    private void WordsListBox_DoubleClick(object sender, EventArgs e)
    {
        m_word_double_click = true;
        if (WordsListBox.Items.Count > 0)
        {
            AddNextWordToFindText();
        }
        else
        {
            FindByTextButton_Click(null, null);
        }
        m_word_double_click = false;
    }
    private void WordsListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (m_word_frequency_dictionary != null)
        {
            int count = 0;
            int total = 0;
            if (WordsListBox.SelectedIndices.Count > 1)
            {
                // update total(unique) counts
                foreach (object item in WordsListBox.SelectedItems)
                {
                    string[] parts = item.ToString().Split();
                    if (parts.Length > 0)
                    {
                        total += int.Parse(parts[0]);
                        count++;
                    }
                }
            }
            else
            {
                // restore total(unique) counts
                foreach (string key in m_word_frequency_dictionary.Keys)
                {
                    total += m_word_frequency_dictionary[key];
                    count++;
                }
            }

            WordsListBoxLabel.Text = total.ToString() + " (" + count.ToString() + ")";
            WordsListBoxLabel.ForeColor = Numbers.GetNumberTypeColor(total);
            WordsListBoxLabel.Refresh();
        }
    }
    private void WordsListBoxLabel_Click(object sender, EventArgs e)
    {
        // Ctrl+Click factorizes number
        if (ModifierKeys == Keys.Control)
        {
            NumericUpDown_Enter(sender, e);
        }
        else
        {
            m_sort_by_word_frequency = !m_sort_by_word_frequency;
            ToolTip.SetToolTip(WordsListBoxLabel, (m_sort_by_word_frequency ? "sort alphabetically" : "sort by frequency"));

            if (m_auto_complete_mode)
            {
                PopulateWordsListBox();
            }
            else
            {
                PopulateWordsListBoxWithHighlightedWords();
            }
        }
    }
    private void AddNextWordToFindText()
    {
        if (WordsListBox.SelectedItem != null)
        {
            string word_to_add = WordsListBox.SelectedItem.ToString();
            int pos = word_to_add.LastIndexOf(' ');
            if (pos > -1)
            {
                word_to_add = word_to_add.Substring(pos + 1);
            }

            string text = FindByTextTextBox.Text;
            int index = text.LastIndexOf(' ');
            if (index != -1)
            {
                if (text.Length > index + 1)
                {
                    if ((text[index + 1] == '+') || (text[index + 1] == '-'))
                    {
                        index++;
                    }
                }

                text = text.Substring(0, index + 1);
                text += word_to_add;
                FindByTextTextBox.Text = text + " ";
            }
            else
            {
                FindByTextTextBox.Text = word_to_add + " ";
            }
            FindByTextTextBox.Refresh();
            FindByTextTextBox.SelectionStart = FindByTextTextBox.Text.Length;
        }
    }
    private void PopulateWordsListBox()
    {
        if (m_text_search_type == TextSearchType.Exact)
        {
            PopulateWordsListBoxWithCurrentOrNextWords();
        }
        else if (m_text_search_type == TextSearchType.Root)
        {
            PopulateWordsListBoxWithRoots();
        }
        else if (m_text_search_type == TextSearchType.Proximity)
        {
            PopulateWordsListBoxWithCurrentWords();
        }
    }
    private void PopulateWordsListBoxWithCurrentOrNextWords()
    {
        try
        {
            for (int i = 0; i < 3; i++) WordsListBox.SelectedIndexChanged -= new EventHandler(WordsListBox_SelectedIndexChanged);

            if (m_client != null)
            {
                //SearchGroupBox.Text = " Search by Exact words      ";
                //SearchGroupBox.Refresh();
                WordsListBoxLabel.Text = "000 (00)";
                WordsListBoxLabel.ForeColor = Numbers.GetNumberTypeColor(0);
                //ToolTip.SetToolTip(WordsListBoxLabel, "total (unique)");
                WordsListBoxLabel.Refresh();

                WordsListBox.BeginUpdate();
                WordsListBox.Items.Clear();

                m_auto_complete_mode = true;

                string text = FindByTextTextBox.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    if (text.EndsWith(" "))
                    {
                        m_word_frequency_dictionary = m_client.GetNextWords(text, m_text_location_in_chapter, m_text_location_in_verse, m_text_location_in_word, m_text_wordness, m_case_sensitive, m_with_diacritics);
                    }
                    else
                    {
                        m_word_frequency_dictionary = m_client.GetCurrentWords(text, m_text_location_in_chapter, m_text_location_in_verse, m_text_location_in_word, m_text_wordness, m_case_sensitive, m_with_diacritics);
                    }

                    if (m_word_frequency_dictionary != null)
                    {
                        // sort dictionary by value or key
                        List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(m_word_frequency_dictionary);
                        if (m_sort_by_word_frequency)
                        {
                            list.Sort(
                                delegate(KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair)
                                {
                                    return nextPair.Value.CompareTo(firstPair.Value);
                                }
                            );
                        }
                        else
                        {
                            list.Sort(
                                delegate(KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair)
                                {
                                    return firstPair.Key.CompareTo(nextPair.Key);
                                }
                            );
                        }

                        int count = 0;
                        int total = 0;
                        foreach (KeyValuePair<string, int> pair in list)
                        {
                            //string value_str = found_words[key].ToString().PadRight(3, ' ');
                            //string key_str = key.PadLeft(10, ' ');
                            //string entry = String.Format("{0} {1}", value_str, key_str);
                            string entry = String.Format("{0,-3} {1,10}", pair.Value, pair.Key);
                            WordsListBox.Items.Add(entry);
                            total += pair.Value;
                            count++;
                        }

                        if (WordsListBox.Items.Count > 0)
                        {
                            WordsListBox.SelectedIndex = 0;
                        }
                        else // no match [either current text_mode doesn't have a match or it was last word in verse]
                        {
                            // m_word_frequency_list_double_click == false if input was via keyboard
                            // m_word_frequency_list_double_click == true  if input was via double click
                            // if no more word when double click, then it means it was the last word in the verse
                            // else the user has entered non-matching text

                            // if last word in verse, remove the extra space after it
                            if ((m_word_double_click) && (WordsListBox.Items.Count == 0) && (FindByTextTextBox.Text.EndsWith(" ")))
                            {
                                for (int i = 0; i < 3; i++) FindByTextTextBox.TextChanged -= new EventHandler(FindByTextTextBox_TextChanged);
                                try
                                {
                                    FindByTextTextBox.Text = FindByTextTextBox.Text.Remove(FindByTextTextBox.Text.Length - 1);
                                }
                                finally
                                {
                                    FindByTextTextBox.TextChanged += new EventHandler(FindByTextTextBox_TextChanged);
                                }
                            }
                        }

                        WordsListBoxLabel.Text = total.ToString() + " (" + count.ToString() + ")";
                        WordsListBoxLabel.ForeColor = Numbers.GetNumberTypeColor(total);
                        WordsListBoxLabel.Refresh();
                    }
                }
            }
        }
        finally
        {
            WordsListBox.EndUpdate();
            WordsListBox.SelectedIndexChanged += new EventHandler(WordsListBox_SelectedIndexChanged);
        }
    }
    private void PopulateWordsListBoxWithCurrentWords()
    {
        try
        {
            for (int i = 0; i < 3; i++) WordsListBox.SelectedIndexChanged -= new EventHandler(WordsListBox_SelectedIndexChanged);

            if (m_client != null)
            {
                //SearchGroupBox.Text = " Search by Proximity        ";
                //SearchGroupBox.Refresh();
                WordsListBoxLabel.Text = "000 (00)";
                WordsListBoxLabel.ForeColor = Numbers.GetNumberTypeColor(0);
                //ToolTip.SetToolTip(WordsListBoxLabel, "total (unique)");
                WordsListBoxLabel.Refresh();

                WordsListBox.BeginUpdate();
                WordsListBox.Items.Clear();

                m_auto_complete_mode = true;

                string text = FindByTextTextBox.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    string[] text_parts = text.Split();
                    text = text_parts[text_parts.Length - 1];
                    if (!String.IsNullOrEmpty(text))
                    {
                        m_word_frequency_dictionary = m_client.GetCurrentWords(text, m_text_location_in_chapter, m_text_location_in_verse, m_text_location_in_word, m_text_wordness, m_case_sensitive, m_with_diacritics);
                        if (m_word_frequency_dictionary != null)
                        {
                            // sort dictionary by value or key
                            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(m_word_frequency_dictionary);
                            if (m_sort_by_word_frequency)
                            {
                                list.Sort(
                                    delegate(KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair)
                                    {
                                        return nextPair.Value.CompareTo(firstPair.Value);
                                    }
                                );
                            }
                            else
                            {
                                list.Sort(
                                    delegate(KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair)
                                    {
                                        return firstPair.Key.CompareTo(nextPair.Key);
                                    }
                                );
                            }

                            int count = 0;
                            int total = 0;
                            foreach (KeyValuePair<string, int> pair in list)
                            {
                                //string value_str = found_words[key].ToString().PadRight(3, ' ');
                                //string key_str = key.PadLeft(10, ' ');
                                //string entry = String.Format("{0} {1}", value_str, key_str);
                                string entry = String.Format("{0,-3} {1,10}", pair.Value, pair.Key);
                                WordsListBox.Items.Add(entry);
                                total += pair.Value;
                                count++;
                            }

                            if (WordsListBox.Items.Count > 0)
                            {
                                WordsListBox.SelectedIndex = 0;
                            }
                            else
                            {
                                // if not a valid word, keep word as is
                            }

                            WordsListBoxLabel.Text = total.ToString() + " (" + count.ToString() + ")";
                            WordsListBoxLabel.ForeColor = Numbers.GetNumberTypeColor(total);
                            WordsListBoxLabel.Refresh();
                        }
                    }
                }
            }
        }
        finally
        {
            WordsListBox.EndUpdate();
            WordsListBox.SelectedIndexChanged += new EventHandler(WordsListBox_SelectedIndexChanged);
        }
    }
    private void PopulateWordsListBoxWithRoots()
    {
        try
        {
            for (int i = 0; i < 3; i++) WordsListBox.SelectedIndexChanged -= new EventHandler(WordsListBox_SelectedIndexChanged);

            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    //SearchGroupBox.Text = " Search by Roots            ";
                    //SearchGroupBox.Refresh();
                    WordsListBoxLabel.Text = "000 (00)";
                    WordsListBoxLabel.ForeColor = Numbers.GetNumberTypeColor(0);
                    //ToolTip.SetToolTip(WordsListBoxLabel, "total (unique)");
                    WordsListBoxLabel.Refresh();

                    WordsListBox.BeginUpdate();
                    WordsListBox.Items.Clear();

                    m_auto_complete_mode = true;

                    string text = FindByTextTextBox.Text;

                    // to support multi root search take the last word a user is currently writing
                    string[] text_parts = text.Split();
                    if (text_parts.Length > 0)
                    {
                        text = text_parts[text_parts.Length - 1];
                    }

                    m_word_frequency_dictionary = new Dictionary<string, int>();
                    if (m_word_frequency_dictionary != null)
                    {
                        switch (m_client.SearchScope)
                        {
                            case SearchScope.Book:
                                {
                                    m_word_frequency_dictionary = m_client.Book.GetWordRoots(m_client.Book.Verses, text, m_text_location_in_word);
                                }
                                break;
                            case SearchScope.Selection:
                                {
                                    m_word_frequency_dictionary = m_client.Book.GetWordRoots(m_client.Selection.Verses, text, m_text_location_in_word);
                                }
                                break;
                            case SearchScope.Result:
                                {
                                    m_word_frequency_dictionary = m_client.Book.GetWordRoots(m_client.FoundVerses, text, m_text_location_in_word);
                                }
                                break;
                        }

                        if (m_word_frequency_dictionary != null)
                        {
                            // sort dictionary by value or key
                            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(m_word_frequency_dictionary);
                            if (m_sort_by_word_frequency)
                            {
                                list.Sort(
                                    delegate(KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair)
                                    {
                                        return nextPair.Value.CompareTo(firstPair.Value);
                                    }
                                );
                            }
                            else
                            {
                                list.Sort(
                                    delegate(KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair)
                                    {
                                        return firstPair.Key.CompareTo(nextPair.Key);
                                    }
                                );
                            }

                            int count = 0;
                            int total = 0;
                            foreach (KeyValuePair<string, int> pair in list)
                            {
                                //string value_str = found_words[key].ToString().PadRight(3, ' ');
                                //string key_str = key.PadLeft(10, ' ');
                                //string entry = String.Format("{0} {1}", value_str, key_str);
                                string entry = String.Format("{0,-3} {1,10}", pair.Value, pair.Key);
                                WordsListBox.Items.Add(entry);
                                total += pair.Value;
                                count++;
                            }

                            if (WordsListBox.Items.Count > 0)
                            {
                                WordsListBox.SelectedIndex = 0;
                            }
                            else
                            {
                                // if not a valid root, put word as is so we can find same rooted words
                                WordsListBox.Items.Add(text);
                            }
                            WordsListBoxLabel.Text = total.ToString() + " (" + count.ToString() + ")";
                            WordsListBoxLabel.ForeColor = Numbers.GetNumberTypeColor(total);
                            WordsListBoxLabel.Refresh();
                        }
                    }
                }
            }
        }
        finally
        {
            WordsListBox.EndUpdate();
            WordsListBox.SelectedIndexChanged += new EventHandler(WordsListBox_SelectedIndexChanged);
        }
    }
    private void PopulateWordsListBoxWithHighlightedWords()
    {
        try
        {
            for (int i = 0; i < 3; i++) WordsListBox.SelectedIndexChanged -= new EventHandler(WordsListBox_SelectedIndexChanged);

            if (m_client != null)
            {
                WordsListBox.BeginUpdate();
                WordsListBox.Items.Clear();

                m_auto_complete_mode = false;

                //CalculateCurrentText();
                string text = m_current_text;
                if (!String.IsNullOrEmpty(text))
                {
                    text = text.Replace("\n", " ");
                    text = text.Replace("\r", "");
                    text = text.Replace("\t", "");
                    text = text.Replace("_", "");
                    text = text.Replace(Constants.OPEN_BRACKET, "");
                    text = text.Replace(Constants.CLOSE_BRACKET, "");
                    foreach (char character in Constants.INDIAN_DIGITS)
                    {
                        text = text.Replace(character.ToString(), "");
                    }
                    foreach (char character in Constants.QURANMARKS)
                    {
                        text = text.Replace(character.ToString(), "");
                    }
                    foreach (char character in Constants.STOPMARKS)
                    {
                        text = text.Replace(character.ToString(), "");
                    }
                    while (text.Contains("  "))
                    {
                        text = text.Replace("  ", " ");
                    }
                    text = text.Trim();

                    string[] words = text.Split();
                    m_word_frequency_dictionary = new Dictionary<string, int>();
                    if (m_word_frequency_dictionary != null)
                    {
                        foreach (string word in words)
                        {
                            if (m_word_frequency_dictionary.ContainsKey(word))
                            {
                                m_word_frequency_dictionary[word]++;
                            }
                            else
                            {
                                m_word_frequency_dictionary.Add(word, 1);
                            }
                        }

                        // sort dictionary by value or key
                        List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(m_word_frequency_dictionary);
                        if (m_sort_by_word_frequency)
                        {
                            list.Sort(
                                delegate(KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair)
                                {
                                    return nextPair.Value.CompareTo(firstPair.Value);
                                }
                            );
                        }
                        else
                        {
                            list.Sort(
                                delegate(KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair)
                                {
                                    return firstPair.Key.CompareTo(nextPair.Key);
                                }
                            );
                        }

                        int count = 0;
                        int total = 0;
                        foreach (KeyValuePair<string, int> pair in list)
                        {
                            string entry = String.Format("{0,-3} {1,10}", pair.Value, pair.Key);
                            WordsListBox.Items.Add(entry);
                            total += pair.Value;
                            count++;
                        }

                        if (WordsListBox.Items.Count > 0)
                        {
                            WordsListBox.SelectedIndex = 0;
                        }
                        else
                        {
                            // if not a valid word, keep word as is
                        }

                        WordsListBoxLabel.Text = total.ToString() + " (" + count.ToString() + ")";
                        WordsListBoxLabel.ForeColor = Numbers.GetNumberTypeColor(total);
                        WordsListBoxLabel.Refresh();
                    }
                }
            }
        }
        finally
        {
            WordsListBox.EndUpdate();
            WordsListBox.SelectedIndexChanged += new EventHandler(WordsListBox_SelectedIndexChanged);
        }
    }
    private void DisplayWordFrequencies()
    {
        PopulateWordsListBoxWithHighlightedWords();
        EnableFindByTextControls();
        FindByTextControls_Enter(null, null);
    }
    private void DisplayWordVerses(string item_text)
    {
        if (!String.IsNullOrEmpty(item_text))
        {
            if (m_client != null)
            {
                List<Verse> backup_found_verses = null;
                List<Phrase> backup_found_phrases = null;
                if (m_client.FoundVerses != null)
                {
                    backup_found_verses = new List<Verse>(m_client.FoundVerses);
                }
                if (m_client.FoundPhrases != null)
                {
                    backup_found_phrases = new List<Phrase>(m_client.FoundPhrases);
                }

                // get startup text from FindTextBox
                string[] startup_words = FindByTextTextBox.Text.Split();
                int count = startup_words.Length;
                // ignore final incomplete word
                if (!FindByTextTextBox.Text.EndsWith(" "))
                {
                    count--;
                }
                string startup_text = "";
                for (int i = 0; i < count; i++)
                {
                    startup_text += startup_words[i] + " ";
                }
                if (startup_text.Length > 0)
                {
                    startup_text = startup_text.Remove(startup_text.Length - 1, 1);
                }

                List<string> word_texts = new List<string>();
                char[] separators = { ' ' };
                string[] parts = item_text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)  // root
                {
                    word_texts.Add(parts[0]);
                }
                else if (parts.Length == 2) // exact or proximity
                {
                    word_texts.Add(parts[1]);
                }

                // setup search parameters
                string text = "";
                //string translation = Client.DEFAULT_TRANSLATION;

                // update m_text_location_in_verse and m_text_location_in_word
                UpdateFindByTextOptions();

                List<Verse> total_verses = new List<Verse>();
                if (word_texts.Count > 0)
                {
                    foreach (string word_text in word_texts)
                    {
                        if (startup_text.Length > 0)
                        {
                            text = startup_text + " " + word_text;
                        }
                        else
                        {
                            text = word_text;
                        }

                        if (!String.IsNullOrEmpty(text))
                        {
                            switch (m_text_search_type)
                            {
                                case TextSearchType.Exact:
                                    {
                                        m_client.FindPhrases(TextSearchBlockSize.Verse, text, m_language_type, null, m_text_location_in_chapter, m_text_location_in_verse, m_text_location_in_word, TextWordness.WholeWord, m_case_sensitive, m_with_diacritics, m_multiplicity, m_multiplicity_number_type, m_multiplicity_comparison_operator, m_multiplicity_remainder);
                                    }
                                    break;
                                case TextSearchType.Root:
                                    {
                                        m_client.FindPhrases(TextSearchBlockSize.Verse, text, m_multiplicity, m_multiplicity_number_type, m_multiplicity_comparison_operator, m_multiplicity_remainder);
                                    }
                                    break;
                                case TextSearchType.Proximity:
                                    {
                                        m_client.FindPhrases(TextSearchBlockSize.Verse, text, m_language_type, null, m_text_proximity_type, TextWordness.WholeWord, m_case_sensitive, m_with_diacritics);
                                    }
                                    break;
                            }

                            total_verses = total_verses.Union(m_client.FoundVerses);
                        }
                    }

                    int i = 0;
                    StringBuilder str = new StringBuilder();
                    foreach (Verse verse in total_verses)
                    {
                        i++;
                        if (i > 114) break;
                        str.AppendLine(verse.Text);
                    }
                    ToolTip.SetToolTip(WordsListBox, str.ToString());
                }

                if (backup_found_verses != null)
                {
                    m_client.FoundVerses = backup_found_verses;
                }
                if (backup_found_phrases != null)
                {
                    m_client.FoundPhrases = backup_found_phrases;
                }
            }
        }
    }
    private void FindSelectedWordsMenuItem_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            // get startup text from FindTextBox
            string[] startup_words = FindByTextTextBox.Text.Split();
            int count = startup_words.Length;
            // ignore final incomplete word
            if (!FindByTextTextBox.Text.EndsWith(" "))
            {
                count--;
            }

            string startup_text = "";
            if (m_auto_complete_mode)
            {
                for (int i = 0; i < count; i++)
                {
                    startup_text += startup_words[i] + " ";
                }
                if (startup_text.Length > 0)
                {
                    startup_text = startup_text.Remove(startup_text.Length - 1, 1);
                }
            }

            // get selected word texts
            List<string> word_texts = new List<string>();
            if (WordsListBox.SelectedIndices.Count > 0)
            {
                char[] separators = { ' ' };
                foreach (object item in WordsListBox.SelectedItems)
                {
                    string[] parts = item.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 1)  // root
                    {
                        word_texts.Add(parts[0]);
                    }
                    else if (parts.Length == 2) // exact or proximity
                    {
                        word_texts.Add(parts[1]);
                    }
                }
            }

            // setup search parameters
            string text = "";
            //string translation = Client.DEFAULT_TRANSLATION;

            // update m_text_location_in_verse and m_text_location_in_word
            UpdateFindByTextOptions();

            List<Phrase> total_phrases = new List<Phrase>();
            List<Verse> total_verses = new List<Verse>();
            if (word_texts.Count > 0)
            {
                foreach (string word_text in word_texts)
                {
                    if (startup_text.Length > 0)
                    {
                        text = startup_text + " " + word_text;
                    }
                    else
                    {
                        text = word_text;
                    }

                    if (!String.IsNullOrEmpty(text))
                    {
                        switch (m_text_search_type)
                        {
                            case TextSearchType.Exact:
                                {
                                    if (FindByTextTextBox.Text.EndsWith(" "))
                                    {
                                        m_client.FindPhrases(TextSearchBlockSize.Verse, text, m_language_type, null, m_text_location_in_chapter, m_text_location_in_verse, m_text_location_in_word, TextWordness.Any, m_case_sensitive, m_with_diacritics, m_multiplicity, m_multiplicity_number_type, m_multiplicity_comparison_operator, m_multiplicity_remainder);
                                    }
                                    else
                                    {
                                        m_client.FindPhrases(TextSearchBlockSize.Verse, text, m_language_type, null, m_text_location_in_chapter, m_text_location_in_verse, m_text_location_in_word, TextWordness.WholeWord, m_case_sensitive, m_with_diacritics, m_multiplicity, m_multiplicity_number_type, m_multiplicity_comparison_operator, m_multiplicity_remainder);
                                    }
                                }
                                break;
                            case TextSearchType.Root:
                                {
                                    m_client.FindPhrases(TextSearchBlockSize.Verse, text, m_multiplicity, m_multiplicity_number_type, m_multiplicity_comparison_operator, m_multiplicity_remainder);
                                }
                                break;
                            case TextSearchType.Proximity:
                                {
                                    m_client.FindPhrases(TextSearchBlockSize.Verse, text, m_language_type, null, m_text_proximity_type, TextWordness.Any, m_case_sensitive, m_with_diacritics);
                                }
                                break;
                        }

                        total_phrases = total_phrases.Union(m_client.FoundPhrases);
                        total_verses = total_verses.Union(m_client.FoundVerses);
                    }
                }

                // write final result to m_client
                m_client.FoundPhrases = total_phrases;
                m_client.FoundVerses = total_verses;
            }

            // display results
            if (m_client.FoundPhrases != null)
            {
                int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                if (m_client.FoundVerses != null)
                {
                    int verse_count = m_client.FoundVerses.Count;
                    m_find_result_header = phrase_count + " " + L[l]["matches"] + " " + L[l]["in"] + " " + verse_count + ((verse_count == 1) ? " " + L[l]["verse"] : " " + L[l]["verses"]) + " " + L[l]["with"] + " " + text + " C_" + m_text_location_in_chapter.ToString() + " V_" + m_text_location_in_verse.ToString() + " W_" + m_text_location_in_word.ToString() + " " + L[l]["in"] + " " + L[l][m_client.SearchScope.ToString()];
                    DisplayFoundVerses(true, true);

                    SearchResultTextBox.Focus();
                    SearchResultTextBox.Refresh();

                    WordsListBoxLabel.Visible = false;
                    WordsListBox.Visible = false;
                }
            }
        }
    }
    private void InspectWordFrequencies()
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            string text = FindByTextTextBox.Text;

            if (Directory.Exists(Globals.STATISTICS_FOLDER))
            {
                string filename = Globals.STATISTICS_FOLDER + "/" + ((m_text_search_type == TextSearchType.Root) ? "root_" : "") + text + ".txt";
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    StringBuilder str = new StringBuilder();
                    str.AppendLine("-----------------");
                    str.AppendLine(((m_text_search_type == TextSearchType.Root) ? "Root" : "Word") + "\t" + "Frequency");
                    str.AppendLine("-----------------");

                    int count = 0;
                    int total = 0;
                    char[] separators = { ' ' };
                    if (WordsListBox.SelectedIndices.Count > 1)
                    {
                        count = WordsListBox.SelectedIndices.Count;
                        foreach (object item in WordsListBox.SelectedItems)
                        {
                            string[] parts = item.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 2)
                            {
                                str.AppendLine(parts[1] + "\t" + parts[0]);
                                total += int.Parse(parts[0]);
                            }
                        }
                    }
                    else
                    {
                        count = WordsListBox.Items.Count;
                        foreach (object item in WordsListBox.Items)
                        {
                            string[] parts = item.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 2)
                            {
                                str.AppendLine(parts[1] + "\t" + parts[0]);
                                total += int.Parse(parts[0]);
                            }
                        }
                    }
                    str.AppendLine("-----------------");
                    str.AppendLine("Count = " + count.ToString());
                    str.AppendLine("Total = " + total.ToString());

                    writer.Write(str.ToString());
                }

                // show file content after save
                if (File.Exists(filename))
                {
                    FileHelper.WaitForReady(filename);

                    System.Diagnostics.Process.Start("Notepad.exe", filename);
                }
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Search Setup
    ///////////////////////////////////////////////////////////////////////////////
    private SearchType m_search_type = SearchType.Text; // named with private to indicate must set via Property, not directly by field
    private LanguageType m_language_type = LanguageType.RightToLeft;
    private void SearchScopeBookLabel_Click(object sender, EventArgs e)
    {
        m_client.SearchScope = SearchScope.Book;
        FindByTextTextBox_TextChanged(null, null);
    }
    private void SearchScopeSelectionLabel_Click(object sender, EventArgs e)
    {
        m_client.SearchScope = SearchScope.Selection;
        FindByTextTextBox_TextChanged(null, null);
    }
    private void SearchScopeResultLabel_Click(object sender, EventArgs e)
    {
        m_client.SearchScope = SearchScope.Result;
        FindByTextTextBox_TextChanged(null, null);
    }
    private void SearchGroupBox_Enter(object sender, EventArgs e)
    {
    }
    private void SearchGroupBox_Leave(object sender, EventArgs e)
    {
        if (!WordsListBox.Focused)
        {
            ToolTip.SetToolTip(InspectChaptersLabel, L[l]["Inspect chapters"]);
            WordsListBoxLabel.Visible = false;
            WordsListBox.Visible = false;
        }
    }
    private void ClearFindMatches()
    {
        m_find_matches = new List<FindMatch>();
        m_find_match_index = -1;
    }
    private void UpdateFindByTextOptions()
    {
        if (FindByTextAtChapterAnyRadioButton.Checked)
        {
            m_text_location_in_chapter = TextLocationInChapter.Any;
        }
        else if (FindByTextAtChapterStartRadioButton.Checked)
        {
            m_text_location_in_chapter = TextLocationInChapter.AtStart;
        }
        else if (FindByTextAtChapterMiddleRadioButton.Checked)
        {
            m_text_location_in_chapter = TextLocationInChapter.AtMiddle;
        }
        else if (FindByTextAtChapterEndRadioButton.Checked)
        {
            m_text_location_in_chapter = TextLocationInChapter.AtEnd;
        }

        if (FindByTextAtVerseAnyRadioButton.Checked)
        {
            m_text_location_in_verse = TextLocationInVerse.Any;
        }
        else if (FindByTextAtVerseStartRadioButton.Checked)
        {
            m_text_location_in_verse = TextLocationInVerse.AtStart;
        }
        else if (FindByTextAtVerseMiddleRadioButton.Checked)
        {
            m_text_location_in_verse = TextLocationInVerse.AtMiddle;
        }
        else if (FindByTextAtVerseEndRadioButton.Checked)
        {
            m_text_location_in_verse = TextLocationInVerse.AtEnd;
        }

        if (FindByTextAtWordAnyRadioButton.Checked)
        {
            m_text_location_in_word = TextLocationInWord.Any;
        }
        else if (FindByTextAtWordStartRadioButton.Checked)
        {
            m_text_location_in_word = TextLocationInWord.AtStart;
        }
        else if (FindByTextAtWordMiddleRadioButton.Checked)
        {
            m_text_location_in_word = TextLocationInWord.AtMiddle;
        }
        else if (FindByTextAtWordEndRadioButton.Checked)
        {
            m_text_location_in_word = TextLocationInWord.AtEnd;
        }

        switch (FindByTextWordnessCheckBox.CheckState)
        {
            case CheckState.Checked:
                m_text_wordness = TextWordness.WholeWord;
                break;
            case CheckState.Indeterminate:
                m_text_wordness = TextWordness.PartOfWord;
                break;
            case CheckState.Unchecked:
                m_text_wordness = TextWordness.Any;
                break;
        }

        m_case_sensitive = true;
    }
    private int GetPhraseCount(List<Phrase> phrases)
    {
        int count = 0;
        foreach (Phrase phrase in phrases)
        {
            if (phrase != null)
            {
                if (!String.IsNullOrEmpty(phrase.Text))
                {
                    count++;
                }
            }
        }
        return count;
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Search By Text
    ///////////////////////////////////////////////////////////////////////////////
    private TextSearchType m_text_search_type = TextSearchType.Exact;
    private TextSearchBlockSize m_text_search_block_size = TextSearchBlockSize.Verse;
    private TextLocationInChapter m_text_location_in_chapter = TextLocationInChapter.Any;
    private TextLocationInVerse m_text_location_in_verse = TextLocationInVerse.Any;
    private TextLocationInWord m_text_location_in_word = TextLocationInWord.Any;
    private TextProximityType m_text_proximity_type = TextProximityType.AllWords;
    private TextWordness m_text_wordness = TextWordness.Any;
    private bool m_case_sensitive = false;
    private bool m_with_diacritics = false;
    private int m_multiplicity = -1;
    private NumberType m_multiplicity_number_type = NumberType.None;
    private ComparisonOperator m_multiplicity_comparison_operator = ComparisonOperator.Equal;
    private int m_multiplicity_remainder = -1;
    private void SetLanguageType(LanguageType language_type)
    {
        if (language_type == LanguageType.RightToLeft)
        {
            m_language_type = language_type;
        }
        else if (language_type == LanguageType.LeftToRight)
        {
            if (m_text_search_type == TextSearchType.Root)
            {
                m_language_type = LanguageType.RightToLeft;
            }
            else
            {
                m_language_type = language_type;
            }
        }
    }
    private void FindByTextExactSearchTypeLabel_Click(object sender, EventArgs e)
    {
        m_text_search_type = TextSearchType.Exact;
        PopulateWordsListBoxWithCurrentOrNextWords();
        FindByTextAtVerseAnyRadioButton.Checked = true;

        EnableFindByTextControls();
        UpdateKeyboard(m_client.NumerologySystem.TextMode);
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextProximitySearchTypeLabel_Click(object sender, EventArgs e)
    {
        m_text_search_type = TextSearchType.Proximity;
        PopulateWordsListBoxWithCurrentWords();
        FindByTextAllWordsRadioButton.Checked = true;

        EnableFindByTextControls();
        UpdateKeyboard(m_client.NumerologySystem.TextMode);
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextRootSearchTypeLabel_Click(object sender, EventArgs e)
    {
        m_text_search_type = TextSearchType.Root;
        PopulateWordsListBoxWithRoots();

        EnableFindByTextControls();
        UpdateKeyboard("Original");
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextSearchBlockSizeVerseLabel_Click(object sender, EventArgs e)
    {
        m_text_search_block_size = TextSearchBlockSize.Verse;
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextSearchBlockSizeChapterLabel_Click(object sender, EventArgs e)
    {
        m_text_search_block_size = TextSearchBlockSize.Chapter;
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextSearchBlockSizePageLabel_Click(object sender, EventArgs e)
    {
        m_text_search_block_size = TextSearchBlockSize.Page;
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextSearchBlockSizeStationLabel_Click(object sender, EventArgs e)
    {
        m_text_search_block_size = TextSearchBlockSize.Station;
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextSearchBlockSizePartLabel_Click(object sender, EventArgs e)
    {
        m_text_search_block_size = TextSearchBlockSize.Part;
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextSearchBlockSizeGroupLabel_Click(object sender, EventArgs e)
    {
        m_text_search_block_size = TextSearchBlockSize.Group;
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextSearchBlockSizeHalfLabel_Click(object sender, EventArgs e)
    {
        m_text_search_block_size = TextSearchBlockSize.Half;
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextSearchBlockSizeQuarterLabel_Click(object sender, EventArgs e)
    {
        m_text_search_block_size = TextSearchBlockSize.Quarter;
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextSearchBlockSizeBowingLabel_Click(object sender, EventArgs e)
    {
        m_text_search_block_size = TextSearchBlockSize.Bowing;
        FindByTextControls_Enter(null, null);
    }
    private void FindByTextRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        UpdateFindByTextOptions();
        PopulateWordsListBox();
    }
    private void FindByTextWordnessCheckBox_CheckStateChanged(object sender, EventArgs e)
    {
        EnableFindByTextControls();
        UpdateFindByTextOptions();
        PopulateWordsListBox();
    }
    private void FindByTextCaseSensitiveCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        EnableFindByTextControls();
        UpdateFindByTextOptions();
        PopulateWordsListBox();
    }
    private void FindByTextControls_Enter(object sender, EventArgs e)
    {
        this.AcceptButton = FindByTextButton;

        FindByTextButton.Enabled = true;

        if (m_text_search_type == TextSearchType.Root)
        {
            ToolTip.SetToolTip(InspectChaptersLabel, L[l]["Inspect root frequencies"]);
        }
        else
        {
            ToolTip.SetToolTip(InspectChaptersLabel, L[l]["Inspect word frequencies"]);
        }
        WordsListBoxLabel.Visible = true;
        WordsListBox.Visible = true;
        WordsListBoxLabel.BringToFront();
        WordsListBox.BringToFront();

        ResetFindByTextSearchTypeLabels();

        switch (m_text_search_type)
        {
            case TextSearchType.Exact:
                {
                    FindByTextExactSearchTypeLabel.BackColor = Color.SteelBlue;
                    FindByTextExactSearchTypeLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            case TextSearchType.Proximity:
                {
                    FindByTextProximitySearchTypeLabel.BackColor = Color.SteelBlue;
                    FindByTextProximitySearchTypeLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            case TextSearchType.Root:
                {
                    FindByTextRootSearchTypeLabel.BackColor = Color.SteelBlue;
                    FindByTextRootSearchTypeLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            default:
                break;
        }
    }
    private void FindByTextControls_Leave(object sender, EventArgs e)
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
    private void FindByTextPanel_Leave(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);
    }
    private void FindByTextTextBox_Enter(object sender, EventArgs e)
    {
        FindByTextTextBox_TextChanged(null, null);
    }
    private void FindByTextTextBox_TextChanged(object sender, EventArgs e)
    {
        EnableFindByTextControls();

        PopulateWordsListBox();

        UpdateLanguageType(FindByTextTextBox.Text);

        UpdateSearchScope();
    }
    private void UpdateSearchScope()
    {
        SearchScopeBookLabel.BackColor = Color.DarkGray;
        SearchScopeBookLabel.BorderStyle = BorderStyle.None;
        SearchScopeSelectionLabel.BackColor = Color.DarkGray;
        SearchScopeSelectionLabel.BorderStyle = BorderStyle.None;
        SearchScopeResultLabel.BackColor = Color.DarkGray;
        SearchScopeResultLabel.BorderStyle = BorderStyle.None;

        switch (m_client.SearchScope)
        {
            case SearchScope.Book:
                {
                    SearchScopeBookLabel.BackColor = Color.SteelBlue;
                    SearchScopeBookLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            case SearchScope.Selection:
                {
                    SearchScopeSelectionLabel.BackColor = Color.SteelBlue;
                    SearchScopeSelectionLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            case SearchScope.Result:
                {
                    SearchScopeResultLabel.BackColor = Color.SteelBlue;
                    SearchScopeResultLabel.BorderStyle = BorderStyle.Fixed3D;
                }
                break;
            default:
                break;
        }
    }
    private void FindByTextTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        FixMicrosoft(sender, e);

        if (e.KeyChar == ' ')
        {
            // prevent double spaces
            if (FindByTextTextBox.SelectionStart > 0)
            {
                if (FindByTextTextBox.Text[FindByTextTextBox.SelectionStart - 1] == ' ')
                {
                    e.Handled = true;
                }
            }
        }
    }
    private void FindByTextTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (e.KeyCode == Keys.A)
            {
                if (sender is TextBoxBase)
                {
                    (sender as TextBoxBase).SelectAll();
                }
            }
        }
        else if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down))
        {
            WordsListBox.Focus();
        }
    }
    private void FindByTextButton_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            switch (m_text_search_type)
            {
                case TextSearchType.Exact:
                    {
                        if (WordsListBox.SelectedIndices.Count > 1)
                        {
                            FindSelectedWordsMenuItem_Click(null, null);
                        }
                        else
                        {
                            FindByExact();
                        }
                    }
                    break;
                case TextSearchType.Proximity:
                    {
                        FindByProximity();
                    }
                    break;
                case TextSearchType.Root:
                    {
                        if (WordsListBox.SelectedIndices.Count > 1)
                        {
                            FindSelectedWordsMenuItem_Click(null, null);
                        }
                        else
                        {
                            FindByRoot();
                        }
                    }
                    break;
                default:
                    {
                        FindByExact();
                    }
                    break;
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }

        SearchGroupBox_Leave(null, null);
    }
    private void FindByExact()
    {
        if (m_client != null)
        {
            string text = FindByTextTextBox.Text;
            if (text.Length > 0)
            {
                ClearFindMatches();

                if (!String.IsNullOrEmpty(text))
                {
                    UpdateFindByTextOptions();

                    //FindByExact(text, m_language_type, translation);
                    FindByExact(text, m_language_type, null); // find in all installed translations if not Arabic
                }
            }
        }
    }
    private void FindByExact(string text, LanguageType language_type, string translation)
    {
        m_search_type = SearchType.Text;

        if (m_client != null)
        {
            if (!String.IsNullOrEmpty(text))
            {
                m_client.FindPhrases(m_text_search_block_size, text, language_type, translation, m_text_location_in_chapter, m_text_location_in_verse, m_text_location_in_word, m_text_wordness, m_case_sensitive, m_with_diacritics, m_multiplicity, m_multiplicity_number_type, m_multiplicity_comparison_operator, m_multiplicity_remainder);
                if (m_client.FoundPhrases != null)
                {
                    if (m_client.FoundVerses != null)
                    {
                        int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                        string block_name = "verse";
                        int block_count = m_client.FoundVerses.Count;
                        if (m_multiplicity == 0)
                        {
                            m_find_result_header = block_count + " " + ((block_count == 1) ? L[l][block_name] : (L[l][block_name] + "s")) + " " + L[l]["without"] + " " + text + " C_" + m_text_location_in_chapter.ToString() + " V_" + m_text_location_in_verse.ToString() + " W_" + m_text_location_in_word.ToString() + " " + L[l]["in"] + " " + L[l][m_client.SearchScope.ToString()];
                        }
                        else
                        {
                            m_find_result_header = phrase_count + " " + L[l]["matches"] + " " + L[l]["in"] + " " + block_count + " " + ((block_count == 1) ? L[l][block_name] : (L[l][block_name + "s"])) + " " + L[l]["with"] + " " + text + " C_" + m_text_location_in_chapter.ToString() + " V_" + m_text_location_in_verse.ToString() + " W_" + m_text_location_in_word.ToString() + " " + L[l]["in"] + " " + L[l][m_client.SearchScope.ToString()];
                        }
                        DisplayFoundVerses(true, true);

                        SearchResultTextBox.Focus();
                        SearchResultTextBox.Refresh();

                        WordsListBoxLabel.Visible = false;
                        WordsListBox.Visible = false;
                    }
                }
            }
        }
    }
    private void FindByProximity()
    {
        if (m_client != null)
        {
            string text = FindByTextTextBox.Text;
            if (text.Length > 0)
            {
                if (FindByTextAllWordsRadioButton.Checked)
                {
                    m_text_proximity_type = TextProximityType.AllWords;
                }
                else if (FindByTextAnyWordRadioButton.Checked)
                {
                    m_text_proximity_type = TextProximityType.AnyWord;
                }

                //FindByProximity(text, m_language_type, translation, m_text_proximity_type);
                FindByProximity(text, m_language_type, null, m_text_proximity_type);
            }
        }
    }
    private void FindByProximity(string text, LanguageType language_type, string translation, TextProximityType text_proximity_type)
    {
        m_search_type = SearchType.Text;

        if (m_client != null)
        {
            if (!String.IsNullOrEmpty(text))
            {
                ClearFindMatches();

                m_client.FindPhrases(m_text_search_block_size, text, language_type, translation, text_proximity_type, m_text_wordness, m_case_sensitive, m_with_diacritics);
                if (m_client.FoundPhrases != null)
                {
                    if (m_client.FoundVerses != null)
                    {
                        int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                        string block_name = "verse";
                        //string block_name = ((m_multiplicity_comparison_operator == ComparisonOperator.Equal) && (m_text_search_block_size != TextSearchBlockSize.Verse)) ? m_text_search_block_size.ToString() : "verse";
                        int block_count = ((m_multiplicity_comparison_operator == ComparisonOperator.Equal) && (m_text_search_block_size != TextSearchBlockSize.Verse)) ? phrase_count / Math.Abs(m_multiplicity) : m_client.FoundVerses.Count;
                        m_find_result_header = phrase_count + " " + L[l]["matches"] + " " + L[l]["in"] + " " + block_count + " " + ((block_count == 1) ? L[l][block_name] : (L[l][block_name + "s"])) + " " + L[l]["with"] + " " + text_proximity_type.ToString() + " " + L[l]["in"] + " " + L[l][m_client.SearchScope.ToString()];
                        DisplayFoundVerses(true, true);

                        SearchResultTextBox.Focus();
                        SearchResultTextBox.Refresh();

                        WordsListBoxLabel.Visible = false;
                        WordsListBox.Visible = false;
                    }
                }
            }
        }
    }
    private void FindByRoot()
    {
        if (m_client != null)
        {
            ClearFindMatches();

            if (FindByTextTextBox.Text.Length > 0)
            {
                // get startup text from FindTextBox
                string[] startup_words = FindByTextTextBox.Text.Split();
                int count = startup_words.Length;

                string text = "";
                if (m_auto_complete_mode)
                {
                    for (int i = 0; i < count; i++)
                    {
                        text += startup_words[i] + " ";
                    }
                    text = text.Trim();
                }

                // update m_text_location_in_verse and m_text_location_in_word
                UpdateFindByTextOptions();

                List<Phrase> total_phrases = new List<Phrase>();
                List<Verse> total_verses = new List<Verse>();
                if (!String.IsNullOrEmpty(text))
                {
                    text = text.Trim();

                    m_client.FindPhrases(TextSearchBlockSize.Verse, text, m_multiplicity, m_multiplicity_number_type, m_multiplicity_comparison_operator, m_multiplicity_remainder);

                    total_phrases = total_phrases.Union(m_client.FoundPhrases);
                    total_verses = total_verses.Union(m_client.FoundVerses);

                    // write final result to m_client
                    m_client.FoundPhrases = total_phrases;
                    m_client.FoundVerses = total_verses;
                }

                // display results
                if (m_client.FoundPhrases != null)
                {
                    int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                    if (m_client.FoundVerses != null)
                    {
                        int verse_count = m_client.FoundVerses.Count;
                        m_find_result_header = phrase_count + " " + L[l]["matches"] + " " + L[l]["in"] + " " + verse_count + ((verse_count == 1) ? " " + L[l]["verse"] : " " + L[l]["verses"]) + " " + L[l]["with"] + " " + text + " C_" + m_text_location_in_chapter.ToString() + " V_" + m_text_location_in_verse.ToString() + " W_" + m_text_location_in_word.ToString() + " " + L[l]["in"] + " " + L[l][m_client.SearchScope.ToString()];
                        DisplayFoundVerses(true, true);

                        SearchResultTextBox.Focus();
                        SearchResultTextBox.Refresh();

                        WordsListBoxLabel.Visible = false;
                        WordsListBox.Visible = false;
                    }
                }
            }
        }
    }
    private void FindByRoot(string text)
    {
        m_search_type = SearchType.Text;

        if (m_client != null)
        {
            ClearFindMatches();

            if (!String.IsNullOrEmpty(text))
            {
                text = text.Trim();

                m_client.FindPhrases(m_text_search_block_size, text, m_multiplicity, m_multiplicity_number_type, m_multiplicity_comparison_operator, m_multiplicity_remainder);
                if (m_client.FoundPhrases != null)
                {
                    int phrase_count = GetPhraseCount(m_client.FoundPhrases);
                    string block_name = "verse";
                    int block_count = m_client.FoundVerses.Count;
                    if (m_multiplicity == 0)
                    {
                        m_find_result_header = block_count + " " + ((block_count == 1) ? L[l][block_name] : (L[l][block_name + "s"])) + " " + L[l]["without"] + " " + " root " + text + " " + L[l]["in"] + " " + L[l][m_client.SearchScope.ToString()];
                    }
                    else
                    {
                        m_find_result_header = phrase_count + " " + L[l]["matches"] + " " + L[l]["in"] + " " + block_count + " " + ((block_count == 1) ? L[l][block_name] : (L[l][block_name + "s"])) + " " + L[l]["with"] + " " + L[l]["root"] + " " + text + " " + L[l]["in"] + " " + L[l][m_client.SearchScope.ToString()];
                    }
                    DisplayFoundVerses(true, true);

                    SearchResultTextBox.Focus();
                    SearchResultTextBox.Refresh();
                }
            }
        }
    }
    private void FindByTextKeyboardLabel_Click(object sender, EventArgs e)
    {
        Control control = (sender as Control);
        if (control != null)
        {
            control.BackColor = Color.LightSteelBlue;
            control.Refresh();

            // prevent double spaces
            if (control == FindByTextSpaceLabel)
            {
                if (FindByTextTextBox.SelectionStart > 0)
                {
                    if (FindByTextTextBox.Text[FindByTextTextBox.SelectionStart - 1] == ' ')
                    {
                        return;
                    }
                }
            }

            string letter = control.Text[0].ToString();
            int pos = FindByTextTextBox.SelectionStart;
            int len = FindByTextTextBox.SelectionLength;
            if (pos > -1)
            {
                if (len > 0)
                {
                    FindByTextTextBox.Text = FindByTextTextBox.Text.Remove(pos, len);
                }
                else
                {
                    // do nothing
                }
                FindByTextTextBox.Text = FindByTextTextBox.Text.Insert(pos, letter);
                FindByTextTextBox.SelectionStart = pos + 1;
                FindByTextTextBox.Refresh();
            }

            Thread.Sleep(100);
            control.BackColor = Color.LightGray;
            control.Refresh();

            FindByTextKeyboardLabel_MouseEnter(sender, e);
            FindByTextControls_Enter(null, null);

            FindByTextTextBox.Focus();
        }
    }
    private void FindByTextBackspaceLabel_Click(object sender, EventArgs e)
    {
        Control control = (sender as Control);
        if (control != null)
        {
            control.BackColor = Color.LightSteelBlue;
            control.Refresh();

            int pos = FindByTextTextBox.SelectionStart;
            int len = FindByTextTextBox.SelectionLength;
            if ((len == 0) && (pos > 0))        // delete character prior to cursor
            {
                FindByTextTextBox.Text = FindByTextTextBox.Text.Remove(pos - 1, 1);
                FindByTextTextBox.SelectionStart = pos - 1;
            }
            else if ((len > 0) && (pos >= 0))   // delete current highlighted characters
            {
                FindByTextTextBox.Text = FindByTextTextBox.Text.Remove(pos, len);
                FindByTextTextBox.SelectionStart = pos;
            }
            else                  // nothing to delete
            {
            }
            FindByTextTextBox.Refresh();

            Thread.Sleep(100);
            control.BackColor = Color.LightGray;
            control.Refresh();

            FindByTextKeyboardLabel_MouseEnter(sender, e);
            FindByTextControls_Enter(null, null);

            FindByTextTextBox.Focus();
        }
    }
    private void FindByTextKeyboardLabel_MouseEnter(object sender, EventArgs e)
    {
        Control control = (sender as Control);
        if (control != null)
        {
            if (control == FindByTextBackspaceLabel)
            {
                control.BackColor = Color.DarkGray;
            }
            else
            {
                control.BackColor = Color.White;
            }
            control.Refresh();
        }
    }
    private void FindByTextKeyboardLabel_MouseLeave(object sender, EventArgs e)
    {
        Control control = (sender as Control);
        if (control != null)
        {
            control.BackColor = Color.LightGray;
            control.Refresh();
        }
    }
    private void FindByTextKeyboardModifierLabel_MouseLeave(object sender, EventArgs e)
    {
        Control control = (sender as Control);
        if (control != null)
        {
            control.BackColor = Color.Silver;
            control.Refresh();
        }
    }
    private void FindByTextOrLabel_MouseHover(object sender, EventArgs e)
    {
        char[] quran_healing_characters = { '♥' };
        char[] idhaar_characters = { 'ء', 'أ', 'إ', 'ح', 'خ', 'ع', 'غ', 'ه', 'ة', 'ى' };
        char[] wasl_characters = { 'ٱ' };
        char[] med_characters = { 'ا', 'آ' };
        char[] iqlaab_characters = { 'ب' };
        char[] idghaam_characters = { 'ر', 'ل' };
        char[] idghaam_ghunna_characters = { 'م', 'ن', 'و', 'ؤ', 'ي', 'ئ' };
        char[] ikhfaa_characters = { 'ت', 'ث', 'ج', 'د', 'ذ', 'ز', 'س', 'ش', 'ص', 'ض', 'ط', 'ظ', 'ف', 'ق', 'ك' };

        Control control = (sender as Control);
        if (control != null)
        {
            string character_sound = null;

            if (character_sound == null)
            {
                foreach (char c in med_characters)
                {
                    if (c == control.Text[0])
                    {
                        character_sound = "مدّ";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char c in wasl_characters)
                {
                    if (c == control.Text[0])
                    {
                        character_sound = "إيصال";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char c in iqlaab_characters)
                {
                    if (c == control.Text[0])
                    {
                        character_sound = "إقلاب";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char c in idghaam_ghunna_characters)
                {
                    if (c == control.Text[0])
                    {
                        character_sound = "إدغام بغنة";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char c in idghaam_characters)
                {
                    if (c == control.Text[0])
                    {
                        //character_sound = "إدغام بلا غنة";
                        character_sound = "إدغام";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char c in idhaar_characters)
                {
                    if (c == control.Text[0])
                    {
                        character_sound = "إظهار";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char c in ikhfaa_characters)
                {
                    if (c == control.Text[0])
                    {
                        //character_sound = "إخفاء بغنة";
                        character_sound = "إخفاء";
                        break;
                    }
                }
            }
            if (character_sound == null)
            {
                foreach (char c in quran_healing_characters)
                {
                    if (c == control.Text[0])
                    {
                        character_sound = "الشفاء بالقرءان \r\nإن شاء الله ";
                        break;
                    }
                }
            }

            int start = "FindByText".Length;
            int length = control.Name.Length - start - "Label".Length;
            ToolTip.SetToolTip(control, control.Name.Substring(start, length) + " " + character_sound);
        }
    }
    private void ResetFindByTextSearchTypeLabels()
    {
        FindByTextExactSearchTypeLabel.BackColor = Color.DarkGray;
        FindByTextExactSearchTypeLabel.BorderStyle = BorderStyle.None;
        FindByTextProximitySearchTypeLabel.BackColor = Color.DarkGray;
        FindByTextProximitySearchTypeLabel.BorderStyle = BorderStyle.None;
        FindByTextRootSearchTypeLabel.BackColor = Color.DarkGray;
        FindByTextRootSearchTypeLabel.BorderStyle = BorderStyle.None;
    }
    private void FindByTextControl_EnabledChanged(object sender, EventArgs e)
    {
        Control control = sender as Control;
        if (control != null)
        {
            control.BackColor = (control.Enabled) ? SystemColors.Window : Color.LightGray;
        }
    }
    private void UpdateKeyboard(string text_mode)
    {
        // allow all letters in Root search type
        if (m_text_search_type == TextSearchType.Root) text_mode = "Original";

        FindByTextHamzaLabel.Visible = false;
        FindByTextTaaMarbootaLabel.Visible = false;
        FindByTextElfMaqsuraLabel.Visible = false;
        FindByTextElfWaslLabel.Visible = false;
        FindByTextHamzaAboveElfLabel.Visible = false;
        FindByTextHamzaBelowElfLabel.Visible = false;
        FindByTextHamzaAboveWawLabel.Visible = false;
        FindByTextHamzaAboveYaaLabel.Visible = false;
        LetterFrequencyWithDiacriticsCheckBox.Visible = true;

        if (text_mode == "Simplified28")
        {
            // do nothing
        }
        else if (text_mode == "Simplified29")
        {
            FindByTextHamzaLabel.Visible = true;
        }
        else if (text_mode == "Simplified30")
        {
            FindByTextTaaMarbootaLabel.Visible = true;
            FindByTextElfMaqsuraLabel.Visible = true;
        }
        else if (text_mode == "Simplified31")
        {
            FindByTextHamzaLabel.Visible = true;
            FindByTextTaaMarbootaLabel.Visible = true;
            FindByTextElfMaqsuraLabel.Visible = true;
        }
        else if (text_mode == "Simplified36")
        {
            FindByTextHamzaLabel.Visible = true;

            FindByTextTaaMarbootaLabel.Visible = true;
            FindByTextElfMaqsuraLabel.Visible = true;

            FindByTextElfWaslLabel.Visible = true;
            FindByTextHamzaAboveElfLabel.Visible = true;
            FindByTextHamzaBelowElfLabel.Visible = true;
            FindByTextHamzaAboveWawLabel.Visible = true;
            FindByTextHamzaAboveYaaLabel.Visible = true;
        }
        else if (text_mode == "Original")
        {
            FindByTextHamzaLabel.Visible = true;

            FindByTextTaaMarbootaLabel.Visible = true;
            FindByTextElfMaqsuraLabel.Visible = true;

            FindByTextElfWaslLabel.Visible = true;
            FindByTextHamzaAboveElfLabel.Visible = true;
            FindByTextHamzaBelowElfLabel.Visible = true;
            FindByTextHamzaAboveWawLabel.Visible = true;
            FindByTextHamzaAboveYaaLabel.Visible = true;
            LetterFrequencyWithDiacriticsCheckBox.Visible = true;
        }
        else
        {
            // do nothing
        }
    }
    private void EnableFindByTextControls()
    {
        FindByTextExactSearchTypeLabel.BackColor = (m_text_search_type == TextSearchType.Exact) ? Color.SteelBlue : Color.DarkGray;
        FindByTextExactSearchTypeLabel.BorderStyle = (m_text_search_type == TextSearchType.Exact) ? BorderStyle.Fixed3D : BorderStyle.None;
        FindByTextProximitySearchTypeLabel.BackColor = (m_text_search_type == TextSearchType.Proximity) ? Color.SteelBlue : Color.DarkGray;
        FindByTextProximitySearchTypeLabel.BorderStyle = (m_text_search_type == TextSearchType.Proximity) ? BorderStyle.Fixed3D : BorderStyle.None;
        FindByTextRootSearchTypeLabel.BackColor = (m_text_search_type == TextSearchType.Root) ? Color.SteelBlue : Color.DarkGray;
        FindByTextRootSearchTypeLabel.BorderStyle = (m_text_search_type == TextSearchType.Root) ? BorderStyle.Fixed3D : BorderStyle.None;

        FindByTextAtChapterStartRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtChapterMiddleRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtChapterEndRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtChapterAnyRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);

        FindByTextAtVerseStartRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtVerseMiddleRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtVerseEndRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);
        FindByTextAtVerseAnyRadioButton.Enabled = (m_text_search_type == TextSearchType.Exact);

        FindByTextAllWordsRadioButton.Enabled = (m_text_search_type == TextSearchType.Proximity);
        FindByTextAnyWordRadioButton.Enabled = (m_text_search_type == TextSearchType.Proximity)
                                                && (!FindByTextTextBox.Text.Contains("-"))
                                                && (!FindByTextTextBox.Text.Contains("+"));
        FindByTextPlusLabel.Visible = ((m_text_search_type == TextSearchType.Proximity) || (m_text_search_type == TextSearchType.Root));
        FindByTextMinusLabel.Visible = ((m_text_search_type == TextSearchType.Proximity) || (m_text_search_type == TextSearchType.Root));

        FindByTextWordnessCheckBox.Enabled = ((m_text_search_type == TextSearchType.Exact) || (m_text_search_type == TextSearchType.Proximity));

        FindByTextAtWordStartRadioButton.Enabled = ((m_text_search_type == TextSearchType.Exact) || (m_text_search_type == TextSearchType.Root));
        FindByTextAtWordMiddleRadioButton.Enabled = ((m_text_search_type == TextSearchType.Exact) || (m_text_search_type == TextSearchType.Root));
        FindByTextAtWordEndRadioButton.Enabled = ((m_text_search_type == TextSearchType.Exact) || (m_text_search_type == TextSearchType.Root));
        FindByTextAtWordAnyRadioButton.Enabled = ((m_text_search_type == TextSearchType.Exact) || (m_text_search_type == TextSearchType.Root));
    }
    private string m_current_phrase = "";
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Display Search Results
    ///////////////////////////////////////////////////////////////////////////////
    private struct FindMatch
    {
        public int Start;
        public int Length;
    }
    // F3 and Shift+F3 Goto next/previous matches
    private List<FindMatch> m_find_matches = null;
    private void BuildFindMatch(int start, int length)
    {
        // build text_selections list for F3 and Shift+F3
        if (m_find_matches != null)
        {
            FindMatch find_match = new FindMatch();
            find_match.Start = start;
            find_match.Length = length;
            m_find_matches.Add(find_match);
        }
    }
    private int m_find_match_index = -1;
    private void GotoPreviousFindMatch()
    {
        if (m_find_matches != null)
        {
            m_find_match_index = -1;
            for (int i = 0; i < m_find_matches.Count; i++)
            {
                if (m_find_matches[i].Start > SearchResultTextBox.SelectionStart)
                {
                    m_find_match_index = i - 1;
                    break;
                }
            }
        }
    }
    private void GotoNextFindMatch()
    {
        if (m_find_matches != null)
        {
            m_find_match_index = m_find_matches.Count;
            for (int i = m_find_matches.Count - 1; i >= 0; i--)
            {
                if (m_find_matches[i].Start < SearchResultTextBox.SelectionStart)
                {
                    m_find_match_index = i + 1;
                    break;
                }
            }
        }
    }
    private void SelectNextFindMatch()
    {
        if (m_found_verses_displayed)
        {
            if (m_find_matches != null)
            {
                if (m_find_matches.Count > 0)
                {
                    // find the index prior to the current cursor postion
                    GotoPreviousFindMatch();
                    m_find_match_index++;

                    // round robin
                    if (m_find_match_index == m_find_matches.Count)
                    {
                        m_find_match_index = 0;
                    }

                    // find next match
                    if ((m_find_match_index >= 0) && (m_find_match_index < m_find_matches.Count))
                    {
                        int start = m_find_matches[m_find_match_index].Start;
                        int length = m_find_matches[m_find_match_index].Length;
                        if ((start >= 0) && (start < SearchResultTextBox.Text.Length))
                        {
                            SearchResultTextBox.Select(start, length);
                            SearchResultTextBox.SelectionColor = Color.Red;
                        }
                    }
                }
            }
        }
        UpdateFindMatchCaption();
    }
    private void SelectPreviousFindMatch()
    {
        if (m_found_verses_displayed)
        {
            if (m_find_matches != null)
            {
                if (m_find_matches.Count > 0)
                {
                    // find the index after the current cursor postion
                    GotoNextFindMatch();
                    m_find_match_index--;

                    // round robin
                    if (m_find_match_index < 0)
                    {
                        m_find_match_index = m_find_matches.Count - 1;
                    }

                    // find previous match
                    if ((m_find_match_index >= 0) && (m_find_match_index < m_find_matches.Count))
                    {
                        int start = m_find_matches[m_find_match_index].Start;
                        int length = m_find_matches[m_find_match_index].Length;
                        if ((start >= 0) && (start < SearchResultTextBox.Text.Length))
                        {
                            SearchResultTextBox.Select(start, length);
                            SearchResultTextBox.SelectionColor = Color.Red;
                        }
                    }
                }
            }
        }
        UpdateFindMatchCaption();
    }
    private void UpdateFindMatchCaption()
    {
        string caption = this.Text;
        int pos = caption.IndexOf(CAPTION_SEPARATOR);
        if (pos > -1)
        {
            caption = caption.Substring(0, pos);
        }

        if (m_found_verses_displayed)
        {
            if (m_find_matches != null)
            {
                caption += CAPTION_SEPARATOR + " " + L[l]["Match"] + " " + ((m_find_match_index + 1) + "/" + m_find_matches.Count);
            }
        }
        else
        {
            //caption += CAPTION_SEPARATOR;
        }

        //this.Text = caption;
    }

    private string m_find_result_header = null;
    private void UpdateHeaderLabel()
    {
        if (m_client != null)
        {
            string text = "";
            int number = 0;

            if (m_found_verses_displayed)
            {
                if (m_find_result_header != null)
                {
                    text = m_find_result_header;

                    string[] parts = m_find_result_header.Split();
                    if (parts.Length > 0)
                    {
                        if (int.TryParse(parts[0], out number))
                        {
                            // do nothing
                        }
                    }
                }
            }
            else
            {
                Verse verse = GetCurrentVerse();
                if (verse != null)
                {
                    if (verse.Chapter != null)
                    {
                        //text = L[l]["Chapter"] + "  " + verse.Chapter.SortedNumber + " " + L[l][verse.Chapter.TransliteratedName] + "   "
                        text = L[l][verse.Chapter.TransliteratedName] + " " + verse.Chapter.SortedNumber + "   "
                             + L[l]["Verse"] + " " + verse.NumberInChapter + "   "
                            //+ L[l]["Station"] + " " + ((verse.Station != null) ? verse.Station.Number : -1) + "   "
                            //+ L[l]["Part"] + " " + ((verse.Part != null) ? verse.Part.Number : -1) + "   "
                            //+ L[l]["Group"] + " " + ((verse.Group != null) ? verse.Group.Number : -1) + "   "
                            //+ L[l]["Half"] + " " + ((verse.Half != null) ? verse.Half.Number : -1) + "   "
                            //+ L[l]["Quarter"] + " " + ((verse.Quarter != null) ? verse.Quarter.Number : -1) + "   "
                            //+ L[l]["Bowing"] + " " + ((verse.Bowing != null) ? verse.Bowing.Number : -1) + "   "
                            //+ "     "
                            //+ L[l]["Page"] + " " + ((verse.Page != null) ? verse.Page.Number : -1)
                        ;

                        number = verse.NumberInChapter;
                    }
                }
            }

            HeaderLabel.Text = text;
            HeaderLabel.ForeColor = Numbers.GetNumberTypeColor(number);
            HeaderLabel.Refresh();
        }
    }

    private RichTextBoxEx m_active_textbox = null;
    private bool m_found_verses_displayed = false;
    private void SwitchToMainTextBox()
    {
        if (m_active_textbox != null)
        {
            if (m_found_verses_displayed)
            {
                m_found_verses_displayed = false;
                PopulateChaptersListBox();
            }

            // in all cases
            SearchResultTextBox.Visible = false;
            MainTextBox.Visible = true;
            m_active_textbox = MainTextBox;

            UpdateWordWrapLabel(m_active_textbox.WordWrap);

            DisplayChapterRevelationInfo();
        }
    }
    private void SwitchToSearchResultTextBox()
    {
        if (m_active_textbox != null)
        {
            // allow subsequent Finds to update chapter list, and browse history
            m_found_verses_displayed = true;
            PopulateChaptersListBox();

            // in all cases
            MainTextBox.Visible = false;
            SearchResultTextBox.Visible = true;
            m_active_textbox = SearchResultTextBox;
            //m_active_textbox.Refresh();

            UpdateWordWrapLabel(m_active_textbox.WordWrap);
            UpdateFindMatchCaption();
        }
    }

    private int[] m_matches_per_chapter = null;
    private void DisplayFoundVerses(bool add_to_history, bool colorize_chapters_by_matches)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.FoundVerses != null)
                {
                    ZoomInLabel.Enabled = (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin));
                    ZoomOutLabel.Enabled = (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin));

                    if (colorize_chapters_by_matches)
                    {
                        if (m_client.Book != null)
                        {
                            m_matches_per_chapter = new int[m_client.Book.Chapters.Count];
                            if ((m_client.FoundPhrases != null) && (m_client.FoundPhrases.Count > 0))
                            {
                                foreach (Phrase phrase in m_client.FoundPhrases)
                                {
                                    if (phrase != null)
                                    {
                                        if (phrase.Verse != null)
                                        {
                                            if (phrase.Verse.Chapter != null)
                                            {
                                                m_matches_per_chapter[phrase.Verse.Chapter.SortedNumber - 1]++;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (Verse verse in m_client.FoundVerses)
                                {
                                    if (verse != null)
                                    {
                                        if (verse.Chapter != null)
                                        {
                                            m_matches_per_chapter[verse.Chapter.SortedNumber - 1]++;
                                        }
                                    }
                                }
                            }
                        }

                        SwitchToSearchResultTextBox();
                    }

                    for (int i = 0; i < 3; i++) SearchResultTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
                    for (int i = 0; i < 3; i++) SearchResultTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
                    SearchResultTextBox.BeginUpdate();

                    StringBuilder str = new StringBuilder();
                    foreach (Verse verse in m_client.FoundVerses)
                    {
                        if (verse != null)
                        {
                            str.Append(verse.ArabicAddress + "\t" + verse.Text + "\n");
                        }
                    }
                    if (str.Length > 1)
                    {
                        str.Remove(str.Length - 1, 1);
                    }
                    m_current_text = str.ToString();

                    m_selection_mode = true;
                    UpdateHeaderLabel();
                    SearchResultTextBox.Text = m_current_text;
                    SearchResultTextBox.Refresh();

                    CalculateCurrentValue();

                    // phrase statistics
                    if (m_client.FoundPhrases != null)
                    {
                        if (m_client.FoundPhrases.Count > 0)
                        {
                            CalcualatePhraseStatistics();
                        }
                    }

                    BuildLetterFrequencies();
                    DisplayLetterFrequencies();

                    if (m_client.FoundPhrases != null)
                    {
                        ColorizePhrases();
                        BuildFindMatches();
                        HighlightVerses();
                    }

                    m_current_found_verse_index = 0;
                    DisplayCurrentPositions();

                    m_current_found_verse_index = 0;
                    RealignFoundMatchedToStart();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            SearchResultTextBox.EndUpdate();
            SearchResultTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            SearchResultTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }
    private void CalcualatePhraseStatistics()
    {
        StringBuilder phrase_str = new StringBuilder();
        int word_count = 0;
        int letter_count = 0;
        long value = 0L;
        foreach (Phrase phrase in m_client.FoundPhrases)
        {
            if (phrase != null)
            {
                phrase_str.AppendLine(phrase.Text);
                word_count += phrase.Text.Split(' ').Length;
                string phrase_nospaces = phrase.Text.SimplifyTo(m_client.NumerologySystem.TextMode).Replace(" ", "");
                letter_count += phrase_nospaces.Length;
                value += m_client.CalculateValue(phrase.Text);
            }
        }

        WordsTextBox.Text = Radix.Encode(word_count, m_radix);
        WordsTextBox.ForeColor = Numbers.GetNumberTypeColor(WordsTextBox.Text, m_radix);
        WordsTextBox.BackColor = (Numbers.Compare(word_count, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.ControlLight;
        WordsTextBox.Refresh();
        LettersTextBox.Text = Radix.Encode(letter_count, m_radix);
        LettersTextBox.ForeColor = Numbers.GetNumberTypeColor(LettersTextBox.Text, m_radix);
        LettersTextBox.BackColor = (Numbers.Compare(letter_count, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.ControlLight;
        LettersTextBox.Refresh();
        ValueTextBox.Text = Radix.Encode(value, m_radix);
        ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(value);
        ValueTextBox.SelectionStart = ValueTextBox.Text.Length;
        ValueTextBox.SelectionLength = 0;
        ValueTextBox.Refresh();
        FactorizeValue(value, true);
    }
    private void DisplayFoundChapters(bool add_to_history, bool colorize_chapters_by_matches)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.FoundChapters != null)
                {
                    ZoomInLabel.Enabled = (m_text_zoom_factor <= (m_max_zoom_factor - m_zoom_factor_increment + m_error_margin));
                    ZoomOutLabel.Enabled = (m_text_zoom_factor >= (m_min_zoom_factor + m_zoom_factor_increment - m_error_margin));

                    if (colorize_chapters_by_matches)
                    {
                        if (m_client.Book != null)
                        {
                            m_matches_per_chapter = new int[m_client.Book.Chapters.Count];
                            foreach (Chapter chapter in m_client.FoundChapters)
                            {
                                if (chapter != null)
                                {
                                    m_matches_per_chapter[chapter.SortedNumber - 1]++;
                                }
                            }
                        }

                        SwitchToSearchResultTextBox();
                    }

                    for (int i = 0; i < 3; i++) ChaptersListBox.SelectedIndexChanged -= new EventHandler(ChaptersListBox_SelectedIndexChanged);
                    if (m_client.FoundChapters.Count > 0)
                    {
                        ChaptersListBox.SelectedIndices.Clear();
                        foreach (Chapter chapter in m_client.FoundChapters)
                        {
                            if (((chapter.SortedNumber - 1) >= 0) && ((chapter.SortedNumber - 1) < ChaptersListBox.Items.Count))
                            {
                                ChaptersListBox.SelectedIndices.Add(chapter.SortedNumber - 1);
                            }
                        }
                    }
                    else
                    {
                        ChaptersListBox.SelectedIndices.Clear();
                    }
                    ChaptersListBox.SelectedIndexChanged += new EventHandler(ChaptersListBox_SelectedIndexChanged);
                    UpdateSelection();

                    for (int i = 0; i < 3; i++) SearchResultTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
                    for (int i = 0; i < 3; i++) SearchResultTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
                    SearchResultTextBox.BeginUpdate();

                    StringBuilder str = new StringBuilder();
                    foreach (Chapter chapter in m_client.FoundChapters)
                    {
                        foreach (Verse verse in chapter.Verses)
                        {
                            if (verse != null)
                            {
                                str.Append(verse.ArabicAddress + "\t" + verse.Text + "\n");
                            }
                        }
                    }
                    if (str.Length > 1)
                    {
                        str.Remove(str.Length - 1, 1);
                    }
                    m_current_text = str.ToString();

                    m_selection_mode = true;
                    UpdateHeaderLabel();
                    SearchResultTextBox.Text = m_current_text;
                    SearchResultTextBox.Refresh();

                    CalculateCurrentValue();

                    BuildLetterFrequencies();
                    DisplayLetterFrequencies();

                    ColorizeChapters(); // too slow

                    m_current_found_verse_index = 0;
                    DisplayCurrentPositions();

                    List<Verse> verses = new List<Verse>();
                    foreach (Chapter chapter in m_client.FoundChapters)
                    {
                        verses.AddRange(chapter.Verses);
                    }

                    m_current_found_verse_index = 0;
                    RealignFoundMatchedToStart();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            SearchResultTextBox.EndUpdate();
            SearchResultTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            SearchResultTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
            this.Cursor = Cursors.Default;
        }
    }

    private void BuildFindMatches()
    {
        if (m_client != null)
        {
            if (m_client.FoundPhrases != null)
            {
                foreach (Phrase phrase in m_client.FoundPhrases)
                {
                    if (phrase != null)
                    {
                        if (phrase.Verse != null)
                        {
                            int start = GetPhrasePositionInRichTextBox(phrase);
                            if ((start >= 0) && (start < SearchResultTextBox.Text.Length))
                            {
                                if (phrase.Text != null)
                                {
                                    int length = phrase.Text.Length;
                                    BuildFindMatch(start, length);
                                }
                            }
                        }
                    }
                }
            }
        }
        UpdateFindMatchCaption();
    }
    private void ColorizePhrases()
    {
        if (m_client != null)
        {
            if (m_client.FoundPhrases != null)
            {
                foreach (Phrase phrase in m_client.FoundPhrases)
                {
                    if (phrase != null)
                    {
                        if (phrase.Verse != null)
                        {
                            int start = GetPhrasePositionInRichTextBox(phrase);
                            if ((start >= 0) && (start < SearchResultTextBox.Text.Length))
                            {
                                if (phrase.Text != null)
                                {
                                    int extra = 0;
                                    foreach (char c in phrase.Text)
                                    {
                                        if ((Constants.STOPMARKS.Contains(c)) || (Constants.QURANMARKS.Contains(c)))
                                        {
                                            extra += 2;
                                        }
                                    }
                                    int length = phrase.Text.Length + extra;
                                    SearchResultTextBox.Select(start, length);
                                    SearchResultTextBox.SelectionColor = Color.Red;
                                }
                            }
                        }
                    }
                }

                UpdateFindMatchCaption();
            }
        }
    }
    private Dictionary<Verse, Color> m_found_verse_backcolors = new Dictionary<Verse, Color>();
    private void HighlightVerses()
    {
        try
        {
            for (int i = 0; i < 3; i++) SearchResultTextBox.TextChanged -= new EventHandler(MainTextBox_TextChanged);
            for (int i = 0; i < 3; i++) SearchResultTextBox.SelectionChanged -= new EventHandler(MainTextBox_SelectionChanged);
            SearchResultTextBox.BeginUpdate();
            SearchResultTextBox.ClearHighlight();

            if (m_client != null)
            {
                m_found_verse_backcolors.Clear();

                Dictionary<Verse, int> phrases_per_verse_dictionary = new Dictionary<Verse, int>();
                if (m_client.FoundPhrases != null)
                {
                    if (m_client.FoundVerses != null)
                    {
                        if (m_client.FoundPhrases.Count >= m_client.FoundVerses.Count)
                        {
                            foreach (Phrase phrase in m_client.FoundPhrases)
                            {
                                if (phrase != null)
                                {
                                    if (phrases_per_verse_dictionary.ContainsKey(phrase.Verse))
                                    {
                                        phrases_per_verse_dictionary[phrase.Verse]++;
                                    }
                                    else
                                    {
                                        phrases_per_verse_dictionary.Add(phrase.Verse, 1);
                                    }
                                }
                            }

                            foreach (Verse verse in m_client.FoundVerses)
                            {
                                int start = GetVerseDisplayStart(verse);
                                int length = GetVerseDisplayLength(verse);
                                if (phrases_per_verse_dictionary.ContainsKey(verse))
                                {
                                    int match_count = phrases_per_verse_dictionary[verse];

                                    // use color shading to represent match_count visually
                                    if (match_count > 1)
                                    {
                                        int red = 255;
                                        int green = 255;
                                        int blue = 255;
                                        green -= ((match_count - 1) * 32);
                                        if (green < 0)
                                        {
                                            red += green;
                                            green = 0;
                                        }
                                        if (red < 0)
                                        {
                                            blue += red;
                                            red = 0;
                                        }
                                        if (blue < 0)
                                        {
                                            blue = 0;
                                        }
                                        m_found_verse_backcolors.Add(verse, Color.FromArgb(red, green, blue));
                                    }
                                    else
                                    {
                                        m_found_verse_backcolors.Add(verse, SearchResultTextBox.BackColor);
                                    }
                                    SearchResultTextBox.Highlight(start, length - 1, m_found_verse_backcolors[verse]);
                                }
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            SearchResultTextBox.EndUpdate();
            SearchResultTextBox.SelectionChanged += new EventHandler(MainTextBox_SelectionChanged);
            SearchResultTextBox.TextChanged += new EventHandler(MainTextBox_TextChanged);
        }
    }
    private void ColorizeChapters()
    {
        if (m_client != null)
        {
            if (m_client.FoundChapters != null)
            {
                if (m_client.FoundChapters.Count > 0)
                {
                    bool colorize = true; // colorize chapters alternatively

                    int line_index = 0;
                    foreach (Chapter chapter in m_client.FoundChapters)
                    {
                        if (chapter != null)
                        {
                            colorize = !colorize; // alternate colorization of chapters

                            int start = SearchResultTextBox.GetLinePosition(line_index);
                            int length = 0;
                            foreach (Verse verse in chapter.Verses)
                            {
                                length += SearchResultTextBox.Lines[line_index].Length + 1; // "\n"
                                line_index++;
                            }
                            SearchResultTextBox.Select(start, length);
                            SearchResultTextBox.SelectionColor = colorize ? Color.Blue : Color.Navy;
                        }
                    }
                }
            }
        }

        //FIX to reset SelectionColor
        SearchResultTextBox.Select(0, 1);
        SearchResultTextBox.SelectionColor = Color.Navy;
        SearchResultTextBox.Select(0, 0);
        SearchResultTextBox.SelectionColor = Color.Navy;
    }

    private void InspectVersesLabel_Click(object sender, EventArgs e)
    {
        if (m_client == null) return;
        if (m_client.NumerologySystem == null) return;
        if (m_client.Book == null) return;
        if (m_client.Selection == null) return;

        string result = null;
        string filename = null;
        if (m_found_verses_displayed)
        {
            result = DisplayVerseInformation(m_client.FoundVerses);
            filename = m_client.NumerologySystem.Name + "_" + m_find_result_header.Replace(" ", "_").Replace("*", "") + Globals.OUTPUT_FILE_EXT;
        }
        else
        {
            List<Verse> verses = m_client.Selection.Verses;
            if (verses != null)
            {
                result = DisplayVerseInformation(verses);
            }

            StringBuilder str = new StringBuilder();
            str.Append(m_client.Selection.Scope.ToString());
            if (m_client.Selection.Scope == SelectionScope.Chapter)
            {
                foreach (Chapter chapter in m_client.Selection.Chapters)
                {
                    str.Append("." + chapter.SortedNumber);
                }
                if (str.Length > 100)
                {
                    str.Remove(100, str.Length - 100);
                    int pos = str.ToString().LastIndexOf('.');
                    if (pos > -1)
                    {
                        str.Remove(pos, str.Length - pos);
                    }

                    if (str[str.Length - 1] == '.')
                    {
                        str.Append("..");
                    }
                    else
                    {
                        str.Append("...");
                    }
                }
            }
            else
            {
                str.Length = 0;
                str.Append("Chapter");
                foreach (Chapter chapter in m_client.Selection.Chapters)
                {
                    str.Append("." + chapter.Number.ToString());
                }
            }

            filename = m_client.NumerologySystem.Name + "_" + str.ToString() + Globals.OUTPUT_FILE_EXT;
        }
        if (Directory.Exists(Globals.STATISTICS_FOLDER))
        {
            string path = Globals.STATISTICS_FOLDER + "/" + filename;
            FileHelper.SaveText(path, result);
            FileHelper.DisplayFile(path);
        }
    }
    private string DisplayChapterInformation(List<Chapter> chapters)
    {
        if (m_client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();

        str.Append("#" + "\t" + "Name" + (m_found_verses_displayed ? "\t" + FindByTextTextBox.Text : "") + "\t" + "Chapter" + "\t" + "Verses" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Value" + "\t");
        NumerologySystem numerology_system = m_client.NumerologySystem;
        if (numerology_system != null)
        {
            if (numerology_system.LetterValues.Keys.Count > 0)
            {
                foreach (char key in numerology_system.LetterValues.Keys)
                {
                    str.Append(key.ToString() + "\t");
                }
                if (str.Length > 1)
                {
                    str.Remove(str.Length - 1, 1); // \t
                }
                str.Append("\r\n");
            }

            int count = 0;
            int sum = 0;
            int chapter_sum = 0;
            int verse_sum = 0;
            int word_sum = 0;
            int letter_sum = 0;
            long value_sum = 0L;
            foreach (Chapter chapter in chapters)
            {
                count++;
                sum += count;
                chapter_sum += chapter.SortedNumber;
                verse_sum += chapter.Verses.Count;
                word_sum += chapter.WordCount;
                letter_sum += chapter.LetterCount;
                long value = m_client.CalculateValue(chapter);
                value_sum += value;

                str.Append(count + "\t");
                str.Append(chapter.Name + "\t");
                if (m_found_verses_displayed)
                {
                    int index = chapter.SortedNumber - 1;
                    if (m_matches_per_chapter != null)
                    {
                        if ((index >= 0) && (index < m_matches_per_chapter.Length))
                        {
                            int match_count = m_matches_per_chapter[index];
                            str.Append(match_count + "\t");
                        }
                    }
                }
                str.Append(chapter.SortedNumber.ToString() + "\t");
                str.Append(chapter.Verses.Count.ToString() + "\t");
                str.Append(chapter.WordCount.ToString() + "\t");
                str.Append(chapter.LetterCount.ToString() + "\t");
                str.Append(value.ToString() + "\t");
                if (numerology_system.LetterValues.Keys.Count > 0)
                {
                    foreach (char key in numerology_system.LetterValues.Keys)
                    {
                        str.Append(chapter.GetLetterFrequency(key) + "\t");
                    }
                    if (str.Length > 1)
                    {
                        str.Remove(str.Length - 1, 1); // \t
                    }
                    str.Append("\r\n");
                }
            }
            if (str.Length > 2)
            {
                str.Remove(str.Length - 2, 2);
            }

            str.Append("\r\n");
            str.AppendLine(sum + "\t" + "Sum" + "\t" + chapter_sum + "\t" + verse_sum + "\t" + word_sum + "\t" + letter_sum + "\t" + value_sum);
        }
        return str.ToString();
    }
    private string DisplayVerseInformation(List<Verse> verses)
    {
        if (m_client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();

        str.Append("#" + "\t" + "Number" + "\t" + "Chapter" + "\t" + "Verse" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Value" + "\t");

        NumerologySystem numerology_system = m_client.NumerologySystem;
        if (numerology_system != null)
        {
            foreach (char key in numerology_system.LetterValues.Keys)
            {
                str.Append(key.ToString() + "\t");
            }
            str.Append("Text");
            str.Append("\r\n");

            int count = 0;
            int sum = 0;
            int verse_sum = 0;
            int chapter_sum = 0;
            int chapter_verse_sum = 0;
            int word_sum = 0;
            int letter_sum = 0;
            long value_sum = 0L;
            foreach (Verse verse in verses)
            {
                count++;
                sum += count;
                verse_sum += verse.Number;
                chapter_sum += verse.Chapter.SortedNumber;
                chapter_verse_sum += verse.NumberInChapter;
                word_sum += verse.Words.Count;
                letter_sum += verse.LetterCount;
                long value = m_client.CalculateValue(verse);
                value_sum += value;

                str.Append(count.ToString() + "\t");
                str.Append(verse.Number.ToString() + "\t");
                str.Append(verse.Chapter.SortedNumber.ToString() + "\t");
                str.Append(verse.NumberInChapter.ToString() + "\t");
                str.Append(verse.Words.Count.ToString() + "\t");
                str.Append(verse.LetterCount.ToString() + "\t");
                str.Append(value.ToString() + "\t");

                foreach (char character in numerology_system.LetterValues.Keys)
                {
                    if (Constants.INDIAN_DIGITS.Contains(character)) continue;
                    if (Constants.STOPMARKS.Contains(character)) continue;
                    if (Constants.QURANMARKS.Contains(character)) continue;
                    if (Constants.OPEN_BRACKET[0] == character) continue;
                    if (Constants.CLOSE_BRACKET[0] == character) continue;
                    str.Append(verse.GetLetterFrequency(character).ToString() + "\t");
                }

                str.Append(verse.Text);

                str.Append("\r\n");
            }
            if (str.Length > 2)
            {
                str.Remove(str.Length - 2, 2);
            }

            str.Append("\r\n");
            str.AppendLine(sum + "\t" + verse_sum + "\t" + chapter_sum + "\t" + chapter_verse_sum + "\t" + word_sum + "\t" + letter_sum + "\t" + value_sum);
        }
        return str.ToString();
    }
    private string DisplayWordInformation(List<Word> words)
    {
        if (words == null) return null;

        StringBuilder str = new StringBuilder();
        if (words.Count > 0)
        {
            str.Append
            (
                "Address" + "\t" +
                "Chapter" + "\t" +
                "Verse" + "\t" +
                "Word" + "\t" +
                "Text" + "\t" +
                "Transliteration" + "\t" +
                "Roots" + "\t" +
                "Meaning" + "\t" +
                "Occurrence" + "\t" +
                "Frequency" + "\t" +
                "Letters" + "\r\n"
            );

            foreach (Word word in words)
            {
                List<string> roots = word.Roots;
                StringBuilder roots_str = new StringBuilder();
                if (roots.Count > 0)
                {
                    foreach (string root in roots)
                    {
                        roots_str.Append(root + "|");
                    }
                    roots_str.Remove(roots_str.Length - 1, 1);
                }

                str.Append
                (
                    word.Address + "\t" +
                    word.Verse.Chapter.SortedNumber.ToString() + "\t" +
                    word.Verse.NumberInChapter.ToString() + "\t" +
                    word.NumberInVerse.ToString() + "\t" +
                    word.Text + "\t" +
                    word.Transliteration + "\t" +
                    roots_str.ToString() + "\t" +
                    word.Meaning + "\t" +
                    word.Occurrence.ToString() + "\t" +
                    word.Frequency.ToString() + "\t" +
                    word.Letters.Count.ToString() + "\r\n"
                );
            }
        }
        return str.ToString();
    }

    private int GetPhrasePositionInRichTextBox(Phrase phrase)
    {
        if (phrase == null) return 0;

        if (m_client != null)
        {
            if (m_client.FoundVerses != null)
            {
                int char_index = 0;
                foreach (Verse verse in m_client.FoundVerses)
                {
                    if (verse != null)
                    {
                        if (phrase.Verse.Number == verse.Number)
                        {
                            return (char_index + verse.Address.Length + 1 + phrase.Position);
                        }
                        char_index += GetVerseDisplayLength(verse);
                    }
                }
            }
        }
        return -1;
    }
    private void RealignFoundMatchedToStart()
    {
        if (m_client != null)
        {
            if (m_found_verses_displayed)
            {
                List<Verse> displayed_verses = new List<Verse>();
                if (m_client.FoundVerses != null)
                {
                    displayed_verses.AddRange(m_client.FoundVerses);
                }
                else if (m_client.FoundChapters != null)
                {
                    foreach (Chapter chapter in m_client.FoundChapters)
                    {
                        if (chapter != null)
                        {
                            displayed_verses.AddRange(chapter.Verses);
                        }
                    }
                }
                else if (m_client.FoundVerseRanges != null)
                {
                    foreach (List<Verse> range in m_client.FoundVerseRanges)
                    {
                        displayed_verses.AddRange(range);
                    }
                }
                else if (m_client.FoundChapterRanges != null)
                {
                    foreach (List<Chapter> range in m_client.FoundChapterRanges)
                    {
                        foreach (Chapter chapter in range)
                        {
                            if (chapter != null)
                            {
                                displayed_verses.AddRange(chapter.Verses);
                            }
                        }
                    }
                }

                int start = 0;
                // scroll to beginning to show complete verse address because in Arabic, pos=0 is after the first number :(
                if (m_client.FoundVerses != null)
                {
                    if (m_client.FoundVerses.Count > 0)
                    {
                        Verse verse = m_client.FoundVerses[0];
                        if (verse != null)
                        {
                            if (verse.Chapter != null)
                            {
                                if (verse.Chapter != null)
                                {
                                    start = verse.Chapter.SortedNumber.ToString().Length;
                                }
                            }
                        }
                    }

                    // re-align to text start
                    if ((start >= 0) && (start < SearchResultTextBox.Text.Length))
                    {
                        SearchResultTextBox.ScrollToCaret();    // must be called first
                        SearchResultTextBox.Select(start, 0);   // must be called second
                    }
                }
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Value Systems
    ///////////////////////////////////////////////////////////////////////////////
    private void LoadNumerologySystem(string numerology_system_name)
    {
        if (m_client != null)
        {
            m_client.LoadNumerologySystem(numerology_system_name);
        }
    }
    private void GLabel_Click(object sender, EventArgs e)
    {
        PLabel.BackColor = Color.MistyRose;
        FLabel.BackColor = Color.MistyRose;
        GLabel.BackColor = Color.MistyRose;

        LoadNumerologySystem("Original_Abjad_Gematria");
        CalculateCurrentValue();

        GLabel.BackColor = Color.RosyBrown;
    }
    private void PLabel_Click(object sender, EventArgs e)
    {
        PLabel.BackColor = Color.MistyRose;
        FLabel.BackColor = Color.MistyRose;
        GLabel.BackColor = Color.MistyRose;

        LoadNumerologySystem("Original_Alphabet_Primes1");
        CalculateCurrentValue();

        PLabel.BackColor = Color.RosyBrown;
    }
    private void FLabel_Click(object sender, EventArgs e)
    {
        PLabel.BackColor = Color.MistyRose;
        FLabel.BackColor = Color.MistyRose;
        GLabel.BackColor = Color.MistyRose;

        LoadNumerologySystem("Original_Frequency_Linear");
        CalculateCurrentValue();

        FLabel.BackColor = Color.RosyBrown;
    }
    /////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Value Calculations
    ///////////////////////////////////////////////////////////////////////////////
    private bool m_with_bism_Allah = true;
    private bool m_waw_as_word = false;
    private bool m_shadda_as_letter = false;
    private void BuildSimplifiedBookAndDisplaySelection()
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    // ALWAYS rebuild book to allow user to edit SimplificationRules file in Notepad and update text on the fly
                    //if ((m_client.Book.TextMode != text_mode) ||
                    //    (m_client.Book.WithBismAllah != m_with_bism_Allah) ||
                    //    (m_client.Book.WawAsWord != m_waw_as_word) ||
                    //    (m_client.Book.ShaddaAsLetter != m_shadda_as_letter)
                    //   )
                    {
                        string text_mode = m_client.NumerologySystem.TextMode;
                        m_client.BuildSimplifiedBook(text_mode, m_with_bism_Allah, m_waw_as_word, m_shadda_as_letter);

                        bool backup_found_verses_displayed = m_found_verses_displayed;

                        DisplaySelection(false);

                        // re-display search result if that was shown when text_mode was changed
                        if (backup_found_verses_displayed)
                        {
                            DisplayFoundVerses(false, false);

                            SearchResultTextBox.Focus();
                            SearchResultTextBox.Refresh();
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

    private void CalculateCurrentValue()
    {
        if (m_active_textbox != null)
        {
            if (m_client != null)
            {
                CalculateCurrentText();
                if (!String.IsNullOrEmpty(m_current_text))
                {
                    if (m_selection_mode)
                    {
                        if (m_found_verses_displayed)
                        {
                            if (m_client.FoundVerses != null)
                            {
                                CalculateAndDisplayCounts(m_client.FoundVerses);
                                CalculateValueAndDisplayFactors(m_client.FoundVerses);
                            }
                        }
                        else
                        {
                            if (m_client.Selection != null)
                            {
                                if (m_client.Selection.Verses != null)
                                {
                                    CalculateAndDisplayCounts(m_client.Selection.Verses);
                                    CalculateValueAndDisplayFactors(m_client.Selection.Verses);
                                }
                            }
                        }
                    }
                    else // cursor inside line OR some text is highlighted
                    {
                        if (m_active_textbox.SelectionLength == 0) // cursor inside line
                        {
                            Verse verse = GetCurrentVerse();
                            if (verse != null)
                            {
                                CalculateAndDisplayCounts(verse);
                                CalculateValueAndDisplayFactors(verse);
                            }
                        }
                        else // some text is selected
                        {
                            CalculateSelectedTextValue();
                        }
                    }
                }
            }
        }
    }
    private void CalculateCurrentText()
    {
        if (m_active_textbox != null)
        {
            if (m_selection_mode)
            {
                m_current_text = m_active_textbox.Text;
            }
            else
            {
                if (m_active_textbox.SelectionLength == 0) // get text at current line
                {
                    Verse verse = GetCurrentVerse();
                    if (verse != null)
                    {
                        m_current_text = verse.Text;
                    }
                    else
                    {
                        m_current_text = "";
                    }
                }
                else // get current selected text
                {
                    m_current_text = m_active_textbox.SelectedText;
                }
            }

            if (!String.IsNullOrEmpty(m_current_text))
            {
                m_current_text = RemoveVerseAddresses(m_current_text);
                m_current_text = RemoveVerseEndMarks(m_current_text);
                m_current_text = m_current_text.Trim();
                m_current_text = m_current_text.Replace("\n", "\r\n");
            }
        }
    }
    private void CalculateSelectedTextValue()
    {
        if (m_active_textbox != null)
        {
            if (m_client != null)
            {
                string selected_text = m_active_textbox.SelectedText;

                int first_char = m_active_textbox.SelectionStart;
                int last_char = m_active_textbox.SelectionStart + m_active_textbox.SelectionLength - 1;

                // skip any \n at beginning of selected text
                // skip any Endmark at beginning of selected text
                while (
                        (selected_text.Length > 0) &&
                        (
                          (selected_text[0] == '\n') ||
                          (selected_text[0] == '\r') ||
                          (selected_text[0] == '\t') ||
                          (selected_text[0] == '_') ||
                          (selected_text[0] == ' ') ||
                          (selected_text[0] == Constants.OPEN_BRACKET[0]) ||
                          (selected_text[0] == Constants.CLOSE_BRACKET[0]) ||
                          Constants.INDIAN_DIGITS.Contains(selected_text[0]) ||
                          Constants.STOPMARKS.Contains(selected_text[0]) ||
                          Constants.QURANMARKS.Contains(selected_text[0])
                        )
                      )
                {
                    selected_text = selected_text.Remove(0, 1);
                    first_char++;
                }

                // skip any \n at end of selected text
                // skip any Endmark at end of selected text
                while (
                        (selected_text.Length > 0) &&
                        (
                          (selected_text[selected_text.Length - 1] == '\n') ||
                          (selected_text[selected_text.Length - 1] == '\r') ||
                          (selected_text[selected_text.Length - 1] == '\t') ||
                          (selected_text[selected_text.Length - 1] == '_') ||
                          (selected_text[selected_text.Length - 1] == ' ') ||
                          (selected_text[selected_text.Length - 1] == Constants.OPEN_BRACKET[0]) ||
                          (selected_text[selected_text.Length - 1] == Constants.CLOSE_BRACKET[0]) ||
                          (selected_text[selected_text.Length - 1] == ' ') ||
                          Constants.INDIAN_DIGITS.Contains(selected_text[selected_text.Length - 1]) ||
                          Constants.STOPMARKS.Contains(selected_text[selected_text.Length - 1]) ||
                          Constants.QURANMARKS.Contains(selected_text[selected_text.Length - 1])
                        )
                      )
                {
                    selected_text = selected_text.Remove(selected_text.Length - 1);
                    last_char--;
                }

                List<Verse> highlighted_verses = new List<Verse>();
                Verse first_verse = GetVerseAtChar(first_char);
                if (first_verse != null)
                {
                    Verse last_verse = GetVerseAtChar(last_char);
                    if (last_verse != null)
                    {
                        List<Verse> verses = null;
                        if (m_found_verses_displayed)
                        {
                            verses = m_client.FoundVerses;
                        }
                        else
                        {
                            if (m_client.Selection != null)
                            {
                                verses = m_client.Selection.Verses;
                            }
                        }

                        if (verses != null)
                        {
                            int first_verse_index = GetVerseIndex(first_verse);
                            int last_verse_index = GetVerseIndex(last_verse);
                            for (int i = first_verse_index; i <= last_verse_index; i++)
                            {
                                highlighted_verses.Add(verses[i]);
                            }

                            Letter letter1 = GetLetterAtChar(first_char);
                            if (letter1 != null)
                            {
                                int first_verse_letter_index = letter1.NumberInVerse - 1;

                                Letter letter2 = GetLetterAtChar(last_char);
                                if (letter2 != null)
                                {
                                    int last_verse_letter_index = letter2.NumberInVerse - 1;

                                    // calculate and display verse_number_sum, word_number_sum, letter_number_sum
                                    CalculateAndDisplayCounts(highlighted_verses, first_verse_letter_index, last_verse_letter_index);

                                    // calculate Letters value
                                    CalculateValueAndDisplayFactors(highlighted_verses, first_verse_letter_index, last_verse_letter_index);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private string RemoveVerseAddresses(string text)
    {
        if (string.IsNullOrEmpty(text)) return null;

        string[] lines = text.Split('\n');
        StringBuilder str = new StringBuilder();
        foreach (string line in lines)
        {
            if (line.Length > 0)
            {
                string[] line_parts = line.Split('\t'); // (TAB delimited)
                if (line_parts.Length > 1) // has address
                {
                    str.Append(line_parts[1] + "\n");  // remove verse address
                }
                else if (line_parts.Length > 0)
                {
                    str.Append(line_parts[0] + "\n");  // leave it as it is
                }
            }
        }
        if (str.Length > 1)
        {
            str.Remove(str.Length - 1, 1);
        }
        return str.ToString();
    }
    private string RemoveVerseEndMarks(string text)
    {
        // RTL script misplaces brackets
        return text; // do nothing for now

        //if (string.IsNullOrEmpty(text)) return null;
        //while (text.Contains(Verse.OPEN_BRACKET) || text.Contains(Verse.CLOSE_BRACKET))
        //{
        //    int start = text.IndexOf(Verse.OPEN_BRACKET);
        //    int end = text.IndexOf(Verse.CLOSE_BRACKET);
        //    if ((start >= 0) && (end >= 0))
        //    {
        //        if (start < end)
        //        {
        //            text = text.Remove(start, (end - start) + 1); // remove space after it
        //        }
        //        else // Arabic script misplaces brackets
        //        {
        //            text = text.Remove(end, (start - end) + 1); // remove space after it
        //        }
        //    }
        //}
        //return text;
    }

    // used for non-Quran text
    private void CalculateValueAndDisplayFactors(string user_text)
    {
        if (m_client != null)
        {
            long value = m_client.CalculateValue(user_text);
            FactorizeValue(value, false);
        }
    }
    // used for Quran text only
    private void CalculateValueAndDisplayFactors(Verse verse)
    {
        if (m_client != null)
        {
            long value = m_client.CalculateValue(verse);
            FactorizeValue(value, false);
        }
    }
    private void CalculateValueAndDisplayFactors(List<Verse> verses)
    {
        if (m_client != null)
        {
            long value = m_client.CalculateValue(verses);
            FactorizeValue(value, false);
        }
    }
    private void CalculateValueAndDisplayFactors(Chapter chapter)
    {
        if (m_client != null)
        {
            long value = m_client.CalculateValue(chapter);
            FactorizeValue(value, false);
        }
    }
    private void CalculateValueAndDisplayFactors(List<Verse> verses, int letter_index_in_verse1, int letter_index_in_verse2)
    {
        if (m_client != null)
        {
            long value = m_client.CalculateValue(verses, letter_index_in_verse1, letter_index_in_verse2);
            FactorizeValue(value, false);
        }
    }

    private void ValueTextBox_TextChanged(object sender, EventArgs e)
    {
        //int digits = Numbers.DigitCount(ValueTextBox.Text, m_radix);
        //ValueInspectLabel.Text = digits.ToString();
    }
    private void ValueTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (e.KeyCode == Keys.A)
            {
                if (sender is TextBoxBase)
                {
                    (sender as TextBoxBase).SelectAll();
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                IncrementValue();
            }
            else if (e.KeyCode == Keys.Down)
            {
                DecrementValue();
            }
        }
        else if (e.KeyCode == Keys.Enter)
        {
            CalculateExpression();
        }
        else
        {
            ValueTextBox.ForeColor = Color.DarkGray;
        }
    }
    private void IncrementValue()
    {
        long value = 0L;
        if (long.TryParse(ValueTextBox.Text, out value))
        {
            if (value < long.MaxValue) value++;
            ValueTextBox.Text = value.ToString();
            FactorizeValue(value, true);
        }
    }
    private void DecrementValue()
    {
        long value = 0L;
        if (long.TryParse(ValueTextBox.Text, out value))
        {
            if (value > 0) value--;
            ValueTextBox.Text = value.ToString();
            FactorizeValue(value, true);
        }
    }
    private void CalculateExpression()
    {
        if (m_client != null)
        {
            string expression = ValueTextBox.Text;

            long value = 0L;
            if (long.TryParse(expression, out value))
            {
                m_double_value = (double)value;
                FactorizeValue(value, true);
            }
            else if (expression.IsArabic() || ((m_radix <= 10) && expression.IsEnglish()))
            {
                m_double_value = m_client.CalculateValue(expression);
                value = (long)Math.Round(m_double_value);
                FactorizeValue(value, true); // user_text
            }
            else
            {
                m_double_value = DoCalculateExpression(expression, m_radix);
                value = (long)Math.Round(m_double_value);
                FactorizeValue(value, true);
            }
        }
    }
    private double m_double_value = 0.0D;
    private string CalculateExpression(string expression, long radix)
    {
        try
        {
            return Evaluator.Evaluate(expression, radix);
        }
        catch
        {
            return expression;
        }
    }
    private double DoCalculateExpression(string expression, long radix)
    {
        double result = 0D;
        if (m_client != null)
        {
            try
            {
                result = Radix.Decode(expression, radix);
                this.ToolTip.SetToolTip(this.ValueTextBox, result.ToString());
            }
            catch // if expression
            {
                string text = CalculateExpression(expression, radix);
                this.ToolTip.SetToolTip(this.ValueTextBox, text); // display the decimal expansion

                try
                {
                    result = double.Parse(text);
                }
                catch
                {
                    try
                    {
                        result = m_client.CalculateValue(expression);
                    }
                    catch // text
                    {
                        //result = m_client.CalculateValue(expression);
                    }
                }
            }
        }
        return result;
    }
    private void FactorizeValue(long value, bool overwrite)
    {
        try
        {
            // if there is a math expression, add to it, don't overwrite it
            if (!overwrite &&
                 (
                   (ValueTextBox.Text.EndsWith("+")) ||
                   (ValueTextBox.Text.EndsWith("-")) ||
                   (ValueTextBox.Text.EndsWith("*")) ||
                   (ValueTextBox.Text.EndsWith("/")) ||
                   (ValueTextBox.Text.EndsWith("^")) ||
                   (ValueTextBox.Text.EndsWith("%"))
                 )
               )
            {
                ValueTextBox.Text += Radix.Encode(value, m_radix);

                ValueTextBox.SelectionLength = 0;
                ValueTextBox.SelectionStart = ValueTextBox.Text.Length;
                // focus so user can continue with +, -, *, /, ^, %, Enter
                ValueTextBox.Focus();
            }
            else
            {
                ValueTextBox.Text = Radix.Encode(value, m_radix);
                ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(value);
                ValueTextBox.SelectionStart = ValueTextBox.Text.Length;
                ValueTextBox.SelectionLength = 0;
                ValueTextBox.Refresh();
            }
        }
        catch //(Exception ex)
        {
            //MessageBox.Show(ex.Message, Application.ProductName);
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Value Display
    ///////////////////////////////////////////////////////////////////////////////
    private long m_radix = DEFAULT_RADIX;
    private long m_divisor = DEFAULT_DIVISOR;
    private void CalculateAndDisplayCounts(string user_text)
    {
        if (!String.IsNullOrEmpty(user_text))
        {
            if (m_client != null)
            {
                if (!user_text.IsArabic())  // eg English
                {
                    user_text = user_text.ToUpper();
                }

                // in all cases
                if (m_client.NumerologySystem != null)
                {
                    // simplify all text_modes
                    user_text = user_text.SimplifyTo(m_client.NumerologySystem.TextMode);
                    user_text = user_text.Replace("_", "");
                    user_text = user_text.Replace("\t", "");
                    while (user_text.Contains("  "))
                    {
                        user_text = user_text.Replace("  ", " ");
                    }
                    user_text = user_text.Replace("\r\n", "\n");

                    int chapter_count = 1;
                    int verse_count = 1;
                    int word_count = 1;
                    int letter_count = 0;
                    foreach (char c in user_text)
                    {
                        if (c == '\n')
                        {
                            verse_count++;
                            if (letter_count > 0)
                            {
                                word_count++;
                            }
                        }
                        else if (c == ' ')
                        {
                            word_count++;
                        }
                        else
                        {
                            letter_count++;
                        }
                    }
                    DisplayCounts(chapter_count, verse_count, word_count, letter_count, -1, -1, -1, -1); // -1 means don't change what is displayed
                }
            }
        }
        else
        {
            DisplayCounts(0, 0, 0, 0, -1, -1, -1, -1); // -1 means don't change what is displayed
        }
    }
    private void CalculateAndDisplayCounts(Verse verse)
    {
        if (verse != null)
        {
            CalculateAndDisplayCountsLocal(verse);
        }
    }
    private void CalculateAndDisplayCountsLocal(Verse verse)
    {
        if (verse != null)
        {
            int chapter_count = 1;
            int verse_count = 1;
            int word_count = verse.Words.Count;
            int letter_count = verse.LetterCount;
            int chapter_number_sum = verse.Chapter.SortedNumber;
            int verse_number_sum = verse.NumberInChapter;
            int word_number_sum = 0;
            int letter_number_sum = 0;

            if (verse.Words != null)
            {
                foreach (Word word in verse.Words)
                {
                    word_number_sum += word.NumberInVerse;
                    if ((word.Letters != null) && (word.Letters.Count > 0))
                    {
                        foreach (Letter letter in word.Letters)
                        {
                            letter_number_sum += letter.NumberInWord;
                        }
                    }
                }
            }
            DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);
        }
    }
    private void CalculateAndDisplayCountsTotal(Verse verse)
    {
        if (verse != null)
        {
            int chapter_count = 1;
            int verse_count = 0;
            int word_count = 0;
            int letter_count = 0;
            int chapter_number_sum = 0;
            int verse_number_sum = 0;
            int word_number_sum = 0;
            int letter_number_sum = 0;

            Chapter chapter = verse.Chapter;
            if (chapter != null)
            {
                chapter_number_sum += chapter.SortedNumber;
                verse_count += chapter.Verses.Count;

                foreach (Verse v in chapter.Verses)
                {
                    word_count += v.Words.Count;
                    letter_count += v.LetterCount;

                    verse_number_sum += v.NumberInChapter;
                    if (v.Words != null)
                    {
                        foreach (Word word in v.Words)
                        {
                            word_number_sum += word.NumberInVerse;
                            if ((word.Letters != null) && (word.Letters.Count > 0))
                            {
                                foreach (Letter letter in word.Letters)
                                {
                                    letter_number_sum += letter.NumberInWord;
                                }
                            }
                        }
                    }
                }
            }

            DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);
        }
    }
    private void CalculateAndDisplayCounts(List<Verse> verses)
    {
        if (verses != null)
        {
            CalculateAndDisplayCountsLocal(verses);
        }
    }
    private void CalculateAndDisplayCountsLocal(List<Verse> verses)
    {
        if (verses != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    List<Chapter> chapters = m_client.Book.GetChapters(verses);
                    if (chapters != null)
                    {
                        int chapter_count = chapters.Count;
                        int verse_count = verses.Count;
                        int word_count = 0;
                        int letter_count = 0;
                        int chapter_number_sum = 0;
                        int verse_number_sum = 0;
                        int word_number_sum = 0;
                        int letter_number_sum = 0;
                        foreach (Chapter chapter in chapters)
                        {
                            if (chapter != null)
                            {
                                chapter_number_sum += chapter.SortedNumber;
                            }
                        }

                        foreach (Verse verse in verses)
                        {
                            word_count += verse.Words.Count;
                            letter_count += verse.LetterCount;

                            verse_number_sum += verse.NumberInChapter;
                            if (verse.Words != null)
                            {
                                foreach (Word word in verse.Words)
                                {
                                    word_number_sum += word.NumberInVerse;
                                    if ((word.Letters != null) && (word.Letters.Count > 0))
                                    {
                                        foreach (Letter letter in word.Letters)
                                        {
                                            letter_number_sum += letter.NumberInWord;
                                        }
                                    }
                                }
                            }
                        }
                        DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);
                    }
                }
            }
        }
    }
    private void CalculateAndDisplayCountsTotal(List<Verse> verses)
    {
        if (verses != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    List<Chapter> chapters = m_client.Book.GetChapters(verses);
                    if (chapters != null)
                    {
                        int chapter_count = chapters.Count;
                        int verse_count = 0;
                        int word_count = 0;
                        int letter_count = 0;
                        int chapter_number_sum = 0;
                        int verse_number_sum = 0;
                        int word_number_sum = 0;
                        int letter_number_sum = 0;
                        foreach (Chapter chapter in chapters)
                        {
                            if (chapter != null)
                            {
                                chapter_number_sum += chapter.SortedNumber;
                                verse_count += chapter.Verses.Count;

                                foreach (Verse verse in chapter.Verses)
                                {
                                    word_count += verse.Words.Count;
                                    letter_count += verse.LetterCount;

                                    verse_number_sum += verse.NumberInChapter;
                                    if (verse.Words != null)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            word_number_sum += word.NumberInVerse;
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                foreach (Letter letter in word.Letters)
                                                {
                                                    letter_number_sum += letter.NumberInWord;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);
                    }
                }
            }
        }
    }
    private void CalculateAndDisplayCounts(List<Verse> verses, int letter_index_in_verse1, int letter_index_in_verse2)
    {
        CalculateAndDisplayCounts(verses, letter_index_in_verse1, letter_index_in_verse2, 0, 0);
    }
    private void CalculateAndDisplayCounts(List<Verse> verses, int letter_index_in_verse1, int letter_index_in_verse2, int stopmarks, int quranmarks)
    {
        if (verses != null)
        {
            if (m_client != null)
            {
                if (m_client.Book != null)
                {
                    List<Chapter> chapters = m_client.Book.GetChapters(verses);
                    if (chapters != null)
                    {
                        int chapter_count = chapters.Count;
                        int verse_count = verses.Count;
                        int word_count = 0;
                        int letter_count = stopmarks + quranmarks;
                        int chapter_number_sum = 0;
                        int verse_number_sum = 0;
                        int word_number_sum = 0;
                        int letter_number_sum = 0;
                        foreach (Chapter chapter in chapters)
                        {
                            if (chapter != null)
                            {
                                chapter_number_sum += chapter.SortedNumber;
                            }
                        }

                        ///////////////////////////
                        // Middle Verse Part (verse1, letter_index_in_verse1, letter_index_in_verse2);
                        ///////////////////////////
                        if (verses.Count == 1)
                        {
                            Verse verse1 = verses[0];
                            if (verse1 != null)
                            {
                                verse_number_sum += verse1.NumberInChapter;

                                if (verse1.Words != null)
                                {
                                    foreach (Word word in verse1.Words)
                                    {
                                        if (word != null)
                                        {
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                if ((word.Letters[word.Letters.Count - 1].NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                if ((word.Letters[0].NumberInVerse - 1) > letter_index_in_verse2) break;
                                                word_count++;
                                                word_number_sum += word.NumberInVerse;

                                                foreach (Letter letter in word.Letters)
                                                {
                                                    if (letter != null)
                                                    {
                                                        if ((letter.NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                        if ((letter.NumberInVerse - 1) > letter_index_in_verse2) break;
                                                        letter_count++;
                                                        letter_number_sum += letter.NumberInWord;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (verses.Count == 2)
                        {
                            ///////////////////////////
                            // End Verse Part (verse1, letter_index_in_verse1);
                            ///////////////////////////
                            Verse verse1 = verses[0];
                            if (verse1 != null)
                            {
                                verse_number_sum += verse1.NumberInChapter;

                                if (verse1.Words != null)
                                {
                                    foreach (Word word in verse1.Words)
                                    {
                                        if (word != null)
                                        {
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                if ((word.Letters[word.Letters.Count - 1].NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                word_count++;
                                                word_number_sum += word.NumberInVerse;

                                                foreach (Letter letter in word.Letters)
                                                {
                                                    if (letter != null)
                                                    {
                                                        if ((letter.NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                        letter_count++;
                                                        letter_number_sum += letter.NumberInWord;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            ///////////////////////////
                            // Beginning Verse Part (verse2, letter_index_in_verse2);
                            ///////////////////////////
                            Verse verse2 = verses[1];
                            if (verse2 != null)
                            {
                                verse_number_sum += verse2.NumberInChapter;

                                if (verse2.Words != null)
                                {
                                    foreach (Word word in verse2.Words)
                                    {
                                        if (word != null)
                                        {
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                if ((word.Letters[0].NumberInVerse - 1) > letter_index_in_verse2) break;
                                                word_count++;
                                                word_number_sum += word.NumberInVerse;

                                                foreach (Letter letter in word.Letters)
                                                {
                                                    if (letter != null)
                                                    {
                                                        if ((letter.NumberInVerse - 1) > letter_index_in_verse2) break;
                                                        letter_count++;
                                                        letter_number_sum += letter.NumberInWord;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (verses.Count > 2)
                        {
                            ///////////////////////////
                            // End Verse Part (verse1, letter_index_in_verse1);
                            ///////////////////////////
                            Verse verse1 = verses[0];
                            if (verse1 != null)
                            {
                                verse_number_sum += verse1.NumberInChapter;

                                if (verse1.Words != null)
                                {
                                    foreach (Word word in verse1.Words)
                                    {
                                        if (word != null)
                                        {
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                if ((word.Letters[word.Letters.Count - 1].NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                word_count++;
                                                word_number_sum += word.NumberInVerse;

                                                foreach (Letter letter in word.Letters)
                                                {
                                                    if (letter != null)
                                                    {
                                                        if ((letter.NumberInVerse - 1) < letter_index_in_verse1) continue;
                                                        letter_count++;
                                                        letter_number_sum += letter.NumberInWord;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            ///////////////////////////
                            // Middle Verses
                            ///////////////////////////
                            for (int i = 1; i < verses.Count - 1; i++)
                            {
                                Verse verse = verses[i];
                                if (verse != null)
                                {
                                    verse_number_sum += verse.NumberInChapter;

                                    if (verse.Words != null)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            word_count++;
                                            word_number_sum += word.NumberInVerse;

                                            if (word != null)
                                            {
                                                if ((word.Letters != null) && (word.Letters.Count > 0))
                                                {
                                                    foreach (Letter letter in word.Letters)
                                                    {
                                                        if (letter != null)
                                                        {
                                                            letter_count++;
                                                            letter_number_sum += letter.NumberInWord;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            ///////////////////////////
                            // Beginning Verse Part (verse2, letter_index_in_verse2);
                            ///////////////////////////
                            Verse verse2 = verses[verses.Count - 1];
                            if (verse2 != null)
                            {
                                verse_number_sum += verse2.NumberInChapter;

                                if (verse2.Words != null)
                                {
                                    foreach (Word word in verse2.Words)
                                    {
                                        if (word != null)
                                        {
                                            if ((word.Letters != null) && (word.Letters.Count > 0))
                                            {
                                                if ((word.Letters[0].NumberInVerse - 1) > letter_index_in_verse2) break;
                                                word_count++;
                                                word_number_sum += word.NumberInVerse;

                                                foreach (Letter letter in word.Letters)
                                                {
                                                    if (letter != null)
                                                    {
                                                        if ((letter.NumberInVerse - 1) > letter_index_in_verse2) break;
                                                        letter_count++;
                                                        letter_number_sum += letter.NumberInWord;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else // verses.Count == 0
                        {
                            // do nothing
                        }
                        DisplayCounts(chapter_count, verse_count, word_count, letter_count, chapter_number_sum, verse_number_sum, word_number_sum, letter_number_sum);
                    }
                }
            }
        }
    }
    private void DisplayCounts(int chapter_count, int verse_count, int word_count, int letter_count, int chapter_number_sum, int verse_number_sum, int word_number_sum, int letter_number_sum)
    {
        DisplayCounts(chapter_count, verse_count, word_count, letter_count);
    }
    private void DisplayCounts(int chapter_count, int verse_count, int word_count, int letter_count)
    {
        ChaptersTextBox.Text = Radix.Encode(chapter_count, m_radix);
        ChaptersTextBox.ForeColor = Numbers.GetNumberTypeColor(ChaptersTextBox.Text, m_radix);
        ChaptersTextBox.BackColor = (Numbers.Compare(chapter_count, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.ControlLight;
        ChaptersTextBox.Refresh();

        VersesTextBox.Text = Radix.Encode(verse_count, m_radix);
        VersesTextBox.ForeColor = Numbers.GetNumberTypeColor(VersesTextBox.Text, m_radix);
        VersesTextBox.BackColor = (Numbers.Compare(verse_count, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.ControlLight;
        VersesTextBox.Refresh();

        WordsTextBox.Text = Radix.Encode(word_count, m_radix);
        WordsTextBox.ForeColor = Numbers.GetNumberTypeColor(WordsTextBox.Text, m_radix);
        WordsTextBox.BackColor = (Numbers.Compare(word_count, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.ControlLight;
        WordsTextBox.Refresh();

        LettersTextBox.Text = Radix.Encode(letter_count, m_radix);
        LettersTextBox.ForeColor = Numbers.GetNumberTypeColor(LettersTextBox.Text, m_radix);
        LettersTextBox.BackColor = (Numbers.Compare(letter_count, m_divisor, ComparisonOperator.DivisibleBy, 0)) ? DIVISOR_COLOR : SystemColors.ControlLight;
        LettersTextBox.Refresh();
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Statistics
    ///////////////////////////////////////////////////////////////////////////////
    private List<char> m_selected_letters = new List<char>();
    private void LetterFrequencyListView_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        if (sender is ListView)
        {
            ListView listview = sender as ListView;
            try
            {
                if (m_client != null)
                {
                    m_client.SortLetterStatistics((StatisticSortMethod)e.Column);
                    DisplayLetterFrequencies();

                    // choose sort marker
                    string sort_marker = (LetterStatistic.SortOrder == StatisticSortOrder.Ascending) ? "▼" : "▲";

                    // remove all sort markers
                    foreach (ColumnHeader column in listview.Columns)
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

                    // display sort marker
                    listview.Columns[e.Column].Text = listview.Columns[e.Column].Text.Replace("  ", " " + sort_marker);
                }
            }
            catch
            {
                // log exception
            }
        }
    }
    private void LetterFrequencyListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        // only update m_selected_letters if user manually select items
        // otherwise we would lose items in they don't appear in a selection
        // and would give wrong results for subsequent selections with these items 
        if (LetterFrequencyListView.Focused)
        {
            m_selected_letters.Clear();
            if (LetterFrequencyListView.SelectedIndices.Count > 0)
            {
                foreach (ListViewItem item in LetterFrequencyListView.SelectedItems)
                {
                    m_selected_letters.Add(item.SubItems[1].Text[0]);
                }
            }
        }

        DisplayLetterFrequenciesTotals();
    }
    private void LetterFrequencyListView_DoubleClick(object sender, EventArgs e)
    {
        char character = '\0';
        Dictionary<char, List<int>> letter_positions = new Dictionary<char, List<int>>();
        Dictionary<char, List<int>> letter_distances = new Dictionary<char, List<int>>();
        foreach (ListViewItem item in LetterFrequencyListView.SelectedItems)
        {
            character = item.SubItems[1].Text[0];
            letter_positions.Add(character, new List<int>());
            letter_distances.Add(character, new List<int>());
        }

        if (!String.IsNullOrEmpty(m_current_text))
        {
            string text = m_current_text.SimplifyTo(m_client.NumerologySystem.TextMode);
            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            text = text.Replace("\t", "");
            text = text.Replace("_", "");
            text = text.Replace(" ", "");
            text = text.Replace(Constants.OPEN_BRACKET, "");
            text = text.Replace(Constants.CLOSE_BRACKET, "");
            foreach (char c in Constants.INDIAN_DIGITS)
            {
                text = text.Replace(c.ToString(), "");
            }

            foreach (char c in letter_positions.Keys)
            {
                int pos = text.IndexOf(c);
                if (pos > -1)
                {
                    letter_positions[c].Add(pos + 1);
                    for (int i = pos + 1; i < text.Length; i++)
                    {
                        if (text[i] == c)
                        {
                            letter_positions[c].Add(i + 1);

                            int letter_distance = i - pos;
                            if (letter_distances.ContainsKey(c))
                            {
                                letter_distances[c].Add(letter_distance);
                            }
                            pos = i;
                        }
                    }
                }
            }

            StringBuilder str = new StringBuilder();
            foreach (char c in letter_positions.Keys)
            {
                str.Append(c.ToString() + " positions" + "\t");
                foreach (int ld in letter_positions[c])
                {
                    str.Append(ld.ToString() + ",");
                }
                if (str.Length > 0)
                {
                    str.Remove(str.Length - 1, 1);
                }
                str.AppendLine();
            }

            str.AppendLine();
            foreach (char c in letter_distances.Keys)
            {
                str.Append(c.ToString() + " distances" + "\t");
                foreach (int ld in letter_distances[c])
                {
                    str.Append(ld.ToString() + ",");
                }
                if (str.Length > 0)
                {
                    str.Remove(str.Length - 1, 1);
                }
                str.AppendLine();
            }

            string filename = character.ToString() + "_" + "LetterPositionsAndDistances" + Globals.OUTPUT_FILE_EXT;
            if (Directory.Exists(Globals.STATISTICS_FOLDER))
            {
                string path = Globals.STATISTICS_FOLDER + "/" + filename;
                FileHelper.SaveText(path, str.ToString());
                FileHelper.DisplayFile(path);
            }
        }

    }
    private void BuildLetterFrequencies()
    {
        if (m_client != null)
        {
            if (!String.IsNullOrEmpty(m_current_text))
            {
                m_client.BuildLetterStatistics(m_current_text, m_with_diacritics);
            }
            else
            {
                if (m_client.LetterStatistics != null)
                {
                    m_client.LetterStatistics.Clear();
                }
            }
        }
    }
    private void DisplayLetterFrequencies()
    {
        if (m_client != null)
        {
            if (m_client.LetterStatistics != null)
            {
                LetterFrequencyListView.Items.Clear();
                if (m_client.LetterStatistics.Count > 0)
                {
                    List<int> selected_indexes = new List<int>();
                    for (int i = 0; i < m_client.LetterStatistics.Count; i++)
                    {
                        string[] item_parts = new string[3];
                        item_parts[0] = m_client.LetterStatistics[i].Order.ToString();
                        item_parts[1] = m_client.LetterStatistics[i].Letter.ToString();
                        item_parts[2] = m_client.LetterStatistics[i].Frequency.ToString();
                        LetterFrequencyListView.Items.Add(new ListViewItem(item_parts, i));

                        // re-select user items if any were selected for previous selection
                        if (m_selected_letters.Contains(m_client.LetterStatistics[i].Letter))
                        {
                            selected_indexes.Add(i);
                        }
                    }
                    // must be done after adding all items
                    foreach (int index in selected_indexes)
                    {
                        LetterFrequencyListView.SelectedIndices.Add(index);
                        LetterFrequencyListView.EnsureVisible(index);
                    }
                }

                DisplayLetterFrequenciesTotals();

                // reset sort-markers
                foreach (ColumnHeader column in LetterFrequencyListView.Columns)
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
                LetterFrequencyListView.Columns[0].Text = LetterFrequencyListView.Columns[0].Text.Replace("  ", " ▲");
                LetterFrequencyListView.Refresh();
            }
        }
    }
    private void DisplayLetterFrequenciesTotals()
    {
        try
        {
            int count = 0;
            long sum = 0L;
            if (LetterFrequencyListView.SelectedIndices.Count > 0)
            {
                count = LetterFrequencyListView.SelectedIndices.Count;
                foreach (ListViewItem item in LetterFrequencyListView.SelectedItems)
                {
                    sum += long.Parse(item.SubItems[2].Text);
                }
            }
            else
            {
                count = LetterFrequencyListView.Items.Count;
                foreach (ListViewItem item in LetterFrequencyListView.Items)
                {
                    sum += long.Parse(item.SubItems[2].Text);
                }
            }

            LetterFrequencyCountLabel.Text = count.ToString();
            LetterFrequencyCountLabel.ForeColor = Numbers.GetNumberTypeColor(count);
            LetterFrequencyCountLabel.Refresh();

            LetterFrequencySumLabel.Text = sum.ToString();
            LetterFrequencySumLabel.ForeColor = Numbers.GetNumberTypeColor(sum);
            LetterFrequencySumLabel.Refresh();
        }
        catch
        {
            // log exception
        }
    }

    private void LetterFrequencyInspectLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            InspectLetterStatistics();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void LetterFrequencyWithDiacriticsCheckBox_Click(object sender, EventArgs e)
    {
        if (m_client != null)
        {
            if (m_client.Book != null)
            {
                m_with_diacritics = LetterFrequencyWithDiacriticsCheckBox.Checked;
                m_client.Book.WithDiacritics = m_with_diacritics;

                //PopulateWordsListBox();

                BuildLetterFrequencies();
                DisplayLetterFrequencies();
            }
        }
    }
    private void InspectLetterStatistics()
    {
        if (m_client != null)
        {
            string text = m_current_text;
            if (!String.IsNullOrEmpty(text))
            {
                if (!String.IsNullOrEmpty(m_current_phrase))
                {
                    string filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".txt";
                    if (m_client.NumerologySystem != null)
                    {
                        filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" + m_client.NumerologySystem.Name + ".txt";
                    }

                    m_client.SaveLetterStatistics(filename, text, m_current_phrase);
                }
                else
                {
                    string filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".txt";
                    if (m_client.NumerologySystem != null)
                    {
                        filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" + m_client.NumerologySystem.Name + ".txt";
                    }

                    m_client.SaveLetterStatistics(filename, text);
                }
            }
        }
    }
    private void ValueInspectLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            InspectValueCalculations();
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void InspectValueCalculations()
    {
        if (m_client != null)
        {
            if (m_client.NumerologySystem != null)
            {
                string filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" + m_client.NumerologySystem.Name + ".txt";

                long value = 0;
                StringBuilder str = new StringBuilder();
                if (long.TryParse(ValueTextBox.Text, out value))
                {
                    str.AppendLine(m_current_text);
                    str.AppendLine("----------------------------------------");
                    str.AppendLine();
                    str.AppendLine("Verses\t\t=\t" + VersesTextBox.Text);
                    str.AppendLine("Words\t\t=\t" + WordsTextBox.Text);
                    str.AppendLine("Letters\t\t=\t" + LettersTextBox.Text);
                    str.AppendLine("Value\t\t=\t" + ValueTextBox.Text + ((m_radix == DEFAULT_RADIX) ? "" : " in base " + m_radix.ToString()));
                    str.AppendLine();
                    m_client.SaveValueCalculations(filename, str.ToString());
                }
            }
        }
    }

    private void StatisticsControls_Enter(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);
        this.AcceptButton = null;
    }
    private void TextBoxLabelControls_CtrlClick(object sender, EventArgs e)
    {
        // Ctrl+Click factorizes number
        if (ModifierKeys == Keys.Control)
        {
            if (sender is Label)
            {
                FactorizeNumber(sender as Label);
            }
            else if (sender is TextBox)
            {
                FactorizeNumber(sender as TextBox);
            }
        }
    }
    private void FactorizeNumber(Label control)
    {
        if (control != null)
        {
            long value = 0L;
            string[] parts = control.Text.Split(' ');
            for (int i = 0; i < parts.Length; i++)
            {
                if ((!m_found_verses_displayed) && (control == HeaderLabel))
                {
                    if (parts[i].Contains("/"))
                    {
                        int pos = parts[i].IndexOf("/");
                        string verse_number_str = parts[i].Remove(pos);
                        if (long.TryParse(verse_number_str, out value))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (long.TryParse(parts[i], out value))
                    {
                        break;
                    }
                    else
                    {
                        double d = 0D;
                        if (double.TryParse(parts[i + 1], out d))
                        {
                            value = (long)d;
                            break;
                        }
                    }
                }
            }
            FactorizeValue(value, false);
        }
    }
    private void FactorizeNumber(TextBox control)
    {
        if (control != null)
        {
            if (control != ValueTextBox)
            {
                long value = 0L;
                try
                {
                    string text = control.Text;
                    if (!String.IsNullOrEmpty(text))
                    {
                        if (control.Name.StartsWith("LetterFrequency"))
                        {
                            value = Math.Abs((long)double.Parse(text));
                        }
                        else if (control.Name.StartsWith("Decimal"))
                        {
                            value = Radix.Decode(text, 10L);
                        }
                        else if (text.StartsWith(SUM_SYMBOL))
                        {
                            text = text.Substring(SUM_SYMBOL.Length, text.Length - SUM_SYMBOL.Length);
                            value = Radix.Decode(text, 10L);
                        }
                        else if (text.StartsWith("4×")) // 4n+1
                        {
                            int start = "4×".Length;
                            int end = text.IndexOf("+");
                            text = text.Substring(start, end - start);
                            value = Radix.Decode(text, 10L);
                        }
                        else
                        {
                            value = Radix.Decode(text, m_radix);
                        }
                    }

                    FactorizeValue(value, false);
                }
                catch
                {
                    //value = -1L; // error
                }
            }
        }
    }
    private void StatusControls_Enter(object sender, EventArgs e)
    {
        SearchGroupBox_Leave(null, null);
        this.AcceptButton = null;
    }
    private void Control_MouseHover(object sender, EventArgs e)
    {
        Control control = sender as Control;
        if (control != null)
        {
            try
            {
                string text = control.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    if (text.StartsWith(SUM_SYMBOL))
                    {
                        text = text.Substring(1);
                    }
                    long number = (long)double.Parse(text);
                    string factors_str = Numbers.FactorizeToString(number);
                    ToolTip.SetToolTip(control, factors_str);
                }
            }
            catch
            {
                ToolTip.SetToolTip(control, null);
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion
    #region Help
    ///////////////////////////////////////////////////////////////////////////////
    private void LinkLabel_Click(object sender, EventArgs e)
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
    /////////////////////////////////////////////////////////////////////////////
    #endregion
}
