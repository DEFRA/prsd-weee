﻿namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin;
    using System;

    public class DeleteViewModel
    {
        public Guid AatfId { get; set; }

        public Guid OrganisationId { get; set; }

        public AatfDeletionData DeletionData { get; set; }

        public FacilityType FacilityType { get; set; }

        public string AatfName { get; set; }

        public string OrganisationName { get; set; }
    }
}