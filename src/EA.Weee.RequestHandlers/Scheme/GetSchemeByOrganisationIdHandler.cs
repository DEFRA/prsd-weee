namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Threading.Tasks;
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using Weee.Security;

    internal class GetSchemeByOrganisationIdHandler : IRequestHandler<GetSchemeByOrganisationId, SchemeData>
        {
            private readonly ISchemeDataAccess dataAccess;
            private readonly IWeeeAuthorization authorization;
            private readonly IMapper mapper;

            public GetSchemeByOrganisationIdHandler(ISchemeDataAccess dataAccess,
                IMapper mapper,
                IWeeeAuthorization authorization)
            {
                this.dataAccess = dataAccess;
                this.mapper = mapper;
                this.authorization = authorization;
            }

            public async Task<SchemeData> HandleAsync(GetSchemeByOrganisationId request)
            {
                authorization.EnsureInternalOrOrganisationAccess(request.OrganisationId);

                var scheme = await dataAccess.GetSchemeOrDefaultByOrganisationId(request.OrganisationId);

                if (scheme == null)
                {
                    var message = $"No scheme was found with organisation id \"{request.OrganisationId}\".";
                    throw new ArgumentException(message);
                }

                var schemeData = mapper.Map<Scheme, SchemeData>(scheme);

                return schemeData;
            }
        }      
}
