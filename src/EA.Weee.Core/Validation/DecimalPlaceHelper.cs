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

            var converted = value.ToString(CultureInfo.InvariantCulture);
            var indexDecimal = converted.IndexOf(".", StringComparison.Ordinal);

            return indexDecimal > 0 ? converted.Substring(indexDecimal + 1).Length : 0;
        }
    }
}
