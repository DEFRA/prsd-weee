namespace EA.Weee.Domain.Lookup
{
    public class QuarterWindowTemplate
    {
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
