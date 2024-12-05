﻿namespace EA.Weee.Web.ViewModels.Shared.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using System;

    public class AatfEditContactAddressViewModel
    {
        public Guid Id { get; set; }

        public AatfContactData ContactData { get; set; }

        public AatfData AatfData { get; set; }

        public Guid OrganisationId { get; set; }

        public AatfEditContactAddressViewModel()
        {
            this.ContactData = new AatfContactData();
        }
    }
}