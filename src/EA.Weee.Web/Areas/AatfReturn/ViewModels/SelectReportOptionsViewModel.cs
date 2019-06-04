namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using FluentValidation.Attributes;
    using Validation;

    [Validator(typeof(SelectReportOptionsViewModelValidator))]
    public class SelectReportOptionsViewModel : SelectReportOptionsModelBase
    {
        public SelectReportOptionsViewModel()
        {
        }

        public SelectReportOptionsViewModel(
            Guid organisationId,
            Guid returnId,
            List<ReportOnQuestion> reportOnQuestions,
            ReturnData returnData,
            int year)
        {
            OrganisationId = organisationId;
            ReturnId = returnId;
            ReportOnQuestions = reportOnQuestions;
            ReturnData = returnData;
            Year = year.ToString();
        }

        public ReturnData ReturnData { get; set; }

        public IList<string> DcfPossibleValues => new List<string> { "Yes", "No" };

        public DateTime QuarterWindowStartDate { get; set; }

        public DateTime QuarterWindowEndDate { get; set; }

        public string Quarter { get; set; }

        public string Period => $"{Quarter} {QuarterWindowStartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindowEndDate.ToString("MMM", CultureInfo.CurrentCulture)}";

        public string Year { get; set; }
    }
}