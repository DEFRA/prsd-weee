namespace EA.Weee.Requests.Admin.Obligations
{
    using System;
    using Core.Shared;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    public class SubmitSchemeObligation : IRequest<Guid>
    {
        public FileInfo FileInfo { get; private set; }
        public CompetentAuthority Authority { get; private set; }

        public int ComplianceYear { get; private set; }

        public SubmitSchemeObligation(FileInfo fileInfo, CompetentAuthority authority, int complianceYear)
        {
            Condition.Requires(fileInfo).IsNotNull();

            FileInfo = fileInfo;
            Authority = authority;
            ComplianceYear = complianceYear;
        }
    }
}
