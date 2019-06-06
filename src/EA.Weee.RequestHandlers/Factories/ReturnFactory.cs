namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.DataReturns;
    using DataAccess.DataAccess;
    using Prsd.Core;

    public class ReturnFactory : IReturnFactory
    {
        private readonly IReturnFactoryDataAccess returnFactoryDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly IQuarterWindowFactory quarterWindowFactory;

        public ReturnFactory(IReturnFactoryDataAccess returnFactoryDataAccess, 
            ISystemDataDataAccess systemDataDataAccess, 
            IQuarterWindowFactory quarterWindowFactory)
        {
            this.returnFactoryDataAccess = returnFactoryDataAccess;
            this.systemDataDataAccess = systemDataDataAccess;
            this.quarterWindowFactory = quarterWindowFactory;
        }

        public async Task<ReturnQuarter> GetReturnQuarter(Guid organisationId, FacilityType facilityType)
        {
            var currentDate = SystemTime.Now;
            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedCurrentDate)
            {
                currentDate = systemSettings.FixedCurrentDate;
            }

            var availableQuarterWindows = await quarterWindowFactory.GetQuarterWindowsForDate(currentDate);

            if (!availableQuarterWindows.Any())
            {
                return null;
            }

            var windowDates = availableQuarterWindows.OrderBy(a => a.StartDate);

            foreach (var quarterWindow in windowDates)
            {
                var date = quarterWindow.StartDate;

                var hasAatfWithApprovalDate = await returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, date, facilityType);
                var hasReturnExists = await returnFactoryDataAccess.HasReturnQuarter(organisationId, date.Year, quarterWindow.QuarterType, facilityType);

                if (hasAatfWithApprovalDate && !hasReturnExists)
                {
                    return new ReturnQuarter(date.Year, (QuarterType)quarterWindow.QuarterType);
                }
            }

            return null;
        }
    }
}
