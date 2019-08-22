﻿namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details.Base
{
    using Core.DataStandards;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public abstract class OrganisationViewModel
    {
        public Guid? OrganisationId { get; set; }

        public Guid? ContactId { get; set; }

        public Guid? AddressId { get; set; }

        [DisplayName("Business trading name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public virtual string BusinessTradingName { get; set; }
    }
}