namespace EA.Weee.Core.Validation
{
    using System.Globalization;
    using System.Text.RegularExpressions;

    public class TonnageValueValidator : ITonnageValueValidator
    {
        public const int MaxTonnageLength = 14;
        /* Regex to validate correct use of commas as thousands separator.  Must also consider presence of decimals*/
        private readonly Regex validThousandRegex = new Regex(@"(^\d{1,3}(,\d{3})*\.\d+$)|(^\d{1,3}(,\d{3})*$)|(^(\d)*\.\d*$)|(^\d*$)");

        public TonnageValidationResult Validate(object value)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return TonnageValidationResult.Success;
            }

            if (Length(value) > MaxTonnageLength)
            {
                return new TonnageValidationResult(TonnageValidationTypeEnum.MaximumDigits);
            }

            if (!decimal.TryParse(value.ToString(), NumberStyles.Number & ~NumberStyles.AllowTrailingSign, CultureInfo.InvariantCulture, out var decimalResult))
            {
                return new TonnageValidationResult(TonnageValidationTypeEnum.NotNumerical);
            }

            if (decimalResult < 0 || (value.ToString().Substring(0, 1) == "-"))
            {
                return new TonnageValidationResult(TonnageValidationTypeEnum.LessThanZero);
            }

            if (!decimal.TryParse(value.ToString(),
                    NumberStyles.Number &
                    ~NumberStyles.AllowLeadingWhite &
                    ~NumberStyles.AllowTrailingWhite &
                    ~NumberStyles.AllowLeadingSign &
                    ~NumberStyles.AllowTrailingSign,
                    CultureInfo.InvariantCulture,
                    out decimalResult))
            {
                return new TonnageValidationResult(TonnageValidationTypeEnum.NotNumerical);
            }

            if (decimalResult.DecimalPlaces() > 3)
            {
                return new TonnageValidationResult(TonnageValidationTypeEnum.DecimalPlaces);
            }

            if (!validThousandRegex.IsMatch(value?.ToString()))
            {
                return new TonnageValidationResult(TonnageValidationTypeEnum.DecimalPlaceFormat);
            }

            return TonnageValidationResult.Success;
        }

        private int Length(object value)
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
