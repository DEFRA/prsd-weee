namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using FluentValidation.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Validation;

    [Validator(typeof(SelectReportOptionsViewModelValidator))]
    [Serializable]
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

        public DateTime QuarterWindowStartDate { get; set; }

        public DateTime QuarterWindowEndDate { get; set; }

        public string Quarter { get; set; }

        public string Period => $"{Quarter} {QuarterWindowStartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindowEndDate.ToString("MMM", CultureInfo.CurrentCulture)}";

        public string Year { get; set; }
    }
}