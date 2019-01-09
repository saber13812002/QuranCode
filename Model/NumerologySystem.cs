using System;
using System.Text;
using System.Collections.Generic;

namespace Model
{
    public class NumerologySystem
    {
        // Primalogy System ©2008 Ali Adams - www.heliwave.com
        //public const string DEFAULT_NAME = "Simplified29_Alphabet_Primes1";
        // Based on the name of surat Al-Fatiha to mean The Opener or The Key and
        // the fact that prime numbers are used in cryptography as private keys and
        // the structure of surat Al-Fatiha is built upon special kind of prime numbers
        // where the digit sum is prime too. They are called Additive prime numbers.
        // Surat Al-Fatiha = [7 verses] = [29 words] = [139 letters], all are prime numbers
        // and their digit sums (7=7, 2+9=11 and 1+3+9=13) are prime numbers too.
        // The Primalogy Systems itself produces additive prime numbers too:
        //   1. Al-Fatiha (8317 and 8+3+1+7=19)
        //   2. Al-Ikhlaas with BismAllah (4021 and 4+0+2+1=7) and
        //      Al-Ikhlaas without BismAllah (3167 and 3+1+6+7=17),
        //   3. Ayat Al-Kursi (11261 with 1+1+2+6+1=11)
        //   4. Ayat Ar-Rahmaan (683 with 6+8+3=17) [Fabiayyi aalaai Rabbikuma tukathibaan] where
        //      683 = 124th prime number = 4 * 31 and the aya has 4 words and is repeated 31 times
        //   5. The word "Allah" (269 with 2+6+9=17)
        // See Help\Primalogy.pdf for full details in English and
        // See Help\Primalogy_AR.pdf for full details in Arabic.
        public const string DEFAULT_NAME = "Original_Alphabet_Primes1";

        //private NumerologySystemScope scope = NumerologySystemScope.Book;
        //public NumerologySystemScope Scope
        //{
        //    get { return scope; }
        //    set
        //    {
        //        scope = value;
        //        //TODO: update letter_order and letter_values
        //    }
        //}

        /// <summary>
        /// Name = TextMode_LetterOrder_LetterValue
        /// </summary>
        private string name = null;
        public string Name
        {
            get { return name; }
        }

        private string text_mode = null;
        public string TextMode
        {
            get { return text_mode; }
        }

        private string letter_order = null;
        public string LetterOrder
        {
            get { return letter_order; }
        }

        private string letter_value = null;
        public string LetterValue
        {
            get { return letter_value; }
        }

