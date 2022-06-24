namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using Domain;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Prsd.Core;

    public class SystemDataDataAccess : ISystemDataDataAccess
    {
        private readonly WeeeContext context;

        public SystemDataDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<SystemData> Get()
        {
            return await context.SystemData.SingleAsync();
        }

        public async Task<DateTime> GetSystemDateTime()
        {
            var currentDate = SystemTime.UtcNow;
            var systemData = await Get();

            if (systemData.UseFixedCurrentDate)
            {
                currentDate = systemData.FixedCurrentDate;
            }

            return currentDate;
        }
    }
}
