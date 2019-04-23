namespace EA.Weee.RequestHandlers.Charges.FetchPendingCharges
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Charges;
    using Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.Scheme;
    using Security;
    using Weee.Security;

    public class FetchPendingChargesHandler : IRequestHandler<Requests.Charges.FetchPendingCharges, ManagePendingCharges>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ICommonDataAccess dataAccess;

        public FetchPendingChargesHandler(
            IWeeeAuthorization authorization,
            ICommonDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<ManagePendingCharges> HandleAsync(Requests.Charges.FetchPendingCharges message)
        {
            authorization.EnsureCanAccessInternalArea(true);

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthority(message.Authority);

            IReadOnlyList<MemberUpload> memberUploads = await dataAccess.FetchSubmittedNonInvoicedMemberUploadsAsync(authority);

            var groups = memberUploads.GroupBy(mu => new { mu.Scheme, mu.ComplianceYear });

            List<PendingCharge> pendingCharges = new List<PendingCharge>();
            foreach (var group in groups)
            {
                PendingCharge pendingCharge = new PendingCharge()
                {
                    SchemeName = group.Key.Scheme.SchemeName,
                    SchemeApprovalNumber = group.Key.Scheme.ApprovalNumber,
                    ComplianceYear = group.Key.ComplianceYear.Value,
                    TotalGBP = group.Sum(p => p.TotalCharges),
                    SubmittedDate = group.FirstOrDefault().SubmittedDate.Value
                };

                pendingCharges.Add(pendingCharge);
            }

            var managePendingCharges = new ManagePendingCharges
            {
                PendingCharges = pendingCharges,
                CanUserIssueCharges = authorization.CheckUserInRole(Roles.InternalAdmin)
            };

            return managePendingCharges;
        }
    }
}
