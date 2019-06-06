namespace EA.Weee.RequestHandlers.AatfReturn
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetReturnsHandler : IRequestHandler<GetReturns, IList<ReturnData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetPopulatedReturn getPopulatedReturn;
        private readonly IReturnDataAccess returnDataAccess;

        public GetReturnsHandler(IWeeeAuthorization authorization,
            IGetPopulatedReturn getPopulatedReturn, 
            IReturnDataAccess returnDataAccess)
        {
            this.authorization = authorization;
            this.getPopulatedReturn = getPopulatedReturn;
            this.returnDataAccess = returnDataAccess;
        }

        public async Task<IList<ReturnData>> HandleAsync(GetReturns message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @returns = await returnDataAccess.GetByOrganisationId(message.OrganisationId);

            var returnsData = new List<ReturnData>();

            foreach (var @return in @returns.Where(p => p.FacilityType.Value == (int)message.Facility))
            {
                returnsData.Add(await getPopulatedReturn.GetReturnData(@return.Id));
            }

            return returnsData;
        }
    }
}