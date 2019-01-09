using System;
using System.Text;
using System.Collections.Generic;

// Enable Extensions in .NET 2.0
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    internal sealed class ExtensionAttribute : Attribute { }
}

public static class Constants
{
    public static string OPEN_BRACKET = "{";
    public static string CLOSE_BRACKET = "}";

    public static List<char> ARABIC_DIGITS = new List<char>()
    { 
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9'
    };

    public static List<char> INDIAN_DIGITS = new List<char>()
    { 
        '٠',
        '١',
        '٢',
        '٣',
        '٤',
        '٥',
        '٦',
        '٧',
        '٨',
        '٩'
    };

    public static List<char> ARABIC_LETTERS = new List<char>
    { 
        'ء',
        'ا',
        'إ',
        'أ',
        'ٱ',
        'آ',
        'ب',
        'ت',
        'ث',
        'ج',
        'ح',
        'خ',
        'د',
        'ذ',
        'ر',
        'ز',
        'س',
        'ش',
        'ص',
        'ض',
        'ط',
        'ظ',
        'ع',
        'غ',
        'ف',
        'ق',
        'ك',
        'ل',
        'م',
        'ن',
        'ه',
        'ة',
        'و',
        'ؤ',
        'ى',
        'ي',
        'ئ'
    };

    public static List<char> STOPMARKS = new List<char> 
    { 
        'ۙ',    // Laaa         MustContinue
        'ۖ',    // Sala         ShouldContinue
        'ۚ',    // Jeem         CanStop
        'ۛ',    // Dots         CanStopAtEither
        'ۗ',    // Qala         ShouldStop
        'ۜ',    // Seen         MustPause
        'ۘ'     // Meem         MustStop
    };

    public static List<char> QURANMARKS = new List<char> 
    { 
        '۞',    // Partition mark
        '۩',    // Prostaration mark (Obligatory)
        '⌂'     // Prostaration mark (Recommended)
    };

    public static List<char> DIACRITICS = new List<char> 
    { 
        'ٰ',
        'ٔ',
        'ْ',
        'ِ',
        'َ',
        'ۥ',
        'ُ',
        'ّ',
        'ٓ',
        'ً',
        'ٍ',
        'ٌ',
        '۟',
        'ۦ',
        'ۧ',
        'ۭ',
        'ۢ',
        'ۜ',
        'ۣ',
        '۠',
        'ۨ',
        '۪',
        '۫',
        '۬',
        'ـ'
    };

    public static List<char> SYMBOLS = new List<char> 
    { 
        '{',
        '}',
        '[',
        ']',
        '<',
        '>',
        '(',
        ')',
        '!',
        ':',
        '=',
        '+',
        '-',
        '*',
        '/',
        '%',
        '\\',
        '"',
        '\'',
        '~',
        '@',
        '#',
        '$',
        '^',
        '&',
        '_',
        '|',
        '?'
    };
}

public static class StringExtensions
{
    public static string RemoveDuplicates(this string source)
    {
        if (String.IsNullOrEmpty(source)) return "";

        string result = "";
        foreach (char c in source)
        {
            if (!result.Contains(c.ToString()))
            {
                result += c;
            }
        }

        return result;
    }
    public static string RemoveDuplicateLetters(this string source)
    {
        if (String.IsNullOrEmpty(source)) return "";

        string result = "";
        foreach (char c in source)
        {
            if (!result.Contains(c.ToString()))
            {
                result += c + " ";
            }
        }
        result = result.Remove(result.Length - 1, 1);

        return result;
    }
    public static string ToNth(this string source)
    {
        if (String.IsNullOrEmpty(source)) return "";

        string nth = "th";
        long number = 0L;
        if (long.TryParse(source, out number))
        {
            if (source.EndsWith("11")) nth = "th";
            else if (source.EndsWith("12")) nth = "th";
            else if (source.EndsWith("13")) nth = "th";
            else if (source.EndsWith("1")) nth = "st";
            else if (source.EndsWith("2")) nth = "nd";
            else if (source.EndsWith("3")) nth = "rd";
            else nth = "th";
        }

        return source + nth;
    }

    //http://www.codeproject.com/Articles/2270/Inside-C-Second-Edition-String-Handling-and-Regula
    public static string ToTitleCase(this string source)
    {
        if (String.IsNullOrEmpty(source)) return "";

        string result = "";
        source = source.ToLower();
        foreach (string words in source.Split())
        {
            result += char.ToUpper(words[0]);
            result += (words.Substring(1, words.Length - 1) + ' ');
        }
        return result;
    }

