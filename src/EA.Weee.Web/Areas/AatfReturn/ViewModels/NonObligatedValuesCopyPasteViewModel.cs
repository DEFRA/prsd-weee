namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;

    public class NonObligatedValuesCopyPasteViewModel
    {
        public Guid OrganisationId { get; set; }
        
        public Guid ReturnId { get; set; }
        
        public String[] PastedValues { get; set; }

        public bool Dcf { get; set; }
    }
}