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
    using Scheme = Domain.Scheme.Scheme;

    public class GetAatfInfoByOrganisationRequestHandler : IRequestHandler<GetAatfInfoByOrganisation, List<AatfData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMapper mapper;
  
        public GetAatfInfoByOrganisationRequestHandler(IWeeeAuthorization authorization, IMapper mapper, IAatfDataAccess aatfDataAccess)
        {
            this.authorization = authorization;
            this.mapper = mapper;
            this.aatfDataAccess = aatfDataAccess;
        }

        public async Task<List<AatfData>> HandleAsync(GetAatfInfoByOrganisation message)
        {
            var aatfs = await aatfDataAccess.GetByOrganisationId(message.OrganisationId);

            //var mapped = mapper.Map<>(aatfs);
            return new List<AatfData>().ToList();
        }
    }
}
