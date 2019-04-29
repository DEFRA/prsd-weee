namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ManageAatfsViewModel
    {
        public List<AatfData> AatfListData { get; set; }

        [Required(ErrorMessage = "You must select a Aatf to manage")]
        public Guid? Selected { get; set; }
    }
}