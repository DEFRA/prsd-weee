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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FacilityType = Core.AatfReturn.FacilityType;
    using QuarterType = Core.DataReturns.QuarterType;

    internal class GetReturnsHandler : IRequestHandler<GetReturns, ReturnsData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetPopulatedReturn getPopulatedReturn;
        private readonly IReturnDataAccess returnDataAccess;
        private readonly IReturnFactory returnFactory;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetReturnsHandler(IWeeeAuthorization authorization,
            IGetPopulatedReturn getPopulatedReturn,
            IReturnDataAccess returnDataAccess,
            IReturnFactory returnFactory,
            IQuarterWindowFactory quarterWindowFactory,
            ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.getPopulatedReturn = getPopulatedReturn;
            this.returnDataAccess = returnDataAccess;
            this.returnFactory = returnFactory;
            this.quarterWindowFactory = quarterWindowFactory;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<ReturnsData> HandleAsync(GetReturns message)
        {
            authorization.EnsureCanAccessExternalArea();

            var currentDate = SystemTime.UtcNow;
            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedCurrentDate)
            {
                currentDate = systemSettings.FixedCurrentDate;
            }

            var @returns = await returnDataAccess.GetByOrganisationId(message.OrganisationId);

            var quarter = await returnFactory.GetReturnQuarter(message.OrganisationId, message.Facility);

            var openQuarters = await quarterWindowFactory.GetQuarterWindowsForDate(currentDate);

            var returnsData = new List<ReturnData>();

            foreach (var @return in @returns.Where(p => p.FacilityType.Value == (int)message.Facility))
            {
               returnsData.Add(await getPopulatedReturn.GetReturnData(@return.Id, false));
            }

            var returnOpenQuarters = new List<Core.DataReturns.Quarter>();

            foreach (var q in openQuarters)
            {
                returnOpenQuarters.Add(new Core.DataReturns.Quarter(q.StartDate.Year, (Core.DataReturns.QuarterType)q.QuarterType));
            }

            Core.AatfReturn.QuarterWindow nextQuarter = null;

            if (openQuarters.Count > 0)
            {
                var latestOpenQuarter = openQuarters.OrderByDescending(p => p.QuarterType).FirstOrDefault();

                var nextWindow = await quarterWindowFactory.GetNextQuarterWindow(latestOpenQuarter.QuarterType, latestOpenQuarter.StartDate.Year);

                nextQuarter = new Core.AatfReturn.QuarterWindow(nextWindow.StartDate, nextWindow.EndDate, (QuarterType)nextWindow.QuarterType);
            }

            return new ReturnsData(returnsData, quarter, returnOpenQuarters, nextQuarter, currentDate);
        }
    }
}