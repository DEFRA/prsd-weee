namespace EA.Weee.Web.ViewModels.Shared
{
    using System.Collections.Generic;
    using System.Linq;

    public class YesNoChoiceViewModel : RadioButtonStringCollectionViewModel
    {
        public YesNoChoiceViewModel()
            : base(new List<string> { "Yes", "No" })
        {
        }
        public YesNoChoiceViewModel(IEnumerable<string> stringsToUse)
        {
            this.PossibleValues = stringsToUse.ToList();
        }
    }
}