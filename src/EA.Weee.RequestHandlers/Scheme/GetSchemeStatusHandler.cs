namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Core.Shared;
using DataAccess;
using EA.Weee.RequestHandlers.Security;
using Prsd.Core.Mapper;
using Prsd.Core.Mediator;
using Requests.Scheme;

    public class GetSchemeStatusHandler : IRequestHandler<GetSchemeStatus, SchemeStatus>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IMap<Domain.Scheme.SchemeStatus, SchemeStatus> mapper;

        public GetSchemeStatusHandler(WeeeContext context,
            IWeeeAuthorization authorization,
            IMap<Domain.Scheme.SchemeStatus,
            SchemeStatus> mapper)
        {
            this.context = context;
            this.authorization = authorization;
            this.mapper = mapper;
        }

        public async Task<SchemeStatus> HandleAsync(GetSchemeStatus message)
        {
            authorization.EnsureOrganisationAccess(message.PcsId);

            var scheme = await context.Schemes
                .SingleOrDefaultAsync(s => s.OrganisationId == message.PcsId);

            if (scheme != null)
            {
                return mapper.Map(scheme.SchemeStatus);
            }

            throw new InvalidOperationException(string.Format("Scheme with Id '{0}' does not exist", message.PcsId));
        }
    }
}
