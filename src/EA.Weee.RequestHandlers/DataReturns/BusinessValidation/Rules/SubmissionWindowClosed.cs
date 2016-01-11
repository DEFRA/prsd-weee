namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.Rules
{
    using System;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess.DataAccess;
    using Domain.DataReturns;
    using Factories;
    using Prsd.Core;
    using ReturnVersionBuilder;

    public class SubmissionWindowClosed : ISubmissionWindowClosed
    {
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public SubmissionWindowClosed(IQuarterWindowFactory quarterWindowFactory, ISystemDataDataAccess systemDataDataAccess)
        {
            this.quarterWindowFactory = quarterWindowFactory;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<DataReturnVersionBuilderResult> Validate(Quarter quarter)
        {
            var result = new DataReturnVersionBuilderResult();

            var quarterWindow = await quarterWindowFactory.GetQuarterWindow(quarter);

            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedComplianceYearAndQuarter)
            {
                if (systemSettings.FixedComplianceYear != quarter.Year
                    || systemSettings.FixedQuarter != quarter.Q)
                {
                    var systemSetQuarter = new Quarter(systemSettings.FixedComplianceYear, systemSettings.FixedQuarter);

                    var systemSetQuarterWindow =
                        await quarterWindowFactory.GetQuarterWindow(systemSetQuarter);

                    return Validate(systemSetQuarterWindow.StartDate, quarterWindow, quarter);
                }
            }
            else
            {
                return Validate(SystemTime.Now, quarterWindow, quarter);
            }

            return result;
        }

        private DataReturnVersionBuilderResult Validate(DateTime currentTime, QuarterWindow quarterWindow, Quarter quarter)
        {
            var result = new DataReturnVersionBuilderResult();

            if (quarterWindow.IsBeforeWindow(currentTime))
            {
                var errorMessage = string.Format(
                    "The submission window for {0} {1} has not yet opened.The submission window will open on the {2}, resubmit your file on or after this date.",
                    quarter.Q, quarter.Year, quarterWindow.StartDate.ToString("dd MMM yyyy"));

                result.ErrorData.Add(new ErrorData(errorMessage, ErrorLevel.Error));
                return result;
            }

            if (quarterWindow.IsAfterWindow(currentTime))
            {
                var errorMessage = string.Format(
                    "The window for resubmitting data returns for the {0} compliance period has closed. Contact your relevant agency.",
                    quarter.Year);

                result.ErrorData.Add(new ErrorData(errorMessage, ErrorLevel.Error));
                return result;
            }

            return result;
        }
    }
}
