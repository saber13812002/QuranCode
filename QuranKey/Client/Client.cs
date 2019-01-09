using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Model;

public class Client
{
    public Client(string numerology_system_name)
    {
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
    public void BuildSimplifiedBook(string text_mode, bool with_bism_Allah, bool waw_as_word, bool shadda_as_letter)
    {
        Server.BuildSimplifiedBook(text_mode, with_bism_Allah, waw_as_word, shadda_as_letter);
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
        }
    }
    // load and replace current numerology system
    public void LoadNumerologySystem(string numerology_system_name)
    {
        Server.LoadNumerologySystem(numerology_system_name);
    }

    // used for non-Quran text
    public long CalculateValue(char user_char)
    {
        return Server.CalculateValue(user_char);
    }
    public long CalculateValue(string user_text)
    {
        return Server.CalculateValue(user_text);
    }
    // used for Quran text only
    public long CalculateValue(Letter letter)
    {
        return Server.CalculateValue(letter);
    }
    public long CalculateValue(Word word)
    {
        return Server.CalculateValue(word);
    }
    public long CalculateValue(Verse verse)
    {
        return Server.CalculateValue(verse);
    }
    public long CalculateValue(List<Verse> verses)
    {
        return Server.CalculateValue(verses);
    }
    public long CalculateValue(Chapter chapter)
    {
        return Server.CalculateValue(chapter);
    }
    public long CalculateValue(Book book)
    {
        return Server.CalculateValue(book);
    }
    public long CalculateValue(List<Verse> verses, int letter_index_in_verse1, int letter_index_in_verse2)
    {
        return Server.CalculateValue(verses, letter_index_in_verse1, letter_index_in_verse2);
    }
    public List<long> CalculateAllVerseValues(List<Verse> verses)
    {
        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            long value = Server.CalculateValue(verse);
            result.Add(value);
        }
        return result;
    }
    public List<long> CalculateAllWordValues(List<Verse> verses)
    {
        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            foreach (Word word in verse.Words)
            {
                long value = Server.CalculateValue(word);
                result.Add(value);
            }
        }
        return result;
    }
    public List<long> CalculateAllLetterValues(List<Verse> verses)
    {
        List<long> letter_values = new List<long>();
        foreach (Verse verse in verses)
        {
            foreach (Word word in verse.Words)
            {
                foreach (Letter letter in word.Letters)
                {
                    long value = Server.CalculateValue(letter);
                    letter_values.Add(value);
                }
            }
        }
        return letter_values;
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
    public void SaveValueCalculations(string filename, string text)
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
                        writer.WriteLine("----------------------------------------");
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
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine();
                        writer.WriteLine(NumerologySystem.ToOverview());
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine("Text");
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine(text);
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine("Letter" + "\t" + "Value");
                        writer.WriteLine("----------------------------------------");

                        int pos = text.IndexOf("----------------------------------------"); ;
                        text = text.Substring(0, pos);


                        text = text.SimplifyTo(NumerologySystem.TextMode);
                        foreach (char character in text)
                        {
                            if (character == '-')
                            {
                                break;
                            }
                            else if ((character == ' ') || (character == '\r') || (character == '\n'))
                            {
                                writer.WriteLine(" " + "\t" + "");
                            }
                            else
                            {
                                writer.WriteLine(character.ToString() + "\t" + CalculateValue(character.ToString().ToUpper()));
                            }
                        }

                        if (!text.IsArabic())  // eg English
                        {
                            text = text.ToUpper();
                        }
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine("Total" + "\t" + CalculateValue(text));
                        writer.WriteLine("----------------------------------------");
                    }
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            if (File.Exists(filename))
            {
                FileHelper.WaitForReady(filename);

                System.Diagnostics.Process.Start("Notepad.exe", filename);
            }
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
    private void ClearSearchResults()
    {
        m_filter_chapters = null;

        m_found_phrases = new List<Phrase>();

        m_found_sentences = new List<Sentence>();

        m_found_words = new List<Word>();
        m_found_word_ranges = new List<List<Word>>();
        m_found_word_sets = new List<List<Word>>();

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

    private List<LetterStatistic> m_letter_statistics = new List<LetterStatistic>();
    public List<LetterStatistic> LetterStatistics
    {
        get { return m_letter_statistics; }
    }
    public void SortLetterStatistics(StatisticSortMethod sort_method)
    {
        LetterStatistic.SortMethod = sort_method;
        m_letter_statistics.Sort();
        if (LetterStatistic.SortOrder == StatisticSortOrder.Ascending)
        {
            LetterStatistic.SortOrder = StatisticSortOrder.Descending;
        }
        else
        {
            LetterStatistic.SortOrder = StatisticSortOrder.Ascending;
        }
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
                text = text.Replace(Constants.OPEN_BRACKET, "");
                text = text.Replace(Constants.CLOSE_BRACKET, "");
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
                            }
                        }

                        if (!is_found)
                        {
                            LetterStatistic letter_statistic = new LetterStatistic();
                            letter_statistic.Order = m_letter_statistics.Count + 1;
                            letter_statistic.Letter = text[i];
                            letter_statistic.Frequency = 1;
                            m_letter_statistics.Add(letter_statistic);
                        }
                    }
                }
            }
        }
    }
    public void SaveLetterStatistics(string filename, string text)
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
                        writer.WriteLine("----------------------------------------");
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
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine();
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine("Text");
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine(text);
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine();
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine("Order" + "\t" + "Letter" + "\t" + "Frequency");
                        writer.WriteLine("----------------------------------------");
                        int count = 0;
                        int sum = 0;
                        foreach (LetterStatistic letter_statistic in m_letter_statistics)
                        {
                            writer.WriteLine(letter_statistic.Order.ToString() + "\t" + letter_statistic.Letter.ToString() + '\t' + letter_statistic.Frequency.ToString());
                            count++;
                            sum += letter_statistic.Frequency;
                        }
                        writer.WriteLine("----------------------------------------");
                        writer.WriteLine("Total" + "\t" + count.ToString() + "\t" + sum.ToString());
                        writer.WriteLine("----------------------------------------");
                    }
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            if (File.Exists(filename))
            {
                FileHelper.WaitForReady(filename);

                System.Diagnostics.Process.Start("Notepad.exe", filename);
            }
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
                text = text.Replace(Constants.OPEN_BRACKET, "");
                text = text.Replace(Constants.CLOSE_BRACKET, "");
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
                            writer.WriteLine("----------------------------------------");
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
                            writer.WriteLine("----------------------------------------");
                            writer.WriteLine();
                            writer.WriteLine("----------------------------------------");
                            writer.WriteLine("Text");
                            writer.WriteLine("----------------------------------------");
                            writer.WriteLine(text);
                            writer.WriteLine("----------------------------------------");
                            writer.WriteLine();
                            writer.WriteLine("----------------------------------------");
                            writer.WriteLine("Phrase");
                            writer.WriteLine("----------------------------------------");
                            writer.WriteLine(phrase);
                            writer.WriteLine("----------------------------------------");
                            writer.WriteLine();
                            writer.WriteLine("----------------------------------------");
                            writer.WriteLine("Order" + "\t" + "Letter" + "\t" + "Frequency");
                            writer.WriteLine("----------------------------------------");
                            int count = m_letter_statistics.Count;
                            int sum = 0;
                            for (int i = 0; i < count; i++)
                            {
                                writer.WriteLine(m_letter_statistics[i].Order.ToString() + "\t" + m_letter_statistics[i].Letter.ToString() + '\t' + m_letter_statistics[i].Frequency.ToString());
                                sum += m_letter_statistics[i].Frequency;
                            }
                            writer.WriteLine("----------------------------------------");
                            writer.WriteLine("Total" + "\t" + count.ToString() + "\t" + sum.ToString());
                            writer.WriteLine("----------------------------------------");
                        }
                    }
                }
            }
            catch
            {
                // silence IO error in case running from read-only media (CD/DVD)
            }

            // show file content after save
            if (File.Exists(filename))
            {
                FileHelper.WaitForReady(filename);

                System.Diagnostics.Process.Start("Notepad.exe", filename);
            }
        }
    }
}
