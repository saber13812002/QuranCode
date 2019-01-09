using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;

public enum NumberType
{
    None,                   // not a number (eg infinity, -infinity)
    Unit,                   // indivisible by anything
    Prime,                  // divisible by self only (dividing by 1 doesn't divide into smaller parts and is misleading)
    AdditivePrime,          // prime with prime digit sum
    NonAdditivePrime,       // prime with non-prime digit sum
    Composite,              // divisible by self and other(s)
    AdditiveComposite,      // composite with composite digit sum
    NonAdditiveComposite,   // composite with non-composite digit sum
    Odd,                    // indivisible by 2
    Even,                   // divisible by 2
    Square,                 // n*n
    Cubic,                  // n*n*n
    Quartic,                // n*n*n*n
    Quintic,                // n*n*n*n*n
    Sextic,                 // n*n*n*n*n*n          // also called hexic
    Septic,                 // n*n*n*n*n*n*n        // also called heptic
    Octic,                  // n*n*n*n*n*n*n*n
    Nonic,                  // n*n*n*n*n*n*n*n*n
    Decic,                  // n*n*n*n*n*n*n*n*n*n
    Natural                 // natural number from 1 to MaxValue
};
public enum IndexType { Prime, Composite };

// = ≠ ≡ < ≤ > ≥ %
public enum ComparisonOperator { Equal, NotEqual, LessThan, LessOrEqual, GreaterThan, GreaterOrEqual, DivisibleBy, IndivisibleBy, EqualSum, Reserved };

// + - * / %
public enum ArithmeticOperator { Plus, Minus, Multiply, Divide, Modulus };

public static class Numbers
{
    public static Color[] NUMBER_TYPE_COLORS =
    { 
        /* NumberType.None */                   Color.Black,
        /* NumberType.Unit */                   Color.DarkViolet,
        /* NumberType.Prime */                  Color.Black,
        /* NumberType.AdditivePrime */          Color.Blue,
        /* NumberType.NonAdditivePrime */       Color.Green,
        /* NumberType.Composite */              Color.Black,
        /* NumberType.AdditiveComposite */      Color.FromArgb(240,32,32),
        /* NumberType.NonAdditiveComposite */   Color.FromArgb(128,32,32),
        /* NumberType.Odd */                    Color.Black,
        /* NumberType.Even */                   Color.Black,
        /* NumberType.Square */                 Color.Black,
        /* NumberType.Cubic */                  Color.Black,
        /* NumberType.Quartic */                Color.Black,
        /* NumberType.Quintic */                Color.Black,
        /* NumberType.Sextic */                 Color.Black,
        /* NumberType.Septic */                 Color.Black,
        /* NumberType.Octic */                  Color.Black,
        /* NumberType.Nonic */                  Color.Black,
        /* NumberType.Decic */                  Color.Black,
        /* NumberType.Natural */                Color.Black
    };
    public static Color[] NUMBER_TYPE_BACKCOLORS =
    { 
        /* NumberType.None */                   Color.Black,
        /* NumberType.Unit */                   Color.DarkViolet,
        /* NumberType.Prime */                  Color.Green,
        /* NumberType.AdditivePrime */          Color.FromArgb(224, 224, 255),
        /* NumberType.NonAdditivePrime */       Color.FromArgb(240, 255, 240),
        /* NumberType.Composite */              Color.Black,
        /* NumberType.AdditiveComposite */      Color.FromArgb(255, 224, 224),
        /* NumberType.NonAdditiveComposite */   Color.FromArgb(232, 216, 216),
        /* NumberType.Odd */                    Color.Black,
        /* NumberType.Even */                   Color.Black,
        /* NumberType.Square */                 Color.Black,
        /* NumberType.Cubic */                  Color.Black,
        /* NumberType.Quartic */                Color.Black,
        /* NumberType.Quintic */                Color.Black,
        /* NumberType.Sextic */                 Color.Black,
        /* NumberType.Septic */                 Color.Black,
        /* NumberType.Octic */                  Color.Black,
        /* NumberType.Nonic */                  Color.Black,
        /* NumberType.Decic */                  Color.Black,
        /* NumberType.Natural */                Color.Black
    };
    public static Color GetNumberTypeColor(long number)
    {
        return GetNumberTypeColor(number.ToString(), 10L);
    }
    public static Color GetNumberTypeColor(string value, long radix)
    {
        // if negative number, remove -ve sign
        if (value.StartsWith("-")) value = value.Remove(0, 1);

        if (IsUnit(value, radix))
        {
            return NUMBER_TYPE_COLORS[(int)NumberType.Unit];
        }

        else if (IsNonAdditivePrime(value, radix))
        {
            return NUMBER_TYPE_COLORS[(int)NumberType.NonAdditivePrime];
        }
        else if (IsAdditivePrime(value, radix))
        {
            return NUMBER_TYPE_COLORS[(int)NumberType.AdditivePrime];
        }
        else if (IsPrime(value, radix))
        {
            return NUMBER_TYPE_COLORS[(int)NumberType.Prime];
        }

        else if (IsNonAdditiveComposite(value, radix))
        {
            return NUMBER_TYPE_COLORS[(int)NumberType.NonAdditiveComposite];
        }
        else if (IsAdditiveComposite(value, radix))
        {
            return NUMBER_TYPE_COLORS[(int)NumberType.AdditiveComposite];
        }
        else if (IsComposite(value, radix))
        {
            return NUMBER_TYPE_COLORS[(int)NumberType.Composite];
        }

        else
        {
            return NUMBER_TYPE_COLORS[(int)NumberType.None];
        }
    }

