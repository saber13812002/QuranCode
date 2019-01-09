using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        MainForm form = new MainForm();

        Application.Run(form);
    }
}
