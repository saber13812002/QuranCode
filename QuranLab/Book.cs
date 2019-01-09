using System;
using System.Collections.Generic;

public class Book
{
    public const int CHAPTERS = 114;
    public static Chapter[] Chapters = null;

    public static bool Initialized
    {
        get { return (Chapters != null); }
    }
    public static bool Initialize(string path)
    {
        if (!Initialized)
        {
            return LoadChapters(path);
        }
        return Initialized;
    }
    private static bool LoadChapters(string filename)
    {
        try
        {
            List<string> lines = FileHelper.LoadLines(filename);

            // skip Heading line
            int count = lines.Count - 1;
            if (count == CHAPTERS)
            {
                Chapters = new Chapter[CHAPTERS];
                for (int i = 0; i < CHAPTERS; i++)
                {
                    // skip Heading line
                    string[] parts = lines[i + 1].Split('\t');

                    if (parts.Length == 4)
                    {
                        int number = int.Parse(parts[0]);
                        int verse_count = int.Parse(parts[1]);
                        int word_count = int.Parse(parts[2]);
                        int letter_count = int.Parse(parts[3]);
                        Chapters[i] = new Chapter(number, verse_count, word_count, letter_count);
                    }
                    else
                    {
                        throw new Exception(filename + "must be of the following format:" + "\r\r" +
                                            "Chapter\tVerses\tWords\tLetters");
                    }
                }
            }
            else
            {
                throw new Exception(filename + " must contain 1 heading line and " + CHAPTERS + " lines." + "\r\n" +
                                   "One line per chapter in the following format:" + "\r\n" +
                                   "Chapter\tVerses\tWords\tLetters");
            }
        }
        catch (Exception /*ex*/)
        {
            return false;
        }
        return true;
    }
}
