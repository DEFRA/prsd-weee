namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using Core.AatfReturn;
    using EA.Weee.Core.DataReturns;

    public class CheckYourReturnViewModel : ReturnViewModelBase
    {
        public CheckYourReturnViewModel()
        {
        }

        public string NonObliagtedTotal { get; set; }

        public string NonObligatedDcfTotal { get; set; }

        public OperatorData ReturnOperator { get; set; }

        public Guid ReturnId { get; set; }

        public CheckYourReturnViewModel(OperatorData returnOperator, decimal? total, decimal? dcftotal, Quarter quarter, QuarterWindow window, int year) : base(quarter, window, year)
        {
            this.NonObliagtedTotal = total.ToString();
            this.NonObligatedDcfTotal = dcftotal.ToString();
            this.ReturnOperator = returnOperator;
        }
    }
}