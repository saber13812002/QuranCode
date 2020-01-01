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
    private static int COLS = 17;
    private TextBox[,] controls = new TextBox[ROWS, COLS];

    private string m_filename = null;
    private void Initialize()
    {
        this.Top = Screen.PrimaryScreen.WorkingArea.Top;
        this.Left = Screen.PrimaryScreen.WorkingArea.Left;
        this.Width = (m_dpi == 96.0F) ? 1126 : 1281;
        this.Height = (m_dpi == 96.0F) ? 681 : 752;
    }
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
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Initialize();
                }
            }
        }
        else // first start
        {
            Initialize();
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
                control.Width = 21;
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
                control.Width = 21;
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
                ToolTip.SetToolTip(control, "Delete, Ctrl to Clear");
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
                control.Width = (m_dpi == 96.0F) ? 64 : 76;
                control.Height = 19;
                control.Top = 0;
                control.Left = 19 + (j * control.Width + 2);
                control.TextAlign = ContentAlignment.MiddleCenter;
                control.Font = new Font("Arial", 8);
                control.Click += Label_Click;
                control.Cursor = Cursors.Hand;
                MainPanel.Controls.Add(control);

                switch (j)
                {
                    case 0: { control.Text = "N"; ToolTip.SetToolTip(control, "\tNumber\r\nAuto-fill, Shift to go back"); control.Click += IndexLabel_Click; control.Cursor = Cursors.PanSouth; break; }
                    case 1: { control.Text = "P"; ToolTip.SetToolTip(control, "Prime index"); break; }
                    case 2: { control.Text = "AP"; ToolTip.SetToolTip(control, "Additive Prime index"); break; }
                    case 3: { control.Text = "XP"; ToolTip.SetToolTip(control, "Non-additive Prime index"); break; }
                    case 4: { control.Text = "AB"; ToolTip.SetToolTip(control, "Abundant index"); break; }
                    case 5: { control.Text = "4n+1"; ToolTip.SetToolTip(control, "4n+1 Prime index"); break; }
                    case 6: { control.Text = "-gon"; ToolTip.SetToolTip(control, "Polygon index"); break; }
                    case 7: { control.Text = "-cgon"; ToolTip.SetToolTip(control, "Centered Polygon index"); break; }
                    case 8: { control.Text = "-pyramid"; ToolTip.SetToolTip(control, "Pyramid index"); break; }
                    case 9: { control.Text = "4-platonic"; ToolTip.SetToolTip(control, "4-faces platonic solid index"); break; }
                    case 10: { control.Text = "6-platonic"; ToolTip.SetToolTip(control, "6-faces platonic solid index"); break; }
                    case 11: { control.Text = "8-platonic"; ToolTip.SetToolTip(control, "8-faces platonic solid index"); break; }
                    case 12: { control.Text = "12-platonic"; ToolTip.SetToolTip(control, "12-faces platonic solid index"); break; }
                    case 13: { control.Text = "20-platonic"; ToolTip.SetToolTip(control, "20-faces platonic solid index"); break; }
                    case 14: { control.Text = "19-hex C2h"; ToolTip.SetToolTip(control, "C(2h) polyhex hydrocarbon index with 19 hexagons"); break; }
                    case 15: { control.Text = "19-hex C2v"; ToolTip.SetToolTip(control, "C(2v) polyhex hydrocarbon index with 19 hexagons"); break; }
                    case 16: { control.Text = "---"; ToolTip.SetToolTip(control, "Reserved index"); break; }
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
                    control.Width = (m_dpi == 96.0F) ? 64 : 75;
                    control.Height = 21;
                    control.Top = 19 + (i * control.Height + 1);
                    control.Left = 19 + (j * control.Width + 2);
                    control.TextAlign = HorizontalAlignment.Center;
                    control.Font = new Font("Arial", 11);
                    control.MaxLength = 7;
                    if (j > 0) control.ReadOnly = true;
                    if (j > 0) control.BackColor = SystemColors.Control;
                    MainPanel.Controls.Add(control);

                    control.KeyPress += FixMicrosoft;
                    if (j == 0) control.TextChanged += TextBox_TextChanged;
                    control.KeyDown += TextBox_KeyDown;
                    control.Enter += TextBox_TextChanged;
                    //control.MouseEnter += TextBox_TextChanged;

                    if (j == 0) control.AllowDrop = true;
                    if (j == 0) control.MouseDown += Control_MouseDown;
                    if (j == 0) control.DragEnter += Control_DragEnter;
                    if (j == 0) control.DragDrop += Control_DragDrop;
                    control.MouseHover += Control_MouseHover;
                    controls[i, j] = control;
                }
            }
        }
        ClearCells();

        AboutToolStripMenuItem.Font = new Font(AboutToolStripMenuItem.Font, AboutToolStripMenuItem.Font.Style | FontStyle.Bold);

        m_filename = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", ".ini");
        LoadSettings();
    }
    private void ClearCells()
    {
        if (controls != null)
        {
            for (int i = 0; i < ROWS; i++)
            {
                ClearCells(i);
            }
        }
    }
    private void ClearCells(int row)
    {
        if (controls != null)
        {
            int i = row;
            {
                for (int j = 0; j < COLS; j++)
                {
                    TextBox control = controls[i, j];
                    if (control != null)
                    {
                        control.Text = "";
                        switch (j)
                        {
                            case 0:
                                control.BackColor = Color.White;
                                break;
                            case 1:
                            case 2:
                            case 3:
                                control.BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[3];
                                break;
                            case 4:
                                control.BackColor = Numbers.NUMBER_KIND_BACKCOLORS[0];
                                break;
                            case 5:
                                control.BackColor = Color.FromArgb(240, 240, 255);
                                break;
                            case 6:
                                control.BackColor = Color.FromArgb(255, 255, 192);
                                break;
                            case 7:
                                control.BackColor = Color.FromArgb(255, 255, 144);
                                break;
                            case 8:
                                control.BackColor = Color.FromArgb(192, 255, 192);
                                break;
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                                control.BackColor = Color.FromArgb(192, 192, 224);
                                break;
                            case 14:
                            case 15:
                                control.BackColor = Color.FromArgb(208, 160, 255);
                                break;
                            default:
                                control.BackColor = SystemColors.Control;
                                break;
                        }

                        controls[i, j] = control;
                    }
                }
            }
        }
    }

    private void Label_Click(object sender, EventArgs e)
    {
        if (sender is Label)
        {
            Clipboard.SetText(ToolTip.GetToolTip(sender as Label));
            string text = this.Text;
            this.Text = "Tooltip has been copied.";
            Thread.Sleep(500);
            this.Text = text;
        }
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
        batch_number = -1;
        ClearCells();
        if (controls != null)
        {
            //for (int i = 0; i < ROWS; i++)
            //{
            //    controls[i, 0].Text = "";
            //}
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
                str.AppendLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------");
                str.AppendLine("N" + "\t" + "P" + "\t" + "AP" + "\t" + "XP" + "\t" + "DF" + "\t" + "P=4n+1" + "\t" + "-gon" + "\t" + "-cgon" + "\t" + "-prymd" + "\t" + "4-plato" + "\t" + "6-plato" + "\t" + "8-plato" + "\t" + "12-plat" + "\t" + "20-plat" + "\t" + "19-xC2h" + "\t" + "19-xC2v" + "\t" + "---");
                str.AppendLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------");
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
                str.AppendLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------");

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
            if (control.Text.Length > 0)
            {
                string[] parts = control.Text.Split(',');
                long sum = 0L;
                foreach (string part in parts)
                {
                    sum += long.Parse(part);
                }
                ToolTip.SetToolTip(control, control.Text + " | sum = " + sum);
            }
            else
            {
                ToolTip.SetToolTip(control, null);
            }
        }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        //string version = typeof(MainForm).Assembly.GetName().Version.ToString();
        //int pos = version.LastIndexOf(".");
        //if (pos > -1)
        //{
        //    VersionLabel.Text = version.Substring(0, pos);
        //}
        VersionLabel.Text = Globals.SHORT_VERSION;

        if (this.Top < 0)
        {
            Initialize();
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
        if (point != null)
        {
            TextBox number_control = controls[point.X, 0];
            if (number_control != null)
            {
                number_control.Text = number_control.Text.Replace(" ", "");
                if (number_control.Text.Length == 0)
                {
                    number_control.Text = "0";
                    number_control.Refresh();
                }

                int number = 0;
                if (int.TryParse(number_control.Text, out number))
                {
                    if (number < int.MaxValue) number++;
                    number_control.Text = number.ToString();
                    number_control.Refresh();
                }
            }
        }
    }
    private void DecrementIndex(object sender)
    {
        Point point = GetControlLocation(sender);
        if (point != null)
        {
            TextBox number_control = controls[point.X, 0];
            if (number_control != null)
            {
                number_control.Text = number_control.Text.Replace(" ", "");
                if (number_control.Text.Length == 0)
                {
                    number_control.Text = "0";
                    number_control.Refresh();
                }

                int number = 0;
                if (int.TryParse(number_control.Text, out number))
                {
                    if (number > 1) number--;
                    number_control.Text = number.ToString();
                    number_control.Refresh();
                }
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
        if (point != null)
        {
            TextBox number_control = controls[point.X, 0];
            if (number_control != null)
            {
                number_control.Text = number_control.Text.Replace(" ", "");

                long number = 0;
                if (long.TryParse(number_control.Text, out number))
                {
                    number_control.ForeColor = Numbers.GetNumberTypeColor(number);

                    if (number > 0)
                    {
                        this.Cursor = Cursors.WaitCursor;
                        try
                        {
                            int count = 0;
                            if (number == 1)
                            {
                                controls[point.X, 1].Text = number.ToString();
                                controls[point.X, 2].Text = number.ToString();
                                controls[point.X, 3].Text = number.ToString();
                                controls[point.X, 1].ForeColor = Numbers.GetNumberTypeColor(number);
                                controls[point.X, 2].ForeColor = Numbers.GetNumberTypeColor(number);
                                controls[point.X, 3].ForeColor = Numbers.GetNumberTypeColor(number);
                                controls[point.X, 1].BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[1];
                                controls[point.X, 2].BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[1];
                                controls[point.X, 3].BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[1];
                                MainPanel.Controls[33 + count + 0].Text = "U";
                                MainPanel.Controls[33 + count + 1].Text = "U";
                                MainPanel.Controls[33 + count + 2].Text = "U";
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count++], "Unit index");
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count++], "Unit index");
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count++], "Unit index");
                            }
                            else
                            {
                                long p = Numbers.PrimeIndexOf(number) + 1;
                                if (p > 0L)
                                {
                                    long ap = Numbers.AdditivePrimeIndexOf(number) + 1;
                                    long xp = Numbers.NonAdditivePrimeIndexOf(number) + 1;
                                    controls[point.X, 1].Text = p.ToString();
                                    controls[point.X, 2].Text = (ap > 0) ? ap.ToString() : "";
                                    controls[point.X, 3].Text = (xp > 0) ? xp.ToString() : "";
                                    controls[point.X, 1].ForeColor = Numbers.GetNumberTypeColor(p);
                                    controls[point.X, 2].ForeColor = Numbers.GetNumberTypeColor(ap);
                                    controls[point.X, 3].ForeColor = Numbers.GetNumberTypeColor(xp);
                                    controls[point.X, 1].BackColor = (ap > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[3] : Numbers.NUMBER_TYPE_BACKCOLORS[4];
                                    controls[point.X, 2].BackColor = (ap > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[3] : Numbers.NUMBER_TYPE_BACKCOLORS[4];
                                    controls[point.X, 3].BackColor = (ap > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[3] : Numbers.NUMBER_TYPE_BACKCOLORS[4];
                                    MainPanel.Controls[33 + count + 0].Text = "P";
                                    MainPanel.Controls[33 + count + 1].Text = "AP";
                                    MainPanel.Controls[33 + count + 2].Text = "XP";
                                    ToolTip.SetToolTip(MainPanel.Controls[33 + count++], "Prime index");
                                    ToolTip.SetToolTip(MainPanel.Controls[33 + count++], "Additive Prime index");
                                    ToolTip.SetToolTip(MainPanel.Controls[33 + count++], "Non-additive Prime index");
                                }
                                else
                                {
                                    long c = Numbers.CompositeIndexOf(number) + 1;
                                    long ac = Numbers.AdditiveCompositeIndexOf(number) + 1;
                                    long xc = Numbers.NonAdditiveCompositeIndexOf(number) + 1;
                                    controls[point.X, 1].Text = c.ToString();
                                    controls[point.X, 2].Text = (ac > 0) ? ac.ToString() : "";
                                    controls[point.X, 3].Text = (xc > 0) ? xc.ToString() : "";
                                    controls[point.X, 1].ForeColor = Numbers.GetNumberTypeColor(c);
                                    controls[point.X, 2].ForeColor = Numbers.GetNumberTypeColor(ac);
                                    controls[point.X, 3].ForeColor = Numbers.GetNumberTypeColor(xc);
                                    controls[point.X, 1].BackColor = (ac > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[6] : Numbers.NUMBER_TYPE_BACKCOLORS[7];
                                    controls[point.X, 2].BackColor = (ac > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[6] : Numbers.NUMBER_TYPE_BACKCOLORS[7];
                                    controls[point.X, 3].BackColor = (ac > 0) ? Numbers.NUMBER_TYPE_BACKCOLORS[6] : Numbers.NUMBER_TYPE_BACKCOLORS[7];
                                    MainPanel.Controls[33 + count + 0].Text = "C";
                                    MainPanel.Controls[33 + count + 1].Text = "AC";
                                    MainPanel.Controls[33 + count + 3].Text = "XC";
                                    ToolTip.SetToolTip(MainPanel.Controls[33 + count++], "Composite index");
                                    ToolTip.SetToolTip(MainPanel.Controls[33 + count++], "Composite Prime index");
                                    ToolTip.SetToolTip(MainPanel.Controls[33 + count++], "Non-composite Prime index");
                                }
                            }

                            long df = Numbers.DeficientIndexOf(number) + 1;
                            long pf = Numbers.PerfectIndexOf(number) + 1;
                            long ab = Numbers.AbundantIndexOf(number) + 1;
                            if (df > 0L)
                            {
                                controls[point.X, 4].Text = df.ToString();
                                controls[point.X, 4].ForeColor = Numbers.GetNumberTypeColor(df);
                                controls[point.X, 4].BackColor = Numbers.NUMBER_KIND_BACKCOLORS[0];
                                MainPanel.Controls[33 + count].Text = "Deficient";
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Deficient index");
                            }
                            else if (pf > 0L)
                            {
                                controls[point.X, 4].Text = pf.ToString();
                                controls[point.X, 4].ForeColor = Numbers.GetNumberTypeColor(pf);
                                controls[point.X, 4].BackColor = Numbers.NUMBER_KIND_BACKCOLORS[1];
                                MainPanel.Controls[33 + count].Text = "Perfect";
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Perfect index");
                            }
                            else if (ab > 0L)
                            {
                                controls[point.X, 4].Text = ab.ToString();
                                controls[point.X, 4].ForeColor = Numbers.GetNumberTypeColor(ab);
                                controls[point.X, 4].BackColor = Numbers.NUMBER_KIND_BACKCOLORS[2];
                                MainPanel.Controls[33 + count].Text = "Abundant";
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Abundant index");
                            }
                            else // default
                            {
                                controls[point.X, 4].Text = ab.ToString();
                                controls[point.X, 4].ForeColor = Color.Black;
                                controls[point.X, 4].BackColor = Numbers.NUMBER_KIND_BACKCOLORS[0];
                                MainPanel.Controls[33 + count].Text = "";
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count], null);
                            }
                            count++;

                            long p4nplus1 = Numbers.Primes4nPlus1.IndexOf(number) + 1;
                            long p4nminus1 = Numbers.Primes4nMinus1.IndexOf(number) + 1;
                            long c4nplus1 = Numbers.Composites4nPlus1.IndexOf(number) + 1;
                            long c4nminus1 = Numbers.Composites4nMinus1.IndexOf(number) + 1;
                            if (p4nplus1 > 0L)
                            {
                                controls[point.X, 5].Text = p4nplus1.ToString();
                                controls[point.X, 5].ForeColor = Numbers.GetNumberTypeColor(p4nplus1);
                                controls[point.X, 5].BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[3];
                                MainPanel.Controls[33 + count].Text = "P=4n+1";
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count], "4n+1 Prime index");
                            }
                            else if (p4nminus1 > 0L)
                            {
                                controls[point.X, 5].Text = p4nminus1.ToString();
                                controls[point.X, 5].ForeColor = Numbers.GetNumberTypeColor(p4nminus1);
                                controls[point.X, 5].BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[4];
                                MainPanel.Controls[33 + count].Text = "P=4n-1";
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count], "4n-1 Prime index");
                            }
                            else if (c4nplus1 > 0L)
                            {
                                controls[point.X, 5].Text = c4nplus1.ToString();
                                controls[point.X, 5].ForeColor = Numbers.GetNumberTypeColor(c4nplus1);
                                controls[point.X, 5].BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[6];
                                MainPanel.Controls[33 + count].Text = "C=4n+1";
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count], "4n+1 Composite index");
                            }
                            else if (c4nminus1 > 0L)
                            {
                                controls[point.X, 5].Text = c4nminus1.ToString();
                                controls[point.X, 5].ForeColor = Numbers.GetNumberTypeColor(c4nminus1);
                                controls[point.X, 5].BackColor = Numbers.NUMBER_TYPE_BACKCOLORS[7];
                                MainPanel.Controls[33 + count].Text = "C=4n-1";
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count], "4n-1 Composite index");
                            }
                            else // default
                            {
                                controls[point.X, 5].Text = "";
                                controls[point.X, 5].ForeColor = Color.Black;
                                controls[point.X, 5].BackColor = SystemColors.Control;
                                MainPanel.Controls[33 + count].Text = "P=4n+1";
                                ToolTip.SetToolTip(MainPanel.Controls[33 + count], "4n+1 Prime index");
                            }
                            count++;
                            int backup_count = count;


                            // Polygons
                            bool no_match_yet = true;
                            controls[point.X, 6].Text = "";
                            MainPanel.Controls[33 + count].Text = "-gon";
                            ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Polygon index");
                            long sum = 0L;
                            for (int sides = 3; sides <= 114; sides++)
                            {
                                long polygon = Numbers.PolygonalNumbers(sides).IndexOf(number) + 1;
                                if (polygon > 0L)
                                {
                                    sum += sides;
                                    if (no_match_yet)
                                    {
                                        controls[point.X, 6].Text = polygon.ToString();
                                        controls[point.X, 6].ForeColor = Numbers.GetNumberTypeColor(polygon);
                                        MainPanel.Controls[33 + count].Text = sides.ToString() + "-gon";
                                        ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Polygon index");
                                        no_match_yet = false;
                                    }
                                    else
                                    {
                                        controls[point.X, 6].Text += "," + polygon.ToString();
                                        string text = MainPanel.Controls[33 + count].Text.Replace("-gon", "") + "," + sides.ToString();
                                        MainPanel.Controls[33 + count].Text = text;
                                        ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Polygon indices: " + text + " | sum = " + sum);
                                    }
                                }
                            }
                            count++;
                            backup_count = count;

                            // Centered Polygons
                            no_match_yet = true;
                            controls[point.X, 7].Text = "";
                            MainPanel.Controls[33 + count].Text = "-cgon";
                            ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Centered Polygon index");
                            sum = 0L;
                            for (int sides = 3; sides <= 114; sides++)
                            {
                                long centered_polygon = Numbers.CenteredPolygonalNumbers(sides).IndexOf(number) + 1;
                                if (centered_polygon > 0L)
                                {
                                    sum += sides;
                                    if (no_match_yet)
                                    {
                                        controls[point.X, 7].Text = centered_polygon.ToString();
                                        controls[point.X, 7].ForeColor = Numbers.GetNumberTypeColor(centered_polygon);
                                        MainPanel.Controls[33 + count].Text = sides.ToString() + "-cgon";
                                        ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Centered Polygon index");
                                        no_match_yet = false;
                                    }
                                    else
                                    {
                                        controls[point.X, 7].Text += "," + centered_polygon.ToString();
                                        string text = MainPanel.Controls[33 + count].Text.Replace("-cgon", "") + "," + sides.ToString();
                                        MainPanel.Controls[33 + count].Text = text;
                                        ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Centered Polygon indices: " + text + " | sum = " + sum);
                                    }
                                }
                            }
                            count++;
                            backup_count = count;

                            // Pyramids
                            no_match_yet = true;
                            controls[point.X, 8].Text = "";
                            MainPanel.Controls[33 + count].Text = "-pyramid";
                            ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Pyramid index");
                            sum = 0L;
                            for (int sides = 3; sides <= 114; sides++)
                            {
                                long pyramid = Numbers.PyramidalNumbers(sides).IndexOf(number) + 1;
                                if (pyramid > 0L)
                                {
                                    sum += sides;
                                    if (no_match_yet)
                                    {
                                        controls[point.X, 8].Text = pyramid.ToString();
                                        controls[point.X, 8].ForeColor = Numbers.GetNumberTypeColor(pyramid);
                                        MainPanel.Controls[33 + count].Text = sides.ToString() + "-pyramid";
                                        ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Pyramid index");
                                        no_match_yet = false;
                                    }
                                    else
                                    {
                                        controls[point.X, 8].Text += "," + pyramid.ToString();
                                        string text = MainPanel.Controls[33 + count].Text.Replace("-pyramid", "") + "," + sides.ToString();
                                        MainPanel.Controls[33 + count].Text = text;
                                        ToolTip.SetToolTip(MainPanel.Controls[33 + count], "Pyramid indices: " + text + " | sum = " + sum);
                                    }
                                }
                            }
                            count++;
                            backup_count = count;

                            // Platonic Solids
                            long polyhedral_4_faces = Numbers.CenteredTetrahedralNumbers.IndexOf(number) + 1;
                            if (polyhedral_4_faces > 1)
                            {
                                controls[point.X, 9].Text = polyhedral_4_faces.ToString();
                                controls[point.X, 9].ForeColor = Numbers.GetNumberTypeColor(polyhedral_4_faces);
                            }
                            else
                            {
                                controls[point.X, 9].Text = "";
                                controls[point.X, 9].ForeColor = Numbers.GetNumberTypeColor(0L);
                            }
                            long polyhedral_6_faces = Numbers.CenteredHexahedronNumbers.IndexOf(number) + 1;
                            if (polyhedral_6_faces > 1)
                            {
                                controls[point.X, 10].Text = polyhedral_6_faces.ToString();
                                controls[point.X, 10].ForeColor = Numbers.GetNumberTypeColor(polyhedral_6_faces);
                            }
                            else
                            {
                                controls[point.X, 10].Text = "";
                                controls[point.X, 10].ForeColor = Numbers.GetNumberTypeColor(0L);
                            }
                            long polyhedral_8_faces = Numbers.CenteredOctahedralNumbers.IndexOf(number) + 1;
                            if (polyhedral_8_faces > 1)
                            {
                                controls[point.X, 11].Text = polyhedral_8_faces.ToString();
                                controls[point.X, 11].ForeColor = Numbers.GetNumberTypeColor(polyhedral_8_faces);
                            }
                            else
                            {
                                controls[point.X, 11].Text = "";
                                controls[point.X, 11].ForeColor = Numbers.GetNumberTypeColor(0L);
                            }
                            long polyhedral_12_faces = Numbers.CenteredDodecahedralNumbers.IndexOf(number) + 1;
                            if (polyhedral_12_faces > 1)
                            {
                                controls[point.X, 12].Text = polyhedral_12_faces.ToString();
                                controls[point.X, 12].ForeColor = Numbers.GetNumberTypeColor(polyhedral_12_faces);
                            }
                            else
                            {
                                controls[point.X, 12].Text = "";
                                controls[point.X, 12].ForeColor = Numbers.GetNumberTypeColor(0L);
                            }
                            long polyhedral_20_faces = Numbers.CenteredIcosahedralNumbers.IndexOf(number) + 1;
                            if (polyhedral_20_faces > 1)
                            {
                                controls[point.X, 13].Text = polyhedral_20_faces.ToString();
                                controls[point.X, 13].ForeColor = Numbers.GetNumberTypeColor(polyhedral_20_faces);
                            }
                            else
                            {
                                controls[point.X, 13].Text = "";
                                controls[point.X, 13].ForeColor = Numbers.GetNumberTypeColor(0L);
                            }

                            // Chemical Compounds based-19
                            long hex19c2h = Numbers.PolyhexNumbers(1).IndexOf(number) + 1;
                            if (hex19c2h > 1)
                            {
                                controls[point.X, 14].Text = hex19c2h.ToString();
                                controls[point.X, 14].ForeColor = Numbers.GetNumberTypeColor(hex19c2h);
                            }
                            else
                            {
                                controls[point.X, 14].Text = "";
                                controls[point.X, 14].ForeColor = Numbers.GetNumberTypeColor(0L);
                            }
                            long hex19c2v = Numbers.PolyhexNumbers(2).IndexOf(number) + 1;
                            if (hex19c2v > 1)
                            {
                                controls[point.X, 15].Text = hex19c2v.ToString();
                                controls[point.X, 15].ForeColor = Numbers.GetNumberTypeColor(hex19c2v);
                            }
                            else
                            {
                                controls[point.X, 15].Text = "";
                                controls[point.X, 15].ForeColor = Numbers.GetNumberTypeColor(0L);
                            }
                        }
                        catch
                        {
                            ClearCells(point.X);
                        }
                        finally
                        {
                            this.Cursor = Cursors.Default;
                        }
                    }
                    else
                    {
                        ClearCells(point.X);
                    }
                }
                else
                {
                    ClearCells(point.X);
                }
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
    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MessageBox.Show
        (
            Application.ProductName + "  v" + Application.ProductVersion + "\r\n" +
            "©2009-2020 Ali Adams - علي عبد الرزاق عبد الكريم القره غولي" + "\r\n" + "\r\n" +
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
                str.AppendLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------");
                //?????str.AppendLine("i" + "\t" + "P" + "\t" + "AP" + "\t" + "XP" + "\t" + "C" + "\t" + "AC" + "\t" + "XC" + "\t" + "DF" + "\t" + "AB" + "\t" + "P=4n+1" + "\t" + "P=4n-1" + "\t" + "C=4n+1" + "\t" + "C=4n-1" + "\t" + "∑i" + "\t" + "∑ds(i)" + "\t" + "∑dr(i)" + "\t" + "Half" + "\t" + "Median" + "\t" + "Product");
                str.AppendLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------");

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

                str.AppendLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------");
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
