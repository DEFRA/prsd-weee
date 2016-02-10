namespace EA.Weee.RequestHandlers.Admin.GetSchemes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Security;

    public class GetSchemesHandler : IRequestHandler<Requests.Admin.GetSchemes, List<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetSchemesHandler(IWeeeAuthorization authorization,  IMap<Scheme, SchemeData> schemeMap, IGetSchemesDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.schemeMap = schemeMap;
        }

        public async Task<List<SchemeData>> HandleAsync(Requests.Admin.GetSchemes request)
        {
            authorization.EnsureCanAccessInternalArea();

            List<Scheme> schemes = await dataAccess.GetSchemes();

            Func<Scheme, bool> filter;
            switch (request.Filter)
            {
                case Requests.Admin.GetSchemes.FilterType.Approved:
                    filter = s => s.SchemeStatus == SchemeStatus.Approved;
                    break;

                case Requests.Admin.GetSchemes.FilterType.ApprovedOrWithdrawn:
                    filter = s => (s.SchemeStatus == SchemeStatus.Approved) || (s.SchemeStatus == SchemeStatus.Withdrawn);
                    break;

                default:
                    throw new NotSupportedException();
            }

            return schemes
                .Where(filter)
                .OrderBy(s => s.SchemeName)
                .Select(s => schemeMap.Map(s))
                .ToList();
        }
    }
}
