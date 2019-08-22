namespace EA.Weee.Core.DataReturns
{
    using System;

    public class TestFileSettings
    {
        public Guid OrganisationID { get; private set; }

        public Quarter Quarter { get; private set; }

        public int NumberOfAatfs { get; set; }

        public int NumberOfAes { get; set; }

        public bool AllProducers { get; set; }

        public int NumberOfProduces { get; set; }

        public TestFileSettings(Guid organisationID, Quarter quarter)
        {
            OrganisationID = organisationID;
            Quarter = quarter;
        }
    }
}
