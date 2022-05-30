namespace EA.Weee.Core.Shared.CsvReading
{
    using System;
    using Domain.Lookup;

    [AttributeUsage((AttributeTargets.Property))]
    
    public class WeeeCategoryAttribute : Attribute
    {
        public WeeeCategory Category { get; private set; }

        public WeeeCategoryAttribute(WeeeCategory category)
        {
            Category = category;
        }
    }
}
