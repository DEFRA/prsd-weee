namespace EA.Weee.Web.ViewModels.Organisation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Requests.Organisations;

    public class OrganisationContactPersonViewModel
    {
        public Guid OrganisationId { get; set; }
        
        public ContactData MainContactPerson { get; set; }
    }
}