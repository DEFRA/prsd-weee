namespace EA.Weee.Core.Validation
{
    using EA.Weee.Core.Constants;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class ExternalAddressValidator
    {
        public static readonly Regex UkPostcodeRegex = new Regex(
            @"^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})$",
            RegexOptions.Compiled);

        private const string Area = @"[A-Za-z]{1,2}";  // One or two letters for the area
        private const string DistrictDigit = @"[0-9]";  // First number
        private const string OptionalSecondDigit = @"[0-9]?";  // Optional second number
        private const string OptionalDistrictLetter = @"[A-Za-z]?";  // Optional letter
        private const string OptionalSpace = @"\s?";  // Optional space
        private const string OptionalInward = @"([0-9][A-Za-z]{2})?";  // Optional inward code

        public static readonly Regex UkPartialPostcodeRegex = new Regex(@"^[A-Za-z]{1,2}[0-9]+.*$", RegexOptions.Compiled);

        public static IEnumerable<ValidationResult> Validate(Guid countryId, string postCode, string countryIdName, string postcodeName)
        {
            if (countryId == Guid.Empty)
            {
                yield return new ValidationResult("Please select a country",
                    new[] { countryIdName });
            }
            else if (UkCountry.ValidIds.Contains(countryId) && !string.IsNullOrWhiteSpace(postCode))
            {
                if (!UkPostcodeRegex.IsMatch(postCode))
                {
                    yield return new ValidationResult("Enter a full UK postcode",
                        new[] { postcodeName });
                }
            }
        }

        public static bool IsValidPartialPostcode(string postcode) =>
            !string.IsNullOrWhiteSpace(postcode) && UkPartialPostcodeRegex.IsMatch(postcode);
    }
}