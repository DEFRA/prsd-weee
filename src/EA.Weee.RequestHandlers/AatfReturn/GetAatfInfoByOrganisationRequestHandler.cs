namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using Specification;
    using Scheme = Domain.Scheme.Scheme;

    public class GetAatfInfoByOrganisationRequestHandler : IRequestHandler<GetAatfByOrganisation, List<AatfData>>
    {
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMap<Aatf, AatfData> mapper;
  
        public GetAatfInfoByOrganisationRequestHandler(IMap<Aatf, AatfData> mapper, IGenericDataAccess genericDataAccess)
        {
            this.mapper = mapper;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<List<AatfData>> HandleAsync(GetAatfByOrganisation message)
        {
            var aatfs = await genericDataAccess.GetManyByExpression(new AatfsByOrganisationSpecification(message.OrganisationId));

            return aatfs.Select(a => mapper.Map(a)).ToList();
        }
    }
}
