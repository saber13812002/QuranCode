using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Composites
{
    class Program
    {
        public static void Main(string[] args)
        {
            StreamWriter writer = null;
            try
            {
                string folder = "Composites";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string filename = folder + "/" + DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".txt";
                writer = System.IO.File.CreateText(filename);

                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Prime-composite Number Indices - (c)2009-2019 Ali Adams");
                Console.WriteLine("-------------------------------------------------------");

                ConsoleKeyInfo exit;
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("factors = -1  ==>  unit, primes and composites.");
                    Console.WriteLine("factors =  0  ==>  composites.");
                    Console.WriteLine("factors =  1  ==>  primes.");
                    Console.WriteLine("factors =  n  ==>  composites with n factors.");
                    Console.WriteLine();
                    Console.Write("How many factors-per-number to find?       ");
                    string n_str = Console.ReadLine();
                    int n = 0;
                    if (int.TryParse(n_str, out n))
                    {
                        if (n >= 0)
                        {
                            BuildFactors(writer, n);
                        }
                        else
                        {
                            BuildFactors(writer, -1);
                        }
                    }
                    else
                    {
                        BuildFactors(writer, -1);
                    }

                    Console.WriteLine();
                    Console.Write("Try another? (y/n)");
                    exit = Console.ReadKey();
                    Console.WriteLine();
                } while ((exit.KeyChar != 'N') && (exit.KeyChar != 'n'));
            }
            finally
            {
                writer.Close();
            }
        }
        private static void BuildFactors(StreamWriter writer, int n)
        {
            if (writer != null)
            {
                int matches = 0;
                StringBuilder str = new StringBuilder();

                writer.WriteLine();
                writer.WriteLine("How many factors-per-number to find?       " + n.ToString());

                Console.Write("Duplicate, unique, or any factors (d/u/a)? ");
                string factors_type = Console.ReadLine();
                writer.WriteLine("Duplicate factors, unique, or any (d/u/a)? " + factors_type);

                Console.Write("Maximum number to factor?                  ");
                string max_str = Console.ReadLine();
                writer.WriteLine("Maximum number to factor?                  " + max_str);
                int max = 0;
                if (int.TryParse(max_str, out max))
                {
                    Console.WriteLine();
                    writer.WriteLine();
                    if (n == 1) // PRIMES
                    {
                        Console.WriteLine("#\tN\tP\tAP\tXP\tFactors");
                        writer.WriteLine("#\tN\tP\tAP\tXP\tFactors");
                    }
                    else // COMPOSITES
                    {
                        Console.WriteLine("#\tN\tC\tAC\tXC\tFactors");
                        writer.WriteLine("#\tN\tC\tAC\tXC\tFactors");
                    }

                    int min = (n == -1) ? 1 : 2;
                    for (int i = min; i <= max; i++)
                    {
                        List<long> factors = Numbers.Factorize(i);

                        // COMPOSITES
                        if ((n == -1) || ((n == 0) && (factors.Count > 1)) || ((n > 1) && (factors.Count == n)))
                        {
                            bool all_are_duplicate = false;
                            bool all_are_unique = true;
                            if (factors_type.ToLower() == "d")
                            {
                                for (int j = 1; j < factors.Count; j++)
                                {
                                    all_are_duplicate = (factors[0] == factors[j]);
                                }
                            }
                            else if (factors_type.ToLower() == "u")
                            {
                                for (int j = 0; j < factors.Count; j++)
                                {
                                    for (int k = j + 1; k < factors.Count; k++)
                                    {
                                        if (factors[j] == factors[k])
                                        {
                                            all_are_unique = false;
                                            break;
                                        }
                                    }
                                    if (!all_are_unique)
                                    {
                                        break;
                                    }
                                }
                            }

                            if (
                                ((factors_type.ToLower() == "d") && all_are_duplicate)
                                ||
                                ((factors_type.ToLower() == "u") && all_are_unique)
                                ||
                                ((factors_type.ToLower() != "d") && (factors_type.ToLower() != "u"))
                               )
                            {
                                matches++;

                                str.Length = 0;
                                foreach (long factor in factors)
                                {
                                    str.Append(factor + "\t");
                                }
                                str.Remove(str.Length - 1, 1);


                                int C = Numbers.CompositeIndexOf(i) + 1;
                                int AC = Numbers.AdditiveCompositeIndexOf(i) + 1;
                                int XC = Numbers.NonAdditiveCompositeIndexOf(i) + 1;

                                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", matches, i, (C <= 0) ? "" : C.ToString(), (AC <= 0) ? "" : AC.ToString(), (XC <= 0) ? "" : XC.ToString(), str.ToString());
                                writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", matches, i, (C <= 0) ? "" : C.ToString(), (AC <= 0) ? "" : AC.ToString(), (XC <= 0) ? "" : XC.ToString(), str.ToString());
                            }
                        }
                        else if ((n == 1) && (factors.Count == n)) // PRIMES
                        {
                            matches++;
                            int P = Numbers.PrimeIndexOf(i) + 1;
                            int AP = Numbers.AdditivePrimeIndexOf(i) + 1;
                            int XP = Numbers.NonAdditivePrimeIndexOf(i) + 1;

                            Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", matches, i, (P <= 0) ? "" : P.ToString(), (AP <= 0) ? "" : AP.ToString(), (XP <= 0) ? "" : XP.ToString(), factors[0].ToString());
                            writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", matches, i, (P <= 0) ? "" : P.ToString(), (AP <= 0) ? "" : AP.ToString(), (XP <= 0) ? "" : XP.ToString(), factors[0].ToString());
                        }
                    } // for
                }
                if (matches == 0)
                {
                    Console.WriteLine("No matches were found!");
                    writer.WriteLine("No matches were found!");
                }

                writer.Flush();
            }
        }
    }
}
