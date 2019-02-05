namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;

    public class CheckYourReturnViewModel
    {
        public CheckYourReturnViewModel()
        {
        }

        public decimal? NonObliagtedTotal { get; set; }

        public decimal? NonObligatedDcfTotal { get; set; }

        public Guid ReturnId { get; set; }

        public CheckYourReturnViewModel(decimal? total, decimal? dcftotal)
        {
            NonObliagtedTotal = total;
            NonObligatedDcfTotal = dcftotal;
        }
    }
}