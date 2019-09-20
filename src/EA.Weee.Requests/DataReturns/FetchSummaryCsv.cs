namespace EA.Weee.Requests.DataReturns
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;
    using System;

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
