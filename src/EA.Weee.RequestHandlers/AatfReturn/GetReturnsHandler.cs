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
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.AatfReturn;
    using Security;
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;

    internal class GetReturnsHandler : IRequestHandler<GetReturns, IList<ReturnData>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetPopulatedReturn getPopulatedReturn;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IFetchAatfByOrganisationIdDataAccess aatfDataAccess;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetReturnsHandler(IWeeeAuthorization authorization,
            IGetPopulatedReturn getPopulatedReturn, 
            IReturnDataAccess returnDataAccess, 
            IFetchAatfByOrganisationIdDataAccess aatfDataAccess, 
            IQuarterWindowFactory quarterWindowFactory, 
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.getPopulatedReturn = getPopulatedReturn;
            this.returnDataAccess = returnDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.quarterWindowFactory = quarterWindowFactory;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<IList<ReturnData>> HandleAsync(GetReturns message)
        {
            authorization.EnsureCanAccessExternalArea();

            var @returns = await returnDataAccess.GetByOrganisationId(message.OrganisationId);

            var aatfList = await aatfDataAccess.FetchAatfByOrganisationId(message.OrganisationId);

            var currentDate = SystemTime.Now;
            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedCurrentDate)
            {
                currentDate = systemSettings.FixedCurrentDate;
            }

            var availableQuarterWindows = await quarterWindowFactory.GetQuarterWindowsForDate(currentDate);

            var returnsData = new List<ReturnData>();

            foreach (var @return in @returns)
            {
                returnsData.Add(await getPopulatedReturn.GetReturnData(@return.Id));
            }

            return returnsData;
        }
    }
}