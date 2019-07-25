﻿using System;
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

        if (!Directory.Exists(Globals.AUDIO_FOLDER))
        {
            Directory.CreateDirectory(Globals.AUDIO_FOLDER);
        }

        if (!Directory.Exists(Globals.TRANSLATIONS_FOLDER))
        {
            Directory.CreateDirectory(Globals.TRANSLATIONS_FOLDER);
        }
    }

    // quran text from http://tanzil.net
    public static List<string> LoadVerseTexts(string filename)
    {
        List<string> result = new List<string>();
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
                    if (!String.IsNullOrEmpty(line))
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

    // recitation infos from http://www.everyayah.com
    public static void LoadRecitationInfos(Book book)
    {
        if (book != null)
        {
            book.RecitationInfos = new Dictionary<string, RecitationInfo>();
            string filename = Globals.AUDIO_FOLDER + "/" + "metadata.txt";
            if (File.Exists(filename))
            {
                using (StreamReader reader = File.OpenText(filename))
                {
                    string line = reader.ReadLine(); // skip header row
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('\t');
                        if (parts.Length >= 4)
                        {
                            RecitationInfo recitation = new RecitationInfo();
                            recitation.Url = parts[0];
                            recitation.Folder = parts[0];
                            recitation.Language = parts[1];
                            recitation.Reciter = parts[2];
                            int.TryParse(parts[3], out recitation.Quality);
                            recitation.Name = recitation.Language + " - " + recitation.Reciter;
                            book.RecitationInfos.Add(parts[0], recitation);
                        }
                    }
                }
            }
        }
    }

    // translations info from http://tanzil.net
    public static void LoadTranslationInfos(Book book)
    {
        if (book != null)
        {
            book.TranslationInfos = new Dictionary<string, TranslationInfo>();
            string filename = Globals.TRANSLATIONS_OFFLINE_FOLDER + "/" + "metadata.txt";
            if (File.Exists(filename))
            {
                using (StreamReader reader = File.OpenText(filename))
                {
                    string line = reader.ReadLine(); // skip header row
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('\t');
                        if (parts.Length >= 4)
                        {
                            TranslationInfo translation = new TranslationInfo();
                            translation.Url = "?transID=" + parts[0] + "&type=" + TranslationInfo.FileType;
                            translation.Flag = parts[1];
                            translation.Language = parts[2];
                            translation.Translator = parts[3];
                            translation.Name = parts[2] + " - " + parts[3];
                            book.TranslationInfos.Add(parts[0], translation);
                        }
                    }
                }
            }
        }
    }

    // translation books from http://tanzil.net
    public static void LoadTranslations(Book book)
    {
        if (book != null)
        {
            try
            {
                if (Directory.Exists(Globals.TRANSLATIONS_FOLDER))
                {
                    string[] filenames = Directory.GetFiles(Globals.TRANSLATIONS_FOLDER + "/");
                    foreach (string filename in filenames)
                    {
                        List<string> translated_lines = FileHelper.LoadLines(filename);
                        if (translated_lines != null)
                        {
                            string translation = filename.Substring((Globals.TRANSLATIONS_FOLDER.Length + 1), filename.Length - (Globals.TRANSLATIONS_FOLDER.Length + 1) - 4);
                            if (translation == "metadata.txt") continue;
                            LoadTranslation(book, translation);
                        }
                    }
                }
            }
            catch
            {
                // ignore error
            }
        }
    }
    public static void LoadTranslation(Book book, string translation)
    {
        if (book != null)
        {
            try
            {
                if (Directory.Exists(Globals.TRANSLATIONS_FOLDER))
                {
                    if (Directory.Exists(Globals.TRANSLATIONS_OFFLINE_FOLDER))
                    {
                        string[] filenames = Directory.GetFiles(Globals.TRANSLATIONS_FOLDER + "/");
                        bool already_loaded = false;
                        foreach (string filename in filenames)
                        {
                            if (filename.Contains(translation))
                            {
                                already_loaded = true;
                                break;
                            }
                        }
                        if (!already_loaded)
                        {
                            File.Copy(Globals.TRANSLATIONS_OFFLINE_FOLDER + "/" + translation + ".txt", Globals.TRANSLATIONS_FOLDER + "/" + translation + ".txt", true);
                        }

                        filenames = Directory.GetFiles(Globals.TRANSLATIONS_FOLDER + "/");
                        foreach (string filename in filenames)
                        {
                            if (filename.Contains(translation))
                            {
                                List<string> translated_lines = FileHelper.LoadLines(filename);
                                if (translated_lines != null)
                                {
                                    if (book.TranslationInfos != null)
                                    {
                                        if (book.TranslationInfos.ContainsKey(translation))
                                        {
                                            if (book.Verses != null)
                                            {
                                                if (book.Verses.Count > 0)
                                                {
                                                    for (int i = 0; i < book.Verses.Count; i++)
                                                    {
                                                        book.Verses[i].Translations[translation] = translated_lines[i];
                                                    }

                                                    // add bismAllah translation to the first verse of each chapter except chapters 1 and 9
                                                    foreach (Chapter chapter in book.Chapters)
                                                    {
                                                        if ((chapter.Number != 1) && (chapter.Number != 9))
                                                        {
                                                            if ((translation != "ar.emlaaei") && (translation != "en.transliteration") && (translation != "en.wordbyword"))
                                                            {
                                                                if (!chapter.Verses[0].Translations[translation].StartsWith(book.Verses[0].Translations[translation]))
                                                                {
                                                                    chapter.Verses[0].Translations[translation] = book.Verses[0].Translations[translation] + " " + chapter.Verses[0].Translations[translation];
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignore error
            }
        }
    }
    public static void UnloadTranslation(Book book, string translation)
    {
        if (book != null)
        {
            try
            {
                if (Directory.Exists(Globals.TRANSLATIONS_FOLDER))
                {
                    string[] filenames = Directory.GetFiles(Globals.TRANSLATIONS_FOLDER + "/");
                    foreach (string filename in filenames)
                    {
                        if (filename.Contains(translation))
                        {
                            if (book.TranslationInfos != null)
                            {
                                if (book.TranslationInfos.ContainsKey(translation))
                                {
                                    if (book.Verses.Count > 0)
                                    {
                                        for (int i = 0; i < book.Verses.Count; i++)
                                        {
                                            book.Verses[i].Translations.Remove(translation);
                                        }
                                        book.TranslationInfos.Remove(translation);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignore error
            }
        }
    }
    public static void SaveTranslation(Book book, string translation)
    {
        if (book != null)
        {
            StringBuilder str = new StringBuilder();
            foreach (Verse verse in book.Verses)
            {
                str.AppendLine(verse.Translations[translation]);
            }
            if (Directory.Exists(Globals.TRANSLATIONS_FOLDER))
            {
                string filename = Globals.TRANSLATIONS_FOLDER + "/" + translation + ".txt";
                FileHelper.SaveText(filename, str.ToString());
            }
        }
    }

    // word meanings from http://qurandev.appspot.com - modified by Ali Adams
    public static void LoadWordMeanings(Book book)
    {
        if (book != null)
        {
            try
            {
                string filename = Globals.TRANSLATIONS_OFFLINE_FOLDER + "/" + "en.wordbyword.txt";
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
                                    if (!String.IsNullOrEmpty(line))
                                    {
                                        string[] parts = line.Split('\t');

                                        int word_count = 0;
                                        if (verse.Words != null)
                                        {
                                            if (verse.Book.WawAsWord)
                                            {
                                                foreach (Word word in verse.Words)
                                                {
                                                    if (word.Text != "و") // WawAsWord
                                                    {
                                                        word_count++;
                                                    }
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
                                                    if ((i >= 0) && (i < parts.Length))
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
                                                        //throw new Exception("Invalid file format.");
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

    // corpus word parts from http://corpus.quran.com version 0.4 - modified by Ali Adams
    public static void LoadWordParts(Book book)
    {
        if (book != null)
        {
            try
            {
                string filename = Globals.DATA_FOLDER + "/" + "word-parts.txt";
                if (File.Exists(filename))
                {
                    using (StreamReader reader = File.OpenText(filename))
                    {
                        int waw_count = 0;
                        int previous_verse_number = 0;
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (!String.IsNullOrEmpty(line))
                            {
                                if (line.StartsWith("#") || line.StartsWith("LOCATION") || line.StartsWith("ADDRESS"))
                                {
                                    continue; // skip header info
                                }
                                else
                                {
                                    string[] parts = line.Split('\t');
                                    if (parts.Length >= 4)
                                    {
                                        string address = parts[0];
                                        if (address.StartsWith("(") && address.EndsWith(")"))
                                        {
                                            address = parts[0].Substring(1, parts[0].Length - 2);
                                        }
                                        string[] address_parts = address.Split(':');
                                        if (address_parts.Length == 4)
                                        {
                                            int chapter_number = int.Parse(address_parts[0]);
                                            int verse_number = int.Parse(address_parts[1]);
                                            if (previous_verse_number != verse_number)
                                            {
                                                waw_count = 0;
                                                previous_verse_number = verse_number;
                                            }
                                            int word_number = int.Parse(address_parts[2]) + waw_count;
                                            int word_part_number = int.Parse(address_parts[3]);

                                            string buckwalter = parts[1];
                                            string tag = parts[2];

                                            if (book.Chapters != null)
                                            {
                                                Chapter chapter = book.Chapters[chapter_number - 1];
                                                if (chapter != null)
                                                {
                                                    Verse verse = chapter.Verses[verse_number - 1];
                                                    if (verse != null)
                                                    {
                                                        if (book.WithBismAllah)
                                                        {
                                                            // add bismAllah manually to each chapter except 1 and 9
                                                            if (
                                                                ((chapter_number != 1) && (chapter_number != 9))
                                                                &&
                                                                ((verse_number == 1) && (word_number == 1) && (word_part_number == 1))
                                                               )
                                                            {
                                                                Verse bismAllah_verse = book.Verses[0];

                                                                // if there is no bismAllah, add one
                                                                if (parts[1] != bismAllah_verse.Words[0].Parts[0].Buckwalter)
                                                                {
                                                                    // insert 4 new words
                                                                    verse.Words.InsertRange(0, new List<Word>(4));

                                                                    //(1:1:1:1)	bi	PP	PREFIX|bi+
                                                                    WordPart word_part = new WordPart(verse.Words[0],
                                                                          bismAllah_verse.Words[0].Parts[0].NumberInWord,
                                                                          bismAllah_verse.Words[0].Parts[0].Buckwalter,
                                                                          bismAllah_verse.Words[0].Parts[0].Tag,
                                                                          new WordPartGrammar(bismAllah_verse.Words[0].Parts[0].Grammar)
                                                                    );
                                                                    if ((chapter_number == 95) || (chapter_number == 97))
                                                                    {
                                                                        // add shadda  { '~', 'ّ' } on B or bism
                                                                        word_part.Buckwalter = word_part.Buckwalter.Insert(1, "~");
                                                                    }

                                                                    //(1:1:1:2)	somi	N	STEM|POS:N|LEM:{som|ROOT:smw|M|GEN
                                                                    new WordPart(verse.Words[0],
                                                                      bismAllah_verse.Words[0].Parts[1].NumberInWord,
                                                                      bismAllah_verse.Words[0].Parts[1].Buckwalter,
                                                                      bismAllah_verse.Words[0].Parts[1].Tag,
                                                                      new WordPartGrammar(bismAllah_verse.Words[0].Parts[1].Grammar)
                                                                    );

                                                                    //(1:1:2:1)	{ll~ahi	PN	STEM|POS:PN|LEM:{ll~ah|ROOT:Alh|GEN
                                                                    new WordPart(verse.Words[1],
                                                                      bismAllah_verse.Words[1].Parts[0].NumberInWord,
                                                                      bismAllah_verse.Words[1].Parts[0].Buckwalter,
                                                                      bismAllah_verse.Words[1].Parts[0].Tag,
                                                                      new WordPartGrammar(bismAllah_verse.Words[1].Parts[0].Grammar)
                                                                    );

                                                                    //(1:1:3:1)	{l	DET	PREFIX|Al+
                                                                    new WordPart(verse.Words[2],
                                                                      bismAllah_verse.Words[2].Parts[0].NumberInWord,
                                                                      bismAllah_verse.Words[2].Parts[0].Buckwalter,
                                                                      bismAllah_verse.Words[2].Parts[0].Tag,
                                                                      new WordPartGrammar(bismAllah_verse.Words[2].Parts[0].Grammar)
                                                                    );

                                                                    //(1:1:3:2)	r~aHoma`ni	ADJ	STEM|POS:ADJ|LEM:r~aHoma`n|ROOT:rHm|MS|GEN
                                                                    new WordPart(verse.Words[2],
                                                                      bismAllah_verse.Words[2].Parts[1].NumberInWord,
                                                                      bismAllah_verse.Words[2].Parts[1].Buckwalter,
                                                                      bismAllah_verse.Words[2].Parts[1].Tag,
                                                                      new WordPartGrammar(bismAllah_verse.Words[2].Parts[1].Grammar)
                                                                    );

                                                                    //(1:1:4:1)	{l	DET	PREFIX|Al+
                                                                    new WordPart(verse.Words[3],
                                                                      bismAllah_verse.Words[3].Parts[0].NumberInWord,
                                                                      bismAllah_verse.Words[3].Parts[0].Buckwalter,
                                                                      bismAllah_verse.Words[3].Parts[0].Tag,
                                                                      new WordPartGrammar(bismAllah_verse.Words[3].Parts[0].Grammar)
                                                                    );

                                                                    //(1:1:4:2)	r~aHiymi	ADJ	STEM|POS:ADJ|LEM:r~aHiym|ROOT:rHm|MS|GEN
                                                                    new WordPart(verse.Words[3],
                                                                      bismAllah_verse.Words[3].Parts[1].NumberInWord,
                                                                      bismAllah_verse.Words[3].Parts[1].Buckwalter,
                                                                      bismAllah_verse.Words[3].Parts[1].Tag,
                                                                      new WordPartGrammar(bismAllah_verse.Words[3].Parts[1].Grammar)
                                                                    );
                                                                }
                                                            }
                                                            // correct word_number (if needed) for all subsequenct chapter word_parts
                                                            if (
                                                                ((chapter_number != 1) && (chapter_number != 9)) && (verse_number == 1)
                                                               )
                                                            {
                                                                word_number += 4;
                                                            }
                                                        }

                                                        Word word = verse.Words[word_number - 1];
                                                        if (word != null)
                                                        {
                                                            List<string> grammar = new List<string>(parts[3].Split('|'));
                                                            if (grammar.Count > 0)
                                                            {
                                                                //(1:5:3:1)	wa	CONJ	PREFIX|w_CONJ+
                                                                //(1:5:3:2)	<iy~aAka	PRON	STEM|POS:PRON|LEM:<iy~aA|2MS
                                                                if (word.Text == "و")
                                                                {
                                                                    waw_count++;
                                                                }
                                                                new WordPart(word, word_part_number, buckwalter, tag, grammar);
                                                            }
                                                            else
                                                            {
                                                                //throw new Exception("Grammar field is missing.\r\n" + filename);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //throw new Exception("Invalid Location Format.\r\n" + filename);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //throw new Exception("Invalid File Format.\r\n" + filename);
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
                throw new Exception("LoadWordParts: " + ex.Message);
            }
        }
    }
}
