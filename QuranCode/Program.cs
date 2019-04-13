using System;
using System.Windows.Forms;
//using System.Threading;
//using System.Runtime.InteropServices;

static class Program
{
    //// disable the X close button
    //const int MF_BYPOSITION = 0x400;
    //[DllImport("User32")]
    //private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);
    //[DllImport("User32")]
    //private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    //[DllImport("User32")]
    //private static extern int GetMenuItemCount(IntPtr hWnd);
    //static void DisableCloseButton(IntPtr system_menu_handle)
    //{
    //    int system_menu_item_count = GetMenuItemCount(system_menu_handle);
    //    RemoveMenu(system_menu_handle, system_menu_item_count - 1, MF_BYPOSITION);
    //}

    /// <summary>
    /// The main entry point for the Application.
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

        //// ensure single instance
        //bool m_first_instance = true;
        //using (Mutex mutex = new Mutex(true, Application.ProductName, out m_first_instance))
        //{
        //    if (m_first_instance)
        //    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        MainForm form = new MainForm();
        if (form != null)
        {
            //            // disable the X close button of the form
            //            IntPtr system_menu_handle = GetSystemMenu(form.Handle, false);
            //            DisableCloseIcon(system_menu_handle);

            try
            {
                Application.Run(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName);
            }
        }
        //    }
        //    else
        //    {
        //        Windows windows = new Windows(true, true);
        //        if (windows != null)
        //        {
        //            foreach (Window window in windows)
        //            {
        //                if (window.Title.StartsWith(Application.ProductName))
        //                {
        //                    window.Visible = true;
        //                    window.BringToFront();
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
