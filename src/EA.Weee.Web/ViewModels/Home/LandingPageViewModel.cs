namespace EA.Weee.Web.ViewModels.Home
{
    using EA.Weee.Web.ViewModels.Shared;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class LandingPageViewModel
    {
        //[DisplayName("Do you already have a personal user account?")] --TODO: Update content to allow legend
        public RadioButtonStringCollectionViewModel Choices { get; set; }

        public LandingPageViewModel()
        {
            List<string> choices = new List<string> {"Yes", "No"};
            this.Choices = new RadioButtonStringCollectionViewModel(choices);
        }
    }
}