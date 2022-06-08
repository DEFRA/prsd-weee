namespace EA.Weee.Core.Validation
{
    using DataReturns;
    using Helpers;

    public static class WeeeCategoryEnumDisplay
    {
        public static string ToCustomDisplayString(this WeeeCategory category)
        {
            if (category == WeeeCategory.ITAndTelecommsEquipment)
            {
                return category.ToDisplayString();
            }

            return category.ToDisplayString().ToLower();
        }
    }
}
