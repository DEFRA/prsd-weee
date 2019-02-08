namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Core.AatfReturn;
    using EA.Prsd.Core;
    using EA.Weee.Core.DataReturns;

    public class CheckYourReturnViewModel : ReturnViewModelBase
    {
        /*
        private readonly QuarterWindow quarterWindow;
        private readonly Quarter quarter;
        private readonly int year;
        */
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