using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;

public partial class MainForm : Form
{
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

    private static int ROWS = 30;
    private static int COLS = 19;
    private TextBox[,] controls = new TextBox[ROWS, COLS];

    private float m_dpi = 0F;
    public MainForm()
    {
        using (Graphics graphics = this.CreateGraphics())
        {
            m_dpi = graphics.DpiX;    // 100% = 96.0F,   125% = 120.0F,   150% = 144.0F
        }
        InitializeComponent();
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

        // Copy label
        {
            Label control = new Label();
            if (control != null)
            {
                control.Width = 14;
                control.Height = 12;
                control.Top = -3;
                control.Left = 0;
                control.Cursor = Cursors.Hand;
                control.TextAlign = ContentAlignment.TopLeft;
                control.Font = new Font("Arial", 10);
                control.Text = "˄";
                control.MouseEnter += Label_MouseEnter;
                control.MouseLeave += Label_MouseLeave;
                control.Click += CopyLabel_Click;
                ToolTip.SetToolTip(control, "Copy");
                MainPanel.Controls.Add(control);
            }
        }

        // Paste label
        {
            Label control = new Label();
            if (control != null)
            {
                control.Width = 14;
                control.Height = 12;
                control.Top = 8;
                control.Left = 0;
                control.Cursor = Cursors.Hand;
                control.TextAlign = ContentAlignment.TopLeft;
                control.Font = new Font("Arial", 10);
                control.Text = "˅";
                control.MouseEnter += Label_MouseEnter;
                control.MouseLeave += Label_MouseLeave;
                control.Click += PasteLabel_Click;
                ToolTip.SetToolTip(control, "Paste");
                MainPanel.Controls.Add(control);
            }
        }

        // Row numbers (DeleteRow)
        for (int i = 0; i < ROWS; i++)
        {
            Label control = new Label();
            if (control != null)
            {
                control.Width = (m_dpi == 96.0F) ? 20 : 24;
                control.Height = 19;
                control.Top = ((m_dpi == 96.0F) ? 23 : 27) + (i * (control.Height + ((m_dpi == 96.0F) ? 1 : 3)));
                control.Left = 0;
                control.TextAlign = ContentAlignment.MiddleRight;
                control.Font = new Font("Arial", 8);
                control.Text = (i + 1).ToString();
                ToolTip.SetToolTip(control, "Delete, Control to Clear");
                control.Cursor = Cursors.PanEast;
                control.Click += DeleteRowLabel_Click;
                MainPanel.Controls.Add(control);
            }
        }

        // Column headings
        for (int j = 0; j < COLS; j++)
        {
            Label control = new Label();
            if (control != null)
            {
                control.Width = (m_dpi == 96.0F) ? 53 : 65;
                control.Height = 19;
                control.Top = 0;
                control.Left = 19 + (j * control.Width + 2);
                control.TextAlign = ContentAlignment.MiddleCenter;
                control.Font = new Font("Arial", 8);
                MainPanel.Controls.Add(control);

                switch (j)
                {
                    case 0: { control.Text = "i"; ToolTip.SetToolTip(control, "\tIndex\r\nAuto-fill, Shift to go back"); control.Click += IndexLabel_Click; control.Cursor = Cursors.PanSouth; break; }
                    case 1: { control.Text = "P"; ToolTip.SetToolTip(control, "Prime"); break; }
                    case 2: { control.Text = "AP"; ToolTip.SetToolTip(control, "Additive Prime"); break; }
                    case 3: { control.Text = "XP"; ToolTip.SetToolTip(control, "Non-additive Prime"); break; }
                    case 4: { control.Text = "C"; ToolTip.SetToolTip(control, "Composite"); break; }
                    case 5: { control.Text = "AC"; ToolTip.SetToolTip(control, "Additive Composite"); break; }
                    case 6: { control.Text = "XC"; ToolTip.SetToolTip(control, "Non-additive Composite"); break; }
                    case 7: { control.Text = "DF"; ToolTip.SetToolTip(control, "Deficient Number"); break; }
                    case 8: { control.Text = "AB"; ToolTip.SetToolTip(control, "Abundant Number"); break; }
                    case 9: { control.Text = "P=4n+1"; ToolTip.SetToolTip(control, "n of Fermat's 4n+1 Prime = a^2 + b^2"); break; }
                    case 10: { control.Text = "a"; ToolTip.SetToolTip(control, "a of 4n+1 Prime = a^2 + b^2"); break; }
                    case 11: { control.Text = "b"; ToolTip.SetToolTip(control, "b of 4n+1 Prime = a^2 + b^2"); break; }
                    case 12: { control.Text = "C=4n+1"; ToolTip.SetToolTip(control, "n of Ali Adams' 4n+1 Composite = a^2 - b^2"); break; }
                    case 13: { control.Text = "a"; ToolTip.SetToolTip(control, "a of 4n+1 Composite = a^2 - b^2"); break; }
                    case 14: { control.Text = "b"; ToolTip.SetToolTip(control, "b of 4n+1 Composite = a^2 - b^2"); break; }
                    case 15: { control.Text = "Sum"; ToolTip.SetToolTip(control, "Sum(1..i) = (i*(i+1))/2"); break; }
                    case 16: { control.Text = "Half"; ToolTip.SetToolTip(control, "i/2"); break; }
                    case 17: { control.Text = "Median"; ToolTip.SetToolTip(control, "Median(1..i/2) = ((i/2)+1)/2"); break; }
                    case 18: { control.Text = "Product"; ToolTip.SetToolTip(control, "Half * Median"); break; }
                    default: break;
                }
            }
        }

        // TextBox cells
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                TextBox control = new TextBox();
                if (control != null)
                {
                    control.Width = (m_dpi == 96.0F) ? 53 : 65;
                    control.Height = 21;
                    control.Top = 19 + (i * control.Height + 1);
                    control.Left = 19 + (j * control.Width + 2);
                    control.TextAlign = HorizontalAlignment.Center;
                    control.Font = new Font("Arial", 11);
                    control.MaxLength = 7;
                    if (j >= 9) control.ReadOnly = true;
                    MainPanel.Controls.Add(control);

                    control.KeyPress += FixMicrosoft;
                    if (j == 0) control.TextChanged += TextBox_TextChanged;
                    control.KeyDown += TextBox_KeyDown;

                    if (j == 0) control.AllowDrop = true;
                    if (j == 0) control.MouseDown += Control_MouseDown;
                    if (j == 0) control.DragEnter += Control_DragEnter;
                    if (j == 0) control.DragDrop += Control_DragDrop;
                    control.MouseHover += Control_MouseHover;

                    switch (j)
                    {
                        case 0:
                            control.BackColor = Color.Snow;
                            break;
                        case 1:
                        case 2:
                        case 3:
                            control.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[3];
                            break;
                        case 4:
                        case 5:
                        case 6:
                            control.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[6];
                            break;
                        case 7:
                            control.BackColor = Numbers.NUMBER_KIND_COLORS[0];
                            break;
                        case 8:
                            control.BackColor = Numbers.NUMBER_KIND_COLORS[2];
                            break;
                        case 9:
                        case 10:
                        case 11:
                            control.BackColor = Color.FromArgb(240, 240, 255);
                            break;
                        case 12:
                        case 13:
                        case 14:
                            control.BackColor = Color.FromArgb(255, 240, 240);
                            break;
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                            control.BackColor = Color.Snow;
                            break;
                        default:
                            control.BackColor = SystemColors.Window;
                            break;
                    }

                    controls[i, j] = control;
                }
            }
        }

        AboutToolStripMenuItem.Font = new Font(AboutToolStripMenuItem.Font, AboutToolStripMenuItem.Font.Style | FontStyle.Bold);

        m_filename = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", ".ini");
        LoadSettings();
    }

    private void DeleteRowLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            ClearLabel_Click(sender, e);
        }
        else
        {
            Control control = (sender as Label);
            if (control != null)
            {
                int i = int.Parse(control.Text) - 1;
                if (controls != null)
                {
                    for (int j = 0; j < COLS; j++)
                    {
                        controls[i, j].Text = "";
                    }
                    controls[i, 0].Focus();
                }
            }
        }
    }
    private void ClearLabel_Click(object sender, EventArgs e)
    {
        if (controls != null)
        {
            for (int i = 0; i < ROWS; i++)
            {
                controls[i, 0].Text = "";
            }
            controls[0, 0].Focus();
        }
    }
    private void Label_MouseEnter(object sender, EventArgs e)
    {
        Control control = sender as Label;
        if (control != null)
        {
            control.BackColor = SystemColors.Info;
        }
    }
    private void Label_MouseLeave(object sender, EventArgs e)
    {
        Control control = sender as Label;
        if (control != null)
        {
            control.BackColor = SystemColors.Control;
        }
    }
    private void CopyLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                string filename = Globals.NUMBERS_FOLDER + "/" + DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" + "Numbers.txt";

                StringBuilder str = new StringBuilder();
                str.AppendLine("-------------------------------------------------------------------------------------------------------------------------------------------------------");
                str.AppendLine("i" + "\t" + "P" + "\t" + "AP" + "\t" + "XP" + "\t" + "C" + "\t" + "AC" + "\t" + "XC" + "\t" + "DF" + "\t" + "AB" + "\t" + "P=4n+1" + "\t" + "a" + "\t" + "b" + "\t" + "C=4n+1" + "\t" + "a" + "\t" + "b" + "\t" + "Sum" + "\t" + "Half" + "\t" + "Median" + "\t" + "Product");
                str.AppendLine("-------------------------------------------------------------------------------------------------------------------------------------------------------");
                for (int i = 0; i < ROWS; i++)
                {
                    for (int j = 0; j < COLS; j++)
                    {
                        str.Append(controls[i, j].Text + "\t");
                    }
                    if (str.Length > 0)
                    {
                        str.Remove(str.Length - 1, 1);
                    }
                    str.AppendLine();
                }
                str.AppendLine("-------------------------------------------------------------------------------------------------------------------------------------------------------");

                FileHelper.SaveText(filename, str.ToString());
                FileHelper.DisplayFile(filename);
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private void PasteLabel_Click(object sender, EventArgs e)
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            string text = Clipboard.GetText();
            string[] lines = text.Split('\n');

            char[] separators = { ' ', '.', ',', ';', '\t', 't', 's', 'n' };
            List<int> numbers = new List<int>();
            foreach (string line in lines)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    int number = 0;
                    //if (Char.IsDigit(line[0])) // 0..9
                    //{
                    string[] parts = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    if (int.TryParse(parts[0], out number))
                    {
                        numbers.Add(number);
                    }
                    else
                    {
                        numbers.Add(0);
                    }
                    //}
                    //else
                    //{
                    //    numbers.Add(0);
                    //}
                }
                else
                {
                    numbers.Add(0);
                }
            }

            if (numbers.Count <= ROWS)
            {
                for (int i = 0; i < numbers.Count; i++)
                {
                    if (numbers[i] > 0)
                    {
                        controls[i, 0].Text = numbers[i].ToString();
                    }
                    else
                    {
                        controls[i, 0].Text = "";
                    }
                }
                controls[0, 0].Focus();
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
    private int batch_number = -1;
    private void IndexLabel_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            if (batch_number > 0)
            {
                batch_number--;
            }
            else
            {
                batch_number = -1;
                ClearLabel_Click(sender, e);
                return;
            }
        }
        else
        {
            batch_number++;
        }

        if (controls != null)
        {
            for (int i = 0; i < ROWS; i++)
            {
                controls[i, 0].Text = ((i + 1) + (batch_number * ROWS)).ToString();
            }
            controls[0, 0].Focus();
        }
    }

    private string DataFormatName = Application.ProductName;
    private void Control_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            Control source = (sender as Control);
            if (source != null)
            {
                DataObject data = new DataObject(DataFormatName, source);
                source.DoDragDrop(data, DragDropEffects.Move);
            }
        }
    }
    private void Control_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormatName))
            e.Effect = e.AllowedEffect;
        else
            e.Effect = DragDropEffects.None;
    }
    private void Control_DragDrop(object sender, DragEventArgs e)
    {
        Control target = (sender as Control);
        if (target != null)
        {
            Control source = (Control)e.Data.GetData(DataFormatName);
            if (source != null)
            {
                string temp = target.Text;
                target.Text = source.Text;
                source.Text = temp;
            }
        }
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
        this.Width = (m_dpi == 96.0F) ? 1053 : 1281;
        this.Height = (m_dpi == 96.0F) ? 681 : 752;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
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
    }
    private void MainForm_Shown(object sender, EventArgs e)
    {
        NotifyIcon.Visible = true;
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
        // remove icon from tray
        if (NotifyIcon != null)
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Dispose();
        }

        SaveSettings();
    }

    private void IncrementIndex(object sender)
    {
        Point point = GetControlLocation(sender);
        TextBox index_control = controls[point.X, 0];
        if (index_control != null)
        {
            index_control.Text = index_control.Text.Replace(" ", "");
            if (index_control.Text.Length == 0)
            {
                index_control.Text = "0";
                index_control.Refresh();
            }

            int index = 0;
            if (int.TryParse(index_control.Text, out index))
            {
                if (index < int.MaxValue) index++;
                index_control.Text = index.ToString();
                index_control.Refresh();
            }
        }
    }
    private void DecrementIndex(object sender)
    {
        Point point = GetControlLocation(sender);
        TextBox index_control = controls[point.X, 0];
        if (index_control != null)
        {
            index_control.Text = index_control.Text.Replace(" ", "");
            if (index_control.Text.Length == 0)
            {
                index_control.Text = "0";
                index_control.Refresh();
            }

            int index = 0;
            if (int.TryParse(index_control.Text, out index))
            {
                if (index > 1) index--;
                index_control.Text = index.ToString();
                index_control.Refresh();
            }
        }
    }
    private Point GetControlLocation(object sender)
    {
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                if (sender == controls[i, j])
                {
                    return new Point(i, j);
                }
            }
        }
        return new Point(-1, -1);
    }
    private void TextBox_TextChanged(object sender, EventArgs e)
    {
        Point point = GetControlLocation(sender);
        TextBox index_control = controls[point.X, 0];
        if (index_control != null)
        {
            index_control.Text = index_control.Text.Replace(" ", "");

            int index = 0;
            if (int.TryParse(index_control.Text, out index))
            {
                index_control.ForeColor = Numbers.GetNumberTypeColor(index);

                if (index >= 0)
                {
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        long p = Numbers.Primes[index - 1];
                        long ap = Numbers.AdditivePrimes[index - 1];
                        long xp = Numbers.NonAdditivePrimes[index - 1];
                        long c = Numbers.Composites[index - 1];
                        long ac = Numbers.AdditiveComposites[index - 1];
                        long xc = Numbers.NonAdditiveComposites[index - 1];
                        controls[point.X, 1].Text = p.ToString();
                        controls[point.X, 2].Text = ap.ToString();
                        controls[point.X, 3].Text = xp.ToString();
                        controls[point.X, 4].Text = c.ToString();
                        controls[point.X, 5].Text = ac.ToString();
                        controls[point.X, 6].Text = xc.ToString();
                        controls[point.X, 1].ForeColor = Numbers.GetNumberTypeColor(p);
                        controls[point.X, 2].ForeColor = Numbers.GetNumberTypeColor(ap);
                        controls[point.X, 3].ForeColor = Numbers.GetNumberTypeColor(xp);
                        controls[point.X, 4].ForeColor = Numbers.GetNumberTypeColor(c);
                        controls[point.X, 5].ForeColor = Numbers.GetNumberTypeColor(ac);
                        controls[point.X, 6].ForeColor = Numbers.GetNumberTypeColor(xc);

                        if (((p - 1) % 4) == 0)  // 4n+1 Prime
                        {
                            long n = (p - 1) / 4;
                            long a;
                            long b;
                            Numbers.GetTwoSquaresInSum(p, out a, out b);
                            a = (long)Math.Sqrt(a);
                            b = (long)Math.Sqrt(b);
                            controls[point.X, 9].Text = n.ToString();
                            controls[point.X, 10].Text = a.ToString();
                            controls[point.X, 11].Text = b.ToString();
                            controls[point.X, 9].ForeColor = Numbers.GetNumberTypeColor(n);
                            controls[point.X, 10].ForeColor = Numbers.GetNumberTypeColor(a);
                            controls[point.X, 11].ForeColor = Numbers.GetNumberTypeColor(b);
                        }
                        else
                        {
                            controls[point.X, 9].Text = "";
                            controls[point.X, 10].Text = "";
                            controls[point.X, 11].Text = "";
                        }

                        if (((c - 1) % 4) == 0)  // 4n+1 Composite
                        {
                            long n = (c - 1) / 4;
                            long a;
                            long b;
                            Numbers.GetTwoSquaresInDiff(c, out a, out b);
                            a = (long)Math.Sqrt(a);
                            b = (long)Math.Sqrt(b);
                            controls[point.X, 12].Text = n.ToString();
                            controls[point.X, 13].Text = a.ToString();
                            controls[point.X, 14].Text = b.ToString();
                            controls[point.X, 12].ForeColor = Numbers.GetNumberTypeColor(n);
                            controls[point.X, 13].ForeColor = Numbers.GetNumberTypeColor(a);
                            controls[point.X, 14].ForeColor = Numbers.GetNumberTypeColor(b);
                        }
                        else
                        {
                            controls[point.X, 12].Text = "";
                            controls[point.X, 13].Text = "";
                            controls[point.X, 14].Text = "";
                        }

                        int sum = (index * (index + 1)) / 2;
                        int half = ((index % 2) == 0) ? index / 2 : 0;
                        int median = ((half % 2) == 1) ? (half + 1) / 2 : 0;
                        int product = half * median;
                        controls[point.X, 15].Text = sum.ToString();
                        controls[point.X, 16].Text = (half > 0) ? half.ToString() : "";
                        controls[point.X, 17].Text = (median > 0) ? median.ToString() : "";
                        controls[point.X, 18].Text = (product > 0) ? product.ToString() : "";
                        controls[point.X, 15].ForeColor = Numbers.GetNumberTypeColor(sum);
                        controls[point.X, 16].ForeColor = Numbers.GetNumberTypeColor(half);
                        controls[point.X, 17].ForeColor = Numbers.GetNumberTypeColor(median);
                        controls[point.X, 18].ForeColor = Numbers.GetNumberTypeColor(product);
                    }
                    catch
                    {
                        controls[point.X, 1].Text = "";
                        controls[point.X, 2].Text = "";
                        controls[point.X, 3].Text = "";
                        controls[point.X, 4].Text = "";
                        controls[point.X, 5].Text = "";
                        controls[point.X, 6].Text = "";
                        controls[point.X, 11].Text = "";
                        controls[point.X, 12].Text = "";
                        controls[point.X, 13].Text = "";
                        controls[point.X, 14].Text = "";
                        controls[point.X, 15].Text = "";
                        controls[point.X, 16].Text = "";
                        controls[point.X, 17].Text = "";
                        controls[point.X, 18].Text = "";
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }

                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        long df = Numbers.Deficients[index - 1];
                        long ab = Numbers.Abundants[index - 1];
                        controls[point.X, 7].Text = df.ToString();
                        controls[point.X, 8].Text = ab.ToString();
                        controls[point.X, 7].ForeColor = Numbers.GetNumberTypeColor(df);
                        controls[point.X, 8].ForeColor = Numbers.GetNumberTypeColor(ab);
                    }
                    catch
                    {
                        controls[point.X, 7].Text = "";
                        controls[point.X, 8].Text = "";
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
                else
                {
                    controls[point.X, 1].Text = "";
                    controls[point.X, 2].Text = "";
                    controls[point.X, 3].Text = "";
                    controls[point.X, 4].Text = "";
                    controls[point.X, 5].Text = "";
                    controls[point.X, 6].Text = "";
                    controls[point.X, 7].Text = "";
                    controls[point.X, 8].Text = "";
                    controls[point.X, 9].Text = "";
                    controls[point.X, 10].Text = "";
                    controls[point.X, 11].Text = "";
                    controls[point.X, 12].Text = "";
                    controls[point.X, 13].Text = "";
                    controls[point.X, 14].Text = "";
                    controls[point.X, 15].Text = "";
                    controls[point.X, 16].Text = "";
                    controls[point.X, 17].Text = "";
                    controls[point.X, 18].Text = "";
                }
            }
            else
            {
                controls[point.X, 1].Text = "";
                controls[point.X, 2].Text = "";
                controls[point.X, 3].Text = "";
                controls[point.X, 4].Text = "";
                controls[point.X, 5].Text = "";
                controls[point.X, 6].Text = "";
                controls[point.X, 7].Text = "";
                controls[point.X, 8].Text = "";
                controls[point.X, 9].Text = "";
                controls[point.X, 10].Text = "";
                controls[point.X, 11].Text = "";
                controls[point.X, 12].Text = "";
                controls[point.X, 13].Text = "";
                controls[point.X, 14].Text = "";
                controls[point.X, 15].Text = "";
                controls[point.X, 16].Text = "";
                controls[point.X, 17].Text = "";
                controls[point.X, 18].Text = "";
            }
        }
    }
    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up)
        {
            IncrementIndex(sender);
        }
        else if (e.KeyCode == Keys.Down)
        {
            DecrementIndex(sender);
        }
        else if (e.KeyCode == Keys.Enter)
        {
            Point point = GetControlLocation(sender);
            if (point.Y > 0)
            {
                Control index_control = controls[point.X, 0] as TextBox;
                if (index_control != null)
                {
                    long value = 0L;
                    if (long.TryParse((sender as TextBox).Text, out value))
                    {
                        int index = 0;
                        switch (point.Y)
                        {
                            case 1:
                                index = Numbers.PrimeIndexOf(value) + 1;
                                break;
                            case 2:
                                index = Numbers.AdditivePrimeIndexOf(value) + 1;
                                break;
                            case 3:
                                index = Numbers.NonAdditivePrimeIndexOf(value) + 1;
                                break;
                            case 4:
                                index = Numbers.CompositeIndexOf(value) + 1;
                                break;
                            case 5:
                                index = Numbers.AdditiveCompositeIndexOf(value) + 1;
                                break;
                            case 6:
                                index = Numbers.NonAdditiveCompositeIndexOf(value) + 1;
                                break;
                            case 7:
                                index = Numbers.DeficientIndexOf(value) + 1;
                                break;
                            case 8:
                                index = Numbers.AbundantIndexOf(value) + 1;
                                break;
                            default:
                                index = 0;
                                break;
                        }
                        index_control.Text = index.ToString();

                        if (index == 0)
                        {
                            (sender as TextBox).Text = "";
                            index_control.Text = "";
                        }
                    }
                }
            }
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
        if (ModifierKeys == Keys.Control)
        {
            GenerateSomeRows();
        }
        else
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
    private void GenerateSomeRows()
    {
        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (Directory.Exists(Globals.NUMBERS_FOLDER))
            {
                string filename = Globals.NUMBERS_FOLDER + "/" + DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + "_" + "Numbers.txt";

                StringBuilder str = new StringBuilder();
                str.AppendLine("-------------------------------------------------------------------------------------------------------------------------------------------------------");
                str.AppendLine("i" + "\t" + "P" + "\t" + "AP" + "\t" + "XP" + "\t" + "C" + "\t" + "AC" + "\t" + "XC" + "\t" + "DF" + "\t" + "AB" + "\t" + "P=4n+1" + "\t" + "a" + "\t" + "b" + "\t" + "C=4n+1" + "\t" + "a" + "\t" + "b" + "\t" + "Sum" + "\t" + "Half" + "\t" + "Median" + "\t" + "Product");
                str.AppendLine("-------------------------------------------------------------------------------------------------------------------------------------------------------");

                if (controls != null)
                {
                    for (int n = 0; n < 1005; n++)
                    {
                        for (int i = 0; i < ROWS; i++)
                        {
                            controls[i, 0].Text = ((i + 1) + (n * ROWS)).ToString();
                        }

                        for (int i = 0; i < ROWS; i++)
                        {
                            for (int j = 0; j < COLS; j++)
                            {
                                str.Append(controls[i, j].Text + "\t");
                            }
                            if (str.Length > 0)
                            {
                                str.Remove(str.Length - 1, 1);
                            }
                            str.AppendLine();
                        }
                    }
                }

                str.AppendLine("-------------------------------------------------------------------------------------------------------------------------------------------------------");
                FileHelper.SaveText(filename, str.ToString());
                FileHelper.DisplayFile(filename);
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }
}
