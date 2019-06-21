namespace EA.Weee.RequestHandlers.Charges.FetchSchemesWithInvoices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Scheme;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Shared;

    public class FetchSchemesWithInvoicesHandler : IRequestHandler<Requests.Charges.FetchSchemesWithInvoices, IReadOnlyList<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ICommonDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;

        public FetchSchemesWithInvoicesHandler(
            IWeeeAuthorization authorization,
            ICommonDataAccess dataAccess, 
            IMap<Scheme, SchemeData> schemeMap)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.schemeMap = schemeMap;
        }

        public async Task<IReadOnlyList<SchemeData>> HandleAsync(Requests.Charges.FetchSchemesWithInvoices message)
        {
            authorization.EnsureCanAccessInternalArea();

            UKCompetentAuthority authority = await dataAccess.FetchCompetentAuthority(message.Authority);

            IEnumerable<MemberUpload> invoicedMemberUploads = await dataAccess.FetchInvoicedMemberUploadsAsync(authority);

            return invoicedMemberUploads
                .GroupBy(s => new { s.Scheme.Id, s.Scheme.SchemeName, s.Scheme })
                .Select(s => schemeMap.Map(s.Key.Scheme))
                .OrderBy(x => x.SchemeName).ToList();
        }
    }
}
