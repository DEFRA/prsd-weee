namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;

    public class ObligatedValuesCopyPasteViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public String AatfName { get; set; }

        public Guid ReturnId { get; set; }

        public Guid SchemeId { get; set; }

        public String SchemeName { get; set; }

        public String[] B2bPastedValues { get; set; }

        public String[] B2cPastedValues { get; set; }

        public ObligatedType Type { get; set; } 
    }
}