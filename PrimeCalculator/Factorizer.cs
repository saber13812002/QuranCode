using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Management;

class Factorizer
{
    private Form m_form = null; // form to notify using BeginInvoke
    MainForm.UpdateProgressBar UpdateProgressBarDelegate = null;
    MainForm.UpdateOutputTextBox UpdateOutputTextBoxDelegate = null;

    private string m_number = null;
    private bool m_is_prime = false;
    private ArrayList m_factors = null;
    private bool m_is_cancel = false;
    private DateTime m_start = DateTime.Now;
    private TimeSpan m_duration = TimeSpan.Zero;
    private Process m_process = null;
    private ProcessStartInfo m_info = null;
    private string m_process_output = null;

    public Factorizer(Form form, string command, string arguments, bool multi_threaded)
    {
        if (form == null) return;

        m_form = form;
        UpdateProgressBarDelegate = ((MainForm)m_form).UpdateProgressBarMethod;
        UpdateOutputTextBoxDelegate = ((MainForm)m_form).UpdateOutputTextBoxMethod;

        //yafu-x64.exe factor(1234567890987654321) -threads 4 -ecm_path .\ecm\ecm.exe -ggnfs_dir .\ggnfs\
        string folder = Path.GetDirectoryName(Application.ExecutablePath) + "/" + "Tools";
        string filename = folder + "/" + (Windows.IsOS64Bit() ? "yafu-x64.exe" : "yafu-Win32.exe");
        string ecm_path = folder + "/" + "ecm" + "/" + "ecm.exe";
        string ggnfs_dir = folder + "/" + "ggnfs";

        m_number = arguments;
        m_is_prime = false;
        m_factors = new ArrayList();
        m_is_cancel = false;
        m_start = DateTime.Now;
        m_duration = TimeSpan.Zero;

        m_info = new ProcessStartInfo();
        m_info.WorkingDirectory = folder;
        m_info.FileName = filename;
        m_info.ErrorDialog = false;

        if (String.IsNullOrEmpty(command))
        {
            m_info.Arguments = m_number.Trim();
        }
        else
        {
            m_info.Arguments = command + "(" + m_number.Trim() + ")" + (multi_threaded ? (" -threads " + Environment.ProcessorCount.ToString()) : "");
            if (File.Exists(ecm_path))
            {
                //m_info.Arguments += " -noecm";
                m_info.Arguments += @" -ecm_path .\ecm\ecm.exe";
            }
            if (Directory.Exists(ggnfs_dir))
            {
                m_info.Arguments += @" -ggnfs_dir .\ggnfs\";
            }
        }

        m_info.CreateNoWindow = true;
        m_info.UseShellExecute = false;
        m_info.RedirectStandardError = true;
        m_info.RedirectStandardInput = true;
        m_info.RedirectStandardOutput = true;
        m_info.WindowStyle = ProcessWindowStyle.Hidden;
    }

    public string Number
    {
        get { return m_number; }
    }

    public bool IsPrime
    {
        get { return m_is_prime; }
        set { m_is_prime = value; }
    }

    public ArrayList Factors
    {
        get { return m_factors; }
    }

    public TimeSpan Duration
    {
        get { return m_duration; }
    }

    public bool Cancel
    {
        get { return m_is_cancel; }
        set { m_is_cancel = value; }
    }

    public void Run()
    {
        if (m_form != null)
        {
            m_start = DateTime.Now;

            m_is_cancel = false;
            try
            {
                m_is_prime = true;

                if (m_factors != null)
                {
                    m_factors.Clear();

                    using (m_process = new Process())
                    {
                        m_process.StartInfo = m_info;
                        m_process.Exited += new EventHandler(Process_Exited);
                        m_process.OutputDataReceived += new DataReceivedEventHandler(Process_OutputDataReceived);
                        m_process.Start();
                        m_process.BeginOutputReadLine();
                        m_process.WaitForExit();
                    }
                }
            }
            catch
            {
                m_is_cancel = true;

                m_process.Close();
                m_process.Dispose();
            }
            finally
            {
                m_duration = DateTime.Now - m_start;
                m_form.BeginInvoke(UpdateProgressBarDelegate, new object[] { 100 });
                Thread.Sleep(((MainForm)m_form).SleepTime);
            }
        }
    }

    private void Process_Exited(object sender, EventArgs e)
    {
        Debug.WriteLine("Process Exited");
    }
    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (m_process != null)
        {
            if (m_is_cancel)
            {
                try
                {
                    if (!m_process.HasExited)
                    {
                        KillProcessAndDescendents(m_process.Id);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName);
                }
            }
            else
            {
                if (e.Data != null)
                {
                    if ((e.Data != Environment.NewLine) && (!e.Data.Contains("ans")))
                    {
                        m_process_output += e.Data + Environment.NewLine;
                    }

                    if (e.Data.Contains("=") && !e.Data.Contains("ans"))
                    {
                        int pos = e.Data.IndexOf("=");
                        if (pos > -1)
                        {
                            string factor = e.Data.Substring(pos + 2);
                            if (Numbers.IsDigitsOnly(factor))
                            {
                                m_factors.Add(factor);
                            }
                        }
                    }
                    m_is_prime = (m_factors.Count == 1);

                    if (m_factors.Count >= 1)
                    {
                        if (e.Data.StartsWith("ans = "))
                        {
                            m_process_output += m_number + " = ";
                            foreach (string f in m_factors)
                            {
                                m_process_output += f + "*";
                            }
                            m_process_output = m_process_output.Remove(m_process_output.Length - "×".Length);
                        }
                    }
                    else // nextprime
                    {
                        if (e.Data.StartsWith("ans = "))
                        {
                            m_process_output = e.Data.Substring(6);
                        }
                    }

                    m_duration = DateTime.Now - m_start;
                    m_form.BeginInvoke(UpdateOutputTextBoxDelegate, new object[] { m_process_output });
                    Thread.Sleep(((MainForm)m_form).SleepTime);
                }
            }
        }
    }

    /// <summary>
    /// Kill a process, and all of its children, grandchildren, etc.
    /// </summary>
    /// <param name="pid">Process ID.</param>
    private static void KillProcessAndDescendents(int pid)
    {
        var mos = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE ParentProcessID=" + pid);
        ManagementObjectCollection moc = mos.Get();
        foreach (ManagementObject mo in moc)
        {
            KillProcessAndDescendents(Convert.ToInt32(mo["ProcessID"]));
        }

        try
        {
            Process p = Process.GetProcessById(pid);
            p.Kill();
        }
        catch (ArgumentException)
        {
            // process already exited
        }
    }
}
