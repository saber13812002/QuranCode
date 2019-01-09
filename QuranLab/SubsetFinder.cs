// Yorye Nathan on 03-May-2015
// http://stackoverflow.com/questions/30006497/find-all-k-size-subsets-with-sum-s-of-an-n-size-bag-of-duplicate-unsorted-positi/30012781#30012781
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

public class SubsetFinder
{
    public const string DATA_FOLDER = "Data";
    public const string METADATA_FILE = "quran-lab.txt";

    private readonly int[] m_tail_sums = null;
    public SubsetFinder(NumberQuery query)
    {
        m_query = query;

        if (!Book.Initialized)
        {
            string path = DATA_FOLDER + "/" + METADATA_FILE;
            Book.Initialize(path);
        }
        if (Book.Initialized)
        {
            // Sort Chapters descendingly by Number
            Array.Sort(Book.Chapters, (a, b) => b.Number.CompareTo(a.Number));

            // Save tail-sums to allow immediate access by index
            m_tail_sums = new int[Book.CHAPTERS + 1];
            int sum = 0;
            for (int i = Book.CHAPTERS - 1; i >= 0; i--)
            {
                sum += Book.Chapters[i].Number;
                m_tail_sums[i] = sum;
            }
        }
    }

    // find subsets
    /// <summary>
    /// Find all subsets where the sum of numbers in a subset equals the target sum.
    /// </summary>
    /// <param name="count">count of items in subsets to be found</param>
    /// <param name="sum">target sum</param>
    /// <param name="subset">subset to evaluate</param>
    /// <param name="callback">method to call when a new subset is found</param>
    public void Find(long sum, Action<Chapter[]> callback)
    {
        for (int count = 1; count <= Book.CHAPTERS; count++)
            this.Find(count, sum, callback);
    }
    /// <summary>
    /// Find all subsets with specified count regardless of sum.
    /// </summary>
    /// <param name="count">count of items in subsets to be found</param>
    /// <param name="subset">subset to evaluate</param>
    /// <param name="callback">method to call when a new subset is found</param>
    public void Find(int count, Action<Chapter[]> callback)
    {
        if (count > Book.CHAPTERS)
            return;

        this.Scan(0, count, new List<Chapter>(), callback);
    }
    /// <summary>
    /// Find all subsets with specified count where the sum of numbers in a subset equals the target sum.
    /// </summary>
    /// <param name="count">count of items in subsets to be found</param>
    /// <param name="sum">target sum</param>
    /// <param name="subset">subset to evaluate</param>
    /// <param name="callback">method to call when a new subset is found</param>
    public void Find(int count, long sum, Action<Chapter[]> callback)
    {
        if (count > Book.CHAPTERS)
            return;

        if ((count > 0) && (sum > 0))
        {
            this.Scan(0, count, sum, new List<Chapter>(), callback);
        }
        else // * means any value
        {
            if ((count > 0) && (sum == 0))
            {
                this.Scan(0, count, new List<Chapter>(), callback);
            }
            else if ((count == 0) && (sum > 0))
            {
                for (int i = 0; i < Book.CHAPTERS; i++)
                {
                    this.Scan(0, i + 1, sum, new List<Chapter>(), callback);
                }
            }
            else if ((count == 0) && (sum == 0))
            {
                for (int i = 0; i < Book.CHAPTERS; i++)
                {
                    this.Scan(0, i + 1, new List<Chapter>(), callback);
                }
            }
        }
    }
    private void Scan(int index, int count, long sum, List<Chapter> chapters, Action<Chapter[]> callback)
    {
        // No more chapters to add.
        // Current subset is guranteed to be valid
        if (count == 0)
        {
            // Callback with current subset
            Chapter[] subset = chapters.ToArray();
            if (IsValid(subset))
            {
                Application.DoEvents();
                if (!MainForm.Running) return;

                callback(subset);
            }
            return;
        }

        // Save the smallest remaining sum
        int start_index = Book.CHAPTERS - count;
        int tail_sum = m_tail_sums[start_index];

        // Smallest possible sum is greater than target sum,
        // so a valid subset cannot be found
        if (tail_sum > sum)
        {
            return;
        }

        // Find largest number that satisfies the condition that a valid subset can be found
        tail_sum -= Book.Chapters[start_index].Number;
        // And remember the last index that satisfies the condition
        int last_index = start_index;
        while ((start_index > index) && (tail_sum + Book.Chapters[start_index - 1].Number <= sum))
        {
            start_index--;
        }

        //// Find the first number in the sorted chapters that is the largest number we just found
        //// (in case of duplicates)
        //while ((start_index > index) && (Chapters[start_index] == Chapters[start_index - 1]))
        //{
        //    start_index--;
        //}

        // [start_index .. last_index] is the full range we must check in recursion
        for (int i = start_index; i <= last_index; i++)
        {
            // Find the largest possible sum, which is the sum of the first k chapters
            // starting at current start_index
            int max_sum = m_tail_sums[i] - m_tail_sums[i + count];

            // The largest possible sum is less than the sum, so a valid subset cannot be found
            if (max_sum < sum)
            {
                return;
            }

            // Add current chapter to the subset
            Chapter chapter = Book.Chapters[i];
            chapters.Add(chapter);

            // Recurse through the sub-problem to the right
            this.Scan(i + 1, count - 1, sum - chapter.Number, chapters, callback);

            // Remove current chapter and continue looping
            chapters.RemoveAt(chapters.Count - 1);
        }
    }
    private void Scan(int index, int count, List<Chapter> chapters, Action<Chapter[]> callback)
    {
        // No more chapters to add.
        // Current subset is guranteed to be valid
        if (count == 0)
        {
            // Callback with current subset
            Chapter[] subset = chapters.ToArray();
            if (IsValid(subset))
            {
                Application.DoEvents();
                if (!MainForm.Running) return;

                callback(subset);
            }
            return;
        }

        // Find largest number that satisfies the condition that a valid subset can be found
        int start_index = Book.CHAPTERS - count;
        //  And remember the last index that satisfies the condition
        int last_index = start_index;
        while (start_index > index)
        {
            start_index--;
        }

        //// Find the first number in the sorted chapters that is the largest number we just found
        //// (in case of duplicates)
        //while ((start_index > index) && (Chapters[start_index] == Chapters[start_index - 1]))
        //{
        //    start_index--;
        //}

        // [start_index .. last_index] is the full range we must check in recursion
        for (int i = start_index; i <= last_index; i++)
        {
            // Add current chapter to the subset
            Chapter chapter = Book.Chapters[i];
            chapters.Add(chapter);

            // Recurse through the sub-problem to the right
            this.Scan(i + 1, count - 1, chapters, callback);

            // Remove current chapter and continue looping
            chapters.RemoveAt(chapters.Count - 1);
        }
    }

