namespace EA.Weee.Web.ViewModels.Shared
{
    using System.Collections.Generic;

    public class YesNoChoiceViewModel : RadioButtonStringCollectionViewModel
    {
        public YesNoChoiceViewModel()
            : base(new List<string> { "Yes", "No" })
        {
        }
    }
}