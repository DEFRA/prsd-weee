namespace EA.Weee.Core.Helpers
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    public partial class Extensions
    {
        public static string HardTrim(this string str, int maxLength)
        {
            if (str.Length > maxLength)
            {
                return str.Substring(0, maxLength - 3) + "...";
            }

            return str;
        }

        /// <summary>
        /// Replaces consecutive spaces with &nbsp;
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceConsecutiveWhiteSpacesWithNonBreakingSpace(this string str)
        {            
            return str.Replace("  ", "&nbsp;&nbsp;").Replace("&nbsp; ", "&nbsp;&nbsp;");
        }

        public static string ToTonnageDisplay(this decimal? tonnage)
        {
            return tonnage.HasValue ? ToTonnageDisplay(tonnage.Value) : "0.000";
        }

        public static string ToTonnageDisplay(this decimal tonnage)
        {
            return $"{tonnage:#,##0.000}";
        }
    }
} 