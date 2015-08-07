namespace EA.Weee.Web.Areas.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Core.PCS;

    public class ManageSchemesViewModel
    {
        public List<SchemeData> Schemes { get; set; }

        public Guid Selected { get; set; }
    }
}