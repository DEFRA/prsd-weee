namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
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
        public List<NonObligatedCategoryValue> NonObligatedWeeeList = new List<NonObligatedCategoryValue>();

        public GetReturnHandler(IWeeeAuthorization authorization, 
            IReturnDataAccess returnDataAccess, 
            IOrganisationDataAccess organisationDataAccess, 
            IMap<ReturnQuarterWindow, ReturnData> mapper, 
            IQuarterWindowFactory quarterWindowFactory, IFetchNonObligatedWeeeForReturnDataAccess nonObligatedDataAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.organisationDataAccess = organisationDataAccess;
            this.mapper = mapper;
            this.quarterWindowFactory = quarterWindowFactory;
            this.nonObligatedDataAccess = nonObligatedDataAccess;
        }

        public async Task<ReturnData> HandleAsync(GetReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(message.ReturnId);

            authorization.EnsureOrganisationAccess(@return.Operator.Organisation.Id);

            var quarterWindow = await quarterWindowFactory.GetQuarterWindow(@return.Quarter);

            List<NonObligatedWeee> nonObligatedWeeeListHolder = await nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(message.ReturnId);

            foreach (var nonObligatedWeee in nonObligatedWeeeListHolder)
            {
                var buffer = new NonObligatedCategoryValue()
                {
                    Dcf = nonObligatedWeee.Dcf,
                    Tonnage = nonObligatedWeee.Tonnage.ToString(),
                    CategoryId = nonObligatedWeee.CategoryId,
                };

                NonObligatedWeeeList.Add(buffer);
            }

            return mapper.Map(new ReturnQuarterWindow(@return, quarterWindow, nonObligatedWeeeListHolder));
        }
    }
}