    public static Color[] NUMBER_KIND_COLORS =
    { 
        /* NumberKind.Deficient */          Color.FromArgb(240, 208, 255),
        /* NumberKind.Perfect */            Color.FromArgb(240, 128, 255),
        /* NumberKind.Abundant */           Color.FromArgb(240, 178, 255)
    };

    public static int MAX_NUMBER = int.MaxValue / (Globals.EDITION == Edition.Standard ? 1024 : 64);

    // pi = circumference / diameter ~= 355/113
    public const double PI = 3.141592653589793238462643383279D;
    // e = Euler's number = 0SUM∞(1/n!)
    public const double E = 2.718281828459045235360287471352D;
    // phi is the golden ratio = (sqrt(5)+1)/2
    public const double PHI = 1.618033988749894848204586834365D;
    // delta_S is the silver ratio = 1 + sqrt(2)
    public const double DELTA_S = 2.4142135623730950488016887242097D;

    static Numbers()
    {
    }

    public static bool IsNumberType(long number, NumberType number_type)
    {
        switch (number_type)
        {
            case NumberType.Natural:
                {
                    return true;
                }
            case NumberType.Prime:
                {
                    return (IsPrime(number));
                }
            case NumberType.AdditivePrime:
                {
                    return (IsAdditivePrime(number));
                }
            case NumberType.NonAdditivePrime:
                {
                    return (IsNonAdditivePrime(number));
                }
            case NumberType.Composite:
                {
                    return (IsComposite(number));
                }
            case NumberType.AdditiveComposite:
                {
                    return (IsAdditiveComposite(number));
                }
            case NumberType.NonAdditiveComposite:
                {
                    return (IsNonAdditiveComposite(number));
                }
            case NumberType.Odd:
                {
                    return (IsOdd(number));
                }
            case NumberType.Even:
                {
                    return (IsEven(number));
                }
            case NumberType.Square:
                {
                    return (IsSquare(number));
                }
            case NumberType.Cubic:
                {
                    return (IsCubic(number));
                }
            case NumberType.Quartic:
                {
                    return (IsQuartic(number));
                }
            case NumberType.Quintic:
                {
                    return (IsQuintic(number));
                }
            case NumberType.Sextic:
                {
                    return (IsSextic(number));
                }
            case NumberType.Septic:
                {
                    return (IsSeptic(number));
                }
            case NumberType.Octic:
                {
                    return (IsOctic(number));
                }
            case NumberType.Nonic:
                {
                    return (IsNonic(number));
                }
            case NumberType.Decic:
                {
                    return (IsDecic(number));
                }
            case NumberType.None:
            default:
                {
                    return false;
                }
        }
    }
    /// <summary>
    /// Compare two numbers
    /// </summary>
    /// <param name="number1">first number</param>
    /// <param name="number2">second number</param>
    /// <param name="comparison_operator">operator for comparing the two numbers</param>
    /// <param name="remainder">remainder for the % operator. -1 means any remainder</param>
    /// <returns>returns comparison result</returns>
    public static bool Compare(long number1, long number2, ComparisonOperator comparison_operator, int remainder)
    {
        switch (comparison_operator)
        {
            case ComparisonOperator.Equal:
                {
                    return (number1 == number2);
                }
            case ComparisonOperator.NotEqual:
                {
                    return (number1 != number2);
                }
            case ComparisonOperator.LessThan:
                {
                    return (number1 < number2);
                }
            case ComparisonOperator.LessOrEqual:
                {
                    return (number1 <= number2);
                }
            case ComparisonOperator.GreaterThan:
                {
                    return (number1 > number2);
                }
            case ComparisonOperator.GreaterOrEqual:
                {
                    return (number1 >= number2);
                }
            case ComparisonOperator.DivisibleBy:
                {
                    if (number2 == 0) return false;
                    if (remainder == -1) // means any remainder
                    {
                        return ((number1 % number2) != 0);
                    }
                    else
                    {
                        // ignore 0
                        return ((number1 > 0) && (Math.Abs((number1 % number2)) == remainder));
                    }
                }
            case ComparisonOperator.IndivisibleBy:
                {
                    // ignore 0
                    if (number2 == 0) return false;
                    return ((number1 > 0) && (Math.Abs((number1 % number2)) != 0));
                }
            case ComparisonOperator.EqualSum:
                {
                    return (number1 == number2); //??? pass sum in number2
                }
            case ComparisonOperator.Reserved:
            default:
                {
                    return false;
                }
        }
    }
    public static long Reverse(long number)
    {
        long result = 0;
        while (number > 0)
        {
            result = (result * 10L) + (number % 10L);
            number /= 10L;
        }
        return result;
    }
    public static bool IsReverse(long number1, long number2)
    {
        return (number1 == Reverse(number2));
    }
    public static bool AreConsecutive(List<int> numbers)
    {
        if (numbers != null)
        {
            for (int i = 0; i < numbers.Count - 1; i++)
            {
                if (numbers[i + 1] != numbers[i] + 1)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public static bool IsUnit(long number)
    {
        if (number < 0L) number *= -1L;
        return (number == 1L);
    }
    public static bool IsUnit(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsUnit(number);
    }
    public static bool IsOdd(long number)
    {
        if (number < 0L) number *= -1L;
        return ((number % 2) == 1L);
    }
    public static bool IsOdd(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsOdd(number);
    }
    public static bool IsEven(long number)
    {
        if (number < 0L) number *= -1L;
        return ((number % 2) == 0L);
    }
    public static bool IsEven(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsEven(number);
    }
    // http://digitalbush.com/2010/02/26/sieve-of-eratosthenes-in-csharp/

    //IList<int> FindPrimes(int max)
    //{
    //    var result = new List<int>((int)(max / (Math.Log(max) - 1.08366)));
    //    var maxSquareRoot = Math.Sqrt(max);
    //    var eliminated = new System.Collections.BitArray(max + 1);
    //    result.Add(2);
    //    for (int i = 3; i <= max; i += 2)
    //    {
    //        if (!eliminated[i])
    //        {
    //            if (i < maxSquareRoot)
    //            {
    //                for (int j = i * i; j <= max; j += 2 * i)
    //                    eliminated[j] = true;
    //            }
    //            result.Add(i);
    //        }
    //    }
    //    return result;
    //}

    // Algorithm Optimizations
    // I cut my work in half by treating the special case of '2'.
    // We know that 2 is prime and all even numbers thereafter are not.
    // So, we'll add two immediately and then start looping at 3 only checking odd numbers from there forward.

    // After we've found a prime, we only need to eliminate numbers from it's square and forward.
    // Let's say we want to find all prime numbers up to 100 and we've just identified 7 as a prime.
    // Per the algorithm, I'll need to eliminate 2*7, 3*7 ,4*7, 5*7, 6*7, 7*7 ,8*7 ,9*7, 10*7 ,11*7, 12*7 ,13*7 and 14*7.
    // None of the even multiples matter (even times an odd is always even) and none of the multiples
    // up to the square of the prime matter since we've already done those multiples in previous loops.
    // So really we only have to eliminate 7*7, 9*7, 11*7 and 13*7.
    // That's a 9 fewer iterations and those savings become more fruitful the deeper you go!

    // The last optimization is the square root calculation and check.
    // We know from above that we only need to start eliminating beginning at the square of the current prime.
    // Therefore it also makes sense that we can stop even trying once we get past the to square root of the max.
    // This saves a bunch more iterations.

    // Language Optimizations
    // Originally I had started by returning an IEnumerable<int>.
    // I wasn't using the list you see above and instead I was using yield return i.
    // I really like that syntax, but once I got to the VB.net version (Coming Soon!),
    // I didn't have a direct translation for the yield keyword.
    // I took the lazy route in the VB version and just stuffed it all into a list and returned that.
    // To my surprise it was faster! I went back and changed the C# version above and it performed better.
    // I'm not sure why, but I'm going with it.

    // What do you think that you get when do a sizeof(bool) in C#?
    // I was surprised to find out that my trusty booleans actually take up a whole byte instead of a single bit.
    // I speculate that there is a performance benefit that all of your types fit into a byte level offset in memory.
    // I was thrilled to find out that we have a BitArray class that is useful for situations above
    // when you need to store a lot of booleans and you need them to only take up a bit in memory.
    // I'm not sure it helped anything, but I feel better knowing I'm using the least amount of memory possible.

    // Conclusion
    // Despite the fact that I know C# really well, I'm very thrilled that I was able to learn a few things about the language.
    // Also, I'm really happy with the performance of this reference implementation.
    // On my machine (2.66 GHz Core2 Duo and 2 GB of RAM) I can find all of the primes under 1,000,000 in 19ms.
    // I think I've squeezed all I can out of this version.
    // Please let me know if you see something I missed or did wrong and I'll make adjustments.

    // EDIT: I just added one more optimization that's worth noting.
    // Instead of constructing my list with an empty constructor, I can save a several milliseconds 
    // off the larger sets by specifying a start size of the internal array structure behind the list.
    // If I set this size at or slightly above the end count of prime numbers,
    // then I avoid a lot of costly array copying as the array bounds keep getting hit.
    // It turns out that there is quite a bit of math involved in accurately predicting the number of primes underneath a given number.
    // I chose to cheat and just use Legendre's constant with the Prime Number Theorem which is close enough for my purposes.
    // I can now calculate all primes under 1,000,000 in 10ms on my machine. Neat!
    //private static List<int> GeneratePrimesUsingSieveOfEratosthenes(int limit)
    //{
    //    // guard against parameter out of range
    //    if (limit < 2)
    //    {
    //        return new List<int>();
    //    }

    //    // Legendre's constant to approximate the number of primes below N
    //    int max_primes = (int)Math.Ceiling((limit / (Math.Log(limit) - 1.08366)));
    //    if (max_primes < 1)
    //    {
    //        max_primes = 1;
    //    }
    //    List<int> primes = new List<int>(max_primes);

    //    // bit array to cross out multiples of primes successively
    //    BitArray candidates = new BitArray(limit + 1, true);

    //    // add number 2 as prime
    //    primes.Add(2);
    //    // and cross out all its multiples
    //    for (int j = 2 * 2; j <= limit; j += 2)
    //    {
    //        candidates[j] = false;
    //    }

    //    // get the ceiling of sqrt of N
    //    int limit_sqrt = (int)Math.Ceiling(Math.Sqrt(limit));

    //    // start from 3 and skip even numbers
    //    // don't go beyond limit or overflow into negative
    //    for (int i = 3; (i > 0 && i <= limit); i += 2)
    //    {
    //        if (candidates[i])
    //        {
    //            // add not-crossed out candidate
    //            primes.Add(i);

    //            // upto the sqrt of N
    //            if (i <= limit_sqrt)
    //            {
    //                // and cross out non-even multiples from i*i and skip even i multiples
    //                // don't go beyond limit, or overflow into negative
    //                for (int j = i * i; (j > 0 && j <= limit); j += 2 * i)
    //                {
    //                    candidates[j] = false;
    //                }
    //            }
    //        }
    //    }

    //    return primes;
    //}
    //private static List<int> GeneratePrimesUsingDivisionTrial(int limit)
    //{
    //    // guard against parameter out of range
    //    if (limit < 2)
    //    {
    //        return new List<int>();
    //    }

    //    // Legendre's constant to approximate the number of primes below N
    //    int max_primes = (int)Math.Ceiling((limit / (Math.Log(limit) - 1.08366)));
    //    if (max_primes < 1)
    //    {
    //        max_primes = 1;
    //    }
    //    List<int> primes = new List<int>(max_primes);

    //    primes.Add(2);

    //    for (int i = 3; i <= limit; i += 2)
    //    {
    //        bool is_prime = true;
    //        for (int j = 3; j <= (int)Math.Sqrt(i); j += 2)
    //        {
    //            if (i % j == 0)
    //            {
    //                is_prime = false;
    //                break;
    //            }
    //        }

    //        if (is_prime)
    //        {
    //            primes.Add(i);
    //        }
    //    }

    //    return primes;
    //}
    public static bool IsPrime(long number)
    {
        if (number < 0L) number *= -1L;

        if (number == 0L)        // 0 is neither prime nor composite
            return false;

        if (number == 1L)        // 1 is the unit, indivisible
            return false;        // NOT prime

        if (number == 2L)        // 2 is the first prime
            return true;

        if (number % 2L == 0L)   // exclude even numbers to speed up search
            return false;

        long sqrt = (long)Math.Round(Math.Sqrt(number));
        for (long i = 3L; i <= sqrt; i += 2L)
        {
            if ((number % i) == 0L)
            {
                return false;
            }
        }
        return true;
    }
    public static bool IsPrime(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsPrime(number);
    }
    public static bool IsAdditivePrime(long number)
    {
        if (IsPrime(number))
        {
            return IsPrime(DigitSum(number));
        }
        return false;
    }
    public static bool IsAdditivePrime(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsAdditivePrime(number);
    }
    public static bool IsNonAdditivePrime(long number)
    {
        if (IsPrime(number))
        {
            return !IsPrime(DigitSum(number));
        }
        return false;
    }
    public static bool IsNonAdditivePrime(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsNonAdditivePrime(number);
    }
    public static bool IsComposite(long number)
    {
        if (number < 0L) number *= -1L;

        if (number == 0L)        // 0 is NOT composite
            return false;

        if (number == 1L)        // 1 is the unit, indivisible
            return false;        // NOT composite

        if (number == 2L)        // 2 is the first prime
            return false;

        if (number % 2L == 0L)   // even numbers are composite
            return true;

        long sqrt = (long)Math.Round(Math.Sqrt(number));
        for (long i = 3L; i <= sqrt; i += 2L)
        {
            if ((number % i) == 0L)
            {
                return true;
            }
        }
        return false;
    }
    public static bool IsComposite(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsComposite(number);
    }
    public static bool IsAdditiveComposite(long number)
    {
        if (IsComposite(number))
        {
            return IsComposite(DigitSum(number));
        }
        return false;
    }
    public static bool IsAdditiveComposite(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsAdditiveComposite(number);
    }
    public static bool IsNonAdditiveComposite(long number)
    {
        if (IsComposite(number))
        {
            return !IsComposite(DigitSum(number));
        }
        return false;
    }
    public static bool IsNonAdditiveComposite(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return IsNonAdditiveComposite(number);
    }
    /// <summary>
    /// Check if three numbers are additive primes and their L2R and R2L concatenations are additive primes too.
    /// <para>Example:</para>
    /// <para>Quran chapter The Key has:</para>
    /// <para>(7, 29, 139) are primes with primes digit sums (7=7, 2+9=11, 1+3+9=13)</para>
    /// <para>and 729139, 139297 primes with prime digit sum (1+3+9+2+9+7=31)</para>
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <param name="n3"></param>
    /// <returns></returns>
    public static bool ArePrimeTriplets(string value1, string value2, string value3, long radix)
    {
        long number1 = Radix.Decode(value1, radix);
        long number2 = Radix.Decode(value2, radix);
        long number3 = Radix.Decode(value3, radix);
        return ArePrimeTriplets(number1, number2, number3);
    }
    public static bool ArePrimeTriplets(long number1, long number2, long number3)
    {
        if (
            IsAdditivePrime(number1)
            &&
            IsAdditivePrime(number2)
            &&
            IsAdditivePrime(number3)
            )
        {
            try
            {
                long l2r = long.Parse(number1.ToString() + number2.ToString() + number3.ToString());
                long r2l = long.Parse(number3.ToString() + number2.ToString() + number1.ToString());
                if (
                    IsAdditivePrime(l2r)
                    &&
                    IsAdditivePrime(r2l)
                    )
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        return false;
    }
    public static List<int> SieveOfEratosthenes(int limit)
    {
        // guard against parameter out of range
        if ((limit < 2) || (limit > (int)(int.MaxValue * 0.9999999)))
        {
            return new List<int>();
        }

        // Legendre's constant to approximate the number of primes below N
        int max_primes = (int)Math.Ceiling((limit / (Math.Log(limit) - 1.08366)));
        if (max_primes < 1)
        {
            max_primes = 1;
        }
        List<int> primes = new List<int>(max_primes);

        // bit array to cross out multiples of primes successively
        // from N^2, jumping 2N at a time (to skip even multiples)
        BitArray candidates = new BitArray(limit + 1, true);

        // add number 2 as prime
        primes.Add(2);
        //// no need to cross out evens as we are skipping them anyway
        //// and cross out all its multiples
        //for (int j = 2 * 2; j <= limit; j += 2)
        //{
        //    candidates[j] = false;
        //}

        // get the ceiling of sqrt of N
        int sqrt_of_limit = (int)Math.Ceiling(Math.Sqrt(limit));

        // start from 3 and skip even numbers
        // don't go beyond limit or overflow into negative
        for (int i = 3; (i > 0 && i <= limit); i += 2)
        {
            // if not-crossed out candidate yet
            if (candidates[i])
            {
                // add candidate
                primes.Add(i);

                // upto the sqrt of N
                if (i <= sqrt_of_limit)
                {
                    // and cross out non-even multiples from i*i and skip even i multiples
                    // don't go beyond limit, or overflow into negative
                    for (int j = i * i; (j > 0 && j <= limit); j += 2 * i)
                    {
                        candidates[j] = false;
                    }
                }
            }
        }
        return primes;
    }

    public static int GetDigitValue(char c)
    {
        int result = -1;
        if (Char.IsDigit(c)) // 0..9
        {
            result = (int)char.GetNumericValue(c);
        }
        else // A..Z
        {
            result = c.CompareTo('A') + 10;
        }
        return result;
    }
    public static List<int> GetDigits(long number)
    {
        List<int> result = new List<int>();
        string str = number.ToString();
        for (int i = 0; i < str.Length; i++)
        {
            result.Add((int)Char.GetNumericValue(str[i]));
        }
        return result;
    }
    public static List<char> GetDigits(string value)
    {
        List<char> result = new List<char>();
        if (value.Length > 0)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (Char.IsDigit(c))
                {
                    result.Add(value[i]);
                }
            }
        }
        return result;
    }
    public static int DigitCount(long number)
    {
        return DigitCount(number.ToString());
    }
    public static int DigitCount(string value)
    {
        return DigitCount(value, 10L);
        //int result = 0;
        //if (value.Length > 0)
        //{
        //    for (int i = 0; i < value.Length; i++)
        //    {
        //        char c = value[i];
        //        if (Char.IsDigit(c))
        //        {
        //            result++;
        //        }
        //    }
        //}
        //return result;
    }
    public static int DigitCount(long number, long radix)
    {
        return DigitCount(number.ToString(), radix);
    }
    public static int DigitCount(string value, long radix)
    {
        int result = 0;
        if (value.Length > 0)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (((c >= ('0')) && (c < ('9' - 9 + radix))) || ((c >= ('A')) && (c < ('A' - 10 + radix))))
                {
                    result++;
                }
            }
        }
        return result;
    }

    //TODO versions with radix
    public static int DigitSum(long number)
    {
        return DigitSum(number.ToString());
    }
    public static int DigitSum(string value)
    {
        int result = 0;
        if (value.Length > 0)
        {
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (Char.IsDigit(c))
                {
                    result += GetDigitValue(c);
                }
            }
        }
        return result;
    }
    public static int DigitalRoot(long number)
    {
        return DigitalRoot(number.ToString());
    }
    public static int DigitalRoot(string value)
    {
        int result = DigitSum(value);
        while (result.ToString().Length > 1)
        {
            result = DigitSum(result);
        }
        return result;
    }

