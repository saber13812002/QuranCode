using System;
using System.Net;
using System.Text;
using System.IO;

public class Downloader : WebClient
{
    private int timeout;

    private Downloader(int timeout)
    {
        this.timeout = timeout;
    }

    protected override WebRequest GetWebRequest(Uri uri)
    {
        var result = base.GetWebRequest(uri);
        result.Timeout = this.timeout;
        return result;
    }

    public static void Download(string uri, string path, int timeout)
    {
        string download_folder = Path.GetDirectoryName(path);
        if (!Directory.Exists(download_folder))
        {
            Directory.CreateDirectory(download_folder);
        }

        using (Downloader web_client = new Downloader(timeout))
        {
            byte[] data = web_client.DownloadData(uri);
            if ((data != null) && (data.Length > 0))
            {
                File.WriteAllBytes(path, data);
            }
            else
            {
                throw new Exception("Invalid server address.\r\nPlease correct address in .INI file.");
            }
        }
    }

    //// Async example from MainForm.cs//
    //private void DownloadAsync(string uri)
    //{
    //    using (WebClient web_client = new WebClient())
    //    {
    //        web_client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataCompleted);
    //        web_client.DownloadDataAsync(new Uri(uri));
    //    }
    //}
    //private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
    //{
    //    // WARNING: runs on different thread to UI thread
    //    byte[] data = e.Result;

    //    string path = "Downloads";
    //    string download_folder = Path.GetDirectoryName(path);
    //    if (!Directory.Exists(download_folder))
    //    {
    //        Directory.CreateDirectory(download_folder);
    //    }

    //    if ((data != null) && (data.Length > 0))
    //    {
    //        File.WriteAllBytes(path, data);
    //    }
    //    else
    //    {
    //        throw new Exception("Invalid server address.\r\nPlease correct address in .INI file.");
    //    }
    //}
}
