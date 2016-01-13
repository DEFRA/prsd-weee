namespace EA.Weee.RequestHandlers.Charges.FetchSchemesWithInvoices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;

    public class FetchSchemesWithInvoicesHandler : IRequestHandler<Requests.Charges.FetchSchemesWithInvoices, IReadOnlyList<string>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ICommonDataAccess dataAccess;

        public FetchSchemesWithInvoicesHandler(
            IWeeeAuthorization authorization,
            ICommonDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<IReadOnlyList<string>> HandleAsync(Requests.Charges.FetchSchemesWithInvoices message)
        {
            authorization.EnsureCanAccessInternalArea();

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthority(message.Authority);

            IEnumerable<MemberUpload> invoicedMemberUploads = await dataAccess.FetchInvoicedMemberUploadsAsync(authority);

            return invoicedMemberUploads
                .Select(mu => mu.Scheme.SchemeName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
    }
}
