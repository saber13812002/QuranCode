using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

public static class FileHelper
{
    // http://stackoverflow.com/questions/1406808/wait-for-file-to-be-freed-by-process
    public static bool IsFileReady(string path)
    {
        FileStream stream = null;
        try
        {
            stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return false;
        }
        finally
        {
            if (stream != null)
            {
                stream.Close();
            }
        }

        //file is not locked
        return true;
    }
    public static void WaitForReady(string path)
    {
        if (File.Exists(path))
        {
            while (!FileHelper.IsFileReady(path))
            {
                Thread.Sleep(1000);
            }
        }
    }
    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            WaitForReady(path);
            File.Delete(path);
        }
    }

    public static void AppendLine(string path, string line)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path, true, Encoding.Unicode))
            {
                writer.WriteLine(line);
            }
        }
        catch
        {
            // silence error
        }
    }
    public static void SaveText(string path, string text)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
            {
                writer.WriteLine(text);
                writer.Close();
            }
        }
        catch
        {
            // silence error
        }
    }
    public static void SaveLetters(string path, char[] characters)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
            {
                foreach (char character in characters)
                {
                    if (character == '\0')
                    {
                        break;
                    }
                    writer.Write(character);
                }
            }
        }
        catch
        {
            // silence error
        }
    }
    public static void SaveWords(string path, List<string> words)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
            {
                foreach (string word in words)
                {
                    if (String.IsNullOrEmpty(word))
                    {
                        break;
                    }
                    writer.Write(word);
                }
            }
        }
        catch
        {
            // silence error
        }
    }
    public static void SaveValues(string path, List<long> values)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
            {
                foreach (long value in values)
                {
                    writer.WriteLine(value.ToString());
                }
            }
        }
        catch
        {
            // silence error
        }
    }

    public static List<string> LoadLines(string path)
    {
        List<string> result = new List<string>();
        try
        {
            if (File.Exists(path))
            {
                FileHelper.WaitForReady(path);

                using (StreamReader reader = File.OpenText(path))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        result.Add(line);
                    }
                }
            }
        }
        catch
        {
            // silence error
        }
        return result;
    }
    public static string LoadText(string path)
    {
        StringBuilder str = new StringBuilder();
        try
        {
            if (File.Exists(path))
            {
                FileHelper.WaitForReady(path);

                using (StreamReader reader = File.OpenText(path))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        str.AppendLine(line);
                    }
                }
            }
        }
        catch
        {
            // silence error
        }
        return str.ToString();
    }

    public static void DisplayFile(string path)
    {
        if (File.Exists(path))
        {
            FileHelper.WaitForReady(path);

            System.Diagnostics.Process.Start("Notepad.exe", path);
        }
    }
}
