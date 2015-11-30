namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Scheme;

    public class ManageSchemesViewModel
    {
        public List<SchemeData> Schemes { get; set; }

        [Required(ErrorMessage = "You must select a scheme to manage")]
        public Guid? Selected { get; set; }
    }
}