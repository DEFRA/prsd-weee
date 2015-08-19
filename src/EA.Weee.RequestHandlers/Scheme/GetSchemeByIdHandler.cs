namespace EA.Weee.RequestHandlers.Scheme
{
    using Core.Scheme;
    using DataAccess;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class GetSchemeByIdHandler : IRequestHandler<GetSchemeById, SchemeData>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;

        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetSchemeByIdHandler(WeeeContext context, IMap<Scheme, SchemeData> schemeMap, IWeeeAuthorization authorization)
        {
            this.context = context;
            this.schemeMap = schemeMap;
            this.authorization = authorization;
        }

        public async Task<SchemeData> HandleAsync(GetSchemeById message)
        {
            authorization.EnsureCanAccessInternalArea();

            var scheme = await context.Schemes.SingleOrDefaultAsync(m => m.Id == message.SchemeId);

            if (scheme == null)
            {
                throw new ArgumentNullException(string.Format("Could not find a Scheme with id {0}", message.SchemeId));
            }

            return schemeMap.Map(scheme);
        }
    }
}
