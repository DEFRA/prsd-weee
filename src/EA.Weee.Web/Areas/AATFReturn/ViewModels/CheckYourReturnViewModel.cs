namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;

    public class CheckYourReturnViewModel
    {
        public int NonObliagtedTotal;

        public int NonObligatedDcfTotal;

        public CheckYourReturnViewModel(int total, int dcftotal)
        {
            NonObliagtedTotal = total;
            NonObligatedDcfTotal = dcftotal;
        }
    }
}