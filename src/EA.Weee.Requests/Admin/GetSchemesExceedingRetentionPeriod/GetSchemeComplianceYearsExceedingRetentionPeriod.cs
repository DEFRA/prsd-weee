namespace EA.Weee.Requests
{
    using System;
    using System.Collections.Generic;
    using Core.Admin;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetSchemeComplianceYearsExceedingRetentionPeriod : IRequest<List<int>>
    {
        public GetSchemeComplianceYearsExceedingRetentionPeriod()
        {
        }
    }
}
