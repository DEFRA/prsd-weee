namespace EA.Weee.Web.Areas.Admin.ViewModels.Ae
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;

    public class ManageAesViewModel : ManageFacilityModelBase
    {
        public List<AatfDataList> AeDataList { get; set; }

        [Required(ErrorMessage = "You must select an AE to manage")]
        public override Guid? Selected { get; set; }
    }
}