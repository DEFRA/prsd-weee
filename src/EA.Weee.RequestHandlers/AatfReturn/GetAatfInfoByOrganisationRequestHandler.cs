namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using Prsd.Core.Mapper;
    using Specification;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class GetAatfInfoByOrganisationRequestHandler : IRequestHandler<GetAatfByOrganisation, List<AatfData>>
    {
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMap<Aatf, AatfData> mapper;
        private readonly IWeeeAuthorization authorization;

        public GetAatfInfoByOrganisationRequestHandler(IMap<Aatf, AatfData> mapper, IGenericDataAccess genericDataAccess, IWeeeAuthorization authorization)
        {
            this.mapper = mapper;
            this.genericDataAccess = genericDataAccess;
            this.authorization = authorization;
        }

        public async Task<List<AatfData>> HandleAsync(GetAatfByOrganisation message)
        {
            authorization.EnsureInternalOrOrganisationAccess(message.OrganisationId);

            var aatfs = await genericDataAccess.GetManyByExpression(new AatfsByOrganisationSpecification(message.OrganisationId));

            return aatfs.Select(a => mapper.Map(a)).ToList();
        }
    }
}
