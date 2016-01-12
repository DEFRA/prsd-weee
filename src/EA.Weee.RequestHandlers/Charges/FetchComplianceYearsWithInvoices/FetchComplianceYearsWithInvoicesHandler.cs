namespace EA.Weee.RequestHandlers.Charges.FetchComplianceYearsWithInvoices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    
    public class FetchComplianceYearsWithInvoicesHandler : IRequestHandler<Requests.Charges.FetchComplianceYearsWithInvoices, IReadOnlyList<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ICommonDataAccess dataAccess;

        public FetchComplianceYearsWithInvoicesHandler(
            IWeeeAuthorization authorization,
            ICommonDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<IReadOnlyList<int>> HandleAsync(Requests.Charges.FetchComplianceYearsWithInvoices message)
        {
            authorization.EnsureCanAccessInternalArea();

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthority(message.Authority);

            IEnumerable<MemberUpload> invoicedMemberUploads = await dataAccess.FetchInvoicedMemberUploadsAsync(authority);

            return invoicedMemberUploads
                .Select(mu => mu.ComplianceYear.Value)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
    }
}
