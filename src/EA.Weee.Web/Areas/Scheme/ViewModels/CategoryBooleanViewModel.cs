namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Shared;

    public class CategoryBooleanViewModel : CategoryValue
    {
        public bool Selected { get; set; }

        public CategoryBooleanViewModel()
            : base()
        {
            Selected = false;
        }

        public CategoryBooleanViewModel(WeeeCategory category)
            : base(category)
        {
            Selected = false;
        }
    }
}
