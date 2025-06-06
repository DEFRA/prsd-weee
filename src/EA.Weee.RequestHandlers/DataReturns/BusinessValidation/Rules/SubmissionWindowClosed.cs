﻿namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.Rules
{
    using Core.Shared;
    using DataAccess.DataAccess;
    using Domain.DataReturns;
    using Factories;
    using Prsd.Core;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SubmissionWindowClosed : ISubmissionWindowClosed
    {
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public SubmissionWindowClosed(IQuarterWindowFactory quarterWindowFactory, ISystemDataDataAccess systemDataDataAccess)
        {
            this.quarterWindowFactory = quarterWindowFactory;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<IEnumerable<ErrorData>> Validate(Quarter quarter)
        {
            var result = new List<ErrorData>();

            var currentDate = SystemTime.Now;
            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedCurrentDate)
            {
                currentDate = systemSettings.FixedCurrentDate;
            }

            var quarterWindow = await quarterWindowFactory.GetQuarterWindow(quarter);

            if (quarterWindow.IsBeforeWindow(currentDate))
            {
                var errorMessage = string.Format(
                    "The submission window for {0} {1} has not yet opened. The submission window will open on the {2}, resubmit your return XML file on or after this date.",
                    quarter.Q, quarter.Year, quarterWindow.StartDate.ToString("dd MMM yyyy"));

                result.Add(new ErrorData(errorMessage, ErrorLevel.Error));
                return result;
            }

            if (quarterWindow.IsAfterWindow(currentDate))
            {
                var errorMessage = string.Format(
                    "The window for resubmitting data returns for the {0} compliance period has closed.",
                    quarter.Year);

                result.Add(new ErrorData(errorMessage, ErrorLevel.Error));
                return result;
            }

            return result;
        }
    }
}
