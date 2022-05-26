namespace EA.Weee.Core.Validation
{
    using System.Globalization;
    using System.Text.RegularExpressions;

    public static class DecimalValidationHelper
    {
        /* Regex to validate correct use of commas as thousands separator.  Must also consider presence of decimals*/
        private static readonly Regex ValidThousandRegex = new Regex(@"(^\d{1,3}(,\d{3})*\.\d+$)|(^\d{1,3}(,\d{3})*$)|(^(\d)*\.\d*$)|(^\d*$)");

        public const int MaxTonnageLength = 14;

        public static bool WeeeDecimalLength(this string value)
        {
            return Length(value) > MaxTonnageLength;
        }

        public static bool WeeeDecimal(this string value, out decimal decimalResult)
        {
            return decimal.TryParse(value, NumberStyles.Number & ~NumberStyles.AllowTrailingSign, CultureInfo.InvariantCulture, out decimalResult);
        }

        public static bool WeeeDecimalWithWhiteSpace(this string value, decimal decimalResult)
        {
            return !decimal.TryParse(value.ToString(),
                NumberStyles.Number &
                ~NumberStyles.AllowLeadingWhite &
                ~NumberStyles.AllowTrailingWhite &
                ~NumberStyles.AllowLeadingSign &
                ~NumberStyles.AllowTrailingSign,
                CultureInfo.InvariantCulture,
                out decimalResult);
        }

        public static bool WeeeNegativeDecimal(this string value, decimal decimalValue)
        {
            return decimalValue < 0 || (value.Substring(0, 1) == "-");
        }

        public static bool WeeeDecimalThreePlaces(this decimal value)
        {
            return value.DecimalPlaces() > 3;
        }

        public static bool WeeeThousandSeparator(this object value)
        {
            if (value == null)
            {
                return false;
            }

            return ValidThousandRegex.IsMatch(value.ToString());
        }

        private static int Length(object value)
        {
            var decimalPlaces = value.ToString().DecimalPlaces();
            var lengthTrimmed = value.ToString().Replace(",", string.Empty).Length;

            if (decimalPlaces > 0)
            {
                return lengthTrimmed - (decimalPlaces + 1);
            }

            return lengthTrimmed;
        }
    }
}
