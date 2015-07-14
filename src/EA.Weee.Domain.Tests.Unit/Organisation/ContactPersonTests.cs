namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using System;
    using Domain.Organisation;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class ContactPersonTests
    {
        [Fact]
        public void AddContact_OrganisationAlreadyHasContact_UpdateContactDetails()
        {
            //Changed as per update contact person details
            var organisation = GetTestOrganisation();
            var contact = GetTestContact();
            organisation.AddOrUpdateMainContactPerson(contact);

            Assert.Same(contact, organisation.Contact);
            
            Assert.Equal(organisation.Contact.FirstName, contact.FirstName);
            Assert.Equal(organisation.Contact.LastName, contact.LastName);
            Assert.Equal(organisation.Contact.Position, contact.Position);
        }

        [Fact]
        public void AddContact_OrganisationHasExistingContact_JustCopiesParameterObject()
        {
            var organisation = GetTestOrganisation();

            var initialContact = GetTestContact();
            organisation.AddOrUpdateMainContactPerson(initialContact);

            var updatedContact = new Contact("different firstname", "different lastname", "different position");
            organisation.AddOrUpdateMainContactPerson(updatedContact);

            Assert.Same(initialContact, organisation.Contact);
            Assert.NotSame(updatedContact, organisation.Contact);

            Assert.Equal(organisation.Contact.FirstName, updatedContact.FirstName);
            Assert.Equal(organisation.Contact.LastName, updatedContact.LastName);
            Assert.Equal(organisation.Contact.Position, updatedContact.Position);
        }

        [Fact]
        public void SetFirstNameforContact_ExceedsMaxCharacters_ThrowException()
        {
            const string longfirstname = "abcdefghijklmnopqrstuvwxyzabcdefghijo";
            Action createContact = () => new Contact(longfirstname, "lastName", "Position");
            Assert.Throws<InvalidOperationException>(createContact);
        }

        [Fact]
        public void SetFirstNameforContact_ToNull_ThrowException()
        {
            Action createContact = () => new Contact(null, "lastName", "Position");
            Assert.Throws<ArgumentNullException>(createContact);
        }

        [Fact]
        public void SetLastNameforContact_ExceedsMaxCharacters_ThrowException()
        {
            const string longlastname = "abcdefghijklmnopqrstuvwxyzabcdefghijo";
            Action createContact = () => new Contact("firstname", longlastname, "Position");
            Assert.Throws<InvalidOperationException>(createContact);
        }

        [Fact]
        public void SetLastNameforContact_ToNull_ThrowException()
        {
            Action createContact = () => new Contact("firstname", null, "Position");
            Assert.Throws<ArgumentNullException>(createContact);
        }

        [Fact]
        public void SetPositionforContact_ExceedsMaxCharacters_ThrowException()
        {
            var contact = GetTestContact();
            const string longposition = "abcdefghijklmnopqrstuvwxyzabcdefghijo";
            Action createContact = () => new Contact("firstname", "lastname", longposition);
            Assert.Throws<InvalidOperationException>(createContact);
        }

        [Fact]
        public void SetPositionforContact_ToNull_ThrowException()
        {
            Action createContact = () => new Contact("Firstname", "lastName", null);
            Assert.Throws<ArgumentNullException>(createContact);
        }

        private static Contact GetTestContact()
        {
            return new Contact("firstName", "LastName", "Position");
        }

        private static Organisation GetTestOrganisation()
        {
            var organisation = Organisation.CreateRegisteredCompany("Test", "AB123456");
            return organisation;
        }
    }
}
