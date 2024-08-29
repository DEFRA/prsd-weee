namespace EA.Weee.Core.Validation
{
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Organisations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class ExternalAddressValidator
    {
        private static readonly Regex UkPostcodeRegex = new Regex(
            @"^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})$",
            RegexOptions.Compiled);

        public static IEnumerable<ValidationResult> Validate(Guid countryId, string postCode, string countryIdName, string postcodeName)
        {
            if (countryId == Guid.Empty)
            {
                yield return new ValidationResult("Please select a country",
                    new[] { countryIdName });
            }
            else if (UkCountryList.ValidCountryIds.Contains(countryId) && !UkPostcodeRegex.IsMatch(postCode))
            {
                yield return new ValidationResult("Enter a full UK postcode",
                    new[] { postcodeName });
            }
        }
    }
}