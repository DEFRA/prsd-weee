namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;

    public class NonObligatedValuesCopyPasteViewModel
    {
        public Guid OrganisationId { get; set; }
        
        public Guid ReturnId { get; set; }
        
        public string[] PastedValues { get; set; }

        public bool Dcf { get; set; }

        public string TypeHeading { get; internal set; }
    }
}