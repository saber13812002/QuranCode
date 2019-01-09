using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Model;

public class Server
{
    static Server()
    {
        if (!Directory.Exists(Globals.STATISTICS_FOLDER))
        {
            Directory.CreateDirectory(Globals.STATISTICS_FOLDER);
        }

        if (!Directory.Exists(Globals.RULES_FOLDER))
        {
            Directory.CreateDirectory(Globals.RULES_FOLDER);
        }

        if (!Directory.Exists(Globals.VALUES_FOLDER))
        {
            Directory.CreateDirectory(Globals.VALUES_FOLDER);
        }

        // load simplification systems
        LoadSimplificationSystems();

        // load numerology systems
        LoadNumerologySystems();
    }

    // the book [DYNAMIC]
    private static Book s_book = null;
    public static Book Book
    {
        get { return s_book; }
    }

    // loaded simplification systems [STATIC]
    private static Dictionary<string, SimplificationSystem> s_loaded_simplification_systems = null;
    public static Dictionary<string, SimplificationSystem> LoadedSimplificationSystems
    {
        get { return s_loaded_simplification_systems; }
    }
    private static void LoadSimplificationSystems()
    {
        if (s_loaded_simplification_systems == null)
        {
            s_loaded_simplification_systems = new Dictionary<string, SimplificationSystem>();
        }

        if (s_loaded_simplification_systems != null)
        {
            s_loaded_simplification_systems.Clear();

            string path = Globals.RULES_FOLDER;
            DirectoryInfo folder = new DirectoryInfo(path);
            if (folder != null)
            {
                FileInfo[] files = folder.GetFiles("*.txt");
                if ((files != null) && (files.Length > 0))
                {
                    foreach (FileInfo file in files)
                    {
                        string text_mode = file.Name.Remove(file.Name.Length - 4, 4);
                        if (!String.IsNullOrEmpty(text_mode))
                        {
                            LoadSimplificationSystem(text_mode);
                        }
                    }

                    // start with default simplification system
                    if (s_loaded_simplification_systems.ContainsKey(SimplificationSystem.DEFAULT_NAME))
                    {
                        s_simplification_system = new SimplificationSystem(s_loaded_simplification_systems[SimplificationSystem.DEFAULT_NAME]);
                    }
                    else
                    {
                        //throw new Exception("ERROR: No default simplification system was found.");
                    }
                }
            }
        }
    }
    // simplification system [DYNAMIC]
    private static SimplificationSystem s_simplification_system = null;
    public static SimplificationSystem SimplificationSystem
    {
        get { return s_simplification_system; }
    }
    public static void LoadSimplificationSystem(string text_mode)
    {
        if (String.IsNullOrEmpty(text_mode)) return;

        if (s_loaded_simplification_systems != null)
        {
            // remove and rebuild on the fly without restarting application
            if (s_loaded_simplification_systems.ContainsKey(text_mode))
            {
                s_loaded_simplification_systems.Remove(text_mode);
            }

            string filename = Globals.RULES_FOLDER + "/" + text_mode + ".txt";
            if (File.Exists(filename))
            {
                List<string> lines = FileHelper.LoadLines(filename);

                SimplificationSystem simplification_system = new SimplificationSystem(text_mode);
                if (simplification_system != null)
                {
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("#")) continue;

                        string[] parts = line.Split('\t');
                        if (parts.Length == 2)
                        {
                            SimplificationRule rule = new SimplificationRule(parts[0], parts[1]);
                            if (rule != null)
                            {
                                simplification_system.Rules.Add(rule);
                            }
                        }
                        else
                        {
                            //throw new Exception(filename + " file format must be:\r\n\tText TAB Replacement");
                        }
                    }
                }

                try
                {
                    // add to dictionary
                    s_loaded_simplification_systems.Add(simplification_system.Name, simplification_system);
                }
                catch
                {
                    // silence error
                }
            }

