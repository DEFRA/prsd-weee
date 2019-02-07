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
        public SubmittedReturnViewModel()
        {
        }

        public SubmittedReturnViewModel(Quarter quarter, QuarterWindow window, int year) : base(quarter, window, year)
        {
        }
    }
}