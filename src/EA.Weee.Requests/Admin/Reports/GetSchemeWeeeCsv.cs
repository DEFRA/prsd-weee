namespace EA.Weee.Requests.Admin.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetSchemeWeeeCsv : IRequest<FileInfo>
    {
        public int ComplianceYear { get; private set; }

        public ObligationType ObligationType { get; private set; }

        public GetSchemeWeeeCsv(int complianceYear, ObligationType obligationType)
        {
            ComplianceYear = complianceYear;
            ObligationType = obligationType;
        }
    }
}
