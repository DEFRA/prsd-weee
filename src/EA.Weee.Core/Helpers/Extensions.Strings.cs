namespace EA.Weee.Core.Helpers
{
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
    }
}
