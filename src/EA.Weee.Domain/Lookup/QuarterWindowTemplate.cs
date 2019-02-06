namespace EA.Weee.Domain.Lookup
{
    using System;

    public class QuarterWindowTemplate
    {
        public Guid Id { get; set; }

        public int Quarter { get; set; }

        public int AddStartYears { get; set; }

        public int StartMonth { get; set; }

        public int StartDay { get; set; }

        public int AddEndYears { get; set; }

        public int EndMonth { get; set; }

        public int EndDay { get; set; }

        public QuarterWindowTemplate()
        {
        }
    }
}
