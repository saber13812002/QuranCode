using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

public partial class AboutBox : Form
{
    public AboutBox()
    {
        InitializeComponent();

        //  Initialize the AboutBox to display the product information from the assembly information.
        //  Change assembly information settings for your application through either:
        //  - Project->Properties->Application->Assembly Information
        //  - AssemblyInfo.cs
        this.Text = String.Format("About {0}", ProductName);
        this.ProductNameLabel.Text = ProductName + " v" + Globals.LONG_VERSION;
        this.CopyrightLabel.Text = AssemblyCopyright;
        //this.CompanyNameLabel.Text = AssemblyCompany;
        this.DescriptionTextBox.Text = AssemblyDescription;
    }

    #region Assembly Attribute Accessors

    public string AssemblyTitle
    {
        get
        {
            // Get all Title attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            // If there is at least one Title attribute
            if (attributes.Length > 0)
            {
                // Select the first one
                AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                // If it is not an empty string, return it
                if (titleAttribute.Title != "")
                    return titleAttribute.Title;
            }
            // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
            return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
        }
    }

    public string AssemblyVersion
    {
        get
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }

    public string AssemblyDescription
    {
        get
        {
            // Get all Description attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            // If there aren't any Description attributes, return an empty string
            if (attributes.Length == 0)
                return null;
            // If there is a Description attribute, return its value
            return ((AssemblyDescriptionAttribute)attributes[0]).Description;
        }
    }

    public string AssemblyProduct
    {
        get
        {
            // Get all Product attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            // If there aren't any Product attributes, return an empty string
            if (attributes.Length == 0)
                return null;
            // If there is a Product attribute, return its value
            return ((AssemblyProductAttribute)attributes[0]).Product;
        }
    }

    public string AssemblyCopyright
    {
        get
        {
            // Get all Copyright attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            // If there aren't any Copyright attributes, return an empty string
            if (attributes.Length == 0)
                return null;
            // If there is a Copyright attribute, return its value
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }
    }

    public string AssemblyCompany
    {
        get
        {
            // Get all Company attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            // If there aren't any Company attributes, return an empty string
            if (attributes.Length == 0)
                return null;
            // If there is a Company attribute, return its value
            return ((AssemblyCompanyAttribute)attributes[0]).Company;
        }
    }

    public string AssemblyTrademark
    {
        get
        {
            // Get all Trademark attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);
            // If there aren't any Company attributes, return an empty string
            if (attributes.Length == 0)
                return null;
            // If there is a Trademark attribute, return its value
            return ((AssemblyTrademarkAttribute)attributes[0]).Trademark;
        }
    }
    #endregion

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

    SplashForm m_splash_form = null;
    private void AboutPictureBox_Click(object sender, EventArgs e)
    {
        if (m_splash_form == null)
        {
            m_splash_form = new SplashForm();
        }
        if (m_splash_form != null)
        {
            m_splash_form.ShowDialog();
        }
    }
    private void AboutBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            if (m_splash_form != null)
            {
                m_splash_form.Close();
                m_splash_form.Dispose();
                m_splash_form = null;
            }
            this.Close();
        }
    }
}
