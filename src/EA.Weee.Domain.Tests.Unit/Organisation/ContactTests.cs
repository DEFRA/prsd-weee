namespace EA.Weee.Domain.Tests.Unit.Organisation
{
    using Domain.Organisation;
    using Xunit;

    public class ContactTests
    {
        [Fact]
        public void Contact_EqualsNullParameter_ReturnsFalse()
        {
            var contact = ContactBuilder.NewContact;

            Assert.NotEqual(contact, null);
        }

        [Fact]
        public void Contact_EqualsObjectParameter_ReturnsFalse()
        {
            var contact = ContactBuilder.NewContact;

            Assert.NotEqual(contact, new object());
        }

        [Fact]
        public void Contact_EqualsSameInstance_ReturnsTrue()
        {
            var contact = ContactBuilder.NewContact;

            Assert.Equal(contact, contact);
        }

        [Fact]
        public void Contact_EqualsContactSameDetails_ReturnsTrue()
        {
            var contact = ContactBuilder.NewContact;
            var contact2 = ContactBuilder.NewContact;

            Assert.Equal(contact, contact2);
        }

        [Fact]
        public void Contact_EqualsContactDifferentFirstName_ReturnsFalse()
        {
            var contact = ContactBuilder.NewContact;
            var contact2 = ContactBuilder.WithFirstName("Test first name");

            Assert.NotEqual(contact, contact2);
        }

        [Fact]
        public void Contact_EqualsContactDifferentLastName_ReturnsFalse()
        {
            var contact = ContactBuilder.NewContact;
            var contact2 = ContactBuilder.WithLastName("Test last name");

            Assert.NotEqual(contact, contact2);
        }

        [Fact]
        public void Contact_EqualsContactDifferentPosition_ReturnsFalse()
        {
            var contact = ContactBuilder.NewContact;
            var contact2 = ContactBuilder.WithPosition("Test position");

            Assert.NotEqual(contact, contact2);
        }

        private class ContactBuilder
        {
            private string FirstName { get; set; }

            private string LastName { get; set; }

            private string Position { get; set; }

            public ContactBuilder()
            {
                FirstName = "FirstName";
                LastName = "LastName";
                Position = "Position";
            }

            public Contact Build()
            {
                return new Contact(FirstName, LastName, Position);
            }

            public static Contact NewContact
            {
                get { return new ContactBuilder().Build(); }
            }

            public static Contact WithFirstName(string firstName)
            {
                var builder = new ContactBuilder();
                builder.FirstName = firstName;

                return builder.Build();
            }

            public static Contact WithLastName(string lastName)
            {
                var builder = new ContactBuilder();
                builder.LastName = lastName;

                return builder.Build();
            }

            public static Contact WithPosition(string position)
            {
                var builder = new ContactBuilder();
                builder.Position = position;

                return builder.Build();
            }
        }
    }
}
