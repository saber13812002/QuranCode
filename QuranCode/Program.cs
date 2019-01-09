using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

static class Program
{
    //// disable the x close icon
    //const int mf_byposition = 0x400;
    //[dllimport("user32")]
    //private static extern int removemenu(intptr hmenu, int nposition, int wflags);
    //[dllimport("user32")]
    //private static extern intptr getsystemmenu(intptr hwnd, bool brevert);
    //[dllimport("user32")]
    //private static extern int getmenuitemcount(intptr hwnd);
    //static void disablecloseicon(intptr system_menu_handle)
    //{
    //    int system_menu_item_count = getmenuitemcount(system_menu_handle);
    //    removemenu(system_menu_handle, system_menu_item_count - 1, mf_byposition);
    //}

    //// single instance
    //    bool m_first_instance = true;
    //    using (mutex mutex = new mutex(true, application.productname, out m_first_instance))
    //    {
    //        if (m_first_instance)
    //        {
    //            application.enablevisualstyles();
    //            application.setcompatibletextrenderingdefault(false);
    //            mainform form = new mainform();

    //            // disable the x close button of the form
    //            intptr system_menu_handle = getsystemmenu(form.handle, false);
    //            disablecloseicon(system_menu_handle);

    //            application.run(form);
    //        }
    //        else
    //        {
    //            windows windows = new windows(true, true);
    //            foreach (window window in windows)
    //            {
    //                if (window.title.startswith(application.productname))
    //                {
    //                    window.visible = true;
    //                    window.bringtofront();
    //                }
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        if ((args != null) && (args.Length > 0))
        {
            if (args[0].ToUpper() == "")
            {
                Globals.EDITION = Edition.Standard;
            }
            else if (args[0].ToUpper() == "U")
            {
                Globals.EDITION = Edition.Ultimate;
            }
            else
            {
                Globals.EDITION = Edition.Standard;
            }
        }
        else
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                Globals.EDITION = Edition.Ultimate;
            }
            else if (Control.ModifierKeys == Keys.Shift)
            {
                Globals.EDITION = Edition.Ultimate;
            }
            else // default
            {
                Globals.EDITION = Edition.Standard;
            }
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        MainForm form = new MainForm();

        //// disable the X close button of the form
        //IntPtr system_menu_handle = GetSystemMenu(form.Handle, false);
        //DisableCloseIcon(system_menu_handle);

        try
        {
            Application.Run(form);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
        }
    }
}
