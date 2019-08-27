﻿namespace EA.Weee.RequestHandlers.Test
{
    using Core.SystemData;
    using DataAccess.DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Test;
    using System.Threading.Tasks;

    public class GetPcsSubmissionWindowSettingsHandler : IRequestHandler<GetPcsSubmissionWindowSettings, PcsSubmissionWindowSettings>
    {
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetPcsSubmissionWindowSettingsHandler(ISystemDataDataAccess systemDataDataAccess)
        {
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<PcsSubmissionWindowSettings> HandleAsync(GetPcsSubmissionWindowSettings message)
        {
            var systemData = await systemDataDataAccess.Get();

            return new PcsSubmissionWindowSettings
            {
                CurrentDate = systemData.FixedCurrentDate,
                FixCurrentDate = systemData.UseFixedCurrentDate
            };
        }
    }
}
