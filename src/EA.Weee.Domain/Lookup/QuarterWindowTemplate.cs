namespace EA.Weee.Domain.Lookup
{
    using System;

    public class QuarterWindowTemplate
    {
        public Guid Id { get; private set; }

        public int Quarter { get; private set; }

        public int AddStartYears { get; private set; }

        public int StartMonth { get; private set; }

        public int StartDay { get; private set; }

        public int AddEndYears { get; private set; }

        public int EndMonth { get; private set; }

        public int EndDay { get; private set; }

        protected QuarterWindowTemplate()
        {
        }
    }
}
