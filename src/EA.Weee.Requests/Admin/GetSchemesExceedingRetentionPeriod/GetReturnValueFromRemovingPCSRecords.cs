namespace EA.Weee.Requests
{
    using System;
    using System.Collections.Generic;
    using Core.Admin;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetReturnValueFromRemovingPCSRecords : IRequest<int>
    {
        public int ComplianceYear { get; set; }

        public Guid SchemeId { get; set; }

        public GetReturnValueFromRemovingPCSRecords(Guid schemeId, int complianceYear)
        {
            SchemeId = schemeId;
            ComplianceYear = complianceYear;
        }
    }
}
