namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using Core.AatfReturn;
    using Core.DataReturns;
    using Prsd.Core;

    public class SubmittedReturnViewModel : ReturnViewModelBase
    {
        //private readonly QuarterWindow quarterWindow;
        //private readonly Quarter quarter;
        //private readonly int year;

        public SubmittedReturnViewModel()
        {
        }

        public SubmittedReturnViewModel(Quarter quarter, QuarterWindow window, int year) : base(quarter, window, year)
        {
        }

        //public string Year => year.ToString();

        //public string Quarter => quarter.Q.ToString();

        //public string Period => $"{Quarter} {quarterWindow.StartDate.ToString("MMM", CultureInfo.CurrentCulture)} - {quarterWindow.EndDate.ToString("MMM", CultureInfo.CurrentCulture)} {year}";
    }
}