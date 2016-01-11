namespace EA.Weee.RequestHandlers.Factories
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using DataAccess.DataAccess;

    public class DateFactory : IDateFactory
    {
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly WeeeContext context;

        public DateFactory(ISystemDataDataAccess systemDataDataAccess, WeeeContext context)
        {
            this.systemDataDataAccess = systemDataDataAccess;
            this.context = context;
        }

        public async Task SetFixedDate(DateTime date)
        {
            var systemData = await systemDataDataAccess.Get();

            systemData.UpdateFixedCurrentDate(date);

            await context.SaveChangesAsync();
        }

        public async Task ToggleFixedDateUsage(bool enabled)
        {
            var systemData = await systemDataDataAccess.Get();

            systemData.ToggleFixedCurrentDateUsage(enabled);

            await context.SaveChangesAsync();
        }
    }
}
