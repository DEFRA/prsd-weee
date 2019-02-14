namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn;
    using Factories;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;

    internal class GetReturnHandler : IRequestHandler<GetReturn, ReturnData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IMap<ReturnQuarterWindow, ReturnData> mapper;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly IFetchNonObligatedWeeeForReturnDataAccess nonObligatedDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess obligatedDataAccess;

        public GetReturnHandler(IWeeeAuthorization authorization,
            IReturnDataAccess returnDataAccess,
            IOrganisationDataAccess organisationDataAccess,
            IMap<ReturnQuarterWindow, ReturnData> mapper,
            IQuarterWindowFactory quarterWindowFactory, 
            IFetchNonObligatedWeeeForReturnDataAccess nonObligatedDataAccess,
            IFetchObligatedWeeeForReturnDataAccess obligatedDataAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.organisationDataAccess = organisationDataAccess;
            this.mapper = mapper;
            this.quarterWindowFactory = quarterWindowFactory;
            this.nonObligatedDataAccess = nonObligatedDataAccess;
            this.obligatedDataAccess = obligatedDataAccess;
        }

        public async Task<ReturnData> HandleAsync(GetReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(message.ReturnId);

            authorization.EnsureOrganisationAccess(@return.Operator.Organisation.Id);

            var quarterWindow = await quarterWindowFactory.GetAnnualQuarter(@return.Quarter);

            var returnNonObligatedValues = await nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(message.ReturnId);

            var returnObligatedValues = await obligatedDataAccess.FetchObligatedWeeeForReturn(message.ReturnId);

            var returnQuarterWindow = new ReturnQuarterWindow(@return, quarterWindow, returnNonObligatedValues, returnObligatedValues, @return.Operator);

            return mapper.Map(returnQuarterWindow);
        }
    }
}
