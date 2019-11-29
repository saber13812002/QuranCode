#region QuranCode Object Model
// Book
// Book.Verses
// Book.Chapters.Verses
// Book.Stations.Verses
// Book.Parts.Verses
// Book.Groups.Verses
// Book.Halfs.Verses
// Book.Quarters.Verses
// Book.Bowings.Verses
// Book.Pages.Verses
// Verse.Words
// Word.WordParts
// Word.Letters
// Client.Bookmarks
// Client.Selection         // readonly, current selection (chapter, station, part, ... , verse, word, letter)
// Client.LetterStatistics  // readonly, statistics for current selection or highlighted text
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Model;

public static class Research
{
    private static List<Verse> GetSourceVerses(Client client, bool in_search_result)
    {
        List<Verse> verses = null;
        if (client != null)
        {
            switch (client.SearchScope)
            {
                case SearchScope.Book:
                    {
                        if (client.Book != null)
                        {
                            verses = client.Book.Verses;
                        }
                    }
                    break;
                case SearchScope.Selection:
                    {
                        if (client.Selection != null)
                        {
                            verses = client.Selection.Verses;
                        }
                    }
                    break;
                case SearchScope.Result:
                    {
                        verses = in_search_result ? client.FoundVerses : client.Selection.Verses;
                    }
                    break;
            }
        }
        return verses;
    }

