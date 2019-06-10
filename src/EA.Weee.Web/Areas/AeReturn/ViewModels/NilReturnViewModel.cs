namespace EA.Weee.Web.Areas.AeReturn.ViewModels
{
    using EA.Weee.Web.ViewModels.Returns;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class NilReturnViewModel : ReturnViewModel
    {
        public Guid OrganisationId { get; set; }
        public Guid ReturnId { get; set; }
    }
}