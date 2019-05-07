namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System.Collections.Generic;

    public class ReturnsViewModel
    {
        public string OrganisationName { get; set; }

        public IList<ReturnViewModel> Returns { get; set; }

        public ReturnsViewModel()
        {
            Returns = new List<ReturnViewModel>();
        }
    }
}