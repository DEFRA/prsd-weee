namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class ViewAATFContactDetailsListViewModel : RadioButtonStringCollectionViewModel
    {
        [Required(ErrorMessage = "Select an activity")]
        public override string SelectedValue { get; set; }

        public Guid OrganisationId { get; set; }

        public List<AatfData> AatfList { get; set; }

        public ViewAATFContactDetailsListViewModel(List<string> activites) : base(activites)
        {
        }
    }
}