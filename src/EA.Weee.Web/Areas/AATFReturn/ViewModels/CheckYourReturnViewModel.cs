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
        /*
        public string Year => year.ToString();

        public string Quarter => quarter.Q.ToString();

        public string Period => $"{Quarter} {quarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {quarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)}";
        */
        public string NonObliagtedTotal { get; set; }

        public string NonObligatedDcfTotal { get; set; }

        public string OperatorName { get; set; }

        public Guid ReturnId { get; set; }

        public CheckYourReturnViewModel(decimal? total, decimal? dcftotal, Quarter quarter, QuarterWindow window, int year) : base(quarter, window, year)
        {
            /*
            Guard.ArgumentNotNull(() => quarter, quarter);
            Guard.ArgumentNotNull(() => window, window);
            this.quarter = quarter;
            this.quarterWindow = window;
            this.year = year;
            */
            this.NonObliagtedTotal = total.ToString();
            this.NonObligatedDcfTotal = dcftotal.ToString();
        }
    }
}