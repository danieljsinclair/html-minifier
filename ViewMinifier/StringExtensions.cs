using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlMinifier
{
    public static class StringExtensions
    {
        // inspired by http://metadeveloper.blogspot.co.uk/2008/06/regex-replace-multiple-strings-in.html
        public static string MultiReplace(this string input, Dictionary<string, string> kp, string regexPrefix = "", string regexSuffix = "", int numOcurrances = 1)
        {
            // sanitise Dictionary for use with regexp
            var regexCompatibleArray = kp.Select(x => regexPrefix + Regex.Escape(x.Key) + regexSuffix).ToArray();
            var regRep = new Regex(String.Join("|", regexCompatibleArray), RegexOptions.None);
            return regRep.Replace(input, delegate(Match m)
            {
                try
                {
                    return kp[m.Value];
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException("Key '" + m.Value + "' not found in replacement list");
                }
            });
        }
    }//class
}
