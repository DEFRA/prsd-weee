namespace EA.Weee.Domain.Tests.Unit.Scheme
{
    using System;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using Xunit;

    public class SchemeContactPersonTests
    {
        [Fact]
        public void AddContact_OrganisationAlreadyHasContact_UpdateContactDetails()
        {
            //Changed as per update contact person details
            var scheme = GetTestScheme();
            var contact = GetTestContact();
            scheme.AddOrUpdateMainContactPerson(contact);

            Assert.Same(contact, scheme.Contact);
            
            Assert.Equal(scheme.Contact.FirstName, contact.FirstName);
            Assert.Equal(scheme.Contact.LastName, contact.LastName);
            Assert.Equal(scheme.Contact.Position, contact.Position);
        }

        [Fact]
        public void AddContact_OrganisationHasExistingContact_JustCopiesParameterObject()
        {
            var scheme = GetTestScheme();

            var initialContact = GetTestContact();
            scheme.AddOrUpdateMainContactPerson(initialContact);

            var updatedContact = new Contact("different firstname", "different lastname", "different position");
            scheme.AddOrUpdateMainContactPerson(updatedContact);

            Assert.Same(initialContact, scheme.Contact);
            Assert.NotSame(updatedContact, scheme.Contact);

            Assert.Equal(scheme.Contact.FirstName, updatedContact.FirstName);
            Assert.Equal(scheme.Contact.LastName, updatedContact.LastName);
            Assert.Equal(scheme.Contact.Position, updatedContact.Position);
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

        private static Scheme GetTestScheme()
        {
            var scheme = new Scheme(A.Dummy<Guid>());
            return scheme;
        }
    }
}
