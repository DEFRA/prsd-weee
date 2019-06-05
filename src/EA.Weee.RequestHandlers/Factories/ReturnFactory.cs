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

            var aatfs = await returnFactoryDataAccess.FetchAatfsByOrganisationFacilityTypeListAndYear(organisationId, currentDate.Year, facilityType);

            if (!aatfs.Any())
            {
                return null;
            }

            var availableQuarterWindows = await quarterWindowFactory.GetQuarterWindowsForDate(currentDate);

            if (!availableQuarterWindows.Any())
            {
                return null;
            }

            return new ReturnQuarter(2010, QuarterType.Q1);
        }
    }
}
