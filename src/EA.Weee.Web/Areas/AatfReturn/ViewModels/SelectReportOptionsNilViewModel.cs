namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Globalization;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Returns;
    using Infrastructure;

    public class SelectReportOptionsNilViewModel : ReturnViewModelBase
    {
        public SelectReportOptionsNilViewModel()
        {
        }

        public SelectReportOptionsNilViewModel(ReturnData returnData) : base(returnData)
        {
        }

        public Guid OrganisationId { get; set; }

        public override string Period => DisplayHelper.QuarterPeriodFormat(this.Quarter, this.QuarterWindow);
    }
}