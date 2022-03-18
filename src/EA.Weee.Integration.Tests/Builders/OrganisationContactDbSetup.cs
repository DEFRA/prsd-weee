namespace EA.Weee.Integration.Tests.Builders
{
    using System.Linq;
    using Base;
    using Domain.Organisation;

    public class OrganisationContactDbSetup : DbTestDataBuilder<Contact, OrganisationContactDbSetup>
    {
        protected override Contact Instantiate()
        {
            instance = new Contact("Contact first name",
                "Contact surname",
                "Contact position");

            return instance;
        }
    }
}
