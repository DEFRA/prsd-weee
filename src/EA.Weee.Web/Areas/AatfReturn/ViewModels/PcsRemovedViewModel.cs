namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.ViewModels.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PcsRemovedViewModel : RadioButtonStringCollectionViewModel
    {
        public List<SchemeData> RemovedSchemeList { get; set; }
        public List<Guid> RemovedSchemes { get; set; }
        public List<Guid> SelectedSchemes { get; set; }
        public Guid ReturnId { get; set; }

        public Guid OrganisationId { get; set; }

        [Required(ErrorMessage = "You must select an option")]
        public override string SelectedValue { get; set; }

        public PcsRemovedViewModel() : base(new List<string> { "Yes", "No" })
        {
        }
    }
}