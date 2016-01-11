namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.DataAccess;
    using Domain.DataReturns;

    public class QuarterFactory : IQuarterFactory
    {
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly WeeeContext context;

        public QuarterFactory(ISystemDataDataAccess systemDataDataAccess, WeeeContext context)
        {
            this.systemDataDataAccess = systemDataDataAccess;
            this.context = context;
        }

        public async Task SetFixedQuarter(Quarter quarter)
        {
            var systemData = await systemDataDataAccess.Get();

            systemData.UpdateQuarterAndComplianceYear(quarter);

            await context.SaveChangesAsync();
        }

        public async Task ToggleFixedQuarterUseage(bool enabled)
        {
            var systemData = await systemDataDataAccess.Get();

            systemData.ToggleFixedQuarterAndComplianceYearUsage(enabled);

            await context.SaveChangesAsync();
        }
    }
}
