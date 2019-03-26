using System;
using System.Collections.Generic;

public static class Constants
{
    public static string ORNATE_RIGHT_PARENTHESIS = "\uFD3F";  // ﴿
    public static string ORNATE_LEFT_PARENTHESIS = "\uFD3E";   // ﴾

    public static List<char> INITIAL_LETTERS = new List<char>()
    {
        'ا',
        'ح',
        'ر',
        'س',
        'ص',
        'ط',
        'ع',
        'ق',
        'ك',
        'ل',
        'م',
        'ن',
        'ه',
        'ي'
    };

    public static List<char> NON_INITIAL_LETTERS = new List<char>()
    {
        'ب',
        'ت',
        'ث',
        'ج',
        'خ',
        'د',
        'ذ',
        'ز',
        'ش',
        'ض',
        'ظ',
        'غ',
        'ف',
        'و'
    };

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
        '۩',    // Prostration mark (Obligatory)
        '⌂'     // Prostration mark (Recommended)
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
