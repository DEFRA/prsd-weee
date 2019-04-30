namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ManageAatfsViewModel
    {
        [Required(ErrorMessage = "You must select a Aatf to manage")]
        public Guid? Selected { get; set; }
        public List<AatfDataList> AatfDataList { get; set; }
    }
}