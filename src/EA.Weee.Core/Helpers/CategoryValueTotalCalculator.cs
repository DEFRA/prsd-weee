namespace EA.Weee.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using EA.Weee.Core.Validation;

    public class CategoryValueTotalCalculator : ICategoryValueTotalCalculator
    {
        public string Total(IList<string> categoryValues)
        {
            var total = 0.000m;
            var values = categoryValues.Where(v => !string.IsNullOrWhiteSpace(v)
                                                       && decimal.TryParse(v,
                                                        NumberStyles.Number &
                                                        ~NumberStyles.AllowLeadingSign & ~NumberStyles.AllowTrailingSign,
                                                        CultureInfo.InvariantCulture, out var output)
                                                       && output.DecimalPlaces() <= 3).ToList();

            if (values.Any())
            {
                var convertedValues = values.ConvertAll(Convert.ToDecimal);
                total = convertedValues.Sum();
            }

            return total.ToTonnageDisplay();
        }
    }
}