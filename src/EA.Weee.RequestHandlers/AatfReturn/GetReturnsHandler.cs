namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using Factories;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FacilityType = Core.AatfReturn.FacilityType;

    internal class GetReturnsHandler : IRequestHandler<GetReturns, ReturnsData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetPopulatedReturn getPopulatedReturn;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IReturnFactory returnFactory;

        public GetReturnsHandler(IWeeeAuthorization authorization,
            IGetPopulatedReturn getPopulatedReturn, 
            IReturnDataAccess returnDataAccess, 
            IReturnFactory returnFactory)
        {
            this.authorization = authorization;
            this.getPopulatedReturn = getPopulatedReturn;
            this.returnDataAccess = returnDataAccess;
            this.returnFactory = returnFactory;
        }

        public async Task<ReturnsData> HandleAsync(GetReturns message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @returns = await returnDataAccess.GetByOrganisationId(message.OrganisationId);

            var quarter = await returnFactory.GetReturnQuarter(message.OrganisationId, message.Facility);

            var returnsData = new List<ReturnData>();

            foreach (var @return in @returns.Where(p => p.FacilityType.Value == (int)message.Facility))
            {
                returnsData.Add(await getPopulatedReturn.GetReturnData(@return.Id));
            }

            return new ReturnsData(returnsData, quarter);
        }
    }
}