using System;
using System.Collections.Generic;

namespace Model
{
    public class Letter
    {
        private Word word = null;
        public Word Word
        {
            get { return word; }
        }

        private int number = 0;
        public int Number
        {
            set { number = value; }
            get { return number; }
        }

        private int number_in_word;
        public int NumberInWord
        {
            set { number_in_word = value; }
            get { return number_in_word; }
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

        public string Address
        {
            get
            {
                if (word != null)
                {
                    return (this.word.Address + ":" + number_in_word.ToString());
                }
                return "XXX:XXX:XXX:XXX";
            }
        }

        public Distance DistanceToPrevious = new Distance();
        public Distance DistanceToNext = new Distance();

        private char character;
        public char Character
        {
            get { return character; }
        }
        public override string ToString()
        {
            return this.Character.ToString();
        }

        public Letter(Word word, int number_in_word, char character)
        {
            this.word = word;
            //this.number = number; // to be filled by book.SetupNumbers
            this.number_in_word = number_in_word;
            //this.number_in_verse = number_in_verse; // to be filled by book.SetupNumbers
            //this.number_in_chapter = number_in_chapter; // to be filled by book.SetupNumbers
            this.character = character;
        }
    }
}
