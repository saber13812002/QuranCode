using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace InitialLetters
{
    public partial class MainForm : Form
    {
        List<string> sentences;
        DateTime m_start_time;
        public MainForm()
        {
            InitializeComponent();

            m_filename = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", ".ini");
            LoadSettings();
        }

        private string m_filename = null;
        private string m_letters = null;
        private void LoadSettings()
        {
            if (File.Exists(m_filename))
            {
                using (StreamReader reader = File.OpenText(m_filename))
                {
                    try
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] parts = line.Split('=');
                            if (parts.Length == 2)
                            {
                                switch (parts[0])
                                {
                                    case "Top":
                                        {
                                            this.Top = int.Parse(parts[1]);
                                        }
                                        break;
                                    case "Left":
                                        {
                                            this.Left = int.Parse(parts[1]);
                                        }
                                        break;
                                    case "Width":
                                        {
                                            this.Width = int.Parse(parts[1]);
                                        }
                                        break;
                                    case "Height":
                                        {
                                            this.Height = int.Parse(parts[1]);
                                        }
                                        break;
                                    case "Letters":
                                        {
                                            this.m_letters = parts[1];
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        this.Top = 0;
                        this.Left = 0;
                    }
                }
            }
            else // first start
            {
                RestoreLocation();
            }
        }
        private void SaveSettings()
        {
            try
            {
                using (StreamWriter writer = File.CreateText(m_filename))
                {
                    writer.WriteLine("[Window]");
                    writer.WriteLine("Top=" + this.Top);
                    writer.WriteLine("Left=" + this.Left);
                    writer.WriteLine("Width=" + this.Width);
                    writer.WriteLine("Height=" + this.Height);
                    writer.WriteLine("[Text]");
                    writer.WriteLine("Letters=" + this.LettersTextBox.Text); // don't use this.m_letters

                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }
        }
        private void RestoreLocation()
        {
            this.Top = (Screen.PrimaryScreen.Bounds.Height / 2) - (this.Height / 2);
            this.Left = (Screen.PrimaryScreen.Bounds.Width / 2) - (this.Width / 2);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            if ((m_letters != null) && (m_letters.Length > 0))
            {
                LettersTextBox.Text = m_letters;
            }
            else
            {
                InitialsUniqueLettersToolStripMenuItem_Click(sender, e);
            }
            UniqueLettersToolStripMenuItem.Checked = false;
            UniqueWordsToolStripMenuItem.Checked = false;
            AllWordsToolStripMenuItem.Checked = false;
            ListView.Enabled = false;
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseApplication();
        }
        private void CloseApplication()
        {
            SaveSettings();
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                GenerateAnagrams();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan timespan = DateTime.Now - m_start_time;
            ElapsedTimeLabel.Text = new DateTime(timespan.Ticks).ToString("HH:mm:ss");//.fff");
        }

        private void ListView_Resize(object sender, EventArgs e)
        {
            // trial and error shows that we must make the column
            // header four pixels narrower than the containing
            // listview in order to avoid a scrollbar.
            if (ListView.Columns.Count > 0)
            {
                ListView.Columns[0].Width = ListView.Width - 23;

                // if the listview is big enough to show all the items, then make sure
                // the first item is at the top.  This works around behavior (which I assume is 
                // a bug in C# or .NET or something) whereby 
                // some blank lines appear before the first item

                if (ListView.Items.Count > 0
                    &&
                    ListView.TopItem != null
                    &&
                    ListView.TopItem.Index == 0)
                    ListView.EnsureVisible(0);
            }
        }
        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            Clipboard.Clear();

            string selected_text = "";
            ListView me = (ListView)sender;
            foreach (ListViewItem it in me.SelectedItems)
            {
                if (selected_text.Length > 0)
                    selected_text += Environment.NewLine;
                selected_text += it.Text;
            }
            // Under some circumstances -- probably a bug in my code somewhere --
            // we can get blank lines in the listview.  And if you click on one, since it
            // has no text, selected_text will be blank; _and_, apparantly, calling
            // Clipboard.set_text with an empty string provokes an aResultLabeless violation ...
            // so avoid that AV.
            if (selected_text.Length > 0)
                Clipboard.SetText(selected_text);
        }
        private void ListView_SortColumnClick(object sender, ColumnClickEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (ListView.Sorting == SortOrder.Ascending)
                {
                    ListView.Columns[0].Text = "Decending order";
                    ListView.Sorting = SortOrder.Descending;
                }
                else
                {
                    ListView.Columns[0].Text = "Ascending order";
                    ListView.Sorting = SortOrder.Ascending;
                }

                SaveResults();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void LettersTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                GenerateAnagrams();

            // Control-A
            if (e.KeyChar == (char)1)
                LettersTextBox.SelectAll();

            m_letters = LettersTextBox.Text + e.KeyChar.ToString();
        }

        private void InitialsUniqueLettersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LettersTextBox.Text = "ق ص ر ع س ن م ل ك ي ط ح ه ا";
            UniqueLettersToolStripMenuItem.Checked = true;
            UniqueWordsToolStripMenuItem.Checked = false;
            AllWordsToolStripMenuItem.Checked = false;
        }
        private void InitialsUniqueWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LettersTextBox.Text = "الم المص الر المر كهيعص طه طسم طس يس ص حم عسق ق ن";
            UniqueLettersToolStripMenuItem.Checked = false;
            UniqueWordsToolStripMenuItem.Checked = true;
            AllWordsToolStripMenuItem.Checked = false;
        }
        private void InitialsAllWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LettersTextBox.Text = "الم الم المص الر الر الر المر الر الر كهيعص طه طسم طس طسم الم الم الم الم يس ص حم حم عسق حم حم حم حم حم ق ن";
            UniqueLettersToolStripMenuItem.Checked = false;
            UniqueWordsToolStripMenuItem.Checked = false;
            AllWordsToolStripMenuItem.Checked = true;
        }
        private void RunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateAnagrams();
        }
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = "";
            if (ListView.Sorting == SortOrder.None)
            {
                filename = LettersTextBox.Text + ".txt";
            }
            else if (ListView.Sorting == SortOrder.Ascending)
            {
                filename = "Asc_" + LettersTextBox.Text + ".txt";
            }
            else if (ListView.Sorting == SortOrder.Descending)
            {
                filename = "Desc_" + LettersTextBox.Text + ".txt";
            }

            // show file content after save
            if (File.Exists(filename))
            {
                try
                {
                    System.Diagnostics.Process.Start(filename);
                }
                catch
                {
                    System.Diagnostics.Process.Start(filename.Replace(@"/", @"\"));
                }
            }
        }
        private void InitialLettersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = "InitialLetters.html";
            string path = Application.StartupPath + "/" + "Help" + "/" + filename;
            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(String.Format(
                "Initial Letters - v{0}", Application.ProductVersion + "\r\n"
                + "\r\n"
                + "©2007 Eric Hanchrow - Anagrams" + "\r\n"
                + "http://github.com/offby1/anagrams" + "\r\n"
                + "\r\n"
                + "©2012 Ali Adams - علي عبد الرزاق عبد الكريم القره غولي" + "\r\n"
                + "http://www.qurancode.com" + "\r\n"
                ),
                Application.ProductName);
        }
        private void GenerateAnagrams()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                LettersTextBox.Enabled = false;
                ListView.Items.Clear();
                ListView.Enabled = false;
                InitialsToolStripMenuItem.Enabled = false;
                RunToolStripMenuItem.Enabled = false;
                SaveToolStripMenuItem.Enabled = false;
                m_start_time = DateTime.Now;
                ElapsedTimeLabel.Text = "00:00:00";
                Timer.Enabled = true;
                ProgressBar.Minimum = 0;
                ProgressBar.Value = 0;

                string filename = "Data" + "/" + "dictionary.txt";
                if (File.Exists(filename))
                {
                    sentences = Anagrams.GenerateAnagrams(filename, LettersTextBox.Text);
                    if (sentences != null)
                    {
                        ProgressBar.Minimum = 0;
                        ProgressBar.Maximum = sentences.Count;
                        for (int i = 0; i < sentences.Count; i++)
                        {
                            ListView.Items.Add(sentences[i]);
                            ListView.EnsureVisible(ListView.Items.Count - 1);
                            ResultLabel.Text = ListView.Items.Count.ToString() + " / " + sentences.Count;
                            if (ListView.Items.Count % 1000 == 0)
                            {
                                Application.DoEvents();
                            }
                            ProgressBar.Value = i + 1;
                        }
                    }

                    ResultLabel.Text = String.Format("{0} sentences.", sentences.Count);
                    if (ListView.Items.Count > 0)
                    {
                        ListView.EnsureVisible(0);
                    }
                    LettersTextBox.Enabled = true;
                    LettersTextBox.Focus();
                    ListView.Enabled = true;
                    if (ListView.Columns.Count > 0)
                    {
                        ListView.Columns[0].Text = "Click to sort";
                    }
                    InitialsToolStripMenuItem.Enabled = true;
                    RunToolStripMenuItem.Enabled = true;
                    SaveToolStripMenuItem.Enabled = true;

                    SaveResults();

                    Timer.Enabled = false;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        private void SaveResults()
        {
            string filename = "";
            if (ListView.Sorting == SortOrder.None)
            {
                filename = LettersTextBox.Text + ".txt";
            }
            else if (ListView.Sorting == SortOrder.Ascending)
            {
                filename = "Asc_" + LettersTextBox.Text + ".txt";
            }
            else if (ListView.Sorting == SortOrder.Descending)
            {
                filename = "Desc_" + LettersTextBox.Text + ".txt";
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    writer.WriteLine("{0} sentences using '{1}'", ListView.Items.Count, LettersTextBox.Text);
                    writer.WriteLine("-----------------------------------");
                    foreach (ListViewItem it in ListView.Items)
                    {
                        writer.WriteLine(it.Text);
                    }
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }
        }

        private void LettersTextBox_TextChanged(object sender, EventArgs e)
        {
            string text = LettersTextBox.Text;
            text = text.Replace(" ", "");
            ResultLabel.Text = "Letters = " + text.Length;
        }
        private string m_backeup_text_with_duplicate_letters = null;
        private void DuplicateLettersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!DuplicateLettersCheckBox.Checked)
            {
                m_backeup_text_with_duplicate_letters = LettersTextBox.Text;
                LettersTextBox.Text = RemoveDuplicateLetters(m_backeup_text_with_duplicate_letters);
            }
            else
            {
                LettersTextBox.Text = m_backeup_text_with_duplicate_letters;
            }
        }

        public string RemoveDuplicates(string text)
        {
            if (String.IsNullOrEmpty(text)) return "";

            string result = "";
            foreach (char c in text)
            {
                if (!result.Contains(c.ToString()))
                {
                    result += c;
                }
            }

            return result;
        }
        private string RemoveDuplicateLetters(string text)
        {
            if (String.IsNullOrEmpty(text)) return "";

            string result = "";
            foreach (char c in text)
            {
                if (!result.Contains(c.ToString()))
                {
                    result += c + " ";
                }
            }
            result = result.Remove(result.Length - 1, 1);

            return result;
        }
    }
}
