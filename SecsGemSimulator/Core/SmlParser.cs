using System;
using System.Collections.Generic;
using System.Linq;

namespace SecsGemSimulator.Core
{
    public static class SmlParser
    {
        public static SecsItem Parse(string sml)
        {
            sml = sml.Trim();
            if (string.IsNullOrEmpty(sml)) return null;

            int index = 0;
            return ParseItem(sml, ref index);
        }

        private static SecsItem ParseItem(string sml, ref int index)
        {
            SkipWhitespace(sml, ref index);
            if (index >= sml.Length) return null;

            if (sml[index] != '<') throw new Exception($"Expected '<' at index {index}");
            index++; // Skip '<'

            SkipWhitespace(sml, ref index);
            
            // Read Type
            string type = ReadToken(sml, ref index).ToUpper();
            
            SecsItem item = null;

            if (type == "L")
            {
                var list = new List<SecsItem>();
                while (true)
                {
                    SkipWhitespace(sml, ref index);
                    if (index >= sml.Length) throw new Exception("Unexpected end of SML in List");
                    if (sml[index] == '>')
                    {
                        index++; // Skip '>'
                        break;
                    }
                    list.Add(ParseItem(sml, ref index));
                }
                item = new SecsList(list);
            }
            else if (type == "A")
            {
                 SkipWhitespace(sml, ref index);
                 // Expect quoted string
                 if (sml[index] == '"')
                 {
                     index++;
                     int start = index;
                     while (index < sml.Length && sml[index] != '"') index++;
                     string val = sml.Substring(start, index - start);
                     index++; // Skip closing '"'
                     item = new SecsAscii(val);
                 }
                 else
                 {
                     // Maybe empty? <A>
                     item = new SecsAscii("");
                 }
                 SkipUntilClose(sml, ref index);
            }
            else if (type == "U1" || type == "U2" || type == "U4" || type == "I1" || type == "I2" || type == "I4")
            {
                 var numbers = ReadNumbers(sml, ref index);
                 if (type == "U1") item = new SecsU1(numbers.Select(n => (byte)n).ToArray());
                 else if (type == "U2") item = new SecsU2(numbers.Select(n => (ushort)n).ToArray());
                 else if (type == "U4") item = new SecsU4(numbers.Select(n => (uint)n).ToArray());
                 else if (type == "I1") item = new SecsI1(numbers.Select(n => (sbyte)n).ToArray());
                 else if (type == "I2") item = new SecsI2(numbers.Select(n => (short)n).ToArray());
                 else item = new SecsI4(numbers.Select(n => (int)n).ToArray());

                 SkipUntilClose(sml, ref index);
            }
            else
            {
                 // Unknown or Binary
                 SkipUntilClose(sml, ref index);
                 item = new SecsBinary(new byte[0]);
            }

            return item;
        }

        private static void SkipWhitespace(string s, ref int index)
        {
            while (index < s.Length && char.IsWhiteSpace(s[index])) index++;
        }
        
        private static string ReadToken(string s, ref int index)
        {
            int start = index;
            while (index < s.Length && !char.IsWhiteSpace(s[index]) && s[index] != '>') index++;
            return s.Substring(start, index - start);
        }

        private static List<long> ReadNumbers(string s, ref int index)
        {
            List<long> list = new List<long>();
            while (index < s.Length)
            {
                SkipWhitespace(s, ref index);
                if (s[index] == '>') break;
                string token = ReadToken(s, ref index);
                if (long.TryParse(token, out long val)) list.Add(val);
            }
            return list;
        }

        private static void SkipUntilClose(string s, ref int index)
        {
             while (index < s.Length && s[index] != '>') index++;
             if (index < s.Length) index++; // Skip '>'
        }
    }
}
