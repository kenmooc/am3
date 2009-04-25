using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace AM3
{
    public class Utility
    {
        /// <summary>
        /// Returns an empty string if the provided string is null
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <returns>string</returns>

        public static string MaskNullString(string s)
        {
            return s == null ? string.Empty : s;
        }

        /// <summary>
        /// Fills an empty string with the provided string
        /// </summary>
        /// <param name="s">The string to fill</param>
        /// <param name="filler">The masking string</param>
        /// <returns>string</returns>
        
        public static string MaskEmptyString(string s, string filler)
        {
            return MaskNullString(s) == string.Empty ? filler : s;
        }

        private Utility() { }
    }
}
