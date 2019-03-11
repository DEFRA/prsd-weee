namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;

    public class ObligatedValuesCopyPasteViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid SchemeId { get; set; }

        public string PastedValues { get; set; }
    }
}