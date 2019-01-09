using System;

public class Chapter
{
    public readonly int Number;
    public readonly int VerseCount;
    public readonly int WordCount;
    public readonly int LetterCount;

    public Chapter(int number, int verse_count, int word_count, int letter_count)
    {
        this.Number = number;
        this.VerseCount = verse_count;
        this.WordCount = word_count;
        this.LetterCount = letter_count;
    }
}
