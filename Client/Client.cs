using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Model;

public class Client : IPublisher, ISubscriber
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
    // ISubscriber method
    public void Notify(Subject subject, FileSystemEventArgs e)
    {
        // notify subscribers
        NotifySubscribers(subject, e);
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
                    if (item != null)
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
    }
    ///////////////////////////////////////////////////////////////////////////////
    #endregion

    public const string DEFAULT_RECITATION = Server.DEFAULT_RECITATION;

    public const string DEFAULT_EMLAAEI_TEXT = Server.DEFAULT_EMLAAEI_TEXT;
    public const string DEFAULT_TRANSLATION = Server.DEFAULT_TRANSLATION;
    public const string DEFAULT_OLD_TRANSLATION = Server.DEFAULT_OLD_TRANSLATION;
    public const string DEFAULT_TRANSLITERATION = Server.DEFAULT_TRANSLITERATION;
    public const string DEFAULT_WORD_MEANINGS = Server.DEFAULT_WORD_MEANINGS;
    public const string DEFAULT_TRANSLATION_1 = Server.DEFAULT_TRANSLATION_1;
    public const string DEFAULT_TRANSLATION_2 = Server.DEFAULT_TRANSLATION_2;
    public const string DEFAULT_TRANSLATION_3 = Server.DEFAULT_TRANSLATION_3;
    public const string DEFAULT_TRANSLATION_4 = Server.DEFAULT_TRANSLATION_4;
    public const string DEFAULT_TRANSLATION_5 = Server.DEFAULT_TRANSLATION_5;
    public const string DEFAULT_TRANSLATION_6 = Server.DEFAULT_TRANSLATION_6;
    public const string DEFAULT_TRANSLATION_7 = Server.DEFAULT_TRANSLATION_7;

    public Client(string numerology_system_name)
    {
        if (!Directory.Exists(Globals.BOOKMARKS_FOLDER))
        {
            Directory.CreateDirectory(Globals.BOOKMARKS_FOLDER);
        }

        if (!Directory.Exists(Globals.HISTORY_FOLDER))
        {
            Directory.CreateDirectory(Globals.HISTORY_FOLDER);
        }

        // notify me of all event types
        Server server = new Server();
        if (server != null)
        {
            server.Subscribe(this, Subject.LanguageSystem);
            server.Subscribe(this, Subject.SimplificationSystem);
            server.Subscribe(this, Subject.NumerologySystem);
            server.Subscribe(this, Subject.DNASequenceSystem);
            server.Subscribe(this, Subject.InterestingNumbers);
            server.WatchFolder(Globals.LANGUAGES_FOLDER, "*.txt");
            server.WatchFolder(Globals.RULES_FOLDER, "*.txt");
            server.WatchFolder(Globals.VALUES_FOLDER, "*.txt");
            server.WatchFolder(Globals.NUMBERS_FOLDER, "*.txt");
        }

        // load and set initial NumerologySystem
        LoadNumerologySystem(numerology_system_name);
    }

    // current book
    public Book Book
    {
        get { return Server.Book; }
    }
    // current simplification system
    public SimplificationSystem SimplificationSystem
    {
        get { return Server.SimplificationSystem; }
    }
    // all loaded simplification systems
    public static Dictionary<string, SimplificationSystem> LoadedSimplificationSystems
    {
        get { return Server.LoadedSimplificationSystems; }
    }
    public void BuildSimplifiedBook(string text_mode, bool with_bism_Allah, bool waw_as_word, bool shadda_as_letter, bool hamza_above_horizontal_line_as_letter, bool elf_above_horizontal_line_as_letter, bool yaa_above_horizontal_line_as_letter, bool noon_above_horizontal_line_as_letter, bool emlaaei_text)
    {
        Server.BuildSimplifiedBook(text_mode, with_bism_Allah, waw_as_word, shadda_as_letter, hamza_above_horizontal_line_as_letter, elf_above_horizontal_line_as_letter, yaa_above_horizontal_line_as_letter, noon_above_horizontal_line_as_letter, emlaaei_text);
        UpdatePhrasePositionsAndLengths(text_mode);
    }

    // current numerology system
    public NumerologySystem NumerologySystem
    {
        get { return Server.NumerologySystem; }
        set { Server.NumerologySystem = value; }
    }
    // all loaded numerology systems
    public Dictionary<string, NumerologySystem> LoadedNumerologySystems
    {
        get { return Server.LoadedNumerologySystems; }
    }
    // update current numerology system
    public void UpdateNumerologySystem(string text)
    {
        Server.UpdateNumerologySystem(text);
    }
    private void UpdatePhrasePositionsAndLengths(string text_mode)
    {
        if (Book != null)
        {
            // update Selection to point at new book object
            if (m_selection != null)
            {
                m_selection = new Selection(Book, m_selection.Scope, m_selection.Indexes);
            }

            if (m_selection != null)
            {
                if (NumerologySystem != null)
                {
                    // update FoundVerses to point at new book object
                    if (Book.Verses != null)
                    {
                        if (m_found_verses != null)
                        {
                            List<Verse> verses = new List<Verse>();
                            foreach (Verse verse in m_found_verses)
                            {
                                int index = verse.Number - 1;
                                if ((index >= 0) && (index < Book.Verses.Count))
                                {
                                    verses.Add(Book.Verses[index]);
                                }
                            }
                            m_found_verses = verses;
                        }
                    }

                    // update FoundPhrases to point at new book object
                    if (Book.Verses != null)
                    {
                        if (m_found_phrases != null)
                        {
                            for (int i = 0; i < m_found_phrases.Count; i++)
                            {
                                Phrase phrase = m_found_phrases[i];
                                if (phrase != null)
                                {
                                    int index = phrase.Verse.Number - 1;
                                    if ((index >= 0) && (index < Book.Verses.Count))
                                    {
                                        phrase = new Phrase(Book.Verses[index], phrase.Position, phrase.Text);
                                        m_found_phrases[i] = Server.SwitchTextMode(phrase, text_mode);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // ALSO should update these less used collections as they are already held by FoundVerses
            // update FoundVerseRanges to point at new book object
            // update FoundVerseSets to point at new book object
            // update FoundChapters to point at new book object
            // update FoundChapterRanges to point at new book object
            // update FoundChapterSets to point at new book object
        }
    }
    // load and replace current numerology system
    public void LoadNumerologySystem(string numerology_system_name)
    {
        Server.LoadNumerologySystem(numerology_system_name);
    }

    // current dna sequence system
    public DNASequenceSystem DNASequenceSystem
    {
        get { return Server.DNASequenceSystem; }
    }
    // all loaded dna sequence systems
    public Dictionary<string, DNASequenceSystem> LoadedDNASequenceSystems
    {
        get { return Server.LoadedDNASequenceSystems; }
    }
    public void LoadDNASequenceSystem(string dna_sequence_system_name)
    {
        Server.LoadDNASequenceSystem(dna_sequence_system_name);
    }

    // translations
    public string GetTranslationKey(string translation)
    {
        return Server.GetTranslationKey(translation);
    }
    public void LoadTranslation(string translation)
    {
        Server.LoadTranslation(translation);
    }
    public void UnloadTranslation(string translation)
    {
        Server.UnloadTranslation(translation);
    }
    public void SaveTranslation(string translation)
    {
        Server.SaveTranslation(translation);
    }

    public CalculationMode CalculationMode
    {
        get { return Server.CalculationMode; }
        set { Server.CalculationMode = value; }
    }
    public bool Logging = false;
    // used for non-Quran text
    public long CalculateValue(char character)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(character);
        }
        else
        {
            return Server.CalculateValue(character);
        }
    }
    public long CalculateValue(string text)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(text);
        }
        else
        {
            return Server.CalculateValue(text);
        }
    }
    // used for Quran text only
    public long CalculateValue(Letter letter)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(letter);
        }
        else
        {
            return Server.CalculateValue(letter);
        }
    }
    public long CalculateValue(List<Letter> letters)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(letters);
        }
        else
        {
            return Server.CalculateValue(letters);
        }
    }
    public long CalculateValue(Word word)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(word);
        }
        else
        {
            return Server.CalculateValue(word);
        }
    }
    public long CalculateValue(List<Word> words)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(words);
        }
        else
        {
            return Server.CalculateValue(words);
        }
    }
    public long CalculateValue(Verse verse)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(verse);
        }
        else
        {
            return Server.CalculateValue(verse);
        }
    }
    public long CalculateValue(List<Verse> verses)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(verses);
        }
        else
        {
            return Server.CalculateValue(verses);
        }
    }
    public long CalculateValue(Chapter chapter)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(chapter);
        }
        else
        {
            return Server.CalculateValue(chapter);
        }
    }
    public long CalculateValue(List<Chapter> chapters)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(chapters);
        }
        else
        {
            return Server.CalculateValue(chapters);
        }
    }
    public long CalculateValue(Book book)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(book);
        }
        else
        {
            return Server.CalculateValue(book);
        }
    }
    public long CalculateValue(List<Verse> verses, Letter start_letter, Letter end_letter)
    {
        if (Logging)
        {
            Server.ClearLog();
            return Server.CalculateValueWithLogging(verses, start_letter, end_letter);
        }
        else
        {
            return Server.CalculateValue(verses, start_letter, end_letter);
        }
    }
    public List<long> CalculateVerseValues(List<Verse> verses)
    {
        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            Server.ClearLog();
            long value = 0L;
            if (Logging)
            {
                Server.ClearLog();
                value = Server.CalculateValue(verse);
            }
            else
            {
                value = Server.CalculateValue(verse);
            }
            result.Add(value);
        }
        return result;
    }
    public List<long> CalculateWordValues(List<Verse> verses)
    {
        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            foreach (Word word in verse.Words)
            {
                Server.ClearLog();
                long value = 0L;
                if (Logging)
                {
                    Server.ClearLog();
                    value = Server.CalculateValue(word);
                }
                else
                {
                    value = Server.CalculateValue(word);
                }
                result.Add(value);
            }
        }
        return result;
    }
    public List<long> CalculateLetterValues(List<Verse> verses)
    {
        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            foreach (Word word in verse.Words)
            {
                foreach (Letter letter in word.Letters)
                {
                    Server.ClearLog();
                    long value = 0L;
                    if (Logging)
                    {
                        Server.ClearLog();
                        value = Server.CalculateValue(letter);
                    }
                    else
                    {
                        value = Server.CalculateValue(letter);
                    }
                    result.Add(value);
                }
            }
        }
        return result;
    }
    public long MaximumVerseValue
    {
        get
        {
            long result = 0L;
            foreach (Verse verse in Book.Verses)
            {
                long value = Server.CalculateValue(verse);
                if (result < value)
                {
                    result = value;
                }
            }
            return result;
        }
    }
    public long MaximumWordValue
    {
        get
        {
            long result = 0L;
            foreach (Verse verse in Book.Verses)
            {
                foreach (Word word in verse.Words)
                {
                    long value = Server.CalculateValue(word);
                    if (result < value)
                    {
                        result = value;
                    }
                }
            }
            return result;
        }
    }
    public long MaximumLetterValue
    {
        get
        {
            long result = 0L;
            if (NumerologySystem != null)
            {
                foreach (long value in NumerologySystem.Values)
                {
                    if (result < value)
                    {
                        result = value;
                    }
                }
            }
            return result;
        }
    }
    public void SaveValueCalculations(string filename, string text, bool verbose)
    {
        if (Directory.Exists(Globals.STATISTICS_FOLDER))
        {
            filename = Globals.STATISTICS_FOLDER + "/" + filename;
            try
            {
                if (NumerologySystem != null)
                {
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                    {
                        StringBuilder numbers = new StringBuilder();
                        foreach (int index in Selection.Indexes)
                        {
                            numbers.Append((index + 1).ToString() + ", ");
                        }
                        if (numbers.Length > 0)
                        {
                            numbers.Remove(numbers.Length - 2, 2);
                        }

                        if (verbose)
                        {
                            writer.WriteLine("------------------------------------------------------------------------------------------");
                            writer.WriteLine(NumerologySystem.Name);
                            writer.WriteLine("Selection = " + Selection.Scope.ToString() + " " + numbers.ToString());
                            writer.WriteLine("------------------------------------------------------------------------------------------");
                            writer.WriteLine(NumerologySystem.ToOverview());
                            writer.WriteLine();
                        }
                        writer.WriteLine("------------------------------------------------------------------------------------------");
                        writer.WriteLine(verbose ? "Text" : "Number Analysis");
                        writer.WriteLine("------------------------------------------------------------------------------------------");
                        writer.WriteLine(text);

                        if (verbose)
                        {
                            if (Logging)
                            {
                                writer.WriteLine("------------------------------------------------------------------------------------------------------------------------------------");
                                writer.WriteLine("Letter" + "\t" + "Value" + "\t" + "\t" + "\t" + "\t" + "L" + "\t" + "W" + "\t" + "V" + "\t" + "C" + "\t" + "L←" + "\t" + "W←" + "\t" + "V←" + "\t" + "C←" + "\t" + "L→" + "\t" + "W→" + "\t" + "V→" + "\t" + "C→");
                                writer.WriteLine("------------------------------------------------------------------------------------------------------------------------------------");
                                writer.WriteLine(Server.Log.ToString());
                                writer.WriteLine("------------------------------------------------------------------------------------------------------------------------------------");
                                writer.WriteLine("Sum" + "\t" + Server.ValueSum + "\t" + "\t" + "\t" + "\t" + Server.LSum + "\t" + Server.WSum + "\t" + Server.VSum + "\t" + Server.CSum + "\t" + Server.pLSum + "\t" + Server.pWSum + "\t" + Server.pVSum + "\t" + Server.pCSum + "\t" + Server.nLSum + "\t" + Server.nWSum + "\t" + Server.nVSum + "\t" + Server.nCSum);
                                writer.WriteLine("------------------------------------------------------------------------------------------------------------------------------------");
                                long total = Server.ValueSum + Server.LSum + Server.WSum + Server.VSum + Server.CSum + Server.pLSum + Server.pWSum + Server.pVSum + Server.pCSum + Server.nLSum + Server.nWSum + Server.nVSum + Server.nCSum;
                                writer.WriteLine("Total" + "\t" + total);
                                writer.WriteLine("------------------------------------------------------------------------------------------------------------------------------------");
                            }
                        }
                    }
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            FileHelper.DisplayFile(filename);
        }
    }
    public void SaveNumberIndexChain(string filename, long number, int chain_length, string text)
    {
        if (Directory.Exists(Globals.STATISTICS_FOLDER))
        {
            filename = Globals.STATISTICS_FOLDER + "/" + filename;
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine(number.ToString() + " with IndexChainLength = " + chain_length.ToString());
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine("Number\tTotal\tPC_L2R\tPC_R2L\tCP_L2R\tCP_R2L\tChain");
                    writer.WriteLine("-----------------------------------------------------");
                    writer.Write(text);
                    writer.WriteLine("-----------------------------------------------------");
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            FileHelper.DisplayFile(filename);
        }
    }
    public void SaveIndexChainLength(string filename, NumberType number_type, int chain_length, string text)
    {
        if (Directory.Exists(Globals.STATISTICS_FOLDER))
        {
            filename = Globals.STATISTICS_FOLDER + "/" + filename;
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine(number_type.ToString() + " numbers with IndexChainLength = " + chain_length.ToString());
                    writer.WriteLine("-----------------------------------------------------");
                    writer.WriteLine("Number\tTotal\tPC_L2R\tPC_R2L\tCP_L2R\tCP_R2L\tChain");
                    writer.WriteLine("-----------------------------------------------------");
                    writer.Write(text);
                    writer.WriteLine("-----------------------------------------------------");
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            FileHelper.DisplayFile(filename);
        }
    }

    private List<Chapter> m_filter_chapters = null;
    public List<Chapter> FilterChapters
    {
        set { m_filter_chapters = value; }
        get { return m_filter_chapters; }
    }
    private Selection m_selection = null;
    public Selection Selection
    {
        get
        {
            if (Book != null)
            {
                return m_selection;
            }
            return null;
        }
        set
        {
            if (Book != null)
            {
                m_selection = value;
            }
        }
    }
    private void ClearSelection()
    {
        if (Book != null)
        {
            if (m_selection != null)
            {
                m_selection = new Selection(Book, SelectionScope.Chapter, new List<int>() { 0 });
            }
        }
    }
    private SearchScope m_search_scope = SearchScope.Book;
    public SearchScope SearchScope
    {
        set { m_search_scope = value; }
        get { return m_search_scope; }
    }
    private List<Phrase> m_found_phrases = null;
    public List<Phrase> FoundPhrases
    {
        set { m_found_phrases = value; }
        get
        {
            if (m_found_phrases == null) return null;
            if (m_filter_chapters == null) return m_found_phrases;

            List<Phrase> filtered_found_phrases = new List<Phrase>();
            foreach (Phrase phrase in m_found_phrases)
            {
                if (phrase != null)
                {
                    if (phrase.Verse != null)
                    {
                        if (phrase.Verse.Chapter != null)
                        {
                            if (m_filter_chapters.Contains(phrase.Verse.Chapter))
                            {
                                filtered_found_phrases.Add(phrase);
                            }
                        }
                    }
                }
            }
            return filtered_found_phrases;
        }
    }
    private List<Letter> m_found_letters = null;
    public List<Letter> FoundLetters
    {
        set { m_found_letters = value; }
        get
        {
            if (m_found_letters == null) return null;
            if (m_filter_chapters == null) return m_found_letters;

            List<Letter> filtered_found_letters = new List<Letter>();
            foreach (Letter letter in m_found_letters)
            {
                if (letter.Word != null)
                {
                    if (letter.Word.Verse != null)
                    {
                        if (letter.Word.Verse.Chapter != null)
                        {
                            if (m_filter_chapters.Contains(letter.Word.Verse.Chapter))
                            {
                                filtered_found_letters.Add(letter);
                            }
                        }
                    }
                }
            }
            return filtered_found_letters;
        }
    }

    private List<Word> m_found_words = null;
    public List<Word> FoundWords
    {
        set { m_found_words = value; }
        get
        {
            if (m_found_words == null) return null;
            if (m_filter_chapters == null) return m_found_words;

            List<Word> filtered_found_words = new List<Word>();
            foreach (Word word in m_found_words)
            {
                if (word.Verse != null)
                {
                    if (word.Verse.Chapter != null)
                    {
                        if (m_filter_chapters.Contains(word.Verse.Chapter))
                        {
                            filtered_found_words.Add(word);
                        }
                    }
                }
            }
            return filtered_found_words;
        }
    }
    private List<List<Word>> m_found_word_ranges = null;
    public List<List<Word>> FoundWordRanges
    {
        set { m_found_word_ranges = value; }
        get
        {
            if (m_found_word_ranges == null) return null;
            if (m_filter_chapters == null) return m_found_word_ranges;

            List<List<Word>> filtered_found_word_ranges = new List<List<Word>>();
            foreach (List<Word> range in m_found_word_ranges)
            {
                bool valid_range = true;
                foreach (Word word in range)
                {
                    if (word.Verse != null)
                    {
                        if (word.Verse.Chapter != null)
                        {
                            if (!m_filter_chapters.Contains(word.Verse.Chapter))
                            {
                                valid_range = false;
                                break;
                            }
                        }
                    }
                }
                if (valid_range)
                {
                    filtered_found_word_ranges.Add(range);
                }
            }
            return filtered_found_word_ranges;
        }
    }
    private List<List<Word>> m_found_word_sets = null;
    public List<List<Word>> FoundWordSets
    {
        set { m_found_word_sets = value; }
        get
        {
            if (m_found_word_sets == null) return null;
            if (m_filter_chapters == null) return m_found_word_sets;

            List<List<Word>> filtered_found_word_sets = new List<List<Word>>();
            foreach (List<Word> set in m_found_word_sets)
            {
                bool valid_set = true;
                foreach (Word word in set)
                {
                    if (word.Verse != null)
                    {
                        if (word.Verse.Chapter != null)
                        {
                            if (!m_filter_chapters.Contains(word.Verse.Chapter))
                            {
                                valid_set = false;
                                break;
                            }
                        }
                    }
                }
                if (valid_set)
                {
                    filtered_found_word_sets.Add(set);
                }
            }
            return filtered_found_word_sets;
        }
    }

    private List<Sentence> m_found_sentences = null;
    public List<Sentence> FoundSentences
    {
        set { m_found_sentences = value; }
        get
        {
            if (m_found_sentences == null) return null;
            if (m_filter_chapters == null) return m_found_sentences;

            List<Sentence> filtered_found_sentences = new List<Sentence>();
            foreach (Sentence sentence in m_found_sentences)
            {
                if (sentence.FirstVerse != null)
                {
                    if (sentence.FirstVerse.Chapter != null)
                    {
                        if (m_filter_chapters.Contains(sentence.FirstVerse.Chapter))
                        {
                            filtered_found_sentences.Add(sentence);
                        }
                    }
                }
            }
            return filtered_found_sentences;
        }
    }

    private List<Verse> m_found_verses = null;
    public List<Verse> FoundVerses
    {
        set { m_found_verses = value; }
        get
        {
            if (m_found_verses == null) return null;
            if (m_filter_chapters == null) return m_found_verses;

            List<Verse> filtered_found_verses = new List<Verse>();
            foreach (Verse verse in m_found_verses)
            {
                if (verse != null)
                {
                    if (verse.Chapter != null)
                    {
                        if (m_filter_chapters.Contains(verse.Chapter))
                        {
                            filtered_found_verses.Add(verse);
                        }
                    }
                }
            }
            return filtered_found_verses;
        }
    }
    private List<List<Verse>> m_found_verse_ranges = null;
    public List<List<Verse>> FoundVerseRanges
    {
        set { m_found_verse_ranges = value; }
        get
        {
            if (m_found_verse_ranges == null) return null;
            if (m_filter_chapters == null) return m_found_verse_ranges;

            List<List<Verse>> filtered_found_verse_ranges = new List<List<Verse>>();
            foreach (List<Verse> range in m_found_verse_ranges)
            {
                bool valid_range = true;
                foreach (Verse verse in range)
                {
                    if (verse != null)
                    {
                        if (verse.Chapter != null)
                        {
                            if (!m_filter_chapters.Contains(verse.Chapter))
                            {
                                valid_range = false;
                                break;
                            }
                        }
                    }
                }
                if (valid_range)
                {
                    filtered_found_verse_ranges.Add(range);
                }
            }
            return filtered_found_verse_ranges;
        }
    }
    private List<List<Verse>> m_found_verse_sets = null;
    public List<List<Verse>> FoundVerseSets
    {
        set { m_found_verse_sets = value; }
        get
        {
            if (m_found_verse_sets == null) return null;
            if (m_filter_chapters == null) return m_found_verse_sets;

            List<List<Verse>> filtered_found_verse_sets = new List<List<Verse>>();
            foreach (List<Verse> set in m_found_verse_sets)
            {
                bool valid_set = true;
                foreach (Verse verse in set)
                {
                    if (verse != null)
                    {
                        if (verse.Chapter != null)
                        {
                            if (!m_filter_chapters.Contains(verse.Chapter))
                            {
                                valid_set = false;
                                break;
                            }
                        }
                    }
                }
                if (valid_set)
                {
                    filtered_found_verse_sets.Add(set);
                }
            }
            return filtered_found_verse_sets;
        }
    }

    private List<Chapter> m_found_chapters = null;
    public List<Chapter> FoundChapters
    {
        set { m_found_chapters = value; }
        get
        {
            if (m_found_chapters == null) return null;
            if (m_filter_chapters == null) return m_found_chapters;

            List<Chapter> filtered_found_chapters = new List<Chapter>();
            foreach (Chapter chapter in m_found_chapters)
            {
                if (chapter != null)
                {
                    if (m_filter_chapters.Contains(chapter))
                    {
                        filtered_found_chapters.Add(chapter);
                    }
                }
            }
            return filtered_found_chapters;
        }
    }
    private List<List<Chapter>> m_found_chapter_ranges = null;
    public List<List<Chapter>> FoundChapterRanges
    {
        set { m_found_chapter_ranges = value; }
        get
        {
            if (m_found_chapter_ranges == null) return null;
            if (m_filter_chapters == null) return m_found_chapter_ranges;

            List<List<Chapter>> filtered_found_chapter_ranges = new List<List<Chapter>>();
            foreach (List<Chapter> range in m_found_chapter_ranges)
            {
                bool valid_range = true;
                foreach (Chapter chapter in range)
                {
                    if (chapter != null)
                    {
                        if (!m_filter_chapters.Contains(chapter))
                        {
                            valid_range = false;
                            break;
                        }
                    }
                }
                if (valid_range)
                {
                    filtered_found_chapter_ranges.Add(range);
                }
            }
            return filtered_found_chapter_ranges;
        }
    }
    private List<List<Chapter>> m_found_chapter_sets = null;
    public List<List<Chapter>> FoundChapterSets
    {
        set { m_found_chapter_sets = value; }
        get
        {
            if (m_found_chapter_sets == null) return null;
            if (m_filter_chapters == null) return m_found_chapter_sets;

            List<List<Chapter>> filtered_found_chapter_sets = new List<List<Chapter>>();
            foreach (List<Chapter> set in m_found_chapter_sets)
            {
                bool valid_set = true;
                foreach (Chapter chapter in set)
                {
                    if (chapter != null)
                    {
                        if (!m_filter_chapters.Contains(chapter))
                        {
                            valid_set = false;
                            break;
                        }
                    }
                }
                if (valid_set)
                {
                    filtered_found_chapter_sets.Add(set);
                }
            }
            return filtered_found_chapter_sets;
        }
    }

    public void ClearSearchResults()
    {
        m_filter_chapters = null;

        m_found_phrases = new List<Phrase>();

        m_found_letters = new List<Letter>();

        m_found_words = new List<Word>();
        m_found_word_ranges = new List<List<Word>>();
        m_found_word_sets = new List<List<Word>>();

        m_found_sentences = new List<Sentence>();

        // previous m_found_verses is needed in nested search
        if (m_search_scope != SearchScope.Result)
        {
            m_found_verses = new List<Verse>();
        }
        m_found_verse_ranges = new List<List<Verse>>();
        m_found_verse_sets = new List<List<Verse>>();

        m_found_chapters = new List<Chapter>();
        m_found_chapter_ranges = new List<List<Chapter>>();
        m_found_chapter_sets = new List<List<Chapter>>();
    }

    // helper methods with GetSourceVerses (not entire book verses)
    public Dictionary<string, int> GetWordsWith(string text, TextLocationInChapter text_location_in_chapter, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        List<Verse> source = Server.GetSourceVerses(m_search_scope, m_selection, m_found_verses, text_location_in_chapter);
        if (Book != null)
        {
            Book.WithDiacritics = with_diacritics;
            result = Book.GetWordsWith(source, text, text_location_in_verse, text_location_in_word, text_wordness);
        }
        return result;
    }
    public Dictionary<string, int> GetCurrentWords(string text, TextLocationInChapter text_location_in_chapter, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        List<Verse> source = Server.GetSourceVerses(m_search_scope, m_selection, m_found_verses, text_location_in_chapter);
        if (Book != null)
        {
            Book.WithDiacritics = with_diacritics;
            result = Book.GetCurrentWords(source, text, text_location_in_verse, text_location_in_word, text_wordness);
        }
        return result;
    }
    public Dictionary<string, int> GetNextWords(string text, TextLocationInChapter text_location_in_chapter, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        List<Verse> source = Server.GetSourceVerses(m_search_scope, m_selection, m_found_verses, text_location_in_chapter);
        if (Book != null)
        {
            Book.WithDiacritics = with_diacritics;
            result = Book.GetNextWords(source, text, text_location_in_verse, text_location_in_word);
        }
        return result;
    }

    // helper method with GetSourceVerses (not entire book verses)
    public Dictionary<string, int> GetWordRoots(string text, TextLocationInWord text_location_in_word)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        List<Verse> source = Server.GetSourceVerses(m_search_scope, m_selection, m_found_verses, TextLocationInChapter.Any);
        if (Book != null)
        {
            result = Book.GetWordRoots(source, text, text_location_in_word);
        }
        return result;
    }

    // helper methods
    public List<string> GetSimplifiedWords()
    {
        List<string> result = null;

        if (this.NumerologySystem != null)
        {
            if (this.Book != null)
            {
                if (this.Book.Verses != null)
                {
                    SortedDictionary<string, int> word_frequencies = new SortedDictionary<string, int>();
                    if (word_frequencies != null)
                    {
                        List<Word> quran_words = new List<Word>();
                        if (quran_words != null)
                        {
                            foreach (Verse verse in this.Book.Verses)
                            {
                                quran_words.AddRange(verse.Words);
                            }
                        }

                        foreach (Word quran_word in quran_words)
                        {
                            string simplified_quran_word_text = quran_word.Text.SimplifyTo(this.NumerologySystem.TextMode);
                            if (word_frequencies.ContainsKey(simplified_quran_word_text))
                            {
                                word_frequencies[simplified_quran_word_text]++;
                            }
                            else
                            {
                                word_frequencies.Add(simplified_quran_word_text, 1);
                            }
                        }

                        result = new List<string>(word_frequencies.Keys);
                    }
                }
            }
        }

        return result;
    }
    public SortedDictionary<string, int> GetSimplifiedWordFrequencies()
    {
        SortedDictionary<string, int> result = new SortedDictionary<string, int>();

        if (this.NumerologySystem != null)
        {
            if (this.Book != null)
            {
                if (this.Book.Verses != null)
                {
                    if (result != null)
                    {
                        List<Word> quran_words = new List<Word>();
                        if (quran_words != null)
                        {
                            foreach (Verse verse in this.Book.Verses)
                            {
                                quran_words.AddRange(verse.Words);
                            }
                        }

                        foreach (Word quran_word in quran_words)
                        {
                            string simplified_quran_word_text = quran_word.Text.SimplifyTo(this.NumerologySystem.TextMode);
                            if (result.ContainsKey(simplified_quran_word_text))
                            {
                                result[simplified_quran_word_text]++;
                            }
                            else
                            {
                                result.Add(simplified_quran_word_text, 1);
                            }
                        }
                    }
                }
            }
        }

        return result;
    }

    // find by text - Exact
    /// <summary>
    /// Find phrases for given exact text that meet all parameters.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="language_type"></param>
    /// <param name="translation"></param>
    /// <param name="text_location_in_verse"></param>
    /// <param name="text_location_in_word"></param>
    /// <param name="case_sensitive"></param>
    /// <param name="text_wordness"></param>
    /// <param name="multiplicity"></param>
    /// <param name="at_word_start"></param>
    /// <returns>Number of found phrases. Result is stored in FoundPhrases.</returns>
    public int FindPhrases(TextSearchBlockSize text_search_block_size, string text, LanguageType language_type, string translation, TextLocationInChapter text_location_in_chapter, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness, bool case_sensitive, bool with_diacritics, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        ClearSearchResults();
        m_found_phrases = Server.FindPhrases(m_search_scope, m_selection, m_found_verses, text_search_block_size, text, language_type, translation, text_location_in_chapter, text_location_in_verse, text_location_in_word, text_wordness, case_sensitive, with_diacritics, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
        if (m_found_phrases != null)
        {
            m_found_verses = new List<Verse>();
            foreach (Phrase phrase in m_found_phrases)
            {
                if (phrase != null)
                {
                    if (!m_found_verses.Contains(phrase.Verse))
                    {
                        m_found_verses.Add(phrase.Verse);
                    }
                }
            }
            return m_found_phrases.Count;
        }
        return 0;
    }
    // find by text - Proximity
    /// <summary>
    /// Find phrases for given text by proximity that meet all parameters.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="language_type"></param>
    /// <param name="translation"></param>
    /// <param name="text_location_in_verse"></param>
    /// <param name="text_location_in_word"></param>
    /// <param name="case_sensitive"></param>
    /// <param name="text_wordness"></param>
    /// <param name="multiplicity"></param>
    /// <param name="at_word_start"></param>
    /// <returns>Number of found phrases. Result is stored in FoundPhrases.</returns>
    public int FindPhrases(TextSearchBlockSize text_search_block_size, string text, LanguageType language_type, string translation, TextProximityType text_proximity_type, TextWordness text_wordness, bool case_sensitive, bool with_diacritics)
    {
        ClearSearchResults();
        m_found_phrases = Server.FindPhrases(m_search_scope, m_selection, m_found_verses, text_search_block_size, text, language_type, translation, text_proximity_type, text_wordness, case_sensitive, with_diacritics);
        if (m_found_phrases != null)
        {
            m_found_verses = new List<Verse>();
            foreach (Phrase phrase in m_found_phrases)
            {
                if (phrase != null)
                {
                    if (!m_found_verses.Contains(phrase.Verse))
                    {
                        m_found_verses.Add(phrase.Verse);
                    }
                }
            }
            return m_found_phrases.Count;
        }
        return 0;
    }
    // find by text - Root
    /// <summary>
    /// Find phrases for given root( or space separate roots) that meet all parameters.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="multiplicity"></param>
    /// <returns>Number of found phrases. Result is stored in FoundPhrases.</returns>
    public int FindPhrases(TextSearchBlockSize text_search_block_size, string text, int multiplicity, NumberType multiplicity_number_type, ComparisonOperator multiplicity_comparison_operator, int multiplicity_remainder)
    {
        ClearSearchResults();
        m_found_phrases = Server.FindPhrases(m_search_scope, m_selection, m_found_verses, text_search_block_size, text, multiplicity, multiplicity_number_type, multiplicity_comparison_operator, multiplicity_remainder);
        if (m_found_phrases != null)
        {
            m_found_verses = new List<Verse>();
            foreach (Phrase phrase in m_found_phrases)
            {
                if (phrase != null)
                {
                    if (!m_found_verses.Contains(phrase.Verse))
                    {
                        m_found_verses.Add(phrase.Verse);
                    }
                }
            }
            return m_found_phrases.Count;
        }
        return 0;
    }
    /// <summary>
    /// Find verses with related words from the same root
    /// </summary>
    /// <param name="verse"></param>
    /// <returns>Number of found verses. Result is stored in FoundVerses.</returns>
    public int FindRelatedVerses(Verse verse)
    {
        ClearSearchResults();
        m_found_verses = Server.FindRelatedVerses(m_search_scope, m_selection, m_found_verses, verse);
        if (m_found_verses != null)
        {
            return m_found_verses.Count;
        }
        return 0;
    }
    // find by text - Repeated phrases with N words
    public int FindRepeatedPhrases(int phrase_word_count, bool with_diacritics)
    {
        ClearSearchResults();
        m_found_phrases = Server.FindRepeatedPhrases(phrase_word_count, with_diacritics);
        if (m_found_phrases != null)
        {
            m_found_verses = new List<Verse>();
            foreach (Phrase phrase in m_found_phrases)
            {
                if (phrase != null)
                {
                    if (!m_found_verses.Contains(phrase.Verse))
                    {
                        m_found_verses.Add(phrase.Verse);
                    }
                }
            }
            return m_found_phrases.Count;
        }
        return 0;
    }

    // find by similarity - phrases similar to given text
    /// <summary>
    /// Find phrases with similar text to given text to given similarity percentage or above.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="similarity_percentage"></param>
    /// <returns>Number of found phrases. Result is stored in FoundPhrases.</returns>
    public int FindPhrases(string text, double similarity_percentage)
    {
        ClearSearchResults();
        m_found_phrases = Server.FindPhrases(m_search_scope, m_selection, m_found_verses, text, similarity_percentage);
        if (m_found_phrases != null)
        {
            m_found_verses = new List<Verse>();
            foreach (Phrase phrase in m_found_phrases)
            {
                if (phrase != null)
                {
                    if (!m_found_verses.Contains(phrase.Verse))
                    {
                        m_found_verses.Add(phrase.Verse);
                    }
                }
            }

            return m_found_phrases.Count;
        }
        return 0;
    }
    // find by similarity - verses similar to given verse
    /// <summary>
    /// Find verses with similar text to verse text to given similarity percentage or above with given similarity method
    /// </summary>
    /// <param name="verse"></param>
    /// <param name="similarity_method"></param>
    /// <param name="similarity_percentage"></param>
    /// <returns>Number of found verses. Result is stored in FoundVerses.</returns>
    public int FindVerses(Verse verse, SimilarityMethod similarity_method, double similarity_percentage)
    {
        ClearSearchResults();
        m_found_verses = Server.FindVerses(m_search_scope, m_selection, m_found_verses, verse, similarity_method, similarity_percentage);
        if (m_found_verses != null)
        {
            return m_found_verses.Count;
        }
        return 0;
    }
    // find by similarity - all similar verses to each other throughout the book
    /// <summary>
    /// Find verse ranges with similar text to each other to given similarity percentage or above.
    /// </summary>
    /// <param name="similarity_method"></param>
    /// <param name="similarity_percentage"></param>
    /// <returns>Number of found verse ranges. Result is stored in FoundVerseRanges.</returns>
    public int FindVerses(SimilarityMethod similarity_method, double similarity_percentage)
    {
        ClearSearchResults();
        m_found_verse_ranges = Server.FindVersess(m_search_scope, m_selection, m_found_verses, similarity_method, similarity_percentage);
        if (m_found_verse_ranges != null)
        {
            m_found_verses = new List<Verse>();
            foreach (List<Verse> verse_range in m_found_verse_ranges)
            {
                m_found_verses.AddRange(verse_range);
            }

            return m_found_verse_ranges.Count;
        }
        return 0;
    }

    // find by numbers - Letters
    /// <summary>
    /// Find letters that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found letters. Result is stored in FoundLetters.</returns>
    public int FindLetters(NumberQuery query)
    {
        ClearSearchResults();
        m_found_letters = Server.FindLetters(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_letters != null)
        {
            m_found_verses = new List<Verse>();
            m_found_phrases = new List<Phrase>();
            foreach (Letter letter in m_found_letters)
            {
                if (letter != null)
                {
                    if (letter.Word != null)
                    {
                        Verse verse = letter.Word.Verse;
                        if (!m_found_verses.Contains(verse))
                        {
                            m_found_verses.Add(verse);
                        }
                    }
                }

                Phrase phrase = new Phrase(letter.Word.Verse, letter.Word.Position + letter.NumberInWord - 1, letter.ToString());
                m_found_phrases.Add(phrase);
            }

            return m_found_letters.Count;
        }
        return 0;
    }
    // find by numbers - Words
    /// <summary>
    /// Find words that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found words. Result is stored in FoundWords.</returns>
    public int FindWords(NumberQuery query)
    {
        ClearSearchResults();
        m_found_words = Server.FindWords(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_words != null)
        {
            m_found_verses = new List<Verse>();
            m_found_phrases = new List<Phrase>();
            foreach (Word word in m_found_words)
            {
                if (word != null)
                {
                    Verse verse = word.Verse;
                    if (!m_found_verses.Contains(verse))
                    {
                        m_found_verses.Add(verse);
                    }
                }

                Phrase phrase = new Phrase(word.Verse, word.Position, word.Text);
                m_found_phrases.Add(phrase);
            }

            return m_found_words.Count;
        }
        return 0;
    }
    // find by numbers - WordRanges
    /// <summary>
    /// Find word ranges that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found word ranges. Result is stored in FoundWordRanges.</returns>
    public int FindWordRanges(NumberQuery query)
    {
        ClearSearchResults();
        m_found_word_ranges = Server.FindWordRanges(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_word_ranges != null)
        {
            m_found_verses = new List<Verse>();
            m_found_phrases = new List<Phrase>();
            foreach (List<Word> range in m_found_word_ranges)
            {
                if (range != null)
                {
                    if (range.Count > 0)
                    {
                        string range_text = null;
                        foreach (Word word in range)
                        {
                            if (word != null)
                            {
                                // prepare found phrase verse
                                Verse verse = word.Verse;

                                // build found verses // prevent duplicate verses in case more than 1 range is found in same verse
                                if (!m_found_verses.Contains(verse))
                                {
                                    m_found_verses.Add(verse);
                                }

                                // prepare found phrase text
                                range_text += word.Text + " ";
                                if (NumerologySystem.TextMode == "Original")
                                {
                                    if (word.Stopmark != Stopmark.None)
                                    {
                                        range_text += StopmarkHelper.GetStopmarkText(word.Stopmark) + " ";
                                    }
                                }
                            }
                        }
                        range_text = range_text.Remove(range_text.Length - 1, 1);

                        // build found phrases // allow multiple phrases even if overlapping inside same verse
                        Phrase phrase = new Phrase(range[0].Verse, range[0].Position, range_text);
                        if (phrase != null)
                        {
                            m_found_phrases.Add(phrase);
                        }
                    }
                }
            }

            return m_found_word_ranges.Count;
        }
        return 0;
    }
    // find by numbers - WordSets
    /// <summary>
    /// Find word sets that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found word sets. Result is stored in FoundWordSets.</returns>
    public int FindWordSets(NumberQuery query)
    {
        ClearSearchResults();
        m_found_word_sets = Server.FindWordSets(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_word_sets != null)
        {
            m_found_verses = new List<Verse>();
            m_found_phrases = new List<Phrase>();
            foreach (List<Word> set in m_found_word_sets)
            {
                if (set != null)
                {
                    if (set.Count > 0)
                    {
                        // prepare found phrase verse
                        Verse verse = set[0].Verse;

                        // build found verses // prevent duplicate verses in case more than 1 set is found in same verse
                        if (!m_found_verses.Contains(verse))
                        {
                            m_found_verses.Add(verse);
                        }

                        // prepare found phrase text
                        string set_text = null;
                        foreach (Word word in set)
                        {
                            set_text += word.Text + " ";
                        }
                        set_text = set_text.Remove(set_text.Length - 1, 1);

                        // prepare found phrase position
                        int set_position = set[0].Position;

                        // build found phrases // allow multiple phrases even if overlapping inside same verse
                        Phrase phrase = new Phrase(verse, set_position, set_text);
                        m_found_phrases.Add(phrase);
                    }
                }
            }

            return m_found_word_sets.Count;
        }
        return 0;
    }
    // find by numbers - Sentences
    /// <summary>
    /// Find sentences across verses that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found sentences. Result is stored in FoundSentences.</returns>
    public int FindSentences(NumberQuery query)
    {
        ClearSearchResults();
        m_found_sentences = Server.FindSentences(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_sentences != null)
        {
            BuildSentencePhrases();

            return m_found_sentences.Count;
        }
        return 0;
    }
    // find by numbers - Verses
    /// <summary>
    /// Find verses that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found verses. Result is stored in FoundVerses.</returns>
    public int FindVerses(NumberQuery query)
    {
        ClearSearchResults();
        m_found_verses = Server.FindVerses(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_verses != null)
        {
            return m_found_verses.Count;
        }
        return 0;
    }
    // find by numbers - VerseRanges
    /// <summary>
    /// Find verse ranges that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found verse ranges. Result is stored in FoundVerseRanges.</returns>
    public int FindVerseRanges(NumberQuery query)
    {
        ClearSearchResults();
        m_found_verse_ranges = Server.FindVerseRanges(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_verse_ranges != null)
        {
            m_found_verses = new List<Verse>();
            foreach (List<Verse> range in m_found_verse_ranges)
            {
                m_found_verses.AddRange(range);
            }

            return m_found_verse_ranges.Count;
        }
        return 0;
    }
    // find by numbers - VerseSets
    /// <summary>
    /// Find verse sets that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found verse sets. Result is stored in FoundVerseSets.</returns>
    public int FindVerseSets(NumberQuery query)
    {
        ClearSearchResults();
        m_found_verse_sets = Server.FindVerseSets(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_verse_sets != null)
        {
            m_found_verses = new List<Verse>();
            foreach (List<Verse> set in m_found_verse_sets)
            {
                m_found_verses.AddRange(set);
            }

            return m_found_verse_sets.Count;
        }
        return 0;
    }
    // find by numbers - Chapters
    /// <summary>
    /// Find chapters that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found chapters. Result is stored in FoundChapters.</returns>
    public int FindChapters(NumberQuery query)
    {
        ClearSearchResults();
        m_found_chapters = Server.FindChapters(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_chapters != null)
        {
            m_found_verses = new List<Verse>();
            foreach (Chapter chapter in m_found_chapters)
            {
                if (chapter != null)
                {
                    m_found_verses.AddRange(chapter.Verses);
                }
            }

            return m_found_chapters.Count;
        }
        return 0;
    }
    // find by numbers - ChapterRanges
    /// <summary>
    /// Find chapter ranges that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found chapter ranges. Result is stored in FoundChapterRanges.</returns>
    public int FindChapterRanges(NumberQuery query)
    {
        ClearSearchResults();
        m_found_chapter_ranges = Server.FindChapterRanges(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_chapter_ranges != null)
        {
            m_found_verses = new List<Verse>();
            foreach (List<Chapter> range in m_found_chapter_ranges)
            {
                foreach (Chapter chapter in range)
                {
                    if (chapter != null)
                    {
                        m_found_verses.AddRange(chapter.Verses);
                    }
                }
            }

            return m_found_chapter_ranges.Count;
        }
        return 0;
    }
    // find by numbers - ChapterSets
    /// <summary>
    /// Find chapter sets that meet query criteria.
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Number of found chapter sets. Result is stored in FoundChapterSets.</returns>
    public int FindChapterSets(NumberQuery query)
    {
        ClearSearchResults();
        m_found_chapter_sets = Server.FindChapterSets(m_search_scope, m_selection, m_found_verses, query);
        if (m_found_chapter_sets != null)
        {
            m_found_verses = new List<Verse>();
            foreach (List<Chapter> set in m_found_chapter_sets)
            {
                foreach (Chapter chapter in set)
                {
                    if (chapter != null)
                    {
                        m_found_verses.AddRange(chapter.Verses);
                    }
                }
            }

            return m_found_chapter_sets.Count;
        }
        return 0;
    }

    // find by prostration type
    /// <summary>
    /// Find verses with given prostration type.
    /// </summary>
    /// <param name="prostration_type"></param>
    /// <returns>Number of found verses. Result is stored in FoundVerses.</returns>
    public int FindVerses(ProstrationType prostration_type)
    {
        ClearSearchResults();
        m_found_verses = Server.FindVerses(m_search_scope, m_selection, m_found_verses, prostration_type);
        if (m_found_verses != null)
        {
            return m_found_verses.Count;
        }
        return 0;
    }

    // find by revelation place
    /// <summary>
    /// Find chapters that were revealed at given revelation place.
    /// </summary>
    /// <param name="revelation_place"></param>
    /// <returns>Number of found chapters. Result is stored in FoundChapters.</returns>
    public int FindChapters(RevelationPlace revelation_place)
    {
        ClearSearchResults();
        m_found_chapters = Server.FindChapters(m_search_scope, m_selection, m_found_verses, revelation_place);
        if (m_found_chapters != null)
        {
            if (m_found_chapters != null)
            {
                m_found_verses = new List<Verse>();
                foreach (Chapter chapter in m_found_chapters)
                {
                    if (chapter != null)
                    {
                        m_found_verses.AddRange(chapter.Verses);
                    }
                }

                return m_found_chapters.Count;
            }
        }
        return 0;
    }

    // find by initialization type
    /// <summary>
    /// Find verses with given initialization type.
    /// </summary>
    /// <param name="initialization_type"></param>
    /// <returns>Number of found verses. Result is stored in FoundVerses.</returns>
    public int FindVerses(InitializationType initialization_type)
    {
        ClearSearchResults();
        m_found_verses = Server.FindVerses(m_search_scope, m_selection, m_found_verses, initialization_type);
        if (m_found_verses != null)
        {
            return m_found_verses.Count;
        }
        return 0;
    }

    // find by letter frequency sum
    /// <summary>
    /// Find words with required letter frequency sum in their text of the given phrase.
    /// </summary>
    /// <param name="phrase"></param>
    /// <param name="sum"></param>
    /// <param name="frequency_search_type"></param>
    /// <returns>Number of found words. Result is stored in FoundWords.</returns>
    public int FindWords(string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        ClearSearchResults();
        m_found_words = Server.FindWords(m_search_scope, m_selection, m_found_verses, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
        if (m_found_words != null)
        {
            m_found_verses = new List<Verse>();
            m_found_phrases = new List<Phrase>();
            foreach (Word word in m_found_words)
            {
                if (word != null)
                {
                    Verse verse = word.Verse;
                    if (!m_found_verses.Contains(verse))
                    {
                        m_found_verses.Add(verse);
                    }
                }

                Phrase word_phrase = new Phrase(word.Verse, word.Position, word.Text);
                m_found_phrases.Add(word_phrase);
            }

            return m_found_words.Count;
        }
        return 0;
    }
    /// <summary>
    /// Find sentences across verses with required letter frequency sum in their text of the given phrase.
    /// </summary>
    /// <param name="phrase"></param>
    /// <param name="sum"></param>
    /// <param name="frequency_search_type"></param>
    /// <returns>Number of found sentences. Result is stored in FoundSentences.</returns>
    public int FindSentences(string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        ClearSearchResults();
        m_found_sentences = Server.FindSentences(m_search_scope, m_selection, m_found_verses, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
        if (m_found_sentences != null)
        {
            BuildSentencePhrases();

            return m_found_sentences.Count;
        }
        return 0;
    }
    private void BuildSentencePhrases()
    {
        if (m_found_sentences != null)
        {
            m_found_verses = new List<Verse>();
            m_found_phrases = new List<Phrase>();
            foreach (Sentence sentence in m_found_sentences)
            {
                if (sentence != null)
                {
                    Verse first_verse = sentence.FirstVerse;
                    Verse last_verse = sentence.LastVerse;
                    if ((first_verse != null) && (last_verse != null))
                    {
                        int start = first_verse.Number - 1;
                        int end = last_verse.Number - 1;
                        if (end >= start)
                        {
                            // add unique verses
                            for (int i = start; i <= end; i++)
                            {
                                if (!m_found_verses.Contains(Book.Verses[i]))
                                {
                                    m_found_verses.Add(Book.Verses[i]);
                                }
                            }

                            // build phrases for colorization
                            if (start == end) // sentence within verse
                            {
                                Phrase sentence_phrase = new Phrase(first_verse, sentence.StartPosition, sentence.Text);
                                m_found_phrases.Add(sentence_phrase);
                            }
                            else // sentence across verses
                            {
                                // first verse
                                string start_text = first_verse.Text.Substring(sentence.StartPosition);
                                Phrase start_phrase = new Phrase(sentence.FirstVerse, sentence.StartPosition, start_text);
                                m_found_phrases.Add(start_phrase);

                                // middle verses
                                for (int i = start + 1; i < end; i++)
                                {
                                    Verse verse = Book.Verses[i];
                                    if (verse != null)
                                    {
                                        Phrase middle_phrase = new Phrase(verse, 0, verse.Text);
                                        m_found_phrases.Add(middle_phrase);
                                    }
                                }

                                // last verse
                                string end_text = last_verse.Text.Substring(0, sentence.EndPosition);
                                Phrase end_phrase = new Phrase(last_verse, 0, end_text);
                                m_found_phrases.Add(end_phrase);
                            }
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Find verses with required letter frequency sum in their text of the given phrase.
    /// </summary>
    /// <param name="phrase"></param>
    /// <param name="sum"></param>
    /// <param name="frequency_search_type"></param>
    /// <returns>Number of found verses. Result is stored in FoundVerses.</returns>
    public int FindVerses(string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        ClearSearchResults();
        m_found_verses = Server.FindVerses(m_search_scope, m_selection, m_found_verses, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
        if (m_found_verses != null)
        {
            return m_found_verses.Count;
        }
        return 0;
    }
    /// <summary>
    /// Find chapters with required letter frequency sum in their text of the given phrase.
    /// </summary>
    /// <param name="phrase"></param>
    /// <param name="sum"></param>
    /// <param name="frequency_search_type"></param>
    /// <returns>Number of found chapters. Result is stored in FoundChapters.</returns>
    public int FindChapters(string phrase, int sum, NumberType number_type, ComparisonOperator comparison_operator, int sum_remainder, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        ClearSearchResults();
        m_found_chapters = Server.FindChapters(m_search_scope, m_selection, m_found_verses, phrase, sum, number_type, comparison_operator, sum_remainder, frequency_search_type, with_diacritics);
        if (m_found_chapters != null)
        {
            if (m_found_chapters != null)
            {
                m_found_verses = new List<Verse>();
                foreach (Chapter chapter in m_found_chapters)
                {
                    if (chapter != null)
                    {
                        m_found_verses.AddRange(chapter.Verses);
                    }
                }

                return m_found_chapters.Count;
            }
        }
        return 0;
    }
    public int CalculateLetterFrequencySum(string text, string phrase, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        return Server.CalculateLetterFrequencySum(text, phrase, frequency_search_type, with_diacritics);
    }

    private List<object> m_history_items = new List<object>();
    public List<object> HistoryItems
    {
        get { return m_history_items; }
    }
    private int m_history_item_index = -1;
    public int HistoryItemIndex
    {
        get { return m_history_item_index; }
    }
    public object CurrentHistoryItem
    {
        get
        {
            if (m_history_items != null)
            {
                if ((m_history_item_index >= 0) && (m_history_item_index < m_history_items.Count))
                {
                    return m_history_items[m_history_item_index];
                }
            }
            return null;
        }
    }
    public void AddHistoryItem(object item)
    {
        if (m_history_items != null)
        {
            m_history_items.Add(item);
            m_history_item_index = m_history_items.Count - 1;
        }
    }
    public void DeleteHistoryItem(object item)
    {
        if (m_history_items != null)
        {
            m_history_items.Remove(item);
            m_history_item_index = m_history_items.Count - 1;
        }
    }
    public void DeleteCurrentHistoryItem()
    {
        if (m_history_items != null)
        {
            if ((m_history_item_index >= 0) && (m_history_item_index < m_history_items.Count))
            {
                object item = m_history_items[m_history_item_index];
                m_history_items.Remove(item);
                m_history_item_index = m_history_items.Count - 1;
            }

            if (m_history_items.Count == 0) // all items deleted
            {
                m_history_item_index = -1;
            }
            else // there are still some item(s)
            {
                // if index becomes outside list, move back into list
                if (m_history_item_index == m_history_items.Count)
                {
                    m_history_item_index = m_history_items.Count - 1;
                }
            }
        }
    }
    public void ClearHistoryItems()
    {
        if (m_history_items != null)
        {
            m_history_items.Clear();
            m_history_item_index = -1;
        }
    }
    public object GotoPreviousHistoryItem()
    {
        object result = null;
        if (m_history_items != null)
        {
            if ((m_history_item_index > 0) && (m_history_item_index < m_history_items.Count))
            {
                m_history_item_index--;
                result = m_history_items[m_history_item_index];
            }
        }
        return result;
    }
    public object GotoNextHistoryItem()
    {
        object result = null;
        if (m_history_items != null)
        {
            if ((m_history_item_index >= -1) && (m_history_item_index < m_history_items.Count - 1))
            {
                m_history_item_index++;
                result = m_history_items[m_history_item_index];
            }
        }
        return result;
    }
    public void SaveHistoryItems()
    {
        if (m_history_items != null)
        {
            if (Directory.Exists(Globals.HISTORY_FOLDER))
            {
                string filename = Globals.HISTORY_FOLDER + "/" + "History.txt";
                try
                {
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                    {
                        StringBuilder str = new StringBuilder();

                        foreach (object history_item in m_history_items)
                        {
                            if (history_item is BrowseHistoryItem)
                            {
                                BrowseHistoryItem item = history_item as BrowseHistoryItem;
                                if (item != null)
                                {
                                    str.AppendLine("BrowseHistoryItem");
                                    str.AppendLine(item.Scope.ToString());
                                    if (item.Indexes.Count > 0)
                                    {
                                        foreach (int index in item.Indexes)
                                        {
                                            str.Append(index.ToString() + ",");
                                        }
                                        if (str.Length > 1)
                                        {
                                            str.Remove(str.Length - 1, 1);
                                        }
                                    }
                                    str.AppendLine();
                                    str.AppendLine(END_OF_HISTORY_ITME_MARKER);
                                }
                            }
                            else if (history_item is SearchHistoryItem)
                            {
                                SearchHistoryItem item = history_item as SearchHistoryItem;
                                if (item != null)
                                {
                                    str.AppendLine("SearchHistoryItem");
                                    str.AppendLine("SearchType" + "\t" + item.SearchType);
                                    str.AppendLine("NumbersResultType" + "\t" + item.NumbersResultType);
                                    str.AppendLine("Text" + "\t" + item.Text);
                                    str.AppendLine("Header" + "\t" + item.Header);
                                    str.AppendLine("Language" + "\t" + item.LanguageType);
                                    str.AppendLine("Translation" + "\t" + item.Translation);

                                    if ((item.Phrases != null) && (item.Phrases.Count > 0))
                                    {
                                        foreach (Phrase phrase in item.Phrases)
                                        {
                                            if (phrase != null)
                                            {
                                                str.AppendLine(phrase.Verse.Number.ToString() + "," + phrase.Text + "," + phrase.Position.ToString());
                                            }
                                        }
                                    }
                                    else // verse.Number
                                    {
                                        //TODO: Save NumberQuery with each search result

                                        foreach (Verse verse in item.Verses)
                                        {
                                            str.AppendLine(verse.Number.ToString());
                                        }
                                    }
                                    str.AppendLine(END_OF_HISTORY_ITME_MARKER);
                                }
                            }
                        }
                        writer.Write(str.ToString());
                    }
                }
                catch
                {
                    // silence IO error in case running from read-only media (CD/DVD)
                }
            }
        }
    }
    public void LoadHistoryItems()
    {
        if (Book != null)
        {
            string filename = Globals.HISTORY_FOLDER + "/" + "History.txt";
            if (File.Exists(filename))
            {
                using (StreamReader reader = File.OpenText(filename))
                {
                    try
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] parts = null;

                            if (line == "")
                            {
                                continue;
                            }
                            else if (line == "BrowseHistoryItem")
                            {
                                line = reader.ReadLine();
                                SelectionScope scope = (SelectionScope)Enum.Parse(typeof(SelectionScope), line);
                                List<int> indexes = new List<int>();

                                line = reader.ReadLine();
                                parts = line.Split(',');
                                foreach (string part in parts)
                                {
                                    int index = -1;
                                    if (int.TryParse(part, out index))
                                    {
                                        indexes.Add(index);
                                    }
                                }

                                BrowseHistoryItem item = new BrowseHistoryItem(Book, scope, indexes);
                                AddHistoryItem(item);
                            }
                            else if (line == "SearchHistoryItem")
                            {
                                SearchHistoryItem item = new SearchHistoryItem();

                                line = reader.ReadLine();
                                parts = line.Split('\t');
                                if ((parts.Length == 2) && (parts[0].Trim() == "SearchType"))
                                {
                                    item.SearchType = (SearchType)Enum.Parse(typeof(SearchType), parts[1].Trim());
                                }

                                line = reader.ReadLine();
                                parts = line.Split('\t');
                                if ((parts.Length == 2) && (parts[0].Trim() == "NumbersResultType"))
                                {
                                    item.NumbersResultType = (NumbersResultType)Enum.Parse(typeof(NumbersResultType), parts[1].Trim());
                                }

                                line = reader.ReadLine();
                                parts = line.Split('\t');
                                if ((parts.Length == 2) && (parts[0].Trim() == "Text"))
                                {
                                    item.Text = parts[1].Trim();
                                }

                                line = reader.ReadLine();
                                parts = line.Split('\t');
                                if ((parts.Length == 2) && (parts[0].Trim() == "Header"))
                                {
                                    item.Header = parts[1].Trim();
                                }

                                line = reader.ReadLine();
                                parts = line.Split('\t');
                                if ((parts.Length == 2) && (parts[0].Trim() == "Language"))
                                {
                                    item.LanguageType = (LanguageType)Enum.Parse(typeof(LanguageType), parts[1].Trim());
                                }

                                line = reader.ReadLine();
                                parts = line.Split('\t');
                                if ((parts.Length == 2) && (parts[0].Trim() == "Translation"))
                                {
                                    item.Translation = parts[1].Trim();
                                }

                                // CSV: Phrase.Verse.Number, Phrase.Text, Phrase.Position
                                while (true)
                                {
                                    line = reader.ReadLine();
                                    if (line == END_OF_HISTORY_ITME_MARKER)
                                    {
                                        break;
                                    }
                                    parts = line.Split(',');
                                    if (parts.Length == 1) // verse.Number
                                    {
                                        //TODO: Load NumberQuery with each search result

                                        int verse_index = int.Parse(parts[0].Trim()) - 1;
                                        if ((verse_index >= 0) && (verse_index < Book.Verses.Count))
                                        {
                                            Verse verse = Book.Verses[verse_index];
                                            if (!item.Verses.Contains(verse))
                                            {
                                                item.Verses.Add(verse);
                                            }
                                        }
                                    }
                                    else if (parts.Length == 3) // phrase.Verse.Number,phrase.Text,phrase.Position
                                    {
                                        int verse_index = int.Parse(parts[0].Trim()) - 1;
                                        if ((verse_index >= 0) && (verse_index < Book.Verses.Count))
                                        {
                                            Verse verse = Book.Verses[verse_index];
                                            if (!item.Verses.Contains(verse))
                                            {
                                                item.Verses.Add(verse);
                                            }

                                            string phrase_text = parts[1].Trim();
                                            if (phrase_text.Length > 0)
                                            {
                                                int phrase_position = int.Parse(parts[2].Trim());
                                                Phrase phrase = new Phrase(verse, phrase_position, phrase_text);
                                                item.Phrases.Add(phrase);
                                            }
                                        }
                                    }
                                } // while

                                AddHistoryItem(item);
                            }
                        } // while
                    }
                    catch
                    {
                        throw new Exception("Invalid " + filename + " format.");
                    }
                }
            }
        }
    }
    private string END_OF_HISTORY_ITME_MARKER = "-------------------";

    private List<LetterStatistic> m_letter_statistics = new List<LetterStatistic>();
    public List<LetterStatistic> LetterStatistics
    {
        get { return m_letter_statistics; }
    }
    public void SortLetterStatistics(StatisticSortMethod sort_method)
    {
        LetterStatistic.SortMethod = sort_method;

        if (LetterStatistic.SortOrder == StatisticSortOrder.Ascending)
        {
            LetterStatistic.SortOrder = StatisticSortOrder.Descending;
        }
        else
        {
            LetterStatistic.SortOrder = StatisticSortOrder.Ascending;
        }

        m_letter_statistics.Sort();
    }
    /// <summary>
    /// Calculate letter statistics for the given text.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="phrase"></param>
    /// <param name="frequency_search_type"></param>
    /// <returns>Result is stored in LetterStatistics.</returns>
    public void BuildLetterStatistics(string text, bool with_diacritics)
    {
        if (String.IsNullOrEmpty(text)) return;

        if (NumerologySystem != null)
        {
            if (m_letter_statistics != null)
            {
                if (!with_diacritics) text = text.SimplifyTo(NumerologySystem.TextMode);
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
                    m_letter_statistics.Clear();
                    for (int i = 0; i < text.Length; i++)
                    {
                        bool is_found = false;
                        for (int j = 0; j < m_letter_statistics.Count; j++)
                        {
                            if (text[i] == m_letter_statistics[j].Letter)
                            {
                                is_found = true;
                                m_letter_statistics[j].Frequency++;
                                int pos = i + 1;
                                long last_pos = m_letter_statistics[j].Positions[m_letter_statistics[j].Positions.Count - 1];
                                m_letter_statistics[j].Positions.Add(pos);
                                m_letter_statistics[j].PositionSum += pos;
                                long distance = pos - last_pos;
                                m_letter_statistics[j].Distances.Add(distance);
                                m_letter_statistics[j].DistanceSum += distance;
                            }
                        }

                        if (!is_found)
                        {
                            LetterStatistic letter_statistic = new LetterStatistic();

                            letter_statistic.Order = m_letter_statistics.Count + 1;
                            letter_statistic.Letter = text[i];
                            letter_statistic.Frequency = 1;
                            int pos = i + 1;
                            letter_statistic.Positions.Add(pos);
                            letter_statistic.PositionSum += pos;

                            m_letter_statistics.Add(letter_statistic);
                        }
                    }
                }
            }
        }
    }
    public void SaveLetterStatistics(string filename, string text, bool verbose)
    {
        if (String.IsNullOrEmpty(filename)) return;
        if (String.IsNullOrEmpty(text)) return;

        if (Directory.Exists(Globals.STATISTICS_FOLDER))
        {
            filename = Globals.STATISTICS_FOLDER + "/" + filename;
            try
            {
                if (NumerologySystem != null)
                {
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                    {
                        writer.WriteLine("-------------------------------------------------------------------");
                        StringBuilder numbers = new StringBuilder();
                        foreach (int index in Selection.Indexes)
                        {
                            numbers.Append((index + 1).ToString() + ", ");
                        }
                        if (numbers.Length > 0)
                        {
                            numbers.Remove(numbers.Length - 2, 2);
                        }
                        writer.WriteLine(NumerologySystem.Name);
                        writer.WriteLine("Selection = " + Selection.Scope.ToString() + " " + numbers.ToString());
                        writer.WriteLine("-------------------------------------------------------------------");
                        writer.WriteLine();
                        writer.WriteLine("-------------------------------------------------------------------");
                        writer.WriteLine("Text");
                        writer.WriteLine("-------------------------------------------------------------------");
                        writer.WriteLine(text);
                        writer.WriteLine("-------------------------------------------------------------------");
                        writer.WriteLine();
                        writer.WriteLine("-------------------------------------------------------------------");
                        if (verbose) writer.WriteLine("Order" + "\t" + "Letter" + "\t" + "Freq" + "\t" + "∑Pos" + "\t" + "\t" + "∑∆" + "\t");
                        else writer.WriteLine("Order" + "\t" + "Letter" + "\t" + "Freq" + "\t" + "∑Pos" + "\t" + "∑∆");
                        writer.WriteLine("-------------------------------------------------------------------");
                        int count = 0;
                        int running_sum = 0;
                        int frequence_sum = 0;
                        long positionsum_sum = 0;
                        long distancesum_sum = 0;
                        foreach (LetterStatistic letter_statistic in m_letter_statistics)
                        {
                            if (verbose)
                            {
                                StringBuilder positions = new StringBuilder();
                                foreach (int value in letter_statistic.Positions)
                                {
                                    positions.Append(value.ToString() + ",");
                                }
                                if (positions.Length > 0)
                                {
                                    positions.Remove(positions.Length - 1, 1);
                                }

                                StringBuilder distances = new StringBuilder();
                                foreach (int value in letter_statistic.Distances)
                                {
                                    distances.Append(value.ToString() + ",");
                                }
                                if (distances.Length > 0)
                                {
                                    distances.Remove(distances.Length - 1, 1);
                                }

                                writer.WriteLine(letter_statistic.Order.ToString() + "\t" +
                                                 letter_statistic.Letter.ToString() + '\t' +
                                                 letter_statistic.Frequency.ToString() + "\t" +
                                                 letter_statistic.PositionSum.ToString() + "\t" + positions.ToString() + "\t" +
                                                 letter_statistic.DistanceSum.ToString() + "\t" + distances.ToString()
                                                 );
                            }
                            else
                            {
                                writer.WriteLine(letter_statistic.Order.ToString() + "\t" +
                                                letter_statistic.Letter.ToString() + '\t' +
                                                letter_statistic.Frequency.ToString() + "\t" +
                                                letter_statistic.PositionSum.ToString() + "\t" +
                                                letter_statistic.DistanceSum.ToString() + "\t"
                                                );
                            }
                            count++;
                            running_sum += count;
                            frequence_sum += letter_statistic.Frequency;
                            positionsum_sum += letter_statistic.PositionSum;
                            distancesum_sum += letter_statistic.DistanceSum;
                        }
                        writer.WriteLine("-------------------------------------------------------------------");
                        //writer.WriteLine(running_sum + "\t" + "Sum" + "\t" + frequence_sum.ToString() + "\t" + positionsum_sum.ToString() + "\t" + "\t" + distancesum_sum.ToString());
                        writer.WriteLine(running_sum + "\t" + "Sum" + "\t" + frequence_sum.ToString() + "\t" + positionsum_sum.ToString() + "\t" + distancesum_sum.ToString());
                        writer.WriteLine("-------------------------------------------------------------------");
                    }
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            FileHelper.DisplayFile(filename);
        }
    }
    /// <summary>
    /// Calculate letter statistics for the given phrase in text.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="phrase"></param>
    /// <param name="frequency_search_type"></param>
    /// <returns>Letter frequency sum. Result is stored in LetterStatistics.</returns>
    public void BuildLetterStatistics(string text, string phrase, FrequencySearchType frequency_search_type, bool with_diacritics)
    {
        if (String.IsNullOrEmpty(text)) return;
        if (String.IsNullOrEmpty(phrase)) return;

        if (NumerologySystem != null)
        {
            if (m_letter_statistics != null)
            {
                if (!with_diacritics) text = text.SimplifyTo(NumerologySystem.TextMode);
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
                    if (!with_diacritics) phrase = phrase.SimplifyTo(NumerologySystem.TextMode);
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
                        phrase = phrase.RemoveDuplicates();
                    }

                    if (!String.IsNullOrEmpty(phrase))
                    {
                        m_letter_statistics.Clear();
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
                                LetterStatistic phrase_letter_statistic = new LetterStatistic();
                                phrase_letter_statistic.Order = m_letter_statistics.Count + 1;
                                phrase_letter_statistic.Letter = phrase[i];
                                phrase_letter_statistic.Frequency = frequency;
                                m_letter_statistics.Add(phrase_letter_statistic);
                            }
                        }
                    }
                }
            }
        }
    }
    public void SaveLetterStatistics(string filename, string text, string phrase)
    {
        if (String.IsNullOrEmpty(filename)) return;
        if (String.IsNullOrEmpty(text)) return;

        if (Directory.Exists(Globals.STATISTICS_FOLDER))
        {
            filename = Globals.STATISTICS_FOLDER + "/" + "Phrase_" + filename;
            try
            {
                if (NumerologySystem != null)
                {
                    if (m_letter_statistics != null)
                    {
                        using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                        {
                            writer.WriteLine("-------------------------------------------------------------------");
                            StringBuilder numbers = new StringBuilder();
                            foreach (int index in Selection.Indexes)
                            {
                                numbers.Append((index + 1).ToString() + ", ");
                            }
                            if (numbers.Length > 0)
                            {
                                numbers.Remove(numbers.Length - 2, 2);
                            }
                            writer.WriteLine(NumerologySystem.Name);
                            writer.WriteLine("Selection = " + Selection.Scope.ToString() + " " + numbers.ToString());
                            writer.WriteLine("-------------------------------------------------------------------");
                            writer.WriteLine();
                            writer.WriteLine("-------------------------------------------------------------------");
                            writer.WriteLine("Text");
                            writer.WriteLine("-------------------------------------------------------------------");
                            writer.WriteLine(text);
                            writer.WriteLine("-------------------------------------------------------------------");
                            writer.WriteLine();
                            writer.WriteLine("-------------------------------------------------------------------");
                            writer.WriteLine("Phrase");
                            writer.WriteLine("-------------------------------------------------------------------");
                            writer.WriteLine(phrase);
                            writer.WriteLine("-------------------------------------------------------------------");
                            writer.WriteLine();
                            writer.WriteLine("-------------------------------------------------------------------");
                            writer.WriteLine("Order" + "\t" + "Letter" + "\t" + "Frequency");
                            writer.WriteLine("-------------------------------------------------------------------");
                            int count = m_letter_statistics.Count;
                            int sum = 0;
                            for (int i = 0; i < count; i++)
                            {
                                writer.WriteLine(m_letter_statistics[i].Order.ToString() + "\t" + m_letter_statistics[i].Letter.ToString() + '\t' + m_letter_statistics[i].Frequency.ToString());
                                sum += m_letter_statistics[i].Frequency;
                            }
                            writer.WriteLine("-------------------------------------------------------------------");
                            writer.WriteLine("Total" + "\t" + count.ToString() + "\t" + sum.ToString());
                            writer.WriteLine("-------------------------------------------------------------------");
                        }
                    }
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            FileHelper.DisplayFile(filename);
        }
    }

    private StringBuilder m_word_symmetry_str = null;
    public void CalculateWordSymmetry(SelectionScope selection_scope, bool include_boundary_cases)
    {
        m_word_symmetry_str = new StringBuilder();
        //m_word_symmetry_str.AppendLine("----------------------------------------------------------------");
        //m_word_symmetry_str.AppendLine(Book.TextMode.ToString() +
        //                                ((Book.WithBismAllah) ? "" : "_WithoutBismAllah") +
        //                                ((Book.WawAsWord) ? "_WawAsWord" : "") +
        //                                ((Book.ShaddaAsLetter) ? "_ShaddaAsLetter" : ""));
        //m_word_symmetry_str.AppendLine((include_boundary_cases ? "Full " : "") + selection_scope.ToString() + " Word Symmetry");
        //m_word_symmetry_str.AppendLine("----------------------------------------------------------------");
        //m_word_symmetry_str.AppendLine();
        //m_word_symmetry_str.AppendLine();
        //m_word_symmetry_str.AppendLine("----------------------------------------------------------");
        m_word_symmetry_str.AppendLine(((selection_scope == SelectionScope.Book) ? "BOOK" : selection_scope.ToString().ToUpper()) + "\tWORDS\t#\tWords\tLetters\tWSum\tLSum\tSymmetry");
        //m_word_symmetry_str.AppendLine("----------------------------------------------------------");

        List<Word> words = new List<Word>();
        switch (selection_scope)
        {
            case SelectionScope.Book:
                {
                    words.Clear();
                    foreach (Verse verse in Book.Verses)
                    {
                        words.AddRange(verse.Words);
                    }
                    DoCalculateWordSymmetry(1, words, include_boundary_cases);
                }
                break;
            case SelectionScope.Chapter:
                {
                    foreach (Chapter chapter in Book.Chapters)
                    {
                        words.Clear();
                        foreach (Verse verse in chapter.Verses)
                        {
                            words.AddRange(verse.Words);
                        }
                        DoCalculateWordSymmetry(chapter.SortedNumber, words, include_boundary_cases);
                    }
                }
                break;
            case SelectionScope.Page:
                {
                    foreach (Page page in Book.Pages)
                    {
                        words.Clear();
                        foreach (Verse verse in page.Verses)
                        {
                            words.AddRange(verse.Words);
                        }
                        DoCalculateWordSymmetry(page.Number, words, include_boundary_cases);
                    }
                }
                break;
            case SelectionScope.Station:
                {
                    foreach (Station station in Book.Stations)
                    {
                        words.Clear();
                        foreach (Verse verse in station.Verses)
                        {
                            words.AddRange(verse.Words);
                        }
                        DoCalculateWordSymmetry(station.Number, words, include_boundary_cases);
                    }
                }
                break;
            case SelectionScope.Part:
                {
                    foreach (Part part in Book.Parts)
                    {
                        words.Clear();
                        foreach (Verse verse in part.Verses)
                        {
                            words.AddRange(verse.Words);
                        }
                        DoCalculateWordSymmetry(part.Number, words, include_boundary_cases);
                    }
                }
                break;
            case SelectionScope.Group:
                {
                    foreach (Group group in Book.Groups)
                    {
                        words.Clear();
                        foreach (Verse verse in group.Verses)
                        {
                            words.AddRange(verse.Words);
                        }
                        DoCalculateWordSymmetry(group.Number, words, include_boundary_cases);
                    }
                }
                break;
            case SelectionScope.Half:
                {
                    foreach (Half half in Book.Halfs)
                    {
                        words.Clear();
                        foreach (Verse verse in half.Verses)
                        {
                            words.AddRange(verse.Words);
                        }
                        DoCalculateWordSymmetry(half.Number, words, include_boundary_cases);
                    }
                }
                break;
            case SelectionScope.Quarter:
                {
                    foreach (Quarter quarter in Book.Quarters)
                    {
                        words.Clear();
                        foreach (Verse verse in quarter.Verses)
                        {
                            words.AddRange(verse.Words);
                        }
                        DoCalculateWordSymmetry(quarter.Number, words, include_boundary_cases);
                    }
                }
                break;
            case SelectionScope.Bowing:
                {
                    foreach (Bowing bowing in Book.Bowings)
                    {
                        words.Clear();
                        foreach (Verse verse in bowing.Verses)
                        {
                            words.AddRange(verse.Words);
                        }
                        DoCalculateWordSymmetry(bowing.Number, words, include_boundary_cases);
                    }
                }
                break;
            case SelectionScope.Verse:
            default:
                {
                    foreach (Verse verse in Book.Verses)
                    {
                        DoCalculateWordSymmetry(verse.Number, verse.Words, include_boundary_cases);
                    }
                }
                break;
        }

        //m_word_symmetry_str.AppendLine("----------------------------------------------------------");

        SaveWordSymmetry(selection_scope, m_word_symmetry_str.ToString(), include_boundary_cases);
    }
    private void DoCalculateWordSymmetry(int selection_number, List<Word> words, bool include_boundary_cases)
    {
        if (m_word_symmetry_str == null) return;

        int count = 0;
        int w_sum = 0;
        int l_sum = 0;
        int max = words.Count - 1;
        if (include_boundary_cases)
        {
            // 0, 0
            count++;
            w_sum += 0;
            l_sum += 0;
            //                             selection_number                     WORDS                           #                         Words        Letters      WSum         LSum         Symmetry
            m_word_symmetry_str.AppendLine(selection_number.ToString() + "\t" + words.Count.ToString() + "\t" + count.ToString() + "\t" + "0" + "\t" + "0" + "\t" + "0" + "\t" + "0" + "\t" + ((double)(count * 100.0D) / (double)words.Count).ToString("0.000") + "%");

            // all words, all letters
            max++;
        }

        int a = 0;
        int z = 0;
        for (int i = 0; i < max; i++)
        {
            int j = words.Count - 1 - i;

            a += words[i].Letters.Count;
            z += words[j].Letters.Count;
            if (a == z)
            {
                count++;
                w_sum += (i + 1);
                l_sum += a;
                //                             selection_number                     WORDS                           #                         Words                       Letters               WSum                      LSum                      Symmetry
                m_word_symmetry_str.AppendLine(selection_number.ToString() + "\t" + words.Count.ToString() + "\t" + count.ToString() + "\t" + (i + 1).ToString() + "\t" + a.ToString() + "\t" + w_sum.ToString() + "\t" + l_sum.ToString() + "\t" + ((double)((count - (include_boundary_cases ? 2 : 0)) * 100.0D) / (double)words.Count).ToString("0.000") + "%");
            }
        }
    }
    private void SaveWordSymmetry(SelectionScope selection_scope, string word_symmetry_text, bool include_boundary_cases)
    {
        if (Directory.Exists(Globals.STATISTICS_FOLDER))
        {
            string filename = Globals.STATISTICS_FOLDER + "/" + (include_boundary_cases ? "Full" : "") + selection_scope.ToString() + "WordSymmetry" + "_" +
                                                                Book.TextMode.ToString() +
                                                                ((Book.WithBismAllah) ? "" : "_WithoutBismAllah") +
                                                                ((Book.WawAsWord) ? "_WawAsWord" : "") +
                                                                ((Book.ShaddaAsLetter) ? "_ShaddaAsLetter" : "") +
                                                                ((Book.HamzaAboveHorizontalLineAsLetter) ? "_HamzaAboveHorizontalLineAsLetter" : "") +
                                                                ((Book.ElfAboveHorizontalLineAsLetter) ? "_ElfAboveHorizontalLineAsLetter" : "") +
                                                                ((Book.YaaAboveHorizontalLineAsLetter) ? "_YaaAboveHorizontalLineAsLetter" : "") +
                                                                ((Book.NoonAboveHorizontalLineAsLetter) ? "_NoonAboveHorizontalLineAsLetter" : "") +
                                                                ".txt";
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                {
                    writer.WriteLine(word_symmetry_text);
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            FileHelper.DisplayFile(filename);
        }
    }

    private List<Bookmark> m_bookmarks = new List<Bookmark>();
    public List<Bookmark> Bookmarks
    {
        get { return m_bookmarks; }
    }
    private Bookmark m_current_bookmark;
    public Bookmark CurrentBookmark
    {
        get { return m_current_bookmark; }
    }
    private int m_current_bookmark_index = -1;
    public int CurrentBookmarkIndex
    {
        get
        {
            if (m_bookmarks != null)
            {
                for (int i = 0; i < m_bookmarks.Count; i++)
                {
                    if (m_bookmarks[i] == m_current_bookmark)
                    {
                        if (i == m_current_bookmark_index)
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }
    }
    public int GetBookmarkIndex(Bookmark bookmark)
    {
        if (m_bookmarks != null)
        {
            for (int i = 0; i < m_bookmarks.Count; i++)
            {
                if (m_bookmarks[i] == bookmark)
                {
                    return i;
                }
            }
        }
        return -1;
    }
    public Bookmark GetBookmark(Selection selection)
    {
        if (selection != null)
        {
            // selection is mutable so we cannot use ==
            //foreach (Bookmark bookmark in m_bookmarks)
            //{
            //    if (bookmark.Selection == selection)
            //    {
            //        return bookmark;
            //    }
            //}
            return GetBookmark(selection.Scope, selection.Indexes);
        }
        return null;
    }
    public Bookmark GetBookmark(SelectionScope scope, List<int> indexes)
    {
        if (m_bookmarks != null)
        {
            foreach (Bookmark bookmark in m_bookmarks)
            {
                if (bookmark.Selection.Scope == scope)
                {
                    if (bookmark.Selection.Indexes.Count == indexes.Count)
                    {
                        int matching_indexes = 0;
                        for (int i = 0; i < bookmark.Selection.Indexes.Count; i++)
                        {
                            if (bookmark.Selection.Indexes[i] == indexes[i])
                            {
                                matching_indexes++;
                            }
                        }
                        if (indexes.Count == matching_indexes)
                        {
                            return bookmark;
                        }
                    }
                }
            }
        }
        return null;
    }
    public Bookmark GotoBookmark(Selection selection)
    {
        Bookmark bookmark = null;
        if (selection != null)
        {
            bookmark = GetBookmark(selection.Scope, selection.Indexes);
            if (bookmark != null)
            {
                m_current_bookmark = bookmark;
                m_current_bookmark_index = GetBookmarkIndex(bookmark);
            }
        }
        return bookmark;
    }
    public Bookmark GotoBookmark(SelectionScope scope, List<int> indexes)
    {
        Bookmark bookmark = GetBookmark(scope, indexes);
        if (bookmark != null)
        {
            m_current_bookmark = bookmark;
            m_current_bookmark_index = GetBookmarkIndex(bookmark);
        }
        return bookmark;
    }
    public Bookmark GotoNextBookmark()
    {
        if (m_bookmarks != null)
        {
            if (m_bookmarks.Count > 0)
            {
                if (m_current_bookmark_index < m_bookmarks.Count - 1)
                {
                    m_current_bookmark_index++;
                    m_current_bookmark = m_bookmarks[m_current_bookmark_index];
                }
            }
        }
        return m_current_bookmark;
    }
    public Bookmark GotoPreviousBookmark()
    {
        if (m_bookmarks != null)
        {
            if (m_bookmarks.Count > 0)
            {
                if (m_current_bookmark_index > 0)
                {
                    m_current_bookmark_index--;
                    m_current_bookmark = m_bookmarks[m_current_bookmark_index];
                }
            }
        }
        return m_current_bookmark;
    }
    public Bookmark CreateBookmark(Selection selection, string note)
    {
        Bookmark bookmark = GetBookmark(selection.Scope, selection.Indexes);
        if (bookmark != null) // overwrite existing bookmark
        {
            bookmark.Note = note;
            bookmark.LastModifiedTime = DateTime.Now;
            m_current_bookmark = bookmark;
        }
        else // create a new bookmark
        {
            bookmark = new Bookmark(selection, note);
            m_bookmarks.Insert(m_current_bookmark_index + 1, bookmark);
            m_current_bookmark_index++;
            m_current_bookmark = m_bookmarks[m_current_bookmark_index];
        }
        return m_current_bookmark;
    }
    public void AddBookmark(Selection selection, string note, DateTime created_time, DateTime last_modified_time)
    {
        if (m_bookmarks != null)
        {
            Bookmark bookmark = CreateBookmark(selection, note);
            if (bookmark != null)
            {
                bookmark.CreatedTime = created_time;
                bookmark.LastModifiedTime = last_modified_time;
            }
        }
    }
    public void DeleteCurrentBookmark()
    {
        Bookmark current_bookmark = CurrentBookmark;
        if (current_bookmark != null)
        {
            if (m_bookmarks != null)
            {
                m_bookmarks.Remove(current_bookmark);
                if (m_bookmarks.Count == 0) // no bookmark to display
                {
                    m_current_bookmark_index = -1;
                    m_current_bookmark = null;
                }
                else // there are bookmarks still
                {
                    // if index becomes outside list, move back into list
                    if (m_current_bookmark_index == m_bookmarks.Count)
                    {
                        m_current_bookmark_index = m_bookmarks.Count - 1;
                    }
                    m_current_bookmark = m_bookmarks[m_current_bookmark_index];
                }
            }
        }
    }
    public void ClearBookmarks()
    {
        if (m_bookmarks != null)
        {
            m_bookmarks.Clear();
            m_current_bookmark_index = -1;
            m_current_bookmark = null;
        }
    }
    public void SaveBookmarks()
    {
        if (Book != null)
        {
            if (Directory.Exists(Globals.BOOKMARKS_FOLDER))
            {
                string filename = Globals.BOOKMARKS_FOLDER + "/" + Book.TextMode + ".txt";
                try
                {
                    using (StreamWriter writer = new StreamWriter(filename, false, Encoding.Unicode))
                    {
                        if (m_bookmarks != null)
                        {
                            foreach (Bookmark bookmark in m_bookmarks)
                            {
                                if (bookmark.Note.Length > 0)
                                {
                                    string scope_str = bookmark.Selection.Scope.ToString();

                                    StringBuilder str = new StringBuilder();
                                    if (bookmark.Selection.Indexes.Count > 0)
                                    {
                                        for (int i = 0; i < bookmark.Selection.Indexes.Count; i++)
                                        {
                                            str.Append((bookmark.Selection.Indexes[i] + 1).ToString() + "+");
                                        }
                                        if (str.Length > 1)
                                        {
                                            str.Remove(str.Length - 1, 1);
                                        }
                                    }

                                    string created_time = bookmark.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss");
                                    string last_modified_time = bookmark.LastModifiedTime.ToString("yyyy-MM-dd HH:mm:ss");
                                    string note = bookmark.Note;

                                    string line = scope_str + "," + str.ToString() + "," + created_time + "," + last_modified_time + "," + note;
                                    writer.WriteLine(line);
                                }
                            }
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
    public void LoadBookmarks()
    {
        if (Book != null)
        {
            string filename = Globals.BOOKMARKS_FOLDER + "/" + Book.TextMode + ".txt";
            if (File.Exists(filename))
            {
                using (StreamReader reader = File.OpenText(filename))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(',');
                        if (parts.Length == 5)
                        {
                            try
                            {
                                SelectionScope scope = (SelectionScope)Enum.Parse(typeof(SelectionScope), parts[0]);

                                string part = parts[1].Trim();
                                string[] sub_parts = part.Split('+');
                                List<int> indexes = new List<int>();
                                foreach (string sub_part in sub_parts)
                                {
                                    indexes.Add(int.Parse(sub_part.Trim()) - 1);
                                }
                                Selection selection = new Selection(Book, scope, indexes);

                                DateTime created_time = DateTime.ParseExact(parts[2], "yyyy-MM-dd HH:mm:ss", null);
                                DateTime last_modified_time = DateTime.ParseExact(parts[3], "yyyy-MM-dd HH:mm:ss", null);
                                string note = parts[4];

                                AddBookmark(selection, note, created_time, last_modified_time);
                            }
                            catch
                            {
                                throw new Exception("Invalid data format in " + filename);
                            }
                        }
                    }
                }
            }
        }
    }

    public List<string> HelpMessages
    {
        get { return Server.HelpMessages; }
    }
}
