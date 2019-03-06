namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System.Globalization;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Prsd.Core;

    public abstract class ReturnViewModelBase : IReturnViewModel
    {
        protected readonly QuarterWindow QuarterWindow;
        private readonly Quarter quarter;
        private readonly int year;

        public virtual string Year => year.ToString();

        public virtual string Quarter => quarter.Q.ToString();

        public virtual string Period => $"{Quarter} {QuarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {QuarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)} {Year}";

        protected ReturnViewModelBase(Quarter quarter, QuarterWindow window, int year)
        {
            Guard.ArgumentNotNull(() => quarter, quarter);
            Guard.ArgumentNotNull(() => window, window);

            this.QuarterWindow = window;
            this.quarter = quarter;
            this.year = year;
        }

        protected ReturnViewModelBase()
        {
        }
    }
}