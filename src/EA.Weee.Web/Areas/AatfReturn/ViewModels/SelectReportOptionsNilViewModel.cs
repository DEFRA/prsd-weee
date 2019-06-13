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

        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        public override string Period => $"{Quarter} {QuarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)}";
    }
}