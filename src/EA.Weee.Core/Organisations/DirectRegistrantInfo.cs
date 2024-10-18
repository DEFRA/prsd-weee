namespace EA.Weee.Core.Organisations
{
    using System;

    [Serializable]
    public class DirectRegistrantInfo
    {
        public Guid DirectRegistrantId { get; set; }

        public bool YearSubmissionStarted { get; set; }

        public string RepresentedCompanyName { get; set; }
    }
}
