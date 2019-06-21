namespace EA.Weee.Core.Validation
{
    using System;
    using System.Globalization;

    public static class DecimalPlaceHelper
    {
        public static int DecimalPlaces(this decimal value)
        {
            if (value == 0)
            {
                return 0;
            }

            return NumberOfPlaces(value.ToString(CultureInfo.InvariantCulture));
        }

        public static int DecimalPlaces(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            return NumberOfPlaces(value);
        }

        private static int NumberOfPlaces(string value)
        {
            var indexDecimal = value.IndexOf(".", StringComparison.Ordinal);

            return indexDecimal > 0 ? value.Substring(indexDecimal + 1).Length : 0;
        }
    }
}
