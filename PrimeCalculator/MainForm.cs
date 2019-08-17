using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;

public partial class MainForm : Form
{
    private Factorizer m_factorizer = null;
    private Thread m_worker_thread = null;

    private void FixMicrosoft(object sender, KeyPressEventArgs e)
    {
        // stop annoying beep due to parent not having an AcceptButton
        if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)Keys.Escape))
        {
            e.Handled = true;
        }
        // enable Ctrl+A to SelectAll in TextBox and RichTextBox
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

    public const int DEFAULT_SLEEP_TIME = 40; // ms
    private int m_sleep_time = DEFAULT_SLEEP_TIME;
    public int SleepTime
    {
        get { return m_sleep_time; }
        set { m_sleep_time = value; }
    }

    private float m_dpi = 0F;
    public MainForm()
    {
        using (Graphics graphics = this.CreateGraphics())
        {
            m_dpi = graphics.DpiX;    // 100% = 96.0F,   125% = 120.0F,   150% = 144.0F
        }
        InitializeComponent();
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

        AboutToolStripMenuItem.Font = new Font(AboutToolStripMenuItem.Font, AboutToolStripMenuItem.Font.Style | FontStyle.Bold);

        m_filename = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", ".ini");
        LoadSettings();
    }

    private string m_filename = null;
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
                        if (!String.IsNullOrEmpty(line))
                        {
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
                                    case "Count":
                                        {
                                            string[] sub_parts = parts[1].Split('\t');
                                            if (sub_parts.Length == 2)
                                            {
                                                int count = int.Parse(sub_parts[0]);
                                                string[] sub_sub_parts = sub_parts[1].Split(',');
                                                if (sub_sub_parts.Length == count)
                                                {
                                                    foreach (string item in sub_sub_parts)
                                                    {
                                                        m_history_items.Add(item);
                                                        m_history_index++;
                                                    }
                                                    ValueTextBox.Text = m_history_items[m_history_index];
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    RestoreLocation();
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

                writer.WriteLine("[History]");
                StringBuilder str = new StringBuilder("Count=" + this.m_history_items.Count + "\t");
                foreach (string item in m_history_items)
                {
                    str.Append(item + ",");
                }
                str.Remove(str.Length - 1, 1);
                writer.WriteLine(str.ToString());
            }
        }
        catch
        {
            // silence IO error in case running from read-only media (CD/DVD)
        }
    }
    private void RestoreLocation()
    {
        this.Top = Screen.PrimaryScreen.WorkingArea.Top;
        this.Left = Screen.PrimaryScreen.WorkingArea.Left;
        this.Width = (m_dpi == 96.0F) ? 256 : 338;
        this.Height = (m_dpi == 96.0F) ? 416 : 507;
        MultithreadingCheckBox.Top = (m_dpi == 96.0F) ? 363 : 444;
        VersionLabel.Top = (m_dpi == 96.0F) ? 360 : 442;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        m_factorizer = null;
        m_worker_thread = null;

        string version = typeof(MainForm).Assembly.GetName().Version.ToString();
        int pos = version.LastIndexOf(".");
        if (pos > -1)
        {
            VersionLabel.Text = "v" + version.Substring(0, pos);
        }

        if (this.Top < 0)
        {
            RestoreLocation();
        }

        PLabel.ForeColor = Numbers.NUMBER_TYPE_COLORS[2];
        APLabel.ForeColor = Numbers.NUMBER_TYPE_COLORS[3];
        XPLabel.ForeColor = Numbers.NUMBER_TYPE_COLORS[4];
        CLabel.ForeColor = Numbers.NUMBER_TYPE_COLORS[5];
        ACLabel.ForeColor = Numbers.NUMBER_TYPE_COLORS[6];
        XCLabel.ForeColor = Numbers.NUMBER_TYPE_COLORS[7];
        //DFLabel.ForeColor = Numbers.NUMBER_KIND_COLORS[0];
        //ABLabel.ForeColor = Numbers.NUMBER_KIND_COLORS[2];
        DeficientNumbersLabel.BackColor = Numbers.NUMBER_KIND_COLORS[0];
        PerfectNumbersLabel.BackColor = Numbers.NUMBER_KIND_COLORS[1];
        AbundantNumbersLabel.BackColor = Numbers.NUMBER_KIND_COLORS[2];
        DFTextBox.BackColor = Numbers.NUMBER_KIND_COLORS[0];
        ABTextBox.BackColor = Numbers.NUMBER_KIND_COLORS[2];
    }
    private void MainForm_Shown(object sender, EventArgs e)
    {
        NotifyIcon.Visible = true;

        EnableEntryControls();
        SetupToolTips();

        ValueTextBox.Focus();
    }
    private void SetupToolTips()
    {
        this.ToolTip.SetToolTip(this.PCIndexChainL2RTextBox, "Prime-composite index chain --> P=0 C=1");
        this.ToolTip.SetToolTip(this.PCIndexChainR2LTextBox, "Prime-composite index chain <-- P=0 C=1");
        this.ToolTip.SetToolTip(this.CPIndexChainL2RTextBox, "Prime-composite index chain --> P=1 C=0");
        this.ToolTip.SetToolTip(this.CPIndexChainR2LTextBox, "Prime-composite index chain <-- P=1 C=0");
        this.ToolTip.SetToolTip(this.IndexChainLengthTextBox, "Prime-composite index chain length");
        this.ToolTip.SetToolTip(this.DigitSumTextBox, "Digit sum");
        this.ToolTip.SetToolTip(this.DigitalRootTextBox, "Digital root");
        this.ToolTip.SetToolTip(this.NthNumberTextBox, "Prime index");
        this.ToolTip.SetToolTip(this.NthAdditiveNumberTextBox, "Additive prime index");
        this.ToolTip.SetToolTip(this.NthNonAdditiveNumberTextBox, "Non-additive prime index");
    }
    private void MainForm_Resize(object sender, EventArgs e)
    {
        ValueTextBox.SelectionStart = 0;

        int left = 7;
        int width = (int)((this.Width - 25) * 0.33333);
        NthNumberTextBox.Left = left;
        NthNumberTextBox.Width = width;
        left += width - 1;
        NthAdditiveNumberTextBox.Left = left;
        NthAdditiveNumberTextBox.Width = width;
        left += width - 1;
        NthNonAdditiveNumberTextBox.Left = left;
        NthNonAdditiveNumberTextBox.Width = width;

        left = 7;
        int small_width = (int)((this.Width - 25) * 0.18333);
        int large_width = (int)((this.Width - 25) * 0.26666);
        NumberKindIndexTextBox.Left = left;
        NumberKindIndexTextBox.Width = small_width;
        left += small_width - 1;
        SumOfProperDivisorsTextBox.Left = left;
        SumOfProperDivisorsTextBox.Width = small_width;
        left += small_width - 1;
        SumOfNumbersTextBox.Left = left;
        SumOfNumbersTextBox.Width = large_width;
        left += large_width - 1;
        SumOfDigitSumsTextBox.Left = left;
        SumOfDigitSumsTextBox.Width = small_width;
        left += small_width - 1;
        SumOfDigitalRootsTextBox.Left = left;
        SumOfDigitalRootsTextBox.Width = small_width;

        left = 7;
        small_width = (int)((this.Width - 25) * 0.09787);
        large_width = (int)((this.Width - 25) * 0.22553);
        PCIndexChainL2RTextBox.Left = left;
        PCIndexChainL2RTextBox.Width = large_width;
        left += large_width - 1;
        PCIndexChainR2LTextBox.Left = left;
        PCIndexChainR2LTextBox.Width = large_width;
        left += large_width - 1;
        CPIndexChainL2RTextBox.Left = left;
        CPIndexChainL2RTextBox.Width = large_width;
        left += large_width - 1;
        CPIndexChainR2LTextBox.Left = left;
        CPIndexChainR2LTextBox.Width = large_width;
        left += large_width - 1;
        IndexChainLengthTextBox.Left = left;
        IndexChainLengthTextBox.Width = small_width;
    }
    private void MainForm_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            if ((m_worker_thread != null) && (m_worker_thread.IsAlive))
            {
                if (MessageBox.Show("Stop and lose all progress?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Cancel();
                }
            }
            else
            {
                ValueTextBox.Text = "";
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
        Cancel();
    }
    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        CloseApplication();
    }
    private void CloseApplication()
    {
        if (m_worker_thread != null)
        {
            m_worker_thread.Join(); // wait for workerThread to terminate
            m_worker_thread = null;
        }

        // remove icon from tray
        if (NotifyIcon != null)
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Dispose();
        }

        SaveSettings();
    }
    private void Control_MouseHover(object sender, EventArgs e)
    {
        Control control = sender as Control;
        if (control != null)
        {
            string text = control.Text;
            if (!String.IsNullOrEmpty(text))
            {
                double value;
                if (double.TryParse(text, out value))
                {
                    string factors_str = Numbers.FactorizeToString((long)value);
                    ToolTip.SetToolTip(control, factors_str);
                }
                else
                {
                    ToolTip.SetToolTip(control, null);
                }
            }
            else
            {
                ToolTip.SetToolTip(control, null);
            }
        }
    }

    private void ValueTextBox_TextChanged(object sender, EventArgs e)
    {
        ClearNumberAnalyses();

        int digits = Numbers.DigitCount(ValueTextBox.Text);
        int length = ValueTextBox.Text.Replace(" ", "").Length;
        if (digits == length)
        {
            PreviousPrimeLabel.Enabled = (digits > 0);
            NextPrimeLabel.Enabled = (digits > 0);
        }
        DigitsLabel.Text = (digits == 0) ? "digits" : digits.ToString();

        long value = 0L;
        long.TryParse(ValueTextBox.Text, out value);
        UpdateNumberKind(value);
        UpdateSumOfDivisors(value);
        UpdateDigitSumRootPCChains(value);

        //this.ToolTip.SetToolTip(this.ValueTextBox, null);
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
            else if (e.KeyCode == Keys.Z)
            {
                PreviousHistoryItem();
            }
            else if (e.KeyCode == Keys.Y)
            {
                NextHistoryItem();
            }
            else if (e.KeyCode == Keys.Up)
            {
                NextPrimeNumber();
            }
            else if (e.KeyCode == Keys.Down)
            {
                PreviousPrimeNumber();
            }
            else if (e.KeyCode == Keys.Delete)
            {
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
        else if (e.KeyCode == Keys.Enter)
        {
            CallRun();
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
            if (value < long.MaxValue)
            {
                value++;
                ValueTextBox.Text = value.ToString();
                ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(value);
                ValueTextBox.Refresh();
                //FactorizeValue(value);
            }
        }
    }
    private void DecrementValue()
    {
        long value = 0L;
        if (long.TryParse(ValueTextBox.Text, out value))
        {
            if (value > 0L)
            {
                value--;
                ValueTextBox.Text = value.ToString();
                ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(value);
                ValueTextBox.Refresh();
                //FactorizeValue(value);
            }
        }
    }

    private void NextPrimeNumber()
    {
        // guard against multiple runs on multiple ENTER keys
        if ((m_worker_thread != null) && (m_worker_thread.IsAlive)) return;

        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (ValueTextBox.Text.Length == 0) ValueTextBox.Text = "1";
            string value = ValueTextBox.Text;

            BeforeProcessing();

            try
            {
                m_factorizer = new Factorizer(this, "nextprime", value, m_multithreading);
                if (m_factorizer != null)
                {
                    m_worker_thread = new Thread(new ThreadStart(m_factorizer.Run));
                    m_worker_thread.Priority = ThreadPriority.Highest;
                    m_worker_thread.IsBackground = false;
                    m_worker_thread.Start();
                }
            }
            catch
            {
                Cancel();
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void PreviousPrimeNumber()
    {
        // guard against multiple runs on multiple ENTER keys
        if ((m_worker_thread != null) && (m_worker_thread.IsAlive)) return;

        this.Cursor = Cursors.WaitCursor;
        try
        {
            string value = ValueTextBox.Text + ",0"; // previousprime = nextprime(n,0), thanks to Benjamin Buhrow (bbuhrow@gmail.com) for telling me that.

            BeforeProcessing();

            try
            {
                m_factorizer = new Factorizer(this, "nextprime", value, m_multithreading);
                if (m_factorizer != null)
                {
                    m_worker_thread = new Thread(new ThreadStart(m_factorizer.Run));
                    m_worker_thread.Priority = ThreadPriority.Highest;
                    m_worker_thread.IsBackground = false;
                    m_worker_thread.Start();
                }
            }
            catch
            {
                Cancel();
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void NextPrimeLabel_Click(object sender, EventArgs e)
    {
        NextPrimeNumber();
    }
    private void PreviousPrimeLabel_Click(object sender, EventArgs e)
    {
        PreviousPrimeNumber();
    }

    private List<string> m_history_items = new List<string>();
    private int m_history_index = -1;
    private void NextHistoryItem()
    {
        if ((m_history_index >= 0) && (m_history_index < m_history_items.Count - 1))
        {
            m_history_index++;
            ValueTextBox.Text = m_history_items[m_history_index];
            CallRun();
        }
        ValueTextBox.Focus();
    }
    private void PreviousHistoryItem()
    {
        if ((m_history_index > 0) && (m_history_index < m_history_items.Count))
        {
            m_history_index--;
            ValueTextBox.Text = m_history_items[m_history_index];
            CallRun();
        }
        ValueTextBox.Focus();
    }

    private double m_double_value = 0.0D;
    private double CalculateValue(string expression)
    {
        double result = 0D;
        try
        {
            result = Radix.Decode(expression, Numbers.DEFAULT_RADIX);
            //this.ToolTip.SetToolTip(this.ValueTextBox, result.ToString());
        }
        catch // if expression
        {
            string text = CalculateExpression(expression);
            //this.ToolTip.SetToolTip(this.ValueTextBox, text); // display the decimal expansion

            try
            {
                result = double.Parse(text);
            }
            catch
            {
                result = 0L;
            }
        }
        return result;
    }
    private string CalculateExpression(string expression)
    {
        try
        {
            return Evaluator.Evaluate(expression, Numbers.DEFAULT_RADIX);
        }
        catch
        {
            return expression;
        }
    }
    private void FactorizeValue(long value)
    {
        if (value < 0L) value *= -1L;

        ValueTextBox.Text = Radix.Encode(value, Numbers.DEFAULT_RADIX);
        ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(value);
        ValueTextBox.Refresh();

        CallRun();
    }
    private void AnalyzeValue(long value)
    {
        if (value < 0L) value *= -1L;

        this.Cursor = Cursors.WaitCursor;
        try
        {
            ValueTextBox.SelectionStart = ValueTextBox.Text.Length;
            ValueTextBox.SelectionLength = 0;
            ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(value);
            ValueTextBox.Refresh();

            int plus1_index = -1;
            int minus1_index = -1;
            string plus1_str = "";
            string plus2_str = "";
            string minus1_str = "";
            string minus2_str = "";
            if (Numbers.IsUnit(value) || Numbers.IsPrime(value))
            {
                plus1_index = Numbers.Prime4nPlus1IndexOf(value) + 1;
                plus1_str = Numbers.Get4nPlus1EqualsSumOfTwoSquares(value);
                plus2_str = Numbers.Get4nPlus1EqualsDiffOfTwoTrivialSquares(value);

                minus1_index = Numbers.Prime4nMinus1IndexOf(value) + 1;
                minus1_str = Numbers.Get4nMinus1EqualsSumOfTwoSquares(value);
                minus2_str = Numbers.Get4nMinus1EqualsSumOfTwoSquares(value);
            }
            else //if composite
            {
                plus1_index = Numbers.Composite4nPlus1IndexOf(value) + 1;
                plus1_str = Numbers.Get4nPlus1EqualsDiffOfTwoSquares(value);

                plus2_str = Numbers.Get4nPlus1EqualsDiffOfTwoTrivialSquares(value);

                minus1_index = Numbers.Composite4nMinus1IndexOf(value) + 1;
                minus1_str = Numbers.Get4nMinus1EqualsSumOfTwoSquares(value);
                minus2_str = Numbers.Get4nMinus1EqualsSumOfTwoSquares(value);
            }

            SquareSumTextBox.Text = (plus1_index > 0) ? plus1_str : (minus1_index > 0) ? minus1_str : "";
            SquareDiffTextBox.Text = (plus1_index > 0) ? plus2_str : (minus1_index > 0) ? minus2_str : "";
            long n = 0L;
            if (plus1_str.StartsWith("4×")) // 4n+1
            {
                int start = "4×".Length;
                int end = plus1_str.IndexOf("+");
                if ((start >= 0) && (end >= start))
                {
                    string text = plus1_str.Substring(start, end - start);
                    n = long.Parse(text);
                }
            }
            else if (minus1_str.StartsWith("4×")) // 4n-1
            {
                int start = "4×".Length;
                int end = minus1_str.IndexOf("-");
                if ((start >= 0) && (end >= start))
                {
                    string text = minus1_str.Substring(start, end - start);
                    n = long.Parse(text);
                }
            }
            Color n_color = Numbers.GetNumberTypeColor(n);
            //double scale = 0.8d;
            //n_color = Color.FromArgb((int)(n_color.R * scale), (int)(n_color.G * scale), (int)(n_color.B * scale));
            SquareSumTextBox.ForeColor = n_color;
            SquareDiffTextBox.ForeColor = n_color;
            SquareSumTextBox.Refresh();
            SquareDiffTextBox.Refresh();

            int _4n1_index = (plus1_index > 0) ? plus1_index : (minus1_index > 0) ? minus1_index : 0;
            Nth4n1NumberTextBox.Text = (_4n1_index > 0) ? _4n1_index.ToString() : "";
            Nth4n1NumberTextBox.ForeColor = Numbers.GetNumberTypeColor(_4n1_index);
            Nth4n1NumberTextBox.Refresh();

            if (long.TryParse(m_factorizer.Number, out value))
            {
                if (value <= 0L)
                {
                    m_index_type = IndexType.Prime;
                }
                else if (value == 1L)
                {
                    m_index_type = IndexType.Prime;
                }
                else
                {
                    int nth_number_index = 0;
                    int nth_additive_number_index = 0;
                    int nth_non_additive_number_index = 0;
                    if (m_factorizer.IsPrime)
                    {
                        m_index_type = IndexType.Prime;
                        nth_number_index = Numbers.PrimeIndexOf(value) + 1;
                        nth_additive_number_index = Numbers.AdditivePrimeIndexOf(value) + 1;
                        nth_non_additive_number_index = Numbers.NonAdditivePrimeIndexOf(value) + 1;
                        NthNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(nth_number_index);
                        NthAdditiveNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(nth_additive_number_index);
                        NthNonAdditiveNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(nth_non_additive_number_index);
                        NthNumberTextBox.BackColor = (nth_additive_number_index > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditivePrime] : Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditivePrime];
                        NthAdditiveNumberTextBox.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditivePrime];
                        NthNonAdditiveNumberTextBox.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditivePrime];
                        NthNumberTextBox.Text = (nth_number_index > 0) ? nth_number_index.ToString() : "";
                        NthAdditiveNumberTextBox.Text = (nth_additive_number_index > 0) ? nth_additive_number_index.ToString() : "";
                        NthNonAdditiveNumberTextBox.Text = (nth_non_additive_number_index > 0) ? nth_non_additive_number_index.ToString() : "";
                        ToolTip.SetToolTip(NthNumberTextBox, "Prime index");
                        ToolTip.SetToolTip(NthAdditiveNumberTextBox, "Additive prime index");
                        ToolTip.SetToolTip(NthNonAdditiveNumberTextBox, "Non-additive prime index");

                        //if (Nth4n1NumberTextBox.Text == "")
                        //{
                        //    Nth4n1NumberTextBox.BackColor = SystemColors.ControlLight;
                        //}
                        //else
                        //{
                        //    if (Numbers.IsPrime(value))
                        //    {
                        //        Nth4n1NumberTextBox.BackColor = (nth_additive_number_index > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditivePrime] : Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditivePrime];
                        //    }
                        //    else // any other index type will be treated as IndexNumberType.Composite
                        //    {
                        //        Nth4n1NumberTextBox.BackColor = (nth_additive_number_index > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditiveComposite] : Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditiveComposite];
                        //    }
                        //}
                        //Nth4n1NumberTextBox.Refresh();
                    }
                    else // Composite
                    {
                        m_index_type = IndexType.Composite;
                        nth_number_index = Numbers.CompositeIndexOf(value) + 1;
                        nth_additive_number_index = Numbers.AdditiveCompositeIndexOf(value) + 1;
                        nth_non_additive_number_index = Numbers.NonAdditiveCompositeIndexOf(value) + 1;
                        NthNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(nth_number_index);
                        NthAdditiveNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(nth_additive_number_index);
                        NthNonAdditiveNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(nth_non_additive_number_index);
                        NthNumberTextBox.BackColor = (nth_additive_number_index > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditiveComposite] : Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditiveComposite];
                        NthAdditiveNumberTextBox.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditiveComposite];
                        NthNonAdditiveNumberTextBox.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditiveComposite];
                        NthNumberTextBox.Text = (nth_number_index > 0) ? nth_number_index.ToString() : "";
                        NthAdditiveNumberTextBox.Text = (nth_additive_number_index > 0) ? nth_additive_number_index.ToString() : "";
                        NthNonAdditiveNumberTextBox.Text = (nth_non_additive_number_index > 0) ? nth_non_additive_number_index.ToString() : "";
                        ToolTip.SetToolTip(NthNumberTextBox, "Composite index");
                        ToolTip.SetToolTip(NthAdditiveNumberTextBox, "Additive composite index");
                        ToolTip.SetToolTip(NthNonAdditiveNumberTextBox, "Non-additive composite index");

                        //if (Nth4n1NumberTextBox.Text == "")
                        //{
                        //    Nth4n1NumberTextBox.BackColor = SystemColors.ControlLight;
                        //}
                        //else
                        //{
                        //    Nth4n1NumberTextBox.BackColor = (nth_additive_number_index > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.AdditiveComposite] : Numbers.NUMBER_TYPE_BACKCOLORS[(int)NumberType.NonAdditiveComposite];
                        //}
                        //Nth4n1NumberTextBox.Refresh();
                    }
                }

                UpdateToolTipNth4n1NumberTextBox();
            }
            else  // BigIntegr so Yafu tool is used
            {
                if (m_factorizer.IsPrime)
                {
                    m_index_type = IndexType.Prime;

                    ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(73L);
                    if (Numbers.IsPrime(Numbers.DigitSum(m_factorizer.Number)))
                    {
                        ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(29L);
                    }
                    ValueTextBox.Refresh();

                    NthNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(-1L);
                    NthAdditiveNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(-1L);
                    NthNonAdditiveNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(-1L);
                    NthNumberTextBox.Text = "-1";
                    NthAdditiveNumberTextBox.Text = "-1";
                    NthNonAdditiveNumberTextBox.Text = "-1";
                    ToolTip.SetToolTip(NthNumberTextBox, "Prime index");
                    ToolTip.SetToolTip(NthAdditiveNumberTextBox, "Additive prime index");
                    ToolTip.SetToolTip(NthNonAdditiveNumberTextBox, "Non-additive prime index");
                }
                else // Composite BigInteger
                {
                    m_index_type = IndexType.Composite;

                    ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(21L);
                    if (Numbers.IsComposite(Numbers.DigitSum(m_factorizer.Number)))
                    {
                        ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(15L);

                        if (Numbers.IsCompositeDigits(m_factorizer.Number))
                        {
                            ValueTextBox.ForeColor = Numbers.GetNumberTypeColor(4L);
                        }
                    }
                    ValueTextBox.Refresh();

                    NthNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(-1L);
                    NthAdditiveNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(-1L);
                    NthNonAdditiveNumberTextBox.ForeColor = Numbers.GetNumberTypeColor(-1L);
                    NthNumberTextBox.Text = "-1";
                    NthAdditiveNumberTextBox.Text = "-1";
                    NthNonAdditiveNumberTextBox.Text = "-1";
                    ToolTip.SetToolTip(NthNumberTextBox, "Composite index");
                    ToolTip.SetToolTip(NthAdditiveNumberTextBox, "Additive composite index");
                    ToolTip.SetToolTip(NthNonAdditiveNumberTextBox, "Non-additive composite index");
                }
            }

            NthNumberTextBox.Refresh();
            NthAdditiveNumberTextBox.Refresh();
            NthNonAdditiveNumberTextBox.Refresh();
        }
        catch //(Exception ex)
        {
            //MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void UpdateDigitSumRootPCChains(long value)
    {
        if (value < 0L) value *= -1L;

        this.Cursor = Cursors.WaitCursor;
        try
        {
            int digit_sum = Numbers.DigitSum(value);
            DigitSumTextBox.Text = (digit_sum == 0) ? "" : digit_sum.ToString();
            DigitSumTextBox.ForeColor = Numbers.GetNumberTypeColor(digit_sum);
            DigitSumTextBox.Refresh();

            int digital_root = Numbers.DigitalRoot(value);
            DigitalRootTextBox.Text = (digital_root == 0) ? "" : digital_root.ToString();
            DigitalRootTextBox.ForeColor = Numbers.GetNumberTypeColor(digital_root);
            DigitalRootTextBox.Refresh();

            long sum_of_numbers = Numbers.SumOfNumbers(value);
            SumOfNumbersTextBox.Text = (sum_of_numbers == 0) ? "" : sum_of_numbers.ToString();
            SumOfNumbersTextBox.ForeColor = Numbers.GetNumberTypeColor(sum_of_numbers);
            SumOfNumbersTextBox.Refresh();

            long sum_of_digit_sums = Numbers.SumOfDigitSums(value);
            SumOfDigitSumsTextBox.Text = (sum_of_digit_sums == 0) ? "" : sum_of_digit_sums.ToString();
            SumOfDigitSumsTextBox.ForeColor = Numbers.GetNumberTypeColor(sum_of_digit_sums);
            SumOfDigitSumsTextBox.Refresh();

            long sum_of_digital_roots = Numbers.SumOfDigitalRoots(value);
            SumOfDigitalRootsTextBox.Text = (sum_of_digital_roots == 0) ? "" : sum_of_digital_roots.ToString();
            SumOfDigitalRootsTextBox.ForeColor = Numbers.GetNumberTypeColor(sum_of_digital_roots);
            SumOfDigitalRootsTextBox.Refresh();

            PCIndexChainL2RTextBox.Text = DecimalPCIndexChainL2R(value).ToString();
            PCIndexChainL2RTextBox.ForeColor = Numbers.GetNumberTypeColor(DecimalPCIndexChainL2R(value));
            PCIndexChainL2RTextBox.Refresh();

            PCIndexChainR2LTextBox.Text = DecimalPCIndexChainR2L(value).ToString();
            PCIndexChainR2LTextBox.ForeColor = Numbers.GetNumberTypeColor(DecimalPCIndexChainR2L(value));
            PCIndexChainR2LTextBox.Refresh();

            CPIndexChainL2RTextBox.Text = DecimalCPIndexChainL2R(value).ToString();
            CPIndexChainL2RTextBox.ForeColor = Numbers.GetNumberTypeColor(DecimalCPIndexChainL2R(value));
            CPIndexChainL2RTextBox.Refresh();

            CPIndexChainR2LTextBox.Text = DecimalCPIndexChainR2L(value).ToString();
            CPIndexChainR2LTextBox.ForeColor = Numbers.GetNumberTypeColor(DecimalCPIndexChainR2L(value));
            CPIndexChainR2LTextBox.Refresh();

            IndexChainLengthTextBox.Text = IndexChainLength(value).ToString();
            //IndexChainLengthTextBox.ForeColor = Numbers.GetNumberTypeColor(IndexChainLength(value));
            IndexChainLengthTextBox.Refresh();
        }
        catch //(Exception ex)
        {
            //MessageBox.Show(ex.Message, Application.ProductName);
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void UpdateNumberKind(long value)
    {
        if (value < 0L) value *= -1L;

        m_number_kind = Numbers.GetNumberKind(value);
        int number_kind_index = 0;
        switch (m_number_kind)
        {
            case NumberKind.Deficient:
                {
                    number_kind_index = Numbers.DeficientIndexOf(value) + 1;
                    NumberKindIndexTextBox.BackColor = Numbers.NUMBER_KIND_COLORS[0];
                }
                break;
            case NumberKind.Perfect:
                {
                    number_kind_index = Numbers.PerfectIndexOf(value) + 1;
                    NumberKindIndexTextBox.BackColor = Numbers.NUMBER_KIND_COLORS[1];
                }
                break;
            case NumberKind.Abundant:
                {
                    number_kind_index = Numbers.AbundantIndexOf(value) + 1;
                    NumberKindIndexTextBox.BackColor = Numbers.NUMBER_KIND_COLORS[2];
                }
                break;
            default:
                {
                    number_kind_index = 0;
                    NumberKindIndexTextBox.BackColor = SystemColors.Control;
                }
                break;
        }
        SumOfProperDivisorsTextBox.BackColor = NumberKindIndexTextBox.BackColor;

        NumberKindIndexTextBox.Text = number_kind_index.ToString();
        NumberKindIndexTextBox.ForeColor = Numbers.GetNumberTypeColor(number_kind_index);
        ToolTip.SetToolTip(NumberKindIndexTextBox, m_number_kind.ToString() + " value index");
        NumberKindIndexTextBox.Refresh();
    }
    private void UpdateNumberKind(int index)
    {
        long value = 0L;
        switch (m_number_kind)
        {
            case NumberKind.Perfect:
                {
                    if ((index > 0) && (index < Numbers.Perfects.Count))
                    {
                        value = Numbers.Perfects[index - 1];
                        FactorizeValue(value);
                    }
                }
                break;
            case NumberKind.Abundant:
                {
                    if ((index > 0) && (index < Numbers.Abundants.Count))
                    {
                        value = Numbers.Abundants[index - 1];
                        FactorizeValue(value);
                    }
                }
                break;
            case NumberKind.Deficient:
                {
                    if ((index > 0) && (index < Numbers.Deficients.Count))
                    {
                        value = Numbers.Deficients[index - 1];
                        FactorizeValue(value);
                    }
                }
                break;
        }
    }
    private void PerfectNumbersLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string filename = Globals.NUMBERS_FOLDER + "/" + "perfect_numbers.txt";
                    FileHelper.DisplayFile(filename);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
        else
        {
            m_number_kind = NumberKind.Perfect;
            int index = 0;
            if (int.TryParse(NumberKindIndexTextBox.Text, out index))
            {
                UpdateNumberKind(index);
            }
        }
    }
    private void AbundantNumbersLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string filename = Globals.NUMBERS_FOLDER + "/" + "abundant_numbers.txt";
                    FileHelper.DisplayFile(filename);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
        else
        {
            m_number_kind = NumberKind.Abundant;
            int index = 0;
            if (int.TryParse(NumberKindIndexTextBox.Text, out index))
            {
                UpdateNumberKind(index);
            }
        }
    }
    private void DeficientNumbersLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string filename = Globals.NUMBERS_FOLDER + "/" + "deficient_numbers.txt";
                    FileHelper.DisplayFile(filename);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
        else
        {
            m_number_kind = NumberKind.Deficient;
            int index = 0;
            if (int.TryParse(NumberKindIndexTextBox.Text, out index))
            {
                UpdateNumberKind(index);
            }
        }
    }
    private void UpdateSumOfDivisors(long value)
    {
        if (value < 0L) value *= -1L;

        long sum_of_proper_divisors = Numbers.SumOfProperDivisors(value);
        string proper_divisors = Numbers.GetProperDivisorsString(value);
        SumOfProperDivisorsTextBox.Text = sum_of_proper_divisors.ToString();
        SumOfProperDivisorsTextBox.ForeColor = Numbers.GetNumberTypeColor(sum_of_proper_divisors);
        SumOfProperDivisorsTextBox.Refresh();
        ToolTip.SetToolTip(SumOfProperDivisorsTextBox, "Sum of proper divisors\r\n" + proper_divisors + " = " + sum_of_proper_divisors);
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
        long value = 0;
        StringBuilder str = new StringBuilder();
        if (long.TryParse(ValueTextBox.Text, out value))
        {
            str.AppendLine("Number\t\t\t=\t" + ValueTextBox.Text);
            long digit_sum = Numbers.DigitSum(value);

            str.AppendLine();
            str.AppendLine("Prime Factors\t\t=\t" + Numbers.FactorizeToString(value));
            int nth_number_index = 0;
            int nth_additive_number_index = 0;
            int nth_non_additive_number_index = 0;
            if (Numbers.IsPrime(value))
            {
                m_index_type = IndexType.Prime;
                nth_number_index = Numbers.PrimeIndexOf(value) + 1;
                nth_additive_number_index = Numbers.AdditivePrimeIndexOf(value) + 1;
                nth_non_additive_number_index = Numbers.NonAdditivePrimeIndexOf(value) + 1;
                str.AppendLine("P  Index\t\t=\t" + ((nth_number_index > 0) ? nth_number_index.ToString() : ""));
                str.AppendLine("AP Index\t\t=\t" + ((nth_additive_number_index > 0) ? nth_additive_number_index.ToString() : ""));
                str.AppendLine("XP Index\t\t=\t" + ((nth_non_additive_number_index > 0) ? nth_non_additive_number_index.ToString() : ""));
            }
            else // any other index type will be treated as IndexNumberType.Composite
            {
                m_index_type = IndexType.Composite;
                nth_number_index = Numbers.CompositeIndexOf(value) + 1;
                nth_additive_number_index = Numbers.AdditiveCompositeIndexOf(value) + 1;
                nth_non_additive_number_index = Numbers.NonAdditiveCompositeIndexOf(value) + 1;
                str.AppendLine("C  Index\t\t=\t" + ((nth_number_index > 0) ? nth_number_index.ToString() : ""));
                str.AppendLine("AC Index\t\t=\t" + ((nth_additive_number_index > 0) ? nth_additive_number_index.ToString() : ""));
                str.AppendLine("XC Index\t\t=\t" + ((nth_non_additive_number_index > 0) ? nth_non_additive_number_index.ToString() : ""));
            }

            str.AppendLine();
            str.AppendLine("Digit Sum            " + "\t=\t" + digit_sum);
            long digital_roor = Numbers.DigitalRoot(value);
            str.AppendLine("Digital Root         " + "\t=\t" + digital_roor);
            long sum_of_numbers = Numbers.SumOfNumbers(value);
            str.AppendLine("Sum Of Numbers       " + "\t=\t" + sum_of_numbers + ((ModifierKeys == Keys.Control) ? (" = " + Numbers.SumOfNumbersString(value)) : ""));
            long sum_of_digit_sums = Numbers.SumOfDigitSums(value);
            str.AppendLine("Sum Of Digit Sums    " + "\t=\t" + sum_of_digit_sums + ((ModifierKeys == Keys.Control) ? (" = " + Numbers.SumOfDigitSumsString(value)) : ""));
            long sum_of_digital_roots = Numbers.SumOfDigitalRoots(value);
            str.AppendLine("Sum Of Digital Roots " + "\t=\t" + sum_of_digital_roots + ((ModifierKeys == Keys.Control) ? (" = " + Numbers.SumOfDigitalRootsString(value)) : ""));

            str.AppendLine();
            string proper_divisors = Numbers.GetProperDivisorsString(value);
            long sum_of_proper_divisors = Numbers.SumOfProperDivisors(value);
            str.AppendLine("Sum Of Proper Divisors" + "\t=\t" + sum_of_proper_divisors + " = " + proper_divisors);
            m_number_kind = Numbers.GetNumberKind(value);
            int number_kind_index = 0;
            switch (m_number_kind)
            {
                case NumberKind.Deficient:
                    {
                        number_kind_index = Numbers.DeficientIndexOf(value) + 1;
                    }
                    break;
                case NumberKind.Perfect:
                    {
                        number_kind_index = Numbers.PerfectIndexOf(value) + 1;
                    }
                    break;
                case NumberKind.Abundant:
                    {
                        number_kind_index = Numbers.AbundantIndexOf(value) + 1;
                    }
                    break;
                default:
                    {
                        number_kind_index = 0;
                    }
                    break;
            }
            str.AppendLine(m_number_kind.ToString() + " Index\t\t=\t" + number_kind_index);

            str.AppendLine();
            int plus1_index = -1;
            int minus1_index = -1;
            string plus1_str = "";
            string plus2_str = "";
            string minus1_str = "";
            string minus2_str = "";
            if (Numbers.IsUnit(value) || Numbers.IsPrime(value))
            {
                plus1_index = Numbers.Prime4nPlus1IndexOf(value) + 1;
                plus1_str = Numbers.Get4nPlus1EqualsSumOfTwoSquares(value);
                plus2_str = Numbers.Get4nPlus1EqualsDiffOfTwoTrivialSquares(value);
                minus1_index = Numbers.Prime4nMinus1IndexOf(value) + 1;
                minus1_str = Numbers.Get4nMinus1EqualsSumOfTwoSquares(value);
                minus2_str = Numbers.Get4nMinus1EqualsSumOfTwoSquares(value);
            }
            else //if composite
            {
                plus1_index = Numbers.Composite4nPlus1IndexOf(value) + 1;
                plus1_str = Numbers.Get4nPlus1EqualsDiffOfTwoSquares(value);
                plus2_str = Numbers.Get4nPlus1EqualsDiffOfTwoTrivialSquares(value);
                minus1_index = Numbers.Composite4nMinus1IndexOf(value) + 1;
                minus1_str = Numbers.Get4nMinus1EqualsSumOfTwoSquares(value);
                minus2_str = Numbers.Get4nMinus1EqualsSumOfTwoSquares(value);
            }
            str.AppendLine((plus1_index > 0) ? ("4n+1 Squares1\t\t=\t" + plus1_str) : (minus1_index > 0) ? ("4n-1 Squares1\t\t=\t" + minus1_str) : "");
            str.AppendLine((plus1_index > 0) ? ("4n+1 Squares2\t\t=\t" + plus2_str) : (minus1_index > 0) ? ("4n-1 Squares2\t\t=\t" + minus2_str) : "");
            str.AppendLine((plus1_index > 0) ? ("4n+1 " + (Numbers.IsComposite(value) ? "Composite" : "Prime") + " Index\t=\t" + plus1_index.ToString()) :
                          (minus1_index > 0) ? ("4n-1 " + (Numbers.IsComposite(value) ? "Composite" : "Prime") + " Index\t=\t" + minus1_index.ToString()) : "");

            str.AppendLine();
            str.AppendLine("Left-to-right prime/composite index chain | P=0 C=1\r\n" + GetPCIndexChainL2R(value) + "\r\n" + "Chain length = " + IndexChainLength(value) + "\t\t" + BinaryPCIndexChainL2R(value) + "  =  " + DecimalPCIndexChainL2R(value));
            str.AppendLine();
            str.AppendLine("Right-to-left prime/composite index chain | P=0 C=1\r\n" + GetPCIndexChainR2L(value) + "\r\n" + "Chain length = " + IndexChainLength(value) + "\t\t" + BinaryPCIndexChainR2L(value) + "  =  " + DecimalPCIndexChainR2L(value));
            str.AppendLine();
            str.AppendLine("Left-to-right composite/prime index chain | C=0 P=1\r\n" + GetCPIndexChainL2R(value) + "\r\n" + "Chain length = " + IndexChainLength(value) + "\t\t" + BinaryCPIndexChainL2R(value) + "  =  " + DecimalCPIndexChainL2R(value));
            str.AppendLine();
            str.AppendLine("Right-to-left composite/prime index chain | C=0 P=1\r\n" + GetCPIndexChainR2L(value) + "\r\n" + "Chain length = " + IndexChainLength(value) + "\t\t" + BinaryCPIndexChainR2L(value) + "  =  " + DecimalCPIndexChainR2L(value));

            string filename = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".txt";
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                filename = Globals.NUMBERS_FOLDER + "/" + filename;
                FileHelper.SaveText(filename, str.ToString());

                // show file content after save
                FileHelper.DisplayFile(filename);
            }
        }
    }

    private long DecimalPCIndexChainL2R(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return 0L;

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Append("0");
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Append("1");
            }
            else // value is too large
            {
                return 0L;
            }
            value = index;
        }
        return Convert.ToInt64(str.ToString(), 2);
    }
    private long DecimalPCIndexChainR2L(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return 0L;

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "0");
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "1");
            }
            else // value is too large
            {
                return 0L;
            }
            value = index;
        }
        return Convert.ToInt64(str.ToString(), 2);
    }
    private long DecimalCPIndexChainL2R(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return 0L;

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Append("1");
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Append("0");
            }
            else // value is too large
            {
                return 0L;
            }
            value = index;
        }
        return Convert.ToInt64(str.ToString(), 2);
    }
    private long DecimalCPIndexChainR2L(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return 0L;

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "1");
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "0");
            }
            else // value is too large
            {
                return 0L;
            }
            value = index;
        }
        return Convert.ToInt64(str.ToString(), 2);
    }
    private int IndexChainLength(long number)
    {
        if (number < 0L) number *= -1L;
        if (number == 0L) return 0;

        int length = 0;
        while (number > 1L)
        {
            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(number) + 1)) > 0)
            {
                length++;
            }
            else if ((index = (Numbers.CompositeIndexOf(number) + 1)) > 0)
            {
                length++;
            }
            else // number is too large
            {
                return 0;
            }
            number = index;
        }
        return length;
    }
    private string BinaryPCIndexChainL2R(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return "0";

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Append("0");
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Append("1");
            }
            else // value is too large
            {
                return "";
            }
            value = index;
        }
        return str.ToString();
    }
    private string BinaryPCIndexChainR2L(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return "0";

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "0");
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "1");
            }
            else // value is too large
            {
                return "";
            }
            value = index;
        }
        return str.ToString();
    }
    private string BinaryCPIndexChainL2R(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return "0";

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Append("1");
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Append("0");
            }
            else // value is too large
            {
                return "";
            }
            value = index;
        }
        return str.ToString();
    }
    private string BinaryCPIndexChainR2L(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return "0";

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "1");
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "0");
            }
            else // value is too large
            {
                return "";
            }
            value = index;
        }
        return str.ToString();
    }
    private string GetPCIndexChainL2R(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return "0";

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            if (str.Length > 0)
            {
                str.Append("-");
            }

            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Append("P" + index.ToString());
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Append("C" + index.ToString());
            }
            else // value is too large
            {
                return "";
            }
            value = index;
        }
        return str.ToString();
    }
    private string GetPCIndexChainR2L(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return "0";

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            if (str.Length > 0)
            {
                str.Insert(0, "-");
            }

            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "P" + index.ToString());
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "C" + index.ToString());
            }
            else // value is too large
            {
                return "";
            }
            value = index;
        }
        return str.ToString();
    }
    private string GetCPIndexChainL2R(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return "0";

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            if (str.Length > 0)
            {
                str.Append("-");
            }

            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Append("P" + index.ToString());
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Append("C" + index.ToString());
            }
            else // value is too large
            {
                return "";
            }
            value = index;
        }
        return str.ToString();
    }
    private string GetCPIndexChainR2L(long value)
    {
        if (value < 0L) value *= -1L;
        if (value == 0L) return "0";

        StringBuilder str = new StringBuilder();
        while (value > 1L)
        {
            if (str.Length > 0)
            {
                str.Insert(0, "-");
            }

            int index = 0;
            if ((index = (Numbers.PrimeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "P" + index.ToString());
            }
            else if ((index = (Numbers.CompositeIndexOf(value) + 1)) > 0)
            {
                str.Insert(0, "C" + index.ToString());
            }
            else // value is too large
            {
                return "";
            }
            value = index;
        }
        return str.ToString();
    }
    private void NumberIndexChain(long value)
    {
        if (value < 0L) value *= -1L;

        string filename = "NumberIndexChain" + "_" + value + ".txt";
        StringBuilder str = new StringBuilder();
        int length = IndexChainLength(value);
        str.AppendLine(value.ToString() + "\t" + length.ToString() + "\t" + GetPCIndexChainL2R(value));
        SaveNumberIndexChain(filename, value, length, str.ToString());
    }
    private void NumbersOfIndexChainLength(int length)
    {
        string filename = "PIndexChainLength" + "_" + length + ".txt";
        StringBuilder str = new StringBuilder();
        long running_total = 0L;
        for (int i = 0; i < Numbers.Primes.Count; i++)
        {
            long value = Numbers.Primes[i];
            int len = IndexChainLength(value);
            if (len == length)
            {
                running_total += value;
                str.AppendLine(value.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(value).ToString() + "\t" + DecimalPCIndexChainR2L(value).ToString() + "\t" + DecimalCPIndexChainL2R(value).ToString() + "\t" + DecimalCPIndexChainR2L(value).ToString() + "\t" + GetPCIndexChainL2R(value));
            }
        }
        SaveIndexChainLength(filename, NumberType.Prime, length, str.ToString());

        filename = "APIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < Numbers.AdditivePrimes.Count; i++)
        {
            long value = Numbers.AdditivePrimes[i];
            int len = IndexChainLength(value);
            if (len == length)
            {
                running_total += value;
                str.AppendLine(value.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(value).ToString() + "\t" + DecimalPCIndexChainR2L(value).ToString() + "\t" + DecimalCPIndexChainL2R(value).ToString() + "\t" + DecimalCPIndexChainR2L(value).ToString() + "\t" + GetPCIndexChainL2R(value));
            }
        }
        SaveIndexChainLength(filename, NumberType.AdditivePrime, length, str.ToString());

        filename = "XPIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < Numbers.NonAdditivePrimes.Count; i++)
        {
            long value = Numbers.NonAdditivePrimes[i];
            int len = IndexChainLength(value);
            if (len == length)
            {
                running_total += value;
                str.AppendLine(value.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(value).ToString() + "\t" + DecimalPCIndexChainR2L(value).ToString() + "\t" + DecimalCPIndexChainL2R(value).ToString() + "\t" + DecimalCPIndexChainR2L(value).ToString() + "\t" + GetPCIndexChainL2R(value));
            }
        }
        SaveIndexChainLength(filename, NumberType.NonAdditivePrime, length, str.ToString());

        filename = "CIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < Numbers.Composites.Count; i++)
        {
            long value = Numbers.Composites[i];
            int len = IndexChainLength(value);
            if (len == length)
            {
                running_total += value;
                str.AppendLine(value.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(value).ToString() + "\t" + DecimalPCIndexChainR2L(value).ToString() + "\t" + DecimalCPIndexChainL2R(value).ToString() + "\t" + DecimalCPIndexChainR2L(value).ToString() + "\t" + GetPCIndexChainL2R(value));
            }
        }
        SaveIndexChainLength(filename, NumberType.Composite, length, str.ToString());

        filename = "ACIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < Numbers.AdditiveComposites.Count; i++)
        {
            long value = Numbers.AdditiveComposites[i];
            int len = IndexChainLength(value);
            if (len == length)
            {
                running_total += value;
                str.AppendLine(value.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(value).ToString() + "\t" + DecimalPCIndexChainR2L(value).ToString() + "\t" + DecimalCPIndexChainL2R(value).ToString() + "\t" + DecimalCPIndexChainR2L(value).ToString() + "\t" + GetPCIndexChainL2R(value));
            }
        }
        SaveIndexChainLength(filename, NumberType.AdditiveComposite, length, str.ToString());

        filename = "XCIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < Numbers.NonAdditiveComposites.Count; i++)
        {
            long value = Numbers.NonAdditiveComposites[i];
            int len = IndexChainLength(value);
            if (len == length)
            {
                running_total += value;
                str.AppendLine(value.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(value).ToString() + "\t" + DecimalPCIndexChainR2L(value).ToString() + "\t" + DecimalCPIndexChainL2R(value).ToString() + "\t" + DecimalCPIndexChainR2L(value).ToString() + "\t" + GetPCIndexChainL2R(value));
            }
        }
        SaveIndexChainLength(filename, NumberType.NonAdditiveComposite, length, str.ToString());

        filename = "NIndexChainLength" + "_" + length + ".txt";
        str = new StringBuilder();
        running_total = 0L;
        for (int i = 0; i < int.MaxValue / 1024; i++)
        {
            int len = IndexChainLength(i);
            if (len == length)
            {
                long value = i;
                running_total += value;
                str.AppendLine(value.ToString() + "\t" + running_total.ToString() + "\t" + DecimalPCIndexChainL2R(value).ToString() + "\t" + DecimalPCIndexChainR2L(value).ToString() + "\t" + DecimalCPIndexChainL2R(value).ToString() + "\t" + DecimalCPIndexChainR2L(value).ToString() + "\t" + GetPCIndexChainL2R(value));
            }
        }
        SaveIndexChainLength(filename, NumberType.Natural, length, str.ToString());
    }
    public void SaveNumberIndexChain(string filename, long value, int chain_length, string text)
    {
        if (value < 0L) value *= -1L;

        if (Directory.Exists(Globals.NUMBERS_FOLDER))
        {
            filename = Globals.NUMBERS_FOLDER + "/" + filename;
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine(value.ToString() + " with IndexChainLength = " + chain_length.ToString());
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine("Number\tTotal\tPC_L2R\tPC_R2L\tCP_L2R\tCP_R2L\tChain");
                    writer.WriteLine("-----------------------------------------------------");
                    writer.Write(text);
                    writer.WriteLine("-----------------------------------------------------");
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            FileHelper.DisplayFile(filename);
        }
    }
    public void SaveIndexChainLength(string filename, NumberType number_type, int chain_length, string text)
    {
        if (Directory.Exists(Globals.NUMBERS_FOLDER))
        {
            filename = Globals.NUMBERS_FOLDER + "/" + filename;
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine(number_type.ToString() + " numbers with IndexChainLength = " + chain_length.ToString());
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine("Number\tTotal\tPC_L2R\tPC_R2L\tCP_L2R\tCP_R2L\tChain");
                    writer.WriteLine("-----------------------------------------------------");
                    writer.Write(text);
                    writer.WriteLine("-----------------------------------------------------");
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            FileHelper.DisplayFile(filename);
        }
    }

    private NumberKind m_number_kind = NumberKind.Deficient;
    private IndexType m_index_type = IndexType.Prime;
    private bool m_plus1_index = true;
    private bool m_minus1_index = false;
    private void IndexTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        Control control = (sender as TextBoxBase);
        if (control != null)
        {
            if (e.KeyCode == Keys.Up)
            {
                IncrementValue(control);
            }
            else if (e.KeyCode == Keys.Down)
            {
                DecrementValue(control);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                FactorizeValue(control);
            }
        }
    }
    private void FactorizeValue(Control control)
    {
        long value = -1L;
        int index = 0;
        if (int.TryParse(control.Text, out index))
        {
            index--;
            if (index >= 0)
            {
                control.ForeColor = Numbers.GetNumberTypeColor(index + 1);

                if (control == NthNumberTextBox)
                {
                    if (m_index_type == IndexType.Prime)
                    {
                        if (index < Numbers.Primes.Count)
                        {
                            value = Numbers.Primes[index];
                        }
                    }
                    else // any other index type will be treated as IndexNumberType.Composite
                    {
                        if (index < Numbers.Composites.Count)
                        {
                            value = Numbers.Composites[index];
                        }
                    }
                }
                else if (control == NthAdditiveNumberTextBox)
                {
                    if (m_index_type == IndexType.Prime)
                    {
                        if (index < Numbers.AdditivePrimes.Count)
                        {
                            value = Numbers.AdditivePrimes[index];
                        }
                    }
                    else // any other index type will be treated as IndexNumberType.Composite
                    {
                        if (index < Numbers.AdditiveComposites.Count)
                        {
                            value = Numbers.AdditiveComposites[index];
                        }
                    }
                }
                else if (control == NthNonAdditiveNumberTextBox)
                {
                    if (m_index_type == IndexType.Prime)
                    {
                        if (index < Numbers.NonAdditivePrimes.Count)
                        {
                            value = Numbers.NonAdditivePrimes[index];
                        }
                    }
                    else // any other index type will be treated as IndexNumberType.Composite
                    {
                        if (index < Numbers.NonAdditiveComposites.Count)
                        {
                            value = Numbers.NonAdditiveComposites[index];
                        }
                    }
                }
                else if (control == Nth4n1NumberTextBox)
                {
                    if (m_index_type == IndexType.Prime)
                    {
                        if (m_plus1_index)
                        {
                            if (index < Numbers.Primes4nPlus1.Count)
                            {
                                value = Numbers.Primes4nPlus1[index];
                            }
                        }
                        else if (m_minus1_index)
                        {
                            if (index < Numbers.Primes4nMinus1.Count)
                            {
                                value = Numbers.Primes4nMinus1[index];
                            }
                        }
                        else
                        {
                            // do nothing
                        }
                    }
                    else if (m_index_type == IndexType.Composite)
                    {
                        if (m_plus1_index)
                        {
                            if (index < Numbers.Composites4nPlus1.Count)
                            {
                                value = Numbers.Composites4nPlus1[index];
                            }
                        }
                        else if (m_minus1_index)
                        {
                            if (index < Numbers.Composites4nMinus1.Count)
                            {
                                value = Numbers.Composites4nMinus1[index];
                            }
                        }
                        else
                        {
                            // do nothing
                        }
                    }
                    else
                    {
                        // do nothing
                    }

                    UpdateToolTipNth4n1NumberTextBox();
                }
                else if (control == NumberKindIndexTextBox)
                {
                    switch (m_number_kind)
                    {
                        case NumberKind.Deficient:
                            {
                                if (index < Numbers.Deficients.Count)
                                {
                                    value = Numbers.Deficients[index];
                                }
                            }
                            break;
                        case NumberKind.Perfect:
                            {
                                if (index < Numbers.Perfects.Count)
                                {
                                    value = Numbers.Perfects[index];
                                }
                            }
                            break;
                        case NumberKind.Abundant:
                            {
                                if (index < Numbers.Abundants.Count)
                                {
                                    value = Numbers.Abundants[index];
                                }
                            }
                            break;
                        default:
                            {
                            }
                            break;
                    }
                }
                else
                {
                    // do nothing
                }

                FactorizeValue(value);
            }
            else
            {
                FactorizeValue(0L);
            }
        }
        else
        {
            FactorizeValue(0L);
        }
    }
    private void IncrementValue(Control control)
    {
        if (control is TextBoxBase)
        {
            long value = 0L;
            if (long.TryParse(control.Text, out value))
            {
                if (value < long.MaxValue)
                {
                    value++;
                    control.Text = value.ToString();
                }
            }
        }
    }
    private void DecrementValue(Control control)
    {
        if (control is TextBoxBase)
        {
            long value = 0L;
            if (long.TryParse(control.Text, out value))
            {
                if (value > 0L)
                {
                    value--;
                    control.Text = value.ToString();
                }
            }
        }
    }
    private void UpdateToolTipNth4n1NumberTextBox()
    {
        long value = 0L;
        if (long.TryParse(ValueTextBox.Text, out value))
        {
            int plus1_index = -1;
            int minus1_index = -1;
            Nth4nPlus1PrimeNumberLabel.BackColor = SystemColors.ControlLight;
            Nth4nMinus1PrimeNumberLabel.BackColor = SystemColors.ControlLight;
            Nth4nPlus1CompositeNumberLabel.BackColor = SystemColors.ControlLight;
            Nth4nMinus1CompositeNumberLabel.BackColor = SystemColors.ControlLight;
            if (Numbers.IsUnit(value) || Numbers.IsPrime(value))
            {
                plus1_index = Numbers.Prime4nPlus1IndexOf(value) + 1;
                minus1_index = Numbers.Prime4nMinus1IndexOf(value) + 1;
            }
            else //if composite
            {
                plus1_index = Numbers.Composite4nPlus1IndexOf(value) + 1;
                minus1_index = Numbers.Composite4nMinus1IndexOf(value) + 1;
            }

            m_plus1_index = (plus1_index > 0);
            m_minus1_index = (minus1_index > 0);
            if (m_plus1_index || m_minus1_index)
            {
                if (Numbers.IsPrime(value))
                {
                    if (m_plus1_index)
                    {
                        ToolTip.SetToolTip(Nth4n1NumberTextBox, "4n+1 prime index");
                        Nth4nPlus1PrimeNumberLabel.BackColor = SystemColors.ControlLightLight;
                    }
                    else if (m_minus1_index)
                    {
                        ToolTip.SetToolTip(Nth4n1NumberTextBox, "4n-1 prime index");
                        Nth4nMinus1PrimeNumberLabel.BackColor = SystemColors.ControlLightLight;
                    }
                    else
                    {
                        ToolTip.SetToolTip(Nth4n1NumberTextBox, null);
                    }
                }
                else // any other index type will be treated as IndexNumberType.Composite
                {
                    if (m_plus1_index)
                    {
                        ToolTip.SetToolTip(Nth4n1NumberTextBox, "4n+1 composite index");
                        Nth4nPlus1CompositeNumberLabel.BackColor = SystemColors.ControlLightLight;
                    }
                    else if (m_minus1_index)
                    {
                        ToolTip.SetToolTip(Nth4n1NumberTextBox, "4n-1 composite index");
                        Nth4nMinus1CompositeNumberLabel.BackColor = SystemColors.ControlLightLight;
                    }
                    else
                    {
                        ToolTip.SetToolTip(Nth4n1NumberTextBox, null);
                    }
                }
            }
            else
            {
                ToolTip.SetToolTip(Nth4n1NumberTextBox, null);
            }
            Nth4n1NumberTextBox.Refresh();
        }
    }
    private void Nth4nPlus1PrimeNumberLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            this.Cursor = Cursors.WaitCursor;
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string filename = Globals.NUMBERS_FOLDER + "/" + "4n+1_primes.txt";
                    FileHelper.DisplayFile(filename);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
        else
        {
            m_index_type = IndexType.Prime;
            m_plus1_index = true;
            m_minus1_index = false;
            FactorizeValue(Nth4n1NumberTextBox);
        }
    }
    private void Nth4nMinus1PrimeNumberLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            this.Cursor = Cursors.WaitCursor;
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                try
                {
                    string filename = Globals.NUMBERS_FOLDER + "/" + "4n-1_primes.txt";
                    FileHelper.DisplayFile(filename);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
        else
        {
            m_index_type = IndexType.Prime;
            m_plus1_index = false;
            m_minus1_index = true;
            FactorizeValue(Nth4n1NumberTextBox);
        }
    }
    private void Nth4nPlus1CompositeNumberLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string filename = Globals.NUMBERS_FOLDER + "/" + "4n+1_composites.txt";
                    FileHelper.DisplayFile(filename);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
        else
        {
            m_index_type = IndexType.Composite;
            m_plus1_index = true;
            m_minus1_index = false;
            FactorizeValue(Nth4n1NumberTextBox);
        }
    }
    private void Nth4nMinus1CompositeNumberLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string filename = Globals.NUMBERS_FOLDER + "/" + "4n-1_composites.txt";
                    FileHelper.DisplayFile(filename);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
        else
        {
            m_index_type = IndexType.Composite;
            m_plus1_index = false;
            m_minus1_index = true;
            FactorizeValue(Nth4n1NumberTextBox);
        }
    }
    private void PrimeNumbersLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string filename = Globals.NUMBERS_FOLDER + "/" + "primes.txt";
                FileHelper.DisplayFile(filename);
                filename = Globals.NUMBERS_FOLDER + "/" + "additive_primes.txt";
                FileHelper.DisplayFile(filename);
                filename = Globals.NUMBERS_FOLDER + "/" + "non_additive_primes.txt";
                FileHelper.DisplayFile(filename);
                //string filename = "Primalogy.pdf";
                //string path = Application.StartupPath + "/" + Globals.HELP_FOLDER + "/" + filename;
                //if (!File.Exists(path))
                //{
                //    DownloadFile("http://heliwave.com/" + filename, path);
                //}
                //if (File.Exists(path))
                //{
                //    FileHelper.WaitForReady(path);

                //    System.Diagnostics.Process.Start(path);
                //}
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    MessageBox.Show(ex.Message, Application.ProductName);
                    ex = ex.InnerException;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        else
        {
            m_index_type = IndexType.Prime;
            FactorizeValue(NthNumberTextBox);
        }
    }
    private void CompositeNumbersLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string filename = Globals.NUMBERS_FOLDER + "/" + "composites.txt";
                FileHelper.DisplayFile(filename);
                filename = Globals.NUMBERS_FOLDER + "/" + "additive_composites.txt";
                FileHelper.DisplayFile(filename);
                filename = Globals.NUMBERS_FOLDER + "/" + "non_additive_composites.txt";
                FileHelper.DisplayFile(filename);
                //string filename = "Primalogy_AR.pdf";
                //string path = Application.StartupPath + "/" + Globals.HELP_FOLDER + "/" + filename;
                //if (!File.Exists(path))
                //{
                //    DownloadFile("http://heliwave.com/" + filename, path);
                //}
                //if (File.Exists(path))
                //{
                //    FileHelper.WaitForReady(path);

                //    System.Diagnostics.Process.Start(path);
                //}
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    MessageBox.Show(ex.Message, Application.ProductName);
                    ex = ex.InnerException;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        else
        {
            m_index_type = IndexType.Composite;
            FactorizeValue(NthNumberTextBox);
        }
    }

    private void TextBoxLabelControls_CtrlClick(object sender, EventArgs e)
    {
        // Ctrl+Click factorizes value
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
    private void PCIndexChainL2RTextBox_TextChanged(object sender, EventArgs e)
    {
        long value = 0L;
        if (long.TryParse(ValueTextBox.Text, out value))
        {
            this.ToolTip.SetToolTip(this.PCIndexChainL2RTextBox, "Left-to-right prime/composite index chain | P=0 C=1\r\n" + GetPCIndexChainL2R(value) + "\r\n" + "Chain length = " + IndexChainLength(value) + "\t\t" + BinaryPCIndexChainL2R(value) + "  =  " + DecimalPCIndexChainL2R(value));
        }
    }
    private void PCIndexChainR2LTextBox_TextChanged(object sender, EventArgs e)
    {
        long value = 0L;
        if (long.TryParse(ValueTextBox.Text, out value))
        {
            this.ToolTip.SetToolTip(this.PCIndexChainR2LTextBox, "Right-to-left prime/composite index chain | P=0 C=1\r\n" + GetPCIndexChainR2L(value) + "\r\n" + "Chain length = " + IndexChainLength(value) + "\t\t" + BinaryPCIndexChainR2L(value) + "  =  " + DecimalPCIndexChainR2L(value));
        }
    }
    private void CPIndexChainL2RTextBox_TextChanged(object sender, EventArgs e)
    {
        long value = 0L;
        if (long.TryParse(ValueTextBox.Text, out value))
        {
            this.ToolTip.SetToolTip(this.CPIndexChainL2RTextBox, "Left-to-right composite/prime index chain | C=0 P=1\r\n" + GetCPIndexChainL2R(value) + "\r\n" + "Chain length = " + IndexChainLength(value) + "\t\t" + BinaryCPIndexChainL2R(value) + "  =  " + DecimalCPIndexChainL2R(value));
        }
    }
    private void CPIndexChainR2LTextBox_TextChanged(object sender, EventArgs e)
    {
        long value = 0L;
        if (long.TryParse(ValueTextBox.Text, out value))
        {
            this.ToolTip.SetToolTip(this.CPIndexChainR2LTextBox, "Right-to-left composite/prime index chain | C=0 P=1\r\n" + GetCPIndexChainR2L(value) + "\r\n" + "Chain length = " + IndexChainLength(value) + "\t\t" + BinaryCPIndexChainR2L(value) + "  =  " + DecimalCPIndexChainR2L(value));
        }
    }
    private void IndexChainLengthTextBox_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            TextBoxLabelControls_CtrlClick(sender, e);
        }
        else
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                int length = 0;
                if (int.TryParse(IndexChainLengthTextBox.Text, out length))
                {
                    NumbersOfIndexChainLength(length);
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
    private void FactorizeNumber(Label control)
    {
        if (control != null)
        {
            long value = 0L;
            try
            {
                string text = control.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    value = Math.Abs((long)double.Parse(text));
                }

                FactorizeValue(value);
            }
            catch
            {
                //value = -1L; // error
            }
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
                            value = Radix.Decode(text, Numbers.DEFAULT_RADIX);
                        }
                        else if (text.StartsWith("4×")) // 4n+1 or 4n-1
                        {
                            int start = "4×".Length;
                            int end = text.IndexOf("+");
                            if (end == -1) end = text.IndexOf("-");
                            if ((start >= 0) && (end >= start))
                            {
                                text = text.Substring(start, end - start);
                                value = Radix.Decode(text, Numbers.DEFAULT_RADIX);
                            }
                        }
                        else
                        {
                            value = Radix.Decode(text, Numbers.DEFAULT_RADIX);
                        }
                    }

                    FactorizeValue(value);
                }
                catch
                {
                    //value = -1L; // error
                }
            }
        }
    }

    private void EnableEntryControls()
    {
        MultithreadingCheckBox.Enabled = true;

        ValueTextBox.Enabled = true;
        ValueTextBox.ForeColor = SystemColors.ControlText;
        ValueTextBox.Refresh();

        m_old_progress = -1;
        ProgressLabel.Text = "Progress";
        ProgressLabel.Refresh();
    }
    private void DisableEntryControls()
    {
        MultithreadingCheckBox.Enabled = false;

        ValueTextBox.Enabled = false;
        ValueTextBox.ForeColor = SystemColors.ControlDark;
        ValueTextBox.Refresh();

        ProgressLabel.Text = "IsPrime ...";
        ProgressLabel.Refresh();
    }
    private void ClearNumberAnalyses()
    {
        Nth4n1NumberTextBox.Text = "";
        Nth4nPlus1PrimeNumberLabel.BackColor = SystemColors.ControlLightLight;
        Nth4nMinus1PrimeNumberLabel.BackColor = SystemColors.ControlLight;
        Nth4nPlus1CompositeNumberLabel.BackColor = SystemColors.ControlLight;
        Nth4nMinus1CompositeNumberLabel.BackColor = SystemColors.ControlLight;

        NthNumberTextBox.Text = "";
        NthAdditiveNumberTextBox.Text = "";
        NthNonAdditiveNumberTextBox.Text = "";

        SquareSumTextBox.Text = "";
        SquareSumTextBox.Refresh();
        SquareDiffTextBox.Text = "";
        SquareDiffTextBox.Refresh();

        ClearFactors();
    }
    private void ClearFactors()
    {
        PrimeFactorsTextBox.Text = "";
        PrimeFactorsTextBox.Refresh();
        OutputTextBox.Text = "";
        OutputTextBox.Refresh();
    }
    private void ClearProgress()
    {
        ElapsedTimeValueLabel.Text = "00:00:00";
        ElapsedTimeValueLabel.Refresh();
        MilliSecondsLabel.Text = "000";
        MilliSecondsLabel.Refresh();
        ProgressBar.Value = 0;
        ProgressValueLabel.Text = ProgressBar.Value + "%";
        ProgressValueLabel.Refresh();
    }
    private void BeforeProcessing()
    {
        DisableEntryControls();
        ClearNumberAnalyses();
        ClearFactors();
        ClearProgress();
    }
    private void CallRun()
    {
        ValueTextBox.Text = ValueTextBox.Text.Replace(" ", "");

        long value = 0L;
        string expression = ValueTextBox.Text;
        if (long.TryParse(expression, out value))
        {
            m_double_value = (double)value;
        }
        else
        {
            m_double_value = CalculateValue(expression);
            value = (long)Math.Round(m_double_value);
        }

        if (m_double_value != value)
        {
            PrimeFactorsTextBox.Text = m_double_value.ToString();
        }

        MainTabControl.SelectedIndex = 0;

        Run();
    }
    private bool m_multithreading = true;
    private void MultithreadingCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        m_multithreading = MultithreadingCheckBox.Checked;
    }
    public void Run()
    {
        // guard against multiple runs on multiple ENTER keys
        if ((m_worker_thread != null) && (m_worker_thread.IsAlive)) return;

        this.Cursor = Cursors.WaitCursor;
        try
        {
            string arguments = ValueTextBox.Text;
            if (arguments.Length == 0) return;
            while (arguments.StartsWith("-")) arguments = arguments.Substring(1);
            if (arguments.StartsWith("0")) return;

            BeforeProcessing();
            try
            {
                if (Numbers.IsDigitsOnly(arguments))
                {
                    m_factorizer = new Factorizer(this, "factor", arguments, m_multithreading);
                }
                else
                {
                    m_factorizer = new Factorizer(this, null, arguments, m_multithreading);
                }

                if (m_factorizer != null)
                {
                    m_worker_thread = new Thread(new ThreadStart(m_factorizer.Run));
                    m_worker_thread.Priority = ThreadPriority.Highest;
                    m_worker_thread.IsBackground = false;
                    m_worker_thread.Start();
                }
            }
            catch
            {
                Cancel();
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    public void Cancel()
    {
        if ((m_worker_thread != null) && (m_worker_thread.IsAlive))
        {
            if (m_factorizer != null)
            {
                m_factorizer.Cancel = true;

                if (m_worker_thread != null)
                {
                    m_worker_thread.Join();
                    m_worker_thread = null;
                }
            }

            AfterCancelled();
        }
    }
    private void AfterCancelled()
    {
        EnableEntryControls();
        ClearProgress();
        ValueTextBox.Focus();
    }
    private void AfterProcessing()
    {
        string item = ValueTextBox.Text;
        if (!m_history_items.Contains(item))
        {
            m_history_items.Insert(m_history_index + 1, item);
            m_history_index++;
        }

        m_index_type = IndexType.Prime;

        if (m_factorizer != null)
        {
            EnableEntryControls();

            long value = 0L;
            if (long.TryParse(m_factorizer.Number, out value))
            {
                AnalyzeValue(value);
            }
        }
        ValueTextBox.Focus();
    }

    private enum TimeDisplayMode { Elapsed, Remaining }
    private TimeDisplayMode m_time_display_mode = TimeDisplayMode.Elapsed;
    private void ElapsedTimeLabel_Click(object sender, EventArgs e)
    {
        if ((m_worker_thread != null) && (m_worker_thread.IsAlive))
        {
            if (m_time_display_mode == TimeDisplayMode.Elapsed)
            {
                ElapsedTimeLabel.Text = "Remaining Time";
                m_time_display_mode = TimeDisplayMode.Remaining;
            }
            else
            {
                ElapsedTimeLabel.Text = "Elapsed Time";
                m_time_display_mode = TimeDisplayMode.Elapsed;
            }
        }
    }

    private int m_old_progress = -1;
    public delegate void UpdateProgressBar(int progress);
    public void UpdateProgressBarMethod(int progress)
    {
        if (!m_factorizer.Cancel)
        {
            if (progress > m_old_progress)
            {
                m_old_progress = progress;
                if (m_factorizer != null)
                {
                    if (!m_factorizer.Cancel)
                    {
                        if ((progress >= 0) && (progress <= 100))
                        {
                            ProgressBar.Value = progress;
                            ProgressValueLabel.Text = ProgressBar.Value + "%";
                            ProgressValueLabel.Refresh();

                            UpdateTimer(progress);
                        }
                    }

                    if (progress == 100) // finished naturally 
                    {
                        AfterProcessing();
                    }
                }
            }
            else // no new progress
            {
                if (m_time_display_mode == TimeDisplayMode.Elapsed)
                {
                    if (m_factorizer != null)
                    {
                        UpdateTimer(progress);
                    }
                }
                else //if (m_time_display_mode == TimeDisplayMode.Remaining)
                {
                    // don't update remaining as it goes up wrongly with time passage and no progress
                    // if progree was double not int then ok but now
                    // it keeps going up up up till it there is progress so t goes down to the real remaining time and then
                    // it keeps going up up up again and so on, like a seesaw
                }
            }
        }
    }
    private void UpdateTimer(int progress)
    {
        TimeSpan timespan = m_factorizer.Duration;
        if (m_time_display_mode == TimeDisplayMode.Remaining)
        {
            if (progress > 0)
            {
                long ticks = (long)((double)timespan.Ticks * ((double)(100 - progress) / (double)progress));
                timespan = new TimeSpan(ticks);
            }
            else
            {
                timespan = new TimeSpan(0, 0, 0);
            }
        }
        ElapsedTimeValueLabel.Text = String.Format("{0:00}:{1:00}:{2:00}", timespan.Hours, timespan.Minutes, timespan.Seconds);
        ElapsedTimeValueLabel.Refresh();
        MilliSecondsLabel.Text = String.Format("{0:000}", timespan.Milliseconds);
        MilliSecondsLabel.Refresh();
    }

    public delegate void UpdateOutputTextBox(string output);
    public void UpdateOutputTextBoxMethod(string output)
    {
        if (m_factorizer != null)
        {
            if (!m_factorizer.Cancel)
            {
                if (m_factorizer.IsPrime == false)
                {
                    ProgressLabel.Text = "Factoring ...";
                    ProgressLabel.Refresh();
                }

                OutputTextBox.Text = output;
                OutputTextBox.Refresh();
                OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                OutputTextBox.ScrollToCaret();

                int pos = output.LastIndexOf("\r\n");
                if (pos > -1)
                {
                    string result = output.Substring(pos);
                    if ((result.Length > 0) && (result != "\r\n"))
                    {
                        pos = result.IndexOf("=");
                        if (pos > -1)
                        {
                            PrimeFactorsTextBox.Text = result.Substring(pos + 1);
                            PrimeFactorsTextBox.Refresh();
                        }
                    }
                }

                if (!output.Contains(Environment.NewLine))
                {
                    if (output.Length == 0) return;
                    while (output.StartsWith("-")) output = output.Substring(1);
                    while (output.StartsWith("0")) output = output.Substring(1);
                    if (output.Length == 0) return; // check again, yes!

                    if (Char.IsLetter(output[0])) return;
                    if (output == "1") return;

                    BeforeProcessing();

                    ValueTextBox.Text = output;
                    ValueTextBox.Refresh();
                    ValueTextBox.SelectionStart = ValueTextBox.Text.Length;

                    try
                    {
                        m_factorizer = new Factorizer(this, "factor", output, m_multithreading);
                        if (m_factorizer != null)
                        {
                            m_worker_thread = new Thread(new ThreadStart(m_factorizer.Run));
                            m_worker_thread.Priority = ThreadPriority.Highest;
                            m_worker_thread.IsBackground = false;
                            m_worker_thread.Start();
                        }
                    }
                    catch
                    {
                        Cancel();
                    }
                }
            }
        }
    }

    private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.AcceptButton = null;

        if (MainTabControl.SelectedIndex == 1)      // Index
        {
            IndexTextBox.Focus();
        }
        else if (MainTabControl.SelectedIndex == 2) // Triangle
        {
            this.AcceptButton = CalculateTriangleButton;
            aTriangleTextBox.Focus();
        }
        else if (MainTabControl.SelectedIndex == 3) // Circle
        {
            rCircleTextBox.Focus();
        }
        else if (MainTabControl.SelectedIndex == 4) // Sphere
        {
            rSphereTextBox.Focus();
        }
    }

    private void IndexTextBox_TextChanged(object sender, EventArgs e)
    {
        IndexTextBox.Text = IndexTextBox.Text.Replace(" ", "");

        if (IndexTextBox.Text.EndsWith("11"))
        {
            NthLabel.Text = "th";
        }
        else if (IndexTextBox.Text.EndsWith("12"))
        {
            NthLabel.Text = "th";
        }
        else if (IndexTextBox.Text.EndsWith("13"))
        {
            NthLabel.Text = "th";
        }
        else if (IndexTextBox.Text.EndsWith("1"))
        {
            NthLabel.Text = "st";
        }
        else if (IndexTextBox.Text.EndsWith("2"))
        {
            NthLabel.Text = "nd";
        }
        else if (IndexTextBox.Text.EndsWith("3"))
        {
            NthLabel.Text = "rd";
        }
        else
        {
            NthLabel.Text = "th";
        }

        int index = 0;
        string index_str = IndexTextBox.Text;
        if (int.TryParse(index_str, out index))
        {
            index--;
            if (index >= 0)
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    long p = Numbers.Primes[index];
                    long ap = Numbers.AdditivePrimes[index];
                    long xp = Numbers.NonAdditivePrimes[index];
                    long c = Numbers.Composites[index];
                    long ac = Numbers.AdditiveComposites[index];
                    long xc = Numbers.NonAdditiveComposites[index];

                    PTextBox.Text = p.ToString();
                    APTextBox.Text = ap.ToString();
                    XPTextBox.Text = xp.ToString();
                    CTextBox.Text = c.ToString();
                    ACTextBox.Text = ac.ToString();
                    XCTextBox.Text = xc.ToString();

                    PTextBox.ForeColor = Numbers.GetNumberTypeColor(p);
                    APTextBox.ForeColor = Numbers.GetNumberTypeColor(ap);
                    XPTextBox.ForeColor = Numbers.GetNumberTypeColor(xp);
                    CTextBox.ForeColor = Numbers.GetNumberTypeColor(c);
                    ACTextBox.ForeColor = Numbers.GetNumberTypeColor(ac);
                    XCTextBox.ForeColor = Numbers.GetNumberTypeColor(xc);
                }
                catch
                {
                    PTextBox.Text = "";
                    APTextBox.Text = "";
                    XPTextBox.Text = "";
                    CTextBox.Text = "";
                    ACTextBox.Text = "";
                    XCTextBox.Text = "";
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }

                this.Cursor = Cursors.WaitCursor;
                try
                {
                    long df = Numbers.Deficients[index];
                    long ab = Numbers.Abundants[index];

                    DFTextBox.Text = df.ToString();
                    ABTextBox.Text = ab.ToString();

                    DFTextBox.ForeColor = Numbers.GetNumberTypeColor(df);
                    ABTextBox.ForeColor = Numbers.GetNumberTypeColor(ab);
                }
                catch
                {
                    DFTextBox.Text = "";
                    ABTextBox.Text = "";
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
            else
            {
                PTextBox.Text = "";
                APTextBox.Text = "";
                XPTextBox.Text = "";
                CTextBox.Text = "";
                ACTextBox.Text = "";
                XCTextBox.Text = "";
                DFTextBox.Text = "";
                ABTextBox.Text = "";
            }
        }
        else
        {
            PTextBox.Text = "";
            APTextBox.Text = "";
            XPTextBox.Text = "";
            CTextBox.Text = "";
            ACTextBox.Text = "";
            XCTextBox.Text = "";
            DFTextBox.Text = "";
            ABTextBox.Text = "";
        }
    }
    //private void IndexTextBox_KeyDown(object sender, KeyEventArgs e)
    //{
    //    if (e.KeyCode == Keys.Up)
    //    {
    //        IncrementIndex();
    //    }
    //    else if (e.KeyCode == Keys.Down)
    //    {
    //        DecrementIndex();
    //    }
    //}
    private void IncrementIndex()
    {
        int index = 0;
        if (int.TryParse(IndexTextBox.Text, out index))
        {
            if (index < int.MaxValue) index++;
            IndexTextBox.Text = index.ToString();
        }
    }
    private void DecrementIndex()
    {
        int index = 0;
        if (int.TryParse(IndexTextBox.Text, out index))
        {
            if (index > 0) index--;
            IndexTextBox.Text = index.ToString();
        }
    }
    private void PTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementIndex();
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementIndex();
        }
        else if (e.KeyCode == Keys.Enter)
        {
            long value = 0L;
            if (long.TryParse((sender as TextBox).Text, out value))
            {
                int index = Numbers.PrimeIndexOf(value) + 1;
                IndexTextBox.Text = index.ToString();
            }
        }
    }
    private void APTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementIndex();
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementIndex();
        }
        else if (e.KeyCode == Keys.Enter)
        {
            long value = 0L;
            if (long.TryParse((sender as TextBox).Text, out value))
            {
                int index = Numbers.AdditivePrimeIndexOf(value) + 1;
                IndexTextBox.Text = index.ToString();
            }
        }
    }
    private void XPTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementIndex();
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementIndex();
        }
        else if (e.KeyCode == Keys.Enter)
        {
            long value = 0L;
            if (long.TryParse((sender as TextBox).Text, out value))
            {
                int index = Numbers.NonAdditivePrimeIndexOf(value) + 1;
                IndexTextBox.Text = index.ToString();
            }
        }
    }
    private void CTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementIndex();
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementIndex();
        }
        else if (e.KeyCode == Keys.Enter)
        {
            long value = 0L;
            if (long.TryParse((sender as TextBox).Text, out value))
            {
                int index = Numbers.CompositeIndexOf(value) + 1;
                IndexTextBox.Text = index.ToString();
            }
        }
    }
    private void ACTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementIndex();
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementIndex();
        }
        else if (e.KeyCode == Keys.Enter)
        {
            long value = 0L;
            if (long.TryParse((sender as TextBox).Text, out value))
            {
                int index = Numbers.AdditiveCompositeIndexOf(value) + 1;
                IndexTextBox.Text = index.ToString();
            }
        }
    }
    private void XCTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementIndex();
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementIndex();
        }
        else if (e.KeyCode == Keys.Enter)
        {
            long value = 0L;
            if (long.TryParse((sender as TextBox).Text, out value))
            {
                int index = Numbers.NonAdditiveCompositeIndexOf(value) + 1;
                IndexTextBox.Text = index.ToString();
            }
        }
    }
    private void DFTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementIndex();
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementIndex();
        }
        else if (e.KeyCode == Keys.Enter)
        {
            long value = 0L;
            if (long.TryParse((sender as TextBox).Text, out value))
            {
                int index = Numbers.DeficientIndexOf(value) + 1;
                IndexTextBox.Text = index.ToString();
            }
        }
    }
    private void ABTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementIndex();
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementIndex();
        }
        else if (e.KeyCode == Keys.Enter)
        {
            long value = 0L;
            if (long.TryParse((sender as TextBox).Text, out value))
            {
                int index = Numbers.AbundantIndexOf(value) + 1;
                IndexTextBox.Text = index.ToString();
            }
        }
    }

    private void CircleTextBox_TextChanged(object sender, EventArgs e)
    {
        CircleCalculations(sender);
    }
    private void CircleTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementCircleParameter(sender);
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementCircleParameter(sender);
        }
    }
    private void IncrementCircleParameter(object sender)
    {
        double value = 0.0D;
        if (double.TryParse((sender as TextBox).Text, out value))
        {
            if (value < double.MaxValue) value++;
            (sender as TextBox).Text = value.ToString("0.0");
        }
    }
    private void DecrementCircleParameter(object sender)
    {
        double value = 0.0D;
        if (double.TryParse((sender as TextBox).Text, out value))
        {
            if (value > 0.0D) value--;
            (sender as TextBox).Text = value.ToString("0.0");
        }
    }
    private void CircleCalculations(object sender)
    {
        double r = 0.0D;
        double d = 0.0D;
        double c = 0.0D;
        double a = 0.0D;

        try
        {
            rCircleTextBox.TextChanged -= CircleTextBox_TextChanged;
            dCircleTextBox.TextChanged -= CircleTextBox_TextChanged;
            cCircleTextBox.TextChanged -= CircleTextBox_TextChanged;
            aCircleTextBox.TextChanged -= CircleTextBox_TextChanged;

            if (sender == rCircleTextBox)
            {
                r = double.Parse(rCircleTextBox.Text);
                d = 2.0D * r;
                c = 2.0D * Math.PI * r;
                a = Math.PI * r * r;
                dCircleTextBox.Text = d.ToString("0.0");
                cCircleTextBox.Text = c.ToString("0.0");
                aCircleTextBox.Text = a.ToString("0.0");
            }
            else if (sender == dCircleTextBox)
            {
                d = double.Parse(dCircleTextBox.Text);
                r = 0.5D * d;
                c = 2.0D * Math.PI * r;
                a = Math.PI * r * r;
                rCircleTextBox.Text = r.ToString("0.0");
                cCircleTextBox.Text = c.ToString("0.0");
                aCircleTextBox.Text = a.ToString("0.0");
            }
            else if (sender == cCircleTextBox)
            {
                c = double.Parse(cCircleTextBox.Text);
                r = c / (2.0D * Math.PI);
                d = 2.0D * r;
                a = Math.PI * r * r;
                rCircleTextBox.Text = r.ToString("0.0");
                dCircleTextBox.Text = d.ToString("0.0");
                aCircleTextBox.Text = a.ToString("0.0");
            }
            else if (sender == aCircleTextBox)
            {
                a = double.Parse(aCircleTextBox.Text);
                r = Math.Sqrt(a / Math.PI);
                d = 2.0D * r;
                c = 2.0D * Math.PI * r;
                rCircleTextBox.Text = r.ToString("0.0");
                dCircleTextBox.Text = d.ToString("0.0");
                cCircleTextBox.Text = c.ToString("0.0");
            }
        }
        catch
        {
            // skip error
        }
        finally
        {
            rCircleTextBox.TextChanged += CircleTextBox_TextChanged;
            dCircleTextBox.TextChanged += CircleTextBox_TextChanged;
            cCircleTextBox.TextChanged += CircleTextBox_TextChanged;
            aCircleTextBox.TextChanged += CircleTextBox_TextChanged;
        }
    }

    private void SphereTextBox_TextChanged(object sender, EventArgs e)
    {
        SphereCalculations(sender);
    }
    private void SphereTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementSphereParameter(sender);
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementSphereParameter(sender);
        }
    }
    private void IncrementSphereParameter(object sender)
    {
        double value = 0.0D;
        if (double.TryParse((sender as TextBox).Text, out value))
        {
            if (value < double.MaxValue) value++;
            (sender as TextBox).Text = value.ToString("0.0");
        }
    }
    private void DecrementSphereParameter(object sender)
    {
        double value = 0.0D;
        if (double.TryParse((sender as TextBox).Text, out value))
        {
            if (value > 0.0D) value--;
            (sender as TextBox).Text = value.ToString("0.0");
        }
    }
    private void SphereCalculations(object sender)
    {
        double r = 0.0D;
        double d = 0.0D;
        double s = 0.0D;
        double v = 0.0D;

        try
        {
            rSphereTextBox.TextChanged -= SphereTextBox_TextChanged;
            dSphereTextBox.TextChanged -= SphereTextBox_TextChanged;
            sSphereTextBox.TextChanged -= SphereTextBox_TextChanged;
            vSphereTextBox.TextChanged -= SphereTextBox_TextChanged;

            if (sender == rSphereTextBox)
            {
                r = double.Parse(rSphereTextBox.Text);
                d = 2.0D * r;
                s = 4.0D * Math.PI * r * r;
                v = (4.0D / 3.0D) * Math.PI * r * r * r;
                dSphereTextBox.Text = d.ToString("0.0");
                sSphereTextBox.Text = s.ToString("0.0");
                vSphereTextBox.Text = v.ToString("0.0");
            }
            else if (sender == dSphereTextBox)
            {
                d = double.Parse(dSphereTextBox.Text);
                r = 0.5D * d;
                s = 4.0D * Math.PI * r * r;
                v = (4.0D / 3.0D) * Math.PI * r * r * r;
                rSphereTextBox.Text = r.ToString("0.0");
                sSphereTextBox.Text = s.ToString("0.0");
                vSphereTextBox.Text = v.ToString("0.0");
            }
            else if (sender == sSphereTextBox)
            {
                s = double.Parse(sSphereTextBox.Text);
                r = Math.Sqrt(s / (4.0D * Math.PI));
                d = 2.0D * r;
                v = (4.0D / 3.0D) * Math.PI * r * r * r;
                rSphereTextBox.Text = r.ToString("0.0");
                dSphereTextBox.Text = d.ToString("0.0");
                vSphereTextBox.Text = v.ToString("0.0");
            }
            else if (sender == vSphereTextBox)
            {
                v = double.Parse(vSphereTextBox.Text);
                r = Math.Pow((v / ((4.0D / 3.0D) * Math.PI)), 1.0D / 3.0D);
                d = 2.0D * r;
                s = 4.0D * Math.PI * r * r;
                rSphereTextBox.Text = r.ToString("0.0");
                dSphereTextBox.Text = d.ToString("0.0");
                sSphereTextBox.Text = s.ToString("0.0");
            }
        }
        catch
        {
            // skip error
        }
        finally
        {
            rSphereTextBox.TextChanged += SphereTextBox_TextChanged;
            dSphereTextBox.TextChanged += SphereTextBox_TextChanged;
            sSphereTextBox.TextChanged += SphereTextBox_TextChanged;
            vSphereTextBox.TextChanged += SphereTextBox_TextChanged;
        }
    }

    private void CalculateTriangleButton_Click(object sender, EventArgs e)
    {
        TriangleCalculations(sender);
    }
    private void ClearTriangleButton_Click(object sender, EventArgs e)
    {
        //aTriangleTextBox.Text = "";
        //bTriangleTextBox.Text = "";
        //cTriangleTextBox.Text = "";

        alphaTriangleTextBox.Text = "";
        betaTriangleTextBox.Text = "";
        gammaTriangleTextBox.Text = "";

        pTriangleTextBox.Text = "";
        tTriangleTextBox.Text = "";
        h1TriangleTextBox.Text = "";
        h2TriangleTextBox.Text = "";
        h3TriangleTextBox.Text = "";

        aTriangleTextBox.Focus();
    }
    private void TriangleTextBox_TextChanged(object sender, EventArgs e)
    {
        //TriangleCalculations(sender);
    }
    private void TriangleTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementTriangleParameter(sender);
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementTriangleParameter(sender);
        }
    }
    private void IncrementTriangleParameter(object sender)
    {
        double value = 0.0D;
        if (double.TryParse((sender as TextBox).Text, out value))
        {
            if (value < double.MaxValue) value++;
            value = Math.Floor(value);
            (sender as TextBox).Text = value.ToString("0");
        }
        TriangleCalculations(sender);
    }
    private void DecrementTriangleParameter(object sender)
    {
        double value = 0.0D;
        if (double.TryParse((sender as TextBox).Text, out value))
        {
            if (value > 0.0D) value--;
            value = Math.Floor(value);
            (sender as TextBox).Text = value.ToString("0");
        }
        TriangleCalculations(sender);
    }
    private void AngleTriangleTextBox_TextChanged(object sender, EventArgs e)
    {
        //TriangleCalculations(sender);
    }
    private void AngleTriangleTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementAngleTriangleParameter(sender);
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementAngleTriangleParameter(sender);
        }
    }
    private void AngleTriangleTextBox_Leave(object sender, EventArgs e)
    {
        double value = 0.0D;
        if (double.TryParse((sender as TextBox).Text, out value))
        {
            if (value > 180.0D)
            {
                value = 180.0D;
            }
            else if (value < 0.0D)
            {
                value = 0.0D;
            }
        }
        (sender as TextBox).Text = value.ToString("0");
    }
    private void IncrementAngleTriangleParameter(object sender)
    {
        double value = 0.0D;
        if (double.TryParse((sender as TextBox).Text, out value))
        {
            double alpha = 0.0D;
            double beta = 0.0D;
            double gamma = 0.0D;
            if (alphaTriangleTextBox.Text.Length > 0) alpha = double.Parse(alphaTriangleTextBox.Text) / (180.0D / Math.PI);
            if (betaTriangleTextBox.Text.Length > 0) beta = double.Parse(betaTriangleTextBox.Text) / (180.0D / Math.PI);
            if (gammaTriangleTextBox.Text.Length > 0) gamma = double.Parse(gammaTriangleTextBox.Text) / (180.0D / Math.PI);
            if ((alpha + beta + gamma) < Math.PI)
            {
                if (value > 180.0D)
                {
                    value = 180.0D;
                }
                else if (value < 0.0D)
                {
                    value = -1.0D;
                }
                if (value < 180.0D) value++;
                value = Math.Floor(value);
                (sender as TextBox).Text = value.ToString("0");
            }
        }
    }
    private void DecrementAngleTriangleParameter(object sender)
    {
        double value = 0.0D;
        if (double.TryParse((sender as TextBox).Text, out value))
        {
            if (value > 180.0D)
            {
                value = 180.0D;
            }
            else if (value < 0.0D)
            {
                value = 0.0D;
            }

            if (value > 0.0D) value--;
            value = Math.Floor(value);
            (sender as TextBox).Text = value.ToString("0");
        }
    }
    private void TriangleCalculations(object sender)
    {
        // http://en.wikipedia.org/wiki/Solution_of_triangles

        double a = 0.0D;
        double b = 0.0D;
        double c = 0.0D;
        double alpha = 0.0D;
        double beta = 0.0D;
        double gamma = 0.0D;
        double p = 0.0D;
        double t = 0.0D;
        double h1 = 0.0D;
        double h2 = 0.0D;
        double h3 = 0.0D;

        try
        {
            aTriangleTextBox.TextChanged -= TriangleTextBox_TextChanged;
            bTriangleTextBox.TextChanged -= TriangleTextBox_TextChanged;
            cTriangleTextBox.TextChanged -= TriangleTextBox_TextChanged;
            alphaTriangleTextBox.TextChanged -= AngleTriangleTextBox_TextChanged;
            betaTriangleTextBox.TextChanged -= AngleTriangleTextBox_TextChanged;
            gammaTriangleTextBox.TextChanged -= AngleTriangleTextBox_TextChanged;

            if (aTriangleTextBox.Text.Length > 0) a = double.Parse(aTriangleTextBox.Text);
            if (bTriangleTextBox.Text.Length > 0) b = double.Parse(bTriangleTextBox.Text);
            if (cTriangleTextBox.Text.Length > 0) c = double.Parse(cTriangleTextBox.Text);
            if (alphaTriangleTextBox.Text.Length > 0) alpha = double.Parse(alphaTriangleTextBox.Text) / (180.0D / Math.PI);
            if (betaTriangleTextBox.Text.Length > 0) beta = double.Parse(betaTriangleTextBox.Text) / (180.0D / Math.PI);
            if (gammaTriangleTextBox.Text.Length > 0) gamma = double.Parse(gammaTriangleTextBox.Text) / (180.0D / Math.PI);


            // SSS
            if ((a > 0.0D) && (b > 0.0D) && (c > 0.0D))
            {
                alpha = Math.Acos(((b * b) + (c * c) - (a * a)) / (2 * b * c));
                beta = Math.Acos(((a * a) + (c * c) - (b * b)) / (2 * a * c));
                gamma = Math.Acos(((a * a) + (b * b) - (c * c)) / (2 * a * b));
            }


            // SAS
            else if ((a > 0.0D) && (b > 0.0D) && (gamma > 0.0D))
            {
                c = Math.Sqrt((a * a) + (b * b) - (2 * a * b * Math.Cos((gamma))));
                alpha = Math.Acos(((b * b) + (c * c) - (a * a)) / (2 * b * c));
                beta = Math.Acos(((a * a) + (c * c) - (b * b)) / (2 * a * c));
            }
            else if ((a > 0.0D) && (c > 0.0D) && (beta > 0.0D))
            {
                b = Math.Sqrt((a * a) + (c * c) - (2 * a * c * Math.Cos((beta))));
                alpha = Math.Acos(((b * b) + (c * c) - (a * a)) / (2 * b * c));
                gamma = Math.Acos(((a * a) + (b * b) - (c * c)) / (2 * a * b));
            }
            else if ((b > 0.0D) && (c > 0.0D) && (alpha > 0.0D))
            {
                a = Math.Sqrt((b * b) + (c * c) - (2 * b * c * Math.Cos((alpha))));
                beta = Math.Acos(((a * a) + (c * c) - (b * b)) / (2 * a * c));
                gamma = Math.Acos(((a * a) + (b * b) - (c * c)) / (2 * a * b));
            }


            // SSA
            else if ((a > 0.0D) && (b > 0.0D) && (alpha > 0.0D))
            {
                double D = b * Math.Sin(alpha) / a;
                if (D > 1)
                    beta = double.NaN;
                else if (D == 1)
                    beta = Math.PI / 2.0D;
                else
                    if (a < b)
                        beta = Math.Asin(D);
                    else
                        beta = Math.PI - Math.Asin(D);
                gamma = Math.PI - alpha - beta;
                if (gamma < 0.0D)
                {
                    beta = Math.Asin(D);
                    gamma = Math.PI - alpha - beta;
                }
                c = b * (Math.Sin(gamma) / Math.Sin(beta));
            }
            else if ((a > 0.0D) && (b > 0.0D) && (beta > 0.0D))
            {
                double D = a * Math.Sin(beta) / b;
                if (D > 1)
                    alpha = double.NaN;
                else if (D == 1)
                    alpha = Math.PI / 2.0D;
                else
                    if (a < b)
                        alpha = Math.Asin(D);
                    else
                        alpha = Math.PI - Math.Asin(D);
                gamma = Math.PI - alpha - beta;
                if (gamma < 0.0D)
                {
                    alpha = Math.Asin(D);
                    gamma = Math.PI - alpha - beta;
                }
                c = b * (Math.Sin(gamma) / Math.Sin(beta));
            }
            else if ((a > 0.0D) && (c > 0.0D) && (alpha > 0.0D))
            {
                double D = c * Math.Sin(alpha) / a;
                if (D > 1)
                    gamma = double.NaN;
                else if (D == 1)
                    gamma = Math.PI / 2.0D;
                else
                    if (a < b)
                        gamma = Math.Asin(D);
                    else
                        gamma = Math.PI - Math.Asin(D);
                beta = Math.PI - alpha - gamma;
                if (beta < 0.0D)
                {
                    gamma = Math.Asin(D);
                    beta = Math.PI - alpha - gamma;
                }
                b = a * (Math.Sin(beta) / Math.Sin(alpha));
            }
            else if ((a > 0.0D) && (c > 0.0D) && (gamma > 0.0D))
            {
                double D = a * Math.Sin(gamma) / c;
                if (D > 1)
                    alpha = double.NaN;
                else if (D == 1)
                    alpha = Math.PI / 2.0D;
                else
                    if (a < b)
                        alpha = Math.Asin(D);
                    else
                        alpha = Math.PI - Math.Asin(D);
                beta = Math.PI - alpha - gamma;
                if (beta < 0.0D)
                {
                    alpha = Math.Asin(D);
                    beta = Math.PI - alpha - gamma;
                }
                b = c * (Math.Sin(beta) / Math.Sin(gamma));
            }
            else if ((b > 0.0D) && (c > 0.0D) && (beta > 0.0D))
            {
                double D = c * Math.Sin(beta) / b;
                if (D > 1)
                    gamma = double.NaN;
                else if (D == 1)
                    gamma = Math.PI / 2.0D;
                else
                    if (a < b)
                        gamma = Math.Asin(D);
                    else
                        gamma = Math.PI - Math.Asin(D);
                alpha = Math.PI - beta - gamma;
                if (alpha < 0.0D)
                {
                    gamma = Math.Asin(D);
                    alpha = Math.PI - beta - gamma;
                }
                a = b * (Math.Sin(alpha) / Math.Sin(beta));
            }
            else if ((b > 0.0D) && (c > 0.0D) && (gamma > 0.0D))
            {
                double D = b * Math.Sin(gamma) / c;
                if (D > 1)
                    beta = double.NaN;
                else if (D == 1)
                    beta = Math.PI / 2.0D;
                else
                    if (a < b)
                        beta = Math.Asin(D);
                    else
                        beta = Math.PI - Math.Asin(D);
                alpha = Math.PI - beta - gamma;
                if (alpha < 0.0D)
                {
                    beta = Math.Asin(D);
                    alpha = Math.PI - beta - gamma;
                }
                a = c * (Math.Sin(alpha) / Math.Sin(gamma));
            }


            // ASA
            else if ((a > 0.0D) && (beta > 0.0D) && (gamma > 0.0D))
            {
                alpha = Math.PI - beta - gamma;
                b = a * (Math.Sin(beta) / Math.Sin(alpha));
                c = a * (Math.Sin(gamma) / Math.Sin(alpha));
            }
            else if ((b > 0.0D) && (alpha > 0.0D) && (gamma > 0.0D))
            {
                beta = Math.PI - alpha - gamma;
                a = b * (Math.Sin(alpha) / Math.Sin(beta));
                c = b * (Math.Sin(gamma) / Math.Sin(beta));
            }
            else if ((c > 0.0D) && (alpha > 0.0D) && (beta > 0.0D))
            {
                gamma = Math.PI - alpha - beta;
                a = c * (Math.Sin(alpha) / Math.Sin(gamma));
                b = c * (Math.Sin(beta) / Math.Sin(gamma));
            }


            // SAA
            else if ((a > 0.0D) && (alpha > 0.0D) && (beta > 0.0D))
            {
                gamma = Math.PI - alpha - beta;
                b = a * (Math.Sin(beta) / Math.Sin(alpha));
                c = a * (Math.Sin(gamma) / Math.Sin(alpha));
            }
            else if ((a > 0.0D) && (alpha > 0.0D) && (gamma > 0.0D))
            {
                beta = Math.PI - alpha - gamma;
                b = a * (Math.Sin(beta) / Math.Sin(alpha));
                c = a * (Math.Sin(gamma) / Math.Sin(alpha));
            }
            else if ((b > 0.0D) && (beta > 0.0D) && (alpha > 0.0D))
            {
                gamma = Math.PI - alpha - beta;
                a = b * (Math.Sin(alpha) / Math.Sin(beta));
                c = b * (Math.Sin(gamma) / Math.Sin(beta));
            }
            else if ((b > 0.0D) && (beta > 0.0D) && (gamma > 0.0D))
            {
                alpha = Math.PI - beta - gamma;
                a = b * (Math.Sin(alpha) / Math.Sin(beta));
                c = b * (Math.Sin(gamma) / Math.Sin(beta));
            }
            else if ((c > 0.0D) && (gamma > 0.0D) && (alpha > 0.0D))
            {
                beta = Math.PI - alpha - gamma;
                a = c * (Math.Sin(alpha) / Math.Sin(gamma));
                b = c * (Math.Sin(beta) / Math.Sin(gamma));
            }
            else if ((c > 0.0D) && (gamma > 0.0D) && (beta > 0.0D))
            {
                alpha = Math.PI - beta - gamma;
                a = c * (Math.Sin(alpha) / Math.Sin(gamma));
                b = c * (Math.Sin(beta) / Math.Sin(gamma));
            }


            // AAA
            else if ((alpha > 0.0D) && (beta > 0.0D) && (gamma > 0.0D))
            {
                if ((a == 0.0D) && (b == 0.0D) && (c == 0.0D))
                {
                    a = 1; // The Unit
                    b = a * (Math.Sin(beta) / Math.Sin(alpha));
                    c = b * (Math.Sin(gamma) / Math.Sin(beta));
                }
                else if (a > 0.0D)
                {
                    b = a * (Math.Sin(beta) / Math.Sin(alpha));
                    c = b * (Math.Sin(gamma) / Math.Sin(beta));
                }
                else if (b > 0.0D)
                {
                    a = b * (Math.Sin(alpha) / Math.Sin(beta));
                    c = b * (Math.Sin(gamma) / Math.Sin(beta));
                }
                else if (c > 0.0D)
                {
                    a = c * (Math.Sin(alpha) / Math.Sin(gamma));
                    b = a * (Math.Sin(beta) / Math.Sin(alpha));
                }
            }



            p = a + b + c;
            t = 0.25D * Math.Sqrt((a + b + c) * (-a + b + c) * (a - b + c) * (a + b - c));
            h1 = 2 * t / a;
            h2 = 2 * t / b;
            h3 = 2 * t / c;

            aTriangleTextBox.Text = a.ToString("0.0");
            bTriangleTextBox.Text = b.ToString("0.0");
            cTriangleTextBox.Text = c.ToString("0.0");
            alphaTriangleTextBox.Text = (alpha * (180.0D / Math.PI)).ToString("0.0");
            betaTriangleTextBox.Text = (beta * (180.0D / Math.PI)).ToString("0.0");
            gammaTriangleTextBox.Text = (gamma * (180.0D / Math.PI)).ToString("0.0");
            pTriangleTextBox.Text = p.ToString("0.0");
            tTriangleTextBox.Text = t.ToString("0.0");
            h1TriangleTextBox.Text = h1.ToString("0.0");
            h2TriangleTextBox.Text = h2.ToString("0.0");
            h3TriangleTextBox.Text = h3.ToString("0.0");
        }
        catch
        {
            // skip error
        }
        finally
        {
            aTriangleTextBox.TextChanged += TriangleTextBox_TextChanged;
            bTriangleTextBox.TextChanged += TriangleTextBox_TextChanged;
            cTriangleTextBox.TextChanged += TriangleTextBox_TextChanged;
            alphaTriangleTextBox.TextChanged += AngleTriangleTextBox_TextChanged;
            betaTriangleTextBox.TextChanged += AngleTriangleTextBox_TextChanged;
            gammaTriangleTextBox.TextChanged += AngleTriangleTextBox_TextChanged;
        }
    }

    private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            // toggle visible
            this.Visible = true;

            // restore if minimized
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            // and bring to foreground
            this.Activate();
        }
    }
    private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        //if (e.Button == MouseButtons.Left)
        //{
        //    // make visible (in case it is hidden)
        //    this.Visible = true;

        //    // toggle maximized
        //    if (this.WindowState == FormWindowState.Maximized)
        //    {
        //        this.WindowState = FormWindowState.Normal;
        //    }
        //    else
        //    {
        //        this.WindowState = FormWindowState.Maximized;
        //    }

        //    // and bring to foreground
        //    this.Activate();
        //}
    }
    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MessageBox.Show
        (
            Application.ProductName + "  v" + Application.ProductVersion + "\r\n" +
            "©2009-2019 Ali Adams - علي عبد الرزاق عبد الكريم القره غولي" + "\r\n" + "\r\n" +
            "Powered by Yet Another Factoring Utility\r\nBenjamin Buhrow <bbuhrow@gmail.com>" + "\r\n" + "\r\n" +
            "God >",
            "About",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1
        );
    }
    private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CloseApplication();

        Application.Exit();
        System.Environment.Exit(0);
    }
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
