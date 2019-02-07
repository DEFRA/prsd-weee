namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using Core.AatfReturn;
    using Core.DataReturns;
    using Prsd.Core;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    public abstract class ReturnViewModelBase : IReturnViewModel
    {
        private readonly QuarterWindow quarterWindow;
        private readonly Quarter quarter;
        private readonly int year;

        public string Year => year.ToString();

        public string Quarter => quarter.Q.ToString();

        public string Period => $"{Quarter} {quarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {quarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)} {Year}";

        protected ReturnViewModelBase(Quarter quarter, QuarterWindow window, int year)
        {
            Guard.ArgumentNotNull(() => quarter, quarter);
            Guard.ArgumentNotNull(() => window, window);

            this.quarterWindow = window;
            this.quarter = quarter;
            this.year = year;
        }

        protected ReturnViewModelBase()
        {
        }
    }
}