    //http://www.codeproject.com/Articles/2270/Inside-C-Second-Edition-String-Handling-and-Regula
    public static bool IsPalindrome(string source)
    {
        if (String.IsNullOrEmpty(source)) return false;

        int fulllength = source.Length - 1;
        int halflength = fulllength / 2;
        for (int i = 0; i <= halflength; i++)
        {
            if (source.Substring(i, 1) != source.Substring(fulllength - i, 1))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// returns an Arabic letter-by-letter de-transliteration of the source buckwater string
    /// </summary>
    /// <returns></returns>
    public static string ToArabic(this string source)
    {
        if (String.IsNullOrEmpty(source)) return "";

        Dictionary<char, char> dictionary = new Dictionary<char, char>()
        {
            { '\'', 'ء' },
            { '>', 'أ' },
            { '&', 'ؤ' },
            { '<', 'إ' },
            { '}', 'ئ' },
            { 'A', 'ا' },
            { 'b', 'ب' },
            { 'p', 'ة' },
            { 't', 'ت' },
            { 'v', 'ث' },
            { 'j', 'ج' },
            { 'H', 'ح' },
            { 'x', 'خ' },
            { 'd', 'د' },
            { '*', 'ذ' },
            { 'r', 'ر' },
            { 'z', 'ز' },
            { 's', 'س' },
            { '$', 'ش' },
            { 'S', 'ص' },
            { 'D', 'ض' },
            { 'T', 'ط' },
            { 'Z', 'ظ' },
            { 'E', 'ع' },
            { 'g', 'غ' },
            { '_', 'ـ' },
            { 'f', 'ف' },
            { 'q', 'ق' },
            { 'k', 'ك' },
            { 'l', 'ل' },
            { 'm', 'م' },
            { 'n', 'ن' },
            { 'h', 'ه' },
            { 'w', 'و' },
            { 'Y', 'ى' },
            { 'y', 'ي' },
            { 'F', 'ً' },
            { 'N', 'ٌ' },
            { 'K', 'ٍ' },
            { 'a', 'َ' },
            { 'u', 'ُ' },
            { 'i', 'ِ' },
            { '~', 'ّ' },
            { 'o', 'ْ' },
            { '^', 'ٓ' },
            { '#', 'ٔ' },
            { '`', 'ٰ' },
            { '{', 'ٱ' },
            { ':', 'ۜ' },
            { '@', '۟' },
            { '"', '۠' },
            { '[', 'ۢ' },
            { ';', 'ۣ' },
            { ',', 'ۥ' },
            { '.', 'ۧ' },
            { '!', 'ۨ' },
            { '-', '۪' },
            { '+', '۫' },
            { '%', '۬' },
            { ']', 'ۭ' }
        };

        StringBuilder str = new StringBuilder();
        if (dictionary != null)
        {
            foreach (char c in source)
            {
                if (dictionary.ContainsKey(c))
                {
                    str.Append(dictionary[c]);
                }
                else
                {
                    str.Append(c);
                }
            }
        }
        return str.ToString();
    }
    /// <summary>
    /// returns a Buckwalter letter-by-letter transliteration of the source arabic string
    /// </summary>
    /// <returns></returns>
    public static string ToBuckwalter(this string source)
    {
        if (String.IsNullOrEmpty(source)) return "";

        Dictionary<char, char> dictionary = new Dictionary<char, char>()
        {
            {'ء', '\'' },
            { 'أ', '>' },
            { 'ؤ', '&' },
            { 'إ', '<' },
            { 'ئ', '}' },
            { 'ا', 'A' },
            { 'ب', 'b' },
            { 'ة', 'p' },
            { 'ت', 't' },
            { 'ث', 'v' },
            { 'ج', 'j' },
            { 'ح', 'H' },
            { 'خ', 'x' },
            { 'د', 'd' },
            { 'ذ', '*' },
            { 'ر', 'r' },
            { 'ز', 'z' },
            { 'س', 's' },
            { 'ش', '$' },
            { 'ص', 'S' },
            { 'ض', 'D' },
            { 'ط', 'T' },
            { 'ظ', 'Z' },
            { 'ع', 'E' },
            { 'غ', 'g' },
            { 'ـ', '_' },
            { 'ف', 'f' },
            { 'ق', 'q' },
            { 'ك', 'k' },
            { 'ل', 'l' },
            { 'م', 'm' },
            { 'ن', 'n' },
            { 'ه', 'h' },
            { 'و', 'w' },
            { 'ى', 'Y' },
            { 'ي', 'y' },
            { 'ً', 'F' },
            { 'ٌ', 'N' },
            { 'ٍ', 'K' },
            { 'َ', 'a' },
            { 'ُ', 'u' },
            { 'ِ', 'i' },
            { 'ّ', '~' },
            { 'ْ', 'o' },
            { 'ٓ', '^' },
            { 'ٔ', '#' },
            { 'ٰ', '`' },
            { 'ٱ', '{' },
            { 'ۜ', ':' },
            { '۟', '@' },
            { '۠', '"' },
            { 'ۢ', '[' },
            { 'ۣ', ';' },
            { 'ۥ', ',' },
            { 'ۧ', '.' },
            { 'ۨ', '!' },
            { '۪', '-' },
            { '۫', '+' },
            { '۬', '%' },
            { 'ۭ', ']' }
        };

        StringBuilder str = new StringBuilder();
        if (dictionary != null)
        {
            foreach (char c in source)
            {
                if (dictionary.ContainsKey(c))
                {
                    str.Append(dictionary[c]);
                }
                else
                {
                    str.Append(c);
                }
            }
        }
        return str.ToString();
    }

    /// <summary>
    /// Is source made up of any combination of:
    /// Arabic letters with or without diacritics (harakaat), stopmarks, common symbols, and/or Indian numbers?
    /// </summary>
    public static bool IsArabic(this string source)
    {
        if (String.IsNullOrEmpty(source)) return false;

        foreach (char character in source)
        {
            if (character == ' ') continue;

            if (
                  !Constants.ARABIC_LETTERS.Contains(character) &&
                  !Constants.INDIAN_DIGITS.Contains(character) &&
                  !Constants.STOPMARKS.Contains(character) &&
                  !Constants.QURANMARKS.Contains(character) &&
                  !Constants.DIACRITICS.Contains(character) &&
                  !Constants.SYMBOLS.Contains(character)
               )
            {
                return false;
            }
        }

        return true;
    }
    /// <summary>
    /// Is source made up of any combination of:
    /// Arabic letters, stopmarks, common symbols, and/or Indian numbers
    /// AND
    /// diacritics (harakaat)?
    /// </summary>
    public static bool IsArabicWithDiacritics(this string source)
    {
        if (String.IsNullOrEmpty(source)) return false;

        if (source.IsArabic())
        {
            string simplified_text = source.Simplify36();
            return (source.Length > simplified_text.Length);
        }

        return false;
    }
    public static bool IsEnglish(this string source)
    {
        if (String.IsNullOrEmpty(source)) return false;

        foreach (char character in source)
        {
            if (character == ' ') continue;

            if (
                !(character >= 'A') && (character <= 'Z') &&
                !(character >= 'a') && (character <= 'z')
               )
            {
                return false;
            }
        }

        return true;
    }

    public static string SimplifyTo(this string source, string text_mode)
    {
        if (String.IsNullOrEmpty(source)) return source;

        if (text_mode == "Simplified28")
        {
            return source.Simplify28();
        }
        else if (text_mode == "Simplified29")
        {
            return source.Simplify29();
        }
        else if (text_mode == "Simplified30")
        {
            return source.Simplify30();
        }
        else if (text_mode == "Simplified31")
        {
            return source.Simplify31();
        }
        else if (text_mode == "Simplified36")
        {
            return source.Simplify36();
        }
        else if (text_mode == "Original")
        {
            return source.Simplify29();
            //return source;
        }
        else
        {
            return source.Simplify29();
        }
    }
    public static string Simplify28(this string source)
    {
        if (String.IsNullOrEmpty(source)) return source;

        string result = source.Simplify29();

        return result.Replace("ء", "");
    }
    public static string Simplify29(this string source)
    {
        if (String.IsNullOrEmpty(source)) return source;

        source = source.Simplify31();

        StringBuilder str = new StringBuilder();
        foreach (char character in source)
        {
            switch (character)
            {
                case 'ة': str.Append("ه"); break;
                case 'ى': str.Append("ي"); break;
                default: str.Append(character); break;
            }
        }
        return str.ToString();
    }
    public static string Simplify30(this string source)
    {
        if (String.IsNullOrEmpty(source)) return source;

        string result = source.Simplify31();

        return result.Replace("ء", "");
    }
    public static string Simplify31(this string source)
    {
        if (String.IsNullOrEmpty(source)) return source;

        source = source.Simplify36();

        StringBuilder str = new StringBuilder();
        foreach (char character in source)
        {
            switch (character)
            {
                case 'إ': str.Append("ا"); break;
                case 'أ': str.Append("ا"); break;
                case 'ٱ': str.Append("ا"); break;
                case 'ؤ': str.Append("و"); break;
                case 'ئ': str.Append("ي"); break;
                default: str.Append(character); break;
            }
        }
        return str.ToString();
    }
    public static string Simplify36(this string source)
    {
        if (String.IsNullOrEmpty(source)) return source;

        StringBuilder str = new StringBuilder();
        foreach (char character in source)
        {
            if (
                  !Constants.ARABIC_DIGITS.Contains(character) &&
                  !Constants.INDIAN_DIGITS.Contains(character) &&
                  (Constants.OPEN_BRACKET[0] != character) &&
                  (Constants.CLOSE_BRACKET[0] != character) &&
                  !Constants.STOPMARKS.Contains(character) &&
                  !Constants.QURANMARKS.Contains(character) &&
                  !Constants.DIACRITICS.Contains(character)
               )
            {
                str.Append(character);
            }
        }

        string result = str.ToString();
        while (result.Contains("  "))
        {
            result = result.Replace("  ", " ");
        }
        return result;
    }

    public static bool Contains(this string source, string value, StringComparison string_comparison)
    {
        if (String.IsNullOrEmpty(source)) return false;

        return source.IndexOf(value, string_comparison) != -1;
    }
    public static bool Contains(this string source, string value)
    {
        if (String.IsNullOrEmpty(source)) return false;

        return Contains(source, value, false);
    }
    public static bool Contains(this string source, string value, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;

        return case_sensitive ? source.Contains(value, StringComparison.InvariantCulture) : source.Contains(value, StringComparison.InvariantCultureIgnoreCase);
    }
    public static string Left(this string source, int length)
    {
        if (String.IsNullOrEmpty(source)) return "";

        if ((length > 0) && (length <= source.Length))
        {
            return source.Substring(0, length);
        }
        else
        {
            throw new ArgumentException();
        }
    }
    public static string Mid(this string source, int start, int end)
    {
        if (String.IsNullOrEmpty(source)) return "";

        if ((start >= end)
            && (start >= 0) && (start < source.Length)
            && (end >= 0) && (end < source.Length)
            )
        {
            return source.Substring(start, end - start);
        }
        else
        {
            throw new ArgumentException();
        }
    }
    public static string Right(this string source, int length)
    {
        if (String.IsNullOrEmpty(source)) return "";

        if ((length > 0) && (length <= source.Length))
        {
            return source.Substring(source.Length - 1 - length);
        }
        else
        {
            throw new ArgumentException();
        }
    }
    /// <summary>
    /// right trims source string at length if too long, or right space-pad source to length if too short.
    /// <para>Examples:</para>
    /// <para>"en.english".Pad(13)         returns  "en.english   "</para>
    /// <para>"en.transliteration".Pad(13) returns  "en.transliter"</para>
    /// </summary>
    public static string Pad(this string source, int length)
    {
        if (String.IsNullOrEmpty(source)) return "";

        if (source.Length >= length)
        {
            return source.Substring(0, length);
        }
        else
        {
            return source.PadRight(length);
        }
    }
    /// <summary>
    /// rigth trims source string from start to length if too long, or right space-pad source to length if too short.
    /// <para>Examples:</para>
    /// <para>"en.english".Pad(3, 10)         returns  "english   "</para>
    /// <para>"en.transliteration".Pad(3, 10) returns  "transliter"</para>
    /// </summary>
    public static string Pad(this string source, int start, int length)
    {
        if (String.IsNullOrEmpty(source)) return "";

        if (source.Length > start)
        {
            // right trim
            source = source.Substring(start);

            if (source.Length >= length)
            {
                return source.Substring(0, length);
            }
            else
            {
                return source.PadRight(length);
            }
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public static string ToArabicNumber(this string source)
    {
        if (String.IsNullOrEmpty(source)) return "";

        try
        {
            int number = int.Parse(source);
            return number.ToArabic();
        }
        catch
        {
            throw new ArgumentException();
        }
    }
    //public static string ToArabicNumber(this string source)
    //{
    //    System.Text.UTF8Encoding utf8Encoder = new UTF8Encoding();
    //    System.Text.Decoder utf8Decoder = utf8Encoder.GetDecoder();
    //    System.Text.StringBuilder convertedChars = new System.Text.StringBuilder();
    //    char[] convertedChar = new char[1];
    //    byte[] bytes = new byte[] { 217, 160 };
    //    char[] inputCharArray = source.ToCharArray();
    //    foreach (char c in inputCharArray)
    //    {
    //        if (char.IsDigit(c))
    //        {
    //            bytes[1] = Convert.ToByte(160 + char.GetNumericValue(c));
    //            utf8Decoder.GetChars(bytes, 0, 2, convertedChar, 0);
    //            convertedChars.Append(convertedChar[0]);
    //        }
    //        else
    //        {
    //            convertedChars.Append(c);
    //        }
    //    }
    //    return convertedChars.ToString();
    //}

    /// <summary> 
    /// returns true if source contains target inside it not at start or end, else false. 
    /// </summary>
    public static bool ContainsInside(this string source, string target)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;

        int index = source.IndexOf(target);
        if (index > 0) // match is after start
        {
            string temp = source.Substring(index + target.Length);
            if (temp.Length > 0) // there is some left over text after match
            {
                return true;
            }
        }

        return false;
    }

    /// <summary> 
    /// returns true if source contains any word of target, else false. 
    /// </summary>
    public static bool ContainsWordOf(this string source, string target)
    {
        return ContainsWordOf(source, target, false);
    }
    public static bool ContainsWordOf(this string source, string target, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;
        if (source == target) return true;

        string[] source_words = source.Split();
        string[] target_words = target.Split();
        int source_word_count = source_words.Length;
        int target_word_count = target_words.Length;

        for (int j = 0; j < target_word_count; j++)
        {
            for (int i = 0; i < source_word_count; i++)
            {
                if (case_sensitive)
                {
                    if (source_words[i].Contains(target_words[j]))
                    {
                        return true;
                    }
                }
                else
                {
                    if (source_words[i].ToLower().Contains(target_words[j].ToLower()))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    /// <summary> 
    /// returns true if source contains word_count words of target in any order, else false. 
    /// </summary>
    public static bool ContainsWordsOf(this string source, string target, int word_count)
    {
        return ContainsWordsOf(source, target, word_count, false);
    }
    public static bool ContainsWordsOf(this string source, string target, int word_count, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;
        if (source == target) return true;

        string[] source_words = source.Split();
        string[] target_words = target.Split();
        int source_word_count = source_words.Length;
        int target_word_count = target_words.Length;

        int common_word_count = 0;
        for (int j = 0; j < target_word_count; j++)
        {
            for (int i = 0; i < source_word_count; i++)
            {
                if (case_sensitive)
                {
                    if (source_words[i] != null)
                    {
                        if (source_words[i].Contains(target_words[j]))
                        {
                            source_words[i] = null; // remove it from list so not to be reused
                            common_word_count++;
                            break;
                        }
                    }
                }
                else
                {
                    if (source_words[i] != null)
                    {
                        if (source_words[i].ToLower().Contains(target_words[j].ToLower()))
                        {
                            source_words[i] = null; // remove it from list so not to be reused
                            common_word_count++;
                            break;
                        }
                    }
                }
            }
        }
        if (common_word_count >= word_count)
        {
            return true;
        }
        return false;
    }
    /// <summary> 
    /// returns true if source contains all words of target in any order, else false. 
    /// </summary>
    public static bool ContainsWordsOf(this string source, string target)
    {
        return ContainsWordsOf(source, target, false);
    }
    public static bool ContainsWordsOf(this string source, string target, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;
        if (source == target) return true;

        string[] target_words = target.Split();
        int target_word_count = target_words.Length;

        return ContainsWordsOf(source, target, target_word_count, case_sensitive);
    }

    /// <summary> 
    /// returns true if source contains any word of target_words, else false. 
    /// </summary>
    public static bool ContainsWordOf(this string source, List<string> target_words)
    {
        return ContainsWordOf(source, target_words, false);
    }
    public static bool ContainsWordOf(this string source, List<string> target_words, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if ((target_words == null) || (target_words.Count == 0)) return false;

        string[] source_words = source.Split();
        int source_word_count = source_words.Length;
        int target_word_count = target_words.Count;

        for (int j = 0; j < target_word_count; j++)
        {
            for (int i = 0; i < source_word_count; i++)
            {
                if (case_sensitive)
                {
                    if (source_words[i].Contains(target_words[j]))
                    {
                        return true;
                    }
                }
                else
                {
                    if (source_words[i].ToLower().Contains(target_words[j].ToLower()))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    /// <summary> 
    /// returns true if source contains word_count words of target_words in any order, else false. 
    /// </summary>
    public static bool ContainsWordsOf(this string source, List<string> target_words, int word_count)
    {
        return ContainsWordsOf(source, target_words, word_count, false);
    }
    public static bool ContainsWordsOf(this string source, List<string> target_words, int word_count, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if ((target_words == null) || (target_words.Count == 0)) return false;

        string[] source_words = source.Split();
        int source_word_count = source_words.Length;
        int target_word_count = target_words.Count;

        int common_word_count = 0;
        for (int j = 0; j < target_word_count; j++)
        {
            for (int i = 0; i < source_word_count; i++)
            {
                if (case_sensitive)
                {
                    if (source_words[i] != null)
                    {
                        if (source_words[i].Contains(target_words[j]))
                        {
                            source_words[i] = null; // remove it from list so not to be reused
                            common_word_count++;
                            break;
                        }
                    }
                }
                else
                {
                    if (source_words[i] != null)
                    {
                        if (source_words[i].ToLower().Contains(target_words[j].ToLower()))
                        {
                            source_words[i] = null; // remove it from list so not to be reused
                            common_word_count++;
                            break;
                        }
                    }
                }
            }
        }

        if (common_word_count >= word_count)
        {
            return true;
        }
        return false;
    }
    /// <summary> 
    /// returns true if source contains all words of target_words in any order, else false. 
    /// </summary>
    public static bool ContainsWordsOf(this string source, List<string> target_words)
    {
        return ContainsWordsOf(source, target_words, false);
    }
    public static bool ContainsWordsOf(this string source, List<string> target_words, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if ((target_words == null) || (target_words.Count == 0)) return false;

        int target_word_count = target_words.Count;

        return ContainsWordsOf(source, target_words, target_word_count, case_sensitive);
    }

    /// <summary> 
    /// returns true if source has at least one similar word to any word of target, else false. 
    /// </summary>
    public static bool HasSimilarWordsTo(this string source, string target, double similarity_percentage)
    {
        return HasSimilarWordsTo(source, target, similarity_percentage, false);
    }
    public static bool HasSimilarWordsTo(this string source, string target, double similarity_percentage, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;

        if (source == target) return true;

        string[] source_words = source.Split();
        string[] target_words = target.Split();
        int source_word_count = source_words.Length;
        int target_word_count = target_words.Length;

        for (int j = 0; j < target_word_count; j++)
        {
            for (int i = 0; i < source_word_count; i++)
            {
                if (source_words[i].IsSimilarTo(target_words[j], similarity_percentage, case_sensitive))
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary> 
    /// returns true if source contains word_count words of target in any order, else false. 
    /// </summary>
    public static bool HasSimilarWordsTo(this string source, string target, int word_count, double similarity_percentage)
    {
        return HasSimilarWordsTo(source, target, word_count, similarity_percentage, false);
    }
    public static bool HasSimilarWordsTo(this string source, string target, int word_count, double similarity_percentage, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;

        if (source == target) return true;

        string[] source_words = source.Split();
        string[] target_words = target.Split();
        int source_word_count = source_words.Length;
        int target_word_count = target_words.Length;

        int common_word_count = 0;
        for (int j = 0; j < target_word_count; j++)
        {
            for (int i = 0; i < source_word_count; i++)
            {
                if (source_words[i] != null)
                {
                    if (source_words[i].IsSimilarTo(target_words[j], similarity_percentage, case_sensitive))
                    {
                        source_words[i] = null; // remove it from list so not to be reused
                        common_word_count++;
                        break;
                    }
                }
            }
        }
        if (common_word_count >= word_count)
        {
            return true;
        }
        return false;
    }
    /// <summary> 
    /// returns true if source and target have similar first word, else false. 
    /// </summary>
    public static bool HasSimilarFirstWordTo(this string source, string target, double similarity_percentage)
    {
        return HasSimilarFirstWordTo(source, target, similarity_percentage, false);
    }
    public static bool HasSimilarFirstWordTo(this string source, string target, double similarity_percentage, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;

        if (source == target) return true;

        string[] source_words = source.Split();
        string[] target_words = target.Split();
        int source_word_count = source_words.Length;
        int target_word_count = target_words.Length;

        if ((source_word_count > 0) && (target_word_count > 0))
        {
            return (source_words[0].IsSimilarTo(target_words[0], similarity_percentage, case_sensitive));
        }
        else
        {
            return false;
        }
    }
    /// <summary> 
    /// returns true if source and target have similar last word, else false. 
    /// </summary>
    public static bool HasSimilarLastWordTo(this string source, string target, double similarity_percentage)
    {
        return HasSimilarLastWordTo(source, target, similarity_percentage, false);
    }
    public static bool HasSimilarLastWordTo(this string source, string target, double similarity_percentage, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;

        if (source == target) return true;

        string[] source_words = source.Split();
        string[] target_words = target.Split();
        int source_word_count = source_words.Length;
        int target_word_count = target_words.Length;

        if ((source_word_count > 0) && (target_word_count > 0))
        {
            return (source_words[source_word_count - 1].IsSimilarTo(target_words[target_word_count - 1], similarity_percentage, case_sensitive));
        }
        else
        {
            return false;
        }
    }
    /// <summary> 
    /// returns true if source's first half has similar words to target in any order, else false. 
    /// </summary>
    public static bool HasSimilarFirstHalfTo(this string source, string target, double similarity_percentage)
    {
        return HasSimilarFirstHalfTo(source, target, similarity_percentage, false);
    }
    public static bool HasSimilarFirstHalfTo(this string source, string target, double similarity_percentage, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;

        if (source == target) return true;

        string[] source_words = source.Split();
        string[] target_words = target.Split();
        int source_word_count = source_words.Length;
        int target_word_count = target_words.Length;

        if ((source_word_count > 0) && (target_word_count > 0))
        {
            int half_source_word_count = source_word_count / 2;

            int matching_words = 0;
            for (int i = 0; i < half_source_word_count; i++)
            {
                for (int j = 0; j < target_word_count; j++)
                {
                    if (source_words[i].IsSimilarTo(target_words[j], similarity_percentage))
                    {
                        matching_words++;
                        break;
                    }
                }

                if (matching_words == half_source_word_count)
                {
                    return true;
                }
            }

            // if not enough matching words
            return false;
        }
        else
        {
            return false;
        }
    }
    /// <summary> 
    /// returns true if source's second half has similar words to target in order, else false. 
    /// </summary>
    public static bool HasSimilarLastHalfTo(this string source, string target, double similarity_percentage)
    {
        return HasSimilarLastHalfTo(source, target, similarity_percentage, false);
    }
    public static bool HasSimilarLastHalfTo(this string source, string target, double similarity_percentage, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;

        if (source == target) return true;

        string[] source_words = source.Split();
        string[] target_words = target.Split();
        int source_word_count = source_words.Length;
        int target_word_count = target_words.Length;

        if ((source_word_count > 0) && (target_word_count > 0))
        {
            int half_source_word_count = source_word_count / 2;

            int matching_words = 0;
            for (int i = half_source_word_count; i < source_word_count; i++)
            {
                for (int j = 0; j < target_word_count; j++)
                {
                    if (source_words[i].IsSimilarTo(target_words[j], similarity_percentage))
                    {
                        matching_words++;
                        break;
                    }
                }

                if (matching_words == half_source_word_count)
                {
                    return true;
                }
            }

            // if not enough matching words
            return false;
        }
        else
        {
            return false;
        }
    }

    //Levenshtein's Edit Distance - Converges slowly but with good matches
    /// <summary> 
    /// returns true if source and target strings are at least percentage similar, else false. 
    /// </summary>
    public static bool IsSimilarTo(this string source, string target, double similarity_percentage)
    {
        return source.IsSimilarTo(target, similarity_percentage, false);
    }
    public static bool IsSimilarTo(this string source, string target, double similarity_percentage, bool case_sensitive)
    {
        if (String.IsNullOrEmpty(source)) return false;
        if (String.IsNullOrEmpty(target)) return false;

        if (source == target) return true;

        if (similarity_percentage == 1.0D) return (source == target);

        double similarity = 1.0D;
        double error_margin = 0.001D;
        if (source != target)
        {
            similarity = case_sensitive ? source.SimilarityTo(target) : source.ToLower().SimilarityTo(target.ToLower());
        }
        return (similarity >= (similarity_percentage - error_margin));
    }
    /// <summary> 
    /// returns the percentage similarity between source and target strings. 
    /// </summary>
    public static double SimilarityTo(this string source, string target)
    {
        if (String.IsNullOrEmpty(source)) return 0.0D;
        if (String.IsNullOrEmpty(target)) return 0.0D;

        if (source == target) return 1.0D;

        int steps_to_the_same = ComputeLevenshteinDistance(source, target);
        return (1.0D - ((double)steps_to_the_same / (double)Math.Max(source.Length, target.Length)));
    }
    /// <summary>
    /// returns the number of steps required to transform the source string into the target string. 
    /// Re: http://www.dotnetperls.com/levenshtein
    /// </summary>
    private static int ComputeLevenshteinDistance(string source, string target)
    {
        if (String.IsNullOrEmpty(source)) return 0;
        if (String.IsNullOrEmpty(target)) return 0;

        if (source == target) return source.Length;

        int source_word_count = source.Length;
        int target_word_count = target.Length;
        int[,] distance = new int[source_word_count + 1, target_word_count + 1];

        // Step 1
        if (source_word_count == 0)
        {
            return target_word_count;
        }
        if (target_word_count == 0)
        {
            return source_word_count;
        }

        // Step 2
        for (int i = 0; i <= source_word_count; distance[i, 0] = i++)
        {
        }
        for (int j = 0; j <= target_word_count; distance[0, j] = j++)
        {
        }

        // Step 3
        for (int i = 1; i <= source_word_count; i++)
        {
            //Step 4
            for (int j = 1; j <= target_word_count; j++)
            {
                // Step 5
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                // Step 6
                distance[i, j] = Math.Min(
                    Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);
            }
        }

        // Step 7
        return distance[source_word_count, target_word_count];
    }
}

public static class IntegerExtensions
{
    public static string ToArabic(this int source)
    {
        StringBuilder str = new StringBuilder();
        try
        {
            int number = source;
            string text = number.ToString();
            for (int i = 0; i < text.Length; i++)
            {
                char digit = Constants.INDIAN_DIGITS[number % 10];
                str.Insert(0, digit);
                number /= 10;
            }
            return str.ToString();
        }
        catch
        {
            throw new ArgumentException();
        }
    }
}

public static class ListExtensions
{
    public static bool ItemEquals<T>(this List<T> source, string value)
    {
        return source.ItemEquals(value, StringComparison.Ordinal);
    }
    public static bool ItemEquals<T>(this List<T> source, string value, StringComparison string_comparison)
    {
        foreach (T item in source)
        {
            if (item.ToString() == value)
            {
                return true;
            }
        }
        return false;
    }

    public static bool ItemContains<T>(this List<T> source, string value)
    {
        return source.ItemContains(value, StringComparison.Ordinal);
    }
    public static bool ItemContains<T>(this List<T> source, string value, StringComparison string_comparison)
    {
        foreach (T item in source)
        {
            if (item.ToString().IndexOf(value, string_comparison) != -1)
            {
                return true;
            }
        }
        return false;
    }

    public static bool ItemStartsWith<T>(this List<T> source, string value)
    {
        return source.ItemStartsWith(value, StringComparison.Ordinal);
    }
    public static bool ItemStartsWith<T>(this List<T> source, string value, StringComparison string_comparison)
    {
        foreach (T item in source)
        {
            if (item.ToString().StartsWith(value, string_comparison))
            {
                return true;
            }
        }
        return false;
    }

    public static List<T> RemoveDuplicates<T>(this List<T> source)
    {
        return source.RemoveDuplicates(StringComparison.Ordinal);
    }
    public static List<T> RemoveDuplicates<T>(this List<T> source, StringComparison string_comparison)
    {
        List<T> result = new List<T>();
        foreach (T item in source)
        {
            if (!result.ItemEquals(item.ToString(), string_comparison))
            {
                result.Add(item);
            }
        }
        return result;
    }

    public static List<T> Intersect<T>(this List<T> source, List<T> target)
    {
        List<T> result = new List<T>();
        foreach (T item in source)
        {
            if (target.Contains(item))
            {
                result.Add(item);
            }
        }
        return result;
    }
    public static List<T> Union<T>(this List<T> source, List<T> target)
    {
        List<T> result = new List<T>(source);
        foreach (T item in target)
        {
            if (!source.Contains(item))
            {
                result.Add(item);
            }
        }
        return result;
    }
    public static List<T> ToSet<T>(this List<T> source)
    {
        List<T> result = new List<T>();
        foreach (T item in source)
        {
            if (!result.Contains(item))
            {
                result.Add(item);
            }
        }
        return result;
    }
}
