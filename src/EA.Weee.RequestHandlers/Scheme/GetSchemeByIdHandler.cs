namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Core.Scheme;
    using DataAccess;
    using Domain.Scheme;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    internal class GetSchemeByIdHandler : IRequestHandler<GetSchemeById, SchemeData>
    {
        private readonly WeeeContext context;

        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetSchemeByIdHandler(WeeeContext context, IMap<Scheme, SchemeData> schemeMap)
        {
            this.context = context;
            this.schemeMap = schemeMap;
        }

        public async Task<SchemeData> HandleAsync(GetSchemeById message)
        {
            var scheme = await context.Schemes.SingleOrDefaultAsync(m => m.Id == message.SchemeId);

            if (scheme == null)
            {
                throw new ArgumentNullException(string.Format("Could not find a Scheme with id {0}", message.SchemeId));
            }

            return schemeMap.Map(scheme);
        }
    }
}
