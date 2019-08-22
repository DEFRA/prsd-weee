namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Returns;
    using Infrastructure;
    using System;

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