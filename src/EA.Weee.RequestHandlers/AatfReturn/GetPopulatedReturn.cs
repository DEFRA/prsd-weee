namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using AatfTaskList;
    using CheckYourReturn;
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Factories;
    using ObligatedSentOn;
    using Prsd.Core.Mapper;
    using Security;

    public class GetPopulatedReturn : IGetPopulatedReturn
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IMap<ReturnQuarterWindow, ReturnData> mapper;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly IFetchNonObligatedWeeeForReturnDataAccess nonObligatedDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess obligatedDataAccess;
        private readonly ISentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchAatfByOrganisationIdDataAccess aatfDataAccess;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;

        public GetPopulatedReturn(IWeeeAuthorization authorization, 
            IReturnDataAccess returnDataAccess,  
            IMap<ReturnQuarterWindow, ReturnData> mapper, 
            IQuarterWindowFactory quarterWindowFactory, 
            IFetchNonObligatedWeeeForReturnDataAccess nonObligatedDataAccess, 
            IFetchObligatedWeeeForReturnDataAccess obligatedDataAccess, 
            ISentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess, 
            IFetchAatfByOrganisationIdDataAccess aatfDataAccess, 
            IReturnSchemeDataAccess returnSchemeDataAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.mapper = mapper;
            this.quarterWindowFactory = quarterWindowFactory;
            this.nonObligatedDataAccess = nonObligatedDataAccess;
            this.obligatedDataAccess = obligatedDataAccess;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.returnSchemeDataAccess = returnSchemeDataAccess;
        }

        public async Task<ReturnData> GetReturnData(Guid returnId)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(returnId);

            authorization.EnsureOrganisationAccess(@return.Operator.Organisation.Id);

            var quarterWindow = await quarterWindowFactory.GetAnnualQuarter(@return.Quarter);

            var returnNonObligatedValues = await nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(returnId);

            var returnObligatedReceivedValues = await obligatedDataAccess.FetchObligatedWeeeReceivedForReturn(returnId);

            var returnObligatedReusedValues = await obligatedDataAccess.FetchObligatedWeeeReusedForReturn(returnId);

            var aatfList = await aatfDataAccess.FetchAatfByOrganisationId(@return.Operator.Organisation.Id);

            var sentOn = await obligatedDataAccess.FetchObligatedWeeeSentOnForReturnByReturn(returnId);

            var returnSchemeList = await returnSchemeDataAccess.GetSelectedSchemesByReturnId(returnId);

            var returnQuarterWindow = new ReturnQuarterWindow(@return,
                quarterWindow,
                aatfList,
                returnNonObligatedValues,
                returnObligatedReceivedValues,
                returnObligatedReusedValues,
                @return.Operator,
                sentOn,
                returnSchemeList);

            return mapper.Map(returnQuarterWindow);
        }
    }
}
