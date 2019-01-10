public enum Edition { Standard }

public static class Globals
{
    public static Edition EDITION = Edition.Standard;
    public static string VERSION = "6.19.829.4"; // updated by Version.bat (with AssemblyInfo.cs of all projects)
    public static string SHORT_VERSION
    {
        get
        {
            string version = VERSION;
            string[] parts = version.Split('.');
            if (parts.Length == 4)
            {
                int pos = version.LastIndexOf('.');
                if (pos > -1)
                {
                    version = version.Substring(0, pos);
                }
            }

            if (EDITION == Edition.Standard)
            {
                return ("v" + version + "");
            }
            else
            {
                return ("v" + version + "!"); // Invalid Edition
            }
        }
    }
    public static string LONG_VERSION
    {
        get
        {
            return (VERSION + " - " + EDITION.ToString() + " Edition");
        }
    }

    // Global Variables
    public static string OUTPUT_FILE_EXT = ".csv"; // to open in Excel
    public static string DELIMITER = "\t";
    public static string SUB_DELIMITER = "|";
    public static string DATE_FORMAT = "yyyy-MM-dd";
    public static string TIME_FORMAT = "HH:mm:ss";
    public static string DATETIME_FORMAT = DATE_FORMAT + " " + TIME_FORMAT;
    public static string NUMBER_FORMAT = "000";

    // Global Folders
    public static string LANGUAGES_FOLDER = "Languages";
    public static string FONTS_FOLDER = "Fonts";
    public static string IMAGES_FOLDER = "Images";
    public static string DATA_FOLDER = "Data";
    public static string RULES_FOLDER = "Rules";
    public static string VALUES_FOLDER = "Values";
    public static string STATISTICS_FOLDER = "Statistics";
}
