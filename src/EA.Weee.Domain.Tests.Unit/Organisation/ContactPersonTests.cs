namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using System;
    using Xunit;
    using Organisation = Domain.Organisation;

    public class ContactPersonTests
    {
        [Fact]
        public void AddContact_OrganisationAlreadyHasContact_Throws()
        {
            var organisation = GetTestOrganisation();
            var contact = GetTestContact();
            organisation.AddMainContactPerson(contact);
            Assert.Throws<InvalidOperationException>(() => organisation.AddMainContactPerson(contact));
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
            var organisation = new Organisation("Test", null, OrganisationType.RegisteredCompany, OrganisationStatus.Incomplete);
            return organisation;
        }
    }
}
