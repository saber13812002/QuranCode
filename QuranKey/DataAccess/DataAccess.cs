using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Model;

public static class DataAccess
{
    static DataAccess()
    {
        if (!Directory.Exists(Globals.DATA_FOLDER))
        {
            Directory.CreateDirectory(Globals.DATA_FOLDER);
        }
    }

    // quran text from http://tanzil.net
    public static List<string> LoadVerseTexts()
    {
        List<string> result = new List<string>();
        string filename = Globals.DATA_FOLDER + "/" + "quran-uthmani.txt";
        if (File.Exists(filename))
        {
            using (StreamReader reader = File.OpenText(filename))
            {
                while (!reader.EndOfStream)
                {
                    // skip # comment lines (tanzil copyrights, other meta info, ...)
                    string line = reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    {
                        if (!line.StartsWith("#"))
                        {
                            line = line.Replace("\r", "");
                            line = line.Replace("\n", "");
                            while (line.Contains("  "))
                            {
                                line = line.Replace("  ", " ");
                            }
                            line = line.Trim();
                            result.Add(line);
                        }
                    }
                }
            }
        }
        return result;
    }
    public static void SaveVerseTexts(Book book, string filename)
    {
        if (book != null)
        {
            StringBuilder str = new StringBuilder();
            foreach (Verse verse in book.Verses)
            {
                str.AppendLine(verse.Text);
            }
            FileHelper.SaveText(filename, str.ToString());
        }
    }

    // end of verse stopmarks are assumed to be end of sentence (Meem) for now
    public static List<Stopmark> LoadVerseStopmarks()
    {
        List<Stopmark> result = new List<Stopmark>();
        string filename = Globals.DATA_FOLDER + "/" + "verse-stopmarks.txt";
        if (File.Exists(filename))
        {
            using (StreamReader reader = File.OpenText(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        result.Add(StopmarkHelper.GetStopmark(line));
                    }
                }
            }
        }
        return result;
    }
    public static void SaveVerseStopmarks(Book book, string filename)
    {
        if (book != null)
        {
            StringBuilder str = new StringBuilder();
            foreach (Verse verse in book.Verses)
            {
                str.AppendLine(StopmarkHelper.GetStopmarkText(verse.Stopmark));
            }
            FileHelper.SaveText(filename, str.ToString());
        }
    }

    // word meanings from http://qurandev.appspot.com - modified by Ali Adams
    public static void LoadWordMeanings(Book book)
    {
        if (book != null)
        {
            try
            {
                string filename = Globals.DATA_FOLDER + "/" + "en.wordbyword.txt";
                if (File.Exists(filename))
                {
                    using (StreamReader reader = File.OpenText(filename))
                    {
                        while (!reader.EndOfStream)
                        {
                            if (book.Verses != null)
                            {
                                foreach (Verse verse in book.Verses)
                                {
                                    string line = reader.ReadLine();
                                    string[] parts = line.Split('\t');

                                    int word_count = 0;
                                    if (verse.Words != null)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            if (word.Text != "و") // WawAsWord
                                            {
                                                word_count++;
                                            }
                                        }

                                        int i = 0;
                                        if (!verse.Book.WithBismAllah)
                                        {
                                            if (verse.NumberInChapter == 1)
                                            {
                                                if ((verse.Chapter.Number != 1) && (verse.Chapter.Number != 9))
                                                {
                                                    i += 4;
                                                }
                                            }
                                        }
                                        if (parts.Length != word_count + i)
                                        {
                                            //throw new Exception("File format error.");
                                        }

                                        foreach (Word word in verse.Words)
                                        {
                                            if (word.Text == "و") // WawAsWord
                                            {
                                                word.Meaning = "and";
                                            }
                                            else
                                            {
                                                word.Meaning = parts[i];
                                                i++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("LoadWordMeanings: " + ex.Message);
            }
        }
    }

    // word and its roots from http://noorsoft.org - reversed and corrected by Ali Adams and Yudi Rohmad
    public static void LoadWordRoots(Book book)
    {
        if (book != null)
        {
            try
            {
                // Id	Chapter	Verse	Word	Text	Roots
                string filename = Globals.DATA_FOLDER + "/" + "word-roots.txt";
                if (File.Exists(filename))
                {
                    using (StreamReader reader = File.OpenText(filename))
                    {
                        while (!reader.EndOfStream)
                        {
                            if (book.Verses != null)
                            {
                                foreach (Verse verse in book.Verses)
                                {
                                    // skip bismAllah if book without it
                                    if (!verse.Book.WithBismAllah)
                                    {
                                        if (verse.NumberInChapter == 1)
                                        {
                                            if ((verse.Chapter.Number != 1) && (verse.Chapter.Number != 9))
                                            {
                                                reader.ReadLine();
                                                reader.ReadLine();
                                                reader.ReadLine();
                                                reader.ReadLine();
                                            }
                                        }
                                    }

                                    if (verse.Words != null)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            if (word.Text == "و") // WawAsWord
                                            {
                                                word.Roots = new List<string>() { "و" };
                                            }
                                            else
                                            {
                                                string line = reader.ReadLine();
                                                if (!String.IsNullOrEmpty(line))
                                                {
                                                    string[] parts = line.Split('\t');

                                                    if (parts.Length == 6)
                                                    {
                                                        string text = parts[4];
                                                        string[] subparts = parts[5].Split('|');
                                                        word.Roots = new List<string>(subparts);
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("Invalid file format.");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("LoadWordRoots: " + ex.Message);
            }
        }
    }
    public static void SaveWordRoots(Book book)
    {
        if (book != null)
        {
            try
            {
                if (Directory.Exists(Globals.DATA_FOLDER))
                {
                    string filename = Globals.DATA_FOLDER + "/" + "word-roots.txt";
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                    {
                        if (book.Verses != null)
                        {
                            StringBuilder str = new StringBuilder();
                            foreach (Verse verse in book.Verses)
                            {
                                if (verse.Words != null)
                                {
                                    foreach (Word word in verse.Words)
                                    {
                                        str.Append(word.Text + "\t");
                                        foreach (string root in word.Roots)
                                        {
                                            str.Append(root + "|");
                                        }
                                        str.Remove(str.Length - 1, 1); // "|"

                                        writer.WriteLine(str);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveWordRoots: " + ex.Message);
            }
        }
    }
}
