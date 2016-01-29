namespace EA.Weee.Requests.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;

    /// <summary>
    /// Requests a CSV file containing details of submitted EEE and WEEE data
    /// for the specified organisation and compliance year.
    /// </summary>
    public class FetchSummaryCsv : IRequest<FileInfo>
    {
        public Guid OrganisationId { get; private set; }

        public int ComplianceYear { get; private set; }

        public FetchSummaryCsv(Guid organisationId, int complianceYear)
        {
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}
