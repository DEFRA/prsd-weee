namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System;

    public class OrganisationViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        private string postcode;
        public string Postcode
        {
            get { return this.postcode; }
            set { this.postcode = value.ToUpperInvariant(); }
        }

        public string TownOrCity { get; set; }
    }
}