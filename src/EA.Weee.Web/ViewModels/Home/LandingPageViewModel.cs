namespace EA.Weee.Web.ViewModels.Home
{
    using Shared;
    using System.Collections.Generic;

    public class LandingPageViewModel : RadioButtonStringCollectionViewModel
    {
        public LandingPageViewModel()
            : base(new List<string> { "Yes", "No" })
        {
        }
    }
}