            // set current simplification system
            if (s_loaded_simplification_systems.ContainsKey(text_mode))
            {
                s_simplification_system = s_loaded_simplification_systems[text_mode];
            }
        }
    }
    public static void BuildSimplifiedBook(string text_mode, bool with_bism_Allah, bool waw_as_word, bool shadda_as_letter)
    {
        if (!String.IsNullOrEmpty(text_mode))
        {
            if (s_loaded_simplification_systems != null)
            {
                if (s_loaded_simplification_systems.ContainsKey(text_mode))
                {
                    s_simplification_system = s_loaded_simplification_systems[text_mode];

                    // reload original Quran text
                    List<string> texts = DataAccess.LoadVerseTexts();

                    // generate Quranmarks/Stopmarks word numbers
                    //int[] numbers = new int[]
                    //{
                    //    7, 286, 200, 176, 120, 165, 206, 75, 129, 109,
                    //    123, 111, 43, 52, 99, 128, 111, 110, 98, 135,
                    //    112, 78, 118, 64, 77, 227, 93, 88, 69, 60,
                    //    34, 30, 73, 54, 45, 83, 182, 88, 75, 85,
                    //    54, 53, 89, 59, 37, 35, 38, 29, 18, 45,
                    //    60, 49, 62, 55, 78, 96, 29, 22, 24, 13,
                    //    14, 11, 11, 18, 12, 12, 30, 52, 52, 44,
                    //    28, 28, 20, 56, 40, 31, 50, 40, 46, 42,
                    //    29, 19, 36, 25, 22, 17, 19, 26, 30, 20,
                    //    15, 21, 11, 8, 8, 19, 5, 8, 8, 11,
                    //    11, 8, 3, 9, 5, 4, 7, 3, 6, 3,
                    //    5, 4, 5, 6
                    //};

                    //str.AppendLine
                    //    (
                    //        "#" + "\t" +
                    //        "Chapter" + "\t" +
                    //        "Verse" + "\t" +
                    //        "Word" + "\t" +
                    //        "Stopmark"
                    //      );

                    //int count = 0;
                    //for (int v = 0; v < texts.Count; v++)
                    //{
                    //    string[] words = texts[v].Split();

                    //    int vv = v + 1;
                    //    int cc = 0 + 1;
                    //    foreach (int n in numbers)
                    //    {
                    //        if (vv <= n) break;

                    //        cc++;
                    //        vv -= n;
                    //    }

                    //    for (int w = 0; w < words.Length; w++)
                    //    {
                    //        if (words[w].Length == 1)
                    //        {
                    //            if (
                    //                Constants.STOPMARKS.Contains(words[w][0])
                    //                ||
                    //                Constants.QURANMARKS.Contains(words[w][0]))
                    //            {
                    //                count++;
                    //                str.AppendLine(
                    //                    count + "\t" +
                    //                    cc + "\t" +
                    //                    vv + "\t" +
                    //                    (w + 1) + "\t" +
                    //                    words[w][0]
                    //                  );
                    //            }
                    //        }
                    //    }
                    //}
                    //string filename = "Stopmarks" + Globals.OUTPUT_FILE_EXT;
                    //if (Directory.Exists(Globals.RESEARCH_FOLDER))
                    //{
                    //    string path = Globals.RESEARCH_FOLDER + "/" + filename;
                    //    FileHelper.SaveText(path, str.ToString());
                    //    FileHelper.DisplayFile(path);
                    //}

                    List<Stopmark> verse_stopmarks = DataAccess.LoadVerseStopmarks();

                    // remove bismAllah from 112 chapters
                    if (!with_bism_Allah)
                    {
                        string bimsAllah_text1 = "بِسْمِ ٱللَّهِ ٱلرَّحْمَٰنِ ٱلرَّحِيمِ ";
                        string bimsAllah_text2 = "بِّسْمِ ٱللَّهِ ٱلرَّحْمَٰنِ ٱلرَّحِيمِ "; // shadda on baa for chapter 95 and 97
                        for (int i = 0; i < texts.Count; i++)
                        {
                            if (texts[i].StartsWith(bimsAllah_text1))
                            {
                                texts[i] = texts[i].Replace(bimsAllah_text1, "");
                            }
                            else if (texts[i].StartsWith(bimsAllah_text2))
                            {
                                texts[i] = texts[i].Replace(bimsAllah_text2, "");
                            }
                        }
                    }

                    // replace shadda with previous letter before any simplification
                    if (shadda_as_letter)
                    {
                        for (int i = 0; i < texts.Count; i++)
                        {
                            StringBuilder str = new StringBuilder(texts[i]);
                            for (int j = 1; j < str.Length; j++)
                            {
                                if (str[j] == 'ّ')
                                {
                                    str[j] = str[j - 1];
                                }
                            }
                            texts[i] = str.ToString();
                        }
                    }

                    // simplify verse texts
                    List<string> verse_texts = new List<string>();
                    foreach (string original_verse_text in texts)
                    {
                        string verse_text = s_simplification_system.Simplify(original_verse_text);
                        verse_texts.Add(verse_text);
                    }

                    // buid verses
                    List<Verse> verses = new List<Verse>();
                    for (int i = 0; i < verse_texts.Count; i++)
                    {
                        Verse verse = new Verse(i + 1, verse_texts[i], verse_stopmarks[i]);
                        if (verse != null)
                        {
                            verses.Add(verse);
                            verse.ApplyWordStopmarks(texts[i]);
                        }
                    }

                    s_book = new Book(text_mode, verses);
                    if (s_book != null)
                    {
                        s_book.WithBismAllah = with_bism_Allah;
                        s_book.WawAsWord = waw_as_word;
                        s_book.ShaddaAsLetter = shadda_as_letter;

                        // build words before DataAccess.Loads
                        if (waw_as_word)
                        {
                            SplitWawsArWords(s_book, text_mode);
                        }
                        DataAccess.LoadWordMeanings(s_book);
                        DataAccess.LoadWordRoots(s_book);

                        // populate root-words dictionary
                        s_book.PopulateRootWords();
                    }
                }
            }
        }
    }
    private static void SplitWawsArWords(Book book, string text_mode)
    {
        if (book != null)
        {
            string filename = Globals.DATA_FOLDER + "/" + "waw-words.txt";
            if (File.Exists(filename))
            {
                List<string> exception_words = new List<string>();
                Dictionary<string, List<Verse>> non_exception_words_in_verses = new Dictionary<string, List<Verse>>();

                List<string> lines = FileHelper.LoadLines(filename);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('\t');
                    string exception_word = parts[0].SimplifyTo(text_mode);
                    exception_words.Add(exception_word);
                    if (parts.Length > 1)
                    {
                        string[] sub_parts = parts[1].Split(',');
                        foreach (string sub_part in sub_parts)
                        {
                            string[] verse_address_parts = sub_part.Split(':');
                            if (verse_address_parts.Length == 2)
                            {
                                try
                                {
                                    Chapter chapter = book.Chapters[int.Parse(verse_address_parts[0]) - 1];
                                    if (chapter != null)
                                    {
                                        Verse verse = chapter.Verses[int.Parse(verse_address_parts[1]) - 1];
                                        if (verse != null)
                                        {
                                            if (non_exception_words_in_verses.ContainsKey(exception_word))
                                            {
                                                List<Verse> verses = non_exception_words_in_verses[exception_word];
                                                if (verses != null)
                                                {
                                                    verses.Add(verse);
                                                    non_exception_words_in_verses.Add(exception_word, verses);
                                                }
                                            }
                                            else
                                            {
                                                List<Verse> verses = new List<Verse>();
                                                if (verses != null)
                                                {
                                                    verses.Add(verse);
                                                    non_exception_words_in_verses.Add(exception_word, verses);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    // skip error
                                }
                            }
                        }
                    }
                }

                foreach (Verse verse in book.Verses)
                {
                    StringBuilder str = new StringBuilder();
                    if (verse.Words.Count > 0)
                    {
                        for (int i = 0; i < verse.Words.Count; i++)
                        {
                            if (verse.Words[i].Text.StartsWith("و"))
                            {
                                if (!exception_words.Contains(verse.Words[i].Text))
                                {
                                    str.Append(verse.Words[i].Text.Insert(1, " ") + " ");
                                }
                                else // don't split exception words unless they are in non_exception_words_in_verses
                                {
                                    if (non_exception_words_in_verses.ContainsKey(verse.Words[i].Text))
                                    {
                                        if (non_exception_words_in_verses[verse.Words[i].Text].Contains(verse))
                                        {
                                            str.Append(verse.Words[i].Text.Insert(1, " ") + " ");
                                        }
                                        else
                                        {
                                            str.Append(verse.Words[i].Text + " ");
                                        }
                                    }
                                    else
                                    {
                                        str.Append(verse.Words[i].Text + " ");
                                    }
                                }
                            }
                            else
                            {
                                str.Append(verse.Words[i].Text + " ");
                            }
                        }
                        if (str.Length > 1)
                        {
                            str.Remove(str.Length - 1, 1); // " "
                        }
                    }

                    // re-create new Words with word stopmarks
                    verse.RecreateWordsApplyStopmarks(str.ToString());
                }

                // update verses/words/letters numbers and distances
                book.SetupBook();
            }
        }
    }

    // loaded numerology systems [STATIC]
    private static Dictionary<string, NumerologySystem> s_loaded_numerology_systems = null;
    public static Dictionary<string, NumerologySystem> LoadedNumerologySystems
    {
        get { return s_loaded_numerology_systems; }
    }
    private static void LoadNumerologySystems()
    {
        if (s_loaded_numerology_systems == null)
        {
            s_loaded_numerology_systems = new Dictionary<string, NumerologySystem>();
        }

        if (s_loaded_numerology_systems != null)
        {
            s_loaded_numerology_systems.Clear();

            if (Directory.Exists(Globals.VALUES_FOLDER))
            {
                string path = Globals.VALUES_FOLDER;
                DirectoryInfo folder = new DirectoryInfo(path);
                if (folder != null)
                {
                    FileInfo[] files = folder.GetFiles("*.txt");
                    if ((files != null) && (files.Length > 0))
                    {
                        foreach (FileInfo file in files)
                        {
                            string numerology_system_name = file.Name.Remove(file.Name.Length - 4, 4);
                            if (!String.IsNullOrEmpty(numerology_system_name))
                            {
                                if (numerology_system_name.Contains("DNA")) continue;

                                string[] parts = numerology_system_name.Split('_');
                                if (parts.Length == 3)
                                {
                                    LoadNumerologySystem(numerology_system_name);
                                }
                                else
                                {
                                    // skip invalid filename
                                    //throw new Exception("ERROR: " + file.FullName + " must contain 3 parts separated by \"_\".");
                                }
                            }
                        }

                        // start with default numerology system
                        if (s_loaded_numerology_systems.ContainsKey(NumerologySystem.DEFAULT_NAME))
                        {
                            s_numerology_system = new NumerologySystem(s_loaded_numerology_systems[NumerologySystem.DEFAULT_NAME]);
                        }
                        else
                        {
                            //throw new Exception("ERROR: No default numerology system was found.");
                        }
                    }
                }
            }
        }
    }
    // numerology system [DYNAMIC]
    private static NumerologySystem s_numerology_system = null;
    public static NumerologySystem NumerologySystem
    {
        get { return s_numerology_system; }
        set { s_numerology_system = value; }
    }
    public static void LoadNumerologySystem(string numerology_system_name)
    {
        if (String.IsNullOrEmpty(numerology_system_name)) return;
        if (numerology_system_name.Contains("DNA")) return;

        if (s_loaded_numerology_systems != null)
        {
            string filename = Globals.VALUES_FOLDER + "/" + numerology_system_name + ".txt";
            if (File.Exists(filename))
            {
                List<string> lines = FileHelper.LoadLines(filename);

                NumerologySystem numerology_system = new NumerologySystem(numerology_system_name);
                if (numerology_system != null)
                {
                    numerology_system.LetterValues.Clear();
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("#")) continue;

                        string[] parts = line.Split('\t');
                        if (parts.Length == 2)
                        {
                            try
                            {
                                numerology_system.LetterValues.Add(parts[0][0], long.Parse(parts[1]));
                            }
                            catch
                            {
                                //throw new Exception(filename + " file format must be:\r\n\tLetter TAB Value");
                            }
                        }
                        else
                        {
                            //throw new Exception(filename + " file format must be:\r\n\tLetter TAB Value");
                        }
                    }
                }

                if (s_loaded_numerology_systems.ContainsKey(numerology_system_name))
                {
                    s_loaded_numerology_systems[numerology_system.Name] = numerology_system;
                }
                else
                {
                    s_loaded_numerology_systems.Add(numerology_system.Name, numerology_system);
                }

                // set current numerology system
                if (s_loaded_numerology_systems.ContainsKey(numerology_system_name))
                {
                    s_numerology_system = new NumerologySystem(s_loaded_numerology_systems[numerology_system_name]);

                    // update chapter values for ChapterSortMethod.ByValue
                    if (s_numerology_system != null)
                    {
                        if (s_book != null)
                        {
                            foreach (Chapter chapter in s_book.Chapters)
                            {
                                CalculateValue(chapter);
                            }
                        }
                    }
                }
            }
        }
    }

    // letter statistics [DYNAMIC]
    private static List<LetterStatistic> s_letter_statistics = new List<LetterStatistic>();
    private static void BuildLetterStatistics(string text)
    {
        if (text == null) // null means Book scope
        {
            if (s_book != null)
            {
                text = s_book.Text;
            }
        }
        if (text != null)
        {
            if (s_letter_statistics != null)
            {
                s_letter_statistics.Clear();
                for (int i = 0; i < text.Length; i++)
                {
                    // calculate letter frequency
                    bool is_found = false;
                    for (int j = 0; j < s_letter_statistics.Count; j++)
                    {
                        if (text[i] == s_letter_statistics[j].Letter)
                        {
                            s_letter_statistics[j].Frequency++;
                            is_found = true;
                            break;
                        }
                    }

                    // add entry into dictionary
                    if (!is_found)
                    {
                        LetterStatistic letter_statistic = new LetterStatistic();
                        letter_statistic.Order = s_letter_statistics.Count + 1;
                        letter_statistic.Letter = text[i];
                        letter_statistic.Frequency++;
                        s_letter_statistics.Add(letter_statistic);
                    }
                }
            }
        }
    }

    // used for non-Quran text
    public static long CalculateValue(char user_char)
    {
        if (user_char == '\0') return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            result = s_numerology_system.CalculateValue(user_char);
        }
        return result;
    }
    public static long CalculateValue(string user_text)
    {
        if (string.IsNullOrEmpty(user_text)) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            result = s_numerology_system.CalculateValue(user_text);
        }
        return result;
    }
    // used for Quran text only
    public static long CalculateValue(Letter letter)
    {
        if (letter == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            // calculate the letter static value
            result += s_numerology_system.CalculateValue(letter.Character);
        }
        return result;
    }
    public static long CalculateValue(Word word)
    {
        if (word == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            foreach (Letter letter in word.Letters)
            {
                // calculate the letter static value
                result += s_numerology_system.CalculateValue(letter.Character);
            }
        }
        return result;
    }
    public static long CalculateValue(Sentence sentence)
    {
        if (sentence == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            List<Word> words = GetCompleteWords(sentence);
            if (words != null)
            {
                foreach (Word word in words)
                {
                    foreach (Letter letter in word.Letters)
                    {
                        // calculate the letter static value
                        result += s_numerology_system.CalculateValue(letter.Character);
                    }
                }
            }
        }
        return result;
    }
    private static List<Word> GetCompleteWords(Sentence sentence)
    {
        if (sentence == null) return null;

        List<Word> result = new List<Word>();
        if (sentence.FirstVerse.Number == sentence.LastVerse.Number)
        {
            foreach (Word word in sentence.FirstVerse.Words)
            {
                if ((word.Position >= sentence.StartPosition) && (word.Position < sentence.EndPosition))
                {
                    result.Add(word);
                }
            }
        }
        else // multi-verse
        {
            // first verse
            foreach (Word word in sentence.FirstVerse.Words)
            {
                if (word.Position >= sentence.StartPosition)
                {
                    result.Add(word);
                }
            }

            // middle verses
            int after_first_index = (sentence.FirstVerse.Number + 1) - 1;
            int before_last_index = (sentence.LastVerse.Number - 1) - 1;
            if (after_first_index <= before_last_index)
            {
                for (int i = after_first_index; i <= before_last_index; i++)
                {
                    result.AddRange(sentence.FirstVerse.Book.Verses[i].Words);
                }
            }

            // last verse
            foreach (Word word in sentence.LastVerse.Words)
            {
                if (word.Position < sentence.EndPosition) // not <= because EndPosition is after the start of the last word in the sentence
                {
                    result.Add(word);
                }
            }
        }
        return result;
    }
    private static List<Verse> GetCompleteVerses(Sentence sentence)
    {
        if (sentence == null) return null;

        List<Verse> result = new List<Verse>();
        if (sentence.FirstVerse.Number == sentence.LastVerse.Number)
        {
            if ((sentence.StartPosition == 0) && (sentence.EndPosition == sentence.Text.Length - 1))
            {
                result.Add(sentence.FirstVerse);
            }
        }
        else // multi-verse
        {
            // first verse
            if (sentence.StartPosition == 0)
            {
                result.Add(sentence.FirstVerse);
            }

            // middle verses
            int after_first_index = (sentence.FirstVerse.Number + 1) - 1;
            int before_last_index = (sentence.LastVerse.Number - 1) - 1;
            if (after_first_index <= before_last_index)
            {
                for (int i = after_first_index; i <= before_last_index; i++)
                {
                    result.Add(sentence.FirstVerse.Book.Verses[i]);
                }
            }

            // last verse
            if (sentence.EndPosition == sentence.LastVerse.Text.Length - 1)
            {
                result.Add(sentence.LastVerse);
            }
        }
        return result;
    }
    public static long CalculateValue(Verse verse)
    {
        if (verse == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            foreach (Word word in verse.Words)
            {
                foreach (Letter letter in word.Letters)
                {
                    // calculate the letter static value
                    result += s_numerology_system.CalculateValue(letter.Character);
                }
            }
        }
        return result;
    }
    public static long CalculateValue(List<Verse> verses)
    {
        if (verses == null) return 0L;
        if (verses.Count == 0) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            foreach (Verse verse in verses)
            {
                result += CalculateValue(verse);
            }
        }
        return result;
    }
    public static long CalculateValue(Chapter chapter)
    {
        if (chapter == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            result = CalculateValue(chapter.Verses);
            chapter.Value = result; // update chapter values for ChapterSortMethod.ByValue
        }
        return result;
    }
    public static long CalculateValue(Book book)
    {
        if (book == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            foreach (Chapter chapter in book.Chapters)
            {
                result += CalculateValue(chapter.Verses);
            }
        }
        return result;
    }
    public static long CalculateValue(List<Verse> verses, int letter_index_in_verse1, int letter_index_in_verse2)
    {
        if (verses == null) return 0L;
        if (verses.Count == 0) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (verses.Count == 1)
            {
                result += CalculateMiddlePartValue(verses[0], letter_index_in_verse1, letter_index_in_verse2);
            }
            else if (verses.Count == 2)
            {
                result += CalculateEndPartValue(verses[0], letter_index_in_verse1);
                result += CalculateBeginningPartValue(verses[1], letter_index_in_verse2);
            }
            else //if (verses.Count > 2)
            {
                result += CalculateEndPartValue(verses[0], letter_index_in_verse1);

                // middle verses
                for (int i = 1; i < verses.Count - 1; i++)
                {
                    result += CalculateValue(verses[i]);
                }

                result += CalculateBeginningPartValue(verses[verses.Count - 1], letter_index_in_verse2);
            }
        }
        return result;
    }
    private static long CalculateBeginningPartValue(Verse verse, int to_letter_index)
    {
        return CalculateMiddlePartValue(verse, 0, to_letter_index);
    }
    private static long CalculateMiddlePartValue(Verse verse, int from_letter_index, int to_letter_index)
    {
        if (verse == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            int word_index = -1;   // in verse
            int letter_index = -1; // in verse
            bool done = false;
            foreach (Word word in verse.Words)
            {
                word_index++;

                if ((word.Letters != null) && (word.Letters.Count > 0))
                {
                    foreach (Letter letter in word.Letters)
                    {
                        letter_index++;

                        if (letter_index < from_letter_index) continue;
                        if (letter_index > to_letter_index)
                        {
                            done = true;
                            break;
                        }

                        // calculate the letter static value
                        result += s_numerology_system.CalculateValue(letter.Character);
                    }

                    if (done) break;
                }
            }
        }
        return result;
    }
    private static long CalculateEndPartValue(Verse verse, int from_letter_index)
    {
        return CalculateMiddlePartValue(verse, from_letter_index, verse.LetterCount - 1);
    }
    private static List<Chapter> GetCompleteChapters(List<Verse> verses, int letter_index_in_verse1, int letter_index_in_verse2)
    {
        if (verses == null) return null;
        if (verses.Count == 0) return null;

        List<Chapter> result = new List<Chapter>();
        List<Verse> complete_verses = new List<Verse>(verses); // make a copy so we don't change the passed verses

        if (complete_verses != null)
        {
            if (complete_verses.Count > 0)
            {
                Verse first_verse = complete_verses[0];
                if (first_verse != null)
                {
                    if (letter_index_in_verse1 != 0)
                    {
                        complete_verses.Remove(first_verse);
                    }
                }

                if (complete_verses.Count > 0) // check again after maybe removing a verse
                {
                    Verse last_verse = complete_verses[complete_verses.Count - 1];
                    if (last_verse != null)
                    {
                        if (letter_index_in_verse2 != last_verse.LetterCount - 1)
                        {
                            complete_verses.Remove(last_verse);
                        }
                    }
                }

                if (complete_verses.Count > 0) // check again after maybe removing a verse
                {
                    foreach (Chapter chapter in s_book.Chapters)
                    {
                        bool include_chapter = true;
                        foreach (Verse v in chapter.Verses)
                        {
                            if (!complete_verses.Contains(v))
                            {
                                include_chapter = false;
                                break;
                            }
                        }

                        if (include_chapter)
                        {
                            result.Add(chapter);
                        }
                    }
                }
            }
        }

        return result;
    }

    // helper methods for finds
    public static List<Verse> GetSourceVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, TextLocationInChapter text_location_in_chapter)
    {
        List<Verse> verses = new List<Verse>();
        if (s_book != null)
        {
            if (search_scope == SearchScope.Book)
            {
                verses = s_book.Verses;
            }
            else if (search_scope == SearchScope.Selection)
            {
                verses = current_selection.Verses;
            }
            else if (search_scope == SearchScope.Result)
            {
                if (previous_verses != null)
                {
                    verses = new List<Verse>(previous_verses);
                }
            }
        }

        List<Verse> result = new List<Verse>();
        switch (text_location_in_chapter)
        {
            case TextLocationInChapter.AtStart:
                {
                    foreach (Verse verse in verses)
                    {
                        if (verse.NumberInChapter == 1)
                        {
                            result.Add(verse);
                        }
                    }
                }
                break;
            case TextLocationInChapter.AtMiddle:
                {
                    foreach (Verse verse in verses)
                    {
                        if ((verse.NumberInChapter != 1) && (verse.NumberInChapter != verse.Chapter.Verses.Count))
                        {
                            result.Add(verse);
                        }
                    }
                }
                break;
            case TextLocationInChapter.AtEnd:
                {
                    foreach (Verse verse in verses)
                    {
                        if (verse.NumberInChapter == verse.Chapter.Verses.Count)
                        {
                            result.Add(verse);
                        }
                    }
                }
                break;
            case TextLocationInChapter.Any:
            default:
                {
                    result = verses;
                }
                break;
        }
        return result;
    }
    public static List<Verse> GetVerses(List<Phrase> phrases)
    {
        List<Verse> result = new List<Verse>();
        if (phrases != null)
        {
            foreach (Phrase phrase in phrases)
            {
                if (phrase != null)
                {
                    if (!result.Contains(phrase.Verse))
                    {
                        result.Add(phrase.Verse);
                    }
                }
            }
        }
        return result;
    }
    public static List<Chapter> GetSourceChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses)
    {
        List<Chapter> result = new List<Chapter>();
        if (s_book != null)
        {
            if (search_scope == SearchScope.Book)
            {
                result = s_book.Chapters;
            }
            else if (search_scope == SearchScope.Selection)
            {
                result = current_selection.Chapters;
            }
            else if (search_scope == SearchScope.Result)
            {
                if (previous_verses != null)
                {
                    result = s_book.GetChapters(previous_verses);
                }
            }
        }
        return result;
    }
    public static List<Chapter> GetChapters(List<Phrase> phrases)
    {
        List<Chapter> result = new List<Chapter>();
        if (phrases != null)
        {
            foreach (Phrase phrase in phrases)
            {
                if (phrase != null)
                {
                    if (!result.Contains(phrase.Verse.Chapter))
                    {
                        result.Add(phrase.Verse.Chapter);
                    }
                }
            }
        }
        return result;
    }
    public static List<Phrase> BuildPhrases(Verse verse, MatchCollection matches)
    {
        List<Phrase> result = new List<Phrase>();
        foreach (Match match in matches)
        {
            foreach (Capture capture in match.Captures)
            {
                string text = capture.Value;
                int position = capture.Index;
                Phrase phrase = new Phrase(verse, position, text);
                if (phrase != null)
                {
                    result.Add(phrase);
                }
            }
        }
        return result;
    }
    private static List<Phrase> BuildPhrasesAndOriginify(Verse verse, MatchCollection matches)
    {
        List<Phrase> result = new List<Phrase>();
        foreach (Match match in matches)
        {
            foreach (Capture capture in match.Captures)
            {
                string text = capture.Value;
                int position = capture.Index;
                Phrase phrase = new Phrase(verse, position, text);
                if (phrase != null)
                {
                    if (s_numerology_system != null)
                    {
                        if (s_numerology_system.TextMode == "Original")
                        {
                            phrase = OriginifyPhrase(phrase);
                        }

                        if (phrase != null)
                        {
                            result.Add(phrase);
                        }
                    }
                }
            }
        }
        return result;
    }
    private static Phrase OriginifyPhrase(Phrase phrase)
    {
        if (phrase != null)
        {
            Verse verse = phrase.Verse;
            if (verse != null)
            {
                string text = phrase.Text;
                int position = phrase.Position;

                int start = 0;
                for (int i = 0; i < verse.Text.Length; i++)
                {
                    char character = verse.Text[i];

                    if ((character == ' ') || (Constants.ARABIC_LETTERS.Contains(character)))
                    {
                        start++;
                    }
                    else if ((Constants.STOPMARKS.Contains(character)) || (Constants.QURANMARKS.Contains(character)))
                    {
                        // superscript Seen letter in words وَيَبْصُۜطُ and بَصْۜطَةًۭ are not stopmarks
                        // Quran 2:245  مَّن ذَا ٱلَّذِى يُقْرِضُ ٱللَّهَ قَرْضًا حَسَنًۭا فَيُضَٰعِفَهُۥ لَهُۥٓ أَضْعَافًۭا كَثِيرَةًۭ ۚ وَٱللَّهُ يَقْبِضُ وَيَبْصُۜطُ وَإِلَيْهِ تُرْجَعُونَ
                        // Quran 7:69  أَوَعَجِبْتُمْ أَن جَآءَكُمْ ذِكْرٌۭ مِّن رَّبِّكُمْ عَلَىٰ رَجُلٍۢ مِّنكُمْ لِيُنذِرَكُمْ ۚ وَٱذْكُرُوٓا۟ إِذْ جَعَلَكُمْ خُلَفَآءَ مِنۢ بَعْدِ قَوْمِ نُوحٍۢ وَزَادَكُمْ فِى ٱلْخَلْقِ بَصْۜطَةًۭ ۖ فَٱذْكُرُوٓا۟ ءَالَآءَ ٱللَّهِ لَعَلَّكُمْ تُفْلِحُونَ
                        if
                            (
                                (character == 'ۜ') &&  // superscript Seen
                                (
                                    ((verse.Chapter.Number == 2) && (verse.NumberInChapter == 245))
                                    ||
                                    ((verse.Chapter.Number == 7) && (verse.NumberInChapter == 69))
                                )
                            )
                        {
                            // not a stopmark but a Seen above Ssad so treat as part in its word
                        }
                        else
                        {
                            start--; // skip the space after stopmark
                        }
                    }
                    else
                    {
                        // treat character as part of its word
                    }

                    // i has reached phrase start
                    if (start > position)
                    {
                        int phrase_length = text.Trim().Length;
                        StringBuilder str = new StringBuilder();

                        int length = 0;
                        for (int j = i; j < verse.Text.Length; j++)
                        {
                            character = verse.Text[j];
                            str.Append(character);

                            if ((character == ' ') || (Constants.ARABIC_LETTERS.Contains(character)))
                            {
                                length++;
                            }
                            else if ((Constants.STOPMARKS.Contains(character)) || (Constants.QURANMARKS.Contains(character)))
                            {
                                length--; // ignore space after stopmark
                                if (length < 0)
                                {
                                    length = 0;
                                }
                            }

                            // j has reached phrase end
                            if (length == phrase_length)
                            {
                                return new Phrase(verse, i, str.ToString());
                            }
                        }
                    }
                }
            }
        }
        return null;
    }
    public static Phrase SwitchTextMode(Phrase phrase, string to_text_mode)
    {
        if (phrase != null)
        {
            Verse phrase_verse = phrase.Verse;
            int phrase_position = phrase.Position;
            string phrase_text = phrase.Text;
            int phrase_length = phrase_text.Length;

            if (phrase_verse != null)
            {
                if (to_text_mode == "Original")
                {
                    Verse original_verse = phrase_verse;
                    int letter_count = 0;
                    int position = 0;
                    foreach (char c in original_verse.Text)
                    {
                        position++;
                        if ((c == ' ') || (Constants.ARABIC_LETTERS.Contains(c)))
                        {
                            letter_count++;
                        }

                        if (letter_count == phrase_position)
                        {
                            break;
                        }
                    }

                    foreach (char c in original_verse.Text)
                    {
                        position++;
                        if ((c == ' ') || (Constants.ARABIC_LETTERS.Contains(c)))
                        {
                            letter_count++;
                        }

                        if (letter_count == phrase_position)
                        {
                            break;
                        }
                    }

                    letter_count = 0;
                    StringBuilder str = new StringBuilder();
                    for (int i = position; i < phrase_verse.Text.Length; i++)
                    {
                        char character = phrase_verse.Text[i];
                        str.Append(character);

                        if ((character == ' ') || (Constants.ARABIC_LETTERS.Contains(character)))
                        {
                            letter_count++;
                        }
                        else if (Constants.STOPMARKS.Contains(character))
                        {
                            letter_count--; // decrement space after stopmark as it will be incremented above
                            if (letter_count < 0)
                            {
                                letter_count = 0;
                            }
                        }
                        else if (Constants.QURANMARKS.Contains(character))
                        {
                            letter_count--; // decrement space after quranmark as it will be incremented above
                        }

                        // check if finished
                        if (letter_count == phrase_length)
                        {
                            // skip any non-letter at start
                            int index = position;
                            if ((index > 0) && (index < phrase_verse.Text.Length))
                            {
                                character = phrase_verse.Text[index];
                                if (!Constants.ARABIC_LETTERS.Contains(character))
                                {
                                    position++;
                                    str.Append(" "); // increment length
                                }
                            }

                            // skip any non-letter at end
                            index = position + str.Length - 1;
                            if ((index > 0) && (position + str.Length < phrase_verse.Text.Length))
                            {
                                character = phrase_verse.Text[index];
                                if (!Constants.ARABIC_LETTERS.Contains(character))
                                {
                                    str.Append(" "); // increment length
                                }
                            }

                            return new Phrase(phrase_verse, position, str.ToString());
                        }
                    }
                }
            }
            else
            {
                // simplifiy text
                Verse simplified_verse = phrase_verse;
                phrase_text = phrase_text.SimplifyTo(to_text_mode);
                phrase_text = phrase_text.Trim();
                if (!String.IsNullOrEmpty(phrase_text)) // re-test in case text was just harakaat which is simplifed to nothing
                {
                    // simplifiy position
                    string verse_text = phrase_verse.Text.SimplifyTo(to_text_mode);
                    phrase_position = verse_text.IndexOf(phrase_text);  // will ONLY build first phrase occurrence in verse

                    // build simplified phrase
                    return new Phrase(phrase_verse, phrase_position, phrase_text);
                }

                //TODO: ConvertPhrase to simplified still not complete:  ONLY builds first phrase occurrence in verse
            }
        }
        return null;
    }

    // find by text - Exact
    public static List<Phrase> FindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, TextSearchBlockSize text_search_block_size, string text, LanguageType language_type, string translation, TextLocationInChapter text_location_in_chapter, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Phrase> result = new List<Phrase>();

        if (language_type == LanguageType.RightToLeft)
        {
            result = DoFindPhrases(search_scope, current_selection, previous_verses, text_search_block_size, text, language_type, translation, text_location_in_chapter, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, true);
        }
        else if (language_type == LanguageType.LeftToRight)
        {
            if (!String.IsNullOrEmpty(translation))
            {
                result = DoFindPhrases(search_scope, current_selection, previous_verses, text_search_block_size, text, language_type, translation, text_location_in_chapter, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, false);
            }
            else
            {
                if (s_book != null)
                {
                    if (s_book.Verses != null)
                    {
                        if (s_book.Verses.Count > 0)
                        {
                            foreach (string key in s_book.Verses[0].Translations.Keys)
                            {
                                List<Phrase> new_phrases = DoFindPhrases(search_scope, current_selection, previous_verses, text_search_block_size, text, language_type, key, text_location_in_chapter, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, false);
                                result.AddRange(new_phrases);
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, TextSearchBlockSize text_search_block_size, string text, LanguageType language_type, string translation, TextLocationInChapter text_location_in_chapter, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder, bool try_emlaaei_if_nothing_found)
    {
        List<Verse> verses = new List<Verse>();
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, text_location_in_chapter);
        if (language_type == LanguageType.RightToLeft)
        {
            return DoFindPhrases(source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, try_emlaaei_if_nothing_found);
        }
        else //if (language_type == FindByTextLanguageType.LeftToRight)
        {
            return DoFindPhrases(translation, source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
        }
    }
    private static List<Phrase> DoFindPhrases(List<Verse> source, Selection current_selection, List<Verse> previous_verses, string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder, bool try_emlaaei_if_nothing_found)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any
                    RegexOptions regex_options = RegexOptions.IgnoreCase | RegexOptions.RightToLeft;

                    try
                    {
                        if (with_diacritics)
                        {
                            string pattern = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                            if (!String.IsNullOrEmpty(pattern))
                            {
                                foreach (Verse verse in source)
                                {
                                    string verse_text = verse.Text.Trim();
                                    MatchCollection matches = Regex.Matches(verse_text, pattern, regex_options);
                                    if (multiplicity == -1) // without multiplicity
                                    {
                                        if (matches.Count > 0)
                                        {
                                            result.AddRange(BuildPhrases(verse, matches));
                                        }
                                    }
                                    else // with multiplicity
                                    {
                                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                        {
                                            if (matches.Count > 0)
                                            {
                                                result.AddRange(BuildPhrases(verse, matches));
                                            }
                                            else
                                            {
                                                result.Add(new Phrase(verse, 0, ""));
                                            }
                                        }
                                    }
                                } // end for
                            }
                        }

                        // if without diacritics or nothing found
                        if ((multiplicity != 0) && (result.Count == 0))
                        {
                            if (s_numerology_system != null)
                            {
                                text = text.SimplifyTo(s_numerology_system.TextMode);
                                if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                                {
                                    string pattern = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                                    if (!String.IsNullOrEmpty(pattern))
                                    {
                                        foreach (Verse verse in source)
                                        {
                                            string verse_text = verse.Text.SimplifyTo(s_numerology_system.TextMode);
                                            verse_text = verse_text.Trim();
                                            MatchCollection matches = Regex.Matches(verse_text, pattern, regex_options);
                                            if (multiplicity == -1) // without multiplicity
                                            {
                                                if (matches.Count > 0)
                                                {
                                                    result.AddRange(BuildPhrasesAndOriginify(verse, matches));
                                                    //result.Add(new Phrase(verse, 0, ""));
                                                }
                                            }
                                            else // with multiplicity
                                            {
                                                if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                                {
                                                    if (matches.Count > 0)
                                                    {
                                                        result.AddRange(BuildPhrasesAndOriginify(verse, matches));
                                                        //result.Add(new Phrase(verse, 0, ""));
                                                    }
                                                    else
                                                    {
                                                        result.Add(new Phrase(verse, 0, ""));
                                                    }
                                                }
                                            }
                                        } // end for
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(string translation, List<Verse> source, Selection current_selection, List<Verse> previous_verses, string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        if (String.IsNullOrEmpty(translation)) return null;

        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any
                    RegexOptions regex_options = case_sensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    if (text.IsArabic()) // Arabic letters in translation (Emlaaei, Urdu, Farsi, etc.) 
                    {
                        regex_options |= RegexOptions.RightToLeft;
                    }

                    string pattern = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                    foreach (Verse verse in source)
                    {
                        MatchCollection matches = Regex.Matches(verse.Translations[translation], pattern, regex_options);
                        if (multiplicity == -1) // without multiplicity
                        {
                            if (matches.Count > 0)
                            {
                                //result.AddRange(BuildPhrasesAndOriginify(verse, matches));
                                result.Add(new Phrase(verse, 0, ""));
                            }
                        }
                        else // with multiplicity
                        {
                            if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                            {
                                if (matches.Count > 0)
                                {
                                    //result.AddRange(BuildPhrasesAndOriginify(verse, matches));
                                    result.Add(new Phrase(verse, 0, ""));
                                }
                                else
                                {
                                    result.Add(new Phrase(verse, 0, ""));
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private static string BuildPattern(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness)
    {
        string pattern = null;

        if (String.IsNullOrEmpty(text)) return text;

        text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

        // search for Quran markers, stopmarks, numbers, etc.
        if (text.Length == 1)
        {
            if (!Constants.ARABIC_LETTERS.Contains(text[0]))
            {
                return text;
            }
        }

        /*
        =====================================================================
        Regular Expressions (RegEx)
        =====================================================================
        Best Reference: http://www.regular-expressions.info/
        =====================================================================
        Matches	Characters 
        x	character x 
        \\	backslash character 
        \0n	character with octal value 0n (0 <= n <= 7) 
        \0nn	character with octal value 0nn (0 <= n <= 7) 
        \0mnn	character with octal value 0mnn (0 <= m <= 3, 0 <= n <= 7) 
        \xhh	character with hexadecimal value 0xhh 
        \uhhhh	character with hexadecimal value 0xhhhh 
        \t	tab character ('\u0009') 
        \n	newline (line feed) character ('\u000A') 
        \r	carriage-return character ('\u000D') 
        \f	form-feed character ('\u000C') 
        \a	alert (bell) character ('\u0007') 
        \e	escape character ('\u001B') 
        \cx	control character corresponding to x 
                                  
        Character Classes 
        [abc]		    a, b, or c				                    (simple class) 
        [^abc]		    any character except a, b, or c		        (negation) 
        [a-zA-Z]	    a through z or A through Z, inclusive	    (range) 
        [a-d[m-p]]	    a through d, or m through p: [a-dm-p]	    (union) 
        [a-z&&[def]]	d, e, or f				                    (intersection) 
        [a-z&&[^bc]]	a through z, except for b and c: [ad-z]	    (subtraction) 
        [a-z&&[^m-p]]	a through z, and not m through p: [a-lq-z]  (subtraction) 
                                  
        Predefined 
        .	any character (inc line terminators) except newline 
        \d	digit				            [0-9] 
        \D	non-digit			            [^0-9] 
        \s	whitespace character		    [ \t\n\x0B\f\r] 
        \S	non-whitespace character	    [^\s] 
        \w	word character (alphanumeric)	[a-zA-Z_0-9] 
        \W	non-word character		        [^\w] 

        Boundary Matchers 
        ^	beginning of a line	(in Multiline)
        $	end of a line  		(in Multiline)
        \b	word boundary, including line start and line end
        \B	non-word boundary 
        \A	beginning of the input 
        \G	end of the previous match 
        \Z	end of the input but for the final terminator, if any 
        \z	end of the input

        Greedy quantifiers 
        X?	X, once or not at all 
        X*	X, zero or more times 
        X+	X, one or more times 
        X{n}	X, exactly n times 
        X{n,}	X, at least n times 
        X{n,m}	X, at least n but not more than m times 
                                  
        Reluctant quantifiers 
        X??	X, once or not at all 
        X*?	X, zero or more times 
        X+?	X, one or more times 
        X{n}?	X, exactly n times 
        X{n,}?	X, at least n times 
        X{n,m}?	X, at least n but not more than m times 
                                  
        Possessive quantifiers 
        X?+	X, once or not at all 
        X*+	X, zero or more times 
        X++	X, one or more times 
        X{n}+	X, exactly n times 
        X{n,}+	X, at least n times 
        X{n,m}+	X, at least n but not more than m times 

        positive lookahead	(?=text)
        negative lookahead	(?!text)
        // eg: not at end of line 	    (?!$)
        positive lookbehind	(?<=text)
        negative lookbehind	(?<!text)
        // eg: not at start of line 	(?<!^)
        =====================================================================
        */

        string pattern_empty_line = @"^$";
        string pattern_whole_line = "(" + @"^" + text + @"$" + ")";

        string pattern_any_with_prefix = "(" + @"\S+?" + text + ")";
        string pattern_any_with_prefix_and_suffix = "(" + @"\S+?" + text + @"\S+?" + ")";
        string pattern_any_with_suffix = "(" + text + @"\S+?" + ")";

        string pattern_word_with_prefix = "(" + pattern_any_with_prefix + @"\b" + ")";
        string pattern_word_with_prefix_and_suffix = "(" + pattern_any_with_prefix_and_suffix + ")";
        string pattern_word_with_suffix = "(" + @"\b" + pattern_any_with_suffix + ")";
        string pattern_word_with_any_fixes = "(" + pattern_word_with_prefix + "|" + pattern_word_with_prefix_and_suffix + "|" + pattern_any_with_suffix + ")";

        // Any == Whole word | Part of word
        string pattern_any_at_start = "(" + pattern_whole_line + "|" + @"^" + text + ")";
        string pattern_any_at_middle = "(" + pattern_whole_line + "|" + @"(?<!^)" + text + @"(?!$)" + ")";
        string pattern_any_at_end = "(" + pattern_whole_line + "|" + text + @"$" + ")";
        string pattern_any_anywhere = text;

        // Part of word
        string pattern_part_word_at_start = "(" + @"^" + pattern_word_with_any_fixes + ")";
        string pattern_part_word_at_middle = "(" + @"(?<!^)" + pattern_word_with_any_fixes + @"(?!$)" + ")";
        string pattern_part_word_at_end = "(" + pattern_word_with_any_fixes + @"$" + ")";
        string pattern_part_word_anywhere = "(" + pattern_part_word_at_start + "|" + pattern_part_word_at_middle + "|" + pattern_part_word_at_end + ")";

        // Whole word
        string pattern_whole_word_at_start = "(" + pattern_whole_line + "|" + @"^" + text + @"\b" + ")";
        string pattern_whole_word_at_middle = "(" + pattern_whole_line + "|" + @"(?<!^)" + @"\b" + text + @"\b" + @"(?!$)" + ")";
        string pattern_whole_word_at_end = "(" + pattern_whole_line + "|" + @"\b" + text + @"$" + ")";
        string pattern_whole_word_anywhere = "(" + pattern_whole_line + "|" + @"\b" + text + @"\b" + ")";

        switch (text_location_in_verse)
        {
            case TextLocationInVerse.Any:
                {
                    if (text_wordness == TextWordness.Any)
                    {
                        pattern += pattern_any_anywhere;
                    }
                    else if (text_wordness == TextWordness.PartOfWord)
                    {
                        pattern += pattern_part_word_anywhere;
                    }
                    else if (text_wordness == TextWordness.WholeWord)
                    {
                        pattern += pattern_whole_word_anywhere;
                    }
                    else
                    {
                        pattern += pattern_empty_line;
                    }
                }
                break;
            case TextLocationInVerse.AtStart:
                {
                    if (text_wordness == TextWordness.Any)
                    {
                        pattern += pattern_any_at_start;
                    }
                    else if (text_wordness == TextWordness.PartOfWord)
                    {
                        pattern += pattern_part_word_at_start;
                    }
                    else if (text_wordness == TextWordness.WholeWord)
                    {
                        pattern += pattern_whole_word_at_start;
                    }
                    else
                    {
                        pattern += pattern_empty_line;
                    }
                }
                break;
            case TextLocationInVerse.AtMiddle:
                {
                    if (text_wordness == TextWordness.Any)
                    {
                        pattern += pattern_any_at_middle;
                    }
                    else if (text_wordness == TextWordness.PartOfWord)
                    {
                        pattern += pattern_part_word_at_middle;
                    }
                    else if (text_wordness == TextWordness.WholeWord)
                    {
                        pattern += pattern_whole_word_at_middle;
                    }
                    else
                    {
                        pattern += pattern_empty_line;
                    }
                }
                break;
            case TextLocationInVerse.AtEnd:
                {
                    if (text_wordness == TextWordness.Any)
                    {
                        pattern += pattern_any_at_end;
                    }
                    else if (text_wordness == TextWordness.PartOfWord)
                    {
                        pattern += pattern_part_word_at_end;
                    }
                    else if (text_wordness == TextWordness.WholeWord)
                    {
                        pattern += pattern_whole_word_at_end;
                    }
                    else
                    {
                        pattern += pattern_empty_line;
                    }
                }
                break;
        }

        switch (text_location_in_word)
        {
            case TextLocationInWord.Any:
                {
                    // do noting
                }
                break;
            case TextLocationInWord.AtStart:
                {
                    pattern = @"(" + @"(?<=\b)" + pattern + @")"; // positive lookbehind
                }
                break;
            case TextLocationInWord.AtMiddle:
                {
                    pattern = @"(" + @"(?<!\s)" + pattern + @"(?!\s)" + @")"; // positive lookbehind and lookahead
                }
                break;
            case TextLocationInWord.AtEnd:
                {
                    pattern = @"(" + pattern + @"(?=\b)" + @")"; // positive lookahead
                }
                break;
        }

        return pattern;
    }
    // find by text - Proximity
    public static List<Phrase> FindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, TextSearchBlockSize text_search_block_size, string text, LanguageType language_type, string translation, TextProximityType text_proximity_type, TextWordness text_wordness, bool case_sensitive, bool with_diacritics)
    {
        List<Phrase> result = new List<Phrase>();

        if (language_type == LanguageType.RightToLeft)
        {
            result = DoFindPhrases(search_scope, current_selection, previous_verses, text_search_block_size, text, language_type, translation, text_proximity_type, text_wordness, case_sensitive, with_diacritics, true);
        }
        else if (language_type == LanguageType.LeftToRight)
        {
            if (!String.IsNullOrEmpty(translation))
            {
                result = DoFindPhrases(search_scope, current_selection, previous_verses, text_search_block_size, text, language_type, translation, text_proximity_type, text_wordness, case_sensitive, with_diacritics, false);
            }
            else
            {
                if (s_book != null)
                {
                    if (s_book.Verses != null)
                    {
                        if (s_book.Verses.Count > 0)
                        {
                            foreach (string key in s_book.Verses[0].Translations.Keys)
                            {
                                List<Phrase> new_phrases = DoFindPhrases(search_scope, current_selection, previous_verses, text_search_block_size, text, language_type, key, text_proximity_type, text_wordness, case_sensitive, with_diacritics, false);
                                result.AddRange(new_phrases);
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, TextSearchBlockSize text_search_block_size, string text, LanguageType language_type, string translation, TextProximityType text_proximity_type, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, bool try_emlaaei_if_nothing_found)
    {
        List<Verse> result = new List<Verse>();
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        if (language_type == LanguageType.RightToLeft)
        {
            return DoFindPhrases(source, current_selection, previous_verses, text, text_proximity_type, text_wordness, case_sensitive, with_diacritics, try_emlaaei_if_nothing_found);
        }
        else //if (language_type == FindByTextLanguageType.LeftToRight)
        {
            return DoFindPhrases(translation, source, current_selection, previous_verses, text, text_proximity_type, text_wordness, case_sensitive, with_diacritics);
        }
    }
    private static List<Phrase> DoFindPhrases(List<Verse> source, Selection current_selection, List<Verse> previous_verses, string text, TextProximityType text_proximity_type, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, bool try_emlaaei_if_nothing_found)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

                    List<string> unsigned_words = null;
                    List<string> positive_words = null;
                    List<string> negative_words = null;

                    try
                    {
                        if (with_diacritics)
                        {
                            BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                            foreach (Verse verse in source)
                            {
                                /////////////////////////
                                // process negative_words
                                /////////////////////////
                                if (negative_words.Count > 0)
                                {
                                    bool found = false;
                                    foreach (string negative_word in negative_words)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            string word_text = word.Text;
                                            if (text_wordness == TextWordness.Any)
                                            {
                                                if (word_text.Contains(negative_word))
                                                {
                                                    found = true; // next verse
                                                    break;
                                                }
                                            }
                                            else if (text_wordness == TextWordness.PartOfWord)
                                            {
                                                if ((word_text.Contains(negative_word)) && (word_text.Length > negative_word.Length))
                                                {
                                                    found = true; // next verse
                                                    break;
                                                }
                                            }
                                            else if (text_wordness == TextWordness.WholeWord)
                                            {
                                                if (word_text == negative_word)
                                                {
                                                    found = true; // next verse
                                                    break;
                                                }
                                            }
                                        }
                                        if (found)
                                        {
                                            break;
                                        }
                                    }
                                    if (found) continue; // next verse
                                }

                                /////////////////////////
                                // process positive_words
                                /////////////////////////
                                if (positive_words.Count > 0)
                                {
                                    int match_count = 0;
                                    foreach (string positive_word in positive_words)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            string word_text = word.Text;
                                            if (text_wordness == TextWordness.Any)
                                            {
                                                if (word_text.Contains(positive_word))
                                                {
                                                    match_count++;
                                                    break; // next positive_word
                                                }
                                            }
                                            else if (text_wordness == TextWordness.PartOfWord)
                                            {
                                                if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                {
                                                    match_count++;
                                                    break; // next positive_word
                                                }
                                            }
                                            else if (text_wordness == TextWordness.WholeWord)
                                            {
                                                if (word_text == positive_word)
                                                {
                                                    match_count++;
                                                    break; // next positive_word
                                                }
                                            }
                                        }
                                    }

                                    // verse failed test, so skip it
                                    if (match_count < positive_words.Count)
                                    {
                                        continue; // next verse
                                    }
                                }

                                //////////////////////////////////////////////////////
                                // both negative and positive conditions have been met
                                //////////////////////////////////////////////////////

                                /////////////////////////
                                // process unsigned_words
                                /////////////////////////
                                //////////////////////////////////////////////////////////
                                // FindByText WORDS All
                                //////////////////////////////////////////////////////////
                                if (text_proximity_type == TextProximityType.AllWords)
                                {
                                    int match_count = 0;
                                    foreach (string unsigned_word in unsigned_words)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            string word_text = word.Text;
                                            if (text_wordness == TextWordness.Any)
                                            {
                                                if (word_text.Contains(unsigned_word))
                                                {
                                                    match_count++;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                            else if (text_wordness == TextWordness.PartOfWord)
                                            {
                                                if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                {
                                                    match_count++;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                            else if (text_wordness == TextWordness.WholeWord)
                                            {
                                                if (word_text == unsigned_word)
                                                {
                                                    match_count++;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                        }
                                    }

                                    if (match_count == unsigned_words.Count)
                                    {
                                        ///////////////////////////////////////////////////////////////
                                        // all negative, positive and unsigned conditions have been met
                                        ///////////////////////////////////////////////////////////////

                                        // add positive matches
                                        foreach (string positive_word in positive_words)
                                        {
                                            foreach (Word word in verse.Words)
                                            {
                                                string word_text = word.Text;
                                                if (text_wordness == TextWordness.Any)
                                                {
                                                    if (word_text.Contains(positive_word))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (text_wordness == TextWordness.PartOfWord)
                                                {
                                                    if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (text_wordness == TextWordness.WholeWord)
                                                {
                                                    if (word_text == positive_word)
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                            }
                                        }

                                        // add unsigned matches
                                        foreach (string unsigned_word in unsigned_words)
                                        {
                                            foreach (Word word in verse.Words)
                                            {
                                                string word_text = word.Text;
                                                if (text_wordness == TextWordness.Any)
                                                {
                                                    if (word_text.Contains(unsigned_word))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (text_wordness == TextWordness.PartOfWord)
                                                {
                                                    if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (text_wordness == TextWordness.WholeWord)
                                                {
                                                    if (word_text == unsigned_word)
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else // verse failed test, so skip it
                                    {
                                        continue; // next verse
                                    }
                                }
                                //////////////////////////////////////////////////////////
                                // FindByText WORDS Any
                                //////////////////////////////////////////////////////////
                                else if (text_proximity_type == TextProximityType.AnyWord)
                                {
                                    bool found = false;
                                    foreach (string unsigned_word in unsigned_words)
                                    {
                                        foreach (Word word in verse.Words)
                                        {
                                            string word_text = word.Text;
                                            if (text_wordness == TextWordness.Any)
                                            {
                                                if (word_text.Contains(unsigned_word))
                                                {
                                                    found = true;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                            else if (text_wordness == TextWordness.PartOfWord)
                                            {
                                                if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                {
                                                    found = true;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                            else if (text_wordness == TextWordness.WholeWord)
                                            {
                                                if (word_text == unsigned_word)
                                                {
                                                    found = true;
                                                    break; // no need to continue even if there are more matches
                                                }
                                            }
                                        }
                                        if (found)
                                        {
                                            break;
                                        }
                                    }

                                    if (found) // found 1 unsigned word in verse, which is enough
                                    {
                                        ///////////////////////////////////////////////////////////////
                                        // all negative, positive and unsigned conditions have been met
                                        ///////////////////////////////////////////////////////////////

                                        // add positive matches
                                        foreach (string positive_word in positive_words)
                                        {
                                            foreach (Word word in verse.Words)
                                            {
                                                string word_text = word.Text;
                                                if (text_wordness == TextWordness.Any)
                                                {
                                                    if (word_text.Contains(positive_word))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (text_wordness == TextWordness.PartOfWord)
                                                {
                                                    if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (text_wordness == TextWordness.WholeWord)
                                                {
                                                    if (word_text == positive_word)
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                            }
                                        }

                                        // add unsigned matches
                                        foreach (string unsigned_word in unsigned_words)
                                        {
                                            foreach (Word word in verse.Words)
                                            {
                                                string word_text = word.Text;
                                                if (text_wordness == TextWordness.Any)
                                                {
                                                    if (word_text.Contains(unsigned_word))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (text_wordness == TextWordness.PartOfWord)
                                                {
                                                    if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                                else if (text_wordness == TextWordness.WholeWord)
                                                {
                                                    if (word_text == unsigned_word)
                                                    {
                                                        result.Add(new Phrase(verse, word.Position, word.Text));
                                                        //break; // no break in case there are more matches
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else // verse failed test, so skip it
                                    {
                                        continue; // next verse
                                    }
                                }
                            } // end for
                        }

                        // if without diacritics or nothing found
                        if (result.Count == 0)
                        {
                            if (s_numerology_system != null)
                            {
                                text = text.SimplifyTo(s_numerology_system.TextMode);
                                if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                                {
                                    BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                                    foreach (Verse verse in source)
                                    {
                                        /////////////////////////
                                        // process negative_words
                                        /////////////////////////
                                        if (negative_words.Count > 0)
                                        {
                                            bool found = false;
                                            foreach (string negative_word in negative_words)
                                            {
                                                foreach (Word word in verse.Words)
                                                {
                                                    string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                    if (text_wordness == TextWordness.Any)
                                                    {
                                                        if (word_text.Contains(negative_word))
                                                        {
                                                            found = true; // next verse
                                                            break;
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((word_text.Contains(negative_word)) && (word_text.Length > negative_word.Length))
                                                        {
                                                            found = true; // next verse
                                                            break;
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.WholeWord)
                                                    {
                                                        if (word_text == negative_word)
                                                        {
                                                            found = true; // next verse
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (found)
                                                {
                                                    break;
                                                }
                                            }
                                            if (found) continue; // next verse
                                        }

                                        /////////////////////////
                                        // process positive_words
                                        /////////////////////////
                                        if (positive_words.Count > 0)
                                        {
                                            int match_count = 0;
                                            foreach (string positive_word in positive_words)
                                            {
                                                foreach (Word word in verse.Words)
                                                {
                                                    // simplify all text_modes
                                                    string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                    if (text_wordness == TextWordness.Any)
                                                    {
                                                        if (word_text.Contains(positive_word))
                                                        {
                                                            match_count++;
                                                            break; // next positive_word
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                        {
                                                            match_count++;
                                                            break; // next positive_word
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.WholeWord)
                                                    {
                                                        if (word_text == positive_word)
                                                        {
                                                            match_count++;
                                                            break; // next positive_word
                                                        }
                                                    }
                                                }
                                            }

                                            // verse failed test, so skip it
                                            if (match_count < positive_words.Count)
                                            {
                                                continue; // next verse
                                            }
                                        }

                                        //////////////////////////////////////////////////////
                                        // both negative and positive conditions have been met
                                        //////////////////////////////////////////////////////

                                        /////////////////////////
                                        // process unsigned_words
                                        /////////////////////////
                                        //////////////////////////////////////////////////////////
                                        // FindByText WORDS All
                                        //////////////////////////////////////////////////////////
                                        if (text_proximity_type == TextProximityType.AllWords)
                                        {
                                            int match_count = 0;
                                            foreach (string unsigned_word in unsigned_words)
                                            {
                                                foreach (Word word in verse.Words)
                                                {
                                                    // simplify all text_modes
                                                    string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                    if (text_wordness == TextWordness.Any)
                                                    {
                                                        if (word_text.Contains(unsigned_word))
                                                        {
                                                            match_count++;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                        {
                                                            match_count++;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.WholeWord)
                                                    {
                                                        if (word_text == unsigned_word)
                                                        {
                                                            match_count++;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                }
                                            }

                                            if (match_count == unsigned_words.Count)
                                            {
                                                ///////////////////////////////////////////////////////////////
                                                // all negative, positive and unsigned conditions have been met
                                                ///////////////////////////////////////////////////////////////

                                                // add positive matches
                                                foreach (string positive_word in positive_words)
                                                {
                                                    foreach (Word word in verse.Words)
                                                    {
                                                        // simplify all text_modes
                                                        string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                        if (text_wordness == TextWordness.Any)
                                                        {
                                                            if (word_text.Contains(positive_word))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (text_wordness == TextWordness.PartOfWord)
                                                        {
                                                            if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (text_wordness == TextWordness.WholeWord)
                                                        {
                                                            if (word_text == positive_word)
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                    }
                                                }

                                                // add unsigned matches
                                                foreach (string unsigned_word in unsigned_words)
                                                {
                                                    foreach (Word word in verse.Words)
                                                    {
                                                        // simplify all text_modes
                                                        string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                        if (text_wordness == TextWordness.Any)
                                                        {
                                                            if (word_text.Contains(unsigned_word))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (text_wordness == TextWordness.PartOfWord)
                                                        {
                                                            if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (text_wordness == TextWordness.WholeWord)
                                                        {
                                                            if (word_text == unsigned_word)
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else // verse failed test, so skip it
                                            {
                                                continue; // next verse
                                            }
                                        }
                                        //////////////////////////////////////////////////////////
                                        // FindByText WORDS Any
                                        //////////////////////////////////////////////////////////
                                        else if (text_proximity_type == TextProximityType.AnyWord)
                                        {
                                            bool found = false;
                                            foreach (string unsigned_word in unsigned_words)
                                            {
                                                foreach (Word word in verse.Words)
                                                {
                                                    // simplify all text_modes
                                                    string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                    if (text_wordness == TextWordness.Any)
                                                    {
                                                        if (word_text.Contains(unsigned_word))
                                                        {
                                                            found = true;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                        {
                                                            found = true;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.WholeWord)
                                                    {
                                                        if (word_text == unsigned_word)
                                                        {
                                                            found = true;
                                                            break; // no need to continue even if there are more matches
                                                        }
                                                    }
                                                }
                                                if (found)
                                                {
                                                    break;
                                                }
                                            }

                                            if (found) // found 1 unsigned word in verse, which is enough
                                            {
                                                ///////////////////////////////////////////////////////////////
                                                // all negative, positive and unsigned conditions have been met
                                                ///////////////////////////////////////////////////////////////

                                                // add positive matches
                                                foreach (string positive_word in positive_words)
                                                {
                                                    foreach (Word word in verse.Words)
                                                    {
                                                        // simplify all text_modes
                                                        string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                        if (text_wordness == TextWordness.Any)
                                                        {
                                                            if (word_text.Contains(positive_word))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (text_wordness == TextWordness.PartOfWord)
                                                        {
                                                            if ((word_text.Contains(positive_word)) && (word_text.Length > positive_word.Length))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (text_wordness == TextWordness.WholeWord)
                                                        {
                                                            if (word_text == positive_word)
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                    }
                                                }

                                                // add unsigned matches
                                                foreach (string unsigned_word in unsigned_words)
                                                {
                                                    foreach (Word word in verse.Words)
                                                    {
                                                        // simplify all text_modes
                                                        string word_text = word.Text.SimplifyTo(s_numerology_system.TextMode);
                                                        if (text_wordness == TextWordness.Any)
                                                        {
                                                            if (word_text.Contains(unsigned_word))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (text_wordness == TextWordness.PartOfWord)
                                                        {
                                                            if ((word_text.Contains(unsigned_word)) && (word_text.Length > unsigned_word.Length))
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                        else if (text_wordness == TextWordness.WholeWord)
                                                        {
                                                            if (word_text == unsigned_word)
                                                            {
                                                                result.Add(new Phrase(verse, word.Position, word.Text));
                                                                //break; // no break in case there are more matches
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else // verse failed test, so skip it
                                            {
                                                continue; // next verse
                                            }
                                        }
                                    } // end for
                                }
                            }
                        }
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(string translation, List<Verse> source, Selection current_selection, List<Verse> previous_verses, string text, TextProximityType text_proximity_type, TextWordness text_wordness, bool case_sensitive, bool with_diacritics)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = text.SimplifyTo(s_numerology_system.TextMode);
                    text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

                    RegexOptions regex_options = case_sensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    if (text.IsArabic()) // Arabic letters in translation (Emlaaei, Urdu, Farsi, etc.) 
                    {
                        regex_options |= RegexOptions.RightToLeft;
                    }

                    try
                    {
                        List<string> negative_words = new List<string>();
                        List<string> positive_words = new List<string>();
                        List<string> unsigned_words = new List<string>();

                        BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                        foreach (Verse verse in source)
                        {
                            if (text_proximity_type == TextProximityType.AllWords)
                            {
                                bool found = false;
                                foreach (string negative_word in negative_words)
                                {
                                    if (text_wordness == TextWordness.Any)
                                    {
                                        if (verse.Translations[translation].Contains(negative_word))
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                    else if (text_wordness == TextWordness.PartOfWord)
                                    {
                                        if ((verse.Translations[translation].Contains(negative_word)) && (verse.Translations[translation].Length > negative_word.Length))
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                    else if (text_wordness == TextWordness.WholeWord)
                                    {
                                        if (verse.Translations[translation] == negative_word)
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                }
                                if (found) continue;

                                foreach (string positive_word in positive_words)
                                {
                                    if (text_wordness == TextWordness.Any)
                                    {
                                        if (!verse.Translations[translation].Contains(positive_word))
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                    else if (text_wordness == TextWordness.PartOfWord)
                                    {
                                        if (!(verse.Translations[translation].Contains(positive_word)) || !(verse.Translations[translation].Length > positive_word.Length))
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                    else if (text_wordness != TextWordness.WholeWord)
                                    {
                                        if (verse.Translations[translation] == positive_word)
                                        {
                                            found = true; // next verse
                                            break;
                                        }
                                    }
                                }
                                if (found) continue;

                                if (
                                     (unsigned_words.Count == 0) ||
                                     (verse.Translations[translation].ContainsWordsOf(unsigned_words))
                                   )
                                {
                                    result.Add(new Phrase(verse, 0, ""));
                                }
                            }
                            else if (text_proximity_type == TextProximityType.AnyWord)
                            {
                                bool found = false;
                                foreach (string negative_word in negative_words)
                                {
                                    if (text_wordness == TextWordness.Any)
                                    {
                                        if (verse.Translations[translation].Contains(negative_word))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (text_wordness == TextWordness.PartOfWord)
                                    {
                                        if ((verse.Translations[translation].Contains(negative_word)) && (verse.Translations[translation].Length > negative_word.Length))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (text_wordness == TextWordness.WholeWord)
                                    {
                                        if (verse.Translations[translation] == negative_word)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                                if (found) continue;

                                foreach (string positive_word in positive_words)
                                {
                                    if (text_wordness == TextWordness.Any)
                                    {
                                        if (!verse.Translations[translation].Contains(positive_word))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (text_wordness == TextWordness.PartOfWord)
                                    {
                                        if (!(verse.Translations[translation].Contains(positive_word)) || !(verse.Translations[translation].Length > positive_word.Length))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    else if (text_wordness != TextWordness.WholeWord)
                                    {
                                        if (verse.Translations[translation] == positive_word)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                                if (found) continue;

                                if (
                                     (negative_words.Count > 0) ||
                                     (positive_words.Count > 0) ||
                                     (
                                       (unsigned_words.Count == 0) ||
                                       (verse.Translations[translation].ContainsWordOf(unsigned_words))
                                     )
                                   )
                                {
                                    result.Add(new Phrase(verse, 0, ""));
                                }
                            }
                        } // end for
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }
        return result;
    }
    private static void BuildWordLists(string text, out List<string> unsigned_words, out List<string> positive_words, out List<string> negative_words)
    {
        unsigned_words = new List<string>();
        positive_words = new List<string>();
        negative_words = new List<string>();

        if (String.IsNullOrEmpty(text)) return;
        text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any
        text = text.Trim();

        string[] text_words = text.Split();
        foreach (string text_word in text_words)
        {
            if (text_word.StartsWith("-"))
            {
                negative_words.Add(text_word.Substring(1));
            }
            else if (text_word.EndsWith("-"))
            {
                negative_words.Add(text_word.Substring(0, text_word.Length - 1));
            }
            else if (text_word.StartsWith("+"))
            {
                positive_words.Add(text_word.Substring(1));
            }
            else if (text_word.EndsWith("+"))
            {
                positive_words.Add(text_word.Substring(0, text_word.Length - 1));
            }
            else
            {
                unsigned_words.Add(text_word);
            }
        }
    }
    // find by text - Root
    public static List<Phrase> FindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, TextSearchBlockSize text_search_block_size, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Phrase> result = new List<Phrase>();
        if (String.IsNullOrEmpty(text)) return null;
        text = text.Simplify36();   // roots use 36 letters
        while (text.Contains("  "))
        {
            text = text.Replace("  ", " ");
        }

        string[] parts = text.Split();
        if (parts.Length > 0) // enable nested searches
        {
            List<Phrase> current_result = null;

            List<string> negative_words = new List<string>();
            List<string> positive_words = new List<string>();
            List<string> unsigned_words = new List<string>();
            BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

            foreach (string negative_word in negative_words)
            {
                current_result = DoFindPhrases(search_scope, current_selection, previous_verses, text_search_block_size, negative_word, 0, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder); // multiplicity = 0 for exclude
                MergePhrases(current_result, ref previous_verses, ref result);
                search_scope = SearchScope.Result;
            }

            foreach (string positive_word in positive_words)
            {
                current_result = DoFindPhrases(search_scope, current_selection, previous_verses, text_search_block_size, positive_word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                MergePhrases(current_result, ref previous_verses, ref result);
                search_scope = SearchScope.Result;
            }

            foreach (string unsigned_word in unsigned_words)
            {
                current_result = DoFindPhrases(search_scope, current_selection, previous_verses, text_search_block_size, unsigned_word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                MergePhrases(current_result, ref previous_verses, ref result);
                search_scope = SearchScope.Result;
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, TextSearchBlockSize text_search_block_size, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Verse> result = new List<Verse>();
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindPhrases(source, text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
    }
    private static List<Phrase> DoFindPhrases(List<Verse> source, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    try
                    {
                        if (s_book != null)
                        {
                            SortedDictionary<string, List<Word>> root_words_dictionary = s_book.RootWords;
                            if (root_words_dictionary != null)
                            {
                                List<Word> root_words = null;
                                if (root_words_dictionary.ContainsKey(text))
                                {
                                    // get all pre-identified root_words
                                    root_words = root_words_dictionary[text];
                                }
                                else // if no such root, search for the matching root_word by its verse position and get its root and then get all root_words
                                {
                                    string new_root = s_book.GetBestRoot(text);
                                    if (!String.IsNullOrEmpty(new_root))
                                    {
                                        // get all pre-identified root_words for new root
                                        root_words = root_words_dictionary[new_root];
                                    }
                                }

                                if (root_words != null)
                                {
                                    result = GetPhrasesWithRootWords(source, root_words, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // log exception
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> GetPhrasesWithRootWords(List<Verse> source, List<Word> root_words, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    Dictionary<Verse, int> multiplicity_dictionary = new Dictionary<Verse, int>();
                    foreach (Word word in root_words)
                    {
                        Verse verse = s_book.Verses[word.Verse.Number - 1];
                        if (source.Contains(verse))
                        {
                            if (multiplicity_dictionary.ContainsKey(verse))
                            {
                                multiplicity_dictionary[verse]++;
                            }
                            else // first found
                            {
                                multiplicity_dictionary.Add(verse, 1);
                            }
                        }
                    }

                    if (multiplicity == 0) // verses not containg word
                    {
                        foreach (Verse verse in source)
                        {
                            if (!multiplicity_dictionary.ContainsKey(verse))
                            {
                                Phrase phrase = new Phrase(verse, 0, "");
                                result.Add(phrase);
                            }
                        }
                    }
                    else // add only matching multiplicity or wildcard (-1)
                    {
                        foreach (Word word in root_words)
                        {
                            int verse_index = word.Verse.Number - 1;
                            if ((verse_index >= 0) && (verse_index < s_book.Verses.Count))
                            {
                                Verse verse = s_book.Verses[verse_index];
                                if (multiplicity_dictionary.ContainsKey(verse))
                                {
                                    if ((multiplicity == -1) || (Compare(multiplicity_dictionary[verse], multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder)))
                                    {
                                        if (source.Contains(verse))
                                        {
                                            int word_index = word.NumberInVerse - 1;
                                            if ((word_index >= 0) && (word_index < verse.Words.Count))
                                            {
                                                Word verse_word = verse.Words[word_index];
                                                string word_text = verse_word.Text;
                                                int word_position = verse_word.Position;
                                                Phrase phrase = new Phrase(verse, word_position, word_text);
                                                result.Add(phrase);
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
        return result;
    }
    private static void MergePhrases(List<Phrase> current_phrases, ref List<Verse> previous_verses, ref List<Phrase> previous_phrases)
    {
        if (current_phrases != null)
        {
            if (previous_phrases != null)
            {
                // extract verses from current phrases
                previous_verses = new List<Verse>(GetVerses(current_phrases));

                if (previous_verses != null)
                {
                    if (previous_phrases.Count == 0)
                    {
                        previous_phrases = current_phrases;
                    }
                    else
                    {
                        // add current phrases
                        List<Phrase> total = new List<Phrase>(current_phrases);

                        // add previous phrases if their verses exist in current phrases
                        foreach (Phrase previous_phrase in previous_phrases)
                        {
                            if (previous_phrase != null)
                            {
                                if (previous_verses.Contains(previous_phrase.Verse))
                                {
                                    total.Add(previous_phrase);
                                }
                            }
                        }
                        previous_phrases = total;
                    }
                }
            }
        }
    }
    // find by root - verses with related words
    public static List<Verse> FindRelatedVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, Verse verse)
    {
        return DoFindRelatedVerses(search_scope, current_selection, previous_result, verse);
    }
    private static List<Verse> DoFindRelatedVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, Verse verse)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result, TextLocationInChapter.Any);
        return DoFindRelatedVerses(source, current_selection, previous_result, verse);
    }
    private static List<Verse> DoFindRelatedVerses(List<Verse> source, Selection current_selection, List<Verse> previous_result, Verse verse)
    {
        List<Verse> result = new List<Verse>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (verse != null)
                {
                    for (int j = 0; j < source.Count; j++)
                    {
                        if (verse.HasRelatedWordsTo(source[j]))
                        {
                            result.Add(source[j]);
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by text - consecutively repeated phrases
    public static List<Phrase> FindRepeatedPhrases(int phrase_word_count, bool with_diacritics)
    {
        if (s_book == null) return null;

        List<Verse> source = s_book.Verses;
        return FindRepeatedPhrases(source, phrase_word_count, with_diacritics);
    }
    private static List<Phrase> FindRepeatedPhrases(List<Verse> source, int phrase_word_count, bool with_diacritics)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (phrase_word_count > 0)
                {
                    List<Word> words = new List<Word>();
                    foreach (Verse verse in source)
                    {
                        words.AddRange(verse.Words);
                    }

                    int n = phrase_word_count;
                    for (int i = 0; i <= words.Count - (2 * n); i++)
                    {
                        string original_phrase1 = null;
                        for (int w = 0; w < n; w++)
                        {
                            original_phrase1 += words[i + w].Text;
                        }
                        string original_phrase2 = null;
                        for (int w = 0; w < n; w++)
                        {
                            original_phrase2 += words[i + n + w].Text;
                        }

                        string simplified_phrase1 = original_phrase1;
                        string simplified_phrase2 = original_phrase2;
                        if (!with_diacritics)
                        {
                            simplified_phrase1 = simplified_phrase1.SimplifyTo(NumerologySystem.TextMode);
                            simplified_phrase2 = simplified_phrase2.SimplifyTo(NumerologySystem.TextMode);
                        }
                        if (simplified_phrase1 == simplified_phrase2)
                        {
                            int spaces = n - 1;
                            result.Add(new Phrase(words[i].Verse, words[i].Position, original_phrase1 + spaces));
                            result.Add(new Phrase(words[i + n].Verse, words[i + n].Position, original_phrase2 + spaces));
                        }
                    }
                }
            }
        }
        return result;
    }

    // find by numbers - helper methods
    private static void CalculateSums(Word word, out int letter_sum)
    {
        letter_sum = 0;
        if (word != null)
        {
            if ((word.Letters != null) && (word.Letters.Count > 0))
            {
                foreach (Letter letter in word.Letters)
                {
                    letter_sum += letter.NumberInWord;
                }
            }
        }
    }
    private static void CalculateSums(List<Word> words, out int word_sum, out int letter_sum)
    {
        word_sum = 0;
        letter_sum = 0;
        if (words != null)
        {
            foreach (Word word in words)
            {
                word_sum += word.NumberInVerse;

                if ((word.Letters != null) && (word.Letters.Count > 0))
                {
                    foreach (Letter letter in word.Letters)
                    {
                        letter_sum += letter.NumberInWord;
                    }
                }
            }
        }
    }
    private static void CalculateSums(Verse verse, out int word_sum, out int letter_sum)
    {
        word_sum = 0;
        letter_sum = 0;
        if (verse != null)
        {
            if (verse.Words != null)
            {
                foreach (Word word in verse.Words)
                {
                    word_sum += word.NumberInVerse;

                    if ((word.Letters != null) && (word.Letters.Count > 0))
                    {
                        foreach (Letter letter in word.Letters)
                        {
                            letter_sum += letter.NumberInWord;
                        }
                    }
                }
            }
        }
    }
    private static void CalculateSums(List<Verse> verses, out int chapter_sum, out int verse_sum, out int word_sum, out int letter_sum)
    {
        chapter_sum = 0;
        verse_sum = 0;
        word_sum = 0;
        letter_sum = 0;
        if (verses != null)
        {
            if (s_book != null)
            {
                List<Chapter> chapters = s_book.GetChapters(verses);
                if (chapters != null)
                {
                    foreach (Chapter chapter in chapters)
                    {
                        if (chapter != null)
                        {
                            chapter_sum += chapter.SortedNumber;
                        }
                    }

                    foreach (Verse verse in verses)
                    {
                        verse_sum += verse.NumberInChapter;
                        if (verse.Words != null)
                        {
                            foreach (Word word in verse.Words)
                            {
                                word_sum += word.NumberInVerse;

                                if ((word.Letters != null) && (word.Letters.Count > 0))
                                {
                                    foreach (Letter letter in word.Letters)
                                    {
                                        letter_sum += letter.NumberInWord;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private static void CalculateSums(Chapter chapter, out int verse_sum, out int word_sum, out int letter_sum)
    {
        verse_sum = 0;
        word_sum = 0;
        letter_sum = 0;
        if (chapter != null)
        {
            foreach (Verse verse in chapter.Verses)
            {
                verse_sum += verse.NumberInChapter;
                if (verse.Words != null)
                {
                    foreach (Word word in verse.Words)
                    {
                        word_sum += word.NumberInVerse;

                        if ((word.Letters != null) && (word.Letters.Count > 0))
                        {
                            foreach (Letter letter in word.Letters)
                            {
                                letter_sum += letter.NumberInWord;
                            }
                        }
                    }
                }
            }
        }
    }
    private static void CalculateSums(List<Chapter> chapters, out int chapter_sum, out int verse_sum, out int word_sum, out int letter_sum)
    {
        chapter_sum = 0;
        verse_sum = 0;
        word_sum = 0;
        letter_sum = 0;
        if (chapters != null)
        {
            foreach (Chapter chapter in chapters)
            {
                if (chapter != null)
                {
                    chapter_sum += chapter.SortedNumber;

                    foreach (Verse verse in chapter.Verses)
                    {
                        verse_sum += verse.NumberInChapter;
                        if (verse.Words != null)
                        {
                            foreach (Word word in verse.Words)
                            {
                                word_sum += word.NumberInVerse;

                                if ((word.Letters != null) && (word.Letters.Count > 0))
                                {
                                    foreach (Letter letter in word.Letters)
                                    {
                                        letter_sum += letter.NumberInWord;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private static bool Compare(Word word, NumberQuery query)
    {
        if (word != null)
        {
            int number = 0;
            switch (query.NumberScope)
            {
                case NumberScope.Number:
                    number = word.Number;
                    break;
                case NumberScope.NumberInChapter:
                    number = word.NumberInChapter;
                    break;
                case NumberScope.NumberInVerse:
                    number = word.NumberInVerse;
                    break;
                default:
                    number = word.NumberInVerse;
                    break;
            }
            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Natural))
            {
                if (query.Number < 0)
                {
                    switch (query.NumberScope)
                    {
                        case NumberScope.Number:
                            query.Number = word.Verse.Chapter.Book.WordCount + query.Number + 1;
                            break;
                        case NumberScope.NumberInChapter:
                            query.Number = word.Verse.Chapter.WordCount + query.Number + 1;
                            break;
                        case NumberScope.NumberInVerse:
                            query.Number = word.Verse.Words.Count + query.Number + 1;
                            break;
                        default:
                            query.Number = word.Verse.Chapter.WordCount + query.Number + 1;
                            break;
                    }
                }
                if (query.Number > 0)
                {
                    if (!Numbers.Compare(number, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(number, query.NumberNumberType))
                {
                    return false;
                }
            }

            if (query.LetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(word.Letters.Count, number, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                {
                    return false;
                }
            }
            else if (query.LetterCountNumberType == NumberType.None)
            {
                if (query.LetterCount > 0)
                {
                    if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int letter_sum;
                        CalculateSums(word, out letter_sum);
                        if (!Numbers.Compare(letter_sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!Numbers.Compare(word.Letters.Count, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int letter_sum;
                    CalculateSums(word, out letter_sum);
                    if (!Numbers.IsNumberType(letter_sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Numbers.IsNumberType(word.Letters.Count, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if (query.UniqueLetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(word.UniqueLetters.Count, number, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                {
                    return false;
                }
            }
            else if (query.UniqueLetterCountNumberType == NumberType.None)
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(word.UniqueLetters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(word.UniqueLetters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            long value = CalculateValue(word);
            if (query.Value > 0)
            {
                if (query.ValueNumberType == NumberType.Natural)
                {
                    if (!Numbers.Compare(value, number, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                    {
                        return false;
                    }
                }
                else if (query.ValueNumberType == NumberType.None)
                {
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Numbers.IsNumberType(value, query.ValueNumberType))
                    {
                        return false;
                    }
                }
            }

            // value digit sum
            if (query.ValueDigitSum > 0)
            {
                if (value == 0L) { value = CalculateValue(word); }
                if (Numbers.DigitSum(value) != query.ValueDigitSum)
                {
                    return false;
                }
            }

            // value digital root
            if (query.ValueDigitalRoot > 0)
            {
                if (value == 0L) { value = CalculateValue(word); }
                if (Numbers.DigitalRoot(value) != query.ValueDigitalRoot)
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(List<Word> words, NumberQuery query)
    {
        if (words != null)
        {
            long value = 0L;

            int sum = 0;
            switch (query.NumberScope)
            {
                case NumberScope.Number:
                    foreach (Word word in words)
                    {
                        sum += word.Number;
                    }
                    break;
                case NumberScope.NumberInChapter:
                    foreach (Word word in words)
                    {
                        sum += word.NumberInChapter;
                    }
                    break;
                case NumberScope.NumberInVerse:
                    foreach (Word word in words)
                    {
                        sum += word.NumberInVerse;
                    }
                    break;
                default:
                    foreach (Word word in words)
                    {
                        sum += word.NumberInVerse;
                    }
                    break;
            }
            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Natural))
            {
                if (query.Number > 0)
                {
                    if (!Numbers.Compare(sum, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(sum, query.NumberNumberType))
                {
                    return false;
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Natural))
            {
                if (query.LetterCount > 0)
                {
                    if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int word_sum; int letter_sum;
                        CalculateSums(words, out word_sum, out letter_sum);
                        if (!Numbers.Compare(letter_sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        sum = 0;
                        foreach (Word word in words)
                        {
                            sum += word.Letters.Count;
                        }
                        if (!Numbers.Compare(sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int word_sum; int letter_sum;
                    CalculateSums(words, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(letter_sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    sum = 0;
                    foreach (Word word in words)
                    {
                        sum += word.Letters.Count;
                    }
                    if (!Numbers.IsNumberType(sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Natural))
            {
                if (query.UniqueLetterCount > 0)
                {
                    List<char> unique_letters = new List<char>();
                    foreach (Word word in words)
                    {
                        foreach (char character in word.UniqueLetters)
                        {
                            if (!unique_letters.Contains(character))
                            {
                                unique_letters.Add(character);
                            }
                        }
                    }
                    if (!Numbers.Compare(unique_letters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                List<char> unique_letters = new List<char>();
                foreach (Word word in words)
                {
                    foreach (char character in word.UniqueLetters)
                    {
                        if (!unique_letters.Contains(character))
                        {
                            unique_letters.Add(character);
                        }
                    }
                }
                if (!Numbers.IsNumberType(unique_letters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Natural))
            {
                if (query.Value > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Word word in words)
                        {
                            value += CalculateValue(word);
                        }
                    }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Word word in words)
                    {
                        value += CalculateValue(word);
                    }
                }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }


            // value digit sums
            if (query.ValueDigitSum > 0)
            {
                if (value == 0L)
                {
                    foreach (Word word in words)
                    {
                        value += CalculateValue(word);
                    }
                }
                if (Numbers.DigitSum(value) != query.ValueDigitSum)
                {
                    return false;
                }
            }

            // value digit root
            if (query.ValueDigitalRoot > 0)
            {
                if (value == 0L)
                {
                    foreach (Word word in words)
                    {
                        value += CalculateValue(word);
                    }
                }
                if (Numbers.DigitalRoot(value) != query.ValueDigitalRoot)
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(Sentence sentence, NumberQuery query)
    {
        if (sentence != null)
        {
            long value = 0L;

            if ((query.WordCountNumberType == NumberType.None) || (query.WordCountNumberType == NumberType.Natural))
            {
                if (query.WordCount > 0)
                {
                    if (!Numbers.Compare(sentence.WordCount, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(sentence.WordCount, query.WordCountNumberType))
                {
                    return false;
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Natural))
            {
                if (query.LetterCount > 0)
                {
                    if (!Numbers.Compare(sentence.LetterCount, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(sentence.LetterCount, query.LetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Natural))
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(sentence.UniqueLetterCount, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(sentence.UniqueLetterCount, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Natural))
            {
                if (query.Value > 0)
                {
                    if (value == 0L) { value = CalculateValue(sentence); }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(sentence); }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            // value digit sums
            if (query.ValueDigitSum > 0)
            {
                if (value == 0L) { value = CalculateValue(sentence); }
                if (Numbers.DigitSum(value) != query.ValueDigitSum)
                {
                    return false;
                }
            }

            // value digital roots
            if (query.ValueDigitalRoot > 0)
            {
                if (value == 0L) { value = CalculateValue(sentence); }
                if (Numbers.DigitalRoot(value) != query.ValueDigitalRoot)
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(Verse verse, NumberQuery query)
    {
        if (verse != null)
        {
            int number = 0;
            switch (query.NumberScope)
            {
                case NumberScope.Number:
                    number = verse.Number;
                    break;
                case NumberScope.NumberInChapter:
                    number = verse.NumberInChapter;
                    break;
                default:
                    number = verse.NumberInChapter;
                    break;
            }

            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Natural))
            {
                if (query.Number < 0)
                {
                    switch (query.NumberScope)
                    {
                        case NumberScope.Number:
                            query.Number = verse.Book.Verses.Count + query.Number + 1;
                            break;
                        case NumberScope.NumberInChapter:
                            query.Number = verse.Chapter.Verses.Count + query.Number + 1;
                            break;
                        default:
                            query.Number = verse.Chapter.Verses.Count + query.Number + 1;
                            break;
                    }
                }
                else if (query.Number > 0)
                {
                    if (!Numbers.Compare(number, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(number, query.NumberNumberType))
                {
                    return false;
                }
            }

            if (query.WordCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(verse.Words.Count, number, query.WordCountComparisonOperator, query.WordCountRemainder))
                {
                    return false;
                }
            }
            else if (query.WordCountNumberType == NumberType.None)
            {
                if (query.WordCount > 0)
                {
                    if (query.WordCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int word_sum; int letter_sum;
                        CalculateSums(verse, out word_sum, out letter_sum);
                        if (!Numbers.Compare(word_sum, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!Numbers.Compare(verse.Words.Count, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.WordCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int word_sum; int letter_sum;
                    CalculateSums(verse, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(word_sum, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Numbers.IsNumberType(verse.Words.Count, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if (query.LetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(verse.LetterCount, number, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                {
                    return false;
                }
            }
            else if (query.LetterCountNumberType == NumberType.None)
            {
                if (query.LetterCount > 0)
                {
                    if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int word_sum; int letter_sum;
                        CalculateSums(verse, out word_sum, out letter_sum);
                        if (!Numbers.Compare(letter_sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!Numbers.Compare(verse.LetterCount, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int word_sum; int letter_sum;
                    CalculateSums(verse, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(letter_sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Numbers.IsNumberType(verse.LetterCount, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if (query.UniqueLetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(verse.UniqueLetters.Count, number, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                {
                    return false;
                }
            }
            else if (query.UniqueLetterCountNumberType == NumberType.None)
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(verse.UniqueLetters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(verse.UniqueLetters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            long value = CalculateValue(verse);
            if (query.ValueNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value, number, query.ValueComparisonOperator, query.ValueRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueNumberType == NumberType.None)
            {
                if (query.Value > 0)
                {
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(verse); }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            // value digit sums
            if (query.ValueDigitSum > 0)
            {
                if (value == 0L) { value = CalculateValue(verse); }
                if (Numbers.DigitSum(value) != query.ValueDigitSum)
                {
                    return false;
                }
            }

            // value digital roots
            if (query.ValueDigitalRoot > 0)
            {
                if (value == 0L) { value = CalculateValue(verse); }
                if (Numbers.DigitalRoot(value) != query.ValueDigitalRoot)
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(List<Verse> verses, NumberQuery query)
    {
        if (verses != null)
        {
            long value = 0L;

            int sum = 0;
            switch (query.NumberScope)
            {
                case NumberScope.Number:
                    foreach (Verse verse in verses)
                    {
                        sum += verse.Number;
                    }
                    break;
                case NumberScope.NumberInChapter:
                    foreach (Verse verse in verses)
                    {
                        sum += verse.NumberInChapter;
                    }
                    break;
                default:
                    foreach (Verse verse in verses)
                    {
                        sum += verse.NumberInChapter;
                    }
                    break;
            }
            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Natural))
            {
                if (query.Number > 0)
                {
                    if (!Numbers.Compare(sum, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(sum, query.NumberNumberType))
                {
                    return false;
                }
            }

            if ((query.WordCountNumberType == NumberType.None) || (query.WordCountNumberType == NumberType.Natural))
            {
                if (query.WordCount > 0)
                {
                    if (query.WordCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                        CalculateSums(verses, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                        if (!Numbers.Compare(word_sum, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        sum = 0;
                        foreach (Verse verse in verses)
                        {
                            sum += verse.Words.Count;
                        }
                        if (!Numbers.Compare(sum, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.WordCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                    CalculateSums(verses, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(word_sum, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    sum = 0;
                    foreach (Verse verse in verses)
                    {
                        sum += verse.Words.Count;
                    }
                    if (!Numbers.IsNumberType(sum, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Natural))
            {
                if (query.LetterCount > 0)
                {
                    if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                        CalculateSums(verses, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                        if (!Numbers.Compare(letter_sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        sum = 0;
                        foreach (Verse verse in verses)
                        {
                            sum += verse.LetterCount;
                        }
                        if (!Numbers.Compare(sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                    CalculateSums(verses, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(letter_sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    sum = 0;
                    foreach (Verse verse in verses)
                    {
                        sum += verse.LetterCount;
                    }
                    if (!Numbers.IsNumberType(sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Natural))
            {
                if (query.UniqueLetterCount > 0)
                {
                    List<char> unique_letters = new List<char>();
                    foreach (Verse verse in verses)
                    {
                        foreach (char character in verse.UniqueLetters)
                        {
                            if (!unique_letters.Contains(character))
                            {
                                unique_letters.Add(character);
                            }
                        }
                    }
                    if (!Numbers.Compare(unique_letters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                List<char> unique_letters = new List<char>();
                foreach (Verse verse in verses)
                {
                    foreach (char character in verse.UniqueLetters)
                    {
                        if (!unique_letters.Contains(character))
                        {
                            unique_letters.Add(character);
                        }
                    }
                }
                if (!Numbers.IsNumberType(unique_letters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Natural))
            {
                if (query.Value > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Verse verse in verses)
                        {
                            value += CalculateValue(verse);
                        }
                    }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Verse verse in verses)
                    {
                        value += CalculateValue(verse);
                    }
                }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }


            // value digit sums
            if (query.ValueDigitSum > 0)
            {
                if (value == 0L)
                {
                    foreach (Verse verse in verses)
                    {
                        value += CalculateValue(verse);
                    }
                }
                if (Numbers.DigitSum(value) != query.ValueDigitSum)
                {
                    return false;
                }
            }

            // value digit root
            if (query.ValueDigitalRoot > 0)
            {
                if (value == 0L)
                {
                    foreach (Verse verse in verses)
                    {
                        value += CalculateValue(verse);
                    }
                }
                if (Numbers.DigitalRoot(value) != query.ValueDigitalRoot)
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(Chapter chapter, NumberQuery query)
    {
        if (chapter != null)
        {
            int number = chapter.SortedNumber;
            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Natural))
            {
                if (query.Number < 0)
                {
                    query.Number = number + query.Number + 1;
                }
                if (query.Number > 0)
                {
                    if (!Numbers.Compare(number, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(number, query.NumberNumberType))
                {
                    return false;
                }
            }

            if (query.VerseCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(chapter.Verses.Count, number, query.VerseCountComparisonOperator, query.VerseCountRemainder))
                {
                    return false;
                }
            }
            else if (query.VerseCountNumberType == NumberType.None)
            {
                if (query.VerseCount > 0)
                {
                    if (query.VerseCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int verse_sum; int word_sum; int letter_sum;
                        CalculateSums(chapter, out verse_sum, out word_sum, out letter_sum);
                        if (!Numbers.Compare(verse_sum, query.VerseCount, query.VerseCountComparisonOperator, query.VerseCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!Numbers.Compare(chapter.Verses.Count, query.VerseCount, query.VerseCountComparisonOperator, query.VerseCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.VerseCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int verse_sum; int word_sum; int letter_sum;
                    CalculateSums(chapter, out verse_sum, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(verse_sum, query.VerseCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Numbers.IsNumberType(chapter.Verses.Count, query.VerseCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if (query.WordCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(chapter.WordCount, number, query.WordCountComparisonOperator, query.WordCountRemainder))
                {
                    return false;
                }
            }
            else if (query.WordCountNumberType == NumberType.None)
            {
                if (query.WordCount > 0)
                {
                    if (query.WordCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int verse_sum; int word_sum; int letter_sum;
                        CalculateSums(chapter, out verse_sum, out word_sum, out letter_sum);
                        if (!Numbers.Compare(word_sum, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!Numbers.Compare(chapter.WordCount, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.WordCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int verse_sum; int word_sum; int letter_sum;
                    CalculateSums(chapter, out verse_sum, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(word_sum, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Numbers.IsNumberType(chapter.WordCount, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if (query.LetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(chapter.LetterCount, number, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                {
                    return false;
                }
            }
            else if (query.LetterCountNumberType == NumberType.None)
            {
                if (query.LetterCount > 0)
                {
                    if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int verse_sum; int word_sum; int letter_sum;
                        CalculateSums(chapter, out verse_sum, out word_sum, out letter_sum);
                        if (!Numbers.Compare(letter_sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!Numbers.Compare(chapter.LetterCount, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int verse_sum; int word_sum; int letter_sum;
                    CalculateSums(chapter, out verse_sum, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(letter_sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Numbers.IsNumberType(chapter.LetterCount, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if (query.UniqueLetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(chapter.UniqueLetters.Count, number, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                {
                    return false;
                }
            }
            else if (query.UniqueLetterCountNumberType == NumberType.None)
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(chapter.UniqueLetters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(chapter.UniqueLetters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }


            long value = CalculateValue(chapter);
            if (query.ValueNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value, number, query.ValueComparisonOperator, query.ValueRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueNumberType == NumberType.None)
            {
                if (query.Value > 0)
                {
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            // value digit sums
            if (query.ValueDigitSum > 0)
            {
                if (value == 0L) { value = CalculateValue(chapter); }
                if (Numbers.DigitSum(value) != query.ValueDigitSum)
                {
                    return false;
                }
            }

            // value digital roots
            if (query.ValueDigitalRoot > 0)
            {
                if (value == 0L) { value = CalculateValue(chapter); }
                if (Numbers.DigitalRoot(value) != query.ValueDigitalRoot)
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    private static bool Compare(List<Chapter> chapters, NumberQuery query)
    {
        if (chapters != null)
        {
            long value = 0L;
            int sum = 0;
            foreach (Chapter chapter in chapters)
            {
                sum += chapter.SortedNumber;
            }
            if ((query.NumberNumberType == NumberType.None) || (query.NumberNumberType == NumberType.Natural))
            {
                if (query.Number > 0)
                {
                    if (!Numbers.Compare(sum, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(sum, query.NumberNumberType))
                {
                    return false;
                }
            }

            if ((query.VerseCountNumberType == NumberType.None) || (query.VerseCountNumberType == NumberType.Natural))
            {
                if (query.VerseCount > 0)
                {
                    if (query.VerseCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                        CalculateSums(chapters, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                        if (!Numbers.Compare(verse_sum, query.VerseCount, query.VerseCountComparisonOperator, query.VerseCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        sum = 0;
                        foreach (Chapter chapter in chapters)
                        {
                            sum += chapter.Verses.Count;
                        }
                        if (!Numbers.Compare(sum, query.VerseCount, query.VerseCountComparisonOperator, query.VerseCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.VerseCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                    CalculateSums(chapters, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(verse_sum, query.VerseCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    sum = 0;
                    foreach (Chapter chapter in chapters)
                    {
                        sum += chapter.Verses.Count;
                    }
                    if (!Numbers.IsNumberType(sum, query.VerseCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if ((query.WordCountNumberType == NumberType.None) || (query.WordCountNumberType == NumberType.Natural))
            {
                if (query.WordCount > 0)
                {
                    if (query.WordCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                        CalculateSums(chapters, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                        if (!Numbers.Compare(word_sum, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        sum = 0;
                        foreach (Chapter chapter in chapters)
                        {
                            sum += chapter.WordCount;
                        }
                        if (!Numbers.Compare(sum, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.WordCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                    CalculateSums(chapters, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(word_sum, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    sum = 0;
                    foreach (Chapter chapter in chapters)
                    {
                        sum += chapter.WordCount;
                    }
                    if (!Numbers.IsNumberType(sum, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if ((query.LetterCountNumberType == NumberType.None) || (query.LetterCountNumberType == NumberType.Natural))
            {
                if (query.LetterCount > 0)
                {
                    if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                    {
                        int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                        CalculateSums(chapters, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                        if (!Numbers.Compare(letter_sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        sum = 0;
                        foreach (Chapter chapter in chapters)
                        {
                            sum += chapter.LetterCount;
                        }
                        if (!Numbers.Compare(sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (query.LetterCountComparisonOperator == ComparisonOperator.EqualSum)
                {
                    int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                    CalculateSums(chapters, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                    if (!Numbers.IsNumberType(letter_sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    sum = 0;
                    foreach (Chapter chapter in chapters)
                    {
                        sum += chapter.LetterCount;
                    }
                    if (!Numbers.IsNumberType(sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if ((query.UniqueLetterCountNumberType == NumberType.None) || (query.UniqueLetterCountNumberType == NumberType.Natural))
            {
                if (query.UniqueLetterCount > 0)
                {
                    List<char> unique_letters = new List<char>();
                    foreach (Chapter chapter in chapters)
                    {
                        foreach (char character in chapter.UniqueLetters)
                        {
                            if (!unique_letters.Contains(character))
                            {
                                unique_letters.Add(character);
                            }
                        }
                    }
                    if (!Numbers.Compare(unique_letters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                List<char> unique_letters = new List<char>();
                foreach (Chapter chapter in chapters)
                {
                    foreach (char character in chapter.UniqueLetters)
                    {
                        if (!unique_letters.Contains(character))
                        {
                            unique_letters.Add(character);
                        }
                    }
                }
                if (!Numbers.IsNumberType(unique_letters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if ((query.ValueNumberType == NumberType.None) || (query.ValueNumberType == NumberType.Natural))
            {
                if (query.Value > 0)
                {
                    if (value == 0L)
                    {
                        foreach (Chapter chapter in chapters)
                        {
                            value += CalculateValue(chapter);
                        }
                    }
                    if (!Numbers.Compare(value, query.Value, query.ValueComparisonOperator, query.ValueRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L)
                {
                    foreach (Chapter chapter in chapters)
                    {
                        value += CalculateValue(chapter);
                    }
                }
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            // value digit sums
            if (query.ValueDigitSum > 0)
            {
                if (value == 0L)
                {
                    foreach (Chapter chapter in chapters)
                    {
                        value += CalculateValue(chapter);
                    }
                }
                if (Numbers.DigitSum(value) != query.ValueDigitSum)
                {
                    return false;
                }
            }

            // value digit root
            if (query.ValueDigitalRoot > 0)
            {
                if (value == 0L)
                {
                    foreach (Chapter chapter in chapters)
                    {
                        value += CalculateValue(chapter);
                    }
                }
                if (Numbers.DigitalRoot(value) != query.ValueDigitalRoot)
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
    }
    // find by numbers - Words
    public static List<Word> FindWords(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindWords(search_scope, current_selection, previous_verses, query);
    }
    private static List<Word> DoFindWords(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindWords(source, query);
    }
    private static List<Word> DoFindWords(List<Verse> source, NumberQuery query)
    {
        List<Word> result = new List<Word>();
        if (source != null)
        {
            if (query.WordCount <= 1) // ensure no range search
            {
                foreach (Verse verse in source)
                {
                    foreach (Word word in verse.Words)
                    {
                        if (Compare(word, query))
                        {
                            result.Add(word);
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - WordRanges
    public static List<List<Word>> FindWordRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindWordRanges(search_scope, current_selection, previous_verses, query);
    }
    private static List<List<Word>> DoFindWordRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        if (query.WithinVerses)
        {
            return DoFindWordRangesWithinVerses(source, query);
        }
        else
        {
            return DoFindWordRanges(source, query);
        }
    }
    private static List<List<Word>> DoFindWordRanges(List<Verse> source, NumberQuery query)
    {
        List<List<Word>> result = new List<List<Word>>();
        if (source != null)
        {
            int range_length = query.WordCount;
            if (range_length == 1)
            {
                result.Add(DoFindWords(source, query));
                return result;
            }

            List<Word> words = new List<Word>();
            foreach (Verse verse in source)
            {
                words.AddRange(verse.Words);
            }

            if (range_length == 0) // non-specified range length
            {
                // limit range length to minimum
                int word_count = 0;
                foreach (Verse verse in source)
                {
                    word_count += verse.Words.Count;
                }

                int limit = word_count - 1;
                if (query.LetterCount > 0)
                {
                    limit = query.LetterCount / 3;
                }
                if (query.Value > 0L)
                {
                    limit = (int)(query.Value / 114L);
                }

                for (int r = 1; r <= limit; r++) // try all possible range lengths
                {
                    for (int i = 0; i < words.Count - r + 1; i++)
                    {
                        // build required range
                        List<Word> range = new List<Word>();
                        for (int j = i; j < i + r; j++)
                        {
                            range.Add(words[j]);
                        }

                        // check range
                        if (Compare(range, query))
                        {
                            result.Add(range);
                        }
                    }
                }
            }
            else // specified range length
            {
                int r = range_length;
                for (int i = 0; i < words.Count - r + 1; i++)
                {
                    // build required range
                    List<Word> range = new List<Word>();
                    for (int j = i; j < i + r; j++)
                    {
                        range.Add(words[j]);
                    }

                    // check range
                    if (Compare(range, query))
                    {
                        result.Add(range);
                    }
                }
            }
        }
        return result;
    }
    private static List<List<Word>> DoFindWordRangesWithinVerses(List<Verse> source, NumberQuery query)
    {
        List<List<Word>> result = new List<List<Word>>();
        if (source != null)
        {
            int range_length = query.WordCount;
            if (range_length == 1)
            {
                result.Add(DoFindWords(source, query));
                return result;
            }

            if (range_length == 0) // non-specified range length
            {
                // limit range length to minimum
                int word_count = 0;
                foreach (Verse verse in source)
                {
                    word_count += verse.Words.Count;
                }

                int limit = word_count - 1;
                if (query.LetterCount > 0)
                {
                    limit = query.LetterCount / 3;
                }
                if (query.Value > 0L)
                {
                    limit = (int)(query.Value / 114L);
                }

                for (int r = 1; r <= limit; r++) // try all possible range lengths
                {
                    foreach (Verse verse in source)
                    {
                        for (int i = 0; i < verse.Words.Count - r + 1; i++)
                        {
                            // build required range
                            List<Word> range = new List<Word>();
                            for (int j = i; j < i + r; j++)
                            {
                                range.Add(verse.Words[j]);
                            }

                            // check range
                            if (Compare(range, query))
                            {
                                result.Add(range);
                            }
                        }
                    }
                }
            }
            else // specified range length
            {
                int r = range_length;
                foreach (Verse verse in source)
                {
                    for (int i = 0; i < verse.Words.Count - r + 1; i++)
                    {
                        // build required range
                        List<Word> range = new List<Word>();
                        for (int j = i; j < i + r; j++)
                        {
                            range.Add(verse.Words[j]);
                        }

                        // check range
                        if (Compare(range, query))
                        {
                            result.Add(range);
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - Sentences
    public static List<Sentence> FindSentences(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindSentences(search_scope, current_selection, previous_verses, query);
    }
    private static List<Sentence> DoFindSentences(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindSentences(source, query);
    }
    private static List<Sentence> DoFindSentences(List<Verse> source, NumberQuery query)
    {
        List<Sentence> result = new List<Sentence>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                List<Word> words = new List<Word>();
                foreach (Verse verse in source)
                {
                    words.AddRange(verse.Words);
                }

                // scan linearly for sequence of words with total Text matching query
                bool done_MustContinue = false;
                for (int i = 0; i < words.Count - 1; i++)
                {
                    StringBuilder str = new StringBuilder();

                    // start building word sequence
                    str.Append(words[i].Text);

                    string stopmark_text = StopmarkHelper.GetStopmarkText(words[i].Stopmark);

                    // 1-word sentence
                    if (
                         (words[i].Stopmark != Stopmark.None) &&
                         (words[i].Stopmark != Stopmark.CanStopAtEither) &&
                         (words[i].Stopmark != Stopmark.MustPause) //&&
                        //(words[i].Stopmark != Stopmark.MustContinue)
                       )
                    {
                        Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[i].Verse, words[i].Position + words[i].Text.Length, str.ToString());
                        if (sentence != null)
                        {
                            if (Compare(sentence, query))
                            {
                                result.Add(sentence);
                            }
                        }
                    }
                    else // multi-word sentence
                    {
                        // mark the start of 1-to-m MustContinue stopmarks
                        int backup_i = i;

                        // continue building with next words until a stopmark
                        bool done_CanStopAtEither = false;
                        for (int j = i + 1; j < words.Count; j++)
                        {
                            str.Append(" " + words[j].Text);

                            if (words[j].Stopmark == Stopmark.None)
                            {
                                continue; // continue building longer senetence
                            }
                            else // there is a real stopmark
                            {
                                if (s_numerology_system != null)
                                {
                                    if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                    {
                                        if (s_numerology_system.TextMode == "Original")
                                        {
                                            str.Append(" " + stopmark_text);
                                        }
                                    }
                                }

                                if (words[j].Stopmark == Stopmark.MustContinue)
                                {
                                    // TEST Stopmark.MustContinue
                                    //----1 2 3 4 sentences
                                    //1268
                                    //4153
                                    //1799
                                    //2973
                                    //----1 12 123 1234 sentences
                                    //1268
                                    //5421
                                    //7220
                                    //10193
                                    //-------------
                                    //ERRORS
                                    //# duplicate 1
                                    //# short str
                                    //  in 123 1234
                                    //-------------
                                    //// not needed yet
                                    //// multi-mid sentences
                                    //5952
                                    //4772

                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        if (Compare(sentence, query))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    if (done_MustContinue)
                                    {
                                        done_MustContinue = false;
                                        continue; // get all overlapping long sentence
                                    }

                                    StringBuilder k_str = new StringBuilder();
                                    for (int k = j + 1; k < words.Count; k++)
                                    {
                                        k_str.Append(words[k].Text + " ");

                                        if (words[k].Stopmark == Stopmark.None)
                                        {
                                            continue; // next k
                                        }
                                        else // there is a stopmark
                                        {
                                            if (s_numerology_system != null)
                                            {
                                                if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                                {
                                                    if (s_numerology_system.TextMode == "Original")
                                                    {
                                                        stopmark_text = StopmarkHelper.GetStopmarkText(words[k].Stopmark);
                                                        k_str.Append(stopmark_text + " ");
                                                    }
                                                }
                                            }
                                            if (k_str.Length > 0)
                                            {
                                                k_str.Remove(k_str.Length - 1, 1);
                                            }

                                            sentence = new Sentence(words[j + 1].Verse, words[j + 1].Position, words[k].Verse, words[k].Position + words[k].Text.Length, k_str.ToString());
                                            if (sentence != null)
                                            {
                                                if (Compare(sentence, query))
                                                {
                                                    result.Add(sentence);
                                                }
                                            }

                                            if (
                                                 (words[k].Stopmark == Stopmark.ShouldContinue) ||
                                                 (words[k].Stopmark == Stopmark.CanStop) ||
                                                 (words[k].Stopmark == Stopmark.ShouldStop)
                                               )
                                            {
                                                done_MustContinue = true;   // restart from beginning skipping any MustContinue
                                            }
                                            else
                                            {
                                                done_MustContinue = false;   // keep building ever-longer multi-MustContinue sentence
                                            }

                                            j = k;
                                            break; // next j
                                        }
                                    }

                                    if (done_MustContinue)
                                    {
                                        i = backup_i - 1;  // start new sentence from beginning
                                        break; // next i
                                    }
                                    else
                                    {
                                        continue; // next j
                                    }
                                }
                                else if (
                                     (words[j].Stopmark == Stopmark.ShouldContinue) ||
                                     (words[j].Stopmark == Stopmark.CanStop) ||
                                     (words[j].Stopmark == Stopmark.ShouldStop)
                                   )
                                {
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        if (Compare(sentence, query))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    i = j; // start new sentence after j
                                    break; // next i
                                }
                                else if (words[j].Stopmark == Stopmark.MustPause)
                                {
                                    if (
                                         (words[j].Text.Simplify29() == "مَنْ".Simplify29()) ||
                                         (words[j].Text.Simplify29() == "بَلْ".Simplify29())
                                       )
                                    {
                                        continue; // continue building longer senetence
                                    }
                                    else if (
                                              (words[j].Text.Simplify29() == "عِوَجَا".Simplify29()) ||
                                              (words[j].Text.Simplify29() == "مَّرْقَدِنَا".Simplify29()) ||
                                              (words[j].Text.Simplify29() == "مَالِيَهْ".Simplify29())
                                            )
                                    {
                                        Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                        if (sentence != null)
                                        {
                                            if (Compare(sentence, query))
                                            {
                                                result.Add(sentence);
                                            }
                                        }

                                        i = j; // start new sentence after j
                                        break; // next i
                                    }
                                    else // unknown case
                                    {
                                        throw new Exception("Unknown paused Quran word.");
                                    }
                                }
                                // first CanStopAtEither found at j
                                else if ((!done_CanStopAtEither) && (words[j].Stopmark == Stopmark.CanStopAtEither))
                                {
                                    // ^ ذَٰلِكَ ٱلْكِتَٰبُ لَا رَيْبَ
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        if (Compare(sentence, query))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    int kk = -1; // start after ^ (e.g. هُدًۭى)
                                    StringBuilder kk_str = new StringBuilder();
                                    StringBuilder kkk_str = new StringBuilder();
                                    for (int k = j + 1; k < words.Count; k++)
                                    {
                                        str.Append(" " + words[k].Text);
                                        if (kkk_str.Length > 0) // skip first k loop
                                        {
                                            kk_str.Append(" " + words[k].Text);
                                        }
                                        kkk_str.Append(" " + words[k].Text);

                                        if (words[k].Stopmark == Stopmark.None)
                                        {
                                            continue; // next k
                                        }
                                        else // there is a stopmark
                                        {
                                            if (s_numerology_system != null)
                                            {
                                                if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                                {
                                                    if (s_numerology_system.TextMode == "Original")
                                                    {
                                                        str.Append(" " + stopmark_text);
                                                        if (kk_str.Length > 0)
                                                        {
                                                            kk_str.Append(" " + stopmark_text);
                                                        }
                                                        kkk_str.Append(" " + stopmark_text);
                                                    }
                                                }
                                            }

                                            // second CanStopAtEither found at k
                                            if (words[k].Stopmark == Stopmark.CanStopAtEither)
                                            {
                                                // ^ ذَٰلِكَ ٱلْكِتَٰبُ لَا رَيْبَ ۛ^ فِيهِ
                                                sentence = new Sentence(words[i].Verse, words[i].Position, words[k].Verse, words[k].Position + words[k].Text.Length, str.ToString());
                                                if (sentence != null)
                                                {
                                                    if (Compare(sentence, query))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                kk = k + 1; // backup k after second ^
                                                continue; // next k
                                            }
                                            else // non-CanStopAtEither stopmark
                                            {
                                                // kkk_str   فِيهِ ۛ^ هُدًۭى لِّلْمُتَّقِينَ
                                                sentence = new Sentence(words[j + 1].Verse, words[j + 1].Position, words[k].Verse, words[k].Position + words[k].Text.Length, kkk_str.ToString());
                                                if (sentence != null)
                                                {
                                                    if (Compare(sentence, query))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                // kk_str   هُدًۭى لِّلْمُتَّقِينَ
                                                sentence = new Sentence(words[kk].Verse, words[kk].Position, words[k].Verse, words[k].Position + words[k].Text.Length, kk_str.ToString());
                                                if (sentence != null)
                                                {
                                                    if (Compare(sentence, query))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                // skip the whole surrounding non-CanStopAtEither sentence
                                                j = k;
                                                break; // next j
                                            }
                                        }
                                    }

                                    // restart from last
                                    str.Length = 0;
                                    j = i - 1; // will be j++ by reloop
                                    done_CanStopAtEither = true;
                                }
                                else if (words[j].Stopmark == Stopmark.MustStop)
                                {
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        if (Compare(sentence, query))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    i = j; // start new sentence after j
                                    break; // next i
                                }
                                else // unknown case
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - Verses
    public static List<Verse> FindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindVerses(search_scope, current_selection, previous_verses, query);
    }
    private static List<Verse> DoFindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindVerses(source, query);
    }
    private static List<Verse> DoFindVerses(List<Verse> source, NumberQuery query)
    {
        List<Verse> result = new List<Verse>();
        if (source != null)
        {
            if (query.VerseCount <= 1) // ensure no range search
            {
                foreach (Verse verse in source)
                {
                    if (Compare(verse, query))
                    {
                        result.Add(verse);
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - VerseRanges
    public static List<List<Verse>> FindVerseRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindVerseRanges(search_scope, current_selection, previous_verses, query);
    }
    private static List<List<Verse>> DoFindVerseRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindVerseRanges(source, query);
    }
    private static List<List<Verse>> DoFindVerseRanges(List<Verse> source, NumberQuery query)
    {
        List<List<Verse>> result = new List<List<Verse>>();
        if (source != null)
        {
            int range_length = query.VerseCount;
            if (range_length == 1)
            {
                result.Add(DoFindVerses(source, query));
                return result;
            }

            if (range_length == 0) // non-specified range length
            {
                int limit = source.Count;
                if (query.VerseCount > 0)
                {
                    limit = query.VerseCount;
                }
                if (query.WordCount > 0)
                {
                    limit = query.WordCount / 7;
                }
                if (query.LetterCount > 0)
                {
                    limit = query.LetterCount / 29;
                }
                if (query.Value > 0L)
                {
                    limit = (int)(query.Value / 449L);
                }

                for (int r = 1; r <= limit; r++) // try all possible range lengths
                {
                    for (int i = 0; i < source.Count - r + 1; i++)
                    {
                        // build required range
                        List<Verse> range = new List<Verse>();
                        for (int j = i; j < i + r; j++)
                        {
                            range.Add(source[j]);
                        }

                        // check range
                        if (Compare(range, query))
                        {
                            result.Add(range);
                        }
                    }
                }
            }
            else // specified range length
            {
                int r = range_length;
                for (int i = 0; i < source.Count - r + 1; i++)
                {
                    // build required range
                    List<Verse> range = new List<Verse>();
                    for (int j = i; j < i + r; j++)
                    {
                        range.Add(source[j]);
                    }

                    // check range
                    if (Compare(range, query))
                    {
                        result.Add(range);
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - Chapters
    public static List<Chapter> FindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindChapters(search_scope, current_selection, previous_verses, query);
    }
    private static List<Chapter> DoFindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindChapters(source, query);
    }
    private static List<Chapter> DoFindChapters(List<Verse> source, NumberQuery query)
    {
        List<Chapter> result = new List<Chapter>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    List<Chapter> chapters = s_book.GetChapters(source);
                    if (chapters != null)
                    {
                        if (query.ChapterCount <= 1) // ensure no range search
                        {
                            foreach (Chapter chapter in chapters)
                            {
                                if (Compare(chapter, query))
                                {
                                    result.Add(chapter);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by numbers - ChapterRanges
    public static List<List<Chapter>> FindChapterRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindChapterRanges(search_scope, current_selection, previous_verses, query);
    }
    private static List<List<Chapter>> DoFindChapterRanges(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindChapterRanges(source, query);
    }
    private static List<List<Chapter>> DoFindChapterRanges(List<Verse> source, NumberQuery query)
    {
        List<List<Chapter>> result = new List<List<Chapter>>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    List<Chapter> chapters = s_book.GetChapters(source);
                    if (chapters != null)
                    {
                        int range_length = query.ChapterCount;
                        if (range_length == 1)
                        {
                            result.Add(DoFindChapters(source, query));
                            return result;
                        }

                        if (range_length == 0) // non-specified range length
                        {
                            // limit range length to minimum
                            int limit = chapters.Count;
                            //if (query.VerseCount > 0)
                            //{
                            //    limit = query.VerseCount;
                            //}
                            //if (query.WordCount > 0)
                            //{
                            //    limit = query.WordCount;
                            //}
                            //if (query.LetterCount > 0)
                            //{
                            //    limit = query.LetterCount;
                            //}
                            //if (query.Value > 0L)
                            //{
                            //    limit = (int)(query.Value / 10L);
                            //}

                            for (int r = 1; r <= limit; r++) // try all possible range lengths
                            {
                                for (int i = 0; i < chapters.Count - r + 1; i++)
                                {
                                    // build required range
                                    List<Chapter> range = new List<Chapter>();
                                    for (int j = i; j < i + r; j++)
                                    {
                                        range.Add(chapters[j]);
                                    }

                                    // check range
                                    if (Compare(range, query))
                                    {
                                        result.Add(range);
                                    }
                                }
                            }
                        }
                        else // specified range length
                        {
                            int r = range_length;
                            for (int i = 0; i < chapters.Count - r + 1; i++)
                            {
                                // build required range
                                List<Chapter> range = new List<Chapter>();
                                for (int j = i; j < i + r; j++)
                                {
                                    range.Add(chapters[j]);
                                }

                                // check range
                                if (Compare(range, query))
                                {
                                    result.Add(range);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }

    // find by prostration type
    public static List<Verse> FindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, ProstrationType prostration_type)
    {
        return DoFindVerses(search_scope, current_selection, previous_verses, prostration_type);
    }
    private static List<Verse> DoFindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, ProstrationType prostration_type)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindVerses(source, prostration_type);
    }
    private static List<Verse> DoFindVerses(List<Verse> source, ProstrationType prostration_type)
    {
        List<Verse> result = new List<Verse>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                foreach (Verse verse in source)
                {
                    if ((verse.ProstrationType & prostration_type) > 0)
                    {
                        result.Add(verse);
                    }
                }
            }
        }
        return result;
    }

    // find by revelation place
    public static List<Chapter> FindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, RevelationPlace revelation_place)
    {
        return DoFindChapters(search_scope, current_selection, previous_verses, revelation_place);
    }
    private static List<Chapter> DoFindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, RevelationPlace revelation_place)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindChapters(source, revelation_place);
    }
    private static List<Chapter> DoFindChapters(List<Verse> source, RevelationPlace revelation_place)
    {
        List<Chapter> result = new List<Chapter>();
        List<Verse> result_verses = new List<Verse>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                foreach (Verse verse in source)
                {
                    if (verse.Chapter != null)
                    {
                        if (verse.Chapter.RevelationPlace == revelation_place)
                        {
                            result_verses.Add(verse);
                        }
                    }
                }
            }
        }

        int current_chapter_number = -1;
        foreach (Verse verse in result_verses)
        {
            if (verse.Chapter != null)
            {
                if (current_chapter_number != verse.Chapter.SortedNumber)
                {
                    current_chapter_number = verse.Chapter.SortedNumber;
                    result.Add(verse.Chapter);
                }
            }
        }
        return result;
    }

    // find by initialization type
    public static List<Verse> FindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, InitializationType initialization_type)
    {
        return DoFindVerses(search_scope, current_selection, previous_verses, initialization_type);
    }
    private static List<Verse> DoFindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, InitializationType initialization_type)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindVerses(source, initialization_type);
    }
    private static List<Verse> DoFindVerses(List<Verse> source, InitializationType initialization_type)
    {
        List<Verse> result = new List<Verse>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                foreach (Verse verse in source)
                {
                    if ((verse.InitializationType & initialization_type) > 0)
                    {
                        result.Add(verse);
                    }
                }
            }
        }
        return result;
    }

    // find by frequency - helper methods   
    private static bool Compare(int number1, int number2, NumberType number_type, ComparisonOperator comparison_operator, int remainder)
    {
        if ((number_type == NumberType.None) || (number_type == NumberType.Natural))
        {
            if (Numbers.Compare(number1, number2, comparison_operator, remainder))
            {
                return true;
            }
        }
        else
        {
            if (Numbers.IsNumberType(number1, number_type))
            {
                return true;
            }
        }

        return false;
    }
    public static int CalculateLetterFrequencySum(string text, string phrase, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        if (String.IsNullOrEmpty(phrase)) return 0;

        int result = 0;
        if (s_numerology_system != null)
        {
            if (!with_diacritics) text = text.SimplifyTo(s_numerology_system.TextMode);
            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            text = text.Replace("\t", "");
            text = text.Replace("_", "");
            text = text.Replace(" ", "");
            text = text.Replace(Constants.OPEN_BRACKET, "");
            text = text.Replace(Constants.CLOSE_BRACKET, "");
            foreach (char character in Constants.INDIAN_DIGITS)
            {
                text = text.Replace(character.ToString(), "");
            }

            if (!String.IsNullOrEmpty(text))
            {
                if (!with_diacritics) phrase = phrase.SimplifyTo(s_numerology_system.TextMode);
                phrase = phrase.Replace("\r", "");
                phrase = phrase.Replace("\n", "");
                phrase = phrase.Replace("\t", "");
                phrase = phrase.Replace("_", "");
                phrase = phrase.Replace(" ", "");
                phrase = phrase.Replace(Constants.OPEN_BRACKET, "");
                phrase = phrase.Replace(Constants.CLOSE_BRACKET, "");
                foreach (char character in Constants.INDIAN_DIGITS)
                {
                    phrase = phrase.Replace(character.ToString(), "");
                }

                if (frequency_search_type == FrequencySearchType.UniqueLetters)
                {
                    phrase = phrase.RemoveDuplicates();
                }

                if (!String.IsNullOrEmpty(phrase))
                {
                    for (int i = 0; i < phrase.Length; i++)
                    {
                        int frequency = 0;
                        for (int j = 0; j < text.Length; j++)
                        {
                            if (phrase[i] == text[j])
                            {
                                frequency++;
                            }
                        }

                        if (frequency > 0)
                        {
                            result += frequency;
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by frequency - Words
    public static List<Word> FindWords(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        return DoFindWords(search_scope, current_selection, previous_verses, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
    }
    private static List<Word> DoFindWords(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindWords(source, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
    }
    private static List<Word> DoFindWords(List<Verse> source, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        List<Word> result = new List<Word>();
        if (!string.IsNullOrEmpty(phrase))
        {
            if (source != null)
            {
                if (source.Count > 0)
                {
                    if (!String.IsNullOrEmpty(phrase))
                    {
                        foreach (Verse verse in source)
                        {
                            if (verse != null)
                            {
                                foreach (Word word in verse.Words)
                                {
                                    string text = word.Text;
                                    if (!with_diacritics) text = text.SimplifyTo(s_numerology_system.TextMode);
                                    if (!with_diacritics) phrase = phrase.SimplifyTo(s_numerology_system.TextMode);

                                    int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                    if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                    {
                                        result.Add(word);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by frequency - Sentences
    public static List<Sentence> FindSentences(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        return DoFindSentences(search_scope, current_selection, previous_verses, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
    }
    private static List<Sentence> DoFindSentences(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindSentences(source, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
    }
    private static List<Sentence> DoFindSentences(List<Verse> source, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        List<Sentence> result = new List<Sentence>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                List<Word> words = new List<Word>();
                foreach (Verse verse in source)
                {
                    words.AddRange(verse.Words);
                }

                // scan linearly for sequence of words with total Text matching query
                bool done_MustContinue = false;
                for (int i = 0; i < words.Count - 1; i++)
                {
                    StringBuilder str = new StringBuilder();

                    // start building word sequence
                    str.Append(words[i].Text);

                    string stopmark_text = StopmarkHelper.GetStopmarkText(words[i].Stopmark);

                    // 1-word sentence
                    if (
                         (words[i].Stopmark != Stopmark.None) &&
                         (words[i].Stopmark != Stopmark.CanStopAtEither) &&
                         (words[i].Stopmark != Stopmark.MustPause) //&&
                        //(words[i].Stopmark != Stopmark.MustContinue)
                       )
                    {
                        Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[i].Verse, words[i].Position + words[i].Text.Length, str.ToString());
                        if (sentence != null)
                        {
                            string text = sentence.ToString();
                            int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                            if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                            {
                                result.Add(sentence);
                            }
                        }
                    }
                    else // multi-word sentence
                    {
                        // mark the start of 1-to-m MustContinue stopmarks
                        int backup_i = i;

                        // continue building with next words until a stopmark
                        bool done_CanStopAtEither = false;
                        for (int j = i + 1; j < words.Count; j++)
                        {
                            str.Append(" " + words[j].Text);

                            if (words[j].Stopmark == Stopmark.None)
                            {
                                continue; // continue building longer senetence
                            }
                            else // there is a real stopmark
                            {
                                if (s_numerology_system != null)
                                {
                                    if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                    {
                                        if (s_numerology_system.TextMode == "Original")
                                        {
                                            str.Append(" " + stopmark_text);
                                        }
                                    }
                                }

                                if (words[j].Stopmark == Stopmark.MustContinue)
                                {
                                    // TEST Stopmark.MustContinue
                                    //----1 2 3 4 sentences
                                    //1268
                                    //4153
                                    //1799
                                    //2973
                                    //----1 12 123 1234 sentences
                                    //1268
                                    //5421
                                    //7220
                                    //10193
                                    //-------------
                                    //ERRORS
                                    //# duplicate 1
                                    //# short str
                                    //  in 123 1234
                                    //-------------
                                    //// not needed yet
                                    //// multi-mid sentences
                                    //5952
                                    //4772

                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        string text = sentence.ToString();
                                        int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                        if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    if (done_MustContinue)
                                    {
                                        done_MustContinue = false;
                                        continue; // get all overlapping long sentence
                                    }

                                    StringBuilder k_str = new StringBuilder();
                                    for (int k = j + 1; k < words.Count; k++)
                                    {
                                        k_str.Append(words[k].Text + " ");

                                        if (words[k].Stopmark == Stopmark.None)
                                        {
                                            continue; // next k
                                        }
                                        else // there is a stopmark
                                        {
                                            if (s_numerology_system != null)
                                            {
                                                if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                                {
                                                    if (s_numerology_system.TextMode == "Original")
                                                    {
                                                        stopmark_text = StopmarkHelper.GetStopmarkText(words[k].Stopmark);
                                                        k_str.Append(stopmark_text + " ");
                                                    }
                                                }
                                            }
                                            if (k_str.Length > 0)
                                            {
                                                k_str.Remove(k_str.Length - 1, 1);
                                            }

                                            sentence = new Sentence(words[j + 1].Verse, words[j + 1].Position, words[k].Verse, words[k].Position + words[k].Text.Length, k_str.ToString());
                                            if (sentence != null)
                                            {
                                                string text = sentence.ToString();
                                                int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                                if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                                {
                                                    result.Add(sentence);
                                                }
                                            }

                                            if (
                                                 (words[k].Stopmark == Stopmark.ShouldContinue) ||
                                                 (words[k].Stopmark == Stopmark.CanStop) ||
                                                 (words[k].Stopmark == Stopmark.ShouldStop)
                                               )
                                            {
                                                done_MustContinue = true;   // restart from beginning skipping any MustContinue
                                            }
                                            else
                                            {
                                                done_MustContinue = false;   // keep building ever-longer multi-MustContinue sentence
                                            }

                                            j = k;
                                            break; // next j
                                        }
                                    }

                                    if (done_MustContinue)
                                    {
                                        i = backup_i - 1;  // start new sentence from beginning
                                        break; // next i
                                    }
                                    else
                                    {
                                        continue; // next j
                                    }
                                }
                                else if (
                                     (words[j].Stopmark == Stopmark.ShouldContinue) ||
                                     (words[j].Stopmark == Stopmark.CanStop) ||
                                     (words[j].Stopmark == Stopmark.ShouldStop)
                                   )
                                {
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        string text = sentence.ToString();
                                        int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                        if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    i = j; // start new sentence after j
                                    break; // next i
                                }
                                else if (words[j].Stopmark == Stopmark.MustPause)
                                {
                                    if (
                                         (words[j].Text.Simplify29() == "مَنْ".Simplify29()) ||
                                         (words[j].Text.Simplify29() == "بَلْ".Simplify29())
                                       )
                                    {
                                        continue; // continue building longer senetence
                                    }
                                    else if (
                                              (words[j].Text.Simplify29() == "عِوَجَا".Simplify29()) ||
                                              (words[j].Text.Simplify29() == "مَّرْقَدِنَا".Simplify29()) ||
                                              (words[j].Text.Simplify29() == "مَالِيَهْ".Simplify29())
                                            )
                                    {
                                        Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                        if (sentence != null)
                                        {
                                            string text = sentence.ToString();
                                            int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                            if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                            {
                                                result.Add(sentence);
                                            }
                                        }

                                        i = j; // start new sentence after j
                                        break; // next i
                                    }
                                    else // unknown case
                                    {
                                        throw new Exception("Unknown paused Quran word.");
                                    }
                                }
                                // first CanStopAtEither found at j
                                else if ((!done_CanStopAtEither) && (words[j].Stopmark == Stopmark.CanStopAtEither))
                                {
                                    // ^ ذَٰلِكَ ٱلْكِتَٰبُ لَا رَيْبَ
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        string text = sentence.ToString();
                                        int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                        if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    int kk = -1; // start after ^ (e.g. هُدًۭى)
                                    StringBuilder kk_str = new StringBuilder();
                                    StringBuilder kkk_str = new StringBuilder();
                                    for (int k = j + 1; k < words.Count; k++)
                                    {
                                        str.Append(" " + words[k].Text);
                                        if (kkk_str.Length > 0) // skip first k loop
                                        {
                                            kk_str.Append(" " + words[k].Text);
                                        }
                                        kkk_str.Append(" " + words[k].Text);

                                        if (words[k].Stopmark == Stopmark.None)
                                        {
                                            continue; // next k
                                        }
                                        else // there is a stopmark
                                        {
                                            if (s_numerology_system != null)
                                            {
                                                if (!String.IsNullOrEmpty(s_numerology_system.TextMode))
                                                {
                                                    if (s_numerology_system.TextMode == "Original")
                                                    {
                                                        str.Append(" " + stopmark_text);
                                                        if (kk_str.Length > 0)
                                                        {
                                                            kk_str.Append(" " + stopmark_text);
                                                        }
                                                        kkk_str.Append(" " + stopmark_text);
                                                    }
                                                }
                                            }

                                            // second CanStopAtEither found at k
                                            if (words[k].Stopmark == Stopmark.CanStopAtEither)
                                            {
                                                // ^ ذَٰلِكَ ٱلْكِتَٰبُ لَا رَيْبَ ۛ^ فِيهِ
                                                sentence = new Sentence(words[i].Verse, words[i].Position, words[k].Verse, words[k].Position + words[k].Text.Length, str.ToString());
                                                if (sentence != null)
                                                {
                                                    string text = sentence.ToString();
                                                    int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                                    if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                kk = k + 1; // backup k after second ^
                                                continue; // next k
                                            }
                                            else // non-CanStopAtEither stopmark
                                            {
                                                // kkk_str   فِيهِ ۛ^ هُدًۭى لِّلْمُتَّقِينَ
                                                sentence = new Sentence(words[j + 1].Verse, words[j + 1].Position, words[k].Verse, words[k].Position + words[k].Text.Length, kkk_str.ToString());
                                                if (sentence != null)
                                                {
                                                    string text = sentence.ToString();
                                                    int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                                    if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                // kk_str   هُدًۭى لِّلْمُتَّقِينَ
                                                sentence = new Sentence(words[kk].Verse, words[kk].Position, words[k].Verse, words[k].Position + words[k].Text.Length, kk_str.ToString());
                                                if (sentence != null)
                                                {
                                                    string text = sentence.ToString();
                                                    int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                                    if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                                    {
                                                        result.Add(sentence);
                                                    }
                                                }

                                                // skip the whole surrounding non-CanStopAtEither sentence
                                                j = k;
                                                break; // next j
                                            }
                                        }
                                    }

                                    // restart from last
                                    str.Length = 0;
                                    j = i - 1; // will be j++ by reloop
                                    done_CanStopAtEither = true;
                                }
                                else if (words[j].Stopmark == Stopmark.MustStop)
                                {
                                    Sentence sentence = new Sentence(words[i].Verse, words[i].Position, words[j].Verse, words[j].Position + words[j].Text.Length, str.ToString());
                                    if (sentence != null)
                                    {
                                        string text = sentence.ToString();
                                        int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                        if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                        {
                                            result.Add(sentence);
                                        }
                                    }

                                    i = j; // start new sentence after j
                                    break; // next i
                                }
                                else // unknown case
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by frequency - Verses
    public static List<Verse> FindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        return DoFindVerses(search_scope, current_selection, previous_verses, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
    }
    private static List<Verse> DoFindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindVerses(source, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
    }
    private static List<Verse> DoFindVerses(List<Verse> source, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        List<Verse> result = new List<Verse>();
        if (!string.IsNullOrEmpty(phrase))
        {
            if (source != null)
            {
                if (source.Count > 0)
                {
                    if (!String.IsNullOrEmpty(phrase))
                    {
                        foreach (Verse verse in source)
                        {
                            if (verse != null)
                            {
                                string text = verse.Text;
                                int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                {
                                    result.Add(verse);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by frequency - Chapters
    public static List<Chapter> FindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        return DoFindChapters(search_scope, current_selection, previous_verses, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
    }
    private static List<Chapter> DoFindChapters(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindChapters(source, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
    }
    private static List<Chapter> DoFindChapters(List<Verse> source, string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        List<Chapter> result = new List<Chapter>();
        if (!string.IsNullOrEmpty(phrase))
        {
            if (source != null)
            {
                if (source.Count > 0)
                {
                    if (s_book != null)
                    {
                        List<Chapter> source_chapters = s_book.GetChapters(source);
                        if (!String.IsNullOrEmpty(phrase))
                        {
                            foreach (Chapter chapter in source_chapters)
                            {
                                if (chapter != null)
                                {
                                    string text = chapter.Text;
                                    int letter_frequency_sum = CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
                                    if (Compare(letter_frequency_sum, sum, number_type, comparison_operator, sum_remainder))
                                    {
                                        result.Add(chapter);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
}
