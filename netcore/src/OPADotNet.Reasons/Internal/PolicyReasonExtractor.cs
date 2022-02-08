using OPADotNet.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Reasons
{
    internal class PolicyReasonExtractor
    {
        public static Dictionary<string, Dictionary<int, string>> ExtractReasons(IReadOnlyList<Policy> policies)
        {
            //Dictionary for each file and each row where the reason message exists
            Dictionary<string, Dictionary<int, string>> reasons = new Dictionary<string, Dictionary<int, string>>();

            foreach(var policy in policies)
            {
                reasons.Add(policy.Id, ParseLines(policy.Raw));
            }

            return reasons;
        }

        private static Dictionary<int, string> ParseLines(string policy)
        {
            Dictionary<int, string> reasons = new Dictionary<int, string>();
            var lines = policy.Split(Environment.NewLine);

            for (int i = 0; i < lines.Length; i++)
            {
                if (TryParseRow(lines[i], out var message))
                {
                    reasons.Add(i + 1, message);
                }
            }
            return reasons;
        }

        private static void ParseWhitespace(string row, ref int i)
        {
            for (; i < row.Length; i++)
            {
                if (char.IsWhiteSpace(row[i]))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
        }

        private static bool ParseText(string row, string text, ref int i)
        {
            for (; i < row.Length; i++)
            {
                if (char.IsWhiteSpace(row[i]))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }

            for (int k = 0 ; i < row.Length; i++)
            {
                if (char.ToUpperInvariant(row[i]) == char.ToUpperInvariant(text[k]))
                {
                    k++;
                    if (k == text.Length)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private static bool TryParseRow(string row, out string message)
        {
            message = null;
            int i = 0;
            if (!ParseText(row, "#", ref i))
            {
                return false;
            }
            i++;
            if (!ParseText(row, "REASON:", ref i))
            {
                return false;
            }
            i++;
            ParseWhitespace(row, ref i);


            message = row.Substring(i);

            return true;
        }
    }
}
