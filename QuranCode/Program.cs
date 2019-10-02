using System;
using System.Windows.Forms;
//using System.Threading;
//using System.Runtime.InteropServices;

//using Microsoft.Win32;
//static class NETVersion
//{
//    public static string GetVersion()
//    {
//        const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

//        using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
//        {
//            if (ndpKey != null && ndpKey.GetValue("Release") != null)
//            {
//                int releaseKey = (int)ndpKey.GetValue("Release");
//                if (releaseKey >= 528040)
//                    return "4.8 or later";
//                if (releaseKey >= 461808)
//                    return "4.7.2";
//                if (releaseKey >= 461308)
//                    return "4.7.1";
//                if (releaseKey >= 460798)
//                    return "4.7";
//                if (releaseKey >= 394802)
//                    return "4.6.2";
//                if (releaseKey >= 394254)
//                    return "4.6.1";
//                if (releaseKey >= 393295)
//                    return "4.6";
//                if (releaseKey >= 379893)
//                    return "4.5.2";
//                if (releaseKey >= 378675)
//                    return "4.5.1";
//                if (releaseKey >= 378389)
//                    return "4.5";
//                return GetOlderVersion();
//            }
//            else
//            {
//                return GetOlderVersion();
//            }
//        }
//    }
//    private static string GetOlderVersion()
//    {
//        // Opens the registry key for the .NET Framework entry.
//        using (RegistryKey ndpKey =
//                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).
//                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
//        {
//            foreach (var versionKeyName in ndpKey.GetSubKeyNames())
//            {
//                // Skip .NET Framework 4.5 version information.
//                if (versionKeyName == "v4")
//                {
//                    continue;
//                }

//                if (versionKeyName.StartsWith("v"))
//                {

//                    RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
//                    // Get the .NET Framework version value.
//                    var name = (string)versionKey.GetValue("Version", "");
//                    // Get the service pack (SP) number.
//                    var sp = versionKey.GetValue("SP", "").ToString();

//                    // Get the installation flag, or an empty string if there is none.
//                    var install = versionKey.GetValue("Install", "").ToString();
//                    if (string.IsNullOrEmpty(install)) // No install info; it must be in a child subkey.
//                        return ("{versionKeyName}  {name}");
//                    else
//                    {
//                        if (!(string.IsNullOrEmpty(sp)) && install == "1")
//                        {
//                            return ("{versionKeyName}  {name}  SP{sp}");
//                        }
//                    }
//                    if (!string.IsNullOrEmpty(name))
//                    {
//                        continue;
//                    }
//                    foreach (var subKeyName in versionKey.GetSubKeyNames())
//                    {
//                        RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
//                        name = (string)subKey.GetValue("Version", "");
//                        if (!string.IsNullOrEmpty(name))
//                            sp = subKey.GetValue("SP", "").ToString();

//                        install = subKey.GetValue("Install", "").ToString();
//                        if (string.IsNullOrEmpty(install)) //No install info; it must be later.
//                            return ("{versionKeyName}  {name}");
//                        else
//                        {
//                            if (!(string.IsNullOrEmpty(sp)) && install == "1")
//                            {
//                                return ("{subKeyName}  {name}  SP{sp}");
//                            }
//                            else if (install == "1")
//                            {
//                                return ("  {subKeyName}  {name}");
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        return "";
//    }
//}

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

            //MessageBox.Show(NETVersion.GetVersion());
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