    private static string _____________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    private static string Half1EvenVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoHalf1EvenVerses(client, verses);
        }
        return null;
    }
    private static string Half2EvenVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoHalf2EvenVerses(client, verses);
        }
        return null;
    }
    private static string Half1OddVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoHalf1OddVerses(client, verses);
        }
        return null;
    }
    private static string Half2OddVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoHalf2OddVerses(client, verses);
        }
        return null;
    }
    private static string DoHalf1EvenVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        int chapter_count = 0;
        int verse_count = 0;
        int verse_sequence_sum = 0;
        foreach (Chapter chapter in chapters)
        {
            if (chapter.SortedNumber <= (chapters.Count / 2))
            {
                if (Numbers.IsEven(chapter.Verses.Count))
                {
                    chapter_count++;
                    foreach (Verse verse in chapter.Verses)
                    {
                        verse_count++;
                        verse_sequence_sum += verse.Number;
                    }
                }
            }
        }

        return (chapter_count + " chapters in 1st half of the Quran with even verses.\r\n" + verse_count + " verses with sum of Book-level verses numbers = " + verse_sequence_sum + ".");
    }
    private static string DoHalf2EvenVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        int chapter_count = 0;
        int verse_count = 0;
        int verse_sequence_sum = 0;
        foreach (Chapter chapter in chapters)
        {
            if (chapter.SortedNumber > (chapters.Count / 2))
            {
                if (Numbers.IsEven(chapter.Verses.Count))
                {
                    chapter_count++;
                    foreach (Verse verse in chapter.Verses)
                    {
                        verse_count++;
                        verse_sequence_sum += verse.Number;
                    }
                }
            }
        }

        return (chapter_count + " chapters in 1st half of the Quran with even verses.\r\n" + verse_count + " verses with sum of Book-level verses numbers = " + verse_sequence_sum + ".");
    }
    private static string DoHalf1OddVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        int chapter_count = 0;
        int verse_count = 0;
        int verse_sequence_sum = 0;
        foreach (Chapter chapter in chapters)
        {
            if (chapter.SortedNumber <= (chapters.Count / 2))
            {
                if (Numbers.IsOdd(chapter.Verses.Count))
                {
                    chapter_count++;
                    foreach (Verse verse in chapter.Verses)
                    {
                        verse_count++;
                        verse_sequence_sum += verse.Number;
                    }
                }
            }
        }

        return (chapter_count + " chapters in 1st half of the Quran with even verses.\r\n" + verse_count + " verses with sum of Book-level verses numbers = " + verse_sequence_sum + ".");
    }
    private static string DoHalf2OddVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        int chapter_count = 0;
        int verse_count = 0;
        int verse_sequence_sum = 0;
        foreach (Chapter chapter in chapters)
        {
            if (chapter.SortedNumber > (chapters.Count / 2))
            {
                if (Numbers.IsOdd(chapter.Verses.Count))
                {
                    chapter_count++;
                    foreach (Verse verse in chapter.Verses)
                    {
                        verse_count++;
                        verse_sequence_sum += verse.Number;
                    }
                }
            }
        }

        return (chapter_count + " chapters in 1st half of the Quran with even verses.\r\n" + verse_count + " verses with sum of Book-level verses numbers = " + verse_sequence_sum + ".");
    }

    private static string ______________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    private static string AllDigits(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoAllDigits(client, verses);
        }
        return null;
    }
    private static string OddDigitChapters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoOddDigitChapters(client, verses);
        }
        return null;
    }
    private static string EvenDigitChapters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoEvenDigitChapters(client, verses);
        }
        return null;
    }
    private static string PrimeDigitChapters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoPrimeDigitChapters(client, verses);
        }
        return null;
    }
    private static string CompositeDigitChapters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoCompositeDigitChapters(client, verses);
        }
        return null;
    }
    private static string PrimeOr1DigitChapters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoPrimeOr1DigitChapters(client, verses);
        }
        return null;
    }
    private static string CompositeOr0DigitChapters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoCompositeOr0DigitChapters(client, verses);
        }
        return null;
    }
    private static string OddDigitVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoOddDigitVerses(client, verses);
        }
        return null;
    }
    private static string EvenDigitVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoEvenDigitVerses(client, verses);
        }
        return null;
    }
    private static string PrimeDigitVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoPrimeDigitVerses(client, verses);
        }
        return null;
    }
    private static string CompositeDigitVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoCompositeDigitVerses(client, verses);
        }
        return null;
    }
    private static string PrimeOr1DigitVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoPrimeOr1DigitVerses(client, verses);
        }
        return null;
    }
    private static string CompositeOr0DigitVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoCompositeOr0DigitVerses(client, verses);
        }
        return null;
    }
    private static string OODigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoOODigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string EEDigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoEEDigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string PPDigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoPPDigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string CCDigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoCCDigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string P1P1DigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoP1P1DigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string C0C0DigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoC0C0DigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string OEDigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoOEDigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string EODigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoEODigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string PCDigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoPCDigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string CPDigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoCPDigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string P1C0DigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoP1C0DigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string C0P1DigitChaptersVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoC0P1DigitChaptersVerses(client, verses);
        }
        return null;
    }
    private static string DoAllDigits(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        StringBuilder str = new StringBuilder();
        List<Chapter> matching_chapters = new List<Chapter>();
        str.AppendLine("OddDigitChapters");
        str.AppendLine(DoOddDigitChapters(client, verses));
        str.AppendLine("EvenDigitChapters");
        str.AppendLine(DoEvenDigitChapters(client, verses));
        str.AppendLine("PrimeDigitChapters");
        str.AppendLine(DoPrimeDigitChapters(client, verses));
        str.AppendLine("CompositeDigitChapters");
        str.AppendLine(DoCompositeDigitChapters(client, verses));
        str.AppendLine("PrimeOr1DigitChapters");
        str.AppendLine(DoPrimeOr1DigitChapters(client, verses));
        str.AppendLine("CompositeOr0DigitChapters");
        str.AppendLine(DoCompositeOr0DigitChapters(client, verses));
        str.AppendLine("OddDigitVerses");
        str.AppendLine(DoOddDigitVerses(client, verses));
        str.AppendLine("EvenDigitVerses");
        str.AppendLine(DoEvenDigitVerses(client, verses));
        str.AppendLine("PrimeDigitVerses");
        str.AppendLine(DoPrimeDigitVerses(client, verses));
        str.AppendLine("CompositeDigitVerses");
        str.AppendLine(DoCompositeDigitVerses(client, verses));
        str.AppendLine("PrimeOr1DigitVerses");
        str.AppendLine(DoPrimeOr1DigitVerses(client, verses));
        str.AppendLine("CompositeOr0DigitVerses");
        str.AppendLine(DoCompositeOr0DigitVerses(client, verses));
        str.AppendLine("OddDigitChapters OddDigitVerses");
        str.AppendLine(DoOODigitChaptersVerses(client, verses));
        str.AppendLine("EvenDigitChapters EvenDigitVerses");
        str.AppendLine(DoEEDigitChaptersVerses(client, verses));
        str.AppendLine("PrimeDigitChapters PrimeDigitVerses");
        str.AppendLine(DoPPDigitChaptersVerses(client, verses));
        str.AppendLine("CompositeDigitChapters CompositeDigitVerses");
        str.AppendLine(DoCCDigitChaptersVerses(client, verses));
        str.AppendLine("PrimeOr1DigitChapters PrimeOr1DigitVerses");
        str.AppendLine(DoP1P1DigitChaptersVerses(client, verses));
        str.AppendLine("CompositeOr0DigitChapters CompositeOr0DigitVerses");
        str.AppendLine(DoC0C0DigitChaptersVerses(client, verses));
        str.AppendLine("OddDigitChapters EvenDigitVerses");
        str.AppendLine(DoOEDigitChaptersVerses(client, verses));
        str.AppendLine("EvenDigitChapters OddDigitVerses");
        str.AppendLine(DoEODigitChaptersVerses(client, verses));
        str.AppendLine("PrimeDigitChapters CompositeDigitVerses");
        str.AppendLine(DoPCDigitChaptersVerses(client, verses));
        str.AppendLine("CompositeDigitChapters PrimeDigitVerses");
        str.AppendLine(DoCPDigitChaptersVerses(client, verses));
        str.AppendLine("PrimeOr1DigitChapters CompositeOr0DigitVerses");
        str.AppendLine(DoP1C0DigitChaptersVerses(client, verses));
        str.AppendLine("CompositeOr0DigitChapters PrimeOr1DigitVerses");
        str.AppendLine(DoC0P1DigitChaptersVerses(client, verses));

        return str.ToString();
    }
    private static string DoOddDigitChapters(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsOddDigits(chapter.SortedNumber))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoEvenDigitChapters(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsEvenDigits(chapter.SortedNumber))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoPrimeDigitChapters(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsPrimeDigits(chapter.SortedNumber))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoCompositeDigitChapters(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsCompositeDigits(chapter.SortedNumber))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoPrimeOr1DigitChapters(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsPrimeOr1Digits(chapter.SortedNumber))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoCompositeOr0DigitChapters(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsCompositeOr0Digits(chapter.SortedNumber))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoOddDigitVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsOddDigits(chapter.Verses.Count))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoEvenDigitVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsEvenDigits(chapter.Verses.Count))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoPrimeDigitVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsPrimeDigits(chapter.Verses.Count))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoCompositeDigitVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsCompositeDigits(chapter.Verses.Count))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoPrimeOr1DigitVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsPrimeOr1Digits(chapter.Verses.Count))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoCompositeOr0DigitVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if (Numbers.IsCompositeOr0Digits(chapter.Verses.Count))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoOODigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsOddDigits(chapter.SortedNumber)) && (Numbers.IsOddDigits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoEEDigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsEvenDigits(chapter.SortedNumber)) && (Numbers.IsEvenDigits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoPPDigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsPrimeDigits(chapter.SortedNumber)) && (Numbers.IsPrimeDigits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoCCDigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsCompositeDigits(chapter.SortedNumber)) && (Numbers.IsCompositeDigits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoP1P1DigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsPrimeOr1Digits(chapter.SortedNumber)) && (Numbers.IsPrimeOr1Digits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoC0C0DigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsCompositeOr0Digits(chapter.SortedNumber)) && (Numbers.IsCompositeOr0Digits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoOEDigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsOddDigits(chapter.SortedNumber)) && (Numbers.IsEvenDigits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoEODigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsEvenDigits(chapter.SortedNumber)) && (Numbers.IsOddDigits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoPCDigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsPrimeDigits(chapter.SortedNumber)) && (Numbers.IsCompositeDigits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoCPDigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsCompositeDigits(chapter.SortedNumber)) && (Numbers.IsPrimeDigits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoP1C0DigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsPrimeOr1Digits(chapter.SortedNumber)) && (Numbers.IsCompositeOr0Digits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string DoC0P1DigitChaptersVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (verses == null) return null;
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<Chapter> matching_chapters = new List<Chapter>();
        foreach (Chapter chapter in chapters)
        {
            if ((Numbers.IsCompositeOr0Digits(chapter.SortedNumber)) && (Numbers.IsPrimeOr1Digits(chapter.Verses.Count)))
            {
                matching_chapters.Add(chapter);
            }
        }
        return BuildChapterVersesTable(matching_chapters);
    }
    private static string BuildChapterVersesTable(List<Chapter> chapters)
    {
        StringBuilder str = new StringBuilder();
        str.AppendLine("\t" + "----------------------");
        str.AppendLine("\t" + "#" + "\t" + "Chapter" + "\t" + "Verses");
        str.AppendLine("\t" + "----------------------");

        int count = 0;
        int chapter_sum = 0;
        int verse_sum = 0;
        foreach (Chapter chapter in chapters)
        {
            count++;
            chapter_sum += chapter.SortedNumber;
            verse_sum += chapter.Verses.Count;

            str.AppendLine("\t" + count + "\t" + chapter.SortedNumber + "\t" + chapter.Verses.Count);
        }
        str.AppendLine("\t" + "----------------------");
        str.AppendLine("\t" + "Sum" + "\t" + chapter_sum + "\t" + verse_sum);
        str.AppendLine("\t" + "----------------------");
        str.AppendLine("\t" + "Factors" + "\t" + Numbers.FactorizeToString(chapter_sum) + "\t" + Numbers.FactorizeToString(verse_sum));
        str.AppendLine("\t" + "----------------------");

        int chapter_sum_p_index = Numbers.PrimeIndexOf(chapter_sum) + 1;
        int chapter_sum_ap_index = Numbers.AdditivePrimeIndexOf(chapter_sum) + 1;
        int chapter_sum_xp_index = Numbers.NonAdditivePrimeIndexOf(chapter_sum) + 1;
        int chapter_sum_c_index = Numbers.CompositeIndexOf(chapter_sum) + 1;
        int chapter_sum_ac_index = Numbers.AdditiveCompositeIndexOf(chapter_sum) + 1;
        int chapter_sum_xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter_sum) + 1;
        int verse_sum_p_index = Numbers.PrimeIndexOf(verse_sum) + 1;
        int verse_sum_ap_index = Numbers.AdditivePrimeIndexOf(verse_sum) + 1;
        int verse_sum_xp_index = Numbers.NonAdditivePrimeIndexOf(verse_sum) + 1;
        int verse_sum_c_index = Numbers.CompositeIndexOf(verse_sum) + 1;
        int verse_sum_ac_index = Numbers.AdditiveCompositeIndexOf(verse_sum) + 1;
        int verse_sum_xc_index = Numbers.NonAdditiveCompositeIndexOf(verse_sum) + 1;

        str.Append("\t" + "\t");
        if (Numbers.IsUnit(chapter_sum)) str.Append("U1");
        else if (Numbers.IsPrime(chapter_sum)) str.Append("P" + chapter_sum_p_index);
        else str.Append("C" + chapter_sum_c_index);
        str.Append("\t");
        if (Numbers.IsPrime(verse_sum)) str.Append("P" + verse_sum_p_index);
        else str.Append("C" + verse_sum_c_index);
        str.AppendLine();

        str.Append("\t" + "\t");
        if (Numbers.IsUnit(chapter_sum)) str.Append("U1");
        else if (Numbers.IsPrime(chapter_sum)) str.Append("AP" + chapter_sum_ap_index);
        else str.Append("AC" + chapter_sum_ac_index);
        str.Append("\t");
        if (Numbers.IsPrime(verse_sum)) str.Append("AP" + verse_sum_ap_index);
        else str.Append("AC" + verse_sum_ac_index);
        str.AppendLine();

        str.Append("\t" + "\t");
        if (Numbers.IsUnit(chapter_sum)) str.Append("U1");
        else if (Numbers.IsPrime(chapter_sum)) str.Append("XP" + chapter_sum_xp_index);
        else str.Append("XC" + chapter_sum_xc_index);
        str.Append("\t");
        if (Numbers.IsPrime(verse_sum)) str.Append("XP" + verse_sum_xp_index);
        else str.Append("XC" + verse_sum_xc_index);
        str.AppendLine();

        return str.ToString();
    }

    private static string _______________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    private static string JumpWordsByX(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByX(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByValue(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByValue(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByPrimeNumbers(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByPrimeNumbers(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByAdditivePrimeNumbers(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByAdditivePrimeNumbers(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByNonAdditivePrimeNumbers(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByNonAdditivePrimeNumbers(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByCompositeNumbers(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByCompositeNumbers(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByAdditiveCompositeNumbers(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByAdditiveCompositeNumbers(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByNonAdditiveCompositeNumbers(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByNonAdditiveCompositeNumbers(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByFibonacciNumbers(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByFibonacciNumbers(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByPiDigits(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByPiDigits(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByEulerDigits(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByEulerDigits(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string JumpWordsByGoldenRatioDigits(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoJumpWordsByGoldenRatioDigits(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static List<string> DoJumpWordsByX(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        int step;
        try
        {
            step = int.Parse(param);
        }
        catch
        {
            step = 1;
        }
        step++; // jump gap

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += step;
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByValue(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)client.CalculateValue(result[result.Count - 1]);
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByPrimeNumbers(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        int n = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)Numbers.Primes[n++];
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByAdditivePrimeNumbers(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        int n = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)Numbers.AdditivePrimes[n++];
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByNonAdditivePrimeNumbers(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        int n = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)Numbers.NonAdditivePrimes[n++];
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByCompositeNumbers(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        int n = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)Numbers.Composites[n++];
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByAdditiveCompositeNumbers(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        int n = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)Numbers.AdditiveComposites[n++];
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByNonAdditiveCompositeNumbers(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        int n = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)Numbers.NonAdditiveComposites[n++];
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByFibonacciNumbers(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        int n = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)Numbers.Fibonaccis[n++];
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByPiDigits(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        int n = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)Numbers.PiDigits[n++];
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByEulerDigits(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        int n = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)Numbers.EDigits[n++];
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }
    private static List<string> DoJumpWordsByGoldenRatioDigits(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        int i = 0;
        int n = 0;
        while (true)
        {
            if (result.Count > 0)
            {
                i += (int)Numbers.PhiDigits[n++];
            }
            if (i >= words.Count) break;

            result.Add(words[i].Text + " ");
        }

        return result;
    }

    private static string ________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    private static string PrimeWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoPrimeWords(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string AdditivePrimeWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoAdditivePrimeWords(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string NonAdditivePrimeWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoNonAdditivePrimeWords(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string CompositeWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoCompositeWords(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string AdditiveCompositeWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoAdditiveCompositeWords(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string NonAdditiveCompositeWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoNonAdditiveCompositeWords(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static string FibonacciWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<string> result = DoFibonacciWords(client, verses, param);
            StringBuilder str = new StringBuilder();
            foreach (string xxx in result)
            {
                str.AppendLine(xxx);
            }
            return str.ToString();
        }
        return null;
    }
    private static List<string> DoPrimeWords(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        for (int i = 0; i < words.Count; i++)
        {
            if (Numbers.IsPrime(i + 1))
            {
                result.Add(words[i].Text + " ");
            }
        }

        return result;
    }
    private static List<string> DoAdditivePrimeWords(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        for (int i = 0; i < words.Count; i++)
        {
            if (Numbers.IsAdditivePrime(i + 1))
            {
                result.Add(words[i].Text + " ");
            }
        }

        return result;
    }
    private static List<string> DoNonAdditivePrimeWords(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        for (int i = 0; i < words.Count; i++)
        {
            if (Numbers.IsNonAdditivePrime(i + 1))
            {
                result.Add(words[i].Text + " ");
            }
        }

        return result;
    }
    private static List<string> DoCompositeWords(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        for (int i = 0; i < words.Count; i++)
        {
            if (Numbers.IsComposite(i + 1))
            {
                result.Add(words[i].Text + " ");
            }
        }

        return result;
    }
    private static List<string> DoAdditiveCompositeWords(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        for (int i = 0; i < words.Count; i++)
        {
            if (Numbers.IsAdditiveComposite(i + 1))
            {
                result.Add(words[i].Text + " ");
            }
        }

        return result;
    }
    private static List<string> DoNonAdditiveCompositeWords(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        for (int i = 0; i < words.Count; i++)
        {
            if (Numbers.IsNonAdditiveComposite(i + 1))
            {
                result.Add(words[i].Text + " ");
            }
        }

        return result;
    }
    private static List<string> DoFibonacciWords(Client client, List<Verse> verses, string param)
    {
        List<string> result = new List<string>();

        List<Word> words = new List<Word>();
        foreach (Verse verse in verses)
        {
            words.AddRange(verse.Words);
        }

        for (int i = 0; i < words.Count; i++)
        {
            if (Numbers.IsFibonacci(i + 1))
            {
                result.Add(words[i].Text + " ");
            }
        }

        return result;
    }

    public static string _________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    public static string ChapterVersesSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<long> result = new List<long>();
        foreach (Chapter chapter in chapters)
        {
            result.Add(chapter.Verses.Count);
        }

        string filename = client.NumerologySystem.Name + "_" + "ChapterVerses" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string ChapterWordsSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<long> result = new List<long>();
        foreach (Chapter chapter in chapters)
        {
            result.Add(chapter.WordCount);
        }

        string filename = client.NumerologySystem.Name + "_" + "ChapterWords" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string ChapterLettersSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<long> result = new List<long>();
        foreach (Chapter chapter in chapters)
        {
            result.Add(chapter.LetterCount);
        }

        string filename = client.NumerologySystem.Name + "_" + "ChapterLetters" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string ChapterValuesSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        List<Chapter> chapters = client.Book.GetChapters(verses);

        List<long> result = new List<long>();
        foreach (Chapter chapter in chapters)
        {
            result.Add(client.CalculateValue(chapter));
        }

        string filename = client.NumerologySystem.Name + "_" + "ChapterValues" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string VerseWordsSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);

        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            result.Add(verse.Words.Count);
        }

        string filename = client.NumerologySystem.Name + "_" + "VerseWords" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string VerseLettersSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);

        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            result.Add(verse.LetterCount);
        }

        string filename = client.NumerologySystem.Name + "_" + "VerseLetters" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string VerseValuesSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);

        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            result.Add(client.CalculateValue(verse));
        }

        string filename = client.NumerologySystem.Name + "_" + "VerseValues" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string VerseDistancesSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);

        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            result.Add(verse.DistanceToPrevious.dV);
        }

        string filename = client.NumerologySystem.Name + "_" + "VerseDistances" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string WordLettersSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);

        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            foreach (Word word in verse.Words)
            {
                result.Add(word.Letters.Count);
            }
        }

        string filename = client.NumerologySystem.Name + "_" + "WordLetters" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string WordValuesSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);

        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            foreach (Word word in verse.Words)
            {
                result.Add(client.CalculateValue(word));
            }
        }

        string filename = client.NumerologySystem.Name + "_" + "WordValues" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string WordDistancesSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);

        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            foreach (Word word in verse.Words)
            {
                result.Add(word.DistanceToPrevious.dW);
            }
        }

        string filename = client.NumerologySystem.Name + "_" + "WordDistances" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string WordFrequenciesSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);

        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            foreach (Word word in verse.Words)
            {
                string word_text = word.Text.SimplifyTo(client.NumerologySystem.TextMode);
                result.Add(word.Frequency);
            }
        }

        string filename = client.NumerologySystem.Name + "_" + "WordFrequencies" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string LetterValuesSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);

        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            foreach (Word word in verse.Words)
            {
                foreach (Letter letter in word.Letters)
                {
                    result.Add(client.CalculateValue(letter));
                }
            }
        }

        string filename = client.NumerologySystem.Name + "_" + "LetterValues" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    public static string LetterDistancesSound(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);

        List<long> result = new List<long>();
        foreach (Verse verse in verses)
        {
            foreach (Word word in verse.Words)
            {
                foreach (Letter letter in word.Letters)
                {
                    result.Add(letter.DistanceToPrevious.dL);
                }
            }
        }

        string filename = client.NumerologySystem.Name + "_" + "LetterDistances" + ".txt";
        if (Directory.Exists(Globals.RESEARCH_FOLDER))
        {
            string path = Globals.RESEARCH_FOLDER + "/" + filename;
            DoSaveAndPlayWAVFile(path, result, param);
        }

        StringBuilder str = new StringBuilder();
        foreach (long xxx in result)
        {
            str.AppendLine(xxx.ToString());
        }
        return str.ToString();
    }
    private static void DoSaveAndPlayWAVFile(string path, List<long> result, string param)
    {
        FileHelper.SaveValues(path, result);
        FileHelper.DisplayFile(path); // *.csv

        int sample_rate = 0;
        if (param.Length > 0)
        {
            if (int.TryParse(param, out sample_rate))
            {
                if (sample_rate > 0)
                {
                    // update ref path.csv to .wav
                    WAVFile.GenerateWAVFile(ref path, result, sample_rate);

                    // play .wav file
                    WAVFile.PlayWAVFile(path);
                }
            }
            else
            {
                throw new Exception("Invalid sample_rate = " + param);
            }
        }
    }

    public static string ___________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    public static string ChapterInformation(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Chapter> chapters = client.Book.GetChapters(verses);
            if (chapters != null)
            {
                return DoChapterInformation(client, chapters);
            }
        }
        return null;
    }
    public static string ChapterValues(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Chapter> chapters = client.Book.GetChapters(verses);
            if (chapters != null)
            {
                return DoChapterValues(client, chapters);
            }
        }
        return null;
    }
    public static string ChapterLetterRatios(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Chapter> chapters = client.Book.GetChapters(verses);
            if (chapters != null)
            {
                return DoChapterLetterRatios(client, chapters);
            }
        }
        return null;
    }
    public static string VerseInformation(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoVerseInformation(client, verses);
        }
        return null;
    }
    public static string VerseValues(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoVerseValues(client, verses);
        }
        return null;
    }
    public static string VerseStarts(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoVerseStarts(client, verses);
        }
        return null;
    }
    public static string VerseEnds(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoVerseEnds(client, verses);
        }
        return null;
    }
    public static string WordInformation(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Word> words = new List<Word>();
            foreach (Verse verse in verses)
            {
                words.AddRange(verse.Words);
            }
            return DoWordInformation(client, words);
        }
        return null;
    }
    public static string WordPartInformation(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Word> words = new List<Word>();
            foreach (Verse verse in verses)
            {
                words.AddRange(verse.Words);
            }
            return DoWordPartInformation(client, words);
        }
        return null;
    }
    public static string LetterFrequencySums(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoLetterFrequencySums(client, verses, param);
        }
        return null;
    }
    public static string UniqueChapterWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Chapters == null) return null;

        List<Chapter> chapters = client.Book.Chapters;
        Dictionary<Chapter, List<Word>> unique_chapter_words = new Dictionary<Chapter, List<Word>>();
        foreach (Chapter chapter in chapters)
        {
            unique_chapter_words.Add(chapter, new List<Word>());
        }

        foreach (Chapter chapter in chapters)
        {
            foreach (Verse verse in chapter.Verses)
            {
                foreach (Word word in verse.Words)
                {
                    bool unique = true;

                    foreach (Chapter c in client.Book.Chapters)
                    {
                        if (c == chapter) continue;

                        foreach (Verse v in c.Verses)
                        {
                            foreach (Word w in v.Words)
                            {
                                if (w.Text == word.Text)
                                {
                                    unique = false;
                                    break;
                                }
                            }
                            if (!unique) break;
                        }
                        if (!unique) break;
                    }

                    if (unique)
                    {
                        if (!unique_chapter_words[chapter].Contains(word))
                        {
                            unique_chapter_words[chapter].Add(word);
                        }
                    }
                }
            }
        }

        StringBuilder str = new StringBuilder();
        int i = 0;
        foreach (List<Word> words in unique_chapter_words.Values)
        {
            str.Append("Chapter " + chapters[i++].SortedNumber.ToString() + "\t");
            str.Append(words.Count + "\t");
            foreach (Word word in words)
            {
                str.Append(word.Text + "\t");
            }
            if (str.Length > 0)
            {
                str.Remove(str.Length - 1, 1);
            }
            str.AppendLine();
        }

        return str.ToString();
    }
    public static string UniqueChapterRoots(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Chapters == null) return null;

        List<Chapter> chapters = client.Book.Chapters;
        Dictionary<Chapter, List<string>> unique_chapter_roots = new Dictionary<Chapter, List<string>>();
        foreach (Chapter chapter in chapters)
        {
            unique_chapter_roots.Add(chapter, new List<string>());
        }

        foreach (Chapter chapter in chapters)
        {
            foreach (Verse verse in chapter.Verses)
            {
                foreach (Word word in verse.Words)
                {
                    bool unique = true;

                    foreach (Chapter c in client.Book.Chapters)
                    {
                        if (c == chapter) continue;

                        foreach (Verse v in c.Verses)
                        {
                            foreach (Word w in v.Words)
                            {
                                if (w.BestRoot == word.BestRoot)
                                {
                                    unique = false;
                                    break;
                                }
                            }
                            if (!unique) break;
                        }
                        if (!unique) break;
                    }

                    if (unique)
                    {
                        if (!unique_chapter_roots[chapter].Contains(word.BestRoot))
                        {
                            unique_chapter_roots[chapter].Add(word.BestRoot);
                        }
                    }
                }
            }
        }

        StringBuilder str = new StringBuilder();
        int i = 0;
        foreach (List<string> roots in unique_chapter_roots.Values)
        {
            str.Append("Chapter " + chapters[i++].SortedNumber.ToString() + "\t");
            str.Append(roots.Count + "\t");
            foreach (string root in roots)
            {
                str.Append(root + "\t");
            }
            if (str.Length > 0)
            {
                str.Remove(str.Length - 1, 1);
            }
            str.AppendLine();
        }

        return str.ToString();
    }
    private static string DoChapterInformation(Client client, List<Chapter> chapters)
    {
        if (client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();

        str.Append("#" + "\t" + "Name" + "\t" + "Transliteration" + "\t" + "English" + "\t" + "Place" + "\t" + "Order" + "\t" + "Page" + "\t" + "Chapter" + "\t" + "Verses" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "Factors" + "\t" + "P" + "\t" + "AP" + "\t" + "XP" + "\t" + "C" + "\t" + "AC" + "\t" + "XC" + "\t");

        NumerologySystem numerology_system = client.NumerologySystem;
        if (numerology_system != null)
        {
            if (numerology_system.LetterValues.Keys.Count > 0)
            {
                foreach (char key in numerology_system.LetterValues.Keys)
                {
                    str.Append(key.ToString() + "\t");
                }
                if (str.Length > 1)
                {
                    str.Remove(str.Length - 1, 1); // \t
                }
                str.AppendLine();
            }

            int count = 0;
            foreach (Chapter chapter in chapters)
            {
                count++;
                str.Append(count.ToString() + "\t");
                str.Append(chapter.Name + "\t");
                str.Append(chapter.TransliteratedName + "\t");
                str.Append(chapter.EnglishName + "\t");
                str.Append(chapter.RevelationPlace.ToString() + "\t");
                str.Append(chapter.RevelationOrder.ToString() + "\t");
                str.Append(chapter.Verses[0].Page.Number.ToString() + "\t");
                str.Append(chapter.SortedNumber.ToString() + "\t");
                str.Append(chapter.Verses.Count.ToString() + "\t");
                str.Append(chapter.WordCount.ToString() + "\t");
                str.Append(chapter.LetterCount.ToString() + "\t");
                str.Append(chapter.UniqueLetters.Count.ToString() + "\t");

                long value = client.CalculateValue(chapter);
                str.Append(value.ToString() + "\t");
                str.Append(Numbers.FactorizeToString(value) + "\t");

                int p = Numbers.PrimeIndexOf(value) + 1;
                int ap = Numbers.AdditivePrimeIndexOf(value) + 1;
                int xp = Numbers.NonAdditivePrimeIndexOf(value) + 1;
                int c = Numbers.CompositeIndexOf(value) + 1;
                int ac = Numbers.AdditiveCompositeIndexOf(value) + 1;
                int xc = Numbers.NonAdditiveCompositeIndexOf(value) + 1;
                str.Append((p == 0 ? "-" : p.ToString()) + "\t"
                        + (ap == 0 ? "-" : ap.ToString()) + "\t"
                        + (xp == 0 ? "-" : xp.ToString()) + "\t"
                        + (c == 0 ? "-" : c.ToString()) + "\t"
                        + (ac == 0 ? "-" : ac.ToString()) + "\t"
                        + (xc == 0 ? "-" : xc.ToString())
                        );
                str.Append("\t");

                if (numerology_system.LetterValues.Keys.Count > 0)
                {
                    foreach (char key in numerology_system.LetterValues.Keys)
                    {
                        str.Append(chapter.GetLetterFrequency(key) + "\t");
                    }
                    if (str.Length > 1)
                    {
                        str.Remove(str.Length - 1, 1); // \t
                    }
                    str.AppendLine();
                }
            }
            if (str.Length > 2)
            {
                str.Remove(str.Length - 2, 2);
            }
        }
        return str.ToString();
    }
    private static string DoChapterValues(Client client, List<Chapter> chapters)
    {
        if (client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("#" + "\t" + "Name" + "\t" + "Chapter" + "\t");
        foreach (string key in client.LoadedNumerologySystems.Keys)
        {
            if (key.StartsWith(client.NumerologySystem.TextMode))
            {
                str.Append(key.Substring(client.NumerologySystem.TextMode.Length + 1) + "\t");
            }
        }
        if (str.Length > 1)
        {
            str.Remove(str.Length - 1, 1); // \t
        }
        str.AppendLine();

        int count = 0;
        foreach (Chapter chapter in chapters)
        {
            count++;
            str.Append(count.ToString() + "\t");
            str.Append(chapter.Name + "\t");
            str.Append(chapter.SortedNumber + "\t");
            foreach (NumerologySystem numerology_system in client.LoadedNumerologySystems.Values)
            {
                if (numerology_system.TextMode == client.NumerologySystem.TextMode)
                {
                    long value = numerology_system.CalculateValue(chapter.Text);
                    str.Append(value.ToString() + "\t");
                }
            }
            if (str.Length > 1)
            {
                str.Remove(str.Length - 1, 1);
            }
            str.AppendLine();
        }
        str.AppendLine();

        string duplicates = GetDuplicateChapterValues(client, chapters);
        if (duplicates.Length > 0)
        {
            str.AppendLine(duplicates);
        }
        else
        {
            str.AppendLine("No duplicate values.");
        }

        if (str.Length > 2)
        {
            str.Remove(str.Length - 2, 2);
        }

        return str.ToString();
    }
    private static string DoChapterLetterRatios(Client client, List<Chapter> chapters)
    {
        if (client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();

        str.Append("#" + "\t" + "Name" + "\t" + "Chapter" + "\t" + "Verses" + "\t" + "Words" + "\t" + "Letters" + "\t");

        NumerologySystem numerology_system = client.NumerologySystem;
        if (numerology_system != null)
        {
            if (numerology_system.LetterValues.Keys.Count > 0)
            {
                foreach (char key in numerology_system.LetterValues.Keys)
                {
                    str.Append(key.ToString() + "\t");
                }
                if (str.Length > 1)
                {
                    str.Remove(str.Length - 1, 1); // \t
                }
                str.AppendLine();
            }

            int count = 0;
            int sum = 0;
            int chapter_sum = 0;
            int verse_sum = 0;
            int word_sum = 0;
            int letter_sum = 0;
            foreach (Chapter chapter in chapters)
            {
                count++;
                sum += count;
                chapter_sum += chapter.SortedNumber;
                verse_sum += chapter.Verses.Count;
                word_sum += chapter.WordCount;
                letter_sum += chapter.LetterCount;

                str.Append(count + "\t");
                str.Append(chapter.Name + "\t");
                str.Append(chapter.SortedNumber.ToString() + "\t");
                str.Append(chapter.Verses.Count.ToString() + "\t");
                str.Append(chapter.WordCount.ToString() + "\t");
                str.Append(chapter.LetterCount.ToString() + "\t");

                if (numerology_system.LetterValues.Keys.Count > 0)
                {
                    foreach (char key in numerology_system.LetterValues.Keys)
                    {
                        str.Append((((double)chapter.GetLetterFrequency(key) * 100.0D) / chapter.LetterCount) + "%" + "\t");
                    }
                    if (str.Length > 1)
                    {
                        str.Remove(str.Length - 1, 1); // \t
                    }
                    str.AppendLine();
                }
            }
            if (str.Length > 2)
            {
                str.Remove(str.Length - 2, 2);
            }

            str.AppendLine();
            str.AppendLine(sum + "\t" + "Sum" + "\t" + chapter_sum + "\t" + verse_sum + "\t" + word_sum + "\t" + letter_sum);
        }

        return str.ToString();
    }
    private static string DoVerseInformation(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();

        str.Append("#" + "\t" + "Number" + "\t" + "Page" + "\t" + "Chapter" + "\t" + "Verse" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "Factors" + "\t" + "P" + "\t" + "AP" + "\t" + "XP" + "\t" + "C" + "\t" + "AC" + "\t" + "XC" + "\t");

        NumerologySystem numerology_system = client.NumerologySystem;
        if (numerology_system != null)
        {
            foreach (char key in numerology_system.LetterValues.Keys)
            {
                str.Append(key.ToString() + "\t");
            }
            str.Append("Text");
            str.AppendLine();

            int count = 0;
            foreach (Verse verse in verses)
            {
                count++;
                str.Append(count.ToString() + "\t");
                str.Append(verse.Number.ToString() + "\t");
                str.Append(verse.Page.Number.ToString() + "\t");
                str.Append(verse.Chapter.SortedNumber.ToString() + "\t");
                str.Append(verse.NumberInChapter.ToString() + "\t");
                str.Append(verse.Words.Count.ToString() + "\t");
                str.Append(verse.LetterCount.ToString() + "\t");
                str.Append(verse.UniqueLetters.Count.ToString() + "\t");

                long value = client.CalculateValue(verse);
                str.Append(value.ToString() + "\t");
                str.Append(Numbers.FactorizeToString(value) + "\t");

                int p = Numbers.PrimeIndexOf(value) + 1;
                int ap = Numbers.AdditivePrimeIndexOf(value) + 1;
                int xp = Numbers.NonAdditivePrimeIndexOf(value) + 1;
                int c = Numbers.CompositeIndexOf(value) + 1;
                int ac = Numbers.AdditiveCompositeIndexOf(value) + 1;
                int xc = Numbers.NonAdditiveCompositeIndexOf(value) + 1;
                str.Append((p == 0 ? "-" : p.ToString()) + "\t"
                        + (ap == 0 ? "-" : ap.ToString()) + "\t"
                        + (xp == 0 ? "-" : xp.ToString()) + "\t"
                        + (c == 0 ? "-" : c.ToString()) + "\t"
                        + (ac == 0 ? "-" : ac.ToString()) + "\t"
                        + (xc == 0 ? "-" : xc.ToString())
                             );
                str.Append("\t");

                foreach (char character in numerology_system.LetterValues.Keys)
                {
                    if (Constants.INDIAN_DIGITS.Contains(character)) continue;
                    if (Constants.STOPMARKS.Contains(character)) continue;
                    if (Constants.QURANMARKS.Contains(character)) continue;
                    if (character == '{') continue;
                    if (character == '}') continue;
                    str.Append(verse.GetLetterFrequency(character).ToString() + "\t");
                }

                str.Append(verse.Text);

                str.AppendLine();
            }
            if (str.Length > 2)
            {
                str.Remove(str.Length - 2, 2);
            }
        }
        return str.ToString();
    }
    private static string DoVerseValues(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("#" + "\t" + "Chapter" + "\t" + "Verse" + "\t");
        foreach (string key in client.LoadedNumerologySystems.Keys)
        {
            if (key.StartsWith(client.NumerologySystem.TextMode))
            {
                str.Append(key.Substring(client.NumerologySystem.TextMode.Length + 1) + "\t");
            }
        }
        if (str.Length > 1)
        {
            str.Remove(str.Length - 1, 1); // \t
        }
        str.AppendLine();

        int count = 0;
        foreach (Verse verse in verses)
        {
            count++;
            str.Append(count.ToString() + "\t");
            str.Append(verse.Chapter.SortedNumber + "\t");
            str.Append(verse.NumberInChapter + "\t");
            foreach (NumerologySystem numerology_system in client.LoadedNumerologySystems.Values)
            {
                if (numerology_system.TextMode == client.NumerologySystem.TextMode)
                {
                    long value = numerology_system.CalculateValue(verse.Text);
                    str.Append(value.ToString() + "\t");
                }
            }
            if (str.Length > 1)
            {
                str.Remove(str.Length - 1, 1);
            }
            str.AppendLine();
        }
        str.AppendLine();

        string duplicates = GetDuplicateVerseValues(client, verses);
        if (duplicates.Length > 0)
        {
            str.AppendLine(duplicates);
        }
        else
        {
            str.AppendLine("No duplicate values.");
        }

        if (str.Length > 2)
        {
            str.Remove(str.Length - 2, 2);
        }

        return str.ToString();
    }
    private static string DoVerseStarts(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.AppendLine("Letter" + "\t" + "Frequency");

        foreach (char c in Constants.ARABIC_LETTERS)
        {
            int count = 0;
            foreach (Verse verse in verses)
            {
                char first = '\0';
                for (int i = 0; i < verse.Text.Length; i++)
                {
                    if (Constants.ARABIC_LETTERS.Contains(verse.Text[i]))
                    {
                        first = verse.Text[i];
                        break;
                    }
                }

                if (c == first)
                {
                    count++;
                }
            }
            str.AppendLine(c.ToString() + "\t" + count.ToString());
        }
        if (str.Length > 2)
        {
            str.Remove(str.Length - 2, 2); // \r\n
        }

        return str.ToString();
    }
    private static string DoVerseEnds(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.AppendLine("Letter" + "\t" + "Frequency");

        foreach (char c in Constants.ARABIC_LETTERS)
        {
            int count = 0;
            foreach (Verse verse in verses)
            {
                char last = '\0';
                for (int i = verse.Text.Length - 1; i >= 0; i--)
                {
                    if (Constants.ARABIC_LETTERS.Contains(verse.Text[i]))
                    {
                        last = verse.Text[i];
                        break;
                    }
                }

                if (c == last)
                {
                    count++;
                }
            }
            str.AppendLine(c.ToString() + "\t" + count.ToString());
        }
        if (str.Length > 2)
        {
            str.Remove(str.Length - 2, 2); // \r\n
        }

        return str.ToString();
    }
    private static string DoWordInformation(Client client, List<Word> words)
    {
        if (client == null) return null;
        if (words == null) return null;

        StringBuilder str = new StringBuilder();
        if (words.Count > 0)
        {
            str.AppendLine
            (
                "#" + "\t" +
                "Number" + "\t" +
                "InChap" + "\t" +
                "InVerse" + "\t" +
                "Address" + "\t" +
                "Chapter" + "\t" +
                "Verse" + "\t" +
                "Word" + "\t" +
                "Text" + "\t" +
                "Transliteration" + "\t" +
                "ArabicGrammar" + "\t" +
                "EnglishGrammar" + "\t" +
                "Roots" + "\t" +
                "Meaning" + "\t" +
                "Occurrence" + "\t" +
                "Frequency" + "\t" +
                "Letters" + "\t" + "Unique" + "\t" +
                "Value" + "\t" + "Factors" + "\t" + "P" + "\t" + "AP" + "\t" + "XP" + "\t" + "C" + "\t" + "AC" + "\t" + "XC"
            );

            int count = 0;
            foreach (Word word in words)
            {
                List<string> roots = word.Roots;
                StringBuilder roots_str = new StringBuilder();
                if (roots.Count > 0)
                {
                    foreach (string root in roots)
                    {
                        roots_str.Append(root + "|");
                    }
                    roots_str.Remove(roots_str.Length - 1, 1);
                }

                StringBuilder parts_arabic_grammar_str = new StringBuilder();
                if (word.Parts.Count > 0)
                {
                    foreach (WordPart word_part in word.Parts)
                    {
                        parts_arabic_grammar_str.Append(
                            GrammarDictionary.Arabic(word_part.Grammar.Position) +
                            " " +
                            GrammarDictionary.Arabic(word_part.Grammar.Attribute) +
                            ((word_part.Grammar.Position == "V") ? (" " + GrammarDictionary.Arabic(word_part.Grammar.Qualifier)) : "") +
                            " و");
                    }
                    parts_arabic_grammar_str.Remove(parts_arabic_grammar_str.Length - 2, 2);
                    parts_arabic_grammar_str.Replace("  ", " ");
                }

                StringBuilder parts_english_grammar_str = new StringBuilder();
                if (word.Parts.Count > 0)
                {
                    foreach (WordPart word_part in word.Parts)
                    {
                        parts_english_grammar_str.Append(
                            GrammarDictionary.English(word_part.Grammar.Position) +
                            " " +
                            GrammarDictionary.English(word_part.Grammar.Attribute) +
                            ((word_part.Grammar.Position == "V") ? (" " + GrammarDictionary.English(word_part.Grammar.Qualifier)) : "") +
                            " and ");
                    }
                    parts_english_grammar_str.Remove(parts_english_grammar_str.Length - 5, 5);
                    parts_english_grammar_str.Replace("  ", " ");
                }

                count++;
                str.Append
                (
                    count + "\t" +
                    word.Number + "\t" +
                    word.NumberInChapter + "\t" +
                    word.NumberInVerse + "\t" +
                    word.Address + "\t" +
                    word.Verse.Chapter.SortedNumber + "\t" +
                    word.Verse.NumberInChapter + "\t" +
                    word.NumberInVerse + "\t" +
                    word.Text + "\t" +
                    word.Transliteration + "\t" +
                    parts_arabic_grammar_str + "\t" +
                    parts_english_grammar_str + "\t" +
                    roots_str + "\t" +
                    word.Meaning + "\t" +
                    word.Occurrence + "\t" +
                    word.Frequency + "\t" +
                    word.Letters.Count + "\t" +
                    word.UniqueLetters.Count.ToString() + "\t"
                );

                long value = client.CalculateValue(word);
                str.Append(value.ToString() + "\t");
                str.Append(Numbers.FactorizeToString(value) + "\t");

                int p = Numbers.PrimeIndexOf(value) + 1;
                int ap = Numbers.AdditivePrimeIndexOf(value) + 1;
                int xp = Numbers.NonAdditivePrimeIndexOf(value) + 1;
                int c = Numbers.CompositeIndexOf(value) + 1;
                int ac = Numbers.AdditiveCompositeIndexOf(value) + 1;
                int xc = Numbers.NonAdditiveCompositeIndexOf(value) + 1;
                str.Append((p == 0 ? "-" : p.ToString()) + "\t"
                        + (ap == 0 ? "-" : ap.ToString()) + "\t"
                        + (xp == 0 ? "-" : xp.ToString()) + "\t"
                        + (c == 0 ? "-" : c.ToString()) + "\t"
                        + (ac == 0 ? "-" : ac.ToString()) + "\t"
                        + (xc == 0 ? "-" : xc.ToString())
                             );
                str.AppendLine();
            }
        }
        return str.ToString();
    }
    private static string DoWordPartInformation(Client client, List<Word> words)
    {
        if (client == null) return null;
        if (words == null) return null;

        StringBuilder str = new StringBuilder();
        if (words.Count > 0)
        {
            str.AppendLine
            (
                "Address" + "\t" +
                "Chapter" + "\t" +
                "Verse" + "\t" +
                "Word" + "\t" +
                "Part" + "\t" +
                "Text" + "\t" +
                "Buckwalter" + "\t" +
                "Tag" + "\t" +
                "ArabicGrammar" + "\t" +
                "EnglishGrammar" + "\t" +
                "Type" + "\t" +
                "Position" + "\t" +
                "Attribute" + "\t" +
                "Qualifier" + "\t" +
                "PersonDegree" + "\t" +
                "PersonGender" + "\t" +
                "PersonNumber" + "\t" +
                "Mood" + "\t" +
                "Lemma" + "\t" +
                "Root" + "\t" +
                "SpecialGroup" + "\t" +
                "WordAddress"
            );

            foreach (Word word in words)
            {
                if (word.Parts != null)
                {
                    foreach (WordPart part in word.Parts)
                    {
                        str.AppendLine
                        (
                            part.Address + "\t" +
                            part.Word.Verse.Chapter.SortedNumber + "\t" +
                            part.Word.Verse.NumberInChapter + "\t" +
                            part.Word.NumberInVerse + "\t" +
                            part.NumberInWord + "\t" +
                            part.Text + "\t" +
                            part.Buckwalter + "\t" +
                            part.Tag + "\t" +
                            GrammarDictionary.Arabic(part.Grammar.Position) + " " + GrammarDictionary.Arabic(part.Grammar.Attribute) +
                            ((part.Grammar.Position == "V") ? (" " + GrammarDictionary.Arabic(part.Grammar.Qualifier)) : "") + "\t" +
                            GrammarDictionary.English(part.Grammar.Position) + " " + GrammarDictionary.English(part.Grammar.Attribute) +
                            ((part.Grammar.Position == "V") ? (" " + GrammarDictionary.English(part.Grammar.Qualifier)) : "") + "\t" +
                            part.Grammar.ToTable() + "\t" +
                            part.Word.Address
                        );
                    }
                }
            }
        }
        return str.ToString();
    }
    private static string DoLetterFrequencySums(Client client, List<Verse> verses, string phrase)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses.Count > 0)
        {
            str.AppendLine("Phrase" + "\t" + phrase);
            str.AppendLine("Verse" + "\t" + "LFSum" + "\t" + "Address" + "\t" + "Text");

            Dictionary<Verse, int> letter_frequency_sums = new Dictionary<Verse, int>();
            if (letter_frequency_sums != null)
            {
                CalculateLetterFrequencySums(client, verses, ref letter_frequency_sums, phrase, client.Book.WithDiacritics);
                if (letter_frequency_sums != null)
                {
                    foreach (Verse verse in letter_frequency_sums.Keys)
                    {
                        str.AppendLine
                        (
                            verse.Number + "\t" + letter_frequency_sums[verse] + "\t" + verse.Address + "\t" + verse.Text
                        );
                    }
                }
            }
        }
        return str.ToString();
    }
    private static void CalculateLetterFrequencySums(Client client, List<Verse> verses, ref Dictionary<Verse, int> letter_frequency_sums, string phrase, bool with_diacritics)
    {
        if (client == null) return;
        if (client.NumerologySystem == null) return;
        if (verses == null) return;

        if (verses.Count > 0)
        {
            foreach (Verse verse in verses)
            {
                int lfsum = client.CalculateLetterFrequencySum(verse.Text, phrase, FrequencySearchType.DuplicateLetters, with_diacritics);
                letter_frequency_sums.Add(verse, lfsum);
            }
        }
    }
    private static string GetDuplicateChapterValues(Client client, List<Chapter> chapters)
    {
        if (client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();
        Dictionary<long, int> value_frequencies = new Dictionary<long, int>();
        Dictionary<long, string> duplicate_values = new Dictionary<long, string>();
        foreach (NumerologySystem numerology_system in client.LoadedNumerologySystems.Values)
        {
            if (numerology_system.TextMode == client.NumerologySystem.TextMode)
            {
                value_frequencies.Clear();
                duplicate_values.Clear();
                int duplicates = 0;
                foreach (Chapter chapter in chapters)
                {
                    long value = numerology_system.CalculateValue(chapter.Text);
                    if (value > 0)
                    {
                        if (!value_frequencies.ContainsKey(value))
                        {
                            value_frequencies.Add(value, 1);
                        }
                        else
                        {
                            value_frequencies[value]++;
                            duplicates++;
                        }

                        if (!duplicate_values.ContainsKey(value))
                        {
                            duplicate_values.Add(value, chapter.Name);
                        }
                        else
                        {
                            duplicate_values[value] += "\t" + chapter.Name;
                        }
                    }
                }

                if (duplicates > 0)
                {
                    str.AppendLine("--------------------------------------------------------------------------");
                    str.AppendLine(duplicates + " chapter group" + ((duplicates > 1) ? "s" : "") + " with the same value in " + numerology_system.Name.Substring(numerology_system.TextMode.Length + 1));
                    str.AppendLine("--------------------------------------------------------------------------");
                    str.AppendLine("Value" + "\t" + "Chapters");
                    str.AppendLine("--------------------------------------------------------------------------");
                    foreach (long value in duplicate_values.Keys)
                    {
                        if (duplicate_values[value].Contains("\t"))
                        {
                            str.AppendLine(value.ToString() + "\t" + duplicate_values[value]);
                        }
                    }
                    str.AppendLine("--------------------------------------------------------------------------");
                    str.AppendLine();
                }
            }
        }

        return str.ToString();
    }
    private static string GetDuplicateVerseValues(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        Dictionary<long, int> value_frequencies = new Dictionary<long, int>();
        Dictionary<long, string> duplicate_values = new Dictionary<long, string>();
        foreach (NumerologySystem numerology_system in client.LoadedNumerologySystems.Values)
        {
            if (numerology_system.TextMode == client.NumerologySystem.TextMode)
            {
                value_frequencies.Clear();
                duplicate_values.Clear();
                int duplicates = 0;
                foreach (Verse verse in verses)
                {
                    long value = numerology_system.CalculateValue(verse.Text);
                    if (value > 0)
                    {
                        if (!value_frequencies.ContainsKey(value))
                        {
                            value_frequencies.Add(value, 1);
                        }
                        else
                        {
                            value_frequencies[value]++;
                            duplicates++;
                        }

                        if (!duplicate_values.ContainsKey(value))
                        {
                            duplicate_values.Add(value, verse.Address);
                        }
                        else
                        {
                            duplicate_values[value] += "\t" + verse.Address;
                        }
                    }
                }

                if (duplicates > 0)
                {
                    str.AppendLine("--------------------------------------------------------------------------");
                    str.AppendLine(duplicates + " verse group" + ((duplicates > 1) ? "s" : "") + " with the same value in " + numerology_system.Name.Substring(numerology_system.TextMode.Length + 1));
                    str.AppendLine("--------------------------------------------------------------------------");
                    str.AppendLine("Value" + "\t" + "Verses");
                    str.AppendLine("--------------------------------------------------------------------------");
                    foreach (long value in duplicate_values.Keys)
                    {
                        if (duplicate_values[value].Contains("\t"))
                        {
                            str.AppendLine(value.ToString() + "\t" + duplicate_values[value]);
                        }
                    }
                    str.AppendLine("--------------------------------------------------------------------------");
                    str.AppendLine();
                }
            }
        }

        return str.ToString();
    }

    private static string ____________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    public static string AllahWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoAllahWords(client, verses);
        }
        return null;
    }
    public static string NonAllahWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoNonAllahWords(client, verses);
        }
        return null;
    }
    public static string RepeatedWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoRepeatedWords(client, verses);
        }
        return null;
    }
    public static string AllWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoAllWords(client, verses);
        }
        return null;
    }
    public static string WordVerbForms(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoWordVerbForms(client, verses);
        }
        return null;
    }
    public static string VerbForms(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoVerbForms(client, verses);
        }
        return null;
    }
    public static string Stopmarks(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoStopmarks(client, verses);
        }
        return null;
    }
    public static string ASCII_Text(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            StringBuilder str = new StringBuilder();
            foreach (Verse verse in verses)
            {
                str.AppendLine(verse.Text);
            }
            return str.ToString().ToBuckwalter();
        }
        return null;
    }
    private static string DoAllahWords(Client client, List<Verse> verses)
    {
        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                try
                {
                    str.AppendLine
                        (
                            "#" + "\t" +
                            "Number" + "\t" +
                            "InChap" + "\t" +
                            "Chapter" + "\t" +
                            "Verse" + "\t" +
                            "Word" + "\t" +
                            "Text" + "\t" +
                            "Order" + "\t" +
                            "Total"
                          );

                    int count = 0;
                    foreach (Verse verse in verses)
                    {
                        foreach (Word word in verse.Words)
                        {
                            // always simplify29 for Allah word comparison
                            string simplified_text = word.Text.Simplify29();

                            if (
                                (simplified_text == "الله") ||
                                (simplified_text == "ءالله") ||
                                (simplified_text == "ابالله") ||
                                (simplified_text == "اللهم") ||
                                (simplified_text == "بالله") ||
                                (simplified_text == "تالله") ||
                                (simplified_text == "فالله") ||
                                (simplified_text == "والله") ||
                                (simplified_text == "وتالله") ||
                                (simplified_text == "لله") ||
                                (simplified_text == "فلله") ||
                                (simplified_text == "ولله")
                              )
                            {
                                count++;
                                str.AppendLine(
                                    count + "\t" +
                                    word.Number + "\t" +
                                    word.NumberInChapter + "\t" +
                                    word.Verse.Chapter.SortedNumber + "\t" +
                                    word.Verse.NumberInChapter + "\t" +
                                    word.NumberInVerse + "\t" +
                                    word.Text + "\t" +
                                    word.Occurrence + "\t" +
                                    word.Frequency
                                  );
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
        return str.ToString();
    }
    private static string DoNonAllahWords(Client client, List<Verse> verses)
    {
        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                try
                {
                    str.AppendLine
                        (
                            "#" + "\t" +
                            "Number" + "\t" +
                            "InChap" + "\t" +
                            "Chapter" + "\t" +
                            "Verse" + "\t" +
                            "Word" + "\t" +
                            "Text" + "\t" +
                            "Order" + "\t" +
                            "Total"
                          );

                    int count = 0;
                    foreach (Verse verse in verses)
                    {
                        foreach (Word word in verse.Words)
                        {
                            // always simplify29 for Allah word comparison
                            string simplified_text = word.Text.Simplify29();

                            if (
                                (simplified_text == "الضلله") ||
                                (simplified_text == "الكلله") ||
                                (simplified_text == "خلله") ||
                                (simplified_text == "خللها") ||
                                (simplified_text == "خللهما") ||
                                (simplified_text == "سلله") ||
                                (simplified_text == "ضلله") ||
                                (simplified_text == "ظلله") ||
                                (simplified_text == "ظللها") ||
                                (simplified_text == "كلله") ||
                                (simplified_text == "للهدي") ||
                                (simplified_text == "وظللهم") ||
                                (simplified_text == "يضلله") ||
                                (simplified_text == "اللهب") ||
                                (simplified_text == "اللهو")
                              )
                            {
                                count++;
                                str.AppendLine(
                                    count + "\t" +
                                    word.Number + "\t" +
                                    word.NumberInChapter + "\t" +
                                    word.Verse.Chapter.SortedNumber + "\t" +
                                    word.Verse.NumberInChapter + "\t" +
                                    word.NumberInVerse + "\t" +
                                    word.Text + "\t" +
                                    word.Occurrence + "\t" +
                                    word.Frequency
                                  );
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
        return str.ToString();
    }
    private static string DoRepeatedWords(Client client, List<Verse> verses)
    {
        if (client == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                str.AppendLine
                (
                    "#" + "\t" +
                    "inQuran" + "\t" +
                    "inChap" + "\t" +
                    "Chapter" + "\t" +
                    "Verse" + "\t" +
                    "Word" + "\t" +
                    "Text" + "\t" +
                    "Order" + "\t" +
                    "Total" + "\t" +
                    "Verse"
                );

                List<Word> words = new List<Word>();
                foreach (Verse verse in verses)
                {
                    words.AddRange(verse.Words);
                }

                int count = 0;
                for (int i = 0; i < words.Count - 1; i++)
                {
                    if (words[i].Text.SimplifyTo(client.NumerologySystem.TextMode)
                 == words[i + 1].Text.SimplifyTo(client.NumerologySystem.TextMode))
                    {
                        count++;
                        str.AppendLine
                        (
                            count + "\t" +
                            words[i].Number + "\t" +
                            words[i].NumberInChapter + "\t" +
                            words[i].Verse.Chapter.SortedNumber + "\t" +
                            words[i].Verse.NumberInChapter + "\t" +
                            words[i].NumberInVerse + "\t" +
                            words[i].Text + "\t" +
                            words[i].Occurrence + "\t" +
                            words[i].Frequency + "\t" +
                            words[i].Verse.Text
                        );
                        str.AppendLine
                        (
                            count + "\t" +
                            words[i + 1].Number + "\t" +
                            words[i + 1].NumberInChapter + "\t" +
                            words[i + 1].Verse.Chapter.SortedNumber + "\t" +
                            words[i + 1].Verse.NumberInChapter + "\t" +
                            words[i + 1].NumberInVerse + "\t" +
                            words[i + 1].Text + "\t" +
                            words[i + 1].Occurrence + "\t" +
                            words[i + 1].Frequency + "\t" +
                            words[i + 1].Verse.Text
                        );
                    }
                }
            }
        }
        return str.ToString();
    }
    private static string DoAllWords(Client client, List<Verse> verses)
    {
        if (client == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                try
                {
                    str.AppendLine
                        (
                            "#" + "\t" +
                            "Number" + "\t" +
                            "InChap" + "\t" +
                            "Chapter" + "\t" +
                            "Verse" + "\t" +
                            "Word" + "\t" +
                            "Text" + "\t" +
                            "Order" + "\t" +
                            "Total"
                          );

                    int count = 0;
                    foreach (Verse verse in verses)
                    {
                        foreach (Word word in verse.Words)
                        {
                            count++;
                            str.AppendLine(
                                count + "\t" +
                                word.Number + "\t" +
                                word.NumberInChapter + "\t" +
                                word.Verse.Chapter.SortedNumber + "\t" +
                                word.Verse.NumberInChapter + "\t" +
                                word.NumberInVerse + "\t" +
                                word.Text + "\t" +
                                word.Occurrence + "\t" +
                                word.Frequency
                              );
                        }
                    }
                }
                catch
                {
                    // log exception
                }
            }
        }
        return str.ToString();
    }
    private static string DoWordVerbForms(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                try
                {
                    string word_heading = "Lemma";
                    str.AppendLine("Word" + "\t" + word_heading + "\t" + "Root" + "\t" + "Form" + "\t" + "Perfect" + "\t" + "Imperfect" + "\t" + "ActiveParticiple" + "\t" + "PassiveParticiple" + "\t" + "VerbalNoun");

                    foreach (Verse verse in verses)
                    {
                        foreach (Word word in verse.Words)
                        {
                            string word_address = word.Address;
                            string word_text = word.Text;
                            string word_lemma = word.Lemma;
                            string root = client.Book.GetBestRoot(word_text);
                            if (root != null)
                            {
                                if (root.Length == 3)
                                {
                                    char Faa = root[0];
                                    char Ain = root[1];
                                    char Laam = root[2];

                                    string form1_perfect = Faa + "َ" + Ain + "َ" + Laam + "َ";
                                    string form1_imperfect = "يَ" + Faa + "ْ" + Ain + "َ" + Laam + "ُ";
                                    string form1_active_participle = Faa + "َ" + Ain + "ِ" + Laam + "ٌ";
                                    string form1_passive_participle = "مَ" + Faa + "ْ" + Ain + "ُ" + Laam + "ٌ";
                                    string form1_verbal_noun = Faa + "ِ" + Ain + "َ" + Laam + "ٌ";

                                    string form2_perfect = Faa + "َ" + Ain + "َّ" + Laam + "َ";
                                    string form2_imperfect = "يُ" + Faa + "َ" + Ain + "ِّ" + Laam + "ُ";
                                    string form2_active_participle = "مُ" + Faa + "َ" + Ain + "ِّ" + Laam + "ٌ";
                                    string form2_passive_participle = "مُ" + Faa + "َ" + Ain + "َّ" + Laam + "ٌ";
                                    string form2_verbal_noun = "تَ" + Faa + "ْ" + Ain + "ِ" + "ي" + Laam + "ٌ";

                                    string form3_perfect = Faa + "َ" + "ا" + Ain + "َ" + Laam + "َ";
                                    string form3_imperfect = "يُ" + Faa + "َ" + "ا" + Ain + "ِ" + Laam + "ُ";
                                    string form3_active_participle = "مُ" + Faa + "" + "ا" + Ain + "ِ" + Laam + "ٌ";
                                    string form3_passive_participle = "مُ" + Faa + "" + "ا" + Ain + "َ" + Laam + "ٌ";
                                    string form3_verbal_noun = "مُ" + Faa + "" + "ا" + Ain + "َ" + Laam + "َ" + "ة" + " / " + Faa + "ِ" + Ain + "" + "ا" + Laam + "ٌ";

                                    string form4_perfect = "اَ" + Faa + "ْ" + Ain + "َ" + Laam + "َ";
                                    string form4_imperfect = "يُ" + Faa + "ْ" + Ain + "ِ" + Laam + "ُ";
                                    string form4_active_participle = "مُ" + Faa + "ْ" + Ain + "ِ" + Laam + "ٌ";
                                    string form4_passive_participle = "مُ" + Faa + "ْ" + Ain + "َ" + Laam + "ٌ";
                                    string form4_verbal_noun = "اِ" + Faa + "ْ" + Ain + "َ" + Laam + "ٌ";

                                    string form5_perfect = "تَ" + Faa + "َ" + Ain + "َّ" + Laam + "َ";
                                    string form5_imperfect = "يَتَ" + Faa + "َ" + Ain + "ِّ" + Laam + "ُ";
                                    string form5_active_participle = "مُتَ" + Faa + "َ" + Ain + "ِّ" + Laam + "ٌ";
                                    string form5_passive_participle = "مُتَ" + Faa + "َ" + Ain + "َّ" + Laam + "ٌ";
                                    string form5_verbal_noun = "تَ" + Faa + "َ" + Ain + "ُّ" + Laam + "ٌ";

                                    string form6_perfect = "تَ" + Faa + "َ" + "ا" + Ain + "َ" + Laam + "َ";
                                    string form6_imperfect = "تَ" + Faa + "َ" + "ا" + Ain + "َ" + Laam + "ٌ";
                                    string form6_active_participle = "مُتَ" + Faa + "َ" + "ا" + Ain + "ِ" + Laam + "ٌ";
                                    string form6_passive_participle = "مُتَ" + Faa + "َ" + "ا" + Ain + "َ" + Laam + "ٌ";
                                    string form6_verbal_noun = "تَ" + Faa + "َ" + "ا" + Ain + "ُ" + Laam + "ٌ";

                                    string form7_perfect = "اِنْ" + Faa + "َ" + Ain + "َ" + Laam + "َ";
                                    string form7_imperfect = "يَنْ" + Faa + "َ" + Ain + "ِ" + Laam + "ُ";
                                    string form7_active_participle = "مُنْ" + Faa + "َ" + Ain + "ِ" + Laam + "ٌ";
                                    string form7_passive_participle = "مُنْ" + Faa + "َ" + Ain + "َ" + Laam + "ٌ";
                                    string form7_verbal_noun = "اِنْ" + Faa + "ِ" + Ain + "" + "ا" + Laam + "ٌ";

                                    string form8_perfect = "إِ" + Faa + "ْ" + "تَ" + Ain + "َ" + Laam + "َ";
                                    string form8_imperfect = "يَ" + Faa + "ْ" + "تَ" + Ain + "ِ" + Laam + "ُ";
                                    string form8_active_participle = "مُ" + Faa + "ْ" + "تَ" + Ain + "ِ" + Laam + "ٌ";
                                    string form8_passive_participle = "مُ" + Faa + "ْ" + "تَ" + Ain + "َ" + Laam + "ٌ";
                                    string form8_verbal_noun = "إ" + Faa + "ْ" + "تِ" + Ain + "َ" + Laam + "ٌ";

                                    string form9_perfect = "إِ" + Faa + "ْ" + Ain + "َ" + Laam + "َّ";
                                    string form9_imperfect = "يَ" + Faa + "ْ" + Ain + "َ" + Laam + "ُّ";
                                    string form9_active_participle = "مُ" + Faa + "ْ" + Ain + "َ" + Laam + "ٌّ";
                                    string form9_passive_participle = "";
                                    string form9_verbal_noun = "إِ" + Faa + "ْ" + Ain + "ِ" + Laam + "ا" + Laam + "ٌ";

                                    string form10_perfect = "إِسْتَ" + Faa + "ْ" + Ain + "َ" + Laam + "َ";
                                    string form10_imperfect = "يَسْتَ" + Faa + "ْ" + Ain + "ِ" + Laam + "ُ";
                                    string form10_active_participle = "مُسْتَ" + Faa + "ْ" + Ain + "ِ" + Laam + "ٌ";
                                    string form10_passive_participle = "مُسْتَ" + Faa + "ْ" + Ain + "َ" + Laam + "ٌ";
                                    string form10_verbal_noun = "اِسْتِ" + Faa + "ْ" + Ain + "" + "ا" + Laam + "ٌ";

                                    str.AppendLine(root + "\t" + "I" + "\t" + form1_perfect + "\t" + form1_imperfect + "\t" + form1_active_participle + "\t" + form1_passive_participle + "\t" + form1_verbal_noun);
                                    str.AppendLine(root + "\t" + "II" + "\t" + form2_perfect + "\t" + form2_imperfect + "\t" + form2_active_participle + "\t" + form2_passive_participle + "\t" + form2_verbal_noun);
                                    str.AppendLine(root + "\t" + "III" + "\t" + form3_perfect + "\t" + form3_imperfect + "\t" + form3_active_participle + "\t" + form3_passive_participle + "\t" + form3_verbal_noun);
                                    str.AppendLine(root + "\t" + "IV" + "\t" + form4_perfect + "\t" + form4_imperfect + "\t" + form4_active_participle + "\t" + form4_passive_participle + "\t" + form4_verbal_noun);
                                    str.AppendLine(root + "\t" + "V" + "\t" + form5_perfect + "\t" + form5_imperfect + "\t" + form5_active_participle + "\t" + form5_passive_participle + "\t" + form5_verbal_noun);
                                    str.AppendLine(root + "\t" + "VI" + "\t" + form6_perfect + "\t" + form6_imperfect + "\t" + form6_active_participle + "\t" + form6_passive_participle + "\t" + form6_verbal_noun);
                                    str.AppendLine(root + "\t" + "VII" + "\t" + form7_perfect + "\t" + form7_imperfect + "\t" + form7_active_participle + "\t" + form7_passive_participle + "\t" + form7_verbal_noun);
                                    str.AppendLine(root + "\t" + "VIII" + "\t" + form8_perfect + "\t" + form8_imperfect + "\t" + form8_active_participle + "\t" + form8_passive_participle + "\t" + form8_verbal_noun);
                                    str.AppendLine(root + "\t" + "IX" + "\t" + form9_perfect + "\t" + form9_imperfect + "\t" + form9_active_participle + "\t" + form9_passive_participle + "\t" + form9_verbal_noun);
                                    str.AppendLine(root + "\t" + "X" + "\t" + form10_perfect + "\t" + form10_imperfect + "\t" + form10_active_participle + "\t" + form10_passive_participle + "\t" + form10_verbal_noun);
                                }
                                else
                                {
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
        return str.ToString();
    }
    private static string DoVerbForms(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.Book.RootWords == null) return null;

        StringBuilder str = new StringBuilder();
        try
        {
            str.AppendLine("Root" + "\t" + "Form" + "\t" + "Perfect" + "\t" + "Imperfect" + "\t" + "ActiveParticiple" + "\t" + "PassiveParticiple" + "\t" + "VerbalNoun");
            foreach (string root in client.Book.RootWords.Keys)
            {
                if (root != null)
                {
                    if (root.Length == 3)
                    {
                        char Faa = root[0];
                        char Ain = root[1];
                        char Laam = root[2];

                        string form1_perfect = Faa + "َ" + Ain + "َ" + Laam + "َ";
                        string form1_imperfect = "يَ" + Faa + "ْ" + Ain + "َ" + Laam + "ُ";
                        string form1_active_participle = Faa + "َ" + Ain + "ِ" + Laam + "ٌ";
                        string form1_passive_participle = "مَ" + Faa + "ْ" + Ain + "ُ" + Laam + "ٌ";
                        string form1_verbal_noun = Faa + "ِ" + Ain + "َ" + Laam + "ٌ";

                        string form2_perfect = Faa + "َ" + Ain + "َّ" + Laam + "َ";
                        string form2_imperfect = "يُ" + Faa + "َ" + Ain + "ِّ" + Laam + "ُ";
                        string form2_active_participle = "مُ" + Faa + "َ" + Ain + "ِّ" + Laam + "ٌ";
                        string form2_passive_participle = "مُ" + Faa + "َ" + Ain + "َّ" + Laam + "ٌ";
                        string form2_verbal_noun = "تَ" + Faa + "ْ" + Ain + "ِ" + "ي" + Laam + "ٌ";

                        string form3_perfect = Faa + "َ" + "ا" + Ain + "َ" + Laam + "َ";
                        string form3_imperfect = "يُ" + Faa + "َ" + "ا" + Ain + "ِ" + Laam + "ُ";
                        string form3_active_participle = "مُ" + Faa + "" + "ا" + Ain + "ِ" + Laam + "ٌ";
                        string form3_passive_participle = "مُ" + Faa + "" + "ا" + Ain + "َ" + Laam + "ٌ";
                        string form3_verbal_noun = "مُ" + Faa + "" + "ا" + Ain + "َ" + Laam + "َ" + "ة" + " / " + Faa + "ِ" + Ain + "" + "ا" + Laam + "ٌ";

                        string form4_perfect = "اَ" + Faa + "ْ" + Ain + "َ" + Laam + "َ";
                        string form4_imperfect = "يُ" + Faa + "ْ" + Ain + "ِ" + Laam + "ُ";
                        string form4_active_participle = "مُ" + Faa + "ْ" + Ain + "ِ" + Laam + "ٌ";
                        string form4_passive_participle = "مُ" + Faa + "ْ" + Ain + "َ" + Laam + "ٌ";
                        string form4_verbal_noun = "اِ" + Faa + "ْ" + Ain + "َ" + Laam + "ٌ";

                        string form5_perfect = "تَ" + Faa + "َ" + Ain + "َّ" + Laam + "َ";
                        string form5_imperfect = "يَتَ" + Faa + "َ" + Ain + "ِّ" + Laam + "ُ";
                        string form5_active_participle = "مُتَ" + Faa + "َ" + Ain + "ِّ" + Laam + "ٌ";
                        string form5_passive_participle = "مُتَ" + Faa + "َ" + Ain + "َّ" + Laam + "ٌ";
                        string form5_verbal_noun = "تَ" + Faa + "َ" + Ain + "ُّ" + Laam + "ٌ";

                        string form6_perfect = "تَ" + Faa + "َ" + "ا" + Ain + "َ" + Laam + "َ";
                        string form6_imperfect = "تَ" + Faa + "َ" + "ا" + Ain + "َ" + Laam + "ٌ";
                        string form6_active_participle = "مُتَ" + Faa + "َ" + "ا" + Ain + "ِ" + Laam + "ٌ";
                        string form6_passive_participle = "مُتَ" + Faa + "َ" + "ا" + Ain + "َ" + Laam + "ٌ";
                        string form6_verbal_noun = "تَ" + Faa + "َ" + "ا" + Ain + "ُ" + Laam + "ٌ";

                        string form7_perfect = "اِنْ" + Faa + "َ" + Ain + "َ" + Laam + "َ";
                        string form7_imperfect = "يَنْ" + Faa + "َ" + Ain + "ِ" + Laam + "ُ";
                        string form7_active_participle = "مُنْ" + Faa + "َ" + Ain + "ِ" + Laam + "ٌ";
                        string form7_passive_participle = "مُنْ" + Faa + "َ" + Ain + "َ" + Laam + "ٌ";
                        string form7_verbal_noun = "اِنْ" + Faa + "ِ" + Ain + "" + "ا" + Laam + "ٌ";

                        string form8_perfect = "إِ" + Faa + "ْ" + "تَ" + Ain + "َ" + Laam + "َ";
                        string form8_imperfect = "يَ" + Faa + "ْ" + "تَ" + Ain + "ِ" + Laam + "ُ";
                        string form8_active_participle = "مُ" + Faa + "ْ" + "تَ" + Ain + "ِ" + Laam + "ٌ";
                        string form8_passive_participle = "مُ" + Faa + "ْ" + "تَ" + Ain + "َ" + Laam + "ٌ";
                        string form8_verbal_noun = "إ" + Faa + "ْ" + "تِ" + Ain + "َ" + Laam + "ٌ";

                        string form9_perfect = "إِ" + Faa + "ْ" + Ain + "َ" + Laam + "َّ";
                        string form9_imperfect = "يَ" + Faa + "ْ" + Ain + "َ" + Laam + "ُّ";
                        string form9_active_participle = "مُ" + Faa + "ْ" + Ain + "َ" + Laam + "ٌّ";
                        string form9_passive_participle = "";
                        string form9_verbal_noun = "إِ" + Faa + "ْ" + Ain + "ِ" + Laam + "ا" + Laam + "ٌ";

                        string form10_perfect = "إِسْتَ" + Faa + "ْ" + Ain + "َ" + Laam + "َ";
                        string form10_imperfect = "يَسْتَ" + Faa + "ْ" + Ain + "ِ" + Laam + "ُ";
                        string form10_active_participle = "مُسْتَ" + Faa + "ْ" + Ain + "ِ" + Laam + "ٌ";
                        string form10_passive_participle = "مُسْتَ" + Faa + "ْ" + Ain + "َ" + Laam + "ٌ";
                        string form10_verbal_noun = "اِسْتِ" + Faa + "ْ" + Ain + "" + "ا" + Laam + "ٌ";

                        str.AppendLine(root + "\t" + "I" + "\t" + form1_perfect + "\t" + form1_imperfect + "\t" + form1_active_participle + "\t" + form1_passive_participle + "\t" + form1_verbal_noun);
                        str.AppendLine(root + "\t" + "II" + "\t" + form2_perfect + "\t" + form2_imperfect + "\t" + form2_active_participle + "\t" + form2_passive_participle + "\t" + form2_verbal_noun);
                        str.AppendLine(root + "\t" + "III" + "\t" + form3_perfect + "\t" + form3_imperfect + "\t" + form3_active_participle + "\t" + form3_passive_participle + "\t" + form3_verbal_noun);
                        str.AppendLine(root + "\t" + "IV" + "\t" + form4_perfect + "\t" + form4_imperfect + "\t" + form4_active_participle + "\t" + form4_passive_participle + "\t" + form4_verbal_noun);
                        str.AppendLine(root + "\t" + "V" + "\t" + form5_perfect + "\t" + form5_imperfect + "\t" + form5_active_participle + "\t" + form5_passive_participle + "\t" + form5_verbal_noun);
                        str.AppendLine(root + "\t" + "VI" + "\t" + form6_perfect + "\t" + form6_imperfect + "\t" + form6_active_participle + "\t" + form6_passive_participle + "\t" + form6_verbal_noun);
                        str.AppendLine(root + "\t" + "VII" + "\t" + form7_perfect + "\t" + form7_imperfect + "\t" + form7_active_participle + "\t" + form7_passive_participle + "\t" + form7_verbal_noun);
                        str.AppendLine(root + "\t" + "VIII" + "\t" + form8_perfect + "\t" + form8_imperfect + "\t" + form8_active_participle + "\t" + form8_passive_participle + "\t" + form8_verbal_noun);
                        str.AppendLine(root + "\t" + "IX" + "\t" + form9_perfect + "\t" + form9_imperfect + "\t" + form9_active_participle + "\t" + form9_passive_participle + "\t" + form9_verbal_noun);
                        str.AppendLine(root + "\t" + "X" + "\t" + form10_perfect + "\t" + form10_imperfect + "\t" + form10_active_participle + "\t" + form10_passive_participle + "\t" + form10_verbal_noun);
                    }
                    else
                    {
                    }
                }
            }
        }
        catch
        {
            // log exception
        }
        return str.ToString();
    }
    private static string DoStopmarks(Client client, List<Verse> verses)
    {
        if (client == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                try
                {
                    str.AppendLine
                        (
                            "#" + "\t" +
                            "Chapter" + "\t" +
                            "Verse" + "\t" +
                            "Word" + "\t" +
                            "Stopmark"
                          );

                    int count = 0;
                    foreach (Verse verse in verses)
                    {
                        foreach (Word word in verse.Words)
                        {
                            if (word.Stopmark != Stopmark.None)
                            {
                                // skip the artificial stopmark after 112 BismAllah
                                if (word.Verse.NumberInChapter == 1)
                                {
                                    if (word.NumberInVerse == 4)
                                    {
                                        if ((word.Text.Simplify29() == "الرحيم") || (word.Text.Simplify29() == "الررحيم"))
                                        {
                                            continue;
                                        }
                                    }
                                }

                                // skip the artificial stopmark of last word in verse
                                if (word.NumberInVerse == word.Verse.Words.Count)
                                {
                                    continue;
                                }

                                // 36:52 has MustPause then ShouldStop
                                if (word.Verse.Chapter.Number == 36)
                                {
                                    if (word.Verse.NumberInChapter == 52)
                                    {
                                        count++;
                                        str.AppendLine(
                                            count + "\t" +
                                            word.Verse.Chapter.SortedNumber + "\t" +
                                            word.Verse.NumberInChapter + "\t" +
                                            word.NumberInVerse + "\t" +
                                            StopmarkHelper.GetStopmarkText(Stopmark.MustPause)
                                          );
                                    }
                                }

                                count++;
                                str.AppendLine(
                                    count + "\t" +
                                    word.Verse.Chapter.SortedNumber + "\t" +
                                    word.Verse.NumberInChapter + "\t" +
                                    word.NumberInVerse + "\t" +
                                    StopmarkHelper.GetStopmarkText(word.Stopmark)
                                  );
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
        return str.ToString();
    }

    private static string __________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    private static string nWords_kLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Word> words = new List<Word>();
            foreach (Verse verse in verses)
            {
                words.AddRange(verse.Words);
            }

            int number_of_words = 0;
            int number_of_letters = 0;
            if (!String.IsNullOrEmpty(param))
            {
                string[] parts = param.Split(',');
                if (parts.Length == 2)
                {
                    int.TryParse(parts[0], out number_of_words);
                    int.TryParse(parts[1], out number_of_letters);
                }
            }
            WordSubsetFinder subset_finder = new WordSubsetFinder(words);
            List<List<Word>> subsets = subset_finder.Find(number_of_words, number_of_letters);

            StringBuilder str = new StringBuilder();
            foreach (List<Word> subset in subsets)
            {
                foreach (Word word in subset)
                {
                    str.Append(word.Text + "\t");
                }
                if (str.Length > 1)
                {
                    str.Remove(str.Length - 1, 1);
                }
                str.AppendLine();
            }

            return str.ToString();
        }
        return null;
    }
    private static string nVerses_kWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            int number_of_verses = 0;
            int number_of_words = 0;
            if (!String.IsNullOrEmpty(param))
            {
                string[] parts = param.Split(',');
                if (parts.Length == 2)
                {
                    int.TryParse(parts[0], out number_of_verses);
                    int.TryParse(parts[1], out number_of_words);
                }
            }
            VerseSubsetFinder subset_finder = new VerseSubsetFinder(verses);
            List<List<Verse>> subsets = subset_finder.Find(number_of_verses, number_of_words);

            StringBuilder str = new StringBuilder();
            foreach (List<Verse> subset in subsets)
            {
                foreach (Verse verse in subset)
                {
                    str.Append(verse.Address + "." + verse.Words.Count + "\t");
                }
                if (str.Length > 1)
                {
                    str.Remove(str.Length - 1, 1);
                }
                str.AppendLine();
            }

            return str.ToString();
        }
        return null;
    }
    private static string nChapters_kVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Chapter> chapters = client.Book.GetChapters(verses);
            if (chapters != null)
            {
                int number_of_chapters = 0;
                int number_of_verses = 0;
                if (!String.IsNullOrEmpty(param))
                {
                    string[] parts = param.Split(',');
                    if (parts.Length == 2)
                    {
                        int.TryParse(parts[0], out number_of_chapters);
                        int.TryParse(parts[1], out number_of_verses);
                    }
                }
                ChapterSubsetFinder subset_finder = new ChapterSubsetFinder(chapters);
                List<List<Chapter>> subsets = subset_finder.Find(number_of_chapters, number_of_verses);

                StringBuilder str = new StringBuilder();
                foreach (List<Chapter> subset in subsets)
                {
                    foreach (Chapter chapter in subset)
                    {
                        str.Append(chapter.SortedNumber + "." + chapter.Verses.Count + "\t");
                    }
                    if (str.Length > 1)
                    {
                        str.Remove(str.Length - 1, 1);
                    }
                    str.AppendLine();
                }

                return str.ToString();
            }
        }
        return null;
    }

    public static string ______________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    public static string RelatedWords(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoRelatedWords(client, verses);
        }
        return null;
    }
    public static string RelatedWordsMeanings(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoRelatedWordsMeanings(client, verses);
        }
        return null;
    }
    public static string RelatedWordAddresses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoRelatedWordAddresses(client, verses);
        }
        return null;
    }
    public static string RelatedWordVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoRelatedWordVerses(client, verses);
        }
        return null;
    }
    public static string RelatedWordsVerseMeanings(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoRelatedWordsVerseMeanings(client, verses);
        }
        return null;
    }
    public static string RelatedWordVerseAddresses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoRelatedWordVerseAddresses(client, verses);
        }
        return null;
    }
    private static string DoRelatedWords(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                str.AppendLine
                (
                    "Address" + "\t" +
                    "Text" + "\t" +
                    "Transliteration" + "\t" +
                    "Meaning" + "\t" +
                    "RelatedWords"
                );

                foreach (Verse verse in verses)
                {
                    foreach (Word word in verse.Words)
                    {
                        bool backup_with_diacritics = client.Book.WithDiacritics;
                        client.Book.WithDiacritics = false;
                        List<Word> related_words = client.Book.GetRelatedWords(word);
                        client.Book.WithDiacritics = backup_with_diacritics;
                        related_words = related_words.RemoveDuplicates();

                        StringBuilder related_words_str = new StringBuilder();
                        if (related_words.Count > 0)
                        {
                            foreach (Word related_word in related_words)
                            {
                                related_words_str.Append(related_word.Text + "|");
                            }
                            related_words_str.Remove(related_words_str.Length - 1, 1);
                        }

                        str.AppendLine
                        (
                            word.Address + "\t" +
                            word.Text + "\t" +
                            word.Transliteration + "\t" +
                            word.Meaning + "\t" +
                            related_words_str
                        );
                    }
                }
            }
        }
        return str.ToString();
    }
    private static string DoRelatedWordsMeanings(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                str.AppendLine
                (
                    "Address" + "\t" +
                    "Text" + "\t" +
                    "Transliteration" + "\t" +
                    "RelatedWordsMeanings"
                );

                foreach (Verse verse in verses)
                {
                    foreach (Word word in verse.Words)
                    {
                        bool backup_with_diacritics = client.Book.WithDiacritics;
                        client.Book.WithDiacritics = false;
                        List<Word> related_words = client.Book.GetRelatedWords(word);
                        client.Book.WithDiacritics = backup_with_diacritics;
                        List<string> related_word_meanings = new List<string>();
                        foreach (Word related_word in related_words)
                        {
                            related_word_meanings.Add(related_word.Meaning);
                        }
                        related_word_meanings = related_word_meanings.RemoveDuplicates();

                        StringBuilder related_word_meanings_str = new StringBuilder();
                        if (related_word_meanings.Count > 0)
                        {
                            foreach (string related_word_meaning in related_word_meanings)
                            {
                                related_word_meanings_str.Append(related_word_meaning + "|");
                            }
                            related_word_meanings_str.Remove(related_word_meanings_str.Length - 1, 1);
                        }

                        str.AppendLine
                        (
                            word.Address + "\t" +
                            word.Text + "\t" +
                            word.Transliteration + "\t" +
                            related_word_meanings_str
                        );
                    }
                }
            }
        }
        return str.ToString();
    }
    private static string DoRelatedWordAddresses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                str.AppendLine
                (
                    "Address" + "\t" +
                    "Text" + "\t" +
                    "Transliteration" + "\t" +
                    "Meaning" + "\t" +
                    "RelatedWords"
                );

                foreach (Verse verse in verses)
                {
                    foreach (Word word in verse.Words)
                    {
                        bool backup_with_diacritics = client.Book.WithDiacritics;
                        client.Book.WithDiacritics = false;
                        List<Word> related_words = client.Book.GetRelatedWords(word);
                        client.Book.WithDiacritics = backup_with_diacritics;
                        related_words = related_words.RemoveDuplicates();

                        StringBuilder related_words_str = new StringBuilder();
                        if (related_words.Count > 0)
                        {
                            foreach (Word related_word in related_words)
                            {
                                related_words_str.Append(related_word.Address + "|");
                            }
                            related_words_str.Remove(related_words_str.Length - 1, 1);
                        }

                        str.AppendLine
                        (
                            word.Address + "\t" +
                            word.Text + "\t" +
                            word.Transliteration + "\t" +
                            word.Meaning + "\t" +
                            related_words_str
                        );
                    }
                }
            }
        }
        return str.ToString();
    }
    private static string DoRelatedWordVerses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                str.AppendLine
                (
                    "Address" + "\t" +
                    "Text" + "\t" +
                    "Transliteration" + "\t" +
                    "Meaning" + "\t" +
                    "RelatedWordVerses"
                );

                foreach (Verse verse in verses)
                {
                    foreach (Word word in verse.Words)
                    {
                        bool backup_with_diacritics = client.Book.WithDiacritics;
                        client.Book.WithDiacritics = false;
                        List<Verse> related_verses = client.Book.GetRelatedWordVerses(word);
                        client.Book.WithDiacritics = backup_with_diacritics;
                        related_verses = related_verses.RemoveDuplicates();

                        StringBuilder related_verses_str = new StringBuilder();
                        if (related_verses.Count > 0)
                        {
                            foreach (Verse related_verse in related_verses)
                            {
                                related_verses_str.Append(related_verse.Text + "\r\n\t\t\t\t");
                            }
                            related_verses_str.Remove(related_verses_str.Length - "\r\n\t\t\t\t".Length, "\r\n\t\t\t\t".Length);
                        }

                        str.AppendLine
                        (
                            word.Address + "\t" +
                            word.Text + "\t" +
                            word.Transliteration + "\t" +
                            word.Meaning + "\t" +
                            related_verses_str
                        );
                    }
                }
            }
        }
        return str.ToString();
    }
    private static string DoRelatedWordsVerseMeanings(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                str.AppendLine
                (
                    "Address" + "\t" +
                    "Text" + "\t" +
                    "Transliteration" + "\t" +
                    "Meaning" + "\t" +
                    "RelatedWordsVerseMeanings"
                );

                foreach (Verse verse in verses)
                {
                    foreach (Word word in verse.Words)
                    {
                        bool backup_with_diacritics = client.Book.WithDiacritics;
                        client.Book.WithDiacritics = false;
                        List<Verse> related_verses = client.Book.GetRelatedWordVerses(word);
                        client.Book.WithDiacritics = backup_with_diacritics;
                        related_verses = related_verses.RemoveDuplicates();

                        StringBuilder related_verse_meanings_str = new StringBuilder();
                        if (related_verses.Count > 0)
                        {
                            foreach (Verse related_verse in related_verses)
                            {
                                if (related_verse.Words.Count > 0)
                                {
                                    foreach (Word related_verse_word in related_verse.Words)
                                    {
                                        related_verse_meanings_str.Append(related_verse_word.Meaning + " ");
                                    }
                                    related_verse_meanings_str.Remove(related_verse_meanings_str.Length - 1, 1);
                                }
                                related_verse_meanings_str.Append("\r\n\t\t\t\t");
                            }
                            related_verse_meanings_str.Remove(related_verse_meanings_str.Length - "\r\n\t\t\t\t".Length, "\r\n\t\t\t\t".Length);
                        }

                        str.AppendLine
                        (
                            word.Address + "\t" +
                            word.Text + "\t" +
                            word.Transliteration + "\t" +
                            word.Meaning + "\t" +
                            related_verse_meanings_str
                        );
                    }
                }
            }
        }
        return str.ToString();
    }
    private static string DoRelatedWordVerseAddresses(Client client, List<Verse> verses)
    {
        if (client == null) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses != null)
        {
            if (verses.Count > 0)
            {
                str.AppendLine
                (
                    "Address" + "\t" +
                    "Text" + "\t" +
                    "Transliteration" + "\t" +
                    "Meaning" + "\t" +
                    "RelatedWordVerses"
                );

                foreach (Verse verse in verses)
                {
                    foreach (Word word in verse.Words)
                    {
                        bool backup_with_diacritics = client.Book.WithDiacritics;
                        client.Book.WithDiacritics = false;
                        List<Verse> related_verses = client.Book.GetRelatedWordVerses(word);
                        client.Book.WithDiacritics = backup_with_diacritics;
                        related_verses = related_verses.RemoveDuplicates();

                        StringBuilder related_verses_str = new StringBuilder();
                        if (related_verses.Count > 0)
                        {
                            foreach (Verse related_verse in related_verses)
                            {
                                related_verses_str.Append(related_verse.Address + "|");
                            }
                            related_verses_str.Remove(related_verses_str.Length - 1, 1);
                        }

                        str.AppendLine
                        (
                            word.Address + "\t" +
                            word.Text + "\t" +
                            word.Transliteration + "\t" +
                            word.Meaning + "\t" +
                            related_verses_str
                        );
                    }
                }
            }
        }
        return str.ToString();
    }

    private static string ________________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    private static string ChapterVerseWordLetterFactors(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Chapter> chapters = client.Book.GetChapters(verses);
            if (chapters != null)
            {
                return DoChapterVerseWordLetterFactors(client, chapters);
            }
        }
        return null;
    }
    private static string ChapterVerseSumFactors(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Chapter> chapters = client.Book.GetChapters(verses);
            if (chapters != null)
            {
                return DoChapterVerseSumFactors(client, chapters);
            }
        }
        return null;
    }
    private static string ChapterVerseSquaresSumFactors(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Chapter> chapters = client.Book.GetChapters(verses);
            if (chapters != null)
            {
                return DoChapterVerseSquaresSumFactors(client, chapters);
            }
        }
        return null;
    }
    private static string ChapterVerseCubesSumFactors(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Chapter> chapters = client.Book.GetChapters(verses);
            if (chapters != null)
            {
                return DoChapterVerseCubesSumFactors(client, chapters);
            }
        }
        return null;
    }
    private static string ChapterVerseWordLetterSumAZ(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Chapter> chapters = client.Book.GetChapters(verses);
            if (chapters != null)
            {
                return DoChapterVerseWordLetterSumAZ(client, chapters);
            }
        }
        return null;
    }
    private static string ChapterVerseWordLetterSumZA(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            List<Chapter> chapters = client.Book.GetChapters(verses);
            if (chapters != null)
            {
                return DoChapterVerseWordLetterSumZA(client, chapters);
            }
        }
        return null;
    }
    private static string DoChapterVerseWordLetterFactors(Client client, List<Chapter> chapters)
    {
        if (client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();

        str.AppendLine("Name" + "\t" + "Chapter" + "\t" + "Verses" + "\t" + "Words" + "\t" + "Letters" + "\t" +
                                       "CFactors" + "\t" + "VFactors" + "\t" + "WFactors" + "\t" + "LFactors" + "\t" +
                                       "CIndex" + "\t" + "VIndex" + "\t" + "WIndex" + "\t" + "LIndex"
                      );

        foreach (Chapter chapter in chapters)
        {
            str.Append(chapter.Name + "\t");
            str.Append(chapter.SortedNumber.ToString() + "\t");
            str.Append(chapter.Verses.Count.ToString() + "\t");
            str.Append(chapter.WordCount.ToString() + "\t");
            str.Append(chapter.LetterCount.ToString() + "\t");

            str.Append(Numbers.FactorizeToString(chapter.SortedNumber) + "\t");
            str.Append(Numbers.FactorizeToString(chapter.Verses.Count) + "\t");
            str.Append(Numbers.FactorizeToString(chapter.WordCount) + "\t");
            str.Append(Numbers.FactorizeToString(chapter.LetterCount) + "\t");

            int p_index = Numbers.PrimeIndexOf(chapter.SortedNumber) + 1;
            int ap_index = Numbers.AdditivePrimeIndexOf(chapter.SortedNumber) + 1;
            int xp_index = Numbers.NonAdditivePrimeIndexOf(chapter.SortedNumber) + 1;
            int c_index = Numbers.CompositeIndexOf(chapter.SortedNumber) + 1;
            int ac_index = Numbers.AdditiveCompositeIndexOf(chapter.SortedNumber) + 1;
            int xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter.SortedNumber) + 1;
            if (Numbers.IsUnit(chapter.SortedNumber))
            {
                str.Append("U1" + "\t");
            }
            else if (Numbers.IsPrime(chapter.SortedNumber))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(chapter.Verses.Count) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(chapter.Verses.Count) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(chapter.Verses.Count) + 1;
            c_index = Numbers.CompositeIndexOf(chapter.Verses.Count) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(chapter.Verses.Count) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter.Verses.Count) + 1;
            if (Numbers.IsPrime(chapter.Verses.Count))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(chapter.WordCount) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(chapter.WordCount) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(chapter.WordCount) + 1;
            c_index = Numbers.CompositeIndexOf(chapter.WordCount) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(chapter.WordCount) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter.WordCount) + 1;
            if (Numbers.IsPrime(chapter.WordCount))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(chapter.LetterCount) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(chapter.LetterCount) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(chapter.LetterCount) + 1;
            c_index = Numbers.CompositeIndexOf(chapter.LetterCount) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(chapter.LetterCount) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter.LetterCount) + 1;
            if (Numbers.IsPrime(chapter.LetterCount))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }
        }
        return str.ToString();
    }
    private static string DoChapterVerseSumFactors(Client client, List<Chapter> chapters)
    {
        if (client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();

        str.AppendLine("Name" + "\t" + "Chapter" + "\t" + "Verses" + "\t" + "Sum" + "\t" + "Factors" + "\t" + "CType" + "\t" + "VType" + "\t" + "SumType");

        foreach (Chapter chapter in chapters)
        {
            int sum = chapter.SortedNumber + chapter.Verses.Count;

            str.Append(chapter.Name + "\t");
            str.Append(chapter.SortedNumber.ToString() + "\t");
            str.Append(chapter.Verses.Count.ToString() + "\t");
            str.Append(sum.ToString() + "\t");
            str.Append(Numbers.FactorizeToString(sum) + "\t");

            int p_index = Numbers.PrimeIndexOf(chapter.SortedNumber) + 1;
            int ap_index = Numbers.AdditivePrimeIndexOf(chapter.SortedNumber) + 1;
            int xp_index = Numbers.NonAdditivePrimeIndexOf(chapter.SortedNumber) + 1;
            int c_index = Numbers.CompositeIndexOf(chapter.SortedNumber) + 1;
            int ac_index = Numbers.AdditiveCompositeIndexOf(chapter.SortedNumber) + 1;
            int xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter.SortedNumber) + 1;
            if (Numbers.IsUnit(chapter.SortedNumber))
            {
                str.Append("U1" + "\t");
            }
            else if (Numbers.IsPrime(chapter.SortedNumber))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(chapter.Verses.Count) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(chapter.Verses.Count) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(chapter.Verses.Count) + 1;
            c_index = Numbers.CompositeIndexOf(chapter.Verses.Count) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(chapter.Verses.Count) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter.Verses.Count) + 1;
            if (Numbers.IsPrime(chapter.Verses.Count))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(sum) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(sum) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(sum) + 1;
            c_index = Numbers.CompositeIndexOf(sum) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(sum) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(sum) + 1;
            if (Numbers.IsPrime(sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }
        }
        return str.ToString();
    }
    private static string DoChapterVerseSquaresSumFactors(Client client, List<Chapter> chapters)
    {
        if (client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();

        str.AppendLine("Name" + "\t" + "Chapter" + "\t" + "Verses" + "\t" + "C^2+V^2" + "\t" + "Factors" + "\t" + "CType" + "\t" + "VType" + "\t" + "SumType");

        foreach (Chapter chapter in chapters)
        {
            int sum = (chapter.SortedNumber * chapter.SortedNumber) + (chapter.Verses.Count * chapter.Verses.Count);

            str.Append(chapter.Name + "\t");
            str.Append(chapter.SortedNumber.ToString() + "\t");
            str.Append(chapter.Verses.Count.ToString() + "\t");
            str.Append(sum.ToString() + "\t");
            str.Append(Numbers.FactorizeToString(sum) + "\t");

            int p_index = Numbers.PrimeIndexOf(chapter.SortedNumber) + 1;
            int ap_index = Numbers.AdditivePrimeIndexOf(chapter.SortedNumber) + 1;
            int xp_index = Numbers.NonAdditivePrimeIndexOf(chapter.SortedNumber) + 1;
            int c_index = Numbers.CompositeIndexOf(chapter.SortedNumber) + 1;
            int ac_index = Numbers.AdditiveCompositeIndexOf(chapter.SortedNumber) + 1;
            int xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter.SortedNumber) + 1;
            if (Numbers.IsUnit(chapter.SortedNumber))
            {
                str.Append("U1" + "\t");
            }
            else if (Numbers.IsPrime(chapter.SortedNumber))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(chapter.Verses.Count) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(chapter.Verses.Count) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(chapter.Verses.Count) + 1;
            c_index = Numbers.CompositeIndexOf(chapter.Verses.Count) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(chapter.Verses.Count) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter.Verses.Count) + 1;
            if (Numbers.IsPrime(chapter.Verses.Count))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(sum) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(sum) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(sum) + 1;
            c_index = Numbers.CompositeIndexOf(sum) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(sum) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(sum) + 1;
            if (Numbers.IsPrime(sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }
        }
        return str.ToString();
    }
    private static string DoChapterVerseCubesSumFactors(Client client, List<Chapter> chapters)
    {
        if (client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();

        str.AppendLine("Name" + "\t" + "Chapter" + "\t" + "Verses" + "\t" + "C^3+V^3" + "\t" + "Factors" + "\t" + "CType" + "\t" + "VType" + "\t" + "SumType");

        foreach (Chapter chapter in chapters)
        {
            int sum = (chapter.SortedNumber * chapter.SortedNumber * chapter.SortedNumber) + (chapter.Verses.Count * chapter.Verses.Count * chapter.Verses.Count);

            str.Append(chapter.Name + "\t");
            str.Append(chapter.SortedNumber.ToString() + "\t");
            str.Append(chapter.Verses.Count.ToString() + "\t");
            str.Append(sum.ToString() + "\t");
            str.Append(Numbers.FactorizeToString(sum) + "\t");

            int p_index = Numbers.PrimeIndexOf(chapter.SortedNumber) + 1;
            int ap_index = Numbers.AdditivePrimeIndexOf(chapter.SortedNumber) + 1;
            int xp_index = Numbers.NonAdditivePrimeIndexOf(chapter.SortedNumber) + 1;
            int c_index = Numbers.CompositeIndexOf(chapter.SortedNumber) + 1;
            int ac_index = Numbers.AdditiveCompositeIndexOf(chapter.SortedNumber) + 1;
            int xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter.SortedNumber) + 1;
            if (Numbers.IsUnit(chapter.SortedNumber))
            {
                str.Append("U1" + "\t");
            }
            else if (Numbers.IsPrime(chapter.SortedNumber))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(chapter.Verses.Count) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(chapter.Verses.Count) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(chapter.Verses.Count) + 1;
            c_index = Numbers.CompositeIndexOf(chapter.Verses.Count) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(chapter.Verses.Count) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(chapter.Verses.Count) + 1;
            if (Numbers.IsPrime(chapter.Verses.Count))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(sum) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(sum) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(sum) + 1;
            c_index = Numbers.CompositeIndexOf(sum) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(sum) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(sum) + 1;
            if (Numbers.IsPrime(sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }
        }
        return str.ToString();
    }
    private static string DoChapterVerseWordLetterSumAZ(Client client, List<Chapter> chapters)
    {
        if (client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();

        str.AppendLine("Name" + "\t" + "Chapter" + "\t" + "Verses" + "\t" + "Words" + "\t" + "Letters" + "\t" +
                                       "CFactors" + "\t" + "VFactors" + "\t" + "WFactors" + "\t" + "LFactors" + "\t" +
                                       "CIndex" + "\t" + "VIndex" + "\t" + "WIndex" + "\t" + "LIndex" + "\t"
                      );

        long c_sum = 0L;
        long v_sum = 0L;
        long w_sum = 0L;
        long l_sum = 0L;
        foreach (Chapter chapter in chapters)
        {
            str.Append(chapter.Name + "\t");

            c_sum += chapter.SortedNumber;
            v_sum += chapter.Verses.Count;
            w_sum += chapter.WordCount;
            l_sum += chapter.LetterCount;

            str.Append(c_sum.ToString() + "\t");
            str.Append(v_sum.ToString() + "\t");
            str.Append(w_sum.ToString() + "\t");
            str.Append(l_sum.ToString() + "\t");

            str.Append(Numbers.FactorizeToString(c_sum) + "\t");
            str.Append(Numbers.FactorizeToString(v_sum) + "\t");
            str.Append(Numbers.FactorizeToString(w_sum) + "\t");
            str.Append(Numbers.FactorizeToString(l_sum) + "\t");

            int p_index = Numbers.PrimeIndexOf(c_sum) + 1;
            int ap_index = Numbers.AdditivePrimeIndexOf(c_sum) + 1;
            int xp_index = Numbers.NonAdditivePrimeIndexOf(c_sum) + 1;
            int c_index = Numbers.CompositeIndexOf(c_sum) + 1;
            int ac_index = Numbers.AdditiveCompositeIndexOf(c_sum) + 1;
            int xc_index = Numbers.NonAdditiveCompositeIndexOf(c_sum) + 1;
            if (Numbers.IsUnit(c_sum))
            {
                str.Append("U1" + "\t");
            }
            else if (Numbers.IsPrime(c_sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(v_sum) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(v_sum) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(v_sum) + 1;
            c_index = Numbers.CompositeIndexOf(v_sum) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(v_sum) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(v_sum) + 1;
            if (Numbers.IsPrime(v_sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(w_sum) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(w_sum) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(w_sum) + 1;
            c_index = Numbers.CompositeIndexOf(w_sum) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(w_sum) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(w_sum) + 1;
            if (Numbers.IsPrime(w_sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(l_sum) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(l_sum) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(l_sum) + 1;
            c_index = Numbers.CompositeIndexOf(l_sum) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(l_sum) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(l_sum) + 1;
            if (Numbers.IsPrime(l_sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }
        }
        return str.ToString();
    }
    private static string DoChapterVerseWordLetterSumZA(Client client, List<Chapter> chapters)
    {
        if (client == null) return null;
        if (chapters == null) return null;

        StringBuilder str = new StringBuilder();

        str.AppendLine("Name" + "\t" + "Chapter" + "\t" + "Verses" + "\t" + "Words" + "\t" + "Letters" + "\t" +
                                       "CFactors" + "\t" + "VFactors" + "\t" + "WFactors" + "\t" + "LFactors" + "\t" +
                                       "CIndex" + "\t" + "VIndex" + "\t" + "WIndex" + "\t" + "LIndex" + "\t"
                      );

        long c_sum = 0L;
        long v_sum = 0L;
        long w_sum = 0L;
        long l_sum = 0L;
        for (int i = chapters.Count - 1; i >= 0; i--)
        {
            str.Append(chapters[i].Name + "\t");

            c_sum += chapters[i].SortedNumber;
            v_sum += chapters[i].Verses.Count;
            w_sum += chapters[i].WordCount;
            l_sum += chapters[i].LetterCount;

            str.Append(c_sum.ToString() + "\t");
            str.Append(v_sum.ToString() + "\t");
            str.Append(w_sum.ToString() + "\t");
            str.Append(l_sum.ToString() + "\t");

            str.Append(Numbers.FactorizeToString(c_sum) + "\t");
            str.Append(Numbers.FactorizeToString(v_sum) + "\t");
            str.Append(Numbers.FactorizeToString(w_sum) + "\t");
            str.Append(Numbers.FactorizeToString(l_sum) + "\t");

            int p_index = Numbers.PrimeIndexOf(c_sum) + 1;
            int ap_index = Numbers.AdditivePrimeIndexOf(c_sum) + 1;
            int xp_index = Numbers.NonAdditivePrimeIndexOf(c_sum) + 1;
            int c_index = Numbers.CompositeIndexOf(c_sum) + 1;
            int ac_index = Numbers.AdditiveCompositeIndexOf(c_sum) + 1;
            int xc_index = Numbers.NonAdditiveCompositeIndexOf(c_sum) + 1;
            if (Numbers.IsUnit(c_sum))
            {
                str.Append("U1" + "\t");
            }
            else if (Numbers.IsPrime(c_sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(v_sum) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(v_sum) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(v_sum) + 1;
            c_index = Numbers.CompositeIndexOf(v_sum) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(v_sum) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(v_sum) + 1;
            if (Numbers.IsPrime(v_sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(w_sum) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(w_sum) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(w_sum) + 1;
            c_index = Numbers.CompositeIndexOf(w_sum) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(w_sum) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(w_sum) + 1;
            if (Numbers.IsPrime(w_sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }

            p_index = Numbers.PrimeIndexOf(l_sum) + 1;
            ap_index = Numbers.AdditivePrimeIndexOf(l_sum) + 1;
            xp_index = Numbers.NonAdditivePrimeIndexOf(l_sum) + 1;
            c_index = Numbers.CompositeIndexOf(l_sum) + 1;
            ac_index = Numbers.AdditiveCompositeIndexOf(l_sum) + 1;
            xc_index = Numbers.NonAdditiveCompositeIndexOf(l_sum) + 1;
            if (Numbers.IsPrime(l_sum))
            {
                str.Append("P" + p_index + " " + ((ap_index > 0) ? ("AP" + ap_index) : "") + "\t" + ((xp_index > 0) ? ("XP" + xp_index) : "") + "\t");
            }
            else
            {
                str.Append("C" + c_index + " " + ((ac_index > 0) ? ("AC" + ac_index) : "") + "\t" + ((xc_index > 0) ? ("XC" + xc_index) : "") + "\t");
            }
        }
        return str.ToString();
    }

    public static string _____________________________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    public static string FirstWordFrequencyEqualsVerseNumber(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoFirstWordFrequencyEqualsVerseNumber(client, verses, param);
        }
        return null;
    }
    public static string LastWordFrequencyEqualsVerseNumber(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoLastWordFrequencyEqualsVerseNumber(client, verses, param);
        }
        return null;
    }
    private static string DoFirstWordFrequencyEqualsVerseNumber(Client client, List<Verse> verses, string param)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses.Count > 0)
        {
            str.AppendLine
            (
                "#" + "\t" +
                "Word" + "\t" +
                "Text" + "\t" +
                "Frequency"
            );

            int count = 0;
            foreach (Verse verse in verses)
            {
                Word word = verse.Words[0];
                if (word != null)
                {
                    int frequency = 0;
                    foreach (Verse v in verses)
                    {
                        foreach (Word w in v.Words)
                        {
                            if (word.Text == w.Text)
                            {
                                frequency++;
                            }
                        }
                    }

                    if (frequency == verse.NumberInChapter)
                    {
                        count++;
                        str.AppendLine
                        (
                            count.ToString() + "\t" +
                            word.Address + "\t" +
                            word.Text + "\t" +
                            frequency.ToString()
                        );
                    }
                }
            }
        }
        return str.ToString();
    }
    private static string DoLastWordFrequencyEqualsVerseNumber(Client client, List<Verse> verses, string param)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        if (verses.Count > 0)
        {
            str.AppendLine
            (
                "#" + "\t" +
                "Word" + "\t" +
                "Text" + "\t" +
                "Frequency"
            );

            int count = 0;
            foreach (Verse verse in verses)
            {
                Word word = verse.Words[verse.Words.Count - 1];
                if (word != null)
                {
                    int frequency = 0;
                    foreach (Verse v in verses)
                    {
                        foreach (Word w in v.Words)
                        {
                            if (word.Text == w.Text)
                            {
                                frequency++;
                            }
                        }
                    }

                    if (frequency == verse.NumberInChapter)
                    {
                        count++;
                        str.AppendLine
                        (
                            count.ToString() + "\t" +
                            word.Address + "\t" +
                            word.Text + "\t" +
                            frequency.ToString()
                        );
                    }
                }
            }
        }
        return str.ToString();
    }

    public static string __________________________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    public static string P_PivotConsecutiveVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoP_PivotConsecutiveVerses(client, verses, param);
        }
        return null;
    }
    private static string AP_PivotConsecutiveVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoAP_PivotConsecutiveVerses(client, verses, param);
        }
        return null;
    }
    private static string XP_PivotConsecutiveVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoXP_PivotConsecutiveVerses(client, verses, param);
        }
        return null;
    }
    public static string C_PivotConsecutiveVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoC_PivotConsecutiveVerses(client, verses, param);
        }
        return null;
    }
    private static string AC_PivotConsecutiveVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoAC_PivotConsecutiveVerses(client, verses, param);
        }
        return null;
    }
    private static string XC_PivotConsecutiveVerses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        List<Verse> verses = GetSourceVerses(client, in_search_result);
        if (verses != null)
        {
            return DoXC_PivotConsecutiveVerses(client, verses, param);
        }
        return null;
    }
    private static string DoP_PivotConsecutiveVerses(Client client, List<Verse> verses, string param)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "N" + "\t" + "M*N" + "\t" + "P" + "\t" + "MN+P" + "\r\n");

        int pivot = 0;
        if (param.Length > 0)
        {
            int min = 16;
            int max = 16;

            if (int.TryParse(param, out pivot))
            {
                if (pivot == 0)
                {
                    min = 1;
                    max = 114;
                }
                else
                {
                    min = pivot;
                    max = pivot;
                }
            }
            else
            {
                string[] parts = param.Split('-');
                if (parts.Length == 2)
                {
                    int.TryParse(parts[0], out min);
                    int.TryParse(parts[1], out max);
                }
            }

            for (int i = min; i <= max; i++)
            {
                int M = 1;
                int N = (i * 2) - 1;

                do
                {
                    long MN = M * N;
                    long P = Numbers.Primes[M - 1];
                    long MN_P = MN + P;

                    string text1 = GetConsecutiveVerses(client, MN, (M - 1) * (N + 1), false);
                    string text2 = GetConsecutiveVerses(client, MN, MN_P, true);
                    string text3 = GetConsecutiveVerses(client, MN, (M + 1) * (N - 1), false);
                    string text4 = "";
                    if ((Numbers.Primes.Count >= 0) && (Numbers.Primes.Count < (M - 2)))
                    {
                        text4 = GetConsecutiveVerses(client, MN_P, ((M - 1) * (N + 1)) + Numbers.Primes[M - 2], false);
                    }
                    string text5 = "";
                    if ((Numbers.Primes.Count >= 0) && (Numbers.Primes.Count < M))
                    {
                        text5 = GetConsecutiveVerses(client, MN_P, ((M + 1) * (N - 1)) + Numbers.Primes[M], false);
                    }

                    string text = "";
                    if (!text.Contains(text1)) text += text1;
                    if (!text.Contains(text2)) text += text2;
                    if (!text.Contains(text3)) text += text3;
                    if (!text.Contains(text4)) text += text4;
                    if (!text.Contains(text5)) text += text5;

                    str.Append(M + "\t" + N + "\t" + MN + "\t" + P + "\t" + MN_P + "\t" + text + "\r\n");

                    M++;
                    N--;
                } while (N > 0);

                str.AppendLine();
                str.AppendLine();
            }
        }
        return str.ToString();
    }
    private static string DoAP_PivotConsecutiveVerses(Client client, List<Verse> verses, string param)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "N" + "\t" + "M*N" + "\t" + "AP" + "\t" + "MN+AP" + "\r\n");

        int pivot = 0;
        if (param.Length > 0)
        {
            int min = 16;
            int max = 16;

            if (int.TryParse(param, out pivot))
            {
                if (pivot == 0)
                {
                    min = 1;
                    max = 114;
                }
                else
                {
                    min = pivot;
                    max = pivot;
                }
            }
            else
            {
                string[] parts = param.Split('-');
                if (parts.Length == 2)
                {
                    int.TryParse(parts[0], out min);
                    int.TryParse(parts[1], out max);
                }
            }

            for (int i = min; i <= max; i++)
            {
                int M = 1;
                int N = (i * 2) - 1;

                do
                {
                    long MN = M * N;
                    long AP = Numbers.AdditivePrimes[M - 1];
                    long MN_AP = MN + AP;

                    string text1 = GetConsecutiveVerses(client, MN, (M - 1) * (N + 1), false);
                    string text2 = GetConsecutiveVerses(client, MN, MN_AP, true);
                    string text3 = GetConsecutiveVerses(client, MN, (M + 1) * (N - 1), false);
                    string text4 = "";
                    if ((Numbers.AdditivePrimes.Count >= 0) && (Numbers.AdditivePrimes.Count < (M - 2)))
                    {
                        text4 = GetConsecutiveVerses(client, MN_AP, ((M - 1) * (N + 1)) + Numbers.AdditivePrimes[M - 2], false);
                    }
                    string text5 = "";
                    if ((Numbers.AdditivePrimes.Count >= 0) && (Numbers.AdditivePrimes.Count < M))
                    {
                        text5 = GetConsecutiveVerses(client, MN_AP, ((M + 1) * (N - 1)) + Numbers.AdditivePrimes[M], false);
                    }

                    string text = "";
                    if (!text.Contains(text1)) text += text1;
                    if (!text.Contains(text2)) text += text2;
                    if (!text.Contains(text3)) text += text3;
                    if (!text.Contains(text4)) text += text4;
                    if (!text.Contains(text5)) text += text5;

                    str.Append(M + "\t" + N + "\t" + MN + "\t" + AP + "\t" + MN_AP + "\t" + text + "\r\n");

                    M++;
                    N--;
                } while (N > 0);

                str.AppendLine();
                str.AppendLine();
            }
        }
        return str.ToString();
    }
    private static string DoXP_PivotConsecutiveVerses(Client client, List<Verse> verses, string param)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "N" + "\t" + "M*N" + "\t" + "XP" + "\t" + "MN+XP" + "\r\n");

        int pivot = 0;
        if (param.Length > 0)
        {
            int min = 16;
            int max = 16;

            if (int.TryParse(param, out pivot))
            {
                if (pivot == 0)
                {
                    min = 1;
                    max = 114;
                }
                else
                {
                    min = pivot;
                    max = pivot;
                }
            }
            else
            {
                string[] parts = param.Split('-');
                if (parts.Length == 2)
                {
                    int.TryParse(parts[0], out min);
                    int.TryParse(parts[1], out max);
                }
            }

            for (int i = min; i <= max; i++)
            {
                int M = 1;
                int N = (i * 2) - 1;

                do
                {
                    long MN = M * N;
                    long XP = Numbers.NonAdditivePrimes[M - 1];
                    long MN_XP = MN + XP;

                    string text1 = GetConsecutiveVerses(client, MN, (M - 1) * (N + 1), false);
                    string text2 = GetConsecutiveVerses(client, MN, MN_XP, true);
                    string text3 = GetConsecutiveVerses(client, MN, (M + 1) * (N - 1), false);
                    string text4 = "";
                    if ((Numbers.NonAdditivePrimes.Count >= 0) && (Numbers.NonAdditivePrimes.Count < (M - 2)))
                    {
                        text4 = GetConsecutiveVerses(client, MN_XP, ((M - 1) * (N + 1)) + Numbers.NonAdditivePrimes[M - 2], false);
                    }
                    string text5 = "";
                    if ((Numbers.NonAdditivePrimes.Count >= 0) && (Numbers.NonAdditivePrimes.Count < M))
                    {
                        text5 = GetConsecutiveVerses(client, MN_XP, ((M + 1) * (N - 1)) + Numbers.NonAdditivePrimes[M], false);
                    }

                    string text = "";
                    if (!text.Contains(text1)) text += text1;
                    if (!text.Contains(text2)) text += text2;
                    if (!text.Contains(text3)) text += text3;
                    if (!text.Contains(text4)) text += text4;
                    if (!text.Contains(text5)) text += text5;

                    str.Append(M + "\t" + N + "\t" + MN + "\t" + XP + "\t" + MN_XP + "\t" + text + "\r\n");

                    M++;
                    N--;
                } while (N > 0);

                str.AppendLine();
                str.AppendLine();
            }
        }
        return str.ToString();
    }
    private static string DoC_PivotConsecutiveVerses(Client client, List<Verse> verses, string param)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "N" + "\t" + "M*N" + "\t" + "C" + "\t" + "MN+C" + "\r\n");

        int pivot = 0;
        if (param.Length > 0)
        {
            int min = 16;
            int max = 16;

            if (int.TryParse(param, out pivot))
            {
                if (pivot == 0)
                {
                    min = 1;
                    max = 114;
                }
                else
                {
                    min = pivot;
                    max = pivot;
                }
            }
            else
            {
                string[] parts = param.Split('-');
                if (parts.Length == 2)
                {
                    int.TryParse(parts[0], out min);
                    int.TryParse(parts[1], out max);
                }
            }

            for (int i = min; i <= max; i++)
            {
                int M = 1;
                int N = (i * 2) - 1;

                do
                {
                    long MN = M * N;
                    long C = Numbers.Composites[M - 1];
                    long MN_C = MN + C;

                    string text1 = GetConsecutiveVerses(client, MN, (M - 1) * (N + 1), false);
                    string text2 = GetConsecutiveVerses(client, MN, MN_C, true);
                    string text3 = GetConsecutiveVerses(client, MN, (M + 1) * (N - 1), false);
                    string text4 = "";
                    if ((Numbers.Composites.Count >= 0) && (Numbers.Composites.Count < (M - 2)))
                    {
                        text4 = GetConsecutiveVerses(client, MN_C, ((M - 1) * (N + 1)) + Numbers.Composites[M - 2], false);
                    }
                    string text5 = "";
                    if ((Numbers.Composites.Count >= 0) && (Numbers.Composites.Count < M))
                    {
                        text5 = GetConsecutiveVerses(client, MN_C, ((M + 1) * (N - 1)) + Numbers.Composites[M], false);
                    }

                    string text = "";
                    if (!text.Contains(text1)) text += text1;
                    if (!text.Contains(text2)) text += text2;
                    if (!text.Contains(text3)) text += text3;
                    if (!text.Contains(text4)) text += text4;
                    if (!text.Contains(text5)) text += text5;

                    str.Append(M + "\t" + N + "\t" + MN + "\t" + C + "\t" + MN_C + "\t" + text + "\r\n");

                    M++;
                    N--;
                } while (N > 0);

                str.AppendLine();
                str.AppendLine();
            }
        }
        return str.ToString();
    }
    private static string DoAC_PivotConsecutiveVerses(Client client, List<Verse> verses, string param)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "N" + "\t" + "M*N" + "\t" + "AC" + "\t" + "MN+AC" + "\r\n");

        int pivot = 0;
        if (param.Length > 0)
        {
            int min = 16;
            int max = 16;

            if (int.TryParse(param, out pivot))
            {
                if (pivot == 0)
                {
                    min = 1;
                    max = 114;
                }
                else
                {
                    min = pivot;
                    max = pivot;
                }
            }
            else
            {
                string[] parts = param.Split('-');
                if (parts.Length == 2)
                {
                    int.TryParse(parts[0], out min);
                    int.TryParse(parts[1], out max);
                }
            }

            for (int i = min; i <= max; i++)
            {
                int M = 1;
                int N = (i * 2) - 1;

                do
                {
                    long MN = M * N;
                    long AC = Numbers.AdditiveComposites[M - 1];
                    long MN_AC = MN + AC;

                    string text1 = GetConsecutiveVerses(client, MN, (M - 1) * (N + 1), false);
                    string text2 = GetConsecutiveVerses(client, MN, MN_AC, true);
                    string text3 = GetConsecutiveVerses(client, MN, (M + 1) * (N - 1), false);
                    string text4 = "";
                    if ((Numbers.AdditiveComposites.Count >= 0) && (Numbers.AdditiveComposites.Count < (M - 2)))
                    {
                        text4 = GetConsecutiveVerses(client, MN_AC, ((M - 1) * (N + 1)) + Numbers.AdditiveComposites[M - 2], false);
                    }
                    string text5 = "";
                    if ((Numbers.AdditiveComposites.Count >= 0) && (Numbers.AdditiveComposites.Count < M))
                    {
                        text5 = GetConsecutiveVerses(client, MN_AC, ((M + 1) * (N - 1)) + Numbers.AdditiveComposites[M], false);
                    }

                    string text = "";
                    if (!text.Contains(text1)) text += text1;
                    if (!text.Contains(text2)) text += text2;
                    if (!text.Contains(text3)) text += text3;
                    if (!text.Contains(text4)) text += text4;
                    if (!text.Contains(text5)) text += text5;

                    str.Append(M + "\t" + N + "\t" + MN + "\t" + AC + "\t" + MN_AC + "\t" + text + "\r\n");

                    M++;
                    N--;
                } while (N > 0);

                str.AppendLine();
                str.AppendLine();
            }
        }
        return str.ToString();
    }
    private static string DoXC_PivotConsecutiveVerses(Client client, List<Verse> verses, string param)
    {
        if (client == null) return null;
        if (verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "N" + "\t" + "M*N" + "\t" + "XC" + "\t" + "MN+XC" + "\r\n");

        int pivot = 0;
        if (param.Length > 0)
        {
            int min = 16;
            int max = 16;

            if (int.TryParse(param, out pivot))
            {
                if (pivot == 0)
                {
                    min = 1;
                    max = 114;
                }
                else
                {
                    min = pivot;
                    max = pivot;
                }
            }
            else
            {
                string[] parts = param.Split('-');
                if (parts.Length == 2)
                {
                    int.TryParse(parts[0], out min);
                    int.TryParse(parts[1], out max);
                }
            }

            for (int i = min; i <= max; i++)
            {
                int M = 1;
                int N = (i * 2) - 1;

                do
                {
                    long MN = M * N;
                    long XC = Numbers.NonAdditiveComposites[M - 1];
                    long MN_XC = MN + XC;

                    string text1 = GetConsecutiveVerses(client, MN, (M - 1) * (N + 1), false);
                    string text2 = GetConsecutiveVerses(client, MN, MN_XC, true);
                    string text3 = GetConsecutiveVerses(client, MN, (M + 1) * (N - 1), false);
                    string text4 = "";
                    if ((Numbers.NonAdditiveComposites.Count >= 0) && (Numbers.NonAdditiveComposites.Count < (M - 2)))
                    {
                        text4 = GetConsecutiveVerses(client, MN_XC, ((M - 1) * (N + 1)) + Numbers.NonAdditiveComposites[M - 2], false);
                    }
                    string text5 = "";
                    if ((Numbers.NonAdditiveComposites.Count >= 0) && (Numbers.NonAdditiveComposites.Count < M))
                    {
                        text5 = GetConsecutiveVerses(client, MN_XC, ((M + 1) * (N - 1)) + Numbers.NonAdditiveComposites[M], false);
                    }

                    string text = "";
                    if (!text.Contains(text1)) text += text1;
                    if (!text.Contains(text2)) text += text2;
                    if (!text.Contains(text3)) text += text3;
                    if (!text.Contains(text4)) text += text4;
                    if (!text.Contains(text5)) text += text5;

                    str.Append(M + "\t" + N + "\t" + MN + "\t" + XC + "\t" + MN_XC + "\t" + text + "\r\n");

                    M++;
                    N--;
                } while (N > 0);

                str.AppendLine();
                str.AppendLine();
            }
        }
        return str.ToString();
    }
    private static string GetConsecutiveVerses(Client client, long value1, long value2, bool display_both)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Verses == null) return null;

        string result = "";
        if ((value1 > 0) && (value2 > 0))
        {
            List<Verse> found_verses1 = new List<Verse>();
            List<Verse> found_verses2 = new List<Verse>();
            foreach (Verse verse in client.Book.Verses)
            {
                long value = client.CalculateValue(verse);
                if (value1 == value)
                {
                    found_verses1.Add(verse);
                }
                if (value2 == value)
                {
                    found_verses2.Add(verse);
                }
            }

            foreach (Verse verse1 in found_verses1)
            {
                foreach (Verse verse2 in found_verses2)
                {
                    if (Math.Abs(verse2.Number - verse1.Number) == 1)
                    {
                        if (display_both)
                        {
                            result += verse1.Text + "\t" + verse2.Text + "\t";
                        }
                        else
                        {
                            result += verse1.Text + "\t";
                        }
                    }
                }
            }
        }
        return result;
    }
    public static string _________________________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    public static string N_P_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "P" + "\t" + "N:P" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int P_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int P = (int)Numbers.Primes[i];
            N_total += N;
            P_total += P;

            string N_P = N + ":" + P;
            if ((N >= 0) && (N < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[N - 1];
                if (chapter != null)
                {
                    if ((P >= 0) && (P < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[P - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + P + "\t" + N_P + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + P + "\t" + N_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + P + "\t" + N_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + P + "\t" + N_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + P_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string N_AP_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "AP" + "\t" + "N:AP" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int AP_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int AP = (int)Numbers.AdditivePrimes[i];
            N_total += N;
            AP_total += AP;

            string N_AP = N + ":" + AP;
            if ((N >= 0) && (N < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[N - 1];
                if (chapter != null)
                {
                    if ((AP >= 0) && (AP < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[AP - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + AP + "\t" + N_AP + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + AP + "\t" + N_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + AP + "\t" + N_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + AP + "\t" + N_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + AP_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string N_XP_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "XP" + "\t" + "N:XP" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int XP_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int XP = (int)Numbers.AdditivePrimes[i];
            N_total += N;
            XP_total += XP;

            string N_XP = N + ":" + XP;
            if ((N >= 0) && (N < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[N - 1];
                if (chapter != null)
                {
                    if ((XP >= 0) && (XP < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[XP - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + XP + "\t" + N_XP + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + XP + "\t" + N_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + XP + "\t" + N_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + XP + "\t" + N_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + XP_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    public static string N_C_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "C" + "\t" + "N:C" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int C_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int C = (int)Numbers.Composites[i];
            N_total += N;
            C_total += C;

            string N_C = N + ":" + C;
            if ((N >= 0) && (N < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[N - 1];
                if (chapter != null)
                {
                    if ((C >= 0) && (C < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[C - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + C + "\t" + N_C + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + C + "\t" + N_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + C + "\t" + N_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + C + "\t" + N_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + C_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string N_AC_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "AC" + "\t" + "N:AC" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int AC_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int AC = (int)Numbers.AdditiveComposites[i];
            N_total += N;
            AC_total += AC;

            string N_AC = N + ":" + AC;
            if ((N >= 0) && (N < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[N - 1];
                if (chapter != null)
                {
                    if ((AC >= 0) && (AC < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[AC - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + AC + "\t" + N_AC + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + AC + "\t" + N_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + AC + "\t" + N_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + AC + "\t" + N_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + AC_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string N_XC_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "XC" + "\t" + "N:XC" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int XC_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int XC = (int)Numbers.AdditiveComposites[i];
            N_total += N;
            XC_total += XC;

            string N_XC = N + ":" + XC;
            if ((N >= 0) && (N < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[N - 1];
                if (chapter != null)
                {
                    if ((XC >= 0) && (XC < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[XC - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + XC + "\t" + N_XC + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + XC + "\t" + N_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + XC + "\t" + N_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + XC + "\t" + N_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + XC_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    public static string rN_P_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "rN" + "\t" + "P" + "\t" + "rN:P" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int rN_total = 0;
        int P_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int rN = 31 - i;
            int P = (int)Numbers.Primes[i];
            N_total += N;
            rN_total += rN;
            P_total += P;

            string rN_P = rN + ":" + P;
            if ((rN >= 0) && (rN < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rN - 1];
                if (chapter != null)
                {
                    if ((P >= 0) && (P < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[P - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + rN + "\t" + P + "\t" + rN_P + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + rN + "\t" + P + "\t" + rN_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + rN + "\t" + P + "\t" + rN_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + rN + "\t" + P + "\t" + rN_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + rN_total + "\t" + P_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string rN_AP_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "rN" + "\t" + "AP" + "\t" + "rN:AP" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int rN_total = 0;
        int AP_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int rN = 31 - i;
            int AP = (int)Numbers.AdditivePrimes[i];
            N_total += N;
            rN_total += rN;
            AP_total += AP;

            string rN_AP = rN + ":" + AP;
            if ((rN >= 0) && (rN < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rN - 1];
                if (chapter != null)
                {
                    if ((AP >= 0) && (AP < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[AP - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + rN + "\t" + AP + "\t" + rN_AP + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + rN + "\t" + AP + "\t" + rN_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + rN + "\t" + AP + "\t" + rN_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + rN + "\t" + AP + "\t" + rN_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + rN_total + "\t" + AP_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string rN_XP_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "rN" + "\t" + "XP" + "\t" + "rN:XP" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int rN_total = 0;
        int XP_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int rN = 31 - i;
            int XP = (int)Numbers.AdditivePrimes[i];
            N_total += N;
            rN_total += rN;
            XP_total += XP;

            string rN_XP = rN + ":" + XP;
            if ((rN >= 0) && (rN < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rN - 1];
                if (chapter != null)
                {
                    if ((XP >= 0) && (XP < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[XP - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + rN + "\t" + XP + "\t" + rN_XP + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + rN + "\t" + XP + "\t" + rN_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + rN + "\t" + XP + "\t" + rN_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + rN + "\t" + XP + "\t" + rN_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + rN_total + "\t" + XP_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    public static string rN_C_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "rN" + "\t" + "C" + "\t" + "rN:C" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int rN_total = 0;
        int C_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int rN = 31 - i;
            int C = (int)Numbers.Composites[i];
            N_total += N;
            rN_total += rN;
            C_total += C;

            string rN_C = rN + ":" + C;
            if ((rN >= 0) && (rN < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rN - 1];
                if (chapter != null)
                {
                    if ((C >= 0) && (C < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[C - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + rN + "\t" + C + "\t" + rN_C + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + rN + "\t" + C + "\t" + rN_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + rN + "\t" + C + "\t" + rN_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + rN + "\t" + C + "\t" + rN_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + rN_total + "\t" + C_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string rN_AC_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "rN" + "\t" + "AC" + "\t" + "rN:AC" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int rN_total = 0;
        int AC_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int rN = 31 - i;
            int AC = (int)Numbers.AdditiveComposites[i];
            N_total += N;
            rN_total += rN;
            AC_total += AC;

            string rN_AC = rN + ":" + AC;
            if ((rN >= 0) && (rN < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rN - 1];
                if (chapter != null)
                {
                    if ((AC >= 0) && (AC < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[AC - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + rN + "\t" + AC + "\t" + rN_AC + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + rN + "\t" + AC + "\t" + rN_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + rN + "\t" + AC + "\t" + rN_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + rN + "\t" + AC + "\t" + rN_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + rN_total + "\t" + AC_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string rN_XC_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "rN" + "\t" + "XC" + "\t" + "rN:XC" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int rN_total = 0;
        int XC_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int rN = 31 - i;
            int XC = (int)Numbers.AdditiveComposites[i];
            N_total += N;
            rN_total += rN;
            XC_total += XC;

            string rN_XC = rN + ":" + XC;
            if ((rN >= 0) && (rN < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rN - 1];
                if (chapter != null)
                {
                    if ((XC >= 0) && (XC < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[XC - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + rN + "\t" + XC + "\t" + rN_XC + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + rN + "\t" + XC + "\t" + rN_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + rN + "\t" + XC + "\t" + rN_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + rN + "\t" + XC + "\t" + rN_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + rN_total + "\t" + XC_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    public static string ________________________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    public static string M_P_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "P" + "\t" + "M:P" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int P_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int P = (int)Numbers.Primes[i];
            M_total += M;
            P_total += P;

            string M_P = M + ":" + P;
            if ((M >= 0) && (M < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[M - 1];
                if (chapter != null)
                {
                    if ((P >= 0) && (P < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[P - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + P + "\t" + M_P + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + P + "\t" + M_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + P + "\t" + M_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + P + "\t" + M_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + P_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string M_AP_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "AP" + "\t" + "M:AP" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int AP_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int AP = (int)Numbers.AdditivePrimes[i];
            M_total += M;
            AP_total += AP;

            string M_AP = M + ":" + AP;
            if ((M >= 0) && (M < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[M - 1];
                if (chapter != null)
                {
                    if ((AP >= 0) && (AP < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[AP - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + AP + "\t" + M_AP + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + AP + "\t" + M_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + AP + "\t" + M_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + AP + "\t" + M_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + AP_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string M_XP_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "XP" + "\t" + "M:XP" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int XP_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int XP = (int)Numbers.AdditivePrimes[i];
            M_total += M;
            XP_total += XP;

            string M_XP = M + ":" + XP;
            if ((M >= 0) && (M < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[M - 1];
                if (chapter != null)
                {
                    if ((XP >= 0) && (XP < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[XP - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + XP + "\t" + M_XP + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + XP + "\t" + M_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + XP + "\t" + M_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + XP + "\t" + M_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + XP_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    public static string M_C_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "C" + "\t" + "M:C" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int C_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int C = (int)Numbers.Composites[i];
            M_total += M;
            C_total += C;

            string M_C = M + ":" + C;
            if ((M >= 0) && (M < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[M - 1];
                if (chapter != null)
                {
                    if ((C >= 0) && (C < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[C - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + C + "\t" + M_C + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + C + "\t" + M_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + C + "\t" + M_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + C + "\t" + M_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + C_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string M_AC_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "AC" + "\t" + "M:AC" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int AC_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int AC = (int)Numbers.AdditiveComposites[i];
            M_total += M;
            AC_total += AC;

            string M_AC = M + ":" + AC;
            if ((M >= 0) && (M < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[M - 1];
                if (chapter != null)
                {
                    if ((AC >= 0) && (AC < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[AC - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + AC + "\t" + M_AC + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + AC + "\t" + M_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + AC + "\t" + M_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + AC + "\t" + M_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + AC_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string M_XC_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "XC" + "\t" + "M:XC" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int XC_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int XC = (int)Numbers.AdditiveComposites[i];
            M_total += M;
            XC_total += XC;

            string M_XC = M + ":" + XC;
            if ((M >= 0) && (M < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[M - 1];
                if (chapter != null)
                {
                    if ((XC >= 0) && (XC < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[XC - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + XC + "\t" + M_XC + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + XC + "\t" + M_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + XC + "\t" + M_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + XC + "\t" + M_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + XC_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    public static string rM_P_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "rM" + "\t" + "P" + "\t" + "rM:P" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int rM_total = 0;
        int P_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int rM = (int)Numbers.MercifulNumbers[31 - 1 - i];
            int P = (int)Numbers.Primes[i];
            M_total += M;
            rM_total += rM;
            P_total += P;

            string rM_P = rM + ":" + P;
            if ((rM >= 0) && (rM < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rM - 1];
                if (chapter != null)
                {
                    if ((P >= 0) && (P < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[P - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + rM + "\t" + P + "\t" + rM_P + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + rM + "\t" + P + "\t" + rM_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + rM + "\t" + P + "\t" + rM_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + rM + "\t" + P + "\t" + rM_P + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + rM_total + "\t" + P_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string rM_AP_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "rM" + "\t" + "AP" + "\t" + "rM:AP" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int rM_total = 0;
        int AP_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int rM = (int)Numbers.MercifulNumbers[31 - 1 - i];
            int AP = (int)Numbers.AdditivePrimes[i];
            M_total += M;
            rM_total += rM;
            AP_total += AP;

            string rM_AP = rM + ":" + AP;
            if ((rM >= 0) && (rM < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rM - 1];
                if (chapter != null)
                {
                    if ((AP >= 0) && (AP < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[AP - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + rM + "\t" + AP + "\t" + rM_AP + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + rM + "\t" + AP + "\t" + rM_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + rM + "\t" + AP + "\t" + rM_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + rM + "\t" + AP + "\t" + rM_AP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + rM_total + "\t" + AP_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string rM_XP_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "rM" + "\t" + "XP" + "\t" + "rM:XP" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int rM_total = 0;
        int XP_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int rM = (int)Numbers.MercifulNumbers[31 - 1 - i];
            int XP = (int)Numbers.AdditivePrimes[i];
            M_total += M;
            rM_total += rM;
            XP_total += XP;

            string rM_XP = rM + ":" + XP;
            if ((rM >= 0) && (rM < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rM - 1];
                if (chapter != null)
                {
                    if ((XP >= 0) && (XP < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[XP - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + rM + "\t" + XP + "\t" + rM_XP + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + rM + "\t" + XP + "\t" + rM_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + rM + "\t" + XP + "\t" + rM_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + rM + "\t" + XP + "\t" + rM_XP + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + rM_total + "\t" + XP_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    public static string rM_C_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "rM" + "\t" + "C" + "\t" + "rM:C" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int rM_total = 0;
        int C_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int rM = (int)Numbers.MercifulNumbers[31 - 1 - i];
            int C = (int)Numbers.Composites[i];
            M_total += M;
            rM_total += rM;
            C_total += C;

            string rM_C = rM + ":" + C;
            if ((rM >= 0) && (rM < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rM - 1];
                if (chapter != null)
                {
                    if ((C >= 0) && (C < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[C - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + rM + "\t" + C + "\t" + rM_C + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + rM + "\t" + C + "\t" + rM_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + rM + "\t" + C + "\t" + rM_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + rM + "\t" + C + "\t" + rM_C + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + rM_total + "\t" + C_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string rM_AC_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "rM" + "\t" + "AC" + "\t" + "rM:AC" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int rM_total = 0;
        int AC_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int rM = (int)Numbers.MercifulNumbers[31 - 1 - i];
            int AC = (int)Numbers.AdditiveComposites[i];
            M_total += M;
            rM_total += rM;
            AC_total += AC;

            string rM_AC = rM + ":" + AC;
            if ((rM >= 0) && (rM < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rM - 1];
                if (chapter != null)
                {
                    if ((AC >= 0) && (AC < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[AC - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + rM + "\t" + AC + "\t" + rM_AC + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + rM + "\t" + AC + "\t" + rM_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + rM + "\t" + AC + "\t" + rM_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + rM + "\t" + AC + "\t" + rM_AC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + rM_total + "\t" + AC_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    private static string rM_XC_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("M" + "\t" + "rM" + "\t" + "XC" + "\t" + "rM:XC" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int M_total = 0;
        int rM_total = 0;
        int XC_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int M = (int)Numbers.MercifulNumbers[i];
            int rM = (int)Numbers.MercifulNumbers[31 - 1 - i];
            int XC = (int)Numbers.AdditiveComposites[i];
            M_total += M;
            rM_total += rM;
            XC_total += XC;

            string rM_XC = rM + ":" + XC;
            if ((rM >= 0) && (rM < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[rM - 1];
                if (chapter != null)
                {
                    if ((XC >= 0) && (XC < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[XC - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(M + "\t" + rM + "\t" + XC + "\t" + rM_XC + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(M + "\t" + rM + "\t" + XC + "\t" + rM_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(M + "\t" + rM + "\t" + XC + "\t" + rM_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(M + "\t" + rM + "\t" + XC + "\t" + rM_XC + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(M_total + "\t" + rM_total + "\t" + XC_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    public static string _______________________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    public static string N_M_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "M" + "\t" + "N:M" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int M_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int M = (int)Numbers.MercifulNumbers[i];
            N_total += N;
            M_total += M;

            string N_M = N + ":" + M;
            if ((N >= 0) && (N < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[N - 1];
                if (chapter != null)
                {
                    if ((M >= 0) && (M < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[M - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + M + "\t" + N_M + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + M + "\t" + N_M + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + M + "\t" + N_M + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + M + "\t" + N_M + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + M_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }
    public static string N_rM_31Verses(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (String.IsNullOrEmpty(param)) return null;
        if (client.Book == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("N" + "\t" + "rM" + "\t" + "N:rM" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "∑Pos" + "\t" + "∑∆" + "\t" + "All∑Pos" + "\t" + "All∑∆" + "\t" + "Text" + "\r\n");

        int N_total = 0;
        int rM_total = 0;
        int words_total = 0;
        int letters_total = 0;
        int unique_total = 0;
        long value_total = 0L;
        long positions_sum_total = 0L;
        long distances_sum_total = 0L;
        long all_positions_sum_total = 0L;
        long all_distances_sum_total = 0L;

        for (int i = 0; i < 31; i++)
        {
            int N = i + 1;
            int rM = (int)Numbers.MercifulNumbers[31 - 1 - i];
            N_total += N;
            rM_total += rM;

            string N_rM = N + ":" + rM;
            if ((N >= 0) && (N < client.Book.Chapters.Count))
            {
                Chapter chapter = client.Book.Chapters[N - 1];
                if (chapter != null)
                {
                    if ((rM >= 0) && (rM < chapter.Verses.Count))
                    {
                        Verse verse = chapter.Verses[rM - 1];
                        if (verse != null)
                        {
                            int words = verse.Words.Count;
                            int letters = verse.LetterCount;
                            int unique = verse.UniqueLetters.Count;
                            long value = client.CalculateValue(verse);
                            long positions_sum = verse.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                            long distances_sum = verse.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            long all_positions_sum = 0;
                            long all_distances_sum = 0;
                            string verses_text = "";
                            if (param == "0")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        all_positions_sum += v.Text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                        all_distances_sum += v.Text.LetterDistancesSum(false, client.Book.WithDiacritics);
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                            }
                            else if (param == "1")
                            {
                                foreach (Verse v in client.Book.Verses)
                                {
                                    if (client.CalculateValue(v) == value)
                                    {
                                        verses_text += v.Text + "\t";
                                    }
                                }
                                verses_text.Remove(verses_text.Length - 1, 1); // \t
                                all_positions_sum = verses_text.LetterPositionsSum(false, client.Book.WithDiacritics);
                                all_distances_sum = verses_text.LetterDistancesSum(false, client.Book.WithDiacritics);
                            }

                            words_total += words;
                            letters_total += letters;
                            unique_total += unique;
                            value_total += value;
                            positions_sum_total += positions_sum;
                            distances_sum_total += distances_sum;
                            all_positions_sum_total += all_positions_sum;
                            all_distances_sum_total += all_distances_sum;

                            str.Append(N + "\t" + rM + "\t" + N_rM + "\t" + words + "\t" + letters + "\t" + unique + "\t" + value + "\t" + positions_sum + "\t" + distances_sum + "\t" + all_positions_sum + "\t" + all_distances_sum + "\t" + verses_text + "\r\n");
                        }
                        else
                        {
                            str.Append(N + "\t" + rM + "\t" + N_rM + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                        }
                    }
                    else
                    {
                        str.Append(N + "\t" + rM + "\t" + N_rM + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
                    }
                }
            }
            else
            {
                str.Append(N + "\t" + rM + "\t" + N_rM + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + "" + "\r\n");
            }
        }
        str.Append(N_total + "\t" + rM_total + "\t" + "" + "\t" + words_total + "\t" + letters_total + "\t" + unique_total + "\t" + value_total + "\t" + positions_sum_total + "\t" + distances_sum_total + "\t" + all_positions_sum_total + "\t" + all_distances_sum_total + "\t" + "" + "\r\n");

        return str.ToString();
    }

    public static string ___________________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    public static string VersesWithIntialLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        return DoVersesWithIntialLetters(client, param, in_search_result);
    }
    public static string VersesWithOnlyIntialLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        return DoVersesWithOnlyIntialLetters(client, param, in_search_result);
    }
    public static string VersesWithAllIntialLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        return DoVersesWithAllIntialLetters(client, param, in_search_result);
    }
    public static string VersesWithOnlyAllIntialLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        return DoVersesWithOnlyAllIntialLetters(client, param, in_search_result);
    }
    public static string VersesWithNoIntialLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        return DoVersesWithNoIntialLetters(client, param, in_search_result);
    }
    private static string DoVersesWithIntialLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("#" + "\t" + "Verse" + "\t" + "Address" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "DigitSum" + "\t" + "Text");
        str.AppendLine();

        int count = 0;
        foreach (Verse verse in client.Book.Verses)
        {
            bool contains_initial_letters = false;
            foreach (char c in Constants.INITIAL_LETTERS)
            {
                if (verse.Text.Contains(c.ToString()))
                {
                    contains_initial_letters = true;
                    break;
                }
            }

            if (contains_initial_letters)
            {
                count++;
                str.Append(count.ToString() + "\t");
                str.Append(verse.Number.ToString() + "\t");
                str.Append(verse.Address.ToString() + "\t");
                str.Append(verse.Words.Count.ToString() + "\t");
                str.Append(verse.LetterCount.ToString() + "\t");
                str.Append(verse.UniqueLetters.Count.ToString() + "\t");
                long value = client.CalculateValue(verse);
                str.Append(value.ToString() + "\t");
                str.Append(Numbers.DigitSum(value).ToString() + "\t");
                str.Append(verse.Text + "\t");
                str.AppendLine();
            }
        }
        return str.ToString();
    }
    private static string DoVersesWithOnlyIntialLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("#" + "\t" + "Verse" + "\t" + "Address" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "DigitSum" + "\t" + "Text");
        str.AppendLine();

        int count = 0;
        foreach (Verse verse in client.Book.Verses)
        {
            string verse_text = verse.Text.SimplifyTo(client.NumerologySystem.TextMode);
            bool contains_only_initial_letters = true;
            foreach (char c in verse_text)
            {
                if (c == ' ') continue;
                if (!Constants.INITIAL_LETTERS.Contains(c))
                {
                    contains_only_initial_letters = false;
                    break;
                }
            }

            if (contains_only_initial_letters)
            {
                count++;
                str.Append(count.ToString() + "\t");
                str.Append(verse.Number.ToString() + "\t");
                str.Append(verse.Address.ToString() + "\t");
                str.Append(verse.Words.Count.ToString() + "\t");
                str.Append(verse.LetterCount.ToString() + "\t");
                str.Append(verse.UniqueLetters.Count.ToString() + "\t");
                long value = client.CalculateValue(verse);
                str.Append(value.ToString() + "\t");
                str.Append(Numbers.DigitSum(value).ToString() + "\t");
                str.Append(verse.Text + "\t");
                str.AppendLine();
            }
        }
        return str.ToString();
    }
    private static string DoVersesWithAllIntialLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("#" + "\t" + "Verse" + "\t" + "Address" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "DigitSum" + "\t" + "Text");
        str.AppendLine();

        int count = 0;
        foreach (Verse verse in client.Book.Verses)
        {
            bool contains_all_initial_letters = true;
            foreach (char c in Constants.INITIAL_LETTERS)
            {
                if (!verse.Text.Contains(c.ToString()))
                {
                    contains_all_initial_letters = false;
                    break;
                }
            }

            if (contains_all_initial_letters)
            {
                count++;
                str.Append(count.ToString() + "\t");
                str.Append(verse.Number.ToString() + "\t");
                str.Append(verse.Address.ToString() + "\t");
                str.Append(verse.Words.Count.ToString() + "\t");
                str.Append(verse.LetterCount.ToString() + "\t");
                str.Append(verse.UniqueLetters.Count.ToString() + "\t");
                long value = client.CalculateValue(verse);
                str.Append(value.ToString() + "\t");
                str.Append(Numbers.DigitSum(value).ToString() + "\t");
                str.Append(verse.Text + "\t");
                str.AppendLine();
            }
        }
        return str.ToString();
    }
    private static string DoVersesWithOnlyAllIntialLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.NumerologySystem == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("#" + "\t" + "Verse" + "\t" + "Address" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "DigitSum" + "\t" + "Text");
        str.AppendLine();

        int count = 0;
        foreach (Verse verse in client.Book.Verses)
        {
            bool contains_all_initial_letters = true;
            foreach (char c in Constants.INITIAL_LETTERS)
            {
                if (!verse.Text.Contains(c.ToString()))
                {
                    contains_all_initial_letters = false;
                    break;
                }
            }

            if (contains_all_initial_letters)
            {
                string verse_text = verse.Text.SimplifyTo(client.NumerologySystem.TextMode);
                bool contains_only_initial_letters = true;
                foreach (char c in verse_text)
                {
                    if (c == ' ') continue;
                    if (!Constants.INITIAL_LETTERS.Contains(c))
                    {
                        contains_only_initial_letters = false;
                        break;
                    }
                }

                if (contains_only_initial_letters)
                {
                    count++;
                    str.Append(count.ToString() + "\t");
                    str.Append(verse.Number.ToString() + "\t");
                    str.Append(verse.Address.ToString() + "\t");
                    str.Append(verse.Words.Count.ToString() + "\t");
                    str.Append(verse.LetterCount.ToString() + "\t");
                    str.Append(verse.UniqueLetters.Count.ToString() + "\t");
                    long value = client.CalculateValue(verse);
                    str.Append(value.ToString() + "\t");
                    str.Append(Numbers.DigitSum(value).ToString() + "\t");
                    str.Append(verse.Text + "\t");
                    str.AppendLine();
                }
            }
        }
        return str.ToString();
    }
    private static string DoVersesWithNoIntialLetters(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("#" + "\t" + "Verse" + "\t" + "Address" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "DigitSum" + "\t" + "Text");
        str.AppendLine();

        int count = 0;
        foreach (Verse verse in client.Book.Verses)
        {
            bool contain_no_initial_letters = true;
            foreach (char c in Constants.INITIAL_LETTERS)
            {
                if (verse.Text.Contains(c.ToString()))
                {
                    contain_no_initial_letters = false;
                    break;
                }
            }

            if (contain_no_initial_letters)
            {
                count++;
                str.Append(count.ToString() + "\t");
                str.Append(verse.Number.ToString() + "\t");
                str.Append(verse.Address.ToString() + "\t");
                str.Append(verse.Words.Count.ToString() + "\t");
                str.Append(verse.LetterCount.ToString() + "\t");
                str.Append(verse.UniqueLetters.Count.ToString() + "\t");
                long value = client.CalculateValue(verse);
                str.Append(value.ToString() + "\t");
                str.Append(Numbers.DigitSum(value).ToString() + "\t");
                str.Append(verse.Text + "\t");
                str.AppendLine();
            }
        }
        return str.ToString();
    }

    private static string _________________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    private static string FindVersesWithXValueDigitSum(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        return DoFindVersesWithXValueDigitSum(client, param, in_search_result, NumberType.Natural);
    }
    private static string FindVersesWithPValueAndXDigitSum(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        return DoFindVersesWithXValueDigitSum(client, param, in_search_result, NumberType.Prime);
    }
    private static string FindVersesWithAPValueAndXDigitSum(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        return DoFindVersesWithXValueDigitSum(client, param, in_search_result, NumberType.AdditivePrime);
    }
    private static string FindVersesWithCValueAndXDigitSum(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        return DoFindVersesWithXValueDigitSum(client, param, in_search_result, NumberType.Composite);
    }
    private static string FindVersesWithACValueAndXDigitSum(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        return DoFindVersesWithXValueDigitSum(client, param, in_search_result, NumberType.AdditiveComposite);
    }
    private static string DoFindVersesWithXValueDigitSum(Client client, string param, bool in_search_result, NumberType number_type)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Verses == null) return null;

        StringBuilder str = new StringBuilder();
        str.Append("#" + "\t" + "Verse" + "\t" + "Address" + "\t" + "Words" + "\t" + "Letters" + "\t" + "Unique" + "\t" + "Value" + "\t" + "DigitSum" + "\t" + "Text");
        str.AppendLine();

        int count = 0;
        foreach (Verse verse in client.Book.Verses)
        {
            long value = client.CalculateValue(verse);

            bool param_condition = false;
            if (param == "") // target == any digit sum
            {
                param_condition = true;
            }
            else if (param.ToUpper() == "P") // target == prime digit sum
            {
                param_condition = Numbers.IsPrime(Numbers.DigitSum(value));
            }
            else if (param.ToUpper() == "AP") // target == additive prime digit sum
            {
                param_condition = Numbers.IsAdditivePrime(Numbers.DigitSum(value));
            }
            else if (param.ToUpper() == "XP") // target == non-additive prime digit sum
            {
                param_condition = Numbers.IsNonAdditivePrime(Numbers.DigitSum(value));
            }
            else if (param.ToUpper() == "C") // target == composite digit sum
            {
                param_condition = Numbers.IsComposite(Numbers.DigitSum(value));
            }
            else if (param.ToUpper() == "AC") // target == additive composite digit sum
            {
                param_condition = Numbers.IsAdditiveComposite(Numbers.DigitSum(value));
            }
            else if (param.ToUpper() == "XC") // target == non-additive composite digit sum
            {
                param_condition = Numbers.IsNonAdditiveComposite(Numbers.DigitSum(value));
            }
            else
            {
                int target;
                if (int.TryParse(param, out target))
                {
                    if (target == 0) // target == any digit sum
                    {
                        param_condition = true;
                    }
                    else
                    {
                        param_condition = (Numbers.DigitSum(value) == target);
                    }
                }
                else
                {
                    return null;  // invalid param data
                }
            }

            if (
                 (
                    (number_type == NumberType.Natural)
                    ||
                    (Numbers.IsNumberType(value, number_type))
                 )
                 &&
                 param_condition
               )
            {
                count++;
                str.Append(count.ToString() + "\t");
                str.Append(verse.Number.ToString() + "\t");
                str.Append(verse.Address.ToString() + "\t");
                str.Append(verse.Words.Count.ToString() + "\t");
                str.Append(verse.LetterCount.ToString() + "\t");
                str.Append(verse.UniqueLetters.Count.ToString() + "\t");
                str.Append(value.ToString() + "\t");
                str.Append(Numbers.DigitSum(value).ToString() + "\t");
                str.Append(verse.Text + "\t");
                str.AppendLine();
            }
        }
        return str.ToString();
    }

    private static string __________________________________________(Client client, string param, bool in_search_result)
    {
        return null;
    }
    private class ZeroDifferenceNumerologySystem
    {
        public NumberType NumberType;
        public NumerologySystem NumerologySystem;

        // these two need to be equal
        public long BismAllahValue = -1L;
        public int AlFatihaValueIndex = -1; // PrimeIndex | AdditivePrimeIndex

        // these two need to be equal
        public long AlFatihaValue = -1L;
        public int BookValueIndex = -1; // PrimeIndex | AdditivePrimeIndex
    }
    private static string FindSystemOfBismAllahEqualsAlFatihaIndex(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Chapters == null) return null;
        if (client.Book.Chapters.Count <= 0) return null;
        if (client.Book.Chapters[0].Verses == null) return null;
        if (client.Book.Chapters[0].Verses.Count <= 0) return null;
        if (client.NumerologySystem == null) return null;

        NumerologySystem backup_numerology_system = new NumerologySystem(client.NumerologySystem);

        long target_difference;
        try
        {
            target_difference = long.Parse(param);
        }
        catch
        {
            target_difference = 0L;
        }

        // zero difference between Value(BismAllah) and ValueIndex(AlFatiha)
        List<ZeroDifferenceNumerologySystem> good_numerology_systems = new List<ZeroDifferenceNumerologySystem>();

        NumberType[] number_types = (NumberType[])Enum.GetValues(typeof(NumberType));
        foreach (NumberType number_type in number_types)
        {
            if (
                (number_type == NumberType.Prime) ||
                (number_type == NumberType.AdditivePrime) ||
                (number_type == NumberType.NonAdditivePrime) ||
                (number_type == NumberType.Composite) ||
                (number_type == NumberType.AdditiveComposite) ||
                (number_type == NumberType.NonAdditiveComposite)
               )
            {
                // Quran 74:30 "Over It Nineteen."
                int PERMUTATIONS = 524288; // 2^19
                for (int i = 0; i < PERMUTATIONS; i++)
                {
                    client.NumerologySystem.AddToLetterLNumber = ((i & 262144) != 0);
                    client.NumerologySystem.AddToLetterWNumber = ((i & 131072) != 0);
                    client.NumerologySystem.AddToLetterVNumber = ((i & 65536) != 0);
                    client.NumerologySystem.AddToLetterCNumber = ((i & 32768) != 0);
                    client.NumerologySystem.AddToLetterLDistance = ((i & 16384) != 0);
                    client.NumerologySystem.AddToLetterWDistance = ((i & 8192) != 0);
                    client.NumerologySystem.AddToLetterVDistance = ((i & 4096) != 0);
                    client.NumerologySystem.AddToLetterCDistance = ((i & 2048) != 0);
                    client.NumerologySystem.AddToWordWNumber = ((i & 1024) != 0);
                    client.NumerologySystem.AddToWordVNumber = ((i & 512) != 0);
                    client.NumerologySystem.AddToWordCNumber = ((i & 256) != 0);
                    client.NumerologySystem.AddToWordWDistance = ((i & 128) != 0);
                    client.NumerologySystem.AddToWordVDistance = ((i & 64) != 0);
                    client.NumerologySystem.AddToWordCDistance = ((i & 32) != 0);
                    client.NumerologySystem.AddToVerseVNumber = ((i & 16) != 0);
                    client.NumerologySystem.AddToVerseCNumber = ((i & 8) != 0);
                    client.NumerologySystem.AddToVerseVDistance = ((i & 4) != 0);
                    client.NumerologySystem.AddToVerseCDistance = ((i & 2) != 0);
                    client.NumerologySystem.AddToChapterCNumber = ((i & 1) != 0);

                    long alfatiha_value = client.CalculateValue(client.Book.Chapters[0]);
                    int alfatiha_value_index = 0;
                    switch (number_type)
                    {
                        case NumberType.Prime:
                            {
                                alfatiha_value_index = Numbers.PrimeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        case NumberType.AdditivePrime:
                            {
                                alfatiha_value_index = Numbers.AdditivePrimeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        case NumberType.NonAdditivePrime:
                            {
                                alfatiha_value_index = Numbers.NonAdditivePrimeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        case NumberType.Composite:
                            {
                                alfatiha_value_index = Numbers.CompositeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        case NumberType.AdditiveComposite:
                            {
                                alfatiha_value_index = Numbers.AdditiveCompositeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        case NumberType.NonAdditiveComposite:
                            {
                                alfatiha_value_index = Numbers.NonAdditiveCompositeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        default:
                            break;
                    }

                    if (alfatiha_value_index > 0)
                    {
                        long bismAllah_value = client.CalculateValue(client.Book.Chapters[0].Verses[0]);

                        long difference = bismAllah_value - (long)alfatiha_value_index;
                        if (Math.Abs(difference) <= target_difference)
                        {
                            ZeroDifferenceNumerologySystem good_numerology_system = new ZeroDifferenceNumerologySystem();
                            good_numerology_system.NumerologySystem = new NumerologySystem(client.NumerologySystem);
                            good_numerology_system.NumberType = number_type;
                            good_numerology_system.BismAllahValue = bismAllah_value;
                            good_numerology_system.AlFatihaValue = alfatiha_value;
                            good_numerology_system.AlFatihaValueIndex = alfatiha_value_index;
                            good_numerology_systems.Add(good_numerology_system);
                        }
                    }

                } // next PERMUTATION

                string filename = "BismAllahEqualsAlFatiha" + number_type.ToString() + "IndexSystem" + ".txt";
                if (Directory.Exists(Globals.RESEARCH_FOLDER))
                {
                    string path = Globals.RESEARCH_FOLDER + "/" + filename;

                    StringBuilder str = new StringBuilder();
                    str.AppendLine("TextMode" +
                            "\t" + "LetterOrder" +
                            "\t" + "LetterValues" +
                            "\t" + "AddToLetterLNumber" +
                            "\t" + "AddToLetterWNumber" +
                            "\t" + "AddToLetterVNumber" +
                            "\t" + "AddToLetterCNumber" +
                            "\t" + "AddToLetterLDistance" +
                            "\t" + "AddToLetterWDistance" +
                            "\t" + "AddToLetterVDistance" +
                            "\t" + "AddToLetterCDistance" +
                            "\t" + "AddToWordWNumber" +
                            "\t" + "AddToWordVNumber" +
                            "\t" + "AddToWordCNumber" +
                            "\t" + "AddToWordWDistance" +
                            "\t" + "AddToWordVDistance" +
                            "\t" + "AddToWordCDistance" +
                            "\t" + "AddToVerseVNumber" +
                            "\t" + "AddToVerseCNumber" +
                            "\t" + "AddToVerseVDistance" +
                            "\t" + "AddToVerseCDistance" +
                            "\t" + "AddToChapterCNumber" +
                            "\t" + "BismAllahValue" +
                            "\t" + "AlFatihaIndex"
                        );
                    foreach (ZeroDifferenceNumerologySystem good_numerology_system in good_numerology_systems)
                    {
                        str.Append(good_numerology_system.NumerologySystem.ToTabbedString());
                        str.Append("\t" + good_numerology_system.BismAllahValue.ToString());
                        str.Append("\t" + good_numerology_system.AlFatihaValueIndex.ToString());
                        str.AppendLine();
                    }
                    FileHelper.SaveText(path, str.ToString());
                    FileHelper.DisplayFile(path);
                }

                // clear for next NumberType
                good_numerology_systems.Clear();

            } // if NumberType

        } // next NumberType

        client.NumerologySystem = backup_numerology_system;

        return null;
    }
    private static string FindSystemOfAlFatihaEqualsQuranIndex(Client client, string param, bool in_search_result)
    {
        if (client == null) return null;
        if (client.Book == null) return null;
        if (client.Book.Chapters == null) return null;
        if (client.Book.Chapters.Count <= 0) return null;
        if (client.Book.Chapters[0].Verses == null) return null;
        if (client.Book.Chapters[0].Verses.Count <= 0) return null;
        if (client.NumerologySystem == null) return null;

        NumerologySystem backup_numerology_system = new NumerologySystem(client.NumerologySystem);

        long target_difference;
        try
        {
            target_difference = long.Parse(param);
        }
        catch
        {
            target_difference = 0L;
        }

        // zero difference between Value(BismAllah) and ValueIndex(AlFatiha)
        List<ZeroDifferenceNumerologySystem> good_numerology_systems = new List<ZeroDifferenceNumerologySystem>();

        // zero difference between Value(AlFatiha) and ValueIndex(Book)
        List<ZeroDifferenceNumerologySystem> best_numerology_systems = new List<ZeroDifferenceNumerologySystem>();

        NumberType[] number_types = (NumberType[])Enum.GetValues(typeof(NumberType));
        foreach (NumberType number_type in number_types)
        {
            if (
                (number_type == NumberType.Prime) ||
                (number_type == NumberType.AdditivePrime) ||
                (number_type == NumberType.NonAdditivePrime) ||
                (number_type == NumberType.Composite) ||
                (number_type == NumberType.AdditiveComposite) ||
                (number_type == NumberType.NonAdditiveComposite)
               )
            {
                // Quran 74:30 "Over It Nineteen."
                int PERMUTATIONS = 524288; // 2^19
                for (int i = 0; i < PERMUTATIONS; i++)
                {
                    client.NumerologySystem.AddToLetterLNumber = ((i & 262144) != 0);
                    client.NumerologySystem.AddToLetterWNumber = ((i & 131072) != 0);
                    client.NumerologySystem.AddToLetterVNumber = ((i & 65536) != 0);
                    client.NumerologySystem.AddToLetterCNumber = ((i & 32768) != 0);
                    client.NumerologySystem.AddToLetterLDistance = ((i & 16384) != 0);
                    client.NumerologySystem.AddToLetterWDistance = ((i & 8192) != 0);
                    client.NumerologySystem.AddToLetterVDistance = ((i & 4096) != 0);
                    client.NumerologySystem.AddToLetterCDistance = ((i & 2048) != 0);
                    client.NumerologySystem.AddToWordWNumber = ((i & 1024) != 0);
                    client.NumerologySystem.AddToWordVNumber = ((i & 512) != 0);
                    client.NumerologySystem.AddToWordCNumber = ((i & 256) != 0);
                    client.NumerologySystem.AddToWordWDistance = ((i & 128) != 0);
                    client.NumerologySystem.AddToWordVDistance = ((i & 64) != 0);
                    client.NumerologySystem.AddToWordCDistance = ((i & 32) != 0);
                    client.NumerologySystem.AddToVerseVNumber = ((i & 16) != 0);
                    client.NumerologySystem.AddToVerseCNumber = ((i & 8) != 0);
                    client.NumerologySystem.AddToVerseVDistance = ((i & 4) != 0);
                    client.NumerologySystem.AddToVerseCDistance = ((i & 2) != 0);
                    client.NumerologySystem.AddToChapterCNumber = ((i & 1) != 0);

                    long alfatiha_value = client.CalculateValue(client.Book.Chapters[0]);
                    int alfatiha_value_index = 0;
                    switch (number_type)
                    {
                        case NumberType.Prime:
                            {
                                alfatiha_value_index = Numbers.PrimeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        case NumberType.AdditivePrime:
                            {
                                alfatiha_value_index = Numbers.AdditivePrimeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        case NumberType.NonAdditivePrime:
                            {
                                alfatiha_value_index = Numbers.NonAdditivePrimeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        case NumberType.Composite:
                            {
                                alfatiha_value_index = Numbers.CompositeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        case NumberType.AdditiveComposite:
                            {
                                alfatiha_value_index = Numbers.AdditiveCompositeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        case NumberType.NonAdditiveComposite:
                            {
                                alfatiha_value_index = Numbers.NonAdditiveCompositeIndexOf(alfatiha_value) + 1;
                            }
                            break;
                        default:
                            break;
                    }

                    if (alfatiha_value_index > 0)
                    {
                        long bismAllah_value = client.CalculateValue(client.Book.Chapters[0].Verses[0]);

                        long difference = bismAllah_value - (long)alfatiha_value_index;
                        if (difference == 0L) // not (Math.Abs(difference) <= target_difference) // use target_difference for best systems only (not good systems)
                        {
                            ZeroDifferenceNumerologySystem good_numerology_system = new ZeroDifferenceNumerologySystem();
                            good_numerology_system.NumerologySystem = new NumerologySystem(client.NumerologySystem);
                            good_numerology_system.NumberType = number_type;
                            good_numerology_system.BismAllahValue = bismAllah_value;
                            good_numerology_system.AlFatihaValue = alfatiha_value;
                            good_numerology_system.AlFatihaValueIndex = alfatiha_value_index;
                            good_numerology_systems.Add(good_numerology_system);

                            // is  Value(Book) == ValueIndex(Al-Faiha)
                            long book_value = client.CalculateValue(client.Book);
                            int book_value_index = 0;
                            switch (good_numerology_system.NumberType)
                            {
                                case NumberType.Prime:
                                    {
                                        book_value_index = Numbers.PrimeIndexOf(book_value) + 1;
                                    }
                                    break;
                                case NumberType.AdditivePrime:
                                    {
                                        book_value_index = Numbers.AdditivePrimeIndexOf(book_value) + 1;
                                    }
                                    break;
                                case NumberType.NonAdditivePrime:
                                    {
                                        book_value_index = Numbers.NonAdditivePrimeIndexOf(book_value) + 1;
                                    }
                                    break;
                                case NumberType.Composite:
                                    {
                                        book_value_index = Numbers.CompositeIndexOf(book_value) + 1;
                                    }
                                    break;
                                case NumberType.AdditiveComposite:
                                    {
                                        book_value_index = Numbers.AdditiveCompositeIndexOf(book_value) + 1;
                                    }
                                    break;
                                case NumberType.NonAdditiveComposite:
                                    {
                                        book_value_index = Numbers.NonAdditiveCompositeIndexOf(book_value) + 1;
                                    }
                                    break;
                                default:
                                    break;
                            }

                            if (book_value_index > 0)
                            {
                                difference = alfatiha_value - (long)book_value_index;
                                if (Math.Abs(difference) <= target_difference)
                                {
                                    ZeroDifferenceNumerologySystem best_numerology_system = good_numerology_system;

                                    // collect all matching systems to print out at the end
                                    best_numerology_systems.Add(best_numerology_system);

                                    // prinet out the current matching system now
                                    string i_filename = "AlFatihaEqualsQuran" + number_type.ToString() + "IndexSystem" + ".txt";
                                    if (Directory.Exists(Globals.RESEARCH_FOLDER))
                                    {
                                        string i_path = Globals.RESEARCH_FOLDER + "/" + i_filename;

                                        StringBuilder i_str = new StringBuilder();
                                        i_str.AppendLine("TextMode" +
                                                "\t" + "LetterOrder" +
                                                "\t" + "LetterValues" +
                                                "\t" + "AddToLetterLNumber" +
                                                "\t" + "AddToLetterWNumber" +
                                                "\t" + "AddToLetterVNumber" +
                                                "\t" + "AddToLetterCNumber" +
                                                "\t" + "AddToLetterLDistance" +
                                                "\t" + "AddToLetterWDistance" +
                                                "\t" + "AddToLetterVDistance" +
                                                "\t" + "AddToLetterCDistance" +
                                                "\t" + "AddToWordWNumber" +
                                                "\t" + "AddToWordVNumber" +
                                                "\t" + "AddToWordCNumber" +
                                                "\t" + "AddToWordWDistance" +
                                                "\t" + "AddToWordVDistance" +
                                                "\t" + "AddToWordCDistance" +
                                                "\t" + "AddToVerseVNumber" +
                                                "\t" + "AddToVerseCNumber" +
                                                "\t" + "AddToVerseVDistance" +
                                                "\t" + "AddToVerseCDistance" +
                                                "\t" + "AddToChapterCNumber" +
                                                "\t" + "BismAllahValue" +
                                                "\t" + "AlFatihaIndex" +
                                                "\t" + "AlFatihaValue" +
                                                "\t" + "BookValueIndex"
                                            );

                                        i_str.Append(best_numerology_system.NumerologySystem.ToTabbedString());
                                        i_str.Append("\t" + best_numerology_system.BismAllahValue.ToString());
                                        i_str.Append("\t" + best_numerology_system.AlFatihaValueIndex.ToString());
                                        i_str.Append("\t" + best_numerology_system.AlFatihaValue.ToString());
                                        i_str.Append("\t" + best_numerology_system.BookValueIndex.ToString());
                                        i_str.AppendLine();

                                        FileHelper.SaveText(i_path, i_str.ToString());
                                        FileHelper.DisplayFile(i_path);
                                    }

                                    // wait for file to be written correctly to prevent cross-thread problem
                                    // if another match was found shortly after this one
                                    Thread.Sleep(3000);
                                }
                            }
                        }
                    }

                } // next PERMUTATION

                string filename = "AlFatihaEqualsQuran" + number_type.ToString() + "IndexSystem" + ".txt";
                if (Directory.Exists(Globals.RESEARCH_FOLDER))
                {
                    string path = Globals.RESEARCH_FOLDER + "/" + filename;

                    StringBuilder str = new StringBuilder();
                    str.AppendLine("TextMode" +
                            "\t" + "LetterOrder" +
                            "\t" + "LetterValues" +
                            "\t" + "AddToLetterLNumber" +
                            "\t" + "AddToLetterWNumber" +
                            "\t" + "AddToLetterVNumber" +
                            "\t" + "AddToLetterCNumber" +
                            "\t" + "AddToLetterLDistance" +
                            "\t" + "AddToLetterWDistance" +
                            "\t" + "AddToLetterVDistance" +
                            "\t" + "AddToLetterCDistance" +
                            "\t" + "AddToWordWNumber" +
                            "\t" + "AddToWordVNumber" +
                            "\t" + "AddToWordCNumber" +
                            "\t" + "AddToWordWDistance" +
                            "\t" + "AddToWordVDistance" +
                            "\t" + "AddToWordCDistance" +
                            "\t" + "AddToVerseVNumber" +
                            "\t" + "AddToVerseCNumber" +
                            "\t" + "AddToVerseVDistance" +
                            "\t" + "AddToVerseCDistance" +
                            "\t" + "AddToChapterCNumber" +
                            "\t" + "BismAllahValue" +
                            "\t" + "AlFatihaIndex" +
                            "\t" + "AlFatihaValue" +
                            "\t" + "BookValueIndex"
                        );
                    foreach (ZeroDifferenceNumerologySystem best_numerology_system in best_numerology_systems)
                    {
                        str.Append(best_numerology_system.NumerologySystem.ToTabbedString());
                        str.Append("\t" + best_numerology_system.BismAllahValue.ToString());
                        str.Append("\t" + best_numerology_system.AlFatihaValueIndex.ToString());
                        str.Append("\t" + best_numerology_system.AlFatihaValue.ToString());
                        str.Append("\t" + best_numerology_system.BookValueIndex.ToString());
                        str.AppendLine();
                    }
                    FileHelper.SaveText(path, str.ToString());
                    FileHelper.DisplayFile(path);
                }

                // clear for next NumberType
                good_numerology_systems.Clear();
                best_numerology_systems.Clear();

            } // if NumberType

        } // next NumberType

        client.NumerologySystem = backup_numerology_system;

        return null;
    }
}
