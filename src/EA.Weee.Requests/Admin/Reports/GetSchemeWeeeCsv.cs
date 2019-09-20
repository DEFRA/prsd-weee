﻿namespace EA.Weee.Requests.Admin.Reports
{
    using Core.Shared;
    using Prsd.Core.Mediator;
    using System;

    public class GetSchemeWeeeCsv : IRequest<FileInfo>
    {
        public int ComplianceYear { get; private set; }

        public Guid? SchemeId { get; private set; }

        public ObligationType ObligationType { get; private set; }

        public GetSchemeWeeeCsv(int complianceYear, Guid? schemeId, ObligationType obligationType)
        {
            ComplianceYear = complianceYear;
            SchemeId = schemeId;
            ObligationType = obligationType;
        }
    }
}
