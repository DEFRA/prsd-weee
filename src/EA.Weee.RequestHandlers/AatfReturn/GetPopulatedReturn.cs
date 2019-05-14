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
    using NonObligated;
    using ObligatedSentOn;
    using Prsd.Core.Mapper;
    using Security;
    using Specification;

    public class GetPopulatedReturn : IGetPopulatedReturn
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IMap<ReturnQuarterWindow, ReturnData> mapper;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly INonObligatedDataAccess nonObligatedDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess obligatedDataAccess;
        private readonly IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess;
        private readonly IFetchAatfByOrganisationIdDataAccess aatfDataAccess;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly IGenericDataAccess genericDataAccess;

        public GetPopulatedReturn(IWeeeAuthorization authorization, 
            IReturnDataAccess returnDataAccess,  
            IMap<ReturnQuarterWindow, ReturnData> mapper, 
            IQuarterWindowFactory quarterWindowFactory,
            INonObligatedDataAccess nonObligatedDataAccess, 
            IFetchObligatedWeeeForReturnDataAccess obligatedDataAccess,
            IWeeeSentOnDataAccess getSentOnAatfSiteDataAccess, 
            IFetchAatfByOrganisationIdDataAccess aatfDataAccess, 
            IReturnSchemeDataAccess returnSchemeDataAccess,
            IGenericDataAccess genericDataAccess)
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
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<ReturnData> GetReturnData(Guid returnId)
        {
            authorization.EnsureCanAccessExternalArea();

            return mapper.Map(await GetReturnQuarterWindow(returnId));
        }

        public async Task<ReturnQuarterWindow> GetReturnQuarterWindow(Guid returnId)
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

            var returnReportsOn = await genericDataAccess.GetManyByExpression(new ReturnReportOnByReturnIdSpecification(returnId));

            return new ReturnQuarterWindow(@return,
                quarterWindow,
                aatfList,
                returnNonObligatedValues,
                returnObligatedReceivedValues,
                returnObligatedReusedValues,
                @return.Operator,
                sentOn,
                returnSchemeList,
                returnReportsOn);
        }
    }
}