    // count subsets
    private long m_count = 0L;
    public long Count(int count, long sum)
    {
        m_count = 0L;

        if (count > Book.CHAPTERS)
            return m_count;

        if ((count > 0) && (sum > 0))
        {
            this.Scan(0, count, sum, new List<Chapter>());
        }
        else // * means any value
        {
            if ((count > 0) && (sum == 0))
            {
                this.Scan(0, count, new List<Chapter>());
            }
            else if ((count == 0) && (sum > 0))
            {
                for (int i = 0; i < Book.CHAPTERS; i++)
                {
                    this.Scan(0, i + 1, new List<Chapter>());
                }
            }
            else if ((count == 0) && (sum == 0))
            {
                for (int i = 0; i < Book.CHAPTERS; i++)
                {
                    this.Scan(0, i + 1, new List<Chapter>());
                }
            }
        }

        return m_count;
    }
    private void Scan(int index, int count, long sum, List<Chapter> chapters)
    {
        // No more chapters to add.
        // Current subset is guranteed to be valid
        if (count == 0)
        {
            // Callback with current subset
            Chapter[] subset = chapters.ToArray();
            if (IsValid(subset))
            {
                //Application.DoEvents();
                //if (!MainForm.Running) return;

                m_count++;
            }
            return;
        }

        // Save the smallest remaining sum
        int start_index = Book.CHAPTERS - count;
        int tail_sum = m_tail_sums[start_index];

        // Smallest possible sum is greater than target sum,
        // so a valid subset cannot be found
        if (tail_sum > sum)
        {
            return;
        }

        // Find largest number that satisfies the condition that a valid subset can be found
        tail_sum -= Book.Chapters[start_index].Number;
        // And remember the last index that satisfies the condition
        int last_index = start_index;
        while ((start_index > index) && (tail_sum + Book.Chapters[start_index - 1].Number <= sum))
        {
            start_index--;
        }

        //// Find the first number in the sorted chapters that is the largest number we just found
        //// (in case of duplicates)
        //while ((start_index > index) && (Chapters[start_index] == Chapters[start_index - 1]))
        //{
        //    start_index--;
        //}

        // [start_index .. last_index] is the full range we must check in recursion
        for (int i = start_index; i <= last_index; i++)
        {
            // Find the largest possible sum, which is the sum of the first k chapters
            // starting at current start_index
            int max_sum = m_tail_sums[i] - m_tail_sums[i + count];

            // The largest possible sum is less than the sum, so a valid subset cannot be found
            if (max_sum < sum)
            {
                return;
            }

            // Add current chapter to the subset
            Chapter chapter = Book.Chapters[i];
            chapters.Add(chapter);

            // Recurse through the sub-problem to the right
            this.Scan(i + 1, count - 1, sum - chapter.Number, chapters);

            // Remove current chapter and continue looping
            chapters.RemoveAt(chapters.Count - 1);
        }
    }
    private void Scan(int index, int count, List<Chapter> chapters)
    {
        // No more chapters to add.
        // Current subset is guranteed to be valid
        if (count == 0)
        {
            // Callback with current subset
            Chapter[] subset = chapters.ToArray();
            if (IsValid(subset))
            {
                //Application.DoEvents();
                //if (!MainForm.Running) return;

                m_count++;
            }
            return;
        }

        // Find largest number that satisfies the condition that a valid subset can be found
        int start_index = Book.CHAPTERS - count;
        //  And remember the last index that satisfies the condition
        int last_index = start_index;
        while (start_index > index)
        {
            start_index--;
        }

        //// Find the first number in the sorted chapters that is the largest number we just found
        //// (in case of duplicates)
        //while ((start_index > index) && (Chapters[start_index] == Chapters[start_index - 1]))
        //{
        //    start_index--;
        //}

        // [start_index .. last_index] is the full range we must check in recursion
        for (int i = start_index; i <= last_index; i++)
        {
            // Add current chapter to the subset
            Chapter chapter = Book.Chapters[i];
            chapters.Add(chapter);

            // Recurse through the sub-problem to the right
            this.Scan(i + 1, count - 1, chapters);

            // Remove current chapter and continue looping
            chapters.RemoveAt(chapters.Count - 1);
        }
    }

