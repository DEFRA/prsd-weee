namespace EA.Weee.Core.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