    public static bool IsDigitsOnly(string text)
    {
        foreach (char c in text)
        {
            if (!Char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }
    private static List<char> s_odd_digits = new List<char> { '1', '3', '5', '7', '9' };
    private static List<char> s_even_digits = new List<char> { '0', '2', '4', '6', '8' };
    private static List<char> s_prime_digits = new List<char> { '2', '3', '5', '7' };
    private static List<char> s_composite_digits = new List<char> { '4', '6', '8', '9' };
    public static List<char> OddDigits
    {
        get { return s_odd_digits; }
    }
    public static List<char> EvenDigits
    {
        get { return s_even_digits; }
    }
    public static List<char> PrimeDigits
    {
        get { return s_prime_digits; }
    }
    public static List<char> CompositeDigits
    {
        get { return s_composite_digits; }
    }
    public static bool IsOddDigits(long number)
    {
        return IsOddDigits(number.ToString());
    }
    public static bool IsOddDigits(string value)
    {
        foreach (char c in value)
        {
            if (Char.IsDigit(c))
            {
                if (!s_odd_digits.Contains(c))
                {
                    return false;
                }
            }
        }
        return true;
    }
    public static bool IsEvenDigits(long number)
    {
        return IsEvenDigits(number.ToString());
    }
    public static bool IsEvenDigits(string value)
    {
        foreach (char c in value)
        {
            if (Char.IsDigit(c))
            {
                if (!s_even_digits.Contains(c))
                {
                    return false;
                }
            }
        }
        return true;
    }
    public static bool IsPrimeDigits(long number)
    {
        return IsPrimeDigits(number.ToString());
    }
    public static bool IsPrimeDigits(string value)
    {
        foreach (char c in value)
        {
            if (Char.IsDigit(c))
            {
                if (!s_prime_digits.Contains(c))
                {
                    return false;
                }
            }
        }
        return true;
    }
    public static bool IsPrimeOr1Digits(long number)
    {
        return IsPrimeOr1Digits(number.ToString());
    }
    public static bool IsPrimeOr1Digits(string value)
    {
        foreach (char c in value)
        {
            if (Char.IsDigit(c))
            {
                if ((c != '1') && !s_prime_digits.Contains(c))
                {
                    return false;
                }
            }
        }
        return true;
    }
    public static bool IsCompositeDigits(long number)
    {
        return IsCompositeDigits(number.ToString());
    }
    public static bool IsCompositeDigits(string value)
    {
        foreach (char c in value)
        {
            if (Char.IsDigit(c))
            {
                if (!s_composite_digits.Contains(c))
                {
                    return false;
                }
            }
        }
        return true;
    }
    public static bool IsCompositeOr0Digits(long number)
    {
        return IsCompositeOr0Digits(number.ToString());
    }
    public static bool IsCompositeOr0Digits(string value)
    {
        foreach (char c in value)
        {
            if (Char.IsDigit(c))
            {
                if ((c != '0') && !s_composite_digits.Contains(c))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static List<long> s_primes = null;
    private static List<long> s_additive_primes = null;
    private static List<long> s_non_additive_primes = null;
    public static List<long> Primes
    {
        get
        {
            if (s_primes == null)
            {
                GeneratePrimes(MAX_NUMBER);
            }
            return s_primes;
        }
    }
    public static List<long> AdditivePrimes
    {
        get
        {
            if (s_additive_primes == null)
            {
                GenerateAdditivePrimes(MAX_NUMBER);
            }
            return s_additive_primes;
        }
    }
    public static List<long> NonAdditivePrimes
    {
        get
        {
            if (s_non_additive_primes == null)
            {
                GenerateNonAdditivePrimes(MAX_NUMBER);
            }
            return s_non_additive_primes;
        }
    }
    public static int PrimeIndexOf(long number)
    {
        if (number < 0L) number *= -1L;

        if (IsPrime(number))
        {
            if (s_primes == null)
            {
                GeneratePrimes(MAX_NUMBER);
            }
            return BinarySearch(s_primes, number);

            //int index = -1;
            //int max = s_max_number_limit;
            //while ((index = BinarySearch(s_primes, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GeneratePrimes(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int AdditivePrimeIndexOf(long number)
    {
        if (number < 0L) number *= -1L;

        if (IsAdditivePrime(number))
        {
            if (s_additive_primes == null)
            {
                GenerateAdditivePrimes(MAX_NUMBER);
            }
            return BinarySearch(s_additive_primes, number);

            //int index = -1;
            //int max = s_max_number_limit;
            //while ((index = BinarySearch(s_additive_primes, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GenerateAdditivePrimes(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int NonAdditivePrimeIndexOf(long number)
    {
        if (number < 0L) number *= -1L;

        if (IsNonAdditivePrime(number))
        {
            if (s_non_additive_primes == null)
            {
                GenerateNonAdditivePrimes(MAX_NUMBER);
            }
            return BinarySearch(s_non_additive_primes, number);

            //int index = -1;
            //int max = s_max_number_limit;
            //while ((index = BinarySearch(s_non_additive_primes, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GenerateNonAdditivePrimes(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int PrimeIndexOf(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return PrimeIndexOf(number);
    }
    public static int AdditivePrimeIndexOf(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return AdditivePrimeIndexOf(number);
    }
    public static int NonAdditivePrimeIndexOf(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return NonAdditivePrimeIndexOf(number);
    }
    private static void GeneratePrimes(int max)
    {
        //if (s_primes != null)
        //{
        //    int primes_upto_max = (int)(max / (Math.Log(max) + 1));
        //    if (s_primes.Count >= primes_upto_max)
        //    {
        //        return; // we already have a large list, no need to RE-generate new one
        //    }
        //}

        if (s_primes == null)
        {
            BitArray composites = new BitArray(max + 1);

            s_primes = new List<long>();

            s_primes.Add(2L);

            // process odd numbers // 3, 5, 7, 9, 11, ..., max
            long sqrt = (long)Math.Round(Math.Sqrt(max)) + 1L;
            for (int i = 3; i <= max; i += 2)
            {
                if (!composites[i])
                {
                    s_primes.Add(i);

                    // mark off multiples of i starting from i*i and skipping even "i"s
                    if (i < sqrt)
                    {
                        for (int j = i * i; j <= max; j += 2 * i)
                        {
                            composites[j] = true;
                        }
                    }
                }
            }
        }
    }
    private static void GenerateAdditivePrimes(int max)
    {
        //// re-generate for new max if larger
        //GeneratePrimes(max);

        if (s_additive_primes == null)
        {
            if (s_primes == null)
            {
                GeneratePrimes(max);
            }

            if (s_primes != null)
            {
                s_additive_primes = new List<long>();
                int count = s_primes.Count;
                for (int i = 0; i < count; i++)
                {
                    if (IsPrime(DigitSum(s_primes[i])))
                    {
                        s_additive_primes.Add(s_primes[i]);
                    }
                }
            }
        }
    }
    private static void GenerateNonAdditivePrimes(int max)
    {
        //// re-generate for new max if larger
        //GeneratePrimes(max);

        if (s_non_additive_primes == null)
        {
            if (s_primes == null)
            {
                GeneratePrimes(max);
            }

            if (s_primes != null)
            {
                s_non_additive_primes = new List<long>();
                int count = s_primes.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!IsPrime(DigitSum(s_primes[i])))
                    {
                        s_non_additive_primes.Add(s_primes[i]);
                    }
                }
            }
        }
    }

    private static List<long> s_composites = null;
    private static List<long> s_additive_composites = null;
    private static List<long> s_non_additive_composites = null;
    public static List<long> Composites
    {
        get
        {
            if (s_composites == null)
            {
                GenerateComposites(MAX_NUMBER / 2);
            }
            return s_composites;
        }
    }
    public static List<long> AdditiveComposites
    {
        get
        {
            if (s_additive_composites == null)
            {
                GenerateAdditiveComposites(MAX_NUMBER / 2);
            }
            return s_additive_composites;
        }
    }
    public static List<long> NonAdditiveComposites
    {
        get
        {
            if (s_non_additive_composites == null)
            {
                GenerateNonAdditiveComposites(MAX_NUMBER / 2);
            }
            return s_non_additive_composites;
        }
    }
    public static int CompositeIndexOf(long number)
    {
        if (number < 0L) number *= -1L;

        if (IsComposite(number))
        {
            int max = MAX_NUMBER / 2;
            if (s_composites == null)
            {
                GenerateComposites(max);
            }
            return BinarySearch(s_composites, number);

            //int index = -1;
            //while ((index = BinarySearch(s_composites, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GenerateComposites(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int AdditiveCompositeIndexOf(long number)
    {
        if (number < 0L) number *= -1L;

        if (IsAdditiveComposite(number))
        {
            int max = MAX_NUMBER / 2;
            if (s_additive_composites == null)
            {
                GenerateAdditiveComposites(max);
            }
            return BinarySearch(s_additive_composites, number);

            //int index = -1;
            //while ((index = BinarySearch(s_additive_composites, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GenerateAdditiveComposites(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int NonAdditiveCompositeIndexOf(long number)
    {
        if (number < 0L) number *= -1L;

        if (IsNonAdditiveComposite(number))
        {
            int max = MAX_NUMBER / 2;
            if (s_non_additive_composites == null)
            {
                GenerateNonAdditiveComposites(max);
            }
            return BinarySearch(s_non_additive_composites, number);

            //int index = -1;
            //while ((index = BinarySearch(s_non_additive_composites, number)) == -1)
            //{
            //    if (max > (int.MaxValue / 32)) break;
            //    max *= 2;
            //    GenerateNonAdditiveComposites(max);
            //}
            //return index;
        }
        return -1;
    }
    public static int CompositeIndexOf(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return CompositeIndexOf(number);
    }
    public static int AdditiveCompositeIndexOf(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return AdditiveCompositeIndexOf(number);
    }
    public static int NonAdditiveCompositeIndexOf(string value, long radix)
    {
        long number = Radix.Decode(value, radix);
        return NonAdditiveCompositeIndexOf(number);
    }
    private static void GenerateComposites(int max)
    {
        //if (s_composites != null)
        //{
        //    int primes_upto_max = (int)(max / (Math.Log(max) + 1));
        //    if (s_composites.Count >= (max - primes_upto_max))
        //    {
        //        return; // we already have a large list, no need to RE-generate new one
        //    }
        //}

        if (s_composites == null)
        {
            BitArray composites = new BitArray(max + 1);

            s_composites = new List<long>(max);

            for (int i = 4; i <= max; i += 2)
            {
                composites[i] = true;
            }

            // process odd numbers // 3, 5, 7, 9, 11, ..., max
            long sqrt = (long)Math.Round(Math.Sqrt(max)) + 1L;
            for (int i = 3; i <= max; i += 2)
            {
                if (!composites[i])
                {
                    // mark off multiples of i
                    if (i <= sqrt)
                    {
                        for (int j = i * i; j <= max; j += 2 * i)
                        {
                            composites[j] = true;
                        }
                    }
                }
            }

            for (int i = 4; i <= max; i++)
            {
                if (composites[i])
                {
                    s_composites.Add(i);
                }
            }
        }
    }
    private static void GenerateAdditiveComposites(int max)
    {
        //// re-generate for new max if larger
        //GenerateComposites(max);

        if (s_additive_composites == null)
        {
            if (s_composites == null)
            {
                GenerateComposites(max);
            }

            if (s_composites != null)
            {
                s_additive_composites = new List<long>();
                int count = s_composites.Count;
                for (int i = 0; i < count; i++)
                {
                    if (IsComposite(DigitSum(s_composites[i])))
                    {
                        s_additive_composites.Add(s_composites[i]);
                    }
                }
            }
        }
    }
    private static void GenerateNonAdditiveComposites(int max)
    {
        //// re-generate for new max if larger
        //GenerateComposites(max);

        if (s_non_additive_composites == null)
        {
            if (s_composites == null)
            {
                GenerateComposites(max);
            }

            if (s_composites != null)
            {
                s_non_additive_composites = new List<long>();
                int count = s_composites.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!IsComposite(DigitSum(s_composites[i])))
                    {
                        s_non_additive_composites.Add(s_composites[i]);
                    }
                }
            }
        }
    }

    public static bool IsSquare(long number)
    {
        return (IsNthPower(number, 2));
    }
    public static bool IsCubic(long number)
    {
        return (IsNthPower(number, 3));
    }
    public static bool IsQuartic(long number)
    {
        return (IsNthPower(number, 4));
    }
    public static bool IsQuintic(long number)
    {
        return (IsNthPower(number, 5));
    }
    public static bool IsSextic(long number)
    {
        return (IsNthPower(number, 6));
    }
    public static bool IsSeptic(long number)
    {
        return (IsNthPower(number, 7));
    }
    public static bool IsOctic(long number)
    {
        return (IsNthPower(number, 8));
    }
    public static bool IsNonic(long number)
    {
        return (IsNthPower(number, 9));
    }
    public static bool IsDecic(long number)
    {
        return (IsNthPower(number, 10));
    }
    public static bool IsNthPower(long number, int power)
    {
        long root = (long)Math.Round(Math.Pow(number, (1.0D / power)));
        long total = 1L;
        for (int i = 0; i < power; i++)
        {
            total *= root;
        }
        return (number == total);
    }

    /// <summary>
    /// Factorize a number into its prime factors.
    /// </summary>
    /// <param name="number">A number to factorize.</param>
    /// <returns>Return all prime factors (including repeated ones).</returns>
    public static List<long> Factorize(long number)
    {
        List<long> result = new List<long>();

        if (number < 0L)
        {
            result.Add(-1L);
            number *= -1L;
        }

        if ((number >= 0L) && (number <= 2L))
        {
            result.Add(number);
        }
        else // if (number > 2L)
        {
            // if number has a prime factor add it to factors,
            // number /= p,
            // reloop until  number == 1L
            while (number != 1L)
            {
                if ((number % 2L) == 0L) // if even number
                {
                    result.Add(2L);
                    number /= 2L;
                }
                else // trial divide by all primes upto sqrt(number)
                {
                    long max = (long)Math.Round(Math.Sqrt(number)) + 1L;	// extra 1 for double calculation errors

                    bool is_factor_found = false;
                    for (long i = 3L; i <= max; i += 2L)
                    {
                        if ((number % i) == 0L)
                        {
                            is_factor_found = true;
                            result.Add(i);
                            number /= i;
                            break; // for loop, reloop while
                        }
                    }

                    // if no prime factor found the number must be prime in the first place
                    if (!is_factor_found)
                    {
                        result.Add(number);
                        break; // while loop
                    }
                }
            }
        }

        s_factors = result;
        return result;
    }
    /// <summary>
    /// Factorize a number into its prime factors.
    /// </summary>
    /// <param name="number">A number to factorize.</param>
    /// <returns>Return all prime factors and their powers.</returns>
    public static Dictionary<long, int> FactorizeByPowers(long number)
    {
        Dictionary<long, int> result = new Dictionary<long, int>();

        int power = 0;
        List<long> factors = Factorize(number);
        result.Add(factors[0], power);
        foreach (long factor in factors)
        {
            if (!result.ContainsKey(factor)) // new factor
            {
                power = 1;
                result.Add(factor, power);
            }
            else // update existing factor
            {
                power++;
                result[factor] = power;
            }
        }

        s_factor_powers = result;
        return result;
    }
    /// <summary>
    /// Get a multiplication string of a number's prime factors.
    /// </summary>
    /// <param name="number">A number to factorize.</param>
    /// <returns></returns>
    public static string FactorizeToString(long number)
    {
        StringBuilder str = new StringBuilder();
        List<long> factors = Factorize(number);
        if (factors != null)
        {
            if (factors.Count > 0)
            {
                foreach (long factor in factors)
                {
                    str.Append(factor.ToString() + "×");
                }
                if (str.Length > 1)
                {
                    str.Remove(str.Length - 1, 1);
                }
            }
        }
        return str.ToString();
    }

    private static List<long> s_factors = null;
    private static Dictionary<long, int> s_factor_powers = null;

    public static int BinarySearch(IList<long> sorted_list, long number)
    {
        if (sorted_list == null) return -1;
        if (sorted_list.Count < 1) return -1;

        int min = 0;
        int max = sorted_list.Count - 1;
        int old_mid = -1;
        int mid;
        while ((mid = (min + max) / 2) != old_mid)
        {
            if (number == sorted_list[min]) { return min; }

            if (number == sorted_list[max]) { return max; }

            if (number == sorted_list[mid]) { return mid; }
            else if (number < sorted_list[mid]) { max = mid; }
            else /*if (number > sorted_list[mid])*/ { min = mid; }

            old_mid = mid;
        }

        return -1;
    }
    public static void QuickSort(IList<long> list, int min, int max)
    {
        if (list == null) return;
        if (list.Count < 1) return;
        if (min > max) return;
        if ((min < 0) || (max >= list.Count)) return;

        int lo = min;
        int hi = max;
        long mid = list[(lo + hi) / 2];	// uses copy constructor

        do
        {
            while (list[lo] < mid)		// uses comparison operator
                lo++;
            while (mid < list[hi])
                hi--;

            if (lo <= hi)
            {
                long temp = list[hi];
                list[hi] = list[lo];
                list[hi] = temp;
                lo++;
                hi--;
            }
        }
        while (lo <= hi);

        if (hi > min)
            QuickSort(list, min, hi);
        if (lo < max)
            QuickSort(list, lo, max);
    }
}
