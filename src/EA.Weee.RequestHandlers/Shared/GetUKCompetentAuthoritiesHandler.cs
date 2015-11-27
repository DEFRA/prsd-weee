namespace EA.Weee.RequestHandlers.Shared
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Shared;

    internal class GetUKCompetentAuthoritiesHandler :
        IRequestHandler<GetUKCompetentAuthorities, IList<UKCompetentAuthorityData>>
    {
        private readonly WeeeContext context;
        private readonly IMap<UKCompetentAuthority, UKCompetentAuthorityData> mapper;

        public GetUKCompetentAuthoritiesHandler(WeeeContext context, IMap<UKCompetentAuthority, UKCompetentAuthorityData> mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IList<UKCompetentAuthorityData>> HandleAsync(GetUKCompetentAuthorities message)
        {
            var ukcompetentauthorities =
                await context.UKCompetentAuthorities.OrderBy(c => c.Abbreviation).ToArrayAsync();

            return ukcompetentauthorities.Select(mapper.Map).ToArray();
        }
    }
}