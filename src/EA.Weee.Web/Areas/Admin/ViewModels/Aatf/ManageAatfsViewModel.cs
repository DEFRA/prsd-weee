namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ManageAatfsViewModel : ManageFacilityModelBase
    {
        public List<AatfDataList> AatfDataList { get; set; }

        public FilteringViewModel Filter { get; set; }

        [Required(ErrorMessage = "You must select an AATF to manage")]
        public override Guid? Selected { get; set; }

        public bool CanAddAatf { get; set; }
    }
}