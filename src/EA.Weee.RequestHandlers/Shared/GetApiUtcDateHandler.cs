namespace EA.Weee.RequestHandlers.Shared
{
    using DataAccess.DataAccess;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.Shared;
    using System;
    using System.Threading.Tasks;

    internal class GetApiUtcDateHandler : IRequestHandler<GetApiUtcDate, DateTime>
    {
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetApiUtcDateHandler(ISystemDataDataAccess systemDataDataAccess)
        {
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<DateTime> HandleAsync(GetApiUtcDate query)
        {
            var currentDate = SystemTime.UtcNow;
            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedCurrentDate)
            {
                currentDate = systemSettings.FixedCurrentDate;
            }

            return currentDate;
        }
    }
}
