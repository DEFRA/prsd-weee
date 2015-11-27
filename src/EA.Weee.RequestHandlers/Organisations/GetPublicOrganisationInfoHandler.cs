namespace EA.Weee.RequestHandlers.Organisations
{
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class GetPublicOrganisationInfoHandler : IRequestHandler<GetPublicOrganisationInfo, PublicOrganisationData>
    {
        private readonly WeeeContext context;
        private readonly IMap<Organisation, PublicOrganisationData> publicOrganisationMap;

        public GetPublicOrganisationInfoHandler(WeeeContext context, IMap<Organisation, PublicOrganisationData> publicOrganisationMap)
        {   
            this.context = context;
            this.publicOrganisationMap = publicOrganisationMap;
        }

        public async Task<PublicOrganisationData> HandleAsync(GetPublicOrganisationInfo query)
        {   
            var org = await context.Organisations
                .SingleOrDefaultAsync(o => o.Id == query.Id);

            if (org == null)
            {
                throw new ArgumentException(string.Format("Could not find an organisation with id {0}",
                    query.Id));
            }

            return publicOrganisationMap.Map(org);
        }
    }
}
