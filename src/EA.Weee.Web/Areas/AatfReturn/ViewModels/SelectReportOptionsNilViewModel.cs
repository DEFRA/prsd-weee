namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Globalization;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Returns;

    public class SelectReportOptionsNilViewModel : ReturnViewModelBase
    {
        public SelectReportOptionsNilViewModel()
        {
        }

        public SelectReportOptionsNilViewModel(ReturnData returnData) : base(returnData)
        {
        }

        public Guid OrganisationId { get; set; }

        public override string Period => $"{Quarter} {QuarterWindow.WindowOpenDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindow.QuarterEnd.ToString("MMM", CultureInfo.CurrentCulture)}";
    }
}