namespace EA.Weee.Web.Extensions
{
    using System;

    public static class DecimalExtensions
    {
        public static decimal? ToDecimal(this string input)
        {
            decimal? value = null;
            if (!string.IsNullOrWhiteSpace(input))
            {
                value = Convert.ToDecimal(input);
            }

            return value;
        }
    }
}