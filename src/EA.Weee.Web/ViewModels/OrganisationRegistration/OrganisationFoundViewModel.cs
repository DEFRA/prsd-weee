namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using EA.Weee.Core.Organisations;
    using Shared;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public class OrganisationFoundViewModel
    {
        public string OrganisationName { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public Guid OrganisationId { get; set; }
    }
}