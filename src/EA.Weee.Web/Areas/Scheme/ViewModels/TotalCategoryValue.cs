namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using Core.DataReturns;
    using Core.Shared;
    using System;

    public class TotalCategoryValue : CategoryValue
    {
        public string TotalReceived { get; set; }

        public string TotalReused { get; set; }

        public bool DisplaySummedCategory => (TotalReceived != null && Decimal.Parse(TotalReceived) != 0) || (TotalReused != null && Decimal.Parse(TotalReused) != 0);

        public TotalCategoryValue()
        {
        }
        public TotalCategoryValue(WeeeCategory category) : base(category)
        {
        }
    }
}