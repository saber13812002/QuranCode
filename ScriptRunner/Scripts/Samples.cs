using System;
using System.Collections.Generic;
using Model;

public class MyScript : IScriptRunner
{
    private bool MyMethod(Client client)
    {
        if (client == null) return false;
        if (client.Book == null) return false;
        if (client.Book.Verses == null) return false;
        if (client.FoundVerses == null) client.FoundVerses = new List<Verse>();

        client.FoundVerses.Clear();
        foreach (Verse verse in client.Book.Verses)
        {
            string text = verse.Text.Replace(" ", "");
            if (text.StartsWith("و"))
            {
                int sum = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == 'و')
                    {
                        sum += (i + 1);
                    }
                }

                if (sum == 479)
                {
                    client.FoundVerses.Add(verse);
                }
            }

            //if (Numbers.IsPrime(verse.Number))                         //  811
            //if (Numbers.IsAdditivePrime(verse.Number))                 //  408
            //if (Numbers.IsNonAdditivePrime(verse.Number))              //  403
            //if (Numbers.IsPrime(verse.NumberInChapter))                // 1730
            //if (Numbers.IsAdditivePrime(verse.NumberInChapter))        // 1060
            //if (Numbers.IsNonAdditivePrime(verse.NumberInChapter))     //  670
            //if (Numbers.IsComposite(verse.Number))                     // 5424
            //if (Numbers.IsAdditiveComposite(verse.Number))             // 3842
            //if (Numbers.IsNonAdditiveComposite(verse.Number))          // 1582
            //if (Numbers.IsComposite(verse.NumberInChapter))            // 4392
            //if (Numbers.IsAdditiveComposite(verse.NumberInChapter))    // 2824
            //if (Numbers.IsNonAdditiveComposite(verse.NumberInChapter)) // 1568

            //if ((verse.Number % 19) == 0)                              // verse number in the Quran is divisble by 19
            //if (verse.NumberInChapter == verse.Words.Count)            // verse number = verse words
            //if (verse.NumberInChapter == verse.LetterCount)            // verse number = verse letters
            //if (client.CalculateValue(verse) == 114)                   // verse value = 114 in current Client.NumerologySystem
            //{
            //    client.FoundVerses.Add(verse);
            //}
        }
        return true; // to close Script window and show client.FoundVerses
    }

    // QuranCode USE ONLY
    /// <summary>
    /// Run implements IScriptRunner interface to be invoked by QuranCode application.
    /// </summary>
    /// <param name="args">Must pass a Client object here.</param>
    /// <returns>Return bool to indicate success or not.</returns>
    public object Run(object[] args)
    {
        if (args.Length >= 1)
        {
            Client client = args[0] as Client;
            if (client != null)
            {
                return MyMethod(client);
            }
        }
        return false;
    }
}
