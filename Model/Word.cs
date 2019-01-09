using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Word
    {
        private Verse verse = null;
        public Verse Verse
        {
            get { return verse; }
        }

        private int number = 0;
        public int Number
        {
            set { number = value; }
            get { return number; }
        }

        private int number_in_verse = 0;
        public int NumberInVerse
        {
            set { number_in_verse = value; }
            get { return number_in_verse; }
        }

        private int number_in_chapter = 0;
        public int NumberInChapter
        {
            set { number_in_chapter = value; }
            get { return number_in_chapter; }
        }

        public Distance DistanceToPrevious = new Distance();

        public string Address
        {
            get
            {
                if (verse != null)
                {
                    if (verse.Chapter != null)
                    {
                        return (this.verse.Chapter.SortedNumber.ToString() + ":" + verse.NumberInChapter.ToString() + ":" + number_in_verse.ToString());
                    }
                }
                return "XXX:XXX:XXX";
            }
        }

        private string transliteration = null;
        public string Transliteration
        {
            get
            {
                if (transliteration == null)
                {
                    if (this.text == "و")
                    {
                        transliteration = "wa";
                    }
                    else
                    {
                        if (this.verse != null)
                        {
                            if (this.verse.Translations.ContainsKey("en.transliteration"))
                            {
                                string verse_transliteration = this.verse.Translations["en.transliteration"];
                                string[] parts = verse_transliteration.Split();

                                int index = this.number_in_verse - 1;

                                if (!this.verse.Book.WithBismAllah)
                                {
                                    if (this.verse.NumberInChapter == 1)
                                    {
                                        if ((this.verse.Chapter.Number != 1) && (this.verse.Chapter.Number != 9))
                                        {
                                            index += 4;
                                        }
                                    }
                                }

                                for (int i = 0; i < this.number_in_verse; i++)
                                {
                                    if ((i >= 0) && (i < this.verse.Words.Count))
                                    {
                                        if (this.verse.Words[i].Text == "و")
                                        {
                                            index--;
                                        }
                                    }
                                }

                                if ((index >= 0) && (index < parts.Length))
                                {
                                    // remove wa from words following wa
                                    int w = this.number_in_verse - 1;
                                    if (((w - 1) >= 0) && ((w - 1) < this.verse.Words.Count))
                                    {
                                        if (this.verse.Words[w - 1].Text == "و")
                                        {
                                            parts[index] = parts[index].Substring(2);
                                        }
                                    }

                                    transliteration = parts[index];
                                }
                            }
                        }
                    }
                }
                if (transliteration != null)
                {
                    return transliteration.Replace("\"", "").Replace("\'", "");
                }
                return null;
            }
        }

        private string meaning = null;
        public string Meaning
        {
            set { meaning = value; }
            get
            {
                if (meaning != null)
                {
                    return meaning.Replace("\"", "").Replace("\'", "");
                }
                return null;
            }
        }

        private List<string> roots = null;
        public List<string> Roots
        {
            set { roots = value; }
            get { return roots; }
        }

        private List<WordPart> parts = null;
        public List<WordPart> Parts
        {
            get { return parts; }
        }

        private string best_root = null;
        public string BestRoot
        {
            get
            {
                if (String.IsNullOrEmpty(best_root))
                {
                    best_root = "";
                    if (this.Roots != null)
                    {
                        if (this.Roots.Count > 0)
                        {
                            foreach (string root in this.Roots)
                            {
                                if (best_root.Length < root.Length)
                                {
                                    best_root = root;
                                }
                            }
                        }
                    }
                }
                return best_root;
            }
        }

        private string root = null;
        public string Root
        {
            get
            {
                if (String.IsNullOrEmpty(root))
                {
                    if (parts != null)
                    {
                        foreach (WordPart part in parts)
                        {
                            if (!String.IsNullOrEmpty(part.Grammar.Root))
                            {
                                root = part.Grammar.Root.ToArabic();
                                break;
                            }
                        }
                    }
                }
                return root;
            }
        }

        private string lemma = null;
        public string Lemma
        {
            get
            {
                if (String.IsNullOrEmpty(lemma))
                {
                    if (parts != null)
                    {
                        foreach (WordPart part in parts)
                        {
                            if (!String.IsNullOrEmpty(part.Grammar.Lemma))
                            {
                                lemma += part.Grammar.Lemma.ToArabic();
                                break;
                            }
                        }
                    }
                }
                return lemma;
            }
        }

        private string special_group = null;
        public string SpecialGroup
        {
            get
            {
                if (String.IsNullOrEmpty(special_group))
                {
                    if (parts != null)
                    {
                        foreach (WordPart part in parts)
                        {
                            if (!String.IsNullOrEmpty(part.Grammar.SpecialGroup))
                            {
                                special_group += part.Grammar.SpecialGroup.ToArabic();
                                break;
                            }
                        }
                    }
                }
                return special_group;
            }
        }

        private string arabic_grammar = null;
        public string ArabicGrammar
        {
            get
            {
                if (String.IsNullOrEmpty(arabic_grammar))
                {
                    StringBuilder result = new StringBuilder();

                    StringBuilder str = new StringBuilder();
                    string previous_word_address = null;
                    if (this.Parts != null) // null in Grammar Edition
                    {
                        for (int i = 0; i < this.Parts.Count; i++)
                        {
                            if (previous_word_address == this.Parts[i].Word.Address)
                            {
                                // continue with current word
                                str.AppendLine(this.Parts[i].ToArabic());
                            }
                            else // new word
                            {
                                previous_word_address = this.Parts[i].Word.Address;

                                // finish up previous word
                                if (str.Length > 2)
                                {
                                    str.Remove(str.Length - 2, 2);
                                    result.Append(str.ToString());
                                    // clear str for new word
                                    str.Length = 0;
                                }

                                // continue with current word
                                str.AppendLine(this.Parts[i].ToArabic());
                            }

                            if (i == this.Parts.Count - 1)
                            {
                                // finish up last word
                                if (str.Length > 2)
                                {
                                    str.Remove(str.Length - 2, 2);
                                    result.Append(str.ToString());
                                }
                            }
                        }
                    }
                    arabic_grammar = result.ToString();
                }

                return arabic_grammar;
            }
        }

        private string english_grammar = null;
        public string EnglishGrammar
        {
            get
            {
                if (String.IsNullOrEmpty(english_grammar))
                {
                    StringBuilder result = new StringBuilder();

                    StringBuilder str = new StringBuilder();
                    string previous_word_address = null;
                    if (this.Parts != null) // null in Grammar Edition
                    {
                        for (int i = 0; i < this.Parts.Count; i++)
                        {
                            if (previous_word_address == this.Parts[i].Word.Address)
                            {
                                // continue with current word
                                str.AppendLine(this.Parts[i].ToEnglish());
                            }
                            else // new word
                            {
                                previous_word_address = this.Parts[i].Word.Address;

                                // finish up previous word
                                if (str.Length > 2)
                                {
                                    str.Remove(str.Length - 2, 2);
                                    result.Append(str.ToString());
                                    // clear str for new word
                                    str.Length = 0;
                                }

                                // continue with current word
                                str.AppendLine(this.Parts[i].ToEnglish());
                            }

                            if (i == this.Parts.Count - 1)
                            {
                                // finish up last word
                                if (str.Length > 2)
                                {
                                    str.Remove(str.Length - 2, 2);
                                    result.Append(str.ToString());
                                }
                            }
                        }
                    }
                    english_grammar = result.ToString();
                }

                return english_grammar;
            }
        }

        private int occurrence = 0;
        public int Occurrence
        {
            get { return occurrence; }
            internal set { occurrence = value; }
        }

        private int frequency = 0;
        public int Frequency
        {
            get { return frequency; }
            internal set { frequency = value; }
        }

        private List<Letter> letters = null;
        public List<Letter> Letters
        {
            get { return letters; }
        }

        private List<char> unique_letters = null;
        public List<char> UniqueLetters
        {
            get
            {
                if (unique_letters == null)
                {
                    unique_letters = new List<char>();
                    if (this.letters != null)
                    {
                        foreach (Letter letter in this.letters)
                        {
                            if (!unique_letters.Contains(letter.Character))
                            {
                                unique_letters.Add(letter.Character);
                            }
                        }
                    }
                }
                return unique_letters;
            }
        }

        private int position = -1;
        public int Position
        {
            get { return position; }
        }

        private string text = null;
        public string Text
        {
            get { return text; }
        }
        public override string ToString()
        {
            return this.Text;
        }

        private Stopmark stopmark = Stopmark.None;
        public Stopmark Stopmark
        {
            get { return stopmark; }
            set { stopmark = value; }
        }

        public Word(Verse verse, int number_in_verse, int position, string text)
        {
            this.verse = verse;
            //this.number = number;                         // to be filled by book.SetupNumbers
            this.number_in_verse = number_in_verse;
            //this.number_in_chapter = number_in_chapter;   // to be filled by book.SetupNumbers
            this.position = position;
            this.text = text;

            this.parts = new List<WordPart>();

            this.letters = new List<Letter>();
            string simplified_text = null;
            if (text != null)
            {
                if (text.IsArabicWithDiacritics())
                {
                    simplified_text = text.SimplifyTo("Original");
                }
                else
                {
                    simplified_text = text;
                }
            }

            if (this.letters != null)
            {
                int letter_number_in_word = 0;
                foreach (char character in simplified_text)
                {
                    letter_number_in_word++;

                    Letter letter = new Letter(this, letter_number_in_word, character);
                    this.letters.Add(letter);
                }
            }
        }
    }
}
