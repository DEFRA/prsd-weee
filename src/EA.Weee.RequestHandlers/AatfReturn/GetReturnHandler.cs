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
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
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
        private readonly ISentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchAatfByOrganisationIdDataAccess aatfDataAccess;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;

        public GetReturnHandler(IWeeeAuthorization authorization,
            IReturnDataAccess returnDataAccess,
            IOrganisationDataAccess organisationDataAccess,
            IMap<ReturnQuarterWindow, ReturnData> mapper,
            IQuarterWindowFactory quarterWindowFactory, 
            IFetchNonObligatedWeeeForReturnDataAccess nonObligatedDataAccess,
            IFetchObligatedWeeeForReturnDataAccess obligatedDataAccess,
            IFetchAatfByOrganisationIdDataAccess aatfDataAccess, 
            ISentOnAatfSiteDataAccess sentOnAatfSiteDataAccess,
            IReturnSchemeDataAccess returnSchemeDataAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.organisationDataAccess = organisationDataAccess;
            this.mapper = mapper;
            this.quarterWindowFactory = quarterWindowFactory;
            this.nonObligatedDataAccess = nonObligatedDataAccess;
            this.obligatedDataAccess = obligatedDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.getSentOnAatfSiteDataAccess = sentOnAatfSiteDataAccess;
             this.returnSchemeDataAccess = returnSchemeDataAccess;
        }

        public async Task<ReturnData> HandleAsync(GetReturn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(message.ReturnId);

            authorization.EnsureOrganisationAccess(@return.Operator.Organisation.Id);

            var quarterWindow = await quarterWindowFactory.GetAnnualQuarter(@return.Quarter);

            var returnNonObligatedValues = await nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(message.ReturnId);

            var returnObligatedReceivedValues = await obligatedDataAccess.FetchObligatedWeeeReceivedForReturn(message.ReturnId);

            var returnObligatedReusedValues = await obligatedDataAccess.FetchObligatedWeeeReusedForReturn(message.ReturnId);

            var aatfList = await aatfDataAccess.FetchAatfByOrganisationId(@return.Operator.Organisation.Id);

            var returnObligatedSentOnValues = new List<WeeeSentOnAmount>();

            var sentOn = await obligatedDataAccess.FetchObligatedWeeeSentOnForReturnByReturn(message.ReturnId);
            
            var returnSchemeList = await returnSchemeDataAccess.GetSelectedSchemesByReturnId(message.ReturnId);

            var returnQuarterWindow = new ReturnQuarterWindow(@return, 
                quarterWindow, 
                aatfList, 
                returnNonObligatedValues, 
                returnObligatedReceivedValues, 
                returnObligatedReusedValues,
                @return.Operator,
                sentOn,
                returnSchemeList);

            var result = mapper.Map(returnQuarterWindow);

            return result;
        }
    }
}