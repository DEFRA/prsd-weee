namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.Organisations;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using Specification;
    using Scheme = Domain.Scheme.Scheme;

    public class GetSchemeRequestHandler : IRequestHandler<GetReturnScheme, SchemeDataList>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly IMapper mapper;
  
        public GetSchemeRequestHandler(IWeeeAuthorization authorization, 
            IReturnSchemeDataAccess returnSchemeDataAccess, 
            IMapper mapper)
        {
            this.authorization = authorization;
            this.returnSchemeDataAccess = returnSchemeDataAccess;
            this.mapper = mapper;
        }

        public async Task<SchemeDataList> HandleAsync(GetReturnScheme message)
        {
            authorization.EnsureCanAccessExternalArea();

            var schemeListData = new List<SchemeData>();

            var returnSchemeList = await returnSchemeDataAccess.GetSelectedSchemesByReturnId(message.ReturnId);

            var schemeDataList = returnSchemeList.Select(s => mapper.Map<Scheme, SchemeData>(s.Scheme)).OrderBy(sd => sd.SchemeName).ToList();

            var organisation = await returnSchemeDataAccess.GetOrganisationByReturnId(message.ReturnId);

            return new SchemeDataList(schemeDataList, mapper.Map<Organisation, OrganisationData>(organisation));
        }
    }
}
