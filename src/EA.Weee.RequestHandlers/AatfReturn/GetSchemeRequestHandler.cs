namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Scheme = Domain.Scheme.Scheme;

    public class GetSchemeRequestHandler : IRequestHandler<GetReturnScheme, List<SchemeData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly RequestHandlers.Scheme.IGetSchemesDataAccess schemeDataAccess;
        private readonly IMapper mapper;
  
        public GetSchemeRequestHandler(IWeeeAuthorization authorization, IReturnSchemeDataAccess returnSchemeDataAccess, RequestHandlers.Scheme.IGetSchemesDataAccess schemeDataAccess, IMapper mapper)
        {
            this.authorization = authorization;
            this.returnSchemeDataAccess = returnSchemeDataAccess;
            this.schemeDataAccess = schemeDataAccess;
            this.mapper = mapper;
        }

        public async Task<List<SchemeData>> HandleAsync(GetReturnScheme message)
        {
            authorization.EnsureCanAccessExternalArea();

            List<Scheme> schemeList = new List<Scheme>();
            List<SchemeData> schemeListData = new List<SchemeData>();
            List<Guid> schemeIdList = await returnSchemeDataAccess.GetSelectedSchemesByReturnId(message.ReturnId);

            foreach (var schemeId in schemeIdList)
            {
                Scheme scheme = await schemeDataAccess.GetSchemeBasedOnId(schemeId);
                schemeList.Add(scheme);
            }

            foreach (var scheme in schemeList)
            {
                SchemeData data = mapper.Map<Scheme, SchemeData>(scheme);
                schemeListData.Add(data);
            }

            return schemeListData;           
        }
    }
}
