namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ManageAatfsViewModel : ManageFacilityModelBase
    {
        public List<AatfDataList> AatfDataList { get; set; }

        [Required(ErrorMessage = "You must select an Aatf to manage")]
        public override Guid? Selected { get; set; }
    }
}