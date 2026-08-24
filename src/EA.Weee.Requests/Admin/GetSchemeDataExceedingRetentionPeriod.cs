namespace EA.Weee.Requests
{
    using System;
    using System.Collections.Generic;
    using Core.Admin;
    using Core.Shared;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess.StoredProcedure;
    using Prsd.Core.Mediator;
    using static EA.Weee.Requests.Admin.GetSchemes;

    public class GetSchemeDataExceedingRetentionPeriod : IRequest<List<SchemeDataExceedingRetentionPeriod>>
    {
        public int? ComplianceYear { get; set; }

        public string SchemeName { get; set; }

        public GetSchemeDataExceedingRetentionPeriod(int? complianceYear, string schemeName)
        {
            ComplianceYear = complianceYear;
            SchemeName = schemeName;
        }
    }
}
