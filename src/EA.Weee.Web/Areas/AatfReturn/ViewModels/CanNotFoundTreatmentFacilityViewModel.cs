namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;

    public class CanNotFoundTreatmentFacilityViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public string AatfName { get; set; }

        public bool IsCanNotFindLinkClick { get; set; }
    }
}