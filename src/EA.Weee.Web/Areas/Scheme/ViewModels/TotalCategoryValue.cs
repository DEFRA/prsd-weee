namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using Core.DataReturns;
    using Core.Shared;

    public class TotalCategoryValue : CategoryValue
    {
        public string TotalReceived { get; set; }

        public string TotalReused { get; set; }

        public TotalCategoryValue()
        {
        }
        public TotalCategoryValue(WeeeCategory category) : base(category)
        {
        }
    }
}