        private Dictionary<char, long> letter_values = null;
        public Dictionary<char, long> LetterValues
        {
            get { return letter_values; }
        }
        public long this[char letter]
        {
            get
            {
                if (letter_values.ContainsKey(letter))
                {
                    return letter_values[letter];
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            set
            {
                if (letter_values.ContainsKey(letter))
                {
                    letter_values[letter] = value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }
        public void Clear()
        {
            letter_values.Clear();
        }
        public int Count
        {
            get { return letter_values.Count; }
        }
        public void Add(char letter, long value)
        {
            if (letter_values.ContainsKey(letter))
            {
                throw new ArgumentException();
            }
            else
            {
                letter_values.Add(letter, value);
            }
        }
        public Dictionary<char, long>.KeyCollection Keys
        {
            get { return letter_values.Keys; }
        }
        public Dictionary<char, long>.ValueCollection Values
        {
            get { return letter_values.Values; }
        }
        public bool ContainsKey(char letter)
        {
            return letter_values.ContainsKey(letter);
        }

        public bool AddToLetterLNumber;
        public bool AddToLetterWNumber;
        public bool AddToLetterVNumber;
        public bool AddToLetterCNumber;
        public bool AddToLetterLDistance;
        public bool AddToLetterWDistance;
        public bool AddToLetterVDistance;
        public bool AddToLetterCDistance;
        public bool AddToWordWNumber;
        public bool AddToWordVNumber;
        public bool AddToWordCNumber;
        public bool AddToWordWDistance;
        public bool AddToWordVDistance;
        public bool AddToWordCDistance;
        public bool AddToVerseVNumber;
        public bool AddToVerseCNumber;
        public bool AddToVerseVDistance;
        public bool AddToVerseCDistance;
        public bool AddToChapterCNumber;

        public bool AddDistancesWithinChapters = true;

        public NumerologySystem()
            : this(DEFAULT_NAME)
        {
        }

        public NumerologySystem(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                string[] parts = name.Split('_');
                if (parts.Length == 3)
                {
                    this.name = name;
                    this.text_mode = parts[0];
                    this.letter_order = parts[1];
                    this.letter_value = parts[2];

                    this.letter_values = new Dictionary<char, long>();
                }
                else
                {
                    throw new Exception("Invalid numerology system name." + "\r\n" +
                                        "Name must be in this format:" + "\r\n" +
                                        "TextMode_LetterOrder_LetterValue");
                }
            }
            else
            {
                throw new Exception("Numerology system name cannot be empty.");
            }
        }

        public NumerologySystem(string name, Dictionary<char, long> letter_values)
            : this(name)
        {
            this.letter_values = new Dictionary<char, long>(letter_values);
        }

        public NumerologySystem(NumerologySystem numerology_system)
            : this(numerology_system.Name)
        {
            if (numerology_system != null)
            {
                this.letter_values.Clear();
                if (letter_values != null)
                {
                    foreach (char key in numerology_system.Keys)
                    {
                        this.letter_values.Add(key, numerology_system[key]);
                    }
                }
            }
        }

        public long CalculateValue(char character)
        {
            if (letter_values == null) return 0L;

            if (letter_values.ContainsKey(character))
            {
                return letter_values[character];
            }
            return 0L;
        }

        public long CalculateValue(string text)
        {
            if (String.IsNullOrEmpty(text)) return 0L;
            if (letter_values == null) return 0L;

            if (!text.IsArabic())  // eg English
            {
                text = text.ToUpper();
            }

            // simplify all text_modes
            text = text.SimplifyTo(text_mode);

            long result = 0L;
            if (letter_value.StartsWith("Base"))
            {
                string radix_str = "";
                int pos = 4;
                while (Char.IsDigit(letter_value[pos]))
                {
                    radix_str += letter_value[pos];
                    pos++;
                }
                int radix;
                if (int.TryParse(radix_str, out radix))
                {
                    StringBuilder str = new StringBuilder();
                    string[] words = text.Split();
                    foreach (string word in words)
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            if (letter_values.ContainsKey(word[i]))
                            {
                                str.Insert(0, letter_values[word[i]]);
                            }
                        }
                        result += Radix.Decode(str.ToString(), radix);
                        str.Length = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (letter_values.ContainsKey(text[i]))
                    {
                        result += letter_values[text[i]];
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine(Name);
            str.AppendLine(this.ToOverview());

            return str.ToString();
        }
        public string ToOverview()
        {
            StringBuilder str = new StringBuilder();

            if (
                 AddToLetterLNumber ||
                 AddToLetterWNumber ||
                 AddToLetterVNumber ||
                 AddToLetterCNumber ||
                 AddToLetterLDistance ||
                 AddToLetterWDistance ||
                 AddToLetterVDistance ||
                 AddToLetterCDistance
               )
            {
                str.Append("Add to each letter  value" + "\t");
                str.Append("\t"); if (AddToLetterLNumber) str.Append("L");
                str.Append("\t"); if (AddToLetterWNumber) str.Append("W");
                str.Append("\t"); if (AddToLetterVNumber) str.Append("V");
                str.Append("\t"); if (AddToLetterCNumber) str.Append("C");
                str.Append("\t"); if (AddToLetterLDistance) str.Append("∆L");
                str.Append("\t"); if (AddToLetterWDistance) str.Append("∆W");
                str.Append("\t"); if (AddToLetterVDistance) str.Append("∆V");
                str.Append("\t"); if (AddToLetterCDistance) str.Append("∆C");
                str.AppendLine();
            }

            if (
                 AddToWordWNumber ||
                 AddToWordVNumber ||
                 AddToWordCNumber ||
                 AddToWordWDistance ||
                 AddToWordVDistance ||
                 AddToWordCDistance
               )
            {
                str.Append("Add to each word    value" + "\t");
                str.Append("\t");
                str.Append("\t"); if (AddToWordWNumber) str.Append("W");
                str.Append("\t"); if (AddToWordVNumber) str.Append("V");
                str.Append("\t"); if (AddToWordCNumber) str.Append("C");
                str.Append("\t");
                str.Append("\t"); if (AddToWordWDistance) str.Append("∆W");
                str.Append("\t"); if (AddToWordVDistance) str.Append("∆V");
                str.Append("\t"); if (AddToWordCDistance) str.Append("∆C");
                str.AppendLine();
            }

            if (
                 AddToVerseVNumber ||
                 AddToVerseCNumber ||
                 AddToVerseVDistance ||
                 AddToVerseCDistance
               )
            {
                str.Append("Add to each verse   value" + "\t");
                str.Append("\t");
                str.Append("\t");
                str.Append("\t"); if (AddToVerseVNumber) str.Append("V");
                str.Append("\t"); if (AddToVerseCNumber) str.Append("C");
                str.Append("\t");
                str.Append("\t");
                str.Append("\t"); if (AddToVerseVDistance) str.Append("∆V");
                str.Append("\t"); if (AddToVerseCDistance) str.Append("∆C");
                str.AppendLine();
            }

            if (
                 AddToChapterCNumber
               )
            {
                str.Append("Add to each chapter value" + "\t");
                str.Append("\t");
                str.Append("\t");
                str.Append("\t");
                str.Append("\t"); if (AddToChapterCNumber) str.Append("C");
                str.Append("\t");
                str.Append("\t");
                str.Append("\t");
                str.Append("\t");
                str.AppendLine();
            }

            str.AppendLine();

            if (
                 AddToLetterLDistance ||
                 AddToLetterWDistance ||
                 AddToLetterVDistance ||
                 AddToLetterCDistance ||
                 AddToWordWDistance ||
                 AddToWordVDistance ||
                 AddToWordCDistance ||
                 AddToVerseVDistance ||
                 AddToVerseCDistance
               )
            {
                str.AppendLine("AddDistancesWithinChapters = " + AddDistancesWithinChapters);
                str.AppendLine();
            }

            if (
                 AddToLetterLNumber ||
                 AddToLetterWNumber ||
                 AddToLetterVNumber ||
                 AddToLetterCNumber ||
                 AddToLetterLDistance ||
                 AddToLetterWDistance ||
                 AddToLetterVDistance ||
                 AddToLetterCDistance ||
                 AddToWordWNumber ||
                 AddToWordVNumber ||
                 AddToWordCNumber ||
                 AddToWordWDistance ||
                 AddToWordVDistance ||
                 AddToWordCDistance ||
                 AddToVerseVNumber ||
                 AddToVerseCNumber ||
                 AddToVerseVDistance ||
                 AddToVerseCDistance ||
                 AddToChapterCNumber
               )
            {
                str.AppendLine("L" + "\t" + "Add letter number in word to each letter value" + "\t" + "أضف رقم الحرف في كلمته الى قيمة كل حرف");
                str.AppendLine("W" + "\t" + "Add word number in verse to each letter value" + "\t" + "أضف رقم كلمة الحرف في ءايتها الى قيمة كل حرف");
                str.AppendLine("V" + "\t" + "Add verse number in chapter to each letter value" + "\t" + "أضف رقم ءاية الحرف في سورتها الى قيمة كل حرف");
                str.AppendLine("C" + "\t" + "Add chapter number in book to each letter value" + "\t" + "أضف رقم سورة الحرف في القرءان الى قيمة كل حرف");
                str.AppendLine("∆L" + "\t" + "Add the number of letters back to the same letter" + "\t" + "أضف عدد الحروف بين الحروف المتماثلة الى قيمة كل حرف");
                str.AppendLine("∆W" + "\t" + "Add the number of words back to the same letter" + "\t" + "أضف عدد الكلمات بين الحروف المتماثلة الى قيمة كل حرف");
                str.AppendLine("∆V" + "\t" + "Add the number of verses back to the same letter" + "\t" + "أضف عدد الءايات بين الحروف المتماثلة الى قيمة كل حرف");
                str.AppendLine("∆C" + "\t" + "Add the number of chapters back to the same letter" + "\t" + "أضف عدد السُوَر بين الحروف المتماثلة الى قيمة كل حرف");
                str.AppendLine("W" + "\t" + "Add word number in verse to each word value" + "\t" + "أضف رقم الكلمة في ءايتها الى قيمة كل كلمة");
                str.AppendLine("V" + "\t" + "Add verse number in chapter to each word value" + "\t" + "أضف رقم ءاية الكلمة في سورتها الى قيمة كل كلمة");
                str.AppendLine("C" + "\t" + "Add chapter number in book to each word value" + "\t" + "أضف رقم سورة الكلمة في القرءان الى قيمة كل كلمة");
                str.AppendLine("∆W" + "\t" + "Add the number of words back to the same word" + "\t" + "أضف عدد الكلمات بين الكلمات المتماثلة الى قيمة كل كلمة");
                str.AppendLine("∆V" + "\t" + "Add the number of verses back to the same word" + "\t" + "أضف عدد الءايات بين الكلمات المتماثلة الى قيمة كل كلمة");
                str.AppendLine("∆C" + "\t" + "Add the number of chapters back to the same word" + "\t" + "أضف عدد السُوَر بين الكلمات المتماثلة الى قيمة كل كلمة");
                str.AppendLine("V" + "\t" + "Add verse number in chapter to each verse value" + "\t" + "أضف رقم الءاية في سورتها الى قيمة كل ءاية");
                str.AppendLine("C" + "\t" + "Add chapter number in book to each verse value" + "\t" + "أضف رقم سورة الءاية في القرءان الى قيمة كل ءاية");
                str.AppendLine("∆V" + "\t" + "Add the number of verses back to the same verse" + "\t" + "أضف عدد الءايات بين الءايات المتماثلة الى قيمة كل ءاية");
                str.AppendLine("∆C" + "\t" + "Add the number of chapters back to the same verse" + "\t" + "أضف عدد السُوَر بين الءايات المتماثلة الى قيمة كل ءاية");
                str.AppendLine("C" + "\t" + "Add chapter number in book to each chapter value" + "\t" + "أضف رقم السورة في القرءان الى قيمة كل سورة");
                str.AppendLine("Pos" + "\t" + "Add positions of letters/words/verses/chapters" + "\t" + "أضف مواقع الحروف/الكلمات/الءايات/السُوَر");
                str.AppendLine("∆" + "\t" + "Add distances to same letters/words/verses back" + "\t" + "أضف المسافات الى الحروف/الكلمات/الءايات المتماثلة السابقة");
                str.AppendLine("∆ ضمن السُوَر" + "\t" + "Add distances within chapters" + "\t" + "أضف المسافات ضمن السُوَر");
                str.AppendLine();
            }

            str.AppendLine("--------------------------");
            str.AppendLine("Letter" + "\t" + "Value");
            str.AppendLine("--------------------------");
            foreach (char letter in LetterValues.Keys)
            {
                str.AppendLine(letter.ToString() + "\t" + LetterValues[letter].ToString());
            }
            str.AppendLine("--------------------------");

            return str.ToString();
        }
        public string ToTabbedString()
        {
            return (TextMode +
            "\t" + LetterOrder +
            "\t" + LetterValue +
            "\t" + (AddToLetterLNumber ? "L" : "") +
            "\t" + (AddToLetterWNumber ? "W" : "") +
            "\t" + (AddToLetterVNumber ? "V" : "") +
            "\t" + (AddToLetterCNumber ? "C" : "") +
            "\t" + (AddToLetterLDistance ? "∆L" : "") +
            "\t" + (AddToLetterWDistance ? "∆W" : "") +
            "\t" + (AddToLetterVDistance ? "∆V" : "") +
            "\t" + (AddToLetterCDistance ? "∆C" : "") +
            "\t" + (AddToWordWNumber ? "W" : "") +
            "\t" + (AddToWordVNumber ? "V" : "") +
            "\t" + (AddToWordCNumber ? "C" : "") +
            "\t" + (AddToWordWDistance ? "∆W" : "") +
            "\t" + (AddToWordVDistance ? "∆V" : "") +
            "\t" + (AddToWordCDistance ? "∆C" : "") +
            "\t" + (AddToVerseVNumber ? "V" : "") +
            "\t" + (AddToVerseCNumber ? "C" : "") +
            "\t" + (AddToVerseVDistance ? "∆V" : "") +
            "\t" + (AddToVerseCDistance ? "∆C" : "") +
            "\t" + (AddToChapterCNumber ? "C" : "") +
            "\t" + (AddDistancesWithinChapters)
            );
        }
    }
}
