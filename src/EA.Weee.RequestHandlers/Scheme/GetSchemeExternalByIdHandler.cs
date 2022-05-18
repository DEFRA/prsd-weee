namespace EA.Weee.RequestHandlers.Scheme
{
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using System;
    using System.Threading.Tasks;

    internal class GetSchemeExternalByIdHandler : IRequestHandler<GetSchemeExternalById, SchemeData>
    {
         private readonly ISchemeDataAccess dataAccess;
         private readonly IWeeeAuthorization authorization;
         private readonly IMapper mapper;

        public GetSchemeExternalByIdHandler(ISchemeDataAccess dataAccess,
          IMapper mapper,
          IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.mapper = mapper;
            this.authorization = authorization;
        }

        public async Task<SchemeData> HandleAsync(GetSchemeExternalById request)
        {
            authorization.EnsureCanAccessExternalArea();

            var scheme = await dataAccess.GetSchemeOrDefault(request.SchemeId);

            if (scheme == null)
            {
                string message = string.Format("No scheme was found with id \"{0}\".", request.SchemeId);
                throw new ArgumentException(message);
            }

            return mapper.Map<Scheme, SchemeData>(scheme);
        }
    }
}