    // validate query conditions
    private readonly NumberQuery m_query;
    private bool IsValid(Chapter[] chapters)
    {
        if (chapters == null) return false;

        int count = chapters.Length;
        if ((m_query.ChapterCountNumberType == NumberType.None) || (m_query.ChapterCountNumberType == NumberType.Natural))
        {
            if (m_query.ChapterCount > 0)
            {
                if (count != m_query.ChapterCount)
                {
                    return false;
                }
            }
        }
        else
        {
            if (!Numbers.IsNumberType(count, m_query.ChapterCountNumberType))
            {
                return false;
            }
        }

        int sum = 0;
        foreach (Chapter chapter in chapters)
        {
            sum += chapter.Number;
        }
        if ((m_query.ChapterSumNumberType == NumberType.None) || (m_query.ChapterSumNumberType == NumberType.Natural))
        {
            if (m_query.ChapterSum > 0)
            {
                if (sum != m_query.ChapterSum)
                {
                    return false;
                }
            }
        }
        else
        {
            if (!Numbers.IsNumberType(sum, m_query.ChapterSumNumberType))
            {
                return false;
            }
        }

        sum = 0;
        foreach (Chapter chapter in chapters)
        {
            sum += chapter.VerseCount;
        }
        if ((m_query.VerseCountNumberType == NumberType.None) || (m_query.VerseCountNumberType == NumberType.Natural))
        {
            if (m_query.VerseCount > 0)
            {
                if (sum != m_query.VerseCount)
                {
                    return false;
                }
            }
        }
        else
        {
            if (!Numbers.IsNumberType(sum, m_query.VerseCountNumberType))
            {
                return false;
            }
        }

        sum = 0;
        foreach (Chapter chapter in chapters)
        {
            sum += chapter.WordCount;
        }
        if ((m_query.WordCountNumberType == NumberType.None) || (m_query.WordCountNumberType == NumberType.Natural))
        {
            if (m_query.WordCount > 0)
            {
                if (sum != m_query.WordCount)
                {
                    return false;
                }
            }
        }
        else
        {
            if (!Numbers.IsNumberType(sum, m_query.WordCountNumberType))
            {
                return false;
            }
        }

        sum = 0;
        foreach (Chapter chapter in chapters)
        {
            sum += chapter.LetterCount;
        }
        if ((m_query.LetterCountNumberType == NumberType.None) || (m_query.LetterCountNumberType == NumberType.Natural))
        {
            if (m_query.LetterCount > 0)
            {
                if (sum != m_query.LetterCount)
                {
                    return false;
                }
            }
        }
        else
        {
            if (!Numbers.IsNumberType(sum, m_query.LetterCountNumberType))
            {
                return false;
            }
        }

        sum = 0;
        foreach (Chapter chapter in chapters)
        {
            sum += (chapter.Number + chapter.VerseCount);
        }
        if ((m_query.CPlusVSumNumberType == NumberType.None) || (m_query.CPlusVSumNumberType == NumberType.Natural))
        {
            if (m_query.CPlusVSum > 0)
            {
                if (sum != m_query.CPlusVSum)
                {
                    return false;
                }
            }
        }
        else
        {
            if (!Numbers.IsNumberType(sum, m_query.CPlusVSumNumberType))
            {
                return false;
            }
        }

        sum = 0;
        foreach (Chapter chapter in chapters)
        {
            sum += Math.Abs((chapter.Number - chapter.VerseCount)); // Absolute
        }
        if ((m_query.CMinusVSumNumberType == NumberType.None) || (m_query.CMinusVSumNumberType == NumberType.Natural))
        {
            if (m_query.CMinusVSum > 0)
            {
                if (m_query.CMinusVSum == int.MaxValue) // Zero subtitute as 0 means ANY
                {
                    if (sum != 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (sum != m_query.CMinusVSum)
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            if (!Numbers.IsNumberType(sum, m_query.CMinusVSumNumberType))
            {
                return false;
            }
        }

        sum = 0;
        foreach (Chapter chapter in chapters)
        {
            sum += (chapter.Number * chapter.VerseCount);
        }
        if ((m_query.CTimesVSumNumberType == NumberType.None) || (m_query.CTimesVSumNumberType == NumberType.Natural))
        {
            if (m_query.CTimesVSum > 0)
            {
                if (sum != m_query.CTimesVSum)
                {
                    return false;
                }
            }
        }
        else
        {
            if (!Numbers.IsNumberType(sum, m_query.CTimesVSumNumberType))
            {
                return false;
            }
        }

        // passed all tests successfully
        return true;
    }
}
