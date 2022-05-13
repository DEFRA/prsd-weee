namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Requests.AatfReturn;
    using Prsd.Core.Mapper;
    using Specification;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class GetAatfByOrganisationFacilityTypeHandler : IRequestHandler<GetAatfByOrganisationFacilityType, List<AatfData>>
    {
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMap<Aatf, AatfData> mapper;

        public GetAatfByOrganisationFacilityTypeHandler(IMap<Aatf, AatfData> mapper, IGenericDataAccess genericDataAccess)
        {
            this.mapper = mapper;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<List<AatfData>> HandleAsync(GetAatfByOrganisationFacilityType message)
        {
            var aatfs = await genericDataAccess.GetManyByExpression(new AatfsByOrganisationAndFacilityTypeSpecification(message.OrganisationId, message.FacilityType));

            return aatfs.Select(a => mapper.Map(a)).ToList();
        }
    }
}
