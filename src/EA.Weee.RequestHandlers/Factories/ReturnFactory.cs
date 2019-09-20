﻿namespace EA.Weee.RequestHandlers.Factories
{
    using Core.AatfReturn;
    using Core.DataReturns;
    using DataAccess.DataAccess;
    using Prsd.Core;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

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

        public async Task<Quarter> GetReturnQuarter(Guid organisationId, FacilityType facilityType)
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
                var year = quarterWindow.QuarterType == Domain.DataReturns.QuarterType.Q4
                    ? quarterWindow.StartDate.Year - 1
                    : quarterWindow.StartDate.Year;

                var hasAatfWithApprovalDate = await returnFactoryDataAccess.ValidateAatfApprovalDate(organisationId, quarterWindow.StartDate, year, facilityType);
                var hasReturnExists = await returnFactoryDataAccess.HasReturnQuarter(organisationId, year, quarterWindow.QuarterType, facilityType);

                if (hasAatfWithApprovalDate && !hasReturnExists)
                {
                    return new Quarter(year, (QuarterType)quarterWindow.QuarterType);
                }
            }

            return null;
        }
    }
}
