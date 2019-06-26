using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Model;

public class Server : IPublisher
{
    #region Interfaces
    ///////////////////////////////////////////////////////////////////////////////
    // IPublisher method
    private Dictionary<Subject, List<ISubscriber>> m_subscribers = null;
    public void Subscribe(ISubscriber subscriber, Subject subject)
    {
        if (m_subscribers == null)
        {
            m_subscribers = new Dictionary<Subject, List<ISubscriber>>();
        }

        if (m_subscribers != null)
        {
            if (!m_subscribers.ContainsKey(subject))
            {
                m_subscribers.Add(subject, new List<ISubscriber>());
            }

            if (m_subscribers.ContainsKey(subject))
            {
                if (m_subscribers[subject] != null)
                {
                    m_subscribers[subject].Add(subscriber);
                }
            }
        }
    }
    // folder watcher
    private FileSystemWatcher m_file_system_watcher = null;
    public void WatchFolder(string folder_name, string filter)
    {
        if (Directory.Exists(folder_name))
        {
            m_file_system_watcher = new FileSystemWatcher();
            if (m_file_system_watcher != null)
            {
                m_file_system_watcher.Filter = filter;
                m_file_system_watcher.Path = folder_name;
                m_file_system_watcher.IncludeSubdirectories = true;

                m_file_system_watcher.NotifyFilter =
                    NotifyFilters.FileName |
                    NotifyFilters.Attributes |
                    NotifyFilters.CreationTime |
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Security |
                    NotifyFilters.Size |
                    NotifyFilters.DirectoryName;

                //m_file_system_watcher.Created += new FileSystemEventHandler(OnFileCreated);
                m_file_system_watcher.Changed += new FileSystemEventHandler(OnFileChanged);
                //m_file_system_watcher.Deleted += new FileSystemEventHandler(OnFileDeleted);
                //m_file_system_watcher.Renamed += new RenamedEventHandler(OnFileRenamed);

                m_file_system_watcher.EnableRaisingEvents = true;
            }
        }
    }
    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (e.FullPath.StartsWith("Languages"))
        {
            NotifySubscribers(Subject.LanguageSystem, e);
        }
        else
        {
            if (e.Name.Contains("interesting"))  // e.g. interesting.txt
            {
                Numbers.LoadInterestingNumbers();
                NotifySubscribers(Subject.InterestingNumbers, e);
            }
            else if (e.Name.Contains("DNA"))   // e.g. Simplified29_A29T29C19G23_DNA.txt
            {
                LoadDNASequenceSystem(s_dna_sequence_system.Name);
                NotifySubscribers(Subject.DNASequenceSystem, e);
            }
            else if (e.Name.Contains("_")) // e.g. Original_Alphabet_Primes1.txt
            {
                LoadNumerologySystem(s_numerology_system.Name);
                NotifySubscribers(Subject.NumerologySystem, e);
            }
            else                           // e.g. Simplified29.txt
            {
                LoadSimplificationSystem(s_simplification_system.Name);
                NotifySubscribers(Subject.SimplificationSystem, e);
            }
        }
    }
    // helper method
    private void NotifySubscribers(Subject subject, FileSystemEventArgs e)
    {
        if (m_subscribers != null)
        {
            if (m_subscribers.ContainsKey(subject))
            {
                foreach (ISubscriber item in m_subscribers[subject])
                {
                    try
                    {
                        item.Notify(subject, e);
                    }
                    catch
                    {
                        // handle exception if desired... }
                    }
                }
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion

    public const string DEFAULT_RECITATION = "Alafasy_64kbps";

    public const string DEFAULT_QURAN_TEXT = "quran-uthmani";
    public const string DEFAULT_EMLAAEI_TEXT = "ar.emlaaei";
    public const string DEFAULT_TRANSLATION = "en.qarai";
    public const string DEFAULT_OLD_TRANSLATION = "en.pickthall";
    public const string DEFAULT_TRANSLITERATION = "en.transliteration";
    public const string DEFAULT_WORD_MEANINGS = "en.wordbyword";
    public const string DEFAULT_TRANSLATION_1 = "en.sarwar";
    public const string DEFAULT_TRANSLATION_2 = "en.shakir";
    public const string DEFAULT_TRANSLATION_3 = "fa.khorramdel";
    public const string DEFAULT_TRANSLATION_4 = "id.muntakhab";
    public const string DEFAULT_TRANSLATION_5 = "tr.yildirim";
    public const string DEFAULT_TRANSLATION_6 = "ur.jawadi";
    public const string DEFAULT_TRANSLATION_7 = "zh.jian";

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

        if (!Directory.Exists(Globals.HELP_FOLDER))
        {
            Directory.CreateDirectory(Globals.HELP_FOLDER);
        }

        if (!Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            Directory.CreateDirectory(Globals.RESEARCH_FOLDER);
        }

        // load simplification systems
        LoadSimplificationSystems();

        // load numerology systems
        LoadNumerologySystems();

        // load dna sequence systems
        LoadDNASequenceSystems();

        // load help messages
        LoadHelpMessages();
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
                        throw new Exception("ERROR: No default simplification system was found.");
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
                            throw new Exception(filename + " file format must be:\r\n\tText TAB Replacement");
                        }
                    }
                }

                try
                {
                    // add to dictionary
                    s_loaded_simplification_systems.Add(simplification_system.Name, simplification_system);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            // set current simplification system
            if (s_loaded_simplification_systems.ContainsKey(text_mode))
            {
                s_simplification_system = s_loaded_simplification_systems[text_mode];
            }
        }
    }
    public static void BuildSimplifiedBook(string text_mode, bool with_bism_Allah, bool waw_as_word, bool shadda_as_letter, bool hamza_above_horizontal_line_as_letter, bool elf_above_horizontal_line_as_letter, bool yaa_above_horizontal_line_as_letter, bool noon_above_horizontal_line_as_letter, bool emlaaei_text)
    {
        if (!String.IsNullOrEmpty(text_mode))
        {
            if (s_loaded_simplification_systems != null)
            {
                if (s_loaded_simplification_systems.ContainsKey(text_mode))
                {
                    s_simplification_system = s_loaded_simplification_systems[text_mode];

                    // reload original Quran text
                    string filename = null;
                    if (emlaaei_text)
                    {
                        filename = Globals.TRANSLATIONS_FOLDER + "/" + DEFAULT_EMLAAEI_TEXT + ".txt";
                    }
                    else
                    {
                        filename = Globals.DATA_FOLDER + "/" + DEFAULT_QURAN_TEXT + ".txt";
                    }
                    List<string> lines = DataAccess.LoadVerseTexts(filename);

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
                    //for (int v = 0; v < lines.Count; v++)
                    //{
                    //    string[] words = lines[v].Split();

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
                    //string filename = "Stopmarks" + ".txt";
                    //if (Directory.Exists(Globals.DATA_FOLDER))
                    //{
                    //    string path = Globals.DATA_FOLDER + "/" + filename;
                    //    FileHelper.SaveText(path, str.ToString());
                    //    FileHelper.DisplayFile(path);
                    //}

                    List<Stopmark> verse_stopmarks = DataAccess.LoadVerseStopmarks();

                    // remove bismAllah from 112 chapters
                    if (!with_bism_Allah)
                    {
                        string bimsAllah_text1 = emlaaei_text ? "بِسْمِ اللَّهِ الرَّحْمَٰنِ الرَّحِيمِ " : "بِسْمِ ٱللَّهِ ٱلرَّحْمَٰنِ ٱلرَّحِيمِ ";
                        string bimsAllah_text2 = emlaaei_text ? "بِّسْمِ اللَّهِ الرَّحْمَٰنِ الرَّحِيمِ " : "بِّسْمِ ٱللَّهِ ٱلرَّحْمَٰنِ ٱلرَّحِيمِ "; // shadda on baa for chapter 95 and 97
                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].StartsWith(bimsAllah_text1))
                            {
                                lines[i] = lines[i].Replace(bimsAllah_text1, "");
                            }
                            else if (lines[i].StartsWith(bimsAllah_text2))
                            {
                                lines[i] = lines[i].Replace(bimsAllah_text2, "");
                            }
                        }
                    }

                    // Load WawAsWord words
                    if (waw_as_word)
                    {
                        LoadWawWords();

                        if (s_waw_words != null)
                        {
                            // replace shadda with previous letter and to waw exception list
                            if (shadda_as_letter)
                            {
                                for (int i = 0; i < lines.Count; i++)
                                {
                                    string[] word_texts = lines[i].Split();
                                    foreach (string word_text in word_texts)
                                    {
                                        if (s_waw_words.Contains(s_simplification_system.Simplify(word_text)))
                                        {
                                            if (word_text.Contains("ّ"))
                                            {
                                                string shadda_waw_word = null;
                                                for (int j = 1; j < word_text.Length; j++)
                                                {
                                                    if (word_text[j] == 'ّ')
                                                    {
                                                        shadda_waw_word = word_text.Insert(j, word_text[j - 1].ToString());
                                                        s_waw_words.Add(s_simplification_system.Simplify(shadda_waw_word));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // replace shadda with previous letter before any simplification
                    if (shadda_as_letter)
                    {
                        for (int i = 0; i < lines.Count; i++)
                        {
                            StringBuilder str = new StringBuilder(lines[i]);
                            for (int j = 1; j < str.Length; j++)
                            {
                                if (str[j] == 'ّ')
                                {
                                    str[j] = str[j - 1];
                                }
                            }
                            lines[i] = str.ToString();
                        }
                    }

                    // convert superscript above horizontal line to letter
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (hamza_above_horizontal_line_as_letter)
                        {
                            lines[i] = lines[i].Replace("ـٔ", "ء");
                        }

                        if (elf_above_horizontal_line_as_letter)
                        {
                            lines[i] = lines[i].Replace("ـٰ", "ا");
                        }

                        if (yaa_above_horizontal_line_as_letter)
                        {
                            lines[i] = lines[i].Replace("ـۧ", "ي");
                        }

                        if (noon_above_horizontal_line_as_letter)
                        {
                            lines[i] = lines[i].Replace("ـۨ", "ن");
                        }
                    }

                    // simplify verse texts
                    List<string> verse_texts = new List<string>();
                    foreach (string line in lines)
                    {
                        string verse_text = s_simplification_system.Simplify(line);
                        verse_texts.Add(verse_text);
                    }

                    // build verses
                    List<Verse> verses = new List<Verse>();
                    for (int i = 0; i < verse_texts.Count; i++)
                    {
                        Verse verse = new Verse(i + 1, verse_texts[i], verse_stopmarks[i]);
                        if (verse != null)
                        {
                            verses.Add(verse);
                            verse.ApplyWordStopmarks(lines[i]);
                        }
                    }

                    if (s_numerology_system != null)
                    {
                        s_book = new Book(text_mode, verses, s_numerology_system.AddDistancesWithinChapters);
                        if (s_book != null)
                        {
                            s_book.WithBismAllah = with_bism_Allah;
                            s_book.WawAsWord = waw_as_word;
                            s_book.ShaddaAsLetter = shadda_as_letter;
                            s_book.HamzaAboveHorizontalLineAsLetter = hamza_above_horizontal_line_as_letter;
                            s_book.ElfAboveHorizontalLineAsLetter = elf_above_horizontal_line_as_letter;
                            s_book.YaaAboveHorizontalLineAsLetter = yaa_above_horizontal_line_as_letter;
                            s_book.NoonAboveHorizontalLineAsLetter = noon_above_horizontal_line_as_letter;

                            // build words before DataAccess.Loads
                            if (waw_as_word)
                            {
                                SplitWawPrefixsAsWords(s_book, text_mode);
                            }
                            DataAccess.LoadRecitationInfos(s_book);
                            DataAccess.LoadTranslationInfos(s_book);
                            DataAccess.LoadTranslations(s_book);
                            DataAccess.LoadWordMeanings(s_book);
                            DataAccess.LoadWordRoots(s_book);
                            DataAccess.LoadWordParts(s_book);

                            // populate root-words dictionary
                            s_book.PopulateRootWords();
                        }
                    }
                }
            }
        }
    }
    private static List<string> s_waw_words = null;
    private static void LoadWawWords()
    {
        string filename = Globals.DATA_FOLDER + "/" + "waw-words.txt";
        if (File.Exists(filename))
        {
            s_waw_words = new List<string>();
            if (s_waw_words != null)
            {
                List<string> lines = FileHelper.LoadLines(filename);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('\t');
                    if (parts.Length > 0)
                    {
                        s_waw_words.Add(parts[0]);
                    }
                }
            }
        }
    }
    private static void SplitWawPrefixsAsWords(Book book, string text_mode)
    {
        if (book != null)
        {
            string filename = Globals.DATA_FOLDER + "/" + "waw-words.txt";
            if (File.Exists(filename))
            {
                // same spelling waw-words but their waw is prefix
                Dictionary<string, List<Verse>> non_exception_words_in_verses = new Dictionary<string, List<Verse>>();

                List<string> lines = FileHelper.LoadLines(filename);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('\t');
                    if (parts.Length > 0)
                    {
                        string exception_word = parts[0].SimplifyTo(text_mode);
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
                }

                if (s_waw_words != null)
                {
                    foreach (Verse verse in book.Verses)
                    {
                        StringBuilder str = new StringBuilder();
                        if (verse.Words.Count > 0)
                        {
                            for (int i = 0; i < verse.Words.Count; i++)
                            {
                                if (verse.Words[i].Text.StartsWith("و"))
                                {
                                    if (!s_waw_words.Contains(verse.Words[i].Text))
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
                }

                // update verses/words/letters numbers and distances
                if (s_numerology_system != null)
                {
                    book.SetupNumbersAndDistances(s_numerology_system.AddDistancesWithinChapters);
                }
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
                                    throw new Exception("ERROR: " + file.FullName + " must contain 3 parts separated by \"_\".");
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
                            throw new Exception("ERROR: No default numerology system was found.");
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
                                throw new Exception(filename + " file format must be:\r\n\tLetter TAB Value");
                            }
                        }
                        else
                        {
                            throw new Exception(filename + " file format must be:\r\n\tLetter TAB Value");
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
    public static void SaveNumerologySystem(string numerology_system_name)
    {
        if (String.IsNullOrEmpty(numerology_system_name)) return;
        if (numerology_system_name.Contains("DNA")) return;

        if (s_loaded_numerology_systems != null)
        {
            if (s_loaded_numerology_systems.ContainsKey(numerology_system_name))
            {
                NumerologySystem numerology_system = s_loaded_numerology_systems[numerology_system_name];
                if (numerology_system != null)
                {
                    if (Directory.Exists(Globals.VALUES_FOLDER))
                    {
                        string filename = Globals.VALUES_FOLDER + "/" + numerology_system.Name + ".txt";
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                            {
                                foreach (char key in numerology_system.Keys)
                                {
                                    writer.WriteLine(key + "\t" + numerology_system[key].ToString());
                                }
                            }
                        }
                        catch
                        {
                            // silence IO error in case running from read-only media (CD/DVD)
                        }
                    }
                }
            }
        }
    }
    public static void UpdateNumerologySystem(string text)
    {
        if (s_numerology_system != null)
        {
            if (!String.IsNullOrEmpty(text))
            {
                text = text.Replace("\r", "");
                text = text.Replace("\n", "");
                text = text.Replace("\t", "");
                text = text.Replace("_", "");
                text = text.Replace(" ", "");
                text = text.Replace(Constants.ORNATE_RIGHT_PARENTHESIS, "");
                text = text.Replace(Constants.ORNATE_LEFT_PARENTHESIS, "");
                foreach (char character in Constants.INDIAN_DIGITS)
                {
                    text = text.Replace(character.ToString(), "");
                }
                foreach (char character in Constants.DIACRITICS)
                {
                    text = text.Replace(character.ToString(), "");
                }
                foreach (char character in Constants.ARABIC_DIGITS)
                {
                    text = text.Replace(character.ToString(), "");
                }
                foreach (char character in Constants.STOPMARKS)
                {
                    text = text.Replace(character.ToString(), "");
                }
                foreach (char character in Constants.QURANMARKS)
                {
                    text = text.Replace(character.ToString(), "");
                }
                foreach (char character in Constants.SYMBOLS)
                {
                    text = text.Replace(character.ToString(), "");
                }

                BuildLetterStatistics(text);

                BuildNumerologySystem(text);

                if (s_book != null)
                {
                    if (s_book.Verses != null)
                    {
                        foreach (Verse verse in s_book.Verses)
                        {
                            CalculateValue(verse);
                        }
                    }
                }
            }
        }
    }
    private static void BuildNumerologySystem(string text)
    {
        if (s_loaded_numerology_systems != null)
        {
            if (s_numerology_system != null)
            {
                if (text != null)
                {
                    // build letter_order using letters in text only
                    string numerology_system_name = s_numerology_system.Name;
                    if (s_loaded_numerology_systems.ContainsKey(numerology_system_name))
                    {
                        NumerologySystem loaded_numerology_system = s_loaded_numerology_systems[numerology_system_name];

                        // re-generate numerology systems
                        List<char> letter_order = new List<char>();
                        List<long> letter_values = new List<long>();

                        switch (s_numerology_system.LetterOrder)
                        {
                            case "Alphabet":
                            case "Alphabet▲":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByLetter;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Ascending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            case "Alphabet▼":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByLetter;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Descending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            case "Appearance":
                            case "Appearance▲":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByOrder;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Ascending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            case "Appearance▼":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByOrder;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Descending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            case "Frequency▲":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByFrequency;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Ascending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            case "Frequency▼":
                            case "Frequency":
                                {
                                    LetterStatistic.SortMethod = StatisticSortMethod.ByFrequency;
                                    LetterStatistic.SortOrder = StatisticSortOrder.Descending;
                                    s_letter_statistics.Sort();
                                    foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                    {
                                        letter_order.Add(letter_statistic.Letter);
                                    }
                                }
                                break;
                            default: // use static numerology system
                                {
                                    foreach (char letter in loaded_numerology_system.LetterValues.Keys)
                                    {
                                        if (text.Contains(letter.ToString()))
                                        {
                                            letter_order.Add(letter);
                                        }
                                    }
                                }
                                break;
                        }

                        if (letter_order.Count > 0)
                        {
                            if (s_numerology_system.Name.EndsWith("Linear"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    letter_values.Add(i + 1L);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("NonAdditivePrimes1"))
                            {
                                letter_values.Add(1L);
                                for (int i = 0; i < letter_order.Count - 1; i++)
                                {
                                    letter_values.Add(Numbers.NonAdditivePrimes[i]);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("NonAdditivePrimes"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    letter_values.Add(Numbers.NonAdditivePrimes[i]);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("AdditivePrimes1"))
                            {
                                letter_values.Add(1L);
                                for (int i = 0; i < letter_order.Count - 1; i++)
                                {
                                    letter_values.Add(Numbers.AdditivePrimes[i]);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("AdditivePrimes"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    letter_values.Add(Numbers.AdditivePrimes[i]);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("Primes1"))
                            {
                                letter_values.Add(1L);
                                for (int i = 0; i < letter_order.Count - 1; i++)
                                {
                                    letter_values.Add(Numbers.Primes[i]);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("Primes"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    letter_values.Add(Numbers.Primes[i]);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("NonAdditiveComposites"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    letter_values.Add(Numbers.NonAdditiveComposites[i]);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("AdditiveComposites"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    letter_values.Add(Numbers.AdditiveComposites[i]);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("Composites"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    letter_values.Add(Numbers.Composites[i]);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("MersennePrimes"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    if (i < Numbers.MersennePrimes.Count)
                                    {
                                        letter_values.Add(Numbers.MersennePrimes[i]);
                                    }
                                    else
                                    {
                                        letter_values.Add(0L);
                                    }
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("Fibonacci"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    if (i < Numbers.Fibonaccis.Count)
                                    {
                                        letter_values.Add(Numbers.Fibonaccis[i]);
                                    }
                                    else
                                    {
                                        letter_values.Add(0L);
                                    }
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("Frequency▲"))
                            {
                                // letter-frequency mismacth: different letters for different frequencies
                                LetterStatistic.SortMethod = StatisticSortMethod.ByFrequency;
                                LetterStatistic.SortOrder = StatisticSortOrder.Ascending;
                                s_letter_statistics.Sort();
                                foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                {
                                    letter_values.Add(letter_statistic.Frequency);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("Frequency"))
                            {
                                letter_order.Clear();
                                foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                {
                                    letter_order.Add(letter_statistic.Letter);
                                    letter_values.Add(letter_statistic.Frequency);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("Frequency▼"))
                            {
                                // letter-frequency mismacth: different letters for different frequencies
                                LetterStatistic.SortMethod = StatisticSortMethod.ByFrequency;
                                LetterStatistic.SortOrder = StatisticSortOrder.Descending;
                                s_letter_statistics.Sort();
                                foreach (LetterStatistic letter_statistic in s_letter_statistics)
                                {
                                    letter_values.Add(letter_statistic.Frequency);
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("Gematria"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    if (i < Numbers.Gematria.Count)
                                    {
                                        letter_values.Add(Numbers.Gematria[i]);
                                    }
                                    else
                                    {
                                        letter_values.Add(0L);
                                    }
                                }
                            }
                            else if (s_numerology_system.Name.EndsWith("QuranNumbers"))
                            {
                                for (int i = 0; i < letter_order.Count; i++)
                                {
                                    if (i < Numbers.QuranNumbers.Count)
                                    {
                                        letter_values.Add(Numbers.QuranNumbers[i]);
                                    }
                                    else
                                    {
                                        letter_values.Add(0L);
                                    }
                                }
                            }
                            else // if not defined in Numbers
                            {
                                // use loadeded numerology system instead
                                foreach (long value in loaded_numerology_system.LetterValues.Values)
                                {
                                    letter_values.Add(value);
                                }
                            }
                        }
                        else // if not defined in Numbers
                        {
                            // use loadeded numerology system instead
                            foreach (long value in loaded_numerology_system.LetterValues.Values)
                            {
                                letter_values.Add(value);
                            }
                        }

                        // rebuild the current numerology system
                        s_numerology_system.Clear();
                        for (int i = 0; i < letter_order.Count; i++)
                        {
                            s_numerology_system.Add(letter_order[i], letter_values[i]);
                        }
                    }
                }
            }
        }
    }

    // loaded dna sequence systems [STATIC]
    private static Dictionary<string, DNASequenceSystem> s_loaded_dna_sequence_systems = null;
    public static Dictionary<string, DNASequenceSystem> LoadedDNASequenceSystems
    {
        get { return s_loaded_dna_sequence_systems; }
    }
    private static void LoadDNASequenceSystems()
    {
        if (s_loaded_dna_sequence_systems == null)
        {
            s_loaded_dna_sequence_systems = new Dictionary<string, DNASequenceSystem>();
        }

        if (s_loaded_dna_sequence_systems != null)
        {
            s_loaded_dna_sequence_systems.Clear();

            string path = Globals.VALUES_FOLDER;
            DirectoryInfo folder = new DirectoryInfo(path);
            if (folder != null)
            {
                FileInfo[] files = folder.GetFiles("*.txt");
                if ((files != null) && (files.Length > 0))
                {
                    foreach (FileInfo file in files)
                    {
                        string dna_sequence_system_name = file.Name.Remove(file.Name.Length - 4, 4);
                        if (!String.IsNullOrEmpty(dna_sequence_system_name))
                        {
                            if (dna_sequence_system_name.Contains("DNA"))
                            {
                                string[] parts = dna_sequence_system_name.Split('_');
                                if (parts.Length == 3)
                                {
                                    LoadDNASequenceSystem(dna_sequence_system_name);
                                }
                                else
                                {
                                    throw new Exception("ERROR: " + file.FullName + " must contain 3 parts separated by \"_\".");
                                }
                            }
                        }
                    }

                    // start with default dna_sequence system
                    if (s_loaded_dna_sequence_systems.ContainsKey(DNASequenceSystem.DEFAULT_NAME))
                    {
                        s_dna_sequence_system = new DNASequenceSystem(s_loaded_dna_sequence_systems[DNASequenceSystem.DEFAULT_NAME]);
                    }
                    else
                    {
                        throw new Exception("ERROR: No default dna sequence system was found.");
                    }
                }
            }
        }
    }
    // dna sequence system [DYNAMIC]
    private static DNASequenceSystem s_dna_sequence_system = null;
    public static DNASequenceSystem DNASequenceSystem
    {
        get { return s_dna_sequence_system; }
    }
    public static void LoadDNASequenceSystem(string dna_sequence_system_name)
    {
        if (String.IsNullOrEmpty(dna_sequence_system_name)) return;

        if (dna_sequence_system_name.Contains("DNA"))
        {
            if (s_loaded_dna_sequence_systems != null)
            {
                // remove and rebuild on the fly without restarting application
                if (s_loaded_dna_sequence_systems.ContainsKey(dna_sequence_system_name))
                {
                    s_loaded_dna_sequence_systems.Remove(dna_sequence_system_name);
                }

                string filename = Globals.VALUES_FOLDER + "/" + dna_sequence_system_name + ".txt";
                if (File.Exists(filename))
                {
                    List<string> lines = FileHelper.LoadLines(filename);

                    DNASequenceSystem dna_sequence_system = new DNASequenceSystem(dna_sequence_system_name);
                    if (dna_sequence_system != null)
                    {
                        dna_sequence_system.LetterValues.Clear();
                        foreach (string line in lines)
                        {
                            if (line.StartsWith("#")) continue;

                            string[] parts = line.Split('\t');
                            if (parts.Length == 2)
                            {
                                dna_sequence_system.LetterValues.Add(parts[0][0], parts[1][0]);
                            }
                            else
                            {
                                throw new Exception(filename + " file format must be:\r\n\tLetter TAB Value");
                            }
                        }
                    }

                    try
                    {
                        // add to dictionary
                        s_loaded_dna_sequence_systems.Add(dna_sequence_system.Name, dna_sequence_system);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                // set current dna_sequence system
                if (s_loaded_dna_sequence_systems.ContainsKey(dna_sequence_system_name))
                {
                    s_dna_sequence_system = s_loaded_dna_sequence_systems[dna_sequence_system_name];
                }
            }
        }
    }
    public static void SaveDNASequenceSystem(string dna_sequence_system_name)
    {
        if (String.IsNullOrEmpty(dna_sequence_system_name)) return;

        if (dna_sequence_system_name.Contains("DNA"))
        {
            if (s_loaded_dna_sequence_systems != null)
            {
                if (s_loaded_dna_sequence_systems.ContainsKey(dna_sequence_system_name))
                {
                    DNASequenceSystem dna_sequence_system = s_loaded_dna_sequence_systems[dna_sequence_system_name];
                    if (dna_sequence_system != null)
                    {
                        if (Directory.Exists(Globals.VALUES_FOLDER))
                        {
                            string filename = Globals.VALUES_FOLDER + "/" + dna_sequence_system.Name + ".txt";
                            try
                            {
                                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                                {
                                    foreach (char key in dna_sequence_system.Keys)
                                    {
                                        writer.WriteLine(key + "\t" + dna_sequence_system[key].ToString());
                                    }
                                }
                            }
                            catch
                            {
                                // silence IO error in case running from read-only media (CD/DVD)
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

    public static CalculationMode CalculationMode = CalculationMode.SumOfLetterValues;
    // log value calculations
    public static StringBuilder Log = new StringBuilder();
    public static long ValueSum = 0L;
    public static long LSum = 0L;
    public static long WSum = 0L;
    public static long VSum = 0L;
    public static long CSum = 0L;
    public static long pLSum = 0L;
    public static long pWSum = 0L;
    public static long pVSum = 0L;
    public static long pCSum = 0L;
    public static long nLSum = 0L;
    public static long nWSum = 0L;
    public static long nVSum = 0L;
    public static long nCSum = 0L;
    public static void ClearLog()
    {
        Log.Length = 0;
        ValueSum = 0L;
        LSum = 0L;
        WSum = 0L;
        VSum = 0L;
        CSum = 0L;
        pLSum = 0L;
        pWSum = 0L;
        pVSum = 0L;
        pCSum = 0L;
        nLSum = 0L;
        nWSum = 0L;
        nVSum = 0L;
        nCSum = 0L;
    }
    // used for user text or Quran highlighted text in CalculationMode.SumOfUniqueLetterValues
    public static long CalculateValue(char character)
    {
        if (character == '\0') return 0L;
        if (character == '\r') return 0L;
        if (character == '\n') return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            string s = character.ToString().SimplifyTo(s_numerology_system.TextMode);
            if (!String.IsNullOrEmpty(s))
            {
                char c = s[0];
                result = s_numerology_system.CalculateValue(c);
            }
        }
        return result;
    }
    public static long CalculateValue(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            text = text.Replace("\r\n", "\n");
            text = text.SimplifyTo(s_numerology_system.TextMode);
            string[] word_texts = text.Split();
            switch (CalculationMode)
            {
                case CalculationMode.SumOfLetterValues:
                    {
                        foreach (string word_text in word_texts)
                        {
                            foreach (char c in word_text)
                            {
                                result += CalculateValue(c);
                            }
                        }
                    }
                    break;
                case CalculationMode.SumOfUniqueLetterValues:
                    {
                        text = text.RemoveDuplicates();
                        foreach (char c in text)
                        {
                            result += CalculateValue(c);
                        }
                    }
                    break;
                case CalculationMode.SumOfWordValueDigitSums:
                    {
                        foreach (string word_text in word_texts)
                        {
                            long word_value = 0L;
                            foreach (char c in word_text)
                            {
                                word_value += CalculateValue(c);
                            }
                            result += Numbers.DigitSum(word_value);
                        }
                    }
                    break;
                case CalculationMode.SumOfWordValueDigitalRoots:
                    {
                        foreach (string word_text in word_texts)
                        {
                            long word_value = 0L;
                            foreach (char c in word_text)
                            {
                                word_value += CalculateValue(c);
                            }
                            result += Numbers.DigitalRoot(word_value);
                        }
                    }
                    break;
                default:
                    {
                        // do nothing
                    }
                    break;
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(char character)
    {
        if (character == '\0') return 0L;
        if (character == '\r') return 0L;
        if (character == '\n') return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            string s = character.ToString().SimplifyTo(s_numerology_system.TextMode);
            if (!String.IsNullOrEmpty(s))
            {
                Log.Append(s);
                char c = s[0];
                long value = s_numerology_system.CalculateValue(c);
                result += value; ValueSum += value;
                Log.AppendLine("\t" + value);
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            text = text.Replace("\r\n", "\n");
            text = text.SimplifyTo(s_numerology_system.TextMode);

            string[] word_texts = text.Split();
            switch (CalculationMode)
            {
                case CalculationMode.SumOfLetterValues:
                    {
                        foreach (char c in text)
                        {
                            if (c == '\n')
                            {
                                Log.AppendLine();
                                Log.AppendLine();
                            }
                            else if (c == ' ')
                            {
                                Log.AppendLine();
                            }
                            else
                            {
                                Log.Append(c.ToString());
                                long value = s_numerology_system.CalculateValue(c);
                                result += value; ValueSum += value;
                                Log.AppendLine("\t" + value);
                            }
                        }
                    }
                    break;
                case CalculationMode.SumOfUniqueLetterValues:
                    {
                        text = text.RemoveDuplicates();
                        foreach (char c in text)
                        {
                            result += CalculateValue(c);
                        }
                    }
                    break;
                case CalculationMode.SumOfWordValueDigitSums:
                    {
                        foreach (string word_text in word_texts)
                        {
                            long word_value = 0L;
                            foreach (char c in word_text)
                            {
                                word_value += CalculateValue(c);
                            }
                            result += Numbers.DigitSum(word_value);
                        }
                    }
                    break;
                case CalculationMode.SumOfWordValueDigitalRoots:
                    {
                        foreach (string word_text in word_texts)
                        {
                            long word_value = 0L;
                            foreach (char c in word_text)
                            {
                                word_value += CalculateValue(c);
                            }
                            result += Numbers.DigitalRoot(word_value);
                        }
                    }
                    break;
                default:
                    {
                        // do nothing
                    }
                    break;
            }
        }
        return result;
    }
    // used for Quran text for non-CalculationMode.SumOfUniqueLetterValues
    public static long CalculateValue(Letter letter)
    {
        if (letter == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            result = s_numerology_system.CalculateValue(letter.Character);

            if (s_numerology_system.LetterValue.StartsWith("Base"))
            {
                // do nothing special
            }
            else
            {
                // adjust value of letter
                result += AdjustValue(letter);

                // adjust value of word
                if (letter.Word.Letters.Count == 1)
                {
                    result += AdjustValue(letter.Word);
                }

                // adjust value of verse
                if (letter.Word.Verse.Words.Count == 1)
                {
                    if (letter.Word.Letters.Count == 1)
                    {
                        result += AdjustValue(letter.Word.Verse);
                    }
                }
            }
        }
        return result;
    }
    public static long CalculateValue(List<Letter> letters)
    {
        if (letters == null) return 0L;
        if (letters.Count == 0) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            foreach (Letter letter in letters)
            {
                result += CalculateValue(letter);
            }

            List<Word> words = GetCompleteWords(letters);
            if (words != null)
            {
                foreach (Word word in words)
                {
                    // adjust value of word
                    result += AdjustValue(word);
                }
            }

            List<Verse> verses = GetCompleteVerses(words);
            if (verses != null)
            {
                foreach (Verse verse in verses)
                {
                    // adjust value of verse
                    result += AdjustValue(verse);
                }
            }

            List<Chapter> chapters = GetCompleteChapters(verses);
            if (chapters != null)
            {
                foreach (Chapter chapter in chapters)
                {
                    // adjust value of chapter
                    result += AdjustValue(chapter);
                }
            }
        }
        return result;
    }
    public static long CalculateValue(Word word)
    {
        if (word == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (s_numerology_system.LetterValue.StartsWith("Base"))
            {
                string radix_str = "";
                int pos = 4;
                while (Char.IsDigit(s_numerology_system.LetterValue[pos]))
                {
                    radix_str += s_numerology_system.LetterValue[pos];
                    pos++;
                }
                int radix;
                if (int.TryParse(radix_str, out radix))
                {
                    StringBuilder str = new StringBuilder();
                    foreach (Letter letter in word.Letters)
                    {
                        str.Insert(0, s_numerology_system.CalculateValue(letter.Character));
                    }
                    result += Radix.Decode(str.ToString(), radix);
                }
            }
            else
            {
                switch (CalculationMode)
                {
                    case CalculationMode.SumOfLetterValues:
                        {
                            foreach (Letter letter in word.Letters)
                            {
                                result += CalculateValue(letter);
                            }
                        }
                        break;
                    case CalculationMode.SumOfUniqueLetterValues:
                        {
                            string text = word.Text;
                            text = text.RemoveDuplicates();
                            foreach (char c in text)
                            {
                                result += CalculateValue(c);
                            }
                        }
                        break;
                    case CalculationMode.SumOfWordValueDigitSums:
                        {
                            long word_value = 0L;
                            foreach (Letter letter in word.Letters)
                            {
                                word_value += CalculateValue(letter);
                            }
                            result += Numbers.DigitSum(word_value);
                        }
                        break;
                    case CalculationMode.SumOfWordValueDigitalRoots:
                        {
                            long word_value = 0L;
                            foreach (Letter letter in word.Letters)
                            {
                                word_value += CalculateValue(letter);
                            }
                            result += Numbers.DigitalRoot(word_value);
                        }
                        break;
                    default:
                        {
                            // do nothing
                        }
                        break;
                }

                if (word.Letters.Count > 0)
                {
                    // adjust value of word
                    result += AdjustValue(word);

                    Verse verse = word.Verse;
                    if (verse != null)
                    {
                        if (word.Verse.Words.Count == 1)
                        {
                            // adjust value of verse
                            result += AdjustValue(word.Verse);
                        }
                    }
                }
            }
        }
        return result;
    }
    public static long CalculateValue(List<Word> words)
    {
        if (words == null) return 0L;
        if (words.Count == 0) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            foreach (Word word in words)
            {
                result += CalculateValue(word);
            }

            List<Verse> verses = GetCompleteVerses(words);
            if (verses != null)
            {
                foreach (Verse verse in verses)
                {
                    // adjust value of verse
                    result += AdjustValue(verse);
                }
            }

            List<Chapter> chapters = GetCompleteChapters(verses);
            if (chapters != null)
            {
                foreach (Chapter chapter in chapters)
                {
                    // adjust value of chapter
                    result += AdjustValue(chapter);
                }
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
            if (s_numerology_system.LetterValue.StartsWith("Base"))
            {
                string radix_str = "";
                int pos = 4;
                while (Char.IsDigit(s_numerology_system.LetterValue[pos]))
                {
                    radix_str += s_numerology_system.LetterValue[pos];
                    pos++;
                }
                int radix;
                if (int.TryParse(radix_str, out radix))
                {
                    StringBuilder str = new StringBuilder();

                    List<Word> words = GetCompleteWords(sentence);
                    if (words != null)
                    {
                        foreach (Word word in words)
                        {
                            foreach (Letter letter in word.Letters)
                            {
                                str.Insert(0, s_numerology_system.CalculateValue(letter.Character));
                            }
                            result += Radix.Decode(str.ToString(), radix);

                            str.Length = 0;
                        }
                    }
                }
            }
            else
            {
                List<Word> words = GetCompleteWords(sentence);
                if (words != null)
                {
                    foreach (Word word in words)
                    {
                        foreach (Letter letter in word.Letters)
                        {
                            result += CalculateValue(letter);
                        }

                        // adjust value of word
                        result += AdjustValue(word);
                    }
                }

                List<Verse> verses = GetCompleteVerses(sentence);
                if (verses != null)
                {
                    foreach (Verse verse in verses)
                    {
                        // adjust value of verse
                        result += AdjustValue(verse);
                    }
                }
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
            if (s_numerology_system.LetterValue.StartsWith("Base"))
            {
                string radix_str = "";
                int pos = 4;
                while (Char.IsDigit(s_numerology_system.LetterValue[pos]))
                {
                    radix_str += s_numerology_system.LetterValue[pos];
                    pos++;
                }
                int radix;
                if (int.TryParse(radix_str, out radix))
                {
                    StringBuilder str = new StringBuilder();
                    foreach (Word word in verse.Words)
                    {
                        foreach (Letter letter in word.Letters)
                        {
                            str.Insert(0, s_numerology_system.CalculateValue(letter.Character));
                        }
                        result += Radix.Decode(str.ToString(), radix);

                        str.Length = 0;
                    }
                }
            }
            else
            {
                if (CalculationMode == CalculationMode.SumOfUniqueLetterValues)
                {
                    string text = verse.Text.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
                    result += CalculateValue(text);
                }
                else
                {
                    foreach (Word word in verse.Words)
                    {
                        result += CalculateValue(word);
                    }
                }

                // adjust value of verse
                result += AdjustValue(verse);
            }
        }
        return result;
    }
    public static long CalculateValue(Verse verse, Letter from_letter, Letter to_letter)
    {
        if (verse == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (s_numerology_system.LetterValue.StartsWith("Base"))
            {
                string radix_str = "";
                int pos = 4;
                while (Char.IsDigit(s_numerology_system.LetterValue[pos]))
                {
                    radix_str += s_numerology_system.LetterValue[pos];
                    pos++;
                }
                int radix;
                if (int.TryParse(radix_str, out radix))
                {
                    StringBuilder str = new StringBuilder();

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

                                if (letter_index < from_letter.NumberInVerse - 1) continue;
                                if (letter_index > to_letter.NumberInVerse - 1)
                                {
                                    done = true;
                                    break;
                                }

                                str.Insert(0, s_numerology_system.CalculateValue(letter.Character));
                            }
                        }
                        result += Radix.Decode(str.ToString(), radix);

                        str.Length = 0;

                        if (done) break;
                    }
                }
            }
            else
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

                            if (letter_index < from_letter.NumberInVerse - 1) continue;
                            if (letter_index > to_letter.NumberInVerse - 1)
                            {
                                done = true;
                                break;
                            }

                            result += CalculateValue(letter);
                        }

                        if ((from_letter.NumberInVerse <= word.Letters[0].NumberInVerse)                    // if selection starts before or at first letter in word
                            &&                                                                              // AND
                            (to_letter.NumberInVerse >= word.Letters[word.Letters.Count - 1].NumberInVerse) // if selection ends   at or after  last  letter in word
                           )
                        {
                            // adjust value of word
                            result += AdjustValue(word);
                        }
                    }

                    if (done) break;
                }

                if ((from_letter.NumberInVerse == 1) && (to_letter.NumberInVerse == verse.LetterCount))
                {
                    // adjust value of verse
                    result += AdjustValue(verse);
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
            if (CalculationMode == CalculationMode.SumOfUniqueLetterValues)
            {
                string text = "";
                foreach (Verse verse in verses)
                {
                    text += verse.Text;
                }
                text = text.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
                result += CalculateValue(text);
            }
            else
            {
                foreach (Verse verse in verses)
                {
                    result += CalculateValue(verse);
                }
            }

            List<Chapter> chapters = GetCompleteChapters(verses);
            if (chapters != null)
            {
                foreach (Chapter chapter in chapters)
                {
                    if (chapter != null)
                    {
                        // adjust value of chapter
                        result += AdjustValue(chapter);
                    }
                }
            }
        }
        return result;
    }
    public static long CalculateValue(List<Verse> verses, Letter start_letter, Letter end_letter)
    {
        if (verses == null) return 0L;
        if (verses.Count == 0) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (verses.Count == 1)
            {
                result += CalculateValue(verses[0], start_letter, end_letter);
            }
            else if (verses.Count == 2)
            {
                Word first_verse_end_word = verses[0].Words[verses[0].Words.Count - 1];
                if (first_verse_end_word != null)
                {
                    if (first_verse_end_word.Letters.Count > 0)
                    {
                        Letter first_verse_end_letter = first_verse_end_word.Letters[first_verse_end_word.Letters.Count - 1];
                        if (first_verse_end_letter != null)
                        {
                            result += CalculateValue(verses[0], start_letter, first_verse_end_letter);
                        }
                    }
                }

                Word last_verse_start_word = verses[1].Words[0];
                if (last_verse_start_word != null)
                {
                    if (last_verse_start_word.Letters.Count > 0)
                    {
                        Letter last_verse_start_letter = last_verse_start_word.Letters[0];
                        if (last_verse_start_letter != null)
                        {
                            result += CalculateValue(verses[1], last_verse_start_letter, end_letter);
                        }
                    }
                }
            }
            else //if (verses.Count > 2)
            {
                // WARNING: no null check ???
                bool first_verse_is_fully_selected = (start_letter.NumberInChapter == 1);
                bool last_verse_is_fully_selected = (end_letter.NumberInChapter == end_letter.Word.Verse.Chapter.LetterCount);
                Chapter first_chapter = start_letter.Word.Verse.Chapter;
                Chapter last_chapter = end_letter.Word.Verse.Chapter;

                // first verse
                Word first_verse_end_word = verses[0].Words[verses[0].Words.Count - 1];
                if (first_verse_end_word != null)
                {
                    if (first_verse_end_word.Letters.Count > 0)
                    {
                        Letter first_verse_end_letter = first_verse_end_word.Letters[first_verse_end_word.Letters.Count - 1];
                        if (first_verse_end_letter != null)
                        {
                            result += CalculateValue(verses[0], start_letter, first_verse_end_letter);
                        }
                    }
                }

                // middle verses
                for (int i = 1; i < verses.Count - 1; i++)
                {
                    result += CalculateValue(verses[i]);

                    Verse verse = verses[i];
                    if (verse != null)
                    {
                        Chapter chapter = verse.Chapter;
                        if (chapter != null)
                        {
                            if (verse.NumberInChapter == verse.Chapter.Verses.Count)    // last verse in chapter
                            {
                                if (
                                     (verse.Chapter != first_chapter)
                                     ||
                                     ((verse.Chapter == first_chapter) && first_verse_is_fully_selected)
                                   )
                                {
                                    List<Chapter> chapters = GetCompleteChapters(verses);
                                    if (chapters != null)
                                    {
                                        if (chapters.Contains(chapter))
                                        {
                                            // adjust value of chapter
                                            result += AdjustValue(chapter);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // last verse
                Word last_verse_start_word = verses[verses.Count - 1].Words[0];
                if (last_verse_start_word != null)
                {
                    if (last_verse_start_word.Letters.Count > 0)
                    {
                        Letter last_verse_start_letter = last_verse_start_word.Letters[0];
                        if (last_verse_start_letter != null)
                        {
                            result += CalculateValue(verses[verses.Count - 1], last_verse_start_letter, end_letter);
                            Verse verse = verses[verses.Count - 1];
                            if (verse != null)
                            {
                                if (
                                     ((last_chapter == first_chapter) && first_verse_is_fully_selected && last_verse_is_fully_selected)
                                     ||
                                     ((last_chapter != first_chapter) && (verse.Chapter == last_chapter) && last_verse_is_fully_selected)
                                   )
                                {
                                    Chapter chapter = verse.Chapter;
                                    if (chapter != null)
                                    {
                                        if (verse.NumberInChapter == verse.Chapter.Verses.Count)    // last verse in chapter
                                        {
                                            List<Chapter> chapters = GetCompleteChapters(verses);
                                            if (chapters != null)
                                            {
                                                if (chapters.Contains(chapter))
                                                {
                                                    // adjust value of chapter
                                                    result += AdjustValue(chapter);
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
        return result;
    }
    public static long CalculateValue(Chapter chapter)
    {
        if (chapter == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (CalculationMode == CalculationMode.SumOfUniqueLetterValues)
            {
                string text = chapter.Text.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
                result += CalculateValue(text);
            }
            else
            {
                result += CalculateValue(chapter.Verses);
            }

            chapter.Value = result; // update chapter values for ChapterSortMethod.ByValue
        }
        return result;
    }
    public static long CalculateValue(List<Chapter> chapters)
    {
        if (chapters == null) return 0L;
        if (chapters.Count == 0) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (CalculationMode == CalculationMode.SumOfUniqueLetterValues)
            {
                string text = "";
                foreach (Chapter chapter in chapters)
                {
                    text += chapter.Text;
                }
                text = text.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
                result += CalculateValue(text);
            }
            else
            {
                foreach (Chapter chapter in chapters)
                {
                    result += CalculateValue(chapter);
                }
            }
        }
        return result;
    }
    public static long CalculateValue(Book book)
    {
        if (book == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (CalculationMode == CalculationMode.SumOfUniqueLetterValues)
            {
                string text = book.Text.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
                result += CalculateValue(text);
            }
            else
            {
                result += CalculateValue(book.Chapters);
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(Letter letter)
    {
        if (letter == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            Log.Append(letter.ToString());

            long value = s_numerology_system.CalculateValue(letter.Character);
            result += value; ValueSum += value;
            Log.Append("\t" + value);

            if (s_numerology_system.LetterValue.StartsWith("Base"))
            {
                // do nothing special
            }
            else
            {
                // adjust value of letter
                result += AdjustValueWithLogging(letter);

                // adjust value of word
                if (letter.Word.Letters.Count == 1)
                {
                    result += AdjustValueWithLogging(letter.Word);
                }

                // adjust value of verse
                if (letter.Word.Verse.Words.Count == 1)
                {
                    if (letter.Word.Letters.Count == 1)
                    {
                        result += AdjustValueWithLogging(letter.Word.Verse);
                    }
                }
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(List<Letter> letters)
    {
        if (letters == null) return 0L;
        if (letters.Count == 0) return 0L;

        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            foreach (Letter letter in letters)
            {
                result += CalculateValueWithLogging(letter);
            }

            List<Word> words = GetCompleteWords(letters);
            if (words != null)
            {
                foreach (Word word in words)
                {
                    value = s_numerology_system.CalculateValue(word.Text);
                    Log.Append("\t" + "\t" + value);
                    // adjust value of word
                    result += AdjustValueWithLogging(word);
                }
            }

            List<Verse> verses = GetCompleteVerses(words);
            if (verses != null)
            {
                foreach (Verse verse in verses)
                {
                    value = s_numerology_system.CalculateValue(verse.Text);
                    Log.Append("\t" + "\t" + "\t" + value);
                    // adjust value of verse
                    result += AdjustValueWithLogging(verse);
                }
            }

            List<Chapter> chapters = GetCompleteChapters(verses);
            if (chapters != null)
            {
                foreach (Chapter chapter in chapters)
                {
                    value = s_numerology_system.CalculateValue(chapter.Text);
                    Log.Append("\t" + "\t" + "\t" + "\t" + value);
                    // adjust value of chapter
                    result += AdjustValueWithLogging(chapter);
                }
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(Word word)
    {
        if (word == null) return 0L;

        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            if (s_numerology_system.LetterValue.StartsWith("Base"))
            {
                string radix_str = "";
                int pos = 4;
                while (Char.IsDigit(s_numerology_system.LetterValue[pos]))
                {
                    radix_str += s_numerology_system.LetterValue[pos];
                    pos++;
                }
                int radix;
                if (int.TryParse(radix_str, out radix))
                {
                    StringBuilder str = new StringBuilder();
                    foreach (Letter letter in word.Letters)
                    {
                        str.Insert(0, s_numerology_system.CalculateValue(letter.Character));
                    }
                    value = Radix.Decode(str.ToString(), radix);
                    result += value; ValueSum += value;
                    Log.Append("\t" + value);
                }
            }
            else
            {
                switch (CalculationMode)
                {
                    case CalculationMode.SumOfLetterValues:
                        {
                            foreach (Letter letter in word.Letters)
                            {
                                result += CalculateValueWithLogging(letter);
                            }
                        }
                        break;
                    case CalculationMode.SumOfUniqueLetterValues:
                        {
                            string text = word.Text;
                            text = text.RemoveDuplicates();
                            foreach (char c in text)
                            {
                                result += CalculateValueWithLogging(c);
                            }
                        }
                        break;
                    case CalculationMode.SumOfWordValueDigitSums:
                        {
                            long word_value = 0L;
                            foreach (Letter letter in word.Letters)
                            {
                                word_value += CalculateValueWithLogging(letter);
                            }
                            result += Numbers.DigitSum(word_value);
                        }
                        break;
                    case CalculationMode.SumOfWordValueDigitalRoots:
                        {
                            long word_value = 0L;
                            foreach (Letter letter in word.Letters)
                            {
                                word_value += CalculateValueWithLogging(letter);
                            }
                            result += Numbers.DigitalRoot(word_value);
                        }
                        break;
                    default:
                        {
                            // do nothing
                        }
                        break;
                }

                //????? update the following code too to take into account CalculationMode
                value = s_numerology_system.CalculateValue(word.Text);
                Log.Append("\t" + "\t" + value);
                // adjust value of word
                result += AdjustValueWithLogging(word);

                Verse verse = word.Verse;
                if (verse != null)
                {
                    if (word.Verse.Words.Count == 1)
                    {
                        value = s_numerology_system.CalculateValue(verse.Text);
                        Log.Append("\t" + "\t" + "\t" + value);
                        // adjust value of verse
                        result += AdjustValueWithLogging(word.Verse);
                    }
                }
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(List<Word> words)
    {
        if (words == null) return 0L;
        if (words.Count == 0) return 0L;

        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            foreach (Word word in words)
            {
                result += CalculateValueWithLogging(word);
            }

            List<Verse> verses = GetCompleteVerses(words);
            if (verses != null)
            {
                foreach (Verse verse in verses)
                {
                    value = s_numerology_system.CalculateValue(verse.Text);
                    Log.Append("\t" + "\t" + "\t" + value);
                    // adjust value of verse
                    result += AdjustValueWithLogging(verse);
                }
            }

            List<Chapter> chapters = GetCompleteChapters(verses);
            if (chapters != null)
            {
                foreach (Chapter chapter in chapters)
                {
                    value = s_numerology_system.CalculateValue(chapter.Text);
                    Log.Append("\t" + "\t" + "\t" + "\t" + value);
                    // adjust value of chapter
                    result += AdjustValueWithLogging(chapter);
                }
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(Sentence sentence)
    {
        if (sentence == null) return 0L;

        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            if (s_numerology_system.LetterValue.StartsWith("Base"))
            {
                string radix_str = "";
                int pos = 4;
                while (Char.IsDigit(s_numerology_system.LetterValue[pos]))
                {
                    radix_str += s_numerology_system.LetterValue[pos];
                    pos++;
                }
                int radix;
                if (int.TryParse(radix_str, out radix))
                {
                    StringBuilder str = new StringBuilder();

                    List<Word> words = GetCompleteWords(sentence);
                    if (words != null)
                    {
                        foreach (Word word in words)
                        {
                            foreach (Letter letter in word.Letters)
                            {
                                str.Insert(0, s_numerology_system.CalculateValue(letter.Character));
                            }
                            value = Radix.Decode(str.ToString(), radix);
                            result += value; ValueSum += value;
                            Log.Append("\t" + value);

                            str.Length = 0;
                        }
                    }
                }
            }
            else
            {
                List<Word> words = GetCompleteWords(sentence);
                if (words != null)
                {
                    foreach (Word word in words)
                    {
                        foreach (Letter letter in word.Letters)
                        {
                            result += CalculateValueWithLogging(letter);
                        }

                        value = s_numerology_system.CalculateValue(word.Text);
                        Log.Append("\t" + "\t" + value);
                        // adjust value of word
                        result += AdjustValueWithLogging(word);
                    }
                }

                List<Verse> verses = GetCompleteVerses(sentence);
                if (verses != null)
                {
                    foreach (Verse verse in verses)
                    {
                        value = s_numerology_system.CalculateValue(verse.Text);
                        Log.Append("\t" + "\t" + "\t" + value);
                        // adjust value of verse
                        result += AdjustValueWithLogging(verse);
                    }
                }
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(Verse verse)
    {
        if (verse == null) return 0L;

        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            if (s_numerology_system.LetterValue.StartsWith("Base"))
            {
                string radix_str = "";
                int pos = 4;
                while (Char.IsDigit(s_numerology_system.LetterValue[pos]))
                {
                    radix_str += s_numerology_system.LetterValue[pos];
                    pos++;
                }
                int radix;
                if (int.TryParse(radix_str, out radix))
                {
                    StringBuilder str = new StringBuilder();
                    foreach (Word word in verse.Words)
                    {
                        foreach (Letter letter in word.Letters)
                        {
                            str.Insert(0, s_numerology_system.CalculateValue(letter.Character));
                        }
                        value = Radix.Decode(str.ToString(), radix);
                        result += value; ValueSum += value;
                        Log.Append("\t" + value);

                        str.Length = 0;
                    }
                }
            }
            else
            {
                if (CalculationMode == CalculationMode.SumOfUniqueLetterValues)
                {
                    string text = verse.Text.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
                    result += CalculateValueWithLogging(text);
                }
                else
                {
                    foreach (Word word in verse.Words)
                    {
                        result += CalculateValueWithLogging(word);
                    }
                }

                value = s_numerology_system.CalculateValue(verse.Text);
                Log.Append("\t" + "\t" + "\t" + value);
                // adjust value of verse
                result += AdjustValueWithLogging(verse);
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(Verse verse, Letter from_letter, Letter to_letter)
    {
        if (verse == null) return 0L;

        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            if (s_numerology_system.LetterValue.StartsWith("Base"))
            {
                string radix_str = "";
                int pos = 4;
                while (Char.IsDigit(s_numerology_system.LetterValue[pos]))
                {
                    radix_str += s_numerology_system.LetterValue[pos];
                    pos++;
                }
                int radix;
                if (int.TryParse(radix_str, out radix))
                {
                    StringBuilder str = new StringBuilder();

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

                                if (letter_index < from_letter.NumberInVerse - 1) continue;
                                if (letter_index > to_letter.NumberInVerse - 1)
                                {
                                    done = true;
                                    break;
                                }

                                str.Insert(0, s_numerology_system.CalculateValue(letter.Character));
                            }
                        }
                        value = Radix.Decode(str.ToString(), radix);
                        result += value; ValueSum += value;
                        Log.Append("\t" + value);

                        str.Length = 0;

                        if (done) break;
                    }
                }
            }
            else
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

                            if (letter_index < from_letter.NumberInVerse - 1) continue;
                            if (letter_index > to_letter.NumberInVerse - 1)
                            {
                                done = true;
                                break;
                            }

                            result += CalculateValueWithLogging(letter);
                        }

                        if ((from_letter.NumberInVerse <= word.Letters[0].NumberInVerse)                    // if selection starts before or at first letter in word
                            &&                                                                              // AND
                            (to_letter.NumberInVerse >= word.Letters[word.Letters.Count - 1].NumberInVerse) // if selection ends   at or after  last  letter in word
                           )
                        {
                            value = s_numerology_system.CalculateValue(word.Text);
                            Log.Append("\t" + "\t" + value);
                            // adjust value of word
                            result += AdjustValueWithLogging(word);
                        }
                        else
                        {
                            Log.AppendLine("\t" + "\t" + "---");
                        }
                    }

                    if (done) break;
                }

                if ((from_letter.NumberInVerse == 1) && (to_letter.NumberInVerse == verse.LetterCount))
                {
                    value = s_numerology_system.CalculateValue(verse.Text);
                    Log.Append("\t" + "\t" + "\t" + value);
                    // adjust value of verse
                    result += AdjustValueWithLogging(verse);
                }
                else
                {
                    Log.AppendLine("\t" + "\t" + "\t" + "---");
                }
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(List<Verse> verses)
    {
        if (verses == null) return 0L;
        if (verses.Count == 0) return 0L;

        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            if (CalculationMode == CalculationMode.SumOfUniqueLetterValues)
            {
                string text = "";
                foreach (Verse verse in verses)
                {
                    text += verse.Text;
                }
                text = text.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
                result += CalculateValueWithLogging(text);
            }
            else
            {
                List<Chapter> chapters = GetCompleteChapters(verses);
                if (chapters != null)
                {
                    foreach (Verse verse in verses)
                    {
                        result += CalculateValueWithLogging(verse);

                        Chapter chapter = verse.Chapter;
                        if (chapter != null)
                        {
                            if (verse.NumberInChapter == chapter.Verses.Count)
                            {
                                if (chapters.Contains(chapter))
                                {
                                    value = s_numerology_system.CalculateValue(chapter.Text);
                                    Log.Append("\t" + "\t" + "\t" + "\t" + value);
                                    // adjust value of chapter
                                    result += AdjustValueWithLogging(chapter);
                                }
                                else
                                {
                                    Log.AppendLine("\t" + "\t" + "\t" + "\t" + "---");
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(List<Verse> verses, Letter start_letter, Letter end_letter)
    {
        if (verses == null) return 0L;
        if (verses.Count == 0) return 0L;

        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            if (verses.Count == 1)
            {
                result += CalculateValueWithLogging(verses[0], start_letter, end_letter);
            }
            else if (verses.Count == 2)
            {
                Word first_verse_end_word = verses[0].Words[verses[0].Words.Count - 1];
                if (first_verse_end_word != null)
                {
                    if (first_verse_end_word.Letters.Count > 0)
                    {
                        Letter first_verse_end_letter = first_verse_end_word.Letters[first_verse_end_word.Letters.Count - 1];
                        if (first_verse_end_letter != null)
                        {
                            result += CalculateValueWithLogging(verses[0], start_letter, first_verse_end_letter);
                        }
                    }
                }

                Word last_verse_start_word = verses[1].Words[0];
                if (last_verse_start_word != null)
                {
                    if (last_verse_start_word.Letters.Count > 0)
                    {
                        Letter last_verse_start_letter = last_verse_start_word.Letters[0];
                        if (last_verse_start_letter != null)
                        {
                            result += CalculateValueWithLogging(verses[1], last_verse_start_letter, end_letter);
                        }
                    }
                }
            }
            else //if (verses.Count > 2)
            {
                // WARNING: no null check ???
                bool first_verse_is_fully_selected = (start_letter.NumberInChapter == 1);
                bool last_verse_is_fully_selected = (end_letter.NumberInChapter == end_letter.Word.Verse.Chapter.LetterCount);
                Chapter first_chapter = start_letter.Word.Verse.Chapter;
                Chapter last_chapter = end_letter.Word.Verse.Chapter;

                // first verse
                Word first_verse_end_word = verses[0].Words[verses[0].Words.Count - 1];
                if (first_verse_end_word != null)
                {
                    if (first_verse_end_word.Letters.Count > 0)
                    {
                        Letter first_verse_end_letter = first_verse_end_word.Letters[first_verse_end_word.Letters.Count - 1];
                        if (first_verse_end_letter != null)
                        {
                            result += CalculateValueWithLogging(verses[0], start_letter, first_verse_end_letter);
                        }
                    }
                }

                // middle verses
                for (int i = 1; i < verses.Count - 1; i++)
                {
                    result += CalculateValueWithLogging(verses[i]);

                    Verse verse = verses[i];
                    if (verse != null)
                    {
                        Chapter chapter = verse.Chapter;
                        if (chapter != null)
                        {
                            if (verse.NumberInChapter == verse.Chapter.Verses.Count)    // last verse in chapter
                            {
                                if (
                                     (verse.Chapter != first_chapter)
                                     ||
                                     ((verse.Chapter == first_chapter) && first_verse_is_fully_selected)
                                   )
                                {
                                    List<Chapter> chapters = GetCompleteChapters(verses);
                                    if (chapters != null)
                                    {
                                        if (chapters.Contains(chapter))
                                        {
                                            value = s_numerology_system.CalculateValue(chapter.Text);
                                            Log.Append("\t" + "\t" + "\t" + "\t" + value);
                                            // adjust value of chapter
                                            result += AdjustValueWithLogging(chapter);
                                        }
                                    }
                                }
                                else
                                {
                                    Log.AppendLine("\t" + "\t" + "\t" + "\t" + "---");
                                }
                            }
                        }
                    }
                }

                // last verse
                Word last_verse_start_word = verses[verses.Count - 1].Words[0];
                if (last_verse_start_word != null)
                {
                    if (last_verse_start_word.Letters.Count > 0)
                    {
                        Letter last_verse_start_letter = last_verse_start_word.Letters[0];
                        if (last_verse_start_letter != null)
                        {
                            result += CalculateValueWithLogging(verses[verses.Count - 1], last_verse_start_letter, end_letter);
                            Verse verse = verses[verses.Count - 1];
                            if (verse != null)
                            {
                                if (
                                     ((last_chapter == first_chapter) && first_verse_is_fully_selected && last_verse_is_fully_selected)
                                     ||
                                     ((last_chapter != first_chapter) && (verse.Chapter == last_chapter) && last_verse_is_fully_selected)
                                   )
                                {
                                    Chapter chapter = verse.Chapter;
                                    if (chapter != null)
                                    {
                                        if (verse.NumberInChapter == verse.Chapter.Verses.Count)    // last verse in chapter
                                        {
                                            List<Chapter> chapters = GetCompleteChapters(verses);
                                            if (chapters != null)
                                            {
                                                if (chapters.Contains(chapter))
                                                {
                                                    value = s_numerology_system.CalculateValue(chapter.Text);
                                                    Log.Append("\t" + "\t" + "\t" + "\t" + value);
                                                    // adjust value of chapter
                                                    result += AdjustValueWithLogging(chapter);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Log.AppendLine("\t" + "\t" + "\t" + "\t" + "---");
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(Chapter chapter)
    {
        if (chapter == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (CalculationMode == CalculationMode.SumOfUniqueLetterValues)
            {
                string text = chapter.Text.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
                result += CalculateValueWithLogging(text);
            }
            else
            {
                result += CalculateValueWithLogging(chapter.Verses);
            }

            chapter.Value = result; // update chapter values for ChapterSortMethod.ByValue
        }
        return result;
    }
    public static long CalculateValueWithLogging(List<Chapter> chapters)
    {
        if (chapters == null) return 0L;
        if (chapters.Count == 0) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (CalculationMode == CalculationMode.SumOfUniqueLetterValues)
            {
                string text = "";
                foreach (Chapter chapter in chapters)
                {
                    text += chapter.Text;
                }
                text = text.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
                result += CalculateValueWithLogging(text);
            }
            else
            {
                foreach (Chapter chapter in chapters)
                {
                    result += CalculateValueWithLogging(chapter);
                }
            }
        }
        return result;
    }
    public static long CalculateValueWithLogging(Book book)
    {
        if (book == null) return 0L;

        long result = 0L;
        if (s_numerology_system != null)
        {
            if (CalculationMode == CalculationMode.SumOfUniqueLetterValues)
            {
                string text = book.Text.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
                result += CalculateValueWithLogging(text);
            }
            else
            {
                result += CalculateValueWithLogging(book.Chapters);
            }
        }
        return result;
    }
    // get complete words/verses/chapters
    private static List<Word> GetCompleteWords(List<Letter> letters)
    {
        if (letters == null) return null;
        if (letters.Count == 0) return null;

        List<Word> result = new List<Word>();
        for (int i = 0; i < letters.Count; i++)
        {
            bool complete = true;
            Word word = letters[i].Word;
            foreach (Letter letter in word.Letters)
            {
                if (!letters.Contains(letter))
                {
                    complete = false;
                    break;
                }
            }
            if (complete)
            {
                if (!result.Contains(word))
                {
                    result.Add(word);
                }
                i += word.Letters.Count;
            }
        }

        return result;
    }
    private static List<Word> GetCompleteWords(Sentence sentence)
    {
        if (sentence == null) return null;
        if (String.IsNullOrEmpty(sentence.Text)) return null;

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
    private static List<Verse> GetCompleteVerses(List<Word> words)
    {
        if (words == null) return null;
        if (words.Count == 0) return null;

        List<Verse> result = new List<Verse>();
        for (int i = 0; i < words.Count; i++)
        {
            bool complete = true;
            Verse verse = words[i].Verse;
            foreach (Word word in verse.Words)
            {
                if (!words.Contains(word))
                {
                    complete = false;
                    break;
                }
            }
            if (complete)
            {
                if (!result.Contains(verse))
                {
                    result.Add(verse);
                }
                i += verse.Words.Count;
            }
        }

        return result;
    }
    private static List<Verse> GetCompleteVerses(Sentence sentence)
    {
        if (sentence == null) return null;
        if (String.IsNullOrEmpty(sentence.Text)) return null;

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
    private static List<Chapter> GetCompleteChapters(List<Verse> verses)
    {
        if (verses == null) return null;
        if (verses.Count == 0) return null;

        List<Chapter> result = new List<Chapter>();
        for (int i = 0; i < verses.Count; i++)
        {
            bool complete = true;
            Chapter chapter = verses[i].Chapter;
            foreach (Verse verse in chapter.Verses)
            {
                if (!verses.Contains(verse))
                {
                    complete = false;
                    break;
                }
            }
            if (complete)
            {
                if (!result.Contains(chapter))
                {
                    result.Add(chapter);
                }
                i += chapter.Verses.Count;
            }
        }

        return result;
    }
    private static List<Chapter> GetCompleteChapters(List<Verse> verses, Letter start_letter, Letter end_letter)
    {
        if (verses == null) return null;
        if (verses.Count == 0) return null;

        List<Chapter> result = new List<Chapter>();
        List<Verse> copy_verses = new List<Verse>(verses); // make a copy so we don't change the passed verses

        if (copy_verses != null)
        {
            if (copy_verses.Count > 0)
            {
                Verse first_verse = copy_verses[0];
                if (first_verse != null)
                {
                    if (start_letter.NumberInVerse > 1)
                    {
                        copy_verses.Remove(first_verse);
                    }
                }

                if (copy_verses.Count > 0) // check again after removing a verse
                {
                    Verse last_verse = copy_verses[copy_verses.Count - 1];
                    if (last_verse != null)
                    {
                        if (end_letter.NumberInVerse < last_verse.LetterCount)
                        {
                            copy_verses.Remove(last_verse);
                        }
                    }
                }

                if (copy_verses.Count > 0) // check again after removing a verse
                {
                    foreach (Chapter chapter in s_book.Chapters)
                    {
                        bool include_chapter = true;
                        foreach (Verse v in chapter.Verses)
                        {
                            if (!copy_verses.Contains(v))
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
    // AddTo... 19 parameters
    private static long AdjustValue(Letter letter)
    {
        long result = 0L;
        if (s_numerology_system != null)
        {
            if (letter != null)
            {
                if (s_numerology_system.AddPositions && s_numerology_system.AddToLetterLNumber)
                {
                    result += letter.NumberInWord;
                }

                if (s_numerology_system.AddPositions && s_numerology_system.AddToLetterWNumber)
                {
                    result += letter.Word.NumberInVerse;
                }

                if (s_numerology_system.AddPositions && s_numerology_system.AddToLetterVNumber)
                {
                    result += letter.Word.Verse.NumberInChapter;
                }

                if (s_numerology_system.AddPositions && s_numerology_system.AddToLetterCNumber)
                {
                    result += letter.Word.Verse.Chapter.SortedNumber;
                }

                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToLetterLDistance)
                {
                    result += letter.DistanceToPrevious.dL;
                }

                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToLetterWDistance)
                {
                    result += letter.DistanceToPrevious.dW;
                }

                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToLetterVDistance)
                {
                    result += letter.DistanceToPrevious.dV;
                }

                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToLetterCDistance)
                {
                    result += letter.DistanceToPrevious.dC;
                }

                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToLetterLDistance)
                {
                    result += letter.DistanceToNext.dL;
                }

                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToLetterWDistance)
                {
                    result += letter.DistanceToNext.dW;
                }

                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToLetterVDistance)
                {
                    result += letter.DistanceToNext.dV;
                }

                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToLetterCDistance)
                {
                    result += letter.DistanceToNext.dC;
                }
            }
        }
        return result;
    }
    private static long AdjustValue(Word word)
    {
        long result = 0L;
        if (s_numerology_system != null)
        {
            if (word != null)
            {
                if (s_numerology_system.AddPositions && s_numerology_system.AddToWordWNumber)
                {
                    result += word.NumberInVerse;
                }

                if (s_numerology_system.AddPositions && s_numerology_system.AddToWordVNumber)
                {
                    result += word.Verse.NumberInChapter;
                }

                if (s_numerology_system.AddPositions && s_numerology_system.AddToWordCNumber)
                {
                    result += word.Verse.Chapter.SortedNumber;
                }

                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToWordWDistance)
                {
                    result += word.DistanceToPrevious.dW;
                }

                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToWordVDistance)
                {
                    result += word.DistanceToPrevious.dV;
                }

                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToWordCDistance)
                {
                    result += word.DistanceToPrevious.dC;
                }

                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToWordWDistance)
                {
                    result += word.DistanceToNext.dW;
                }

                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToWordVDistance)
                {
                    result += word.DistanceToNext.dV;
                }

                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToWordCDistance)
                {
                    result += word.DistanceToNext.dC;
                }
            }
        }
        return result;
    }
    private static long AdjustValue(Verse verse)
    {
        long result = 0L;
        if (s_numerology_system != null)
        {
            if (verse != null)
            {
                if (s_numerology_system.AddPositions && s_numerology_system.AddToVerseVNumber)
                {
                    result += verse.NumberInChapter;
                }

                if (s_numerology_system.AddPositions && s_numerology_system.AddToVerseCNumber)
                {
                    result += verse.Chapter.SortedNumber;
                }

                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToVerseVDistance)
                {
                    result += verse.DistanceToPrevious.dV;
                }

                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToVerseCDistance)
                {
                    result += verse.DistanceToPrevious.dC;
                }

                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToVerseVDistance)
                {
                    result += verse.DistanceToNext.dV;
                }

                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToVerseCDistance)
                {
                    result += verse.DistanceToNext.dC;
                }
            }
        }
        return result;
    }
    private static long AdjustValue(Chapter chapter)
    {
        long result = 0L;
        if (s_numerology_system != null)
        {
            if (chapter != null)
            {
                if (s_numerology_system.AddPositions && s_numerology_system.AddToChapterCNumber)
                {
                    result += chapter.SortedNumber;
                }
            }
        }
        return result;
    }
    private static long AdjustValueWithLogging(Letter letter)
    {
        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            if (letter != null)
            {
                Log.Append("\t" + "\t" + "\t");

                Log.Append("\t");
                if (s_numerology_system.AddPositions && s_numerology_system.AddToLetterLNumber)
                {
                    value = letter.NumberInWord;
                    result += value; LSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddPositions && s_numerology_system.AddToLetterWNumber)
                {
                    value = letter.Word.NumberInVerse;
                    result += value; WSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddPositions && s_numerology_system.AddToLetterVNumber)
                {
                    value = letter.Word.Verse.NumberInChapter;
                    result += value; VSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddPositions && s_numerology_system.AddToLetterCNumber)
                {
                    value = letter.Word.Verse.Chapter.SortedNumber;
                    result += value; CSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToLetterLDistance)
                {
                    value = letter.DistanceToPrevious.dL;
                    result += value; pLSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToLetterWDistance)
                {
                    value = letter.DistanceToPrevious.dW;
                    result += value; pWSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToLetterVDistance)
                {
                    value = letter.DistanceToPrevious.dV;
                    result += value; pVSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToLetterCDistance)
                {
                    value = letter.DistanceToPrevious.dC;
                    result += value; pCSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToLetterLDistance)
                {
                    value = letter.DistanceToNext.dL;
                    result += value; nLSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToLetterWDistance)
                {
                    value = letter.DistanceToNext.dW;
                    result += value; nWSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToLetterVDistance)
                {
                    value = letter.DistanceToNext.dV;
                    result += value; nVSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToLetterCDistance)
                {
                    value = letter.DistanceToNext.dC;
                    result += value; nCSum += value;
                    Log.Append(value);
                }

                Log.AppendLine();
            }
        }
        return result;
    }
    private static long AdjustValueWithLogging(Word word)
    {
        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            if (word != null)
            {
                Log.Append("\t" + "\t");

                Log.Append("\t");
                Log.Append("\t");
                if (s_numerology_system.AddPositions && s_numerology_system.AddToWordWNumber)
                {
                    value = word.NumberInVerse;
                    result += value; WSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddPositions && s_numerology_system.AddToWordVNumber)
                {
                    value = word.Verse.NumberInChapter;
                    result += value; VSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddPositions && s_numerology_system.AddToWordCNumber)
                {
                    value = word.Verse.Chapter.SortedNumber;
                    result += value; CSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                Log.Append("\t");
                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToWordWDistance)
                {
                    value = word.DistanceToPrevious.dW;
                    result += value; pWSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToWordVDistance)
                {
                    value = word.DistanceToPrevious.dV;
                    result += value; pVSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToWordCDistance)
                {
                    value = word.DistanceToPrevious.dC;
                    result += value; pCSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                Log.Append("\t");
                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToWordWDistance)
                {
                    value = word.DistanceToNext.dW;
                    result += value; nWSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToWordVDistance)
                {
                    value = word.DistanceToNext.dV;
                    result += value; nVSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToWordCDistance)
                {
                    value = word.DistanceToNext.dC;
                    result += value; nCSum += value;
                    Log.Append(value);
                }

                Log.AppendLine();
            }
        }
        return result;
    }
    private static long AdjustValueWithLogging(Verse verse)
    {
        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            if (verse != null)
            {
                Log.Append("\t");

                Log.Append("\t");
                Log.Append("\t");
                Log.Append("\t");
                if (s_numerology_system.AddPositions && s_numerology_system.AddToVerseVNumber)
                {
                    value = verse.NumberInChapter;
                    result += value; VSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddPositions && s_numerology_system.AddToVerseCNumber)
                {
                    value = verse.Chapter.SortedNumber;
                    result += value; CSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                Log.Append("\t");
                Log.Append("\t");
                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToVerseVDistance)
                {
                    value = verse.DistanceToPrevious.dV;
                    result += value; pVSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToPrevious && s_numerology_system.AddToVerseCDistance)
                {
                    value = verse.DistanceToPrevious.dC;
                    result += value; pCSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                Log.Append("\t");
                Log.Append("\t");
                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToVerseVDistance)
                {
                    value = verse.DistanceToNext.dV;
                    result += value; nVSum += value;
                    Log.Append(value);
                }

                Log.Append("\t");
                if (s_numerology_system.AddDistancesToNext && s_numerology_system.AddToVerseCDistance)
                {
                    value = verse.DistanceToNext.dC;
                    result += value; nCSum += value;
                    Log.Append(value);
                }

                Log.AppendLine();
            }
        }
        return result;
    }
    private static long AdjustValueWithLogging(Chapter chapter)
    {
        long result = 0L;
        long value = 0L;
        if (s_numerology_system != null)
        {
            if (chapter != null)
            {
                Log.Append("");

                Log.Append("\t");
                Log.Append("\t");
                Log.Append("\t");
                Log.Append("\t");
                if (s_numerology_system.AddPositions && s_numerology_system.AddToChapterCNumber)
                {
                    value = chapter.SortedNumber;
                    result += value; CSum += value;
                    Log.Append(value);
                }

                Log.AppendLine();
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
        switch (text_search_block_size)
        {
            case TextSearchBlockSize.Verse:
                {
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
            case TextSearchBlockSize.Chapter:
                {
                    List<Chapter> chapters = DoFindChapters(text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, true);
                    if (chapters != null)
                    {
                        foreach (Chapter chapter in chapters)
                        {
                            if (chapter != null)
                            {
                                verses.AddRange(chapter.Verses);
                            }
                        }

                        List<Verse> source = GetSourceVerses(SearchScope.Result, current_selection, verses, text_location_in_chapter);
                        if (language_type == LanguageType.RightToLeft)
                        {
                            return DoFindPhrases(source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1, try_emlaaei_if_nothing_found);
                        }
                        else //if (language_type == FindByTextLanguageType.LeftToRight)
                        {
                            return DoFindPhrases(translation, source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1);
                        }
                    }
                    return null;
                }
            case TextSearchBlockSize.Page:
                {
                    List<Page> pages = DoFindPages(text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, true);
                    if (pages != null)
                    {
                        foreach (Page page in pages)
                        {
                            if (page != null)
                            {
                                verses.AddRange(page.Verses);
                            }
                        }

                        List<Verse> source = GetSourceVerses(SearchScope.Result, current_selection, verses, text_location_in_chapter);
                        if (language_type == LanguageType.RightToLeft)
                        {
                            return DoFindPhrases(source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1, try_emlaaei_if_nothing_found);
                        }
                        else //if (language_type == FindByTextLanguageType.LeftToRight)
                        {
                            return DoFindPhrases(translation, source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1);
                        }
                    }
                    return null;
                }
            case TextSearchBlockSize.Station:
                {
                    List<Station> stations = DoFindStations(text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, true);
                    if (stations != null)
                    {
                        foreach (Station station in stations)
                        {
                            if (station != null)
                            {
                                verses.AddRange(station.Verses);
                            }
                        }

                        List<Verse> source = GetSourceVerses(SearchScope.Result, current_selection, verses, text_location_in_chapter);
                        if (language_type == LanguageType.RightToLeft)
                        {
                            return DoFindPhrases(source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1, try_emlaaei_if_nothing_found);
                        }
                        else //if (language_type == FindByTextLanguageType.LeftToRight)
                        {
                            return DoFindPhrases(translation, source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1);
                        }
                    }
                    return null;
                }
            case TextSearchBlockSize.Part:
                {
                    List<Part> parts = DoFindParts(text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, true);
                    if (parts != null)
                    {
                        foreach (Part part in parts)
                        {
                            if (part != null)
                            {
                                verses.AddRange(part.Verses);
                            }
                        }

                        List<Verse> source = GetSourceVerses(SearchScope.Result, current_selection, verses, text_location_in_chapter);
                        if (language_type == LanguageType.RightToLeft)
                        {
                            return DoFindPhrases(source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1, try_emlaaei_if_nothing_found);
                        }
                        else //if (language_type == FindByTextLanguageType.LeftToRight)
                        {
                            return DoFindPhrases(translation, source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1);
                        }
                    }
                    return null;
                }
            case TextSearchBlockSize.Group:
                {
                    List<Model.Group> groups = DoFindGroups(text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, true);
                    if (groups != null)
                    {
                        foreach (Model.Group group in groups)
                        {
                            if (group != null)
                            {
                                verses.AddRange(group.Verses);
                            }
                        }

                        List<Verse> source = GetSourceVerses(SearchScope.Result, current_selection, verses, text_location_in_chapter);
                        if (language_type == LanguageType.RightToLeft)
                        {
                            return DoFindPhrases(source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1, try_emlaaei_if_nothing_found);
                        }
                        else //if (language_type == FindByTextLanguageType.LeftToRight)
                        {
                            return DoFindPhrases(translation, source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1);
                        }
                    }
                    return null;
                }
            case TextSearchBlockSize.Half:
                {
                    List<Half> halfs = DoFindHalfs(text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, true);
                    if (halfs != null)
                    {
                        foreach (Half half in halfs)
                        {
                            if (half != null)
                            {
                                verses.AddRange(half.Verses);
                            }
                        }

                        List<Verse> source = GetSourceVerses(SearchScope.Result, current_selection, verses, text_location_in_chapter);
                        if (language_type == LanguageType.RightToLeft)
                        {
                            return DoFindPhrases(source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1, try_emlaaei_if_nothing_found);
                        }
                        else //if (language_type == FindByTextLanguageType.LeftToRight)
                        {
                            return DoFindPhrases(translation, source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1);
                        }
                    }
                    return null;
                }
            case TextSearchBlockSize.Quarter:
                {
                    List<Quarter> quarters = DoFindQuarters(text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, true);
                    if (quarters != null)
                    {
                        foreach (Quarter quarter in quarters)
                        {
                            if (quarter != null)
                            {
                                verses.AddRange(quarter.Verses);
                            }
                        }

                        List<Verse> source = GetSourceVerses(SearchScope.Result, current_selection, verses, text_location_in_chapter);
                        if (language_type == LanguageType.RightToLeft)
                        {
                            return DoFindPhrases(source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1, try_emlaaei_if_nothing_found);
                        }
                        else //if (language_type == FindByTextLanguageType.LeftToRight)
                        {
                            return DoFindPhrases(translation, source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1);
                        }
                    }
                    return null;
                }
            case TextSearchBlockSize.Bowing:
                {
                    List<Bowing> bowings = DoFindBowings(text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder, true);
                    if (bowings != null)
                    {
                        foreach (Bowing bowing in bowings)
                        {
                            if (bowing != null)
                            {
                                verses.AddRange(bowing.Verses);
                            }
                        }

                        List<Verse> source = GetSourceVerses(SearchScope.Result, current_selection, verses, text_location_in_chapter);
                        if (language_type == LanguageType.RightToLeft)
                        {
                            return DoFindPhrases(source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1, try_emlaaei_if_nothing_found);
                        }
                        else //if (language_type == FindByTextLanguageType.LeftToRight)
                        {
                            return DoFindPhrases(translation, source, current_selection, previous_verses, text, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, -1, NumberType.None, ComparisonOperator.Equal, -1);
                        }
                    }
                    return null;
                }
            default:
                return null;
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
                                    MatchCollection matches = Regex.Matches(verse_text, pattern);
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
                                            //MatchCollection matches = Regex.Matches(verse_text, pattern);
                                            MatchCollection matches = Regex.Matches(verse_text, pattern);
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

                        // if nothing found
                        if ((multiplicity != 0) && (result.Count == 0))
                        {
                            //  search in emlaaei
                            if (try_emlaaei_if_nothing_found)
                            {
                                if (s_numerology_system != null)
                                {
                                    text = text.SimplifyTo(s_numerology_system.TextMode);
                                    if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                                    {
                                        string pattern = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                                        if (!String.IsNullOrEmpty(pattern))
                                        {
                                            if ((source != null) && (source.Count > 0))
                                            {
                                                foreach (Verse verse in source)
                                                {
                                                    string emlaaei_text = verse.Translations[DEFAULT_EMLAAEI_TEXT];
                                                    emlaaei_text = emlaaei_text.SimplifyTo(s_numerology_system.TextMode);
                                                    emlaaei_text = emlaaei_text.Trim();
                                                    while (emlaaei_text.Contains("  "))
                                                    {
                                                        emlaaei_text = emlaaei_text.Replace("  ", " ");
                                                    }

                                                    MatchCollection matches = Regex.Matches(emlaaei_text, pattern);
                                                    if (multiplicity == -1) // without multiplicity
                                                    {
                                                        if (matches.Count > 0)
                                                        {
                                                            //result.AddRange(BuildPhrases(verse, matches));
                                                            result.Add(new Phrase(verse, 0, ""));
                                                        }
                                                    }
                                                    else // with multiplicity
                                                    {
                                                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                                        {
                                                            //result.AddRange(BuildPhrases(verse, matches));
                                                            result.Add(new Phrase(verse, 0, ""));
                                                        }
                                                    }
                                                } // end for
                                            }
                                        }
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
    private static List<Chapter> DoFindChapters(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder, bool try_emlaaei_if_nothing_found)
    {
        List<Chapter> result = new List<Chapter>();

        if (!String.IsNullOrEmpty(text))
        {
            text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

            try
            {
                if (s_book != null)
                {
                    if (with_diacritics)
                    {
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                            if (!String.IsNullOrEmpty(text))
                            {
                                foreach (Chapter chapter in s_book.Chapters)
                                {
                                    string chapter_text = chapter.Text.Trim();
                                    MatchCollection matches = Regex.Matches(chapter_text, text);
                                    if (multiplicity == -1) // without multiplicity
                                    {
                                        if (matches.Count > 0)
                                        {
                                            if (!result.Contains(chapter))
                                            {
                                                result.Add(chapter);
                                            }
                                        }
                                    }
                                    else // with multiplicity
                                    {
                                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                        {
                                            if (!result.Contains(chapter))
                                            {
                                                result.Add(chapter);
                                            }
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
                            if (!String.IsNullOrEmpty(text))
                            {
                                text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                                if (!String.IsNullOrEmpty(text))
                                {
                                    foreach (Chapter chapter in s_book.Chapters)
                                    {
                                        string chapter_text = chapter.Text.Trim();
                                        chapter_text = chapter_text.SimplifyTo(s_numerology_system.TextMode);
                                        chapter_text = chapter_text.Trim();
                                        MatchCollection matches = Regex.Matches(chapter_text, text);
                                        if (multiplicity == -1) // without multiplicity
                                        {
                                            if (matches.Count > 0)
                                            {
                                                if (!result.Contains(chapter))
                                                {
                                                    result.Add(chapter);
                                                }
                                            }
                                        }
                                        else // with multiplicity
                                        {
                                            if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                            {
                                                if (!result.Contains(chapter))
                                                {
                                                    result.Add(chapter);
                                                }
                                            }
                                        }
                                    } // end for
                                }
                            }
                        }
                    }

                    //// if nothing found
                    //if ((multiplicity != 0) && (result.Count == 0))
                    //{
                    //    //  search in emlaaei
                    //    if (try_emlaaei_if_nothing_found)
                    //    {
                    //        if (s_numerology_system != null)
                    //        {
                    //            text = text.SimplifyTo(s_numerology_system.TextMode);
                    //            if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                    //            {
                    //                foreach (Chapter chapter in s_book.Chapters)
                    //                {
                    //                    string emlaaei_text = "";
                    //                    foreach (Verse verse in chapter.Verses)
                    //                    {
                    //                        emlaaei_text += verse.Translations[DEFAULT_EMLAAEI_TEXT] + "\r\n";
                    //                    }
                    //                    emlaaei_text = emlaaei_text.SimplifyTo(s_numerology_system.TextMode);
                    //                    emlaaei_text = emlaaei_text.Trim();
                    //                    while (emlaaei_text.Contains("  "))
                    //                    {
                    //                        emlaaei_text = emlaaei_text.Replace("  ", " ");
                    //                    }

                    //                    MatchCollection matches = Regex.Matches(emlaaei_text, text);
                    //                    if (multiplicity == -1) // without multiplicity
                    //                    {
                    //                        if (matches.Count > 0)
                    //                        {
                    //                            if (!result.Contains(chapter))
                    //                            {
                    //                                result.Add(chapter);
                    //                            }
                    //                        }
                    //                    }
                    //                    else // with multiplicity
                    //                    {
                    //                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                    //                        {
                    //                            if (!result.Contains(chapter))
                    //                            {
                    //                                result.Add(chapter);
                    //                            }
                    //                        }
                    //                    }
                    //                } // end for
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Page> DoFindPages(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder, bool try_emlaaei_if_nothing_found)
    {
        List<Page> result = new List<Page>();

        if (!String.IsNullOrEmpty(text))
        {
            text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

            try
            {
                if (s_book != null)
                {
                    if (with_diacritics)
                    {
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                            if (!String.IsNullOrEmpty(text))
                            {
                                foreach (Page page in s_book.Pages)
                                {
                                    string page_text = page.Text.Trim();
                                    MatchCollection matches = Regex.Matches(page_text, text);
                                    if (multiplicity == -1) // without multiplicity
                                    {
                                        if (matches.Count > 0)
                                        {
                                            if (!result.Contains(page))
                                            {
                                                result.Add(page);
                                            }
                                        }
                                    }
                                    else // with multiplicity
                                    {
                                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                        {
                                            if (!result.Contains(page))
                                            {
                                                result.Add(page);
                                            }
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
                            if (!String.IsNullOrEmpty(text))
                            {
                                text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                                if (!String.IsNullOrEmpty(text))
                                {
                                    foreach (Page page in s_book.Pages)
                                    {
                                        string page_text = page.Text.Trim();
                                        page_text = page_text.SimplifyTo(s_numerology_system.TextMode);
                                        page_text = page_text.Trim();
                                        MatchCollection matches = Regex.Matches(page_text, text);
                                        if (multiplicity == -1) // without multiplicity
                                        {
                                            if (matches.Count > 0)
                                            {
                                                if (!result.Contains(page))
                                                {
                                                    result.Add(page);
                                                }
                                            }
                                        }
                                        else // with multiplicity
                                        {
                                            if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                            {
                                                if (!result.Contains(page))
                                                {
                                                    result.Add(page);
                                                }
                                            }
                                        }
                                    } // end for
                                }
                            }
                        }
                    }

                    //// if nothing found
                    //if ((multiplicity != 0) && (result.Count == 0))
                    //{
                    //    //  search in emlaaei
                    //    if (try_emlaaei_if_nothing_found)
                    //    {
                    //        if (s_numerology_system != null)
                    //        {
                    //            text = text.SimplifyTo(s_numerology_system.TextMode);
                    //            if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                    //            {
                    //                foreach (Page page in s_book.Pages)
                    //                {
                    //                    string emlaaei_text = "";
                    //                    foreach (Verse verse in page.Verses)
                    //                    {
                    //                        emlaaei_text += verse.Translations[DEFAULT_EMLAAEI_TEXT] + "\r\n";
                    //                    }
                    //                    emlaaei_text = emlaaei_text.SimplifyTo(s_numerology_system.TextMode);
                    //                    emlaaei_text = emlaaei_text.Trim();
                    //                    while (emlaaei_text.Contains("  "))
                    //                    {
                    //                        emlaaei_text = emlaaei_text.Replace("  ", " ");
                    //                    }

                    //                    MatchCollection matches = Regex.Matches(emlaaei_text, text);
                    //                    if (multiplicity == -1) // without multiplicity
                    //                    {
                    //                        if (matches.Count > 0)
                    //                        {
                    //                            if (!result.Contains(page))
                    //                            {
                    //                                result.Add(page);
                    //                            }
                    //                        }
                    //                    }
                    //                    else // with multiplicity
                    //                    {
                    //                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                    //                        {
                    //                            if (!result.Contains(page))
                    //                            {
                    //                                result.Add(page);
                    //                            }
                    //                        }
                    //                    }
                    //                } // end for
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Station> DoFindStations(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder, bool try_emlaaei_if_nothing_found)
    {
        List<Station> result = new List<Station>();

        if (!String.IsNullOrEmpty(text))
        {
            text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

            try
            {
                if (s_book != null)
                {
                    if (with_diacritics)
                    {
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                            if (!String.IsNullOrEmpty(text))
                            {
                                foreach (Station station in s_book.Stations)
                                {
                                    string station_text = station.Text.Trim();
                                    MatchCollection matches = Regex.Matches(station_text, text);
                                    if (multiplicity == -1) // without multiplicity
                                    {
                                        if (matches.Count > 0)
                                        {
                                            if (!result.Contains(station))
                                            {
                                                result.Add(station);
                                            }
                                        }
                                    }
                                    else // with multiplicity
                                    {
                                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                        {
                                            if (!result.Contains(station))
                                            {
                                                result.Add(station);
                                            }
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
                            if (!String.IsNullOrEmpty(text))
                            {
                                text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                                if (!String.IsNullOrEmpty(text))
                                {
                                    foreach (Station station in s_book.Stations)
                                    {
                                        string station_text = station.Text.Trim();
                                        station_text = station_text.SimplifyTo(s_numerology_system.TextMode);
                                        station_text = station_text.Trim();
                                        MatchCollection matches = Regex.Matches(station_text, text);
                                        if (multiplicity == -1) // without multiplicity
                                        {
                                            if (matches.Count > 0)
                                            {
                                                if (!result.Contains(station))
                                                {
                                                    result.Add(station);
                                                }
                                            }
                                        }
                                        else // with multiplicity
                                        {
                                            if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                            {
                                                if (!result.Contains(station))
                                                {
                                                    result.Add(station);
                                                }
                                            }
                                        }
                                    } // end for
                                }
                            }
                        }
                    }

                    //// if nothing found
                    //if ((multiplicity != 0) && (result.Count == 0))
                    //{
                    //    //  search in emlaaei
                    //    if (try_emlaaei_if_nothing_found)
                    //    {
                    //        if (s_numerology_system != null)
                    //        {
                    //            text = text.SimplifyTo(s_numerology_system.TextMode);
                    //            if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                    //            {
                    //                foreach (Station station in s_book.Stations)
                    //                {
                    //                    string emlaaei_text = "";
                    //                    foreach (Verse verse in station.Verses)
                    //                    {
                    //                        emlaaei_text += verse.Translations[DEFAULT_EMLAAEI_TEXT] + "\r\n";
                    //                    }
                    //                    emlaaei_text = emlaaei_text.SimplifyTo(s_numerology_system.TextMode);
                    //                    emlaaei_text = emlaaei_text.Trim();
                    //                    while (emlaaei_text.Contains("  "))
                    //                    {
                    //                        emlaaei_text = emlaaei_text.Replace("  ", " ");
                    //                    }

                    //                    MatchCollection matches = Regex.Matches(emlaaei_text, text);
                    //                    if (multiplicity == -1) // without multiplicity
                    //                    {
                    //                        if (matches.Count > 0)
                    //                        {
                    //                            if (!result.Contains(station))
                    //                            {
                    //                                result.Add(station);
                    //                            }
                    //                        }
                    //                    }
                    //                    else // with multiplicity
                    //                    {
                    //                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                    //                        {
                    //                            if (!result.Contains(station))
                    //                            {
                    //                                result.Add(station);
                    //                            }
                    //                        }
                    //                    }
                    //                } // end for
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Part> DoFindParts(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder, bool try_emlaaei_if_nothing_found)
    {
        List<Part> result = new List<Part>();

        if (!String.IsNullOrEmpty(text))
        {
            text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

            try
            {
                if (s_book != null)
                {
                    if (with_diacritics)
                    {
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                            if (!String.IsNullOrEmpty(text))
                            {
                                foreach (Part part in s_book.Parts)
                                {
                                    string part_text = part.Text.Trim();
                                    MatchCollection matches = Regex.Matches(part_text, text);
                                    if (multiplicity == -1) // without multiplicity
                                    {
                                        if (matches.Count > 0)
                                        {
                                            if (!result.Contains(part))
                                            {
                                                result.Add(part);
                                            }
                                        }
                                    }
                                    else // with multiplicity
                                    {
                                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                        {
                                            if (!result.Contains(part))
                                            {
                                                result.Add(part);
                                            }
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
                            if (!String.IsNullOrEmpty(text))
                            {
                                text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                                if (!String.IsNullOrEmpty(text))
                                {
                                    foreach (Part part in s_book.Parts)
                                    {
                                        string part_text = part.Text.Trim();
                                        part_text = part_text.SimplifyTo(s_numerology_system.TextMode);
                                        part_text = part_text.Trim();
                                        MatchCollection matches = Regex.Matches(part_text, text);
                                        if (multiplicity == -1) // without multiplicity
                                        {
                                            if (matches.Count > 0)
                                            {
                                                if (!result.Contains(part))
                                                {
                                                    result.Add(part);
                                                }
                                            }
                                        }
                                        else // with multiplicity
                                        {
                                            if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                            {
                                                if (!result.Contains(part))
                                                {
                                                    result.Add(part);
                                                }
                                            }
                                        }
                                    } // end for
                                }
                            }
                        }
                    }

                    //// if nothing found
                    //if ((multiplicity != 0) && (result.Count == 0))
                    //{
                    //    //  search in emlaaei
                    //    if (try_emlaaei_if_nothing_found)
                    //    {
                    //        if (s_numerology_system != null)
                    //        {
                    //            text = text.SimplifyTo(s_numerology_system.TextMode);
                    //            if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                    //            {
                    //                foreach (Part part in s_book.Parts)
                    //                {
                    //                    string emlaaei_text = "";
                    //                    foreach (Verse verse in part.Verses)
                    //                    {
                    //                        emlaaei_text += verse.Translations[DEFAULT_EMLAAEI_TEXT] + "\r\n";
                    //                    }
                    //                    emlaaei_text = emlaaei_text.SimplifyTo(s_numerology_system.TextMode);
                    //                    emlaaei_text = emlaaei_text.Trim();
                    //                    while (emlaaei_text.Contains("  "))
                    //                    {
                    //                        emlaaei_text = emlaaei_text.Replace("  ", " ");
                    //                    }

                    //                    MatchCollection matches = Regex.Matches(emlaaei_text, text);
                    //                    if (multiplicity == -1) // without multiplicity
                    //                    {
                    //                        if (matches.Count > 0)
                    //                        {
                    //                            if (!result.Contains(part))
                    //                            {
                    //                                result.Add(part);
                    //                            }
                    //                        }
                    //                    }
                    //                    else // with multiplicity
                    //                    {
                    //                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                    //                        {
                    //                            if (!result.Contains(part))
                    //                            {
                    //                                result.Add(part);
                    //                            }
                    //                        }
                    //                    }
                    //                } // end for
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Model.Group> DoFindGroups(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder, bool try_emlaaei_if_nothing_found)
    {
        List<Model.Group> result = new List<Model.Group>();

        if (!String.IsNullOrEmpty(text))
        {
            text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

            try
            {
                if (s_book != null)
                {
                    if (with_diacritics)
                    {
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                            if (!String.IsNullOrEmpty(text))
                            {
                                foreach (Model.Group group in s_book.Groups)
                                {
                                    string group_text = group.Text.Trim();
                                    MatchCollection matches = Regex.Matches(group_text, text);
                                    if (multiplicity == -1) // without multiplicity
                                    {
                                        if (matches.Count > 0)
                                        {
                                            if (!result.Contains(group))
                                            {
                                                result.Add(group);
                                            }
                                        }
                                    }
                                    else // with multiplicity
                                    {
                                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                        {
                                            if (!result.Contains(group))
                                            {
                                                result.Add(group);
                                            }
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
                            if (!String.IsNullOrEmpty(text))
                            {
                                text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                                if (!String.IsNullOrEmpty(text))
                                {
                                    foreach (Model.Group group in s_book.Groups)
                                    {
                                        string group_text = group.Text.Trim();
                                        group_text = group_text.SimplifyTo(s_numerology_system.TextMode);
                                        group_text = group_text.Trim();
                                        MatchCollection matches = Regex.Matches(group_text, text);
                                        if (multiplicity == -1) // without multiplicity
                                        {
                                            if (matches.Count > 0)
                                            {
                                                if (!result.Contains(group))
                                                {
                                                    result.Add(group);
                                                }
                                            }
                                        }
                                        else // with multiplicity
                                        {
                                            if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                            {
                                                if (!result.Contains(group))
                                                {
                                                    result.Add(group);
                                                }
                                            }
                                        }
                                    } // end for
                                }
                            }
                        }
                    }

                    //// if nothing found
                    //if ((multiplicity != 0) && (result.Count == 0))
                    //{
                    //    //  search in emlaaei
                    //    if (try_emlaaei_if_nothing_found)
                    //    {
                    //        if (s_numerology_system != null)
                    //        {
                    //            text = text.SimplifyTo(s_numerology_system.TextMode);
                    //            if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                    //            {
                    //                foreach (Model.Group group in s_book.Groups)
                    //                {
                    //                    string emlaaei_text = "";
                    //                    foreach (Verse verse in group.Verses)
                    //                    {
                    //                        emlaaei_text += verse.Translations[DEFAULT_EMLAAEI_TEXT] + "\r\n";
                    //                    }
                    //                    emlaaei_text = emlaaei_text.SimplifyTo(s_numerology_system.TextMode);
                    //                    emlaaei_text = emlaaei_text.Trim();
                    //                    while (emlaaei_text.Contains("  "))
                    //                    {
                    //                        emlaaei_text = emlaaei_text.Replace("  ", " ");
                    //                    }

                    //                    MatchCollection matches = Regex.Matches(emlaaei_text, text);
                    //                    if (multiplicity == -1) // without multiplicity
                    //                    {
                    //                        if (matches.Count > 0)
                    //                        {
                    //                            if (!result.Contains(group))
                    //                            {
                    //                                result.Add(group);
                    //                            }
                    //                        }
                    //                    }
                    //                    else // with multiplicity
                    //                    {
                    //                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                    //                        {
                    //                            if (!result.Contains(group))
                    //                            {
                    //                                result.Add(group);
                    //                            }
                    //                        }
                    //                    }
                    //                } // end for
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Half> DoFindHalfs(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder, bool try_emlaaei_if_nothing_found)
    {
        List<Half> result = new List<Half>();

        if (!String.IsNullOrEmpty(text))
        {
            text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

            try
            {
                if (s_book != null)
                {
                    if (with_diacritics)
                    {
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                            if (!String.IsNullOrEmpty(text))
                            {
                                foreach (Half half in s_book.Halfs)
                                {
                                    string half_text = half.Text.Trim();
                                    MatchCollection matches = Regex.Matches(half_text, text);
                                    if (multiplicity == -1) // without multiplicity
                                    {
                                        if (matches.Count > 0)
                                        {
                                            if (!result.Contains(half))
                                            {
                                                result.Add(half);
                                            }
                                        }
                                    }
                                    else // with multiplicity
                                    {
                                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                        {
                                            if (!result.Contains(half))
                                            {
                                                result.Add(half);
                                            }
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
                            if (!String.IsNullOrEmpty(text))
                            {
                                text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                                if (!String.IsNullOrEmpty(text))
                                {
                                    foreach (Half half in s_book.Halfs)
                                    {
                                        string half_text = half.Text.Trim();
                                        half_text = half_text.SimplifyTo(s_numerology_system.TextMode);
                                        half_text = half_text.Trim();
                                        MatchCollection matches = Regex.Matches(half_text, text);
                                        if (multiplicity == -1) // without multiplicity
                                        {
                                            if (matches.Count > 0)
                                            {
                                                if (!result.Contains(half))
                                                {
                                                    result.Add(half);
                                                }
                                            }
                                        }
                                        else // with multiplicity
                                        {
                                            if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                            {
                                                if (!result.Contains(half))
                                                {
                                                    result.Add(half);
                                                }
                                            }
                                        }
                                    } // end for
                                }
                            }
                        }
                    }

                    //// if nothing found
                    //if ((multiplicity != 0) && (result.Count == 0))
                    //{
                    //    //  search in emlaaei
                    //    if (try_emlaaei_if_nothing_found)
                    //    {
                    //        if (s_numerology_system != null)
                    //        {
                    //            text = text.SimplifyTo(s_numerology_system.TextMode);
                    //            if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                    //            {
                    //                foreach (Half half in s_book.Halfs)
                    //                {
                    //                    string emlaaei_text = "";
                    //                    foreach (Verse verse in half.Verses)
                    //                    {
                    //                        emlaaei_text += verse.Translations[DEFAULT_EMLAAEI_TEXT] + "\r\n";
                    //                    }
                    //                    emlaaei_text = emlaaei_text.SimplifyTo(s_numerology_system.TextMode);
                    //                    emlaaei_text = emlaaei_text.Trim();
                    //                    while (emlaaei_text.Contains("  "))
                    //                    {
                    //                        emlaaei_text = emlaaei_text.Replace("  ", " ");
                    //                    }

                    //                    MatchCollection matches = Regex.Matches(emlaaei_text, text);
                    //                    if (multiplicity == -1) // without multiplicity
                    //                    {
                    //                        if (matches.Count > 0)
                    //                        {
                    //                            if (!result.Contains(half))
                    //                            {
                    //                                result.Add(half);
                    //                            }
                    //                        }
                    //                    }
                    //                    else // with multiplicity
                    //                    {
                    //                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                    //                        {
                    //                            if (!result.Contains(half))
                    //                            {
                    //                                result.Add(half);
                    //                            }
                    //                        }
                    //                    }
                    //                } // end for
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Quarter> DoFindQuarters(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder, bool try_emlaaei_if_nothing_found)
    {
        List<Quarter> result = new List<Quarter>();

        if (!String.IsNullOrEmpty(text))
        {
            text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

            try
            {
                if (s_book != null)
                {
                    if (with_diacritics)
                    {
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                            if (!String.IsNullOrEmpty(text))
                            {
                                foreach (Quarter quarter in s_book.Quarters)
                                {
                                    string quarter_text = quarter.Text.Trim();
                                    MatchCollection matches = Regex.Matches(quarter_text, text);
                                    if (multiplicity == -1) // without multiplicity
                                    {
                                        if (matches.Count > 0)
                                        {
                                            if (!result.Contains(quarter))
                                            {
                                                result.Add(quarter);
                                            }
                                        }
                                    }
                                    else // with multiplicity
                                    {
                                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                        {
                                            if (!result.Contains(quarter))
                                            {
                                                result.Add(quarter);
                                            }
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
                            if (!String.IsNullOrEmpty(text))
                            {
                                text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                                if (!String.IsNullOrEmpty(text))
                                {
                                    foreach (Quarter quarter in s_book.Quarters)
                                    {
                                        string quarter_text = quarter.Text.Trim();
                                        quarter_text = quarter_text.SimplifyTo(s_numerology_system.TextMode);
                                        quarter_text = quarter_text.Trim();
                                        MatchCollection matches = Regex.Matches(quarter_text, text);
                                        if (multiplicity == -1) // without multiplicity
                                        {
                                            if (matches.Count > 0)
                                            {
                                                if (!result.Contains(quarter))
                                                {
                                                    result.Add(quarter);
                                                }
                                            }
                                        }
                                        else // with multiplicity
                                        {
                                            if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                            {
                                                if (!result.Contains(quarter))
                                                {
                                                    result.Add(quarter);
                                                }
                                            }
                                        }
                                    } // end for
                                }
                            }
                        }
                    }

                    //// if nothing found
                    //if ((multiplicity != 0) && (result.Count == 0))
                    //{
                    //    //  search in emlaaei
                    //    if (try_emlaaei_if_nothing_found)
                    //    {
                    //        if (s_numerology_system != null)
                    //        {
                    //            text = text.SimplifyTo(s_numerology_system.TextMode);
                    //            if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                    //            {
                    //                foreach (Quarter quarter in s_book.Quarters)
                    //                {
                    //                    string emlaaei_text = "";
                    //                    foreach (Verse verse in quarter.Verses)
                    //                    {
                    //                        emlaaei_text += verse.Translations[DEFAULT_EMLAAEI_TEXT] + "\r\n";
                    //                    }
                    //                    emlaaei_text = emlaaei_text.SimplifyTo(s_numerology_system.TextMode);
                    //                    emlaaei_text = emlaaei_text.Trim();
                    //                    while (emlaaei_text.Contains("  "))
                    //                    {
                    //                        emlaaei_text = emlaaei_text.Replace("  ", " ");
                    //                    }

                    //                    MatchCollection matches = Regex.Matches(emlaaei_text, text);
                    //                    if (multiplicity == -1) // without multiplicity
                    //                    {
                    //                        if (matches.Count > 0)
                    //                        {
                    //                            if (!result.Contains(quarter))
                    //                            {
                    //                                result.Add(quarter);
                    //                            }
                    //                        }
                    //                    }
                    //                    else // with multiplicity
                    //                    {
                    //                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                    //                        {
                    //                            if (!result.Contains(quarter))
                    //                            {
                    //                                result.Add(quarter);
                    //                            }
                    //                        }
                    //                    }
                    //                } // end for
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Bowing> DoFindBowings(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder, bool try_emlaaei_if_nothing_found)
    {
        List<Bowing> result = new List<Bowing>();

        if (!String.IsNullOrEmpty(text))
        {
            text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

            try
            {
                if (s_book != null)
                {
                    if (with_diacritics)
                    {
                        if (!String.IsNullOrEmpty(text))
                        {
                            text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                            if (!String.IsNullOrEmpty(text))
                            {
                                foreach (Bowing bowing in s_book.Bowings)
                                {
                                    string bowing_text = bowing.Text.Trim();
                                    MatchCollection matches = Regex.Matches(bowing_text, text);
                                    if (multiplicity == -1) // without multiplicity
                                    {
                                        if (matches.Count > 0)
                                        {
                                            if (!result.Contains(bowing))
                                            {
                                                result.Add(bowing);
                                            }
                                        }
                                    }
                                    else // with multiplicity
                                    {
                                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                        {
                                            if (!result.Contains(bowing))
                                            {
                                                result.Add(bowing);
                                            }
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
                            if (!String.IsNullOrEmpty(text))
                            {
                                text = BuildPattern(text, text_location_in_verse, text_location_in_word, text_wordness);
                                if (!String.IsNullOrEmpty(text))
                                {
                                    foreach (Bowing bowing in s_book.Bowings)
                                    {
                                        string bowing_text = bowing.Text.Trim();
                                        bowing_text = bowing_text.SimplifyTo(s_numerology_system.TextMode);
                                        bowing_text = bowing_text.Trim();
                                        MatchCollection matches = Regex.Matches(bowing_text, text);
                                        if (multiplicity == -1) // without multiplicity
                                        {
                                            if (matches.Count > 0)
                                            {
                                                if (!result.Contains(bowing))
                                                {
                                                    result.Add(bowing);
                                                }
                                            }
                                        }
                                        else // with multiplicity
                                        {
                                            if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                                            {
                                                if (!result.Contains(bowing))
                                                {
                                                    result.Add(bowing);
                                                }
                                            }
                                        }
                                    } // end for
                                }
                            }
                        }
                    }

                    //// if nothing found
                    //if ((multiplicity != 0) && (result.Count == 0))
                    //{
                    //    //  search in emlaaei
                    //    if (try_emlaaei_if_nothing_found)
                    //    {
                    //        if (s_numerology_system != null)
                    //        {
                    //            text = text.SimplifyTo(s_numerology_system.TextMode);
                    //            if (!String.IsNullOrEmpty(text)) // re-test in case text was just harakaat which is simplifed to nothing
                    //            {
                    //                foreach (Bowing bowing in s_book.Bowings)
                    //                {
                    //                    string emlaaei_text = "";
                    //                    foreach (Verse verse in bowing.Verses)
                    //                    {
                    //                        emlaaei_text += verse.Translations[DEFAULT_EMLAAEI_TEXT] + "\r\n";
                    //                    }
                    //                    emlaaei_text = emlaaei_text.SimplifyTo(s_numerology_system.TextMode);
                    //                    emlaaei_text = emlaaei_text.Trim();
                    //                    while (emlaaei_text.Contains("  "))
                    //                    {
                    //                        emlaaei_text = emlaaei_text.Replace("  ", " ");
                    //                    }

                    //                    MatchCollection matches = Regex.Matches(emlaaei_text, text);
                    //                    if (multiplicity == -1) // without multiplicity
                    //                    {
                    //                        if (matches.Count > 0)
                    //                        {
                    //                            if (!result.Contains(bowing))
                    //                            {
                    //                                result.Add(bowing);
                    //                            }
                    //                        }
                    //                    }
                    //                    else // with multiplicity
                    //                    {
                    //                        if (Compare(matches.Count, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder))
                    //                        {
                    //                            if (!result.Contains(bowing))
                    //                            {
                    //                                result.Add(bowing);
                    //                            }
                    //                        }
                    //                    }
                    //                } // end for
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    //private static string BuildPattern(string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness)
    //{
    //    if (String.IsNullOrEmpty(text)) return text;
    //    text = Regex.Replace(text, @"\s+", " "); // remove double space or higher if any

    //    // search for Quran markers, stopmarks, numbers, etc.
    //    if (text.Length == 1)
    //    {
    //        if (!Constants.ARABIC_LETTERS.Contains(text[0]))
    //        {
    //            return text;
    //        }
    //    }

    //    /*
    //    =====================================================================
    //    Regular Expressions (RegEx)
    //    =====================================================================
    //    Best Reference: http://www.regular-expressions.info/
    //    =====================================================================
    //    Matches	Characters 
    //    x	character x 
    //    \\	backslash character 
    //    \0n	character with octal value 0n (0 <= n <= 7) 
    //    \0nn	character with octal value 0nn (0 <= n <= 7) 
    //    \0mnn	character with octal value 0mnn (0 <= m <= 3, 0 <= n <= 7) 
    //    \xhh	character with hexadecimal value 0xhh 
    //    \uhhhh	character with hexadecimal value 0xhhhh 
    //    \t	tab character ('\u0009') 
    //    \n	newline (line feed) character ('\u000A') 
    //    \r	carriage-return character ('\u000D') 
    //    \f	form-feed character ('\u000C') 
    //    \a	alert (bell) character ('\u0007') 
    //    \e	escape character ('\u001B') 
    //    \cx	control character corresponding to x 

    //    Character Classes 
    //    [abc]		    a, b, or c				                    (simple class) 
    //    [^abc]		    any character except a, b, or c		        (negation) 
    //    [a-zA-Z]	    a through z or A through Z, inclusive	    (range) 
    //    [a-d[m-p]]	    a through d, or m through p: [a-dm-p]	    (union) 
    //    [a-z&&[def]]	d, e, or f				                    (intersection) 
    //    [a-z&&[^bc]]	a through z, except for b and c: [ad-z]	    (subtraction) 
    //    [a-z&&[^m-p]]	a through z, and not m through p: [a-lq-z]  (subtraction) 

    //    Predefined 
    //    .	any character (inc line terminators) except newline 
    //    \d	digit				            [0-9] 
    //    \D	non-digit			            [^0-9] 
    //    \s	whitespace character		    [ \t\n\x0B\f\r] 
    //    \S	non-whitespace character	    [^\s] 
    //    \w	word character (alphanumeric)	[a-zA-Z_0-9] 
    //    \W	non-word character		        [^\w] 

    //    Boundary Matchers 
    //    ^	beginning of a line	(in Multiline)
    //    $	end of a line  		(in Multiline)
    //    \b	word boundary, including line start and line end
    //    \B	non-word boundary 
    //    \A	beginning of the input 
    //    \G	end of the previous match 
    //    \Z	end of the input but for the final terminator, if any 
    //    \z	end of the input

    //    Greedy quantifiers 
    //    X?	X, once or not at all 
    //    X*	X, zero or more times 
    //    X+	X, one or more times 
    //    X{n}	X, exactly n times 
    //    X{n,}	X, at least n times 
    //    X{n,m}	X, at least n but not more than m times 

    //    Reluctant quantifiers 
    //    X??	X, once or not at all 
    //    X*?	X, zero or more times 
    //    X+?	X, one or more times 
    //    X{n}?	X, exactly n times 
    //    X{n,}?	X, at least n times 
    //    X{n,m}?	X, at least n but not more than m times 

    //    Possessive quantifiers 
    //    X?+	X, once or not at all 
    //    X*+	X, zero or more times 
    //    X++	X, one or more times 
    //    X{n}+	X, exactly n times 
    //    X{n,}+	X, at least n times 
    //    X{n,m}+	X, at least n but not more than m times 

    //    positive lookahead	(?=text)
    //    negative lookahead	(?!text)
    //    // eg: not at end of line 	    (?!$)
    //    positive lookbehind	(?<=text)
    //    negative lookbehind	(?<!text)
    //    // eg: not at start of line 	(?<!^)
    //    =====================================================================
    //    */

    //    // helper patterns
    //    string whole_line = @"(" + @"^" + text.Trim() + @"$" + @")";
    //    string whole_word = @"(" + @"\b" + text.Trim() + @"\b" + @")";
    //    string word_with_prefix = @"(" + @"\b" + @"\S+?" + text.TrimEnd() + @"\b" + @")";
    //    string word_with_suffix = @"(" + @"\b" + text.TrimStart() + @"\S+?" + @"\b" + @")";
    //    string word_with_prefix_and_suffix = @"(" + @"\b" + @"\S+?" + text + @"\S+?" + @"\b" + @")";
    //    string word_with_fix_or_fixes = @"(" + word_with_prefix + "|" + word_with_suffix + "|" + word_with_prefix_and_suffix + @")";

    //    /////////////////////////////////////////////////
    //    ////// wordness_wordlocation_verselocation //////
    //    /////////////////////////////////////////////////
    //    string whole____start____start = whole_line + "|" + @"(" + @"^" + whole_word + @"\s" + @")";
    //    string whole____start___middle = whole_line + "|" + @"(" + @"\s" + whole_word + @"\s" + @")";
    //    string whole____start______end = whole_line + "|" + @"(" + @"\s" + whole_word + @"$" + @")";
    //    string whole____start_anywhere = whole_line + "|" + @"(" + whole_word + @")";
    //    string whole___middle____start = whole_line + "|" + @"(" + @"^" + whole_word + @"\s" + @")";
    //    string whole___middle___middle = whole_line + "|" + @"(" + @"\s" + whole_word + @"\s" + @")";
    //    string whole___middle______end = whole_line + "|" + @"(" + @"\s" + whole_word + @"$" + @")";
    //    string whole___middle_anywhere = whole_line + "|" + @"(" + whole_word + @")";
    //    string whole______end____start = whole_line + "|" + @"(" + @"^" + whole_word + @"\s" + @")";
    //    string whole______end___middle = whole_line + "|" + @"(" + @"\s" + whole_word + @"\s" + @")";
    //    string whole______end______end = whole_line + "|" + @"(" + @"\s" + whole_word + @"$" + @")";
    //    string whole______end_anywhere = whole_line + "|" + @"(" + whole_word + @")";
    //    string whole_anywhere____start = whole_line + "|" + @"(" + @"^" + whole_word + @"\s" + @")";
    //    string whole_anywhere___middle = whole_line + "|" + @"(" + @"\s" + whole_word + @"\s" + @")";
    //    string whole_anywhere______end = whole_line + "|" + @"(" + @"\s" + whole_word + @"$" + @")";
    //    string whole_anywhere_anywhere = whole_line + "|" + @"(" + whole_word + @")";

    //    string part_____start____start = @"(" + @"^" + word_with_suffix + @")";
    //    string part_____start___middle = @"(" + @"\s" + word_with_suffix + @"\s" + @")";
    //    string part_____start______end = @"(" + word_with_suffix + @"$" + @")";
    //    string part_____start_anywhere = @"(" + word_with_suffix + @")";
    //    string part____middle____start = @"(" + @"^" + word_with_prefix_and_suffix + @")";
    //    string part____middle___middle = @"(" + @"\s" + word_with_prefix_and_suffix + @"\s" + @")";
    //    string part____middle______end = @"(" + word_with_prefix_and_suffix + @"$" + @")";
    //    string part____middle_anywhere = @"(" + word_with_prefix_and_suffix + @")";
    //    string part_______end____start = @"(" + @"^" + word_with_prefix + @")";
    //    string part_______end___middle = @"(" + @"\s" + word_with_prefix + @"\s" + @")";
    //    string part_______end______end = @"(" + word_with_prefix + @"$" + @")";
    //    string part_______end_anywhere = @"(" + word_with_prefix + @")";
    //    string part__anywhere____start = @"(" + @"^" + word_with_fix_or_fixes + @")";
    //    string part__anywhere___middle = @"(" + @"\s" + word_with_fix_or_fixes + @"\s" + @")";
    //    string part__anywhere______end = @"(" + word_with_fix_or_fixes + @"$" + @")";
    //    string part__anywhere_anywhere = @"(" + word_with_fix_or_fixes + @")";

    //    string any______start____start = whole____start____start + "|" + part_____start____start;
    //    string any______start___middle = whole____start___middle + "|" + part_____start___middle;
    //    string any______start______end = whole____start______end + "|" + part_____start______end;
    //    string any______start_anywhere = whole____start_anywhere + "|" + part_____start_anywhere;
    //    string any_____middle____start = whole___middle____start + "|" + part____middle____start;
    //    string any_____middle___middle = whole___middle___middle + "|" + part____middle___middle;
    //    string any_____middle______end = whole___middle______end + "|" + part____middle______end;
    //    string any_____middle_anywhere = whole___middle_anywhere + "|" + part____middle_anywhere;
    //    string any________end____start = whole______end____start + "|" + part_______end____start;
    //    string any________end___middle = whole______end___middle + "|" + part_______end___middle;
    //    string any________end______end = whole______end______end + "|" + part_______end______end;
    //    string any________end_anywhere = whole______end_anywhere + "|" + part_______end_anywhere;
    //    string any___anywhere____start = whole_anywhere____start + "|" + part__anywhere____start;
    //    string any___anywhere___middle = whole_anywhere___middle + "|" + part__anywhere___middle;
    //    string any___anywhere______end = whole_anywhere______end + "|" + part__anywhere______end;
    //    string any___anywhere_anywhere = whole_anywhere_anywhere + "|" + part__anywhere_anywhere;


    //    string pattern = null;
    //    switch (text_wordness)
    //    {
    //        case TextWordness.WholeWord:
    //            switch (text_location_in_word)
    //            {
    //                case TextLocationInWord.AtStart:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = whole____start____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = whole____start___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = whole____start______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = whole____start_anywhere;
    //                            break;
    //                    }
    //                    break;
    //                case TextLocationInWord.AtMiddle:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = whole___middle____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = whole___middle___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = whole___middle______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = whole___middle_anywhere;
    //                            break;
    //                    }
    //                    break;
    //                case TextLocationInWord.AtEnd:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = whole______end____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = whole______end___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = whole______end______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = whole______end_anywhere;
    //                            break;
    //                    }
    //                    break;
    //                case TextLocationInWord.Any:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = whole_anywhere____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = whole_anywhere___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = whole_anywhere______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = whole_anywhere_anywhere;
    //                            break;
    //                    }
    //                    break;
    //            }
    //            break;
    //        case TextWordness.PartOfWord:
    //            switch (text_location_in_word)
    //            {
    //                case TextLocationInWord.AtStart:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = part_____start____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = part_____start___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = part_____start______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = part_____start_anywhere;
    //                            break;
    //                    }
    //                    break;
    //                case TextLocationInWord.AtMiddle:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = part____middle____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = part____middle___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = part____middle______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = part____middle_anywhere;
    //                            break;
    //                    }
    //                    break;
    //                case TextLocationInWord.AtEnd:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = part_______end____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = part_______end___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = part_______end______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = part_______end_anywhere;
    //                            break;
    //                    }
    //                    break;
    //                case TextLocationInWord.Any:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = part__anywhere____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = part__anywhere___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = part__anywhere______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = part__anywhere_anywhere;
    //                            break;
    //                    }
    //                    break;
    //            }
    //            break;
    //        case TextWordness.Any:
    //            switch (text_location_in_word)
    //            {
    //                case TextLocationInWord.AtStart:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = any______start____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = any______start___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = any______start______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = any______start_anywhere;
    //                            break;
    //                    }
    //                    break;
    //                case TextLocationInWord.AtMiddle:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = any_____middle____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = any_____middle___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = any_____middle______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = any_____middle_anywhere;
    //                            break;
    //                    }
    //                    break;
    //                case TextLocationInWord.AtEnd:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = any________end____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = any________end___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = any________end______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = any________end_anywhere;
    //                            break;
    //                    }
    //                    break;
    //                case TextLocationInWord.Any:
    //                    switch (text_location_in_verse)
    //                    {
    //                        case TextLocationInVerse.AtStart:
    //                            pattern = any___anywhere____start;
    //                            break;
    //                        case TextLocationInVerse.AtMiddle:
    //                            pattern = any___anywhere___middle;
    //                            break;
    //                        case TextLocationInVerse.AtEnd:
    //                            pattern = any___anywhere______end;
    //                            break;
    //                        case TextLocationInVerse.Any:
    //                            pattern = any___anywhere_anywhere;
    //                            break;
    //                    }
    //                    break;
    //            }
    //            break;
    //    }

    //    return pattern;
    //}
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
        switch (text_search_block_size)
        {
            case TextSearchBlockSize.Verse:
                {
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
            case TextSearchBlockSize.Chapter:
                {
                    List<Chapter> chapters = DoFindChapters(text, text_proximity_type);
                    if (chapters != null)
                    {
                        foreach (Chapter chapter in chapters)
                        {
                            if (chapter != null)
                            {
                                result.AddRange(chapter.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Page:
                {
                    List<Page> pages = DoFindPages(text, text_proximity_type);
                    if (pages != null)
                    {
                        foreach (Page page in pages)
                        {
                            if (page != null)
                            {
                                result.AddRange(page.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Station:
                {
                    List<Station> stations = DoFindStations(text, text_proximity_type);
                    if (stations != null)
                    {
                        foreach (Station station in stations)
                        {
                            if (station != null)
                            {
                                result.AddRange(station.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Part:
                {
                    List<Part> parts = DoFindParts(text, text_proximity_type);
                    if (parts != null)
                    {
                        foreach (Part part in parts)
                        {
                            if (part != null)
                            {
                                result.AddRange(part.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Group:
                {
                    List<Model.Group> groups = DoFindGroups(text, text_proximity_type);
                    if (groups != null)
                    {
                        foreach (Model.Group group in groups)
                        {
                            if (group != null)
                            {
                                result.AddRange(group.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Half:
                {
                    List<Half> halfs = DoFindHalfs(text, text_proximity_type);
                    if (halfs != null)
                    {
                        foreach (Half half in halfs)
                        {
                            if (half != null)
                            {
                                result.AddRange(half.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Quarter:
                {
                    List<Quarter> quarters = DoFindQuarters(text, text_proximity_type);
                    if (quarters != null)
                    {
                        foreach (Quarter quarter in quarters)
                        {
                            if (quarter != null)
                            {
                                result.AddRange(quarter.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Bowing:
                {
                    List<Bowing> bowings = DoFindBowings(text, text_proximity_type);
                    if (bowings != null)
                    {
                        foreach (Bowing bowing in bowings)
                        {
                            if (bowing != null)
                            {
                                result.AddRange(bowing.Verses);
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
        if (language_type == LanguageType.RightToLeft)
        {
            return DoFindPhrases(result, current_selection, null, text, TextProximityType.AnyWord, TextWordness.Any, false, false, true);
        }
        else //if (language_type == FindByTextLanguageType.LeftToRight)
        {
            return DoFindPhrases(translation, result, current_selection, null, text, TextProximityType.AnyWord, TextWordness.Any, false, false);
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

                        // if nothing found, try emlaaei
                        if (result.Count == 0)
                        {
                            //  search in emlaaei
                            if (try_emlaaei_if_nothing_found)
                            {
                                if (s_numerology_system != null)
                                {
                                    if ((source != null) && (source.Count > 0))
                                    {
                                        foreach (Verse verse in source)
                                        {
                                            //foreach (Word word in verse.Words)
                                            //{
                                            string verse_emlaaei_text = verse.Translations[DEFAULT_EMLAAEI_TEXT].SimplifyTo(s_numerology_system.TextMode);
                                            if (text_proximity_type == TextProximityType.AllWords)
                                            {
                                                bool found = false;
                                                foreach (string negative_word in negative_words)
                                                {
                                                    if (text_wordness == TextWordness.Any)
                                                    {
                                                        if (verse_emlaaei_text.Contains(negative_word))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((verse_emlaaei_text.Contains(negative_word)) && (verse_emlaaei_text.Length > negative_word.Length))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.WholeWord)
                                                    {
                                                        if (verse_emlaaei_text == negative_word) //??? we need word_emlaaei_text not verse_emlaaei_text
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                }
                                                if (found) continue;

                                                foreach (string positive_word in positive_words)
                                                {
                                                    if (text_wordness == TextWordness.Any)
                                                    {
                                                        if (!verse_emlaaei_text.Contains(positive_word))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.PartOfWord)
                                                    {
                                                        if (!(verse_emlaaei_text.Contains(positive_word)) || !(verse_emlaaei_text.Length > positive_word.Length))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.WholeWord)
                                                    {
                                                        if (verse_emlaaei_text != positive_word) //??? we need word_emlaaei_text not verse_emlaaei_text
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                }
                                                if (found) continue;

                                                if (
                                                     (unsigned_words.Count == 0) ||
                                                     (verse_emlaaei_text.ContainsWords(unsigned_words))
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
                                                        if (verse_emlaaei_text.Contains(negative_word))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.PartOfWord)
                                                    {
                                                        if ((verse_emlaaei_text.Contains(negative_word)) && (verse_emlaaei_text.Length > negative_word.Length))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.WholeWord)
                                                    {
                                                        if (verse_emlaaei_text == negative_word) //??? we need word_emlaaei_text not verse_emlaaei_text
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                }
                                                if (found) continue;

                                                foreach (string positive_word in positive_words)
                                                {
                                                    if (text_wordness == TextWordness.Any)
                                                    {
                                                        if (!verse_emlaaei_text.Contains(positive_word))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.PartOfWord)
                                                    {
                                                        if (!(verse_emlaaei_text.Contains(positive_word)) || !(verse_emlaaei_text.Length > positive_word.Length))
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                    else if (text_wordness == TextWordness.WholeWord)
                                                    {
                                                        if (verse_emlaaei_text != positive_word) //??? we need word_emlaaei_text not verse_emlaaei_text
                                                        {
                                                            found = true;
                                                            //break; // no break in case there are more matches
                                                        }
                                                    }
                                                }
                                                if (found) continue;

                                                if (
                                                     (negative_words.Count > 0) ||
                                                     (positive_words.Count > 0) ||
                                                     (
                                                       (unsigned_words.Count == 0) ||
                                                       (verse_emlaaei_text.ContainsWord(unsigned_words))
                                                     )
                                                   )
                                                {
                                                    result.Add(new Phrase(verse, 0, ""));
                                                }
                                            }
                                        } // end for
                                        //}
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

                    RegexOptions regex_options = case_sensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    text = Regex.Replace(text, @"\s+", " ", regex_options); // remove double space or higher if any

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
                                     (verse.Translations[translation].ContainsWords(unsigned_words))
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
                                       (verse.Translations[translation].ContainsWord(unsigned_words))
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
    private static List<Chapter> DoFindChapters(string text, TextProximityType text_proximity_type)
    {
        List<Chapter> result = new List<Chapter>();

        if (!String.IsNullOrEmpty(text))
        {
            text = text.SimplifyTo(s_numerology_system.TextMode);
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            try
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (s_book != null)
                {
                    foreach (Chapter chapter in s_book.Chapters)
                    {
                        string chapter_text = chapter.Text.Trim();
                        chapter_text = chapter_text.SimplifyTo(s_numerology_system.TextMode);
                        chapter_text = chapter_text.Trim();

                        if (text_proximity_type == TextProximityType.AllWords)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (chapter_text.Contains(word))
                                {
                                    found = true; // next chapter
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string word in positive_words)
                            {
                                if (!chapter_text.Contains(word))
                                {
                                    found = true; // next chapter
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (unsigned_words.Count == 0) ||
                                 (chapter_text.ContainsWords(unsigned_words))
                               )
                            {
                                if (!result.Contains(chapter))
                                {
                                    result.Add(chapter);
                                }
                            }
                        }
                        else if (text_proximity_type == TextProximityType.AnyWord)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (chapter_text.Contains(word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string positive_word in positive_words)
                            {
                                if (!chapter_text.Contains(positive_word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (negative_words.Count > 0) ||
                                 (positive_words.Count > 0) ||
                                 (
                                   (unsigned_words.Count == 0) ||
                                   (chapter_text.ContainsWord(unsigned_words))
                                 )
                               )
                            {
                                if (!result.Contains(chapter))
                                {
                                    result.Add(chapter);
                                }
                            }
                        }
                    } // end for
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Page> DoFindPages(string text, TextProximityType text_proximity_type)
    {
        List<Page> result = new List<Page>();

        if (!String.IsNullOrEmpty(text))
        {
            text = text.SimplifyTo(s_numerology_system.TextMode);
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            try
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (s_book != null)
                {
                    foreach (Page page in s_book.Pages)
                    {
                        string page_text = page.Text.Trim();
                        page_text = page_text.SimplifyTo(s_numerology_system.TextMode);
                        page_text = page_text.Trim();

                        if (text_proximity_type == TextProximityType.AllWords)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (page_text.Contains(word))
                                {
                                    found = true; // next page
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string word in positive_words)
                            {
                                if (!page_text.Contains(word))
                                {
                                    found = true; // next page
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (unsigned_words.Count == 0) ||
                                 (page_text.ContainsWords(unsigned_words))
                               )
                            {
                                if (!result.Contains(page))
                                {
                                    result.Add(page);
                                }
                            }
                        }
                        else if (text_proximity_type == TextProximityType.AnyWord)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (page_text.Contains(word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string positive_word in positive_words)
                            {
                                if (!page_text.Contains(positive_word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (negative_words.Count > 0) ||
                                 (positive_words.Count > 0) ||
                                 (
                                   (unsigned_words.Count == 0) ||
                                   (page_text.ContainsWord(unsigned_words))
                                 )
                               )
                            {
                                if (!result.Contains(page))
                                {
                                    result.Add(page);
                                }
                            }
                        }
                    } // end for
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Station> DoFindStations(string text, TextProximityType text_proximity_type)
    {
        List<Station> result = new List<Station>();

        if (!String.IsNullOrEmpty(text))
        {
            text = text.SimplifyTo(s_numerology_system.TextMode);
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            try
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (s_book != null)
                {
                    foreach (Station station in s_book.Stations)
                    {
                        string station_text = station.Text.Trim();
                        station_text = station_text.SimplifyTo(s_numerology_system.TextMode);
                        station_text = station_text.Trim();

                        if (text_proximity_type == TextProximityType.AllWords)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (station_text.Contains(word))
                                {
                                    found = true; // next station
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string word in positive_words)
                            {
                                if (!station_text.Contains(word))
                                {
                                    found = true; // next station
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (unsigned_words.Count == 0) ||
                                 (station_text.ContainsWords(unsigned_words))
                               )
                            {
                                if (!result.Contains(station))
                                {
                                    result.Add(station);
                                }
                            }
                        }
                        else if (text_proximity_type == TextProximityType.AnyWord)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (station_text.Contains(word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string positive_word in positive_words)
                            {
                                if (!station_text.Contains(positive_word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (negative_words.Count > 0) ||
                                 (positive_words.Count > 0) ||
                                 (
                                   (unsigned_words.Count == 0) ||
                                   (station_text.ContainsWord(unsigned_words))
                                 )
                               )
                            {
                                if (!result.Contains(station))
                                {
                                    result.Add(station);
                                }
                            }
                        }
                    } // end for
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Part> DoFindParts(string text, TextProximityType text_proximity_type)
    {
        List<Part> result = new List<Part>();

        if (!String.IsNullOrEmpty(text))
        {
            text = text.SimplifyTo(s_numerology_system.TextMode);
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            try
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (s_book != null)
                {
                    foreach (Part part in s_book.Parts)
                    {
                        string part_text = part.Text.Trim();
                        part_text = part_text.SimplifyTo(s_numerology_system.TextMode);
                        part_text = part_text.Trim();

                        if (text_proximity_type == TextProximityType.AllWords)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (part_text.Contains(word))
                                {
                                    found = true; // next part
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string word in positive_words)
                            {
                                if (!part_text.Contains(word))
                                {
                                    found = true; // next part
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (unsigned_words.Count == 0) ||
                                 (part_text.ContainsWords(unsigned_words))
                               )
                            {
                                if (!result.Contains(part))
                                {
                                    result.Add(part);
                                }
                            }
                        }
                        else if (text_proximity_type == TextProximityType.AnyWord)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (part_text.Contains(word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string positive_word in positive_words)
                            {
                                if (!part_text.Contains(positive_word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (negative_words.Count > 0) ||
                                 (positive_words.Count > 0) ||
                                 (
                                   (unsigned_words.Count == 0) ||
                                   (part_text.ContainsWord(unsigned_words))
                                 )
                               )
                            {
                                if (!result.Contains(part))
                                {
                                    result.Add(part);
                                }
                            }
                        }
                    } // end for
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Model.Group> DoFindGroups(string text, TextProximityType text_proximity_type)
    {
        List<Model.Group> result = new List<Model.Group>();

        if (!String.IsNullOrEmpty(text))
        {
            text = text.SimplifyTo(s_numerology_system.TextMode);
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            try
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (s_book != null)
                {
                    foreach (Model.Group group in s_book.Groups)
                    {
                        string group_text = group.Text.Trim();
                        group_text = group_text.SimplifyTo(s_numerology_system.TextMode);
                        group_text = group_text.Trim();

                        if (text_proximity_type == TextProximityType.AllWords)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (group_text.Contains(word))
                                {
                                    found = true; // next group
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string word in positive_words)
                            {
                                if (!group_text.Contains(word))
                                {
                                    found = true; // next group
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (unsigned_words.Count == 0) ||
                                 (group_text.ContainsWords(unsigned_words))
                               )
                            {
                                if (!result.Contains(group))
                                {
                                    result.Add(group);
                                }
                            }
                        }
                        else if (text_proximity_type == TextProximityType.AnyWord)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (group_text.Contains(word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string positive_word in positive_words)
                            {
                                if (!group_text.Contains(positive_word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (negative_words.Count > 0) ||
                                 (positive_words.Count > 0) ||
                                 (
                                   (unsigned_words.Count == 0) ||
                                   (group_text.ContainsWord(unsigned_words))
                                 )
                               )
                            {
                                if (!result.Contains(group))
                                {
                                    result.Add(group);
                                }
                            }
                        }
                    } // end for
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Half> DoFindHalfs(string text, TextProximityType text_proximity_type)
    {
        List<Half> result = new List<Half>();

        if (!String.IsNullOrEmpty(text))
        {
            text = text.SimplifyTo(s_numerology_system.TextMode);
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            try
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (s_book != null)
                {
                    foreach (Half half in s_book.Halfs)
                    {
                        string half_text = half.Text.Trim();
                        half_text = half_text.SimplifyTo(s_numerology_system.TextMode);
                        half_text = half_text.Trim();

                        if (text_proximity_type == TextProximityType.AllWords)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (half_text.Contains(word))
                                {
                                    found = true; // next half
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string word in positive_words)
                            {
                                if (!half_text.Contains(word))
                                {
                                    found = true; // next half
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (unsigned_words.Count == 0) ||
                                 (half_text.ContainsWords(unsigned_words))
                               )
                            {
                                if (!result.Contains(half))
                                {
                                    result.Add(half);
                                }
                            }
                        }
                        else if (text_proximity_type == TextProximityType.AnyWord)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (half_text.Contains(word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string positive_word in positive_words)
                            {
                                if (!half_text.Contains(positive_word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (negative_words.Count > 0) ||
                                 (positive_words.Count > 0) ||
                                 (
                                   (unsigned_words.Count == 0) ||
                                   (half_text.ContainsWord(unsigned_words))
                                 )
                               )
                            {
                                if (!result.Contains(half))
                                {
                                    result.Add(half);
                                }
                            }
                        }
                    } // end for
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Quarter> DoFindQuarters(string text, TextProximityType text_proximity_type)
    {
        List<Quarter> result = new List<Quarter>();

        if (!String.IsNullOrEmpty(text))
        {
            text = text.SimplifyTo(s_numerology_system.TextMode);
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            try
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (s_book != null)
                {
                    foreach (Quarter quarter in s_book.Quarters)
                    {
                        string quarter_text = quarter.Text.Trim();
                        quarter_text = quarter_text.SimplifyTo(s_numerology_system.TextMode);
                        quarter_text = quarter_text.Trim();

                        if (text_proximity_type == TextProximityType.AllWords)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (quarter_text.Contains(word))
                                {
                                    found = true; // next quarter
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string word in positive_words)
                            {
                                if (!quarter_text.Contains(word))
                                {
                                    found = true; // next quarter
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (unsigned_words.Count == 0) ||
                                 (quarter_text.ContainsWords(unsigned_words))
                               )
                            {
                                if (!result.Contains(quarter))
                                {
                                    result.Add(quarter);
                                }
                            }
                        }
                        else if (text_proximity_type == TextProximityType.AnyWord)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (quarter_text.Contains(word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string positive_word in positive_words)
                            {
                                if (!quarter_text.Contains(positive_word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (negative_words.Count > 0) ||
                                 (positive_words.Count > 0) ||
                                 (
                                   (unsigned_words.Count == 0) ||
                                   (quarter_text.ContainsWord(unsigned_words))
                                 )
                               )
                            {
                                if (!result.Contains(quarter))
                                {
                                    result.Add(quarter);
                                }
                            }
                        }
                    } // end for
                }
            }
            catch
            {
                // log exception
            }
        }

        return result;
    }
    private static List<Bowing> DoFindBowings(string text, TextProximityType text_proximity_type)
    {
        List<Bowing> result = new List<Bowing>();

        if (!String.IsNullOrEmpty(text))
        {
            text = text.SimplifyTo(s_numerology_system.TextMode);
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            try
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (s_book != null)
                {
                    foreach (Bowing bowing in s_book.Bowings)
                    {
                        string bowing_text = bowing.Text.Trim();
                        bowing_text = bowing_text.SimplifyTo(s_numerology_system.TextMode);
                        bowing_text = bowing_text.Trim();

                        if (text_proximity_type == TextProximityType.AllWords)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (bowing_text.Contains(word))
                                {
                                    found = true; // next bowing
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string word in positive_words)
                            {
                                if (!bowing_text.Contains(word))
                                {
                                    found = true; // next bowing
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (unsigned_words.Count == 0) ||
                                 (bowing_text.ContainsWords(unsigned_words))
                               )
                            {
                                if (!result.Contains(bowing))
                                {
                                    result.Add(bowing);
                                }
                            }
                        }
                        else if (text_proximity_type == TextProximityType.AnyWord)
                        {
                            bool found = false;
                            foreach (string word in negative_words)
                            {
                                if (bowing_text.Contains(word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            foreach (string positive_word in positive_words)
                            {
                                if (!bowing_text.Contains(positive_word))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found) continue;

                            if (
                                 (negative_words.Count > 0) ||
                                 (positive_words.Count > 0) ||
                                 (
                                   (unsigned_words.Count == 0) ||
                                   (bowing_text.ContainsWord(unsigned_words))
                                 )
                               )
                            {
                                if (!result.Contains(bowing))
                                {
                                    result.Add(bowing);
                                }
                            }
                        }
                    } // end for
                }
            }
            catch
            {
                // log exception
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
        switch (text_search_block_size)
        {
            case TextSearchBlockSize.Verse:
                {
                    List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
                    return DoFindPhrases(source, text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                }
            case TextSearchBlockSize.Chapter:
                {
                    List<Chapter> chapters = DoFindChapters(text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                    if (chapters != null)
                    {
                        foreach (Chapter chapter in chapters)
                        {
                            if (chapter != null)
                            {
                                result.AddRange(chapter.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Page:
                {
                    List<Page> pages = DoFindPages(text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                    if (pages != null)
                    {
                        foreach (Page page in pages)
                        {
                            if (page != null)
                            {
                                result.AddRange(page.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Station:
                {
                    List<Station> stations = DoFindStations(text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                    if (stations != null)
                    {
                        foreach (Station station in stations)
                        {
                            if (station != null)
                            {
                                result.AddRange(station.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Part:
                {
                    List<Part> parts = DoFindParts(text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                    if (parts != null)
                    {
                        foreach (Part part in parts)
                        {
                            if (part != null)
                            {
                                result.AddRange(part.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Group:
                {
                    List<Model.Group> groups = DoFindGroups(text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                    if (groups != null)
                    {
                        foreach (Model.Group group in groups)
                        {
                            if (group != null)
                            {
                                result.AddRange(group.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Half:
                {
                    List<Half> halfs = DoFindHalfs(text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                    if (halfs != null)
                    {
                        foreach (Half half in halfs)
                        {
                            if (half != null)
                            {
                                result.AddRange(half.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Quarter:
                {
                    List<Quarter> quarters = DoFindQuarters(text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                    if (quarters != null)
                    {
                        foreach (Quarter quarter in quarters)
                        {
                            if (quarter != null)
                            {
                                result.AddRange(quarter.Verses);
                            }
                        }
                    }
                }
                break;
            case TextSearchBlockSize.Bowing:
                {
                    List<Bowing> bowings = DoFindBowings(text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                    if (bowings != null)
                    {
                        foreach (Bowing bowing in bowings)
                        {
                            if (bowing != null)
                            {
                                result.AddRange(bowing.Verses);
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
        return DoFindPhrases(result, text, -1, NumberType.None, ComparisonOperator.Equal, -1);
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
    private static List<Chapter> DoFindChapters(string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Chapter> result = new List<Chapter>();
        if (s_book != null)
        {
            List<Chapter> source = s_book.Chapters;

            if (String.IsNullOrEmpty(text)) return null;
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            string[] parts = text.Split();
            if (parts.Length > 0) // enable nested searches
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (negative_words.Count > 0)
                {
                    foreach (string word in negative_words)
                    {
                        List<Chapter> temp = DoFindChapters(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result = new List<Chapter>(source);
                        foreach (Chapter chapter in temp)
                        {
                            result.Remove(chapter);
                        }

                        source = new List<Chapter>(result);
                    }
                }

                if (positive_words.Count > 0)
                {
                    foreach (string word in positive_words)
                    {
                        List<Chapter> temp = DoFindChapters(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Chapter chapter in temp)
                        {
                            if (!result.Contains(chapter))
                            {
                                result.Add(chapter);
                            }
                        }

                        source = new List<Chapter>(result);
                    }
                }

                if (unsigned_words.Count > 0)
                {
                    foreach (string word in unsigned_words)
                    {
                        List<Chapter> temp = DoFindChapters(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Chapter chapter in temp)
                        {
                            if (!result.Contains(chapter))
                            {
                                result.Add(chapter);
                            }
                        }

                        source = new List<Chapter>(result);
                    }
                }
            }
        }
        return result;
    }
    private static List<Chapter> DoFindChapters(List<Chapter> source, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Chapter> result = new List<Chapter>();
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
                            result = GetChaptersWithRootWords(source, root_words, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                        }
                    }
                }
            }
            catch
            {
                // log exception
            }
        }
        return result;
    }
    private static List<Chapter> GetChaptersWithRootWords(List<Chapter> source, List<Word> root_words, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Chapter> result = new List<Chapter>();
        if (source != null)
        {
            Dictionary<Chapter, int> multiplicity_dictionary = new Dictionary<Chapter, int>();
            foreach (Word word in root_words)
            {
                Chapter chapter = word.Verse.Chapter;
                if (multiplicity_dictionary.ContainsKey(chapter))
                {
                    multiplicity_dictionary[chapter]++;
                }
                else // first found
                {
                    multiplicity_dictionary.Add(chapter, 1);
                }
            }

            if (multiplicity == 0) // chapters not containg word
            {
                foreach (Chapter chapter in source)
                {
                    if (!multiplicity_dictionary.ContainsKey(chapter))
                    {
                        if (!result.Contains(chapter))
                        {
                            result.Add(chapter);
                        }
                    }
                }
            }
            else // add only matching multiplicity or wildcard (-1)
            {
                foreach (Word word in root_words)
                {
                    Chapter chapter = word.Verse.Chapter;
                    if (source.Contains(chapter))
                    {
                        if (multiplicity_dictionary.ContainsKey(chapter))
                        {
                            if ((multiplicity == -1) || (Compare(multiplicity_dictionary[chapter], multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder)))
                            {
                                if (!result.Contains(chapter))
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
    private static List<Page> DoFindPages(string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Page> result = new List<Page>();
        if (s_book != null)
        {
            List<Page> source = s_book.Pages;

            if (String.IsNullOrEmpty(text)) return null;
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            string[] parts = text.Split();
            if (parts.Length > 0) // enable nested searches
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (negative_words.Count > 0)
                {
                    foreach (string word in negative_words)
                    {
                        List<Page> temp = DoFindPages(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result = new List<Page>(source);
                        foreach (Page page in temp)
                        {
                            result.Remove(page);
                        }

                        source = new List<Page>(result);
                    }
                }

                if (positive_words.Count > 0)
                {
                    foreach (string word in positive_words)
                    {
                        List<Page> temp = DoFindPages(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Page page in temp)
                        {
                            if (!result.Contains(page))
                            {
                                result.Add(page);
                            }
                        }

                        source = new List<Page>(result);
                    }
                }

                if (unsigned_words.Count > 0)
                {
                    foreach (string word in unsigned_words)
                    {
                        List<Page> temp = DoFindPages(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Page page in temp)
                        {
                            if (!result.Contains(page))
                            {
                                result.Add(page);
                            }
                        }

                        source = new List<Page>(result);
                    }
                }
            }
        }
        return result;
    }
    private static List<Page> DoFindPages(List<Page> source, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Page> result = new List<Page>();
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
                            result = GetPagesWithRootWords(source, root_words, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                        }
                    }
                }
            }
            catch
            {
                // log exception
            }
        }
        return result;
    }
    private static List<Page> GetPagesWithRootWords(List<Page> source, List<Word> root_words, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Page> result = new List<Page>();
        if (source != null)
        {
            Dictionary<Page, int> multiplicity_dictionary = new Dictionary<Page, int>();
            foreach (Word word in root_words)
            {
                Page page = word.Verse.Page;
                if (multiplicity_dictionary.ContainsKey(page))
                {
                    multiplicity_dictionary[page]++;
                }
                else // first found
                {
                    multiplicity_dictionary.Add(page, 1);
                }
            }

            if (multiplicity == 0) // pages not containg word
            {
                foreach (Page page in source)
                {
                    if (!multiplicity_dictionary.ContainsKey(page))
                    {
                        if (!result.Contains(page))
                        {
                            result.Add(page);
                        }
                    }
                }
            }
            else // add only matching multiplicity or wildcard (-1)
            {
                foreach (Word word in root_words)
                {
                    Page page = word.Verse.Page;
                    if (source.Contains(page))
                    {
                        if (multiplicity_dictionary.ContainsKey(page))
                        {
                            if ((multiplicity == -1) || (Compare(multiplicity_dictionary[page], multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder)))
                            {
                                if (!result.Contains(page))
                                {
                                    result.Add(page);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private static List<Station> DoFindStations(string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Station> result = new List<Station>();
        if (s_book != null)
        {
            List<Station> source = s_book.Stations;

            if (String.IsNullOrEmpty(text)) return null;
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            string[] parts = text.Split();
            if (parts.Length > 0) // enable nested searches
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (negative_words.Count > 0)
                {
                    foreach (string word in negative_words)
                    {
                        List<Station> temp = DoFindStations(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result = new List<Station>(source);
                        foreach (Station station in temp)
                        {
                            result.Remove(station);
                        }

                        source = new List<Station>(result);
                    }
                }

                if (positive_words.Count > 0)
                {
                    foreach (string word in positive_words)
                    {
                        List<Station> temp = DoFindStations(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Station station in temp)
                        {
                            if (!result.Contains(station))
                            {
                                result.Add(station);
                            }
                        }

                        source = new List<Station>(result);
                    }
                }

                if (unsigned_words.Count > 0)
                {
                    foreach (string word in unsigned_words)
                    {
                        List<Station> temp = DoFindStations(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Station station in temp)
                        {
                            if (!result.Contains(station))
                            {
                                result.Add(station);
                            }
                        }

                        source = new List<Station>(result);
                    }
                }
            }
        }
        return result;
    }
    private static List<Station> DoFindStations(List<Station> source, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Station> result = new List<Station>();
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
                            result = GetStationsWithRootWords(source, root_words, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                        }
                    }
                }
            }
            catch
            {
                // log exception
            }
        }
        return result;
    }
    private static List<Station> GetStationsWithRootWords(List<Station> source, List<Word> root_words, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Station> result = new List<Station>();
        if (source != null)
        {
            Dictionary<Station, int> multiplicity_dictionary = new Dictionary<Station, int>();
            foreach (Word word in root_words)
            {
                Station station = word.Verse.Station;
                if (multiplicity_dictionary.ContainsKey(station))
                {
                    multiplicity_dictionary[station]++;
                }
                else // first found
                {
                    multiplicity_dictionary.Add(station, 1);
                }
            }

            if (multiplicity == 0) // stations not containg word
            {
                foreach (Station station in source)
                {
                    if (!multiplicity_dictionary.ContainsKey(station))
                    {
                        if (!result.Contains(station))
                        {
                            result.Add(station);
                        }
                    }
                }
            }
            else // add only matching multiplicity or wildcard (-1)
            {
                foreach (Word word in root_words)
                {
                    Station station = word.Verse.Station;
                    if (source.Contains(station))
                    {
                        if (multiplicity_dictionary.ContainsKey(station))
                        {
                            if ((multiplicity == -1) || (Compare(multiplicity_dictionary[station], multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder)))
                            {
                                if (!result.Contains(station))
                                {
                                    result.Add(station);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private static List<Part> DoFindParts(string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Part> result = new List<Part>();
        if (s_book != null)
        {
            List<Part> source = s_book.Parts;

            if (String.IsNullOrEmpty(text)) return null;
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            string[] parts = text.Split();
            if (parts.Length > 0) // enable nested searches
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (negative_words.Count > 0)
                {
                    foreach (string word in negative_words)
                    {
                        List<Part> temp = DoFindParts(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result = new List<Part>(source);
                        foreach (Part part in temp)
                        {
                            result.Remove(part);
                        }

                        source = new List<Part>(result);
                    }
                }

                if (positive_words.Count > 0)
                {
                    foreach (string word in positive_words)
                    {
                        List<Part> temp = DoFindParts(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Part part in temp)
                        {
                            if (!result.Contains(part))
                            {
                                result.Add(part);
                            }
                        }

                        source = new List<Part>(result);
                    }
                }

                if (unsigned_words.Count > 0)
                {
                    foreach (string word in unsigned_words)
                    {
                        List<Part> temp = DoFindParts(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Part part in temp)
                        {
                            if (!result.Contains(part))
                            {
                                result.Add(part);
                            }
                        }

                        source = new List<Part>(result);
                    }
                }
            }
        }
        return result;
    }
    private static List<Part> DoFindParts(List<Part> source, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Part> result = new List<Part>();
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
                            result = GetPartsWithRootWords(source, root_words, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                        }
                    }
                }
            }
            catch
            {
                // log exception
            }
        }
        return result;
    }
    private static List<Part> GetPartsWithRootWords(List<Part> source, List<Word> root_words, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Part> result = new List<Part>();
        if (source != null)
        {
            Dictionary<Part, int> multiplicity_dictionary = new Dictionary<Part, int>();
            foreach (Word word in root_words)
            {
                Part part = word.Verse.Part;
                if (multiplicity_dictionary.ContainsKey(part))
                {
                    multiplicity_dictionary[part]++;
                }
                else // first found
                {
                    multiplicity_dictionary.Add(part, 1);
                }
            }

            if (multiplicity == 0) // parts not containg word
            {
                foreach (Part part in source)
                {
                    if (!multiplicity_dictionary.ContainsKey(part))
                    {
                        if (!result.Contains(part))
                        {
                            result.Add(part);
                        }
                    }
                }
            }
            else // add only matching multiplicity or wildcard (-1)
            {
                foreach (Word word in root_words)
                {
                    Part part = word.Verse.Part;
                    if (source.Contains(part))
                    {
                        if (multiplicity_dictionary.ContainsKey(part))
                        {
                            if ((multiplicity == -1) || (Compare(multiplicity_dictionary[part], multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder)))
                            {
                                if (!result.Contains(part))
                                {
                                    result.Add(part);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private static List<Model.Group> DoFindGroups(string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Model.Group> result = new List<Model.Group>();
        if (s_book != null)
        {
            List<Model.Group> source = s_book.Groups;

            if (String.IsNullOrEmpty(text)) return null;
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            string[] parts = text.Split();
            if (parts.Length > 0) // enable nested searches
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (negative_words.Count > 0)
                {
                    foreach (string word in negative_words)
                    {
                        List<Model.Group> temp = DoFindGroups(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result = new List<Model.Group>(source);
                        foreach (Model.Group group in temp)
                        {
                            result.Remove(group);
                        }

                        source = new List<Model.Group>(result);
                    }
                }

                if (positive_words.Count > 0)
                {
                    foreach (string word in positive_words)
                    {
                        List<Model.Group> temp = DoFindGroups(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Model.Group group in temp)
                        {
                            if (!result.Contains(group))
                            {
                                result.Add(group);
                            }
                        }

                        source = new List<Model.Group>(result);
                    }
                }

                if (unsigned_words.Count > 0)
                {
                    foreach (string word in unsigned_words)
                    {
                        List<Model.Group> temp = DoFindGroups(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Model.Group group in temp)
                        {
                            if (!result.Contains(group))
                            {
                                result.Add(group);
                            }
                        }

                        source = new List<Model.Group>(result);
                    }
                }
            }
        }
        return result;
    }
    private static List<Model.Group> DoFindGroups(List<Model.Group> source, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Model.Group> result = new List<Model.Group>();
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
                            result = GetGroupsWithRootWords(source, root_words, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                        }
                    }
                }
            }
            catch
            {
                // log exception
            }
        }
        return result;
    }
    private static List<Model.Group> GetGroupsWithRootWords(List<Model.Group> source, List<Word> root_words, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Model.Group> result = new List<Model.Group>();
        if (source != null)
        {
            Dictionary<Model.Group, int> multiplicity_dictionary = new Dictionary<Model.Group, int>();
            foreach (Word word in root_words)
            {
                Model.Group group = word.Verse.Group;
                if (multiplicity_dictionary.ContainsKey(group))
                {
                    multiplicity_dictionary[group]++;
                }
                else // first found
                {
                    multiplicity_dictionary.Add(group, 1);
                }
            }

            if (multiplicity == 0) // groups not containg word
            {
                foreach (Model.Group group in source)
                {
                    if (!multiplicity_dictionary.ContainsKey(group))
                    {
                        if (!result.Contains(group))
                        {
                            result.Add(group);
                        }
                    }
                }
            }
            else // add only matching multiplicity or wildcard (-1)
            {
                foreach (Word word in root_words)
                {
                    Model.Group group = word.Verse.Group;
                    if (source.Contains(group))
                    {
                        if (multiplicity_dictionary.ContainsKey(group))
                        {
                            if ((multiplicity == -1) || (Compare(multiplicity_dictionary[group], multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder)))
                            {
                                if (!result.Contains(group))
                                {
                                    result.Add(group);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private static List<Half> DoFindHalfs(string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Half> result = new List<Half>();
        if (s_book != null)
        {
            List<Half> source = s_book.Halfs;

            if (String.IsNullOrEmpty(text)) return null;
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            string[] parts = text.Split();
            if (parts.Length > 0) // enable nested searches
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (negative_words.Count > 0)
                {
                    foreach (string word in negative_words)
                    {
                        List<Half> temp = DoFindHalfs(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result = new List<Half>(source);
                        foreach (Half half in temp)
                        {
                            result.Remove(half);
                        }

                        source = new List<Half>(result);
                    }
                }

                if (positive_words.Count > 0)
                {
                    foreach (string word in positive_words)
                    {
                        List<Half> temp = DoFindHalfs(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Half half in temp)
                        {
                            if (!result.Contains(half))
                            {
                                result.Add(half);
                            }
                        }

                        source = new List<Half>(result);
                    }
                }

                if (unsigned_words.Count > 0)
                {
                    foreach (string word in unsigned_words)
                    {
                        List<Half> temp = DoFindHalfs(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Half half in temp)
                        {
                            if (!result.Contains(half))
                            {
                                result.Add(half);
                            }
                        }

                        source = new List<Half>(result);
                    }
                }
            }
        }
        return result;
    }
    private static List<Half> DoFindHalfs(List<Half> source, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Half> result = new List<Half>();
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
                            result = GetHalfsWithRootWords(source, root_words, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                        }
                    }
                }
            }
            catch
            {
                // log exception
            }
        }
        return result;
    }
    private static List<Half> GetHalfsWithRootWords(List<Half> source, List<Word> root_words, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Half> result = new List<Half>();
        if (source != null)
        {
            Dictionary<Half, int> multiplicity_dictionary = new Dictionary<Half, int>();
            foreach (Word word in root_words)
            {
                Half half = word.Verse.Half;
                if (multiplicity_dictionary.ContainsKey(half))
                {
                    multiplicity_dictionary[half]++;
                }
                else // first found
                {
                    multiplicity_dictionary.Add(half, 1);
                }
            }

            if (multiplicity == 0) // halfs not containg word
            {
                foreach (Half half in source)
                {
                    if (!multiplicity_dictionary.ContainsKey(half))
                    {
                        if (!result.Contains(half))
                        {
                            result.Add(half);
                        }
                    }
                }
            }
            else // add only matching multiplicity or wildcard (-1)
            {
                foreach (Word word in root_words)
                {
                    Half half = word.Verse.Half;
                    if (source.Contains(half))
                    {
                        if (multiplicity_dictionary.ContainsKey(half))
                        {
                            if ((multiplicity == -1) || (Compare(multiplicity_dictionary[half], multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder)))
                            {
                                if (!result.Contains(half))
                                {
                                    result.Add(half);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private static List<Quarter> DoFindQuarters(string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Quarter> result = new List<Quarter>();
        if (s_book != null)
        {
            List<Quarter> source = s_book.Quarters;

            if (String.IsNullOrEmpty(text)) return null;
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            string[] parts = text.Split();
            if (parts.Length > 0) // enable nested searches
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (negative_words.Count > 0)
                {
                    foreach (string word in negative_words)
                    {
                        List<Quarter> temp = DoFindQuarters(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result = new List<Quarter>(source);
                        foreach (Quarter quarter in temp)
                        {
                            result.Remove(quarter);
                        }

                        source = new List<Quarter>(result);
                    }
                }

                if (positive_words.Count > 0)
                {
                    foreach (string word in positive_words)
                    {
                        List<Quarter> temp = DoFindQuarters(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Quarter quarter in temp)
                        {
                            if (!result.Contains(quarter))
                            {
                                result.Add(quarter);
                            }
                        }

                        source = new List<Quarter>(result);
                    }
                }

                if (unsigned_words.Count > 0)
                {
                    foreach (string word in unsigned_words)
                    {
                        List<Quarter> temp = DoFindQuarters(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Quarter quarter in temp)
                        {
                            if (!result.Contains(quarter))
                            {
                                result.Add(quarter);
                            }
                        }

                        source = new List<Quarter>(result);
                    }
                }
            }
        }
        return result;
    }
    private static List<Quarter> DoFindQuarters(List<Quarter> source, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Quarter> result = new List<Quarter>();
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
                            result = GetQuartersWithRootWords(source, root_words, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                        }
                    }
                }
            }
            catch
            {
                // log exception
            }
        }
        return result;
    }
    private static List<Quarter> GetQuartersWithRootWords(List<Quarter> source, List<Word> root_words, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Quarter> result = new List<Quarter>();
        if (source != null)
        {
            Dictionary<Quarter, int> multiplicity_dictionary = new Dictionary<Quarter, int>();
            foreach (Word word in root_words)
            {
                Quarter quarter = word.Verse.Quarter;
                if (multiplicity_dictionary.ContainsKey(quarter))
                {
                    multiplicity_dictionary[quarter]++;
                }
                else // first found
                {
                    multiplicity_dictionary.Add(quarter, 1);
                }
            }

            if (multiplicity == 0) // quarters not containg word
            {
                foreach (Quarter quarter in source)
                {
                    if (!multiplicity_dictionary.ContainsKey(quarter))
                    {
                        if (!result.Contains(quarter))
                        {
                            result.Add(quarter);
                        }
                    }
                }
            }
            else // add only matching multiplicity or wildcard (-1)
            {
                foreach (Word word in root_words)
                {
                    Quarter quarter = word.Verse.Quarter;
                    if (source.Contains(quarter))
                    {
                        if (multiplicity_dictionary.ContainsKey(quarter))
                        {
                            if ((multiplicity == -1) || (Compare(multiplicity_dictionary[quarter], multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder)))
                            {
                                if (!result.Contains(quarter))
                                {
                                    result.Add(quarter);
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private static List<Bowing> DoFindBowings(string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Bowing> result = new List<Bowing>();
        if (s_book != null)
        {
            List<Bowing> source = s_book.Bowings;

            if (String.IsNullOrEmpty(text)) return null;
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            string[] parts = text.Split();
            if (parts.Length > 0) // enable nested searches
            {
                List<string> negative_words = new List<string>();
                List<string> positive_words = new List<string>();
                List<string> unsigned_words = new List<string>();
                BuildWordLists(text, out unsigned_words, out positive_words, out negative_words);

                if (negative_words.Count > 0)
                {
                    foreach (string word in negative_words)
                    {
                        List<Bowing> temp = DoFindBowings(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result = new List<Bowing>(source);
                        foreach (Bowing bowing in temp)
                        {
                            result.Remove(bowing);
                        }

                        source = new List<Bowing>(result);
                    }
                }

                if (positive_words.Count > 0)
                {
                    foreach (string word in positive_words)
                    {
                        List<Bowing> temp = DoFindBowings(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Bowing bowing in temp)
                        {
                            if (!result.Contains(bowing))
                            {
                                result.Add(bowing);
                            }
                        }

                        source = new List<Bowing>(result);
                    }
                }

                if (unsigned_words.Count > 0)
                {
                    foreach (string word in unsigned_words)
                    {
                        List<Bowing> temp = DoFindBowings(source, word, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);

                        result.Clear();
                        foreach (Bowing bowing in temp)
                        {
                            if (!result.Contains(bowing))
                            {
                                result.Add(bowing);
                            }
                        }

                        source = new List<Bowing>(result);
                    }
                }
            }
        }
        return result;
    }
    private static List<Bowing> DoFindBowings(List<Bowing> source, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Bowing> result = new List<Bowing>();
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
                            result = GetBowingsWithRootWords(source, root_words, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
                        }
                    }
                }
            }
            catch
            {
                // log exception
            }
        }
        return result;
    }
    private static List<Bowing> GetBowingsWithRootWords(List<Bowing> source, List<Word> root_words, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        List<Bowing> result = new List<Bowing>();
        if (source != null)
        {
            Dictionary<Bowing, int> multiplicity_dictionary = new Dictionary<Bowing, int>();
            foreach (Word word in root_words)
            {
                Bowing bowing = word.Verse.Bowing;
                if (multiplicity_dictionary.ContainsKey(bowing))
                {
                    multiplicity_dictionary[bowing]++;
                }
                else // first found
                {
                    multiplicity_dictionary.Add(bowing, 1);
                }
            }

            if (multiplicity == 0) // bowings not containg word
            {
                foreach (Bowing bowing in source)
                {
                    if (!multiplicity_dictionary.ContainsKey(bowing))
                    {
                        if (!result.Contains(bowing))
                        {
                            result.Add(bowing);
                        }
                    }
                }
            }
            else // add only matching multiplicity or wildcard (-1)
            {
                foreach (Word word in root_words)
                {
                    Bowing bowing = word.Verse.Bowing;
                    if (source.Contains(bowing))
                    {
                        if (multiplicity_dictionary.ContainsKey(bowing))
                        {
                            if ((multiplicity == -1) || (Compare(multiplicity_dictionary[bowing], multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder)))
                            {
                                if (!result.Contains(bowing))
                                {
                                    result.Add(bowing);
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

    // find by similarity - phrases similar to given text
    public static List<Phrase> FindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, double similarity_percentage)
    {
        List<Phrase> result = new List<Phrase>();
        List<Verse> found_verses = previous_result;

        while (text.Contains("  "))
        {
            text = text.Replace("  ", " ");
        }
        while (text.Contains("+"))
        {
            text = text.Replace("+", "");
        }
        while (text.Contains("-"))
        {
            text = text.Replace("-", "");
        }

        string[] word_texts = text.Split();
        if (word_texts.Length == 0)
        {
            return result;
        }
        else if (word_texts.Length == 1)
        {
            return DoFindPhrases(search_scope, current_selection, previous_result, text, similarity_percentage);
        }
        else if (word_texts.Length > 1) // enable nested searches
        {
            if (text.Length > 1) // enable nested searches
            {
                List<Phrase> phrases = null;
                List<Verse> verses = null;

                foreach (string word_text in word_texts)
                {
                    phrases = DoFindPhrases(search_scope, current_selection, found_verses, word_text, similarity_percentage);
                    verses = new List<Verse>(GetVerses(phrases));

                    // if first result
                    if (found_verses == null)
                    {
                        // fill it up with a copy of the first similar word search result
                        result = new List<Phrase>(phrases);
                        found_verses = new List<Verse>(verses);

                        // prepare for nested search by search
                        search_scope = SearchScope.Result;
                    }
                    else // subsequent search result
                    {
                        found_verses = new List<Verse>(verses);

                        List<Phrase> union_phrases = new List<Phrase>(phrases);
                        foreach (Phrase phrase in result)
                        {
                            if (phrase != null)
                            {
                                if (verses.Contains(phrase.Verse))
                                {
                                    union_phrases.Add(phrase);
                                }
                            }
                        }
                        result = union_phrases;
                    }
                }
            }
        }
        return result;
    }
    private static List<Phrase> DoFindPhrases(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, string text, double similarity_percentage)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result, TextLocationInChapter.Any);
        return DoFindPhrases(source, current_selection, previous_result, text, similarity_percentage);
    }
    private static List<Phrase> DoFindPhrases(List<Verse> source, Selection current_selection, List<Verse> previous_result, string text, double similarity_percentage)
    {
        List<Phrase> result = new List<Phrase>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    foreach (Verse verse in source)
                    {
                        foreach (Word word in verse.Words)
                        {
                            if (word.Text.IsSimilarTo(text, similarity_percentage))
                            {
                                result.Add(new Phrase(verse, word.Position, word.Text));
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    // find by similarity - verses similar to given verse
    public static List<Verse> FindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, Verse verse, SimilarityMethod similarity_method, double similarity_percentage)
    {
        return DoFindVerses(search_scope, current_selection, previous_result, verse, similarity_method, similarity_percentage);
    }
    private static List<Verse> DoFindVerses(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, Verse verse, SimilarityMethod similarity_method, double similarity_percentage)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result, TextLocationInChapter.Any);
        return DoFindVerses(source, current_selection, previous_result, verse, similarity_method, similarity_percentage);
    }
    private static List<Verse> DoFindVerses(List<Verse> source, Selection current_selection, List<Verse> previous_result, Verse verse, SimilarityMethod find_similarity_method, double similarity_percentage)
    {
        List<Verse> result = new List<Verse>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (verse != null)
                {
                    switch (find_similarity_method)
                    {
                        case SimilarityMethod.SimilarText:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.IsSimilarTo(source[j].Text, similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        case SimilarityMethod.SimilarWords:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.HasSimilarWordsTo(source[j].Text, (int)Math.Round((Math.Min(verse.Words.Count, source[j].Words.Count) * similarity_percentage)), similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        case SimilarityMethod.SimilarFirstHalf:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.HasSimilarFirstHalfTo(source[j].Text, similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        case SimilarityMethod.SimilarLastHalf:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.HasSimilarLastHalfTo(source[j].Text, similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        case SimilarityMethod.SimilarFirstWord:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.HasSimilarFirstWordTo(source[j].Text, similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        case SimilarityMethod.SimilarLastWord:
                            {
                                for (int j = 0; j < source.Count; j++)
                                {
                                    if (verse.Text.HasSimilarLastWordTo(source[j].Text, similarity_percentage))
                                    {
                                        result.Add(source[j]);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        return result;
    }
    // find by similarity - all similar verses to each other throughout the book
    public static List<List<Verse>> FindVersess(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, SimilarityMethod similarity_method, double similarity_percentage)
    {
        return DoFindVersess(search_scope, current_selection, previous_result, similarity_method, similarity_percentage);
    }
    private static List<List<Verse>> DoFindVersess(SearchScope search_scope, Selection current_selection, List<Verse> previous_result, SimilarityMethod similarity_method, double similarity_percentage)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_result, TextLocationInChapter.Any);
        return DoFindVersess(source, current_selection, previous_result, similarity_method, similarity_percentage);
    }
    private static List<List<Verse>> DoFindVersess(List<Verse> source, Selection current_selection, List<Verse> previous_result, SimilarityMethod find_similarity_method, double similarity_percentage)
    {
        List<List<Verse>> result = new List<List<Verse>>();
        Dictionary<Verse, List<Verse>> verse_ranges = new Dictionary<Verse, List<Verse>>(); // need dictionary to check if key exist
        bool[] already_compared = new bool[source.Count];
        if (source != null)
        {
            if (source.Count > 0)
            {
                switch (find_similarity_method)
                {
                    case SimilarityMethod.SimilarText:
                        {
                            for (int i = 0; i < source.Count - 1; i++)
                            {
                                for (int j = i + 1; j < source.Count; j++)
                                {
                                    if (!already_compared[j])
                                    {
                                        if (source[i].Text.IsSimilarTo(source[j].Text, similarity_percentage))
                                        {
                                            if (!verse_ranges.ContainsKey(source[i])) // first time matching verses found
                                            {
                                                List<Verse> similar_verses = new List<Verse>();
                                                verse_ranges.Add(source[i], similar_verses);
                                                similar_verses.Add(source[i]);
                                                similar_verses.Add(source[j]);
                                                already_compared[i] = true;
                                                already_compared[j] = true;
                                            }
                                            else // matching verses already exists
                                            {
                                                List<Verse> similar_verses = verse_ranges[source[i]];
                                                similar_verses.Add(source[j]);
                                                already_compared[j] = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case SimilarityMethod.SimilarWords:
                        {
                            for (int i = 0; i < source.Count - 1; i++)
                            {
                                for (int j = i + 1; j < source.Count; j++)
                                {
                                    if (!already_compared[j])
                                    {
                                        if (source[i].Text.HasSimilarWordsTo(source[j].Text, (int)Math.Round((Math.Min(source[i].Words.Count, source[j].Words.Count) * similarity_percentage)), similarity_percentage))
                                        {
                                            if (!verse_ranges.ContainsKey(source[i])) // first time matching verses found
                                            {
                                                List<Verse> similar_verses = new List<Verse>();
                                                verse_ranges.Add(source[i], similar_verses);
                                                similar_verses.Add(source[i]);
                                                similar_verses.Add(source[j]);
                                                already_compared[i] = true;
                                                already_compared[j] = true;
                                            }
                                            else // matching verses already exists
                                            {
                                                List<Verse> similar_verses = verse_ranges[source[i]];
                                                similar_verses.Add(source[j]);
                                                already_compared[j] = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case SimilarityMethod.SimilarFirstWord:
                        {
                            for (int i = 0; i < source.Count - 1; i++)
                            {
                                for (int j = i + 1; j < source.Count; j++)
                                {
                                    if (!already_compared[j])
                                    {
                                        if (source[j].Text.HasSimilarFirstWordTo(source[j].Text, similarity_percentage))
                                        {
                                            if (!verse_ranges.ContainsKey(source[i])) // first time matching verses found
                                            {
                                                List<Verse> similar_verses = new List<Verse>();
                                                verse_ranges.Add(source[i], similar_verses);
                                                similar_verses.Add(source[i]);
                                                similar_verses.Add(source[j]);
                                                already_compared[i] = true;
                                                already_compared[j] = true;
                                            }
                                            else // matching verses already exists
                                            {
                                                List<Verse> similar_verses = verse_ranges[source[i]];
                                                similar_verses.Add(source[j]);
                                                already_compared[j] = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case SimilarityMethod.SimilarLastWord:
                        {
                            for (int i = 0; i < source.Count - 1; i++)
                            {
                                for (int j = i + 1; j < source.Count; j++)
                                {
                                    if (!already_compared[j])
                                    {
                                        if (source[i].Text.HasSimilarLastWordTo(source[j].Text, similarity_percentage))
                                        {
                                            if (!verse_ranges.ContainsKey(source[i])) // first time matching verses found
                                            {
                                                List<Verse> similar_verses = new List<Verse>();
                                                verse_ranges.Add(source[i], similar_verses);
                                                similar_verses.Add(source[i]);
                                                similar_verses.Add(source[j]);
                                                already_compared[i] = true;
                                                already_compared[j] = true;
                                            }
                                            else // matching verses already exists
                                            {
                                                List<Verse> similar_verses = verse_ranges[source[i]];
                                                similar_verses.Add(source[j]);
                                                already_compared[j] = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        // copy dictionary to list of list
        if (verse_ranges.Count > 0)
        {
            foreach (List<Verse> verse_range in verse_ranges.Values)
            {
                result.Add(verse_range);
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
    private static bool Compare(Letter letter, NumberQuery query)
    {
        if (letter != null)
        {
            int number = 0;
            switch (query.NumberScope)
            {
                case NumberScope.Number:
                    number = letter.Number;
                    break;
                case NumberScope.NumberInChapter:
                    number = letter.NumberInChapter;
                    break;
                case NumberScope.NumberInVerse:
                    number = letter.NumberInVerse;
                    break;
                case NumberScope.NumberInWord:
                    number = letter.NumberInWord;
                    break;
                default:
                    number = letter.NumberInWord;
                    break;
            }
            if (query.NumberNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(number, number, query.NumberComparisonOperator, query.NumberRemainder))
                {
                    return false;
                }
            }
            else if (query.NumberNumberType == NumberType.None)
            {
                if (query.Number != 0)
                {
                    if (query.Number < 0)
                    {
                        switch (query.NumberScope)
                        {
                            case NumberScope.Number:
                                query.Number = letter.Word.Verse.Chapter.Book.LetterCount + query.Number + 1;
                                break;
                            case NumberScope.NumberInChapter:
                                query.Number = letter.Word.Verse.Chapter.LetterCount + query.Number + 1;
                                break;
                            case NumberScope.NumberInVerse:
                                query.Number = letter.Word.Verse.LetterCount + query.Number + 1;
                                break;
                            case NumberScope.NumberInWord:
                                query.Number = letter.Word.Letters.Count + query.Number + 1;
                                break;
                            default:
                                query.Number = letter.Word.Letters.Count + query.Number + 1;
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
                    else
                    {
                        return false; // number_out_of_range
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

            long value = CalculateValue(letter);
            if (query.ValueNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value, number, query.ValueComparisonOperator, query.ValueRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueNumberType == NumberType.None)
            {
                if (query.Value != 0)
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

            int value_digit_sum = Numbers.DigitSum(value);
            if (query.ValueDigitSumNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digit_sum, number, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitSumNumberType == NumberType.None)
            {
                if (query.ValueDigitSum != 0)
                {
                    if (!Numbers.Compare(value_digit_sum, query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digit_sum, query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            int value_digital_root = Numbers.DigitalRoot(value);
            if (query.ValueDigitalRootNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digital_root, number, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitalRootNumberType == NumberType.None)
            {
                if (query.ValueDigitalRoot != 0)
                {
                    if (!Numbers.Compare(value_digital_root, query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digital_root, query.ValueDigitalRootNumberType))
                {
                    return false;
                }
            }
        }

        // passed all tests successfully
        return true;
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
            if (query.NumberNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(number, number, query.NumberComparisonOperator, query.NumberRemainder))
                {
                    return false;
                }
            }
            else if (query.NumberNumberType == NumberType.None)
            {
                if (query.Number != 0)
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
                                query.Number = word.Verse.Words.Count + query.Number + 1;
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
                    else
                    {
                        return false; // number_out_of_range
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
                if (!Numbers.Compare(word.UniqueLetters.Count, number, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
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
            if (query.ValueNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value, number, query.ValueComparisonOperator, query.ValueRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueNumberType == NumberType.None)
            {
                if (query.Value != 0)
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

            int value_digit_sum = Numbers.DigitSum(value);
            if (query.ValueDigitSumNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digit_sum, number, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitSumNumberType == NumberType.None)
            {
                if (query.ValueDigitSum != 0)
                {
                    if (!Numbers.Compare(value_digit_sum, query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digit_sum, query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            int value_digital_root = Numbers.DigitalRoot(value);
            if (query.ValueDigitalRootNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digital_root, number, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitalRootNumberType == NumberType.None)
            {
                if (query.ValueDigitalRoot != 0)
                {
                    if (!Numbers.Compare(value_digital_root, query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digital_root, query.ValueDigitalRootNumberType))
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
            if (query.NumberNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(sum, sum, query.NumberComparisonOperator, query.NumberRemainder))
                {
                    return false;
                }
            }
            else if (query.NumberNumberType == NumberType.None)
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

            sum = 0;
            foreach (Word word in words)
            {
                sum += word.Letters.Count;
            }
            if (query.LetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(sum, sum, query.LetterCountComparisonOperator, query.LetterCountRemainder))
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
                        CalculateSums(words, out word_sum, out letter_sum);
                        if (!Numbers.Compare(letter_sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
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
                    if (!Numbers.IsNumberType(sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
            }

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
            if (query.UniqueLetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(unique_letters.Count, sum, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                {
                    return false;
                }
            }
            else if (query.UniqueLetterCountNumberType == NumberType.None)
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(unique_letters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(unique_letters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if (value == 0L)
            {
                foreach (Word word in words)
                {
                    value += CalculateValue(word);
                }
            }
            if (query.ValueNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value, sum, query.ValueComparisonOperator, query.ValueRemainder))
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

            int value_digit_sum = Numbers.DigitSum(value);
            if (query.ValueDigitSumNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digit_sum, sum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitSumNumberType == NumberType.None)
            {
                if (query.ValueDigitSum != 0)
                {
                    if (!Numbers.Compare(value_digit_sum, query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digit_sum, query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }


            int value_digital_root = Numbers.DigitalRoot(value);
            if (query.ValueDigitalRootNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digital_root, sum, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitalRootNumberType == NumberType.None)
            {
                if (query.ValueDigitalRoot != 0)
                {
                    if (!Numbers.Compare(value_digital_root, query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digital_root, query.ValueDigitalRootNumberType))
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

            if (query.WordCountNumberType == NumberType.Natural)
            {
                return false;
            }
            else if (query.WordCountNumberType == NumberType.None)
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

            if (query.LetterCountNumberType == NumberType.Natural)
            {
                return false;
            }
            else if (query.LetterCountNumberType == NumberType.None)
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

            if (query.UniqueLetterCountNumberType == NumberType.Natural)
            {
                return false;
            }
            else if (query.UniqueLetterCountNumberType == NumberType.None)
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

            if (query.ValueNumberType == NumberType.Natural)
            {
                return false;
            }
            else if (query.ValueNumberType == NumberType.None)
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

            if (query.ValueDigitSumNumberType == NumberType.Natural)
            {
                return false;
            }
            else if (query.ValueDigitSumNumberType == NumberType.None)
            {
                if (query.ValueDigitSum > 0)
                {
                    if (value == 0L) { value = CalculateValue(sentence); }
                    int value_digit_sum = Numbers.DigitSum(value);
                    if (!Numbers.Compare(value_digit_sum, query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(sentence); }
                int value_digit_sum = Numbers.DigitSum(value);
                if (!Numbers.IsNumberType(value_digit_sum, query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            if (query.ValueDigitalRootNumberType == NumberType.Natural)
            {
                return false;
            }
            else if (query.ValueDigitalRootNumberType == NumberType.None)
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (value == 0L) { value = CalculateValue(sentence); }
                    int value_digit_sum = Numbers.DigitSum(value);
                    if (!Numbers.Compare(value_digit_sum, query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (value == 0L) { value = CalculateValue(sentence); }
                int value_digit_sum = Numbers.DigitSum(value);
                if (!Numbers.IsNumberType(value_digit_sum, query.ValueDigitalRootNumberType))
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
            if (query.NumberNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(number, number, query.NumberComparisonOperator, query.NumberRemainder))
                {
                    return false;
                }
            }
            else if (query.NumberNumberType == NumberType.None)
            {
                if (query.Number != 0)
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

                    if (query.Number > 0)
                    {
                        if (!Numbers.Compare(number, query.Number, query.NumberComparisonOperator, query.NumberRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false; // number_out_of_range
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
                if (!Numbers.IsNumberType(value, query.ValueNumberType))
                {
                    return false;
                }
            }

            int value_digit_sum = Numbers.DigitSum(value);
            if (query.ValueDigitSumNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digit_sum, number, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitSumNumberType == NumberType.None)
            {
                if (query.ValueDigitSum > 0)
                {
                    if (!Numbers.Compare(value_digit_sum, query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digit_sum, query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            int value_digital_root = Numbers.DigitalRoot(value);
            if (query.ValueDigitalRootNumberType == NumberType.Natural)
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (!Numbers.Compare(value_digital_root, number, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else if (query.ValueDigitalRootNumberType == NumberType.None)
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (!Numbers.Compare(value_digital_root, query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digital_root, query.ValueDigitalRootNumberType))
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
            if (query.NumberNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(sum, sum, query.NumberComparisonOperator, query.NumberRemainder))
                {
                    return false;
                }
            }
            else if (query.NumberNumberType == NumberType.None)
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

            sum = 0;
            foreach (Verse verse in verses)
            {
                sum += verse.Words.Count;
            }
            if (query.WordCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(sum, sum, query.WordCountComparisonOperator, query.WordCountRemainder))
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
                        int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                        CalculateSums(verses, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                        if (!Numbers.Compare(word_sum, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
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
                    if (!Numbers.IsNumberType(sum, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
            }

            sum = 0;
            foreach (Verse verse in verses)
            {
                sum += verse.LetterCount;
            }
            if (query.LetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(sum, sum, query.LetterCountComparisonOperator, query.LetterCountRemainder))
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
                        int chapter_sum; int verse_sum; int word_sum; int letter_sum;
                        CalculateSums(verses, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
                        if (!Numbers.Compare(letter_sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
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
                    if (!Numbers.IsNumberType(sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
            }

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
            if (query.UniqueLetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(unique_letters.Count, sum, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                {
                    return false;
                }
            }
            else if (query.UniqueLetterCountNumberType == NumberType.None)
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(unique_letters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(unique_letters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if (value == 0L)
            {
                foreach (Verse verse in verses)
                {
                    value += CalculateValue(verse);
                }
            }
            if (query.ValueNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value, sum, query.ValueComparisonOperator, query.ValueRemainder))
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


            int value_digit_sum = Numbers.DigitSum(value);
            if (query.ValueDigitSumNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digit_sum, sum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitSumNumberType == NumberType.None)
            {
                if (query.ValueDigitSum > 0)
                {
                    if (!Numbers.Compare(value_digit_sum, query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digit_sum, query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            int value_digital_root = Numbers.DigitalRoot(value);
            if (query.ValueDigitalRootNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digital_root, sum, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitalRootNumberType == NumberType.None)
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (!Numbers.Compare(value_digital_root, query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digital_root, query.ValueDigitalRootNumberType))
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
            if (query.NumberNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(number, number, query.NumberComparisonOperator, query.NumberRemainder))
                {
                    return false;
                }
            }
            else if (query.NumberNumberType == NumberType.None)
            {
                if (query.Number != 0)
                {
                    if (query.Number < 0)
                    {
                        switch (query.NumberScope)
                        {
                            case NumberScope.Number:
                                query.Number = Book.Chapters.Count + query.Number + 1;
                                break;
                            default:
                                query.Number = Book.Chapters.Count + query.Number + 1;
                                break;
                        }
                    }

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
                    else
                    {
                        return false; // number_out_of_range
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

            int value_digit_sum = Numbers.DigitSum(value);
            if (query.ValueDigitSumNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digit_sum, number, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitSumNumberType == NumberType.None)
            {
                if (query.ValueDigitSum > 0)
                {
                    if (!Numbers.Compare(value_digit_sum, query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digit_sum, query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            int value_digital_root = Numbers.DigitalRoot(value);
            if (query.ValueDigitalRootNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digital_root, number, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitalRootNumberType == NumberType.None)
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (!Numbers.Compare(value_digital_root, query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digital_root, query.ValueDigitalRootNumberType))
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
            if (query.NumberNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(sum, sum, query.NumberComparisonOperator, query.NumberRemainder))
                {
                    return false;
                }
            }
            else if (query.NumberNumberType == NumberType.None)
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

            int chapter_sum; int verse_sum; int word_sum; int letter_sum;
            CalculateSums(chapters, out chapter_sum, out verse_sum, out word_sum, out letter_sum);
            sum = 0;
            foreach (Chapter chapter in chapters)
            {
                sum += chapter.Verses.Count;
            }
            if (query.VerseCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(verse_sum, sum, query.VerseCountComparisonOperator, query.VerseCountRemainder))
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
                        if (!Numbers.Compare(verse_sum, query.VerseCount, query.VerseCountComparisonOperator, query.VerseCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
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
                    if (!Numbers.IsNumberType(verse_sum, query.VerseCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Numbers.IsNumberType(sum, query.VerseCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if (query.WordCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(word_sum, sum, query.WordCountComparisonOperator, query.WordCountRemainder))
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
                        if (!Numbers.Compare(word_sum, query.WordCount, query.WordCountComparisonOperator, query.WordCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
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
                    if (!Numbers.IsNumberType(word_sum, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Numbers.IsNumberType(sum, query.WordCountNumberType))
                    {
                        return false;
                    }
                }
            }

            if (query.LetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(letter_sum, sum, query.LetterCountComparisonOperator, query.LetterCountRemainder))
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
                        if (!Numbers.Compare(letter_sum, query.LetterCount, query.LetterCountComparisonOperator, query.LetterCountRemainder))
                        {
                            return false;
                        }
                    }
                    else
                    {
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
                    if (!Numbers.IsNumberType(letter_sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!Numbers.IsNumberType(sum, query.LetterCountNumberType))
                    {
                        return false;
                    }
                }
            }

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
            if (query.UniqueLetterCountNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(unique_letters.Count, sum, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                {
                    return false;
                }
            }
            else if (query.UniqueLetterCountNumberType == NumberType.None)
            {
                if (query.UniqueLetterCount > 0)
                {
                    if (!Numbers.Compare(unique_letters.Count, query.UniqueLetterCount, query.UniqueLetterCountComparisonOperator, query.UniqueLetterCountRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(unique_letters.Count, query.UniqueLetterCountNumberType))
                {
                    return false;
                }
            }

            if (value == 0L)
            {
                foreach (Chapter chapter in chapters)
                {
                    value += CalculateValue(chapter);
                }
            }
            if (query.ValueNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value, sum, query.ValueComparisonOperator, query.ValueRemainder))
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

            int value_digit_sum = Numbers.DigitSum(value);
            if (query.ValueDigitSumNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digit_sum, sum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitSumNumberType == NumberType.None)
            {
                if (query.ValueDigitSum > 0)
                {
                    if (!Numbers.Compare(value_digit_sum, query.ValueDigitSum, query.ValueDigitSumComparisonOperator, query.ValueDigitSumRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digit_sum, query.ValueDigitSumNumberType))
                {
                    return false;
                }
            }

            int value_digital_root = Numbers.DigitalRoot(value);
            if (query.ValueDigitalRootNumberType == NumberType.Natural)
            {
                if (!Numbers.Compare(value_digital_root, sum, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                {
                    return false;
                }
            }
            else if (query.ValueDigitalRootNumberType == NumberType.None)
            {
                if (query.ValueDigitalRoot > 0)
                {
                    if (!Numbers.Compare(value_digital_root, query.ValueDigitalRoot, query.ValueDigitalRootComparisonOperator, query.ValueDigitalRootRemainder))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!Numbers.IsNumberType(value_digital_root, query.ValueDigitalRootNumberType))
                {
                    return false;
                }
            }

        }

        // passed all tests successfully
        return true;
    }
    // find by numbers - Letters
    public static List<Letter> FindLetters(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindLetters(search_scope, current_selection, previous_verses, query);
    }
    private static List<Letter> DoFindLetters(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindLetters(source, query);
    }
    private static List<Letter> DoFindLetters(List<Verse> source, NumberQuery query)
    {
        List<Letter> result = new List<Letter>();
        if (source != null)
        {
            foreach (Verse verse in source)
            {
                foreach (Word word in verse.Words)
                {
                    foreach (Letter letter in word.Letters)
                    {
                        if (Compare(letter, query))
                        {
                            result.Add(letter);
                        }
                    }
                }
            }
        }
        return result;
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
        return DoFindWordRanges(source, query);
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
                    limit = (int)(query.Value / 7L);
                }
                if (limit == 0) limit = 1;

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
    // find by numbers - WordSets
    public static List<List<Word>> FindWordSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindWordSets(search_scope, current_selection, previous_verses, query);
    }
    private static List<List<Word>> DoFindWordSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindWordSets(source, query);
    }
    private static List<List<Word>> DoFindWordSets(List<Verse> source, NumberQuery query)
    {
        List<List<Word>> result = new List<List<Word>>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    List<Word> words = new List<Word>();
                    if (words != null)
                    {
                        int set_size = query.WordCount;
                        if (set_size == 1)
                        {
                            result.Add(DoFindWords(source, query));
                            return result;
                        }

                        foreach (Verse verse in source)
                        {
                            words.AddRange(verse.Words);
                        }

                        if (set_size == 0) // non-specified set size
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

                            for (int i = 0; i < limit; i++) // try all possible set sizes
                            {
                                int size = i + 1;
                                Combinations<Word> sets = new Combinations<Word>(words, size, GenerateOption.WithoutRepetition);
                                foreach (List<Word> set in sets)
                                {
                                    // check set against query
                                    if (Compare(set, query))
                                    {
                                        result.Add(set);
                                    }
                                }
                            }
                        }
                        else // specified set size
                        {
                            Combinations<Word> sets = new Combinations<Word>(words, set_size, GenerateOption.WithoutRepetition);
                            foreach (List<Word> set in sets)
                            {
                                // check set against query
                                if (Compare(set, query))
                                {
                                    result.Add(set);
                                }
                            }
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
            foreach (Verse verse in source)
            {
                if (Compare(verse, query))
                {
                    result.Add(verse);
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
                    limit = (int)(query.Value / 114L);
                }
                if (limit == 0) limit = 1;

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
    // find by numbers - VerseSets
    public static List<List<Verse>> FindVerseSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindVerseSets(search_scope, current_selection, previous_verses, query);
    }
    private static List<List<Verse>> DoFindVerseSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindVerseSets(source, query);
    }
    private static List<List<Verse>> DoFindVerseSets(List<Verse> source, NumberQuery query)
    {
        List<List<Verse>> result = new List<List<Verse>>();
        if (source != null)
        {
            if (source.Count > 0)
            {
                if (s_book != null)
                {
                    List<Verse> verses = source;
                    if (verses != null)
                    {
                        int set_size = query.VerseCount;
                        if (set_size == 1)
                        {
                            result.Add(DoFindVerses(source, query));
                            return result;
                        }

                        if (set_size == 0) // non-specified set size
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

                            for (int i = 0; i < limit; i++) // try all possible set sizes
                            {
                                int size = i + 1;
                                Combinations<Verse> sets = new Combinations<Verse>(verses, size, GenerateOption.WithoutRepetition);
                                foreach (List<Verse> set in sets)
                                {
                                    // check set against query
                                    if (Compare(set, query))
                                    {
                                        result.Add(set);
                                    }
                                }
                            }
                        }
                        else // specified set size
                        {
                            Combinations<Verse> sets = new Combinations<Verse>(verses, set_size, GenerateOption.WithoutRepetition);
                            foreach (List<Verse> set in sets)
                            {
                                // check set against query
                                if (Compare(set, query))
                                {
                                    result.Add(set);
                                }
                            }
                        }
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
                            //    limit = (int)(query.Value / 506L);
                            //}
                            //if (limit == 0) limit = 1;

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
    // find by numbers - ChapterSets
    public static List<List<Chapter>> FindChapterSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        return DoFindChapterSets(search_scope, current_selection, previous_verses, query);
    }
    private static List<List<Chapter>> DoFindChapterSets(SearchScope search_scope, Selection current_selection, List<Verse> previous_verses, NumberQuery query)
    {
        List<Verse> source = GetSourceVerses(search_scope, current_selection, previous_verses, TextLocationInChapter.Any);
        return DoFindChapterSets(source, query);
    }
    private static List<List<Chapter>> DoFindChapterSets(List<Verse> source, NumberQuery query)
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
                        int set_size = query.ChapterCount;
                        if (set_size == 1)
                        {
                            result.Add(DoFindChapters(source, query));
                            return result;
                        }

                        if (set_size == 0) // non-specified set size
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

                            for (int i = 0; i < limit; i++) // try all possible set sizes
                            {
                                int size = i + 1;
                                Combinations<Chapter> sets = new Combinations<Chapter>(chapters, size, GenerateOption.WithoutRepetition);
                                foreach (List<Chapter> set in sets)
                                {
                                    // check set against query
                                    if (Compare(set, query))
                                    {
                                        result.Add(set);
                                    }
                                }
                            }
                        }
                        else // specified set size
                        {
                            Combinations<Chapter> sets = new Combinations<Chapter>(chapters, set_size, GenerateOption.WithoutRepetition);
                            foreach (List<Chapter> set in sets)
                            {
                                // check set against query
                                if (Compare(set, query))
                                {
                                    result.Add(set);
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
            text = text.Replace(Constants.ORNATE_RIGHT_PARENTHESIS, "");
            text = text.Replace(Constants.ORNATE_LEFT_PARENTHESIS, "");
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
                phrase = phrase.Replace(Constants.ORNATE_RIGHT_PARENTHESIS, "");
                phrase = phrase.Replace(Constants.ORNATE_LEFT_PARENTHESIS, "");
                foreach (char character in Constants.INDIAN_DIGITS)
                {
                    phrase = phrase.Replace(character.ToString(), "");
                }

                if (frequency_search_type == FrequencySearchType.UniqueLetters)
                {
                    phrase = phrase.SimplifyTo(s_numerology_system.TextMode).RemoveDuplicates();
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

    public static string GetTranslationKey(string translation)
    {
        string result = null;
        if (s_book != null)
        {
            if (s_book.TranslationInfos != null)
            {
                foreach (string key in s_book.TranslationInfos.Keys)
                {
                    if (s_book.TranslationInfos[key].Name == translation)
                    {
                        result = key;
                    }
                }
            }
        }
        return result;
    }
    public static void LoadTranslation(string translation)
    {
        DataAccess.LoadTranslation(s_book, translation);
    }
    public static void UnloadTranslation(string translation)
    {
        DataAccess.UnloadTranslation(s_book, translation);
    }
    public static void SaveTranslation(string translation)
    {
        DataAccess.SaveTranslation(s_book, translation);
    }

    // help messages
    private static List<string> s_help_messages = new List<string>();
    public static List<string> HelpMessages
    {
        get { return s_help_messages; }
    }
    private static void LoadHelpMessages()
    {
        string filename = Globals.HELP_FOLDER + "/" + "Messages.txt";
        if (File.Exists(filename))
        {
            s_help_messages = FileHelper.LoadLines(filename);
        }
    }
}
