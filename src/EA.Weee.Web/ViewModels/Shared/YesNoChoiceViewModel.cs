namespace EA.Weee.Web.ViewModels.Shared
{
    using System.Collections.Generic;

    public class YesNoChoiceViewModel
    {
        public RadioButtonStringCollectionViewModel Choices { get; set; }

        public YesNoChoiceViewModel()
        {
            List<string> choices = new List<string> {"Yes", "No"};
            this.Choices = new RadioButtonStringCollectionViewModel(choices);
        }
    }
}