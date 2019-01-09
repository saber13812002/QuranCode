using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Model
{
    public class Book
    {
        private string text_mode = null;
        public string TextMode
        {
            get { return text_mode; }
            set { text_mode = value; }
        }

        private bool with_diacritics = true;
        public bool WithDiacritics
        {
            get { return with_diacritics; }
            set { with_diacritics = value; }
        }

        private bool with_bism_Allah = true;
        public bool WithBismAllah
        {
            get { return with_bism_Allah; }
            set { with_bism_Allah = value; }
        }

        private bool waw_as_word = false;
        public bool WawAsWord
        {
            get { return waw_as_word; }
            set { waw_as_word = value; }
        }

        private bool shadda_as_letter = false;
        public bool ShaddaAsLetter
        {
            get { return shadda_as_letter; }
            set { shadda_as_letter = value; }
        }

        private List<Chapter> chapters = null;
        public List<Chapter> Chapters
        {
            get { return chapters; }
        }

        private List<Station> stations = null;
        public List<Station> Stations
        {
            get { return stations; }
        }

        private List<Part> parts = null;
        public List<Part> Parts
        {
            get { return parts; }
        }

        private List<Group> groups = null;
        public List<Group> Groups
        {
            get { return groups; }
        }

        private List<Half> halfs = null;
        public List<Half> Halfs
        {
            get { return halfs; }
        }

        private List<Quarter> quarters = null;
        public List<Quarter> Quarters
        {
            get { return quarters; }
        }

        private List<Bowing> bowings = null;
        public List<Bowing> Bowings
        {
            get { return bowings; }
        }

        private List<Page> pages = null;
        public List<Page> Pages
        {
            get { return pages; }
        }

        private List<Verse> verses = null;
        public List<Verse> Verses
        {
            get { return verses; }
        }

        public Word GetWord(int index)
        {
            if (this.verses != null)
            {
                foreach (Verse verse in this.verses)
                {
                    if (verse.Words != null)
                    {
                        if (index >= verse.Words.Count)
                        {
                            index -= verse.Words.Count;
                        }
                        else if (index >= 0)
                        {
                            return verse.Words[index];
                        }
                    }
                }
            }
            return null;
        }
        private int word_count = 0;
        public int WordCount
        {
            get
            {
                if (word_count <= 0)
                {
                    if (this.verses != null)
                    {
                        foreach (Verse verse in this.verses)
                        {
                            if (verse.Words != null)
                            {
                                word_count += verse.Words.Count;
                            }
                        }
                    }
                }
                return word_count;
            }
        }

        public Letter GetLetter(int index)
        {
            if (this.verses != null)
            {
                foreach (Verse verse in this.verses)
                {
                    if (verse.Words != null)
                    {
                        foreach (Word word in verse.Words)
                        {
                            if ((word.Letters != null) && (word.Letters.Count > 0))
                            {
                                if (index >= word.Letters.Count)
                                {
                                    index -= word.Letters.Count;
                                }
                                else if (index >= 0)
                                {
                                    return word.Letters[index];
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        private int letter_count = 0;
        public int LetterCount
        {
            get
            {
                if (letter_count <= 0)
                {
                    if (this.verses != null)
                    {
                        foreach (Verse verse in this.verses)
                        {
                            if (verse.Words != null)
                            {
                                foreach (Word word in verse.Words)
                                {
                                    if ((word.Letters != null) && (word.Letters.Count > 0))
                                    {
                                        letter_count += word.Letters.Count;
                                    }
                                }
                            }
                        }
                    }
                }
                return letter_count;
            }
        }

        private List<char> unique_letters = null;
        public List<char> UniqueLetters
        {
            get
            {
                if (unique_letters == null)
                {
                    unique_letters = new List<char>();
                    if (this.verses != null)
                    {
                        foreach (Verse verse in this.verses)
                        {
                            if (verse.Words != null)
                            {
                                foreach (Word word in verse.Words)
                                {
                                    if (word.UniqueLetters != null)
                                    {
                                        foreach (char character in word.UniqueLetters)
                                        {
                                            if (!unique_letters.Contains(character))
                                            {
                                                unique_letters.Add(character);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return unique_letters;
            }
        }
        public int GetLetterFrequency(char character)
        {
            int result = 0;
            if (this.verses != null)
            {
                foreach (Verse verse in this.verses)
                {
                    if (verse.Words != null)
                    {
                        foreach (Word word in verse.Words)
                        {
                            if ((word.Letters != null) && (word.Letters.Count > 0))
                            {
                                foreach (Letter letter in word.Letters)
                                {
                                    if (letter.Character == character)
                                    {
                                        result++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private Dictionary<string, List<string>> unique_wordss = null;
        private List<string> UniqueWords
        {
            get
            {
                if (unique_wordss == null)
                {
                    unique_wordss = new Dictionary<string, List<string>>();
                }
                List<string> unique_words = new List<string>();

                if (this.verses != null)
                {
                    foreach (Verse verse in this.verses)
                    {
                        if (verse.Words != null)
                        {
                            foreach (Word word in verse.Words)
                            {
                                if (!unique_words.Contains(word.Text))
                                {
                                    unique_words.Add(word.Text);
                                }
                            }
                        }
                    }
                }
                return unique_words;
            }
        }

        public int GetVerseNumber(int chapter_number, int verse_number_in_chapter)
        {
            if (this.chapters != null)
            {
                foreach (Chapter chapter in this.chapters)
                {
                    if (chapter.SortedNumber == chapter_number)
                    {
                        if (chapter.Verses != null)
                        {
                            foreach (Verse verse in chapter.Verses)
                            {
                                if (verse.NumberInChapter == verse_number_in_chapter)
                                {
                                    return verse.Number;
                                }
                            }
                        }
                    }
                }
            }
            return 0;
        }
        public Verse GetVerseByVerseNumber(int number)
        {
            if (this.verses != null)
            {
                number %= verses.Count;
                if ((number > 0) && (number <= this.verses.Count))
                {
                    return this.verses[number - 1];
                }
            }
            return null;
        }
        public Verse GetVerseByWordNumber(int number)
        {
            number %= WordCount;
            if ((number > 0) && (number <= this.WordCount))
            {
                if (this.chapters != null)
                {
                    foreach (Chapter chapter in this.chapters)
                    {
                        if (chapter.Verses != null)
                        {
                            foreach (Verse verse in chapter.Verses)
                            {
                                if (number > verse.Words.Count)
                                {
                                    number -= verse.Words.Count;
                                }
                                else
                                {
                                    return verse;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        public Verse GetVerseByLetterNumber(int number)
        {
            number %= LetterCount;
            if ((number > 0) && (number <= this.LetterCount))
            {
                if (this.chapters != null)
                {
                    foreach (Chapter chapter in this.chapters)
                    {
                        if (chapter.Verses != null)
                        {
                            foreach (Verse verse in chapter.Verses)
                            {
                                int letter_count = verse.LetterCount;
                                if (number > letter_count)
                                {
                                    number -= letter_count;
                                }
                                else
                                {
                                    return verse;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        public Word GetWordByWordNumber(int number)
        {
            number %= WordCount;
            if ((number > 0) && (number <= this.WordCount))
            {
                if (this.chapters != null)
                {
                    foreach (Chapter chapter in this.chapters)
                    {
                        if (chapter.Verses != null)
                        {
                            foreach (Verse verse in chapter.Verses)
                            {
                                if (verse.Words != null)
                                {
                                    int word_count = verse.Words.Count;
                                    if (number > word_count)
                                    {
                                        number -= word_count;
                                    }
                                    else
                                    {
                                        return verse.Words[number - 1];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        public Word GetWordByLetterNumber(int number)
        {
            number %= LetterCount;
            if ((number > 0) && (number <= this.LetterCount))
            {
                if (this.chapters != null)
                {
                    foreach (Chapter chapter in this.chapters)
                    {
                        if (chapter.Verses != null)
                        {
                            foreach (Verse verse in chapter.Verses)
                            {
                                if (verse.Words != null)
                                {
                                    foreach (Word word in verse.Words)
                                    {
                                        if ((word.Letters != null) && (word.Letters.Count > 0))
                                        {
                                            int letter_count = word.Letters.Count;
                                            if (number > letter_count)
                                            {
                                                number -= letter_count;
                                            }
                                            else
                                            {
                                                return word;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public Book(string text_mode, List<Verse> verses)
        {
            this.text_mode = text_mode;

            SetupPartitions(verses);

            SetupBook();
        }
        private void SetupPartitions(List<Verse> verses)
        {
            if (verses != null)
            {
                this.verses = verses;
                foreach (Verse verse in this.verses)
                {
                    verse.Book = this;
                }

                this.chapters = new List<Chapter>();
                this.stations = new List<Station>();
                this.parts = new List<Part>();
                this.groups = new List<Group>();
                this.halfs = new List<Half>();
                this.quarters = new List<Quarter>();
                this.bowings = new List<Bowing>();
                this.pages = new List<Page>();

                this.min_words = 1;
                int word_count = 0;
                foreach (Verse verse in this.verses)
                {
                    if (verse.Words != null)
                    {
                        word_count += verse.Words.Count;
                    }
                }
                this.max_words = word_count;

                this.min_letters = 1;
                this.max_letters = int.MaxValue; // verse.Letters is not populated yet

                if (s_quran_metadata == null)
                {
                    LoadQuranMetadata();
                }

                if (s_quran_metadata != null)
                {
                    // setup Chapters
                    for (int i = 0; i < s_quran_metadata.Chapters.Count; i++)
                    {
                        int number = s_quran_metadata.Chapters[i].Number;
                        int verse_count = s_quran_metadata.Chapters[i].Verses;
                        int first_verse = s_quran_metadata.Chapters[i].FirstVerse;
                        int last_verse = first_verse + verse_count;
                        string name = s_quran_metadata.Chapters[i].Name;
                        string transliterated_name = s_quran_metadata.Chapters[i].TransliteratedName;
                        string english_name = s_quran_metadata.Chapters[i].EnglishName;
                        RevelationPlace revelation_place = s_quran_metadata.Chapters[i].RevelationPlace;
                        int revelation_order = s_quran_metadata.Chapters[i].RevelationOrder;
                        int bowing_count = s_quran_metadata.Chapters[i].Bowings;

                        List<Verse> chapter_verses = new List<Verse>();
                        if (this.verses != null)
                        {
                            for (int j = first_verse; j < last_verse; j++)
                            {
                                int index = j - 1;
                                if ((index >= 0) && (index < this.verses.Count))
                                {
                                    Verse verse = this.verses[index];
                                    chapter_verses.Add(verse);
                                }
                            }

                            Chapter chapter = new Chapter(this, number, name, transliterated_name, english_name, revelation_place, revelation_order, bowing_count, chapter_verses);
                            this.chapters.Add(chapter);
                        }
                    }

                    // setup Stations
                    for (int i = 0; i < s_quran_metadata.Stations.Count; i++)
                    {
                        int number = s_quran_metadata.Stations[i].Number;
                        int start_chapter = s_quran_metadata.Stations[i].StartChapter;
                        int start_chapter_verse = s_quran_metadata.Stations[i].StartChapterVerse;

                        int first_verse = 0;
                        for (int j = 0; j < start_chapter - 1; j++)
                        {
                            first_verse += this.chapters[j].Verses.Count;
                        }
                        first_verse += start_chapter_verse;

                        int next_station_first_verse = 0;
                        if (i < s_quran_metadata.Stations.Count - 1)
                        {
                            int next_station_start_chapter = s_quran_metadata.Stations[i + 1].StartChapter;
                            int next_station_start_chapter_verse = s_quran_metadata.Stations[i + 1].StartChapterVerse;
                            for (int j = 0; j < next_station_start_chapter - 1; j++)
                            {
                                next_station_first_verse += this.chapters[j].Verses.Count;
                            }
                            next_station_first_verse += next_station_start_chapter_verse;
                        }
                        else
                        {
                            next_station_first_verse = this.verses.Count + 1; // beyond end
                        }

                        int last_verse = next_station_first_verse;

                        List<Verse> station_verses = new List<Verse>();
                        for (int j = first_verse; j < last_verse; j++)
                        {
                            int index = j - 1;
                            if ((index >= 0) && (index < this.verses.Count))
                            {
                                Verse verse = this.verses[index];
                                station_verses.Add(verse);
                            }
                        }

                        Station station = new Station(this, number, station_verses);
                        this.stations.Add(station);
                    }

                    // setup Parts
                    for (int i = 0; i < s_quran_metadata.Parts.Count; i++)
                    {
                        int number = s_quran_metadata.Parts[i].Number;
                        int start_chapter = s_quran_metadata.Parts[i].StartChapter;
                        int start_chapter_verse = s_quran_metadata.Parts[i].StartChapterVerse;

                        int first_verse = 0;
                        for (int j = 0; j < start_chapter - 1; j++)
                        {
                            first_verse += this.chapters[j].Verses.Count;
                        }
                        first_verse += start_chapter_verse;

                        int next_part_first_verse = 0;
                        if (i < s_quran_metadata.Parts.Count - 1)
                        {
                            int next_part_start_chapter = s_quran_metadata.Parts[i + 1].StartChapter;
                            int next_part_start_chapter_verse = s_quran_metadata.Parts[i + 1].StartChapterVerse;
                            for (int j = 0; j < next_part_start_chapter - 1; j++)
                            {
                                next_part_first_verse += this.chapters[j].Verses.Count;
                            }
                            next_part_first_verse += next_part_start_chapter_verse;
                        }
                        else
                        {
                            next_part_first_verse = this.verses.Count + 1; // beyond end
                        }

                        int last_verse = next_part_first_verse;

                        List<Verse> part_verses = new List<Verse>();
                        for (int j = first_verse; j < last_verse; j++)
                        {
                            int index = j - 1;
                            if ((index >= 0) && (index < this.verses.Count))
                            {
                                Verse verse = this.verses[index];
                                part_verses.Add(verse);
                            }
                        }

                        Part part = new Part(this, number, part_verses);
                        this.parts.Add(part);
                    }

                    // setup Group
                    for (int i = 0; i < s_quran_metadata.Groups.Count; i++)
                    {
                        int number = s_quran_metadata.Groups[i].Number;
                        int start_chapter = s_quran_metadata.Groups[i].StartChapter;
                        int start_chapter_verse = s_quran_metadata.Groups[i].StartChapterVerse;

                        int first_verse = 0;
                        for (int j = 0; j < start_chapter - 1; j++)
                        {
                            first_verse += this.chapters[j].Verses.Count;
                        }
                        first_verse += start_chapter_verse;

                        int next_group_first_verse = 0;
                        if (i < s_quran_metadata.Groups.Count - 1)
                        {
                            int next_group_start_chapter = s_quran_metadata.Groups[i + 1].StartChapter;
                            int next_group_start_chapter_verse = s_quran_metadata.Groups[i + 1].StartChapterVerse;
                            for (int j = 0; j < next_group_start_chapter - 1; j++)
                            {
                                next_group_first_verse += this.chapters[j].Verses.Count;
                            }
                            next_group_first_verse += next_group_start_chapter_verse;
                        }
                        else
                        {
                            next_group_first_verse = this.verses.Count + 1; // beyond end
                        }

                        int last_verse = next_group_first_verse;

                        List<Verse> group_verses = new List<Verse>();
                        for (int j = first_verse; j < last_verse; j++)
                        {
                            int index = j - 1;
                            if ((index >= 0) && (index < this.verses.Count))
                            {
                                Verse verse = this.verses[index];
                                group_verses.Add(verse);
                            }
                        }

                        Group group = new Group(this, number, group_verses);
                        this.groups.Add(group);
                    }

                    // setup Halfs
                    for (int i = 0; i < s_quran_metadata.Halfs.Count; i++)
                    {
                        int number = s_quran_metadata.Halfs[i].Number;
                        int start_chapter = s_quran_metadata.Halfs[i].StartChapter;
                        int start_chapter_verse = s_quran_metadata.Halfs[i].StartChapterVerse;

                        int first_verse = 0;
                        for (int j = 0; j < start_chapter - 1; j++)
                        {
                            first_verse += this.chapters[j].Verses.Count;
                        }
                        first_verse += start_chapter_verse;

                        int next_half_first_verse = 0;
                        if (i < s_quran_metadata.Halfs.Count - 1)
                        {
                            int next_half_start_chapter = s_quran_metadata.Halfs[i + 1].StartChapter;
                            int next_half_start_chapter_verse = s_quran_metadata.Halfs[i + 1].StartChapterVerse;
                            for (int j = 0; j < next_half_start_chapter - 1; j++)
                            {
                                next_half_first_verse += this.chapters[j].Verses.Count;
                            }
                            next_half_first_verse += next_half_start_chapter_verse;
                        }
                        else
                        {
                            next_half_first_verse = this.verses.Count + 1; // beyond end
                        }

                        int last_verse = next_half_first_verse;

                        List<Verse> half_verses = new List<Verse>();
                        for (int j = first_verse; j < last_verse; j++)
                        {
                            int index = j - 1;
                            if ((index >= 0) && (index < this.verses.Count))
                            {
                                Verse verse = this.verses[index];
                                half_verses.Add(verse);
                            }
                        }

                        Half half = new Half(this, number, half_verses);
                        this.halfs.Add(half);
                    }

                    // setup Quarters
                    for (int i = 0; i < s_quran_metadata.Quarters.Count; i++)
                    {
                        int number = s_quran_metadata.Quarters[i].Number;
                        int start_chapter = s_quran_metadata.Quarters[i].StartChapter;
                        int start_chapter_verse = s_quran_metadata.Quarters[i].StartChapterVerse;

                        int first_verse = 0;
                        for (int j = 0; j < start_chapter - 1; j++)
                        {
                            first_verse += this.chapters[j].Verses.Count;
                        }
                        first_verse += start_chapter_verse;

                        int next_quarter_first_verse = 0;
                        if (i < s_quran_metadata.Quarters.Count - 1)
                        {
                            int next_quarter_start_chapter = s_quran_metadata.Quarters[i + 1].StartChapter;
                            int next_quarter_start_chapter_verse = s_quran_metadata.Quarters[i + 1].StartChapterVerse;
                            for (int j = 0; j < next_quarter_start_chapter - 1; j++)
                            {
                                next_quarter_first_verse += this.chapters[j].Verses.Count;
                            }
                            next_quarter_first_verse += next_quarter_start_chapter_verse;
                        }
                        else
                        {
                            next_quarter_first_verse = this.verses.Count + 1; // beyond end
                        }

                        int last_verse = next_quarter_first_verse;

                        List<Verse> quarter_verses = new List<Verse>();
                        for (int j = first_verse; j < last_verse; j++)
                        {
                            int index = j - 1;
                            if ((index >= 0) && (index < this.verses.Count))
                            {
                                Verse verse = this.verses[index];
                                quarter_verses.Add(verse);
                            }
                        }

                        Quarter quarter = new Quarter(this, number, quarter_verses);
                        this.quarters.Add(quarter);
                    }

                    // setup Bowings
                    for (int i = 0; i < s_quran_metadata.Bowings.Count; i++)
                    {
                        int number = s_quran_metadata.Bowings[i].Number;
                        int start_chapter = s_quran_metadata.Bowings[i].StartChapter;
                        int start_chapter_verse = s_quran_metadata.Bowings[i].StartChapterVerse;

                        int first_verse = 0;
                        for (int j = 0; j < start_chapter - 1; j++)
                        {
                            first_verse += this.chapters[j].Verses.Count;
                        }
                        first_verse += start_chapter_verse;

                        int next_bowing_first_verse = 0;
                        if (i < s_quran_metadata.Bowings.Count - 1)
                        {
                            int next_bowing_start_chapter = s_quran_metadata.Bowings[i + 1].StartChapter;
                            int next_bowing_start_chapter_verse = s_quran_metadata.Bowings[i + 1].StartChapterVerse;
                            for (int j = 0; j < next_bowing_start_chapter - 1; j++)
                            {
                                next_bowing_first_verse += this.chapters[j].Verses.Count;
                            }
                            next_bowing_first_verse += next_bowing_start_chapter_verse;
                        }
                        else
                        {
                            next_bowing_first_verse = this.verses.Count + 1; // beyond end
                        }

                        int last_verse = next_bowing_first_verse;

                        List<Verse> bowing_verses = new List<Verse>();
                        for (int j = first_verse; j < last_verse; j++)
                        {
                            int index = j - 1;
                            if ((index >= 0) && (index < this.verses.Count))
                            {
                                Verse verse = this.verses[index];
                                bowing_verses.Add(verse);
                            }
                        }

                        Bowing bowing = new Bowing(this, number, bowing_verses);
                        this.bowings.Add(bowing);
                    }

                    // setup Pages
                    for (int i = 0; i < s_quran_metadata.Pages.Count; i++)
                    {
                        int number = s_quran_metadata.Pages[i].Number;
                        int start_chapter = s_quran_metadata.Pages[i].StartChapter;
                        int start_chapter_verse = s_quran_metadata.Pages[i].StartChapterVerse;

                        int first_verse = 0;
                        for (int j = 0; j < start_chapter - 1; j++)
                        {
                            first_verse += this.chapters[j].Verses.Count;
                        }
                        first_verse += start_chapter_verse;

                        int next_page_first_verse = 0;
                        if (i < s_quran_metadata.Pages.Count - 1)
                        {
                            int next_page_start_chapter = s_quran_metadata.Pages[i + 1].StartChapter;
                            int next_page_start_chapter_verse = s_quran_metadata.Pages[i + 1].StartChapterVerse;
                            for (int j = 0; j < next_page_start_chapter - 1; j++)
                            {
                                next_page_first_verse += this.chapters[j].Verses.Count;
                            }
                            next_page_first_verse += next_page_start_chapter_verse;
                        }
                        else
                        {
                            next_page_first_verse = this.verses.Count + 1; // beyond end
                        }

                        int last_verse = next_page_first_verse;

                        List<Verse> page_verses = new List<Verse>();
                        for (int j = first_verse; j < last_verse; j++)
                        {
                            int index = j - 1;
                            if ((index >= 0) && (index < this.verses.Count))
                            {
                                Verse verse = this.verses[index];
                                page_verses.Add(verse);
                            }
                        }

                        Page page = new Page(this, number, page_verses);
                        this.pages.Add(page);
                    }

                    // setup Prostration
                    for (int i = 0; i < s_quran_metadata.Prostrations.Count; i++)
                    {
                        int number = s_quran_metadata.Prostrations[i].Number;
                        int chapter = s_quran_metadata.Prostrations[i].Chapter;
                        int chapter_verse = s_quran_metadata.Prostrations[i].ChapterVerse;
                        ProstrationType type = s_quran_metadata.Prostrations[i].Type;

                        int index = -1;
                        for (int j = 0; j < chapter - 1; j++)
                        {
                            index += this.chapters[j].Verses.Count;
                        }
                        index += chapter_verse;

                        if ((index > 0) && (index < this.verses.Count))
                        {
                            Verse verse = this.verses[index];
                            verse.ProstrationType = type;
                            if (verse.ProstrationType == ProstrationType.Recommended)
                            {
                                verse.Text = verse.Text.Replace("۩", "⌂");
                            }
                        }
                    }

                    // setup Initialization
                    for (int i = 0; i < s_quran_metadata.Initializations.Count; i++)
                    {
                        int number = s_quran_metadata.Initializations[i].Number;
                        int chapter = s_quran_metadata.Initializations[i].Chapter;
                        int chapter_verse = s_quran_metadata.Initializations[i].ChapterVerse;
                        InitializationType type = s_quran_metadata.Initializations[i].Type;

                        int index = -1;
                        for (int j = 0; j < chapter - 1; j++)
                        {
                            index += this.chapters[j].Verses.Count;
                        }
                        index += chapter_verse;

                        if ((index > 0) && (index < this.verses.Count))
                        {
                            Verse verse = this.verses[index];
                            verse.InitializationType = type;

                            if (verse.Chapter.Number == 1)
                            {
                                this.chapters[chapter - 1].InitializationType = InitializationType.Key;
                            }
                            else if (verse.Chapter.Number == 42)
                            {
                                this.chapters[chapter - 1].InitializationType = InitializationType.DoublyInitialized;
                            }
                            else
                            {
                                this.chapters[chapter - 1].InitializationType = type;
                            }
                        }
                    }
                }
            }
        }
        public void SetupBook()
        {
            SetupNumbers();
            SetupWordOccurrences();
            SetupWordFrequencies();
        }
        private void SetupNumbers()
        {
            int chapter_number = 1;
            int verse_number = 1;
            int word_number = 1;
            int letter_number = 1;

            if (this.chapters != null)
            {
                foreach (Chapter chapter in this.chapters)
                {
                    chapter.SortedNumber = chapter_number++;

                    int verse_number_in_chapter = 1;
                    int word_number_in_chapter = 1;
                    int letter_number_in_chapter = 1;
                    foreach (Verse verse in chapter.Verses)
                    {
                        verse.Number = verse_number++;
                        verse.NumberInChapter = verse_number_in_chapter++;

                        int word_number_in_verse = 1;
                        int letter_number_in_verse = 1;
                        if (verse.Words != null)
                        {
                            foreach (Word word in verse.Words)
                            {
                                word.Number = word_number++;
                                word.NumberInChapter = word_number_in_chapter++;
                                word.NumberInVerse = word_number_in_verse++;

                                int letter_number_in_word = 1;
                                foreach (Letter letter in word.Letters)
                                {
                                    letter.Number = letter_number++;
                                    letter.NumberInChapter = letter_number_in_chapter++;
                                    letter.NumberInVerse = letter_number_in_verse++;
                                    letter.NumberInWord = letter_number_in_word++;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void SetupWordOccurrences()
        {
            Dictionary<string, int> frequencies = new Dictionary<string, int>();
            foreach (Verse verse in this.Verses)
            {
                foreach (Word word in verse.Words)
                {
                    string word_text = word.Text;
                    if (text_mode == "Original")
                    {
                        word_text = word_text.Simplify29();
                    }

                    if (frequencies.ContainsKey(word_text))
                    {
                        frequencies[word_text]++;
                        word.Occurrence = frequencies[word_text];
                    }
                    else
                    {
                        frequencies.Add(word_text, 1);
                        word.Occurrence = 1;
                    }
                }
            }
        }
        private void SetupWordFrequencies()
        {
            Dictionary<string, int> frequencies = new Dictionary<string, int>();
            foreach (Verse verse in this.Verses)
            {
                foreach (Word word in verse.Words)
                {
                    string word_text = word.Text;
                    if (text_mode == "Original")
                    {
                        word_text = word_text.Simplify29();
                    }

                    if (frequencies.ContainsKey(word_text))
                    {
                        frequencies[word_text]++;
                    }
                    else
                    {
                        frequencies.Add(word_text, 1);
                    }
                }
            }
            foreach (Verse verse in this.Verses)
            {
                foreach (Word word in verse.Words)
                {
                    string word_text = word.Text;
                    if (text_mode == "Original")
                    {
                        word_text = word_text.Simplify29();
                    }

                    if (frequencies.ContainsKey(word_text))
                    {
                        word.Frequency = frequencies[word_text];
                    }
                    else
                    {
                        word.Frequency = 0;
                    }
                }
            }
        }

        private class QuranMetadataChapter
        {
            public int Number;
            public int Verses;
            public int FirstVerse;
            public string Name;
            public string TransliteratedName;
            public string EnglishName;
            public RevelationPlace RevelationPlace;
            public int RevelationOrder;
            public int Bowings;
        }
        private class QuranMetadataStation
        {
            public int Number;
            public int StartChapter;
            public int StartChapterVerse;
        }
        private class QuranMetadataPart
        {
            public int Number;
            public int StartChapter;
            public int StartChapterVerse;
        }
        private class QuranMetadataGroup
        {
            public int Number;
            public int StartChapter;
            public int StartChapterVerse;
        }
        private class QuranMetadataHalf
        {
            public int Number;
            public int StartChapter;
            public int StartChapterVerse;
        }
        private class QuranMetadataQuarter
        {
            public int Number;
            public int StartChapter;
            public int StartChapterVerse;
        }
        private class QuranMetadataBowing
        {
            public int Number;
            public int StartChapter;
            public int StartChapterVerse;
        }
        private class QuranMetadataPage
        {
            public int Number;
            public int StartChapter;
            public int StartChapterVerse;
        }
        private class QuranMetadataProstration
        {
            public int Number;
            public int Chapter;
            public int ChapterVerse;
            public ProstrationType Type;
        }
        private class QuranMetadataInitialization
        {
            public int Number;
            public int Chapter;
            public int ChapterVerse;
            public InitializationType Type;
        }
        private class QuranMetadata
        {
            public List<QuranMetadataChapter> Chapters = new List<QuranMetadataChapter>();
            public List<QuranMetadataStation> Stations = new List<QuranMetadataStation>();
            public List<QuranMetadataPart> Parts = new List<QuranMetadataPart>();
            public List<QuranMetadataGroup> Groups = new List<QuranMetadataGroup>();
            public List<QuranMetadataHalf> Halfs = new List<QuranMetadataHalf>();
            public List<QuranMetadataQuarter> Quarters = new List<QuranMetadataQuarter>();
            public List<QuranMetadataBowing> Bowings = new List<QuranMetadataBowing>();
            public List<QuranMetadataPage> Pages = new List<QuranMetadataPage>();
            public List<QuranMetadataProstration> Prostrations = new List<QuranMetadataProstration>();
            public List<QuranMetadataInitialization> Initializations = new List<QuranMetadataInitialization>();
        }
        private static QuranMetadata s_quran_metadata = null;
        private static string END_OF_SECTION = "";
        private static void LoadQuranMetadata()
        {
            if (Directory.Exists(Globals.DATA_FOLDER))
            {
                string filename = Globals.DATA_FOLDER + "/" + "quran-metadata.txt";
                if (File.Exists(filename))
                {
                    try
                    {
                        s_quran_metadata = new QuranMetadata();

                        using (StreamReader reader = File.OpenText(filename))
                        {
                            string line = null;
                            while ((line = reader.ReadLine()) != null)
                            {
                                switch (line)
                                {
                                    case "chapter\tverses\tfirst_verse\tname\ttransliterated_name\tenglish_name\trevelation_place\trevelation_order\tbowings":
                                        {
                                            while ((line = reader.ReadLine()) != null)
                                            {
                                                if (line == END_OF_SECTION) break;

                                                string[] parts = line.Split('\t');
                                                if (parts.Length != 9) throw new Exception("Invalid Chapter metadata in " + filename);

                                                QuranMetadataChapter info = new QuranMetadataChapter();
                                                info.Number = int.Parse(parts[0]);
                                                info.Verses = int.Parse(parts[1]);
                                                info.FirstVerse = int.Parse(parts[2]);
                                                info.Name = parts[3];
                                                info.TransliteratedName = parts[4];
                                                info.EnglishName = parts[5];
                                                info.RevelationPlace = (RevelationPlace)Enum.Parse(typeof(RevelationPlace), parts[6]);
                                                info.RevelationOrder = int.Parse(parts[7]);
                                                info.Bowings = int.Parse(parts[8]);
                                                s_quran_metadata.Chapters.Add(info);
                                            }
                                        }
                                        break;
                                    case "station\tstart_chapter\tstart_chapter_verse":
                                        {
                                            while ((line = reader.ReadLine()) != null)
                                            {
                                                if (line == END_OF_SECTION) break;

                                                string[] parts = line.Split('\t');
                                                if (parts.Length != 3) throw new Exception("Invalid Station metadata in " + filename);

                                                QuranMetadataStation info = new QuranMetadataStation();
                                                info.Number = int.Parse(parts[0]);
                                                info.StartChapter = int.Parse(parts[1]);
                                                info.StartChapterVerse = int.Parse(parts[2]);
                                                s_quran_metadata.Stations.Add(info);
                                            }
                                        }
                                        break;
                                    case "part\tstart_chapter\tstart_chapter_verse":
                                        {
                                            while ((line = reader.ReadLine()) != null)
                                            {
                                                if (line == END_OF_SECTION) break;

                                                string[] parts = line.Split('\t');
                                                if (parts.Length != 3) throw new Exception("Invalid Part metadata in " + filename);

                                                QuranMetadataPart info = new QuranMetadataPart();
                                                info.Number = int.Parse(parts[0]);
                                                info.StartChapter = int.Parse(parts[1]);
                                                info.StartChapterVerse = int.Parse(parts[2]);
                                                s_quran_metadata.Parts.Add(info);
                                            }
                                        }
                                        break;
                                    case "group\tstart_chapter\tstart_chapter_verse":
                                        {
                                            while ((line = reader.ReadLine()) != null)
                                            {
                                                if (line == END_OF_SECTION) break;

                                                string[] parts = line.Split('\t');
                                                if (parts.Length != 3) throw new Exception("Invalid Group metadata in " + filename);

                                                QuranMetadataGroup info = new QuranMetadataGroup();
                                                info.Number = int.Parse(parts[0]);
                                                info.StartChapter = int.Parse(parts[1]);
                                                info.StartChapterVerse = int.Parse(parts[2]);
                                                s_quran_metadata.Groups.Add(info);
                                            }
                                        }
                                        break;
                                    case "half\tstart_chapter\tstart_chapter_verse":
                                        {
                                            while ((line = reader.ReadLine()) != null)
                                            {
                                                if (line == END_OF_SECTION) break;

                                                string[] parts = line.Split('\t');
                                                if (parts.Length != 3) throw new Exception("Invalid Half metadata in " + filename);

                                                QuranMetadataHalf info = new QuranMetadataHalf();
                                                info.Number = int.Parse(parts[0]);
                                                info.StartChapter = int.Parse(parts[1]);
                                                info.StartChapterVerse = int.Parse(parts[2]);
                                                s_quran_metadata.Halfs.Add(info);
                                            }
                                        }
                                        break;
                                    case "quarter\tstart_chapter\tstart_chapter_verse":
                                        {
                                            while ((line = reader.ReadLine()) != null)
                                            {
                                                if (line == END_OF_SECTION) break;

                                                string[] parts = line.Split('\t');
                                                if (parts.Length != 3) throw new Exception("Invalid Quarter metadata in " + filename);

                                                QuranMetadataQuarter info = new QuranMetadataQuarter();
                                                info.Number = int.Parse(parts[0]);
                                                info.StartChapter = int.Parse(parts[1]);
                                                info.StartChapterVerse = int.Parse(parts[2]);
                                                s_quran_metadata.Quarters.Add(info);
                                            }
                                        }
                                        break;
                                    case "bowing\tstart_chapter\tstart_chapter_verse":
                                        {
                                            while ((line = reader.ReadLine()) != null)
                                            {
                                                if (line == END_OF_SECTION) break;

                                                string[] parts = line.Split('\t');
                                                if (parts.Length != 3) throw new Exception("Invalid Bowing metadata in " + filename);

                                                QuranMetadataBowing info = new QuranMetadataBowing();
                                                info.Number = int.Parse(parts[0]);
                                                info.StartChapter = int.Parse(parts[1]);
                                                info.StartChapterVerse = int.Parse(parts[2]);
                                                s_quran_metadata.Bowings.Add(info);
                                            }
                                        }
                                        break;
                                    case "page\tstart_chapter\tstart_chapter_verse":
                                        {
                                            while ((line = reader.ReadLine()) != null)
                                            {
                                                if (line == END_OF_SECTION) break;

                                                string[] parts = line.Split('\t');
                                                if (parts.Length != 3) throw new Exception("Invalid Page metadata in " + filename);

                                                QuranMetadataPage info = new QuranMetadataPage();
                                                info.Number = int.Parse(parts[0]);
                                                info.StartChapter = int.Parse(parts[1]);
                                                info.StartChapterVerse = int.Parse(parts[2]);
                                                s_quran_metadata.Pages.Add(info);
                                            }
                                        }
                                        break;
                                    case "prostration\tchapter\tchapter_verse\ttype":
                                        {
                                            while ((line = reader.ReadLine()) != null)
                                            {
                                                if (line == END_OF_SECTION) break;

                                                string[] parts = line.Split('\t');
                                                if (parts.Length != 4) throw new Exception("Invalid Prostration metadata in " + filename);

                                                QuranMetadataProstration info = new QuranMetadataProstration();
                                                info.Number = int.Parse(parts[0]);
                                                info.Chapter = int.Parse(parts[1]);
                                                info.ChapterVerse = int.Parse(parts[2]);
                                                info.Type = (ProstrationType)Enum.Parse(typeof(ProstrationType), parts[3]);
                                                s_quran_metadata.Prostrations.Add(info);
                                            }
                                        }
                                        break;
                                    case "initialization\tchapter\tchapter_verse\ttype":
                                        {
                                            while ((line = reader.ReadLine()) != null)
                                            {
                                                if (line == END_OF_SECTION) break;

                                                string[] parts = line.Split('\t');
                                                if (parts.Length != 4) throw new Exception("Invalid Initialization metadata in " + filename);

                                                QuranMetadataInitialization info = new QuranMetadataInitialization();
                                                info.Number = int.Parse(parts[0]);
                                                info.Chapter = int.Parse(parts[1]);
                                                info.ChapterVerse = int.Parse(parts[2]);
                                                info.Type = (InitializationType)Enum.Parse(typeof(InitializationType), parts[3]);
                                                s_quran_metadata.Initializations.Add(info);
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            continue;
                                        }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("LoadQuranMetadata: " + ex.Message);
                    }
                }
            }
        }

        public List<Chapter> GetChapters(List<Verse> verses)
        {
            if (verses == null) return null;

            List<Chapter> result = new List<Chapter>();
            foreach (Verse verse in verses)
            {
                if (!result.Contains(verse.Chapter))
                {
                    result.Add(verse.Chapter);
                }
            }
            return result;
        }
        //public List<Chapter> GetCompleteChapters(List<Verse> verses)
        //{
        //    if (verses == null) return null;

        //    List<Chapter> result = new List<Chapter>();
        //    Chapter chapter = null;
        //    foreach (Verse verse in verses)
        //    {
        //        if (chapter != verse.Chapter)
        //        {
        //            chapter = verse.Chapter;
        //            if (!result.Contains(chapter))
        //            {
        //                bool include_chapter = true;
        //                foreach (Verse chapter_verse in chapter.Verses)
        //                {
        //                    if (!verses.Contains(chapter_verse))
        //                    {
        //                        include_chapter = false;
        //                        break;
        //                    }
        //                }

        //                if (include_chapter)
        //                {
        //                    result.Add(chapter);
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        public void SortChapters(ChapterSortMethod sort_method, ChapterSortOrder sort_order, bool pin_chapter1)
        {
            if (this.chapters != null)
            {
                Chapter.SortMethod = sort_method;
                Chapter.SortOrder = sort_order;
                Chapter.PinChapter1 = pin_chapter1;

                // sort chapters using Chapter.CompareTo()
                this.chapters.Sort();

                // update chapter numbers in new sort order
                for (int i = 0; i < this.chapters.Count; i++)
                {
                    this.chapters[i].SortedNumber = i + 1;
                }

                // update verse order in new sort order
                List<Verse> verses = new List<Verse>();
                foreach (Chapter chapter in this.chapters)
                {
                    verses.AddRange(chapter.Verses);
                }
                this.verses = verses;

                SetupBook();
            }
        }

        /// <summary>
        /// Get verses with ALL their words used inside the parameter "words".
        /// Verses with some words used in the parameter "words" are not returned.  
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public List<Verse> GetVerses(List<Word> words)
        {
            if (words == null) return null;

            List<Verse> result = new List<Verse>();
            Verse verse = null;
            foreach (Word word in words)
            {
                if (verse != word.Verse)
                {
                    verse = word.Verse;
                    if (!result.Contains(verse))
                    {
                        bool include_verse = true;
                        foreach (Word verse_word in verse.Words)
                        {
                            if (!words.Contains(verse_word))
                            {
                                include_verse = false;
                                break;
                            }
                        }

                        if (include_verse)
                        {
                            result.Add(verse);
                        }
                    }
                }
            }
            return result;
        }

        public string Text
        {
            get
            {
                StringBuilder str = new StringBuilder();
                if (this.verses != null)
                {
                    if (this.verses.Count > 0)
                    {
                        foreach (Verse verse in this.verses)
                        {
                            str.AppendLine(verse.Text);
                        }
                        if (str.Length > 2)
                        {
                            str.Remove(str.Length - 2, 2);
                        }
                    }
                }
                return str.ToString();
            }
        }
        public override string ToString()
        {
            return this.Text;
        }

        private int min_words;
        public int MinWords
        {
            get { return min_words; }
        }
        private int max_words;
        public int MaxWords
        {
            get { return max_words; }
        }
        private int min_letters;
        public int MinLetters
        {
            get { return min_letters; }
        }
        private int max_letters;
        public int MaxLetters
        {
            get { return max_letters; }
        }

        // root words
        private SortedDictionary<string, List<Word>> root_words = null;
        public SortedDictionary<string, List<Word>> RootWords
        {
            get { return root_words; }
            set { root_words = value; }
        }
        public void PopulateRootWords()
        {
            if (root_words == null)
            {
                root_words = new SortedDictionary<string, List<Word>>();
                if (root_words != null)
                {
                    List<string> roots = new List<string>();
                    foreach (Verse verse in this.Verses)
                    {
                        if (verse.Words != null)
                        {
                            foreach (Word word in verse.Words)
                            {
                                if (word.Roots != null)
                                {
                                    foreach (string root in word.Roots)
                                    {
                                        if (!root_words.ContainsKey(root))
                                        {
                                            root_words.Add(root, new List<Word>());
                                        }
                                        root_words[root].Add(word);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // get verse range
        public List<Verse> GetVerses(int start, int end)
        {
            List<Verse> result = new List<Verse>();
            if (
                (start >= end)
                &&
                (start >= 1 && start <= this.verses.Count)
                &&
                (end >= start && end <= this.verses.Count)
                )
            {
                if (this.verses != null)
                {
                    foreach (Verse verse in this.verses)
                    {
                        if ((verse.Number >= start) && (verse.Number <= end))
                        {
                            result.Add(verse);
                        }
                    }
                }
            }
            return result;
        }
        // get words
        public Dictionary<string, int> GetWordsWith(List<Verse> verses, string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            if (verses != null)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = text.Trim();
                    if (!text.Contains(" "))
                    {
                        if ((this.text_mode == "Original") && (!with_diacritics))
                        {
                            text = text.Simplify29();
                        }

                        foreach (Verse verse in verses)
                        {
                            string verse_text = verse.Text;
                            if ((this.text_mode == "Original") && (!with_diacritics))
                            {
                                verse_text = verse_text.Simplify29();
                            }

                            verse_text = verse_text.Trim();
                            while (verse_text.Contains("  "))
                            {
                                verse_text = verse_text.Replace("  ", " ");
                            }
                            string[] verse_words = verse_text.Split();

                            for (int i = 0; i < verse_words.Length; i++)
                            {
                                bool break_loop = false;
                                switch (text_location_in_verse)
                                {
                                    case TextLocationInVerse.Any:
                                        {
                                            // do nothing
                                        }
                                        break;
                                    case TextLocationInVerse.AtStart:
                                        {
                                            if (i > 0) break_loop = true;
                                        }
                                        break;
                                    case TextLocationInVerse.AtMiddle:
                                        {
                                            if (i == 0) continue;
                                            if (i == verse_words.Length - 1) continue;
                                        }
                                        break;
                                    case TextLocationInVerse.AtEnd:
                                        {
                                            if (i < verse_words.Length - 1) continue;
                                        }
                                        break;
                                }
                                if (break_loop) break;

                                switch (text_wordness)
                                {
                                    case TextWordness.WholeWord:
                                        {
                                            if (verse_words[i] == text)
                                            {
                                                if (!result.ContainsKey(verse_words[i]))
                                                {
                                                    result.Add(verse_words[i], 1);
                                                }
                                                else
                                                {
                                                    result[verse_words[i]]++;
                                                }
                                            }
                                        }
                                        break;
                                    case TextWordness.PartOfWord:
                                        {
                                            if ((verse_words[i] != text) && (verse_words[i].Contains(text)))
                                            {
                                                if (!result.ContainsKey(verse_words[i]))
                                                {
                                                    result.Add(verse_words[i], 1);
                                                }
                                                else
                                                {
                                                    result[verse_words[i]]++;
                                                }
                                            }
                                        }
                                        break;
                                    case TextWordness.Any:
                                        {
                                            switch (text_location_in_word)
                                            {
                                                case TextLocationInWord.AtStart:
                                                    {
                                                        if (verse_words[i].StartsWith(text))
                                                        {
                                                            if (!result.ContainsKey(verse_words[i]))
                                                            {
                                                                result.Add(verse_words[i], 1);
                                                            }
                                                            else
                                                            {
                                                                result[verse_words[i]]++;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case TextLocationInWord.AtMiddle:
                                                    {
                                                        if (verse_words[i].ContainsInside(text))
                                                        {
                                                            if (!result.ContainsKey(verse_words[i]))
                                                            {
                                                                result.Add(verse_words[i], 1);
                                                            }
                                                            else
                                                            {
                                                                result[verse_words[i]]++;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case TextLocationInWord.AtEnd:
                                                    {
                                                        if (verse_words[i].EndsWith(text))
                                                        {
                                                            if (!result.ContainsKey(verse_words[i]))
                                                            {
                                                                result.Add(verse_words[i], 1);
                                                            }
                                                            else
                                                            {
                                                                result[verse_words[i]]++;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case TextLocationInWord.Any:
                                                    {
                                                        if (verse_words[i].Contains(text))
                                                        {
                                                            if (!result.ContainsKey(verse_words[i]))
                                                            {
                                                                result.Add(verse_words[i], 1);
                                                            }
                                                            else
                                                            {
                                                                result[verse_words[i]]++;
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
        public Dictionary<string, int> GetCurrentWords(List<Verse> verses, string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word, TextWordness text_wordness)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            if (verses != null)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = text.Trim();
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

                    if ((this.text_mode == "Original") && (!with_diacritics))
                    {
                        text = text.Simplify29();
                    }

                    string[] text_words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (text_words.Length > 0)
                    {
                        foreach (Verse verse in verses)
                        {
                            string verse_text = verse.Text;
                            if ((this.text_mode == "Original") && (!with_diacritics))
                            {
                                verse_text = verse_text.Simplify29();
                            }

                            verse_text = verse_text.Trim();
                            while (verse_text.Contains("  "))
                            {
                                verse_text = verse_text.Replace("  ", " ");
                            }
                            string[] verse_words = verse_text.Split();

                            if (verse_words.Length >= text_words.Length)
                            {
                                for (int i = 0; i < verse_words.Length; i++)
                                {
                                    bool break_loop = false;
                                    switch (text_location_in_verse)
                                    {
                                        case TextLocationInVerse.Any:
                                            {
                                                // do nothing
                                            }
                                            break;
                                        case TextLocationInVerse.AtStart:
                                            {
                                                if (i > 0) break_loop = true;
                                            }
                                            break;
                                        case TextLocationInVerse.AtMiddle:
                                            {
                                                if (i == 0) continue;
                                                if (i == verse_words.Length - 1) continue;
                                            }
                                            break;
                                        case TextLocationInVerse.AtEnd:
                                            {
                                                if (i < verse_words.Length - 1) continue;
                                            }
                                            break;
                                    }
                                    if (break_loop) break;

                                    int match_count = 0;
                                    if (text_words.Length == 1) // 1 word search term
                                    {
                                        switch (text_wordness)
                                        {
                                            case TextWordness.WholeWord:
                                                {
                                                    if (verse_words[i] == text_words[0])
                                                    {
                                                        match_count = 1;
                                                    }
                                                }
                                                break;
                                            case TextWordness.PartOfWord:
                                                {
                                                    if ((verse_words[i] != text_words[0]) && (verse_words[i].Contains(text_words[0])))
                                                    {
                                                        switch (text_location_in_word)
                                                        {
                                                            case TextLocationInWord.AtStart:
                                                                {
                                                                    if (verse_words[i].StartsWith(text_words[0]))
                                                                    {
                                                                        match_count = 1;
                                                                    }
                                                                }
                                                                break;
                                                            case TextLocationInWord.AtMiddle:
                                                                {
                                                                    if (verse_words[i].ContainsInside(text_words[0]))
                                                                    {
                                                                        RegexOptions regex_options = RegexOptions.IgnoreCase | RegexOptions.RightToLeft;
                                                                        MatchCollection matches = Regex.Matches(verse_words[i], text_words[0], regex_options);
                                                                        match_count = matches.Count;

                                                                        if (match_count > 0)
                                                                        {
                                                                            if (verse_words[i].StartsWith(text_words[0]))
                                                                            {
                                                                                match_count--;
                                                                            }

                                                                            if (verse_words[i].EndsWith(text_words[0]))
                                                                            {
                                                                                match_count--;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            case TextLocationInWord.AtEnd:
                                                                {
                                                                    if (verse_words[i].EndsWith(text_words[0]))
                                                                    {
                                                                        match_count = 1;
                                                                    }
                                                                }
                                                                break;
                                                            case TextLocationInWord.Any:
                                                                {
                                                                    if (verse_words[i].Contains(text_words[0]))
                                                                    {
                                                                        RegexOptions regex_options = RegexOptions.IgnoreCase | RegexOptions.RightToLeft;
                                                                        MatchCollection matches = Regex.Matches(verse_words[i], text_words[0], regex_options);
                                                                        match_count = matches.Count;
                                                                    }
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                break;
                                            case TextWordness.Any:
                                                {
                                                    switch (text_location_in_word)
                                                    {
                                                        case TextLocationInWord.AtStart:
                                                            {
                                                                if (verse_words[i].StartsWith(text_words[0]))
                                                                {
                                                                    match_count = 1;
                                                                }
                                                            }
                                                            break;
                                                        case TextLocationInWord.AtMiddle:
                                                            {
                                                                if (verse_words[i].ContainsInside(text_words[0]))
                                                                {
                                                                    RegexOptions regex_options = RegexOptions.IgnoreCase | RegexOptions.RightToLeft;
                                                                    MatchCollection matches = Regex.Matches(verse_words[i], text_words[0], regex_options);
                                                                    match_count = matches.Count;

                                                                    if (match_count > 0)
                                                                    {
                                                                        if (verse_words[i].StartsWith(text_words[0]))
                                                                        {
                                                                            match_count--;
                                                                        }

                                                                        if (verse_words[i].EndsWith(text_words[0]))
                                                                        {
                                                                            match_count--;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            break;
                                                        case TextLocationInWord.AtEnd:
                                                            {
                                                                if (verse_words[i].EndsWith(text_words[0]))
                                                                {
                                                                    match_count = 1;
                                                                }
                                                            }
                                                            break;
                                                        case TextLocationInWord.Any:
                                                            {
                                                                if (verse_words[i].Contains(text_words[0]))
                                                                {
                                                                    RegexOptions regex_options = RegexOptions.IgnoreCase | RegexOptions.RightToLeft;
                                                                    MatchCollection matches = Regex.Matches(verse_words[i], text_words[0], regex_options);
                                                                    match_count = matches.Count;
                                                                }
                                                            }
                                                            break;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    else if (text_words.Length > 1) // multiple words search term
                                    {

                                        switch (text_location_in_word)
                                        {
                                            case TextLocationInWord.AtStart:
                                                {
                                                    if (verse_words[i].StartsWith(text_words[0]))
                                                    {
                                                        if (verse_text.Contains(text))
                                                        {
                                                            match_count = 1;
                                                        }
                                                    }
                                                }
                                                break;
                                            case TextLocationInWord.AtMiddle:
                                                {
                                                    if (verse_words[i].ContainsInside(text_words[0]))
                                                    {
                                                        if (verse_text.Contains(text))
                                                        {
                                                            RegexOptions regex_options = RegexOptions.IgnoreCase | RegexOptions.RightToLeft;
                                                            MatchCollection matches = Regex.Matches(verse_words[i], text_words[0], regex_options);
                                                            match_count = matches.Count;

                                                            if (match_count > 0)
                                                            {
                                                                if (verse_words[i].StartsWith(text_words[0]))
                                                                {
                                                                    match_count--;
                                                                }

                                                                if (verse_words[i].EndsWith(text_words[0]))
                                                                {
                                                                    match_count--;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            case TextLocationInWord.AtEnd:
                                                {
                                                    if (verse_words[i].EndsWith(text_words[0]))
                                                    {
                                                        if (verse_text.Contains(text))
                                                        {
                                                            match_count = 1;
                                                        }
                                                    }
                                                }
                                                break;
                                            case TextLocationInWord.Any:
                                                {
                                                    if (verse_words[i].EndsWith(text_words[0]))
                                                    {
                                                        if (verse_words.Length >= (i + text_words.Length))
                                                        {
                                                            // match text minus last word
                                                            bool match_found_minus_last_word = true;
                                                            for (int j = 1; j < text_words.Length - 1; j++)
                                                            {
                                                                if (verse_words[j + i] != text_words[j])
                                                                {
                                                                    match_found_minus_last_word = false;
                                                                    break;
                                                                }
                                                            }

                                                            // is still true, check the last word
                                                            if (match_found_minus_last_word)
                                                            {
                                                                int last_j = text_words.Length - 1;
                                                                if (verse_words[last_j + i].StartsWith(text_words[last_j])) // last text_word
                                                                {
                                                                    match_count = 1;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }

                                    if (match_count > 0)
                                    {
                                        // skip all text but not found good_word in case it followed by good_word too
                                        i += text_words.Length - 1;

                                        // get last word variation
                                        if (i < verse_words.Length)
                                        {
                                            string matching_word = verse_words[i];
                                            if (!result.ContainsKey(matching_word))
                                            {
                                                result.Add(matching_word, match_count);
                                            }
                                            else
                                            {
                                                result[matching_word] += match_count;
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
        public Dictionary<string, int> GetNextWords(List<Verse> verses, string text, TextLocationInVerse text_location_in_verse, TextLocationInWord text_location_in_word)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            if (verses != null)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    text = text.Trim();
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

                    if ((this.text_mode == "Original") && (!with_diacritics))
                    {
                        text = text.Simplify29();
                    }

                    string[] text_words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (text_words.Length > 0)
                    {
                        foreach (Verse verse in verses)
                        {
                            string verse_text = verse.Text;
                            if ((this.text_mode == "Original") && (!with_diacritics))
                            {
                                verse_text = verse_text.Simplify29();
                            }

                            verse_text = verse_text.Trim();
                            while (verse_text.Contains("  "))
                            {
                                verse_text = verse_text.Replace("  ", " ");
                            }
                            string[] verse_words = verse_text.Split();

                            if (verse_words.Length >= text_words.Length)
                            {
                                for (int i = 0; i < verse_words.Length; i++)
                                {
                                    bool break_loop = false;
                                    switch (text_location_in_verse)
                                    {
                                        case TextLocationInVerse.Any:
                                            {
                                                // do nothing
                                            }
                                            break;
                                        case TextLocationInVerse.AtStart:
                                            {
                                                if (i > 0) break_loop = true;
                                            }
                                            break;
                                        case TextLocationInVerse.AtMiddle:
                                            {
                                                if (i == 0) continue;
                                                if (i == verse_words.Length - 1) continue;
                                            }
                                            break;
                                        case TextLocationInVerse.AtEnd:
                                            {
                                                if (i < verse_words.Length - 1) continue;
                                            }
                                            break;
                                    }
                                    if (break_loop) break;

                                    bool start_found = false;
                                    switch (text_location_in_word)
                                    {
                                        case TextLocationInWord.AtStart:
                                            {
                                                start_found = verse_words[i].Equals(text_words[0]);
                                            }
                                            break;
                                        case TextLocationInWord.AtMiddle:
                                        case TextLocationInWord.AtEnd:
                                        case TextLocationInWord.Any:
                                            {
                                                start_found = verse_words[i].EndsWith(text_words[0]);
                                            }
                                            break;
                                    }

                                    if (start_found)
                                    {
                                        if (verse_words.Length >= (i + text_words.Length))
                                        {
                                            // check rest of text_words if matching
                                            bool match_found = true;
                                            for (int j = 1; j < text_words.Length; j++)
                                            {
                                                if (verse_words[j + i] != text_words[j])
                                                {
                                                    match_found = false;
                                                    break;
                                                }
                                            }

                                            if (match_found)
                                            {
                                                i += text_words.Length;

                                                // add next word to result (if not added already)
                                                if (i < verse_words.Length)
                                                {
                                                    string matching_word = verse_words[i];
                                                    if (!result.ContainsKey(matching_word))
                                                    {
                                                        result.Add(matching_word, 1);
                                                    }
                                                    else
                                                    {
                                                        result[matching_word]++;
                                                    }
                                                }

                                                i--; // check following word in case it contains a match too
                                                // Example: search for " ير" to not miss يطير in طاير يطير
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
        // get roots
        public List<string> GetRoots()
        {
            List<string> result = new List<string>();
            foreach (string key in this.RootWords.Keys)
            {
                result.Add(key);
            }
            return result;
        }
        public Dictionary<string, int> GetWordRoots(List<Verse> verses, string text)
        {
            return GetWordRoots(verses, text, TextLocationInWord.Any);
        }
        public Dictionary<string, int> GetWordRoots(List<Verse> verses, string text, TextLocationInWord text_location_in_word)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            if (verses != null)
            {
                if (text != null)
                {
                    SortedDictionary<string, List<Word>> root_words_dictionary = this.RootWords;
                    if (root_words_dictionary != null)
                    {
                        if (root_words_dictionary.Keys != null)
                        {
                            foreach (string key in root_words_dictionary.Keys)
                            {
                                if (root_words_dictionary.ContainsKey(key))
                                {
                                    int count = 0;
                                    List<Word> root_words = root_words_dictionary[key];
                                    if (verses.Count == this.Verses.Count)
                                    {
                                        count = root_words.Count;
                                    }
                                    else
                                    {
                                        foreach (Word root_word in root_words)
                                        {
                                            if (verses.Contains(root_word.Verse))
                                            {
                                                count++;
                                            }
                                        }
                                    }

                                    foreach (Word root_word in root_words)
                                    {
                                        if (verses.Contains(root_word.Verse))
                                        {
                                            switch (text_location_in_word)
                                            {
                                                case TextLocationInWord.AtStart:
                                                    {
                                                        if (text.Length == 0)
                                                        {
                                                            result.Add(key, count);
                                                        }
                                                        else
                                                        {
                                                            if ((this.text_mode == "Original") && (!with_diacritics))
                                                            {
                                                                if (key.StartsWith(text.Simplify36()))
                                                                {
                                                                    result.Add(key, count);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (key.StartsWith(text))
                                                                {
                                                                    result.Add(key, count);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case TextLocationInWord.AtMiddle:
                                                    {
                                                        if (text.Length == 0)
                                                        {
                                                            result.Add(key, count);
                                                        }
                                                        else
                                                        {
                                                            if ((this.text_mode == "Original") && (!with_diacritics))
                                                            {
                                                                if (key.ContainsInside(text.Simplify36()))
                                                                {
                                                                    result.Add(key, count);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (key.ContainsInside(text))
                                                                {
                                                                    result.Add(key, count);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case TextLocationInWord.AtEnd:
                                                    {
                                                        if (text.Length == 0)
                                                        {
                                                            result.Add(key, count);
                                                        }
                                                        else
                                                        {
                                                            if ((this.text_mode == "Original") && (!with_diacritics))
                                                            {
                                                                if (key.EndsWith(text.Simplify36()))
                                                                {
                                                                    result.Add(key, count);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (key.EndsWith(text))
                                                                {
                                                                    result.Add(key, count);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case TextLocationInWord.Any:
                                                    {
                                                        if (text.Length == 0)
                                                        {
                                                            result.Add(key, count);
                                                        }
                                                        else
                                                        {
                                                            if ((this.text_mode == "Original") && (!with_diacritics))
                                                            {
                                                                if (key.Contains(text.Simplify36()))
                                                                {
                                                                    result.Add(key, count);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (key.Contains(text))
                                                                {
                                                                    result.Add(key, count);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                            break;
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
        public string GetBestRoot(string user_text)
        {
            if (!String.IsNullOrEmpty(user_text))
            {
                string simplified_user_text = user_text.Simplify29();

                // try all roots in case user_text is an existing root
                SortedDictionary<string, List<Word>> root_words_dictionary = this.RootWords;
                if (root_words_dictionary != null)
                {
                    foreach (string key in root_words_dictionary.Keys)
                    {
                        if (
                                (key.Length >= 3)
                                ||
                                (key.Length == simplified_user_text.Length - 1)
                                ||
                                (key.Length == simplified_user_text.Length)
                                ||
                                (key.Length == simplified_user_text.Length + 1)
                           )
                        {
                            if (root_words_dictionary.ContainsKey(key))
                            {
                                List<Word> root_words = root_words_dictionary[key];
                                foreach (Word root_word in root_words)
                                {
                                    int verse_index = root_word.Verse.Number - 1;
                                    if ((verse_index >= 0) && (verse_index < this.verses.Count))
                                    {
                                        Verse verse = this.verses[verse_index];
                                        int word_index = root_word.NumberInVerse - 1;
                                        if ((word_index >= 0) && (word_index < verse.Words.Count))
                                        {
                                            Word verse_word = verse.Words[word_index];
                                            // user_text is an existing root
                                            if (verse_word.Text.Simplify29() == simplified_user_text)
                                            {
                                                return key;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // get most similar root to user_text
                string best_root = null;
                double max_similirity = double.MinValue;
                Dictionary<string, int> roots = GetWordRoots(this.Verses, user_text);
                foreach (string root in roots.Keys)
                {
                    double similirity = root.SimilarityTo(simplified_user_text);
                    if (similirity >= max_similirity)
                    {
                        max_similirity = similirity;
                        best_root = root;
                    }
                }
                return best_root;
            }
            return null;
        }
        // get related words and verses
        public List<Word> GetRelatedWords(Word word)
        {
            List<Word> result = new List<Word>();
            if (word != null)
            {
                string simplified_word_text = word.Text.Simplify31();
                SortedDictionary<string, List<Word>> root_words_dictionary = this.RootWords;
                if (root_words_dictionary != null)
                {
                    // try all roots in case word_text is a root
                    if (root_words_dictionary.ContainsKey(simplified_word_text))
                    {
                        List<Word> root_words = root_words_dictionary[simplified_word_text];
                        foreach (Word root_word in root_words)
                        {
                            int verse_index = root_word.Verse.Number - 1;
                            if ((verse_index >= 0) && (verse_index < this.verses.Count))
                            {
                                Verse verse = this.verses[verse_index];
                                int word_index = root_word.NumberInVerse - 1;
                                if ((word_index >= 0) && (word_index < verse.Words.Count))
                                {
                                    Word verse_word = verse.Words[word_index];
                                    if (!result.Contains(verse_word))
                                    {
                                        result.Add(verse_word);
                                    }
                                }
                            }
                        }
                    }
                    else // if no such root, search for the matching root_word by its verse position and get its root and then get all root_words
                    {
                        string root = word.BestRoot;
                        if (!String.IsNullOrEmpty(root))
                        {
                            if (root_words_dictionary.ContainsKey(root))
                            {
                                List<Word> root_words = root_words_dictionary[root];
                                foreach (Word root_word in root_words)
                                {
                                    int verse_index = root_word.Verse.Number - 1;
                                    if ((verse_index >= 0) && (verse_index < this.verses.Count))
                                    {
                                        Verse verse = this.verses[verse_index];
                                        int word_index = root_word.NumberInVerse - 1;
                                        if ((word_index >= 0) && (word_index < verse.Words.Count))
                                        {
                                            Word verse_word = verse.Words[word_index];
                                            if (!result.Contains(verse_word))
                                            {
                                                result.Add(verse_word);
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
        public List<Verse> GetRelatedWordVerses(Word word)
        {
            List<Verse> result = new List<Verse>();
            if (word != null)
            {
                SortedDictionary<string, List<Word>> root_words_dictionary = this.RootWords;
                if (root_words_dictionary != null)
                {
                    // try all roots in case word_text is a root
                    if (root_words_dictionary.ContainsKey(word.Text))
                    {
                        List<Word> root_words = root_words_dictionary[word.Text];
                        foreach (Word root_word in root_words)
                        {
                            int verse_index = root_word.Verse.Number - 1;
                            if ((verse_index >= 0) && (verse_index < this.verses.Count))
                            {
                                Verse verse = this.verses[verse_index];
                                if (!result.Contains(verse))
                                {
                                    result.Add(verse);
                                }
                            }
                        }
                    }
                    else // if no such root, search for the matching root_word by its verse position and get its root and then get all root_words
                    {
                        string root = word.BestRoot;
                        if (!String.IsNullOrEmpty(root))
                        {
                            if (root_words_dictionary.ContainsKey(root))
                            {
                                List<Word> root_words = root_words_dictionary[root];
                                foreach (Word root_word in root_words)
                                {
                                    int verse_index = root_word.Verse.Number - 1;
                                    if ((verse_index >= 0) && (verse_index < this.verses.Count))
                                    {
                                        Verse verse = this.verses[verse_index];
                                        if (!result.Contains(verse))
                                        {
                                            result.Add(verse);
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
}
