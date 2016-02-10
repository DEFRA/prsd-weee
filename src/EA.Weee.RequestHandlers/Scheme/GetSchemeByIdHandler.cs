namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Core.Security;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    internal class GetSchemeByIdHandler : IRequestHandler<GetSchemeById, SchemeData>
    {
        private readonly IGetSchemeByIdDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        private readonly IMap<Scheme, SchemeData> schemeMap;

        public GetSchemeByIdHandler(
            IGetSchemeByIdDataAccess dataAccess,
            IMap<Scheme, SchemeData> schemeMap,
            IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.schemeMap = schemeMap;
            this.authorization = authorization;
        }

        public async Task<SchemeData> HandleAsync(GetSchemeById request)
        {
            authorization.EnsureCanAccessInternalArea();

            var scheme = await dataAccess.GetSchemeOrDefault(request.SchemeId);
            
            if (scheme == null)
            {
                string message = string.Format("No scheme was found with id \"{0}\".", request.SchemeId);
                throw new ArgumentException(message);
            }
            SchemeData schemeData = schemeMap.Map(scheme);
            schemeData.CanEdit = authorization.CheckUserInRole(Roles.InternalAdmin);
            return schemeData;
        }
    }
}
