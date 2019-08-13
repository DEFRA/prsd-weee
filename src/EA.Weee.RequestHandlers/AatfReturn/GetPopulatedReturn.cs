namespace EA.Weee.RequestHandlers.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AatfTaskList;
    using CheckYourReturn;
    using Core.AatfReturn;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Factories;
    using NonObligated;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Security;
    using Specification;
    using ReturnStatus = Core.AatfReturn.ReturnStatus;

    public class GetPopulatedReturn : IGetPopulatedReturn
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IMap<ReturnQuarterWindow, ReturnData> mapper;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly INonObligatedDataAccess nonObligatedDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess obligatedDataAccess;
        private readonly IFetchAatfDataAccess aatfDataAccess;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetPopulatedReturn(IWeeeAuthorization authorization, 
            IReturnDataAccess returnDataAccess,  
            IMap<ReturnQuarterWindow, ReturnData> mapper, 
            IQuarterWindowFactory quarterWindowFactory,
            INonObligatedDataAccess nonObligatedDataAccess, 
            IFetchObligatedWeeeForReturnDataAccess obligatedDataAccess, 
            IFetchAatfDataAccess aatfDataAccess, 
            IReturnSchemeDataAccess returnSchemeDataAccess,
            IGenericDataAccess genericDataAccess, ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.returnDataAccess = returnDataAccess;
            this.mapper = mapper;
            this.quarterWindowFactory = quarterWindowFactory;
            this.nonObligatedDataAccess = nonObligatedDataAccess;
            this.obligatedDataAccess = obligatedDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.returnSchemeDataAccess = returnSchemeDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<ReturnData> GetReturnData(Guid returnId, bool forSummary)
        {
            authorization.EnsureCanAccessExternalArea();

            var @return = await returnDataAccess.GetById(returnId);

            if (@return == null)
            {
                throw new ArgumentException($"No return found for return id {returnId}");
            }

            authorization.EnsureOrganisationAccess(@return.Organisation.Id);

            var annualQuarterWindow = await quarterWindowFactory.GetQuarterWindow(@return.Quarter);

            var returnNonObligatedValues = await nonObligatedDataAccess.FetchNonObligatedWeeeForReturn(returnId);

            var returnObligatedReceivedValues = await obligatedDataAccess.FetchObligatedWeeeReceivedForReturn(returnId);

            var returnObligatedReusedValues = await obligatedDataAccess.FetchObligatedWeeeReusedForReturn(returnId);

            List<Aatf> aatfs;
            if (forSummary && @return.ReturnStatus.Equals(EA.Weee.Domain.AatfReturn.ReturnStatus.Submitted))
            {
                aatfs = await aatfDataAccess.FetchAatfByReturnId(@return.Id);
            }
            else
            {
                aatfs = await aatfDataAccess.FetchAatfByReturnQuarterWindow(@return);
            }

            var sentOn = await obligatedDataAccess.FetchObligatedWeeeSentOnForReturnByReturn(returnId);

            var returnSchemeList = await returnSchemeDataAccess.GetSelectedSchemesByReturnId(returnId);

            var returnReportsOn = await genericDataAccess.GetManyByExpression(new ReturnReportOnByReturnIdSpecification(returnId));

            var currentDate = SystemTime.Now;
            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedCurrentDate)
            {
                currentDate = systemSettings.FixedCurrentDate;
            }

            var returnQuarterWindow = new ReturnQuarterWindow(@return,
                annualQuarterWindow,
                aatfs,
                returnNonObligatedValues,
                returnObligatedReceivedValues,
                returnObligatedReusedValues,
                @return.Organisation,
                sentOn,
                returnSchemeList,
                returnReportsOn,
                currentDate);

            return mapper.Map(returnQuarterWindow);
        }
    }
}
