namespace EA.Weee.RequestHandlers.Shared
{
    using DataAccess.DataAccess;
    using Prsd.Core;
    using Prsd.Core.Mediator;
    using Requests.Shared;
    using System;
    using System.Threading.Tasks;

    internal class GetApiDateHandler : IRequestHandler<GetApiDate, DateTime>
    {
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetApiDateHandler(ISystemDataDataAccess systemDataDataAccess)
        {
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<DateTime> HandleAsync(GetApiDate query)
        {
            var currentDate = SystemTime.Now;
            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedCurrentDate)
            {
                currentDate = systemSettings.FixedCurrentDate;
            }

            return currentDate;
        }
    }
}
