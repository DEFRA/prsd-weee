namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using System;

    public class DeleteViewModel
    {
        public Guid Id { get; set; }

        public CanAatfBeDeletedFlags CanDeleteFlags { get; set; }

        public FacilityType FacilityType { get; set; }
    }
}