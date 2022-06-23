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
    using Aatf;
    using DataAccess.DataAccess;
    using Mappings;
    using Prsd.Core;

    public class GetAatfInfoByOrganisationRequestHandler : IRequestHandler<GetAatfByOrganisation, List<AatfData>>
    {
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMap<AatfWithSystemDateMapperObject, AatfData> mapper;
        private readonly IWeeeAuthorization authorization;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetAatfInfoByOrganisationRequestHandler(IMap<AatfWithSystemDateMapperObject, AatfData> mapper, 
            IAatfDataAccess aatfDataAccess, 
            IWeeeAuthorization authorization, 
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.mapper = mapper;
            this.aatfDataAccess = aatfDataAccess;
            this.authorization = authorization;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<List<AatfData>> HandleAsync(GetAatfByOrganisation message)
        {
            authorization.EnsureInternalOrOrganisationAccess(message.OrganisationId);

            var systemSettings = await systemDataDataAccess.Get();
            var currentDate = SystemTime.Now;

            if (systemSettings.UseFixedCurrentDate)
            {
                currentDate = systemSettings.FixedCurrentDate;
            }

            var aatfs = await aatfDataAccess.GetAatfsForOrganisation(message.OrganisationId);

            return aatfs.Select(a => mapper.Map(new AatfWithSystemDateMapperObject(a, currentDate))).ToList();
        }
    }
}
