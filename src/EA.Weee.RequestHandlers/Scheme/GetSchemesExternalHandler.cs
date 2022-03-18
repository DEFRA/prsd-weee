namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using Core.Scheme;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetSchemesExternalHandler : IRequestHandler<GetSchemesExternal, List<SchemeData>>
    {
        private readonly IGetSchemesDataAccess dataAccess;
        private readonly IMap<Scheme, SchemeData> schemeMap;
        private readonly IWeeeAuthorization authorization;

        public GetSchemesExternalHandler(
            IGetSchemesDataAccess dataAccess,
            IMap<Scheme, SchemeData> schemeMap,
            IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.schemeMap = schemeMap;
            this.authorization = authorization;
        }

        public async Task<List<SchemeData>> HandleAsync(GetSchemesExternal message)
        {
            authorization.EnsureCanAccessExternalArea();

            var schemes = await dataAccess.GetCompleteSchemes();

            Func<Scheme, bool> filter;
            if (message.IncludeWithdrawn)
            {
                filter = s => (s.SchemeStatus == SchemeStatus.Approved) || (s.SchemeStatus == SchemeStatus.Withdrawn);
            }
            else
            {
                filter = s => s.SchemeStatus == SchemeStatus.Approved;
            }
           
            return schemes.Where(filter)
                .Select(s => schemeMap.Map(s))
                .OrderBy(sd => sd.SchemeName)
                .ToList();
        }
    }
}
