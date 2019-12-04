using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

static class Program
{
    /// <summary>
    /// The main entry point for a single instance application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        if ((args != null) && (args.Length > 0))
        {
            if (args[0] == "")
            {
                Globals.EDITION = Edition.Standard;
            }
            else if (args[0] == "L")
            {
                Globals.EDITION = Edition.Lite;
            }
            else if (args[0] == "R")
            {
                Globals.EDITION = Edition.Research;
            }
            else if (args[0] == "U")
            {
                Globals.EDITION = Edition.Ultimate;
            }
            else // default
            {
                Globals.EDITION = Edition.Standard;
            }
        }
        else
        {
            if (Control.ModifierKeys == (Keys.Shift | Keys.Control))
            {
                Globals.EDITION = Edition.Ultimate;
            }
            else if (Control.ModifierKeys == Keys.Control)
            {
                Globals.EDITION = Edition.Research;
            }
            else if (Control.ModifierKeys == Keys.Shift)
            {
                Globals.EDITION = Edition.Lite;
            }
            else // default
            {
                Globals.EDITION = Edition.Standard;
            }
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        MainForm form = new MainForm();
        Application.Run(form);
    }

    // single instance
    //static void Main()
    //{
    //    bool is_first_instance = true;
    //    using (Mutex mutex = new Mutex(true, Application.ProductName, out is_first_instance))
    //    {
    //        if (is_first_instance)
    //        {
    //            Application.EnableVisualStyles();
    //            Application.SetCompatibleTextRenderingDefault(false);
    //            Application.Run(new MainForm());
    //        }
    //        else
    //        {
    //            Windows windows = new Windows(true, true);
    //            foreach (Window window in windows)
    //            {
    //                if (window.Title == "Indices")
    //                {
    //                    window.Visible = true;
    //                    //window.Activate();
    //                    window.BringToFront();
    //                }
    //            }
    //        }
    //    }
    //}
}
