namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.XmlBusinessRules
{
    using System;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess.DataAccess;
    using Domain.DataReturns;
    using Factories;
    using Prsd.Core;
    using ReturnVersionBuilder;
    using Shared;
    using Xml.DataReturns;

    public class SubmissionWindowClosed : ISubmissionWindowClosed
    {
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public SubmissionWindowClosed(IQuarterWindowFactory quarterWindowFactory, ISystemDataDataAccess systemDataDataAccess)
        {
            this.quarterWindowFactory = quarterWindowFactory;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<DataReturnVersionBuilderResult> Validate(SchemeReturn xmlSchemeReturn)
        {
            var result = new DataReturnVersionBuilderResult();

            var quarter = new Quarter(int.Parse(xmlSchemeReturn.ComplianceYear),
                xmlSchemeReturn.ReturnPeriod.ToDomainQuarterType());
            var quarterWindow = await quarterWindowFactory.GetQuarterWindow(quarter);

            var systemSettings = await systemDataDataAccess.Get();

            if (systemSettings.UseFixedComplianceYearAndQuarter)
            {
                if (systemSettings.FixedComplianceYear != quarter.Year
                    || systemSettings.FixedQuarter != quarter.Q)
                {
                    var message =
                    string.Format(
                        "Only data returns for {0} {1} are valid because this has been fixed for testing. If you wish to allow data returns for other quarters, either change the test settings or upload a return for {0} {1}",
                        systemSettings.FixedQuarter, systemSettings.FixedComplianceYear);
                    result.ErrorData.Add(new ErrorData(message, ErrorLevel.Error));
                    return result;
                }
            }
            else
            {
                if (quarterWindow.IsBeforeWindow(SystemTime.Now))
                {
                    var errorMessage = string.Format(
                        "The submission window for {0} {1} has not yet opened.The submission window will open on the {2}, resubmit your file on or after this date.",
                        quarter.Q, quarter.Year, quarterWindow.StartDate.ToString("dd MMM yyyy"));

                    result.ErrorData.Add(new ErrorData(errorMessage, ErrorLevel.Error));
                    return result;
                }

                if (quarterWindow.IsAfterWindow(SystemTime.Now))
                {
                    var errorMessage = string.Format(
                        "The window for resubmitting data returns for the {0} compliance period has closed. Contact your relevant agency.",
                        quarter.Year);

                    result.ErrorData.Add(new ErrorData(errorMessage, ErrorLevel.Error));
                    return result;
                }
            }

            return result;
        }
    }
}
