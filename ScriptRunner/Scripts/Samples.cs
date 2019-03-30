using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;
using Model;

public class MyScript : IScriptRunner
{
    /// <summary>
    /// Write your C# script inside this method.
    /// Don't change its name or parameter list
    /// </summary>
    /// <param name="client">Client object holding a reference to the currently selected Book object in TextMode (eg Simplified29)</param>
    /// <param name="param">any user parameter in the TextBox next to the EXE button (ex Frequency, LettersToJump, DigitSum target, etc)</param>
    /// <returns>true to disply back in QuranCode matching verses. false to keep script window open</returns>
    private bool MyMethod(Client client, string param)
    {
        if (client == null) return false;
        if (client.Selection == null) return false;
        List<Verse> verses = client.Selection.Verses;

        if (client.Book != null)
        {
            // Query the whole book, not just the current verses
            verses = client.Book.Verses;

            int target;
            if (param == "")
            {
                target = 0;
            }
            else
            {
                if (!int.TryParse(param, out target))
                {
                    string[] parts = param.Split(',');
                    if (parts.Length == 2)
                    {
                        try
                        {
                            int unique_letters = int.Parse(parts[0]);
                            int letter_frequency = int.Parse(parts[1]);
                            foreach (Verse verse in verses)
                            {
                                if (verse.UniqueLetters.Count == unique_letters)
                                {
                                    foreach (char key in client.NumerologySystem.Keys)
                                    {
                                        if (verse.GetLetterFrequency(key) == letter_frequency)
                                        {
                                            client.FoundVerses.Add(verse);
                                        }
                                    }
                                }
                            }
                            return true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Application.ProductName);
                            return false;
                        }
                    }
                    return false;
                }
            }

            try
            {
                client.FoundVerses = new List<Verse>();
                foreach (Verse verse in verses)
                {
                    long value = client.CalculateValue(verse);

                    bool param_condition = false;
                    if (target == 0) // target == any digit sum
                    {
                        param_condition = true;
                    }
                    else
                    {
                        if (param.ToUpper() == "P") // target == prime digit sum
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
                            param_condition = (Numbers.DigitSum(value) == target);
                        }
                    }

                    if ((Numbers.IsAdditivePrime(value) && param_condition))
                    {
                        client.FoundVerses.Add(verse);
                    }
                }

                return true; // to close Script window and show Selection
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName);
                return false; // to stay in the Script window
            }
        }
        return false;
    }

    /// <summary>
    /// Run implements IScriptRunner interface to be invoked by QuranCode application
    /// </summary>
    /// <param name="args">any number and type of arguments</param>
    /// <returns>return any type</returns>
    public object Run(object[] args)
    {
        try
        {
            if (args.Length == 2)   // ScriptMethod(Client, string)
            {
                Client client = args[0] as Client;
                string param = args[1].ToString();

                if (client != null)
                {
                    return MyMethod(client, param);
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Application.ProductName);
            return null;
        }
    }
}
