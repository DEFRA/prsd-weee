namespace EA.Weee.RequestHandlers.Admin.Helpers
{
    using EA.Weee.Domain.Lookup;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    public static class ReportHelper
    {
        public static Dictionary<int, string> GetCategoryDisplayNames()
        {
            Dictionary<int, string> categories = new Dictionary<int, string>();

            foreach (WeeeCategory category in Enum.GetValues(typeof(WeeeCategory)))
            {
                int enumValue = (int)category;

                FieldInfo field = category.GetType().GetField(category.ToString());

                DisplayAttribute displayName = field.GetCustomAttribute<DisplayAttribute>();

                if (enumValue <= 9)
                {
                    var categoryNumber = "0" + Convert.ToString(enumValue);
                    categories.Add((int)category, $"{categoryNumber}. {displayName.Name}");
                }
                else
                {
                    categories.Add((int)category, $"{enumValue}. {displayName.Name}");
                }
            }

            return categories;
        }
    }
}
