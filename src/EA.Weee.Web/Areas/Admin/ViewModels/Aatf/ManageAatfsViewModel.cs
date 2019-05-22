﻿namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ManageAatfsViewModel
    {
        public List<AatfDataList> AatfDataList { get; set; }

        [Required(ErrorMessage = "You must select an Aatf to manage")]
        public Guid? Selected { get; set; }

        public bool CanAddAatf { get; set; }
    }
}