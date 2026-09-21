namespace EA.Weee.Requests
{
    using System.Collections.Generic;
    using EA.Weee.DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;

    public class GetSchemeDataExceedingRetentionPeriodRequest : IRequest<List<SchemeDataExceedingRetentionPeriod>>
    {
        public int? ComplianceYear { get; set; }

        public string SchemeName { get; set; }

        public GetSchemeDataExceedingRetentionPeriodRequest(int? complianceYear, string schemeName)
        {
            ComplianceYear = complianceYear;
            SchemeName = schemeName;
        }
    }
}
