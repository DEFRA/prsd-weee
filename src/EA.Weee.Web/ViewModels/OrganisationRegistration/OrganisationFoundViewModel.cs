namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;

    [Serializable]
    public class OrganisationFoundViewModel
    {
        public string OrganisationName { get; set; }

        public string CompanyRegistrationNumber { get; set; }

        public Guid OrganisationId { get; set; }

        public bool NpwdMigrated { get; set; }

        public bool NpwdMigratedComplete { get; set; }
    }
}