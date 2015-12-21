namespace EA.Weee.Domain.Tests.Unit.Producer
{
    using EA.Weee.Domain.Producer;
    using Xunit;

    public class ProducerContactTests
    {
        [Fact]
        public void ProducerContact_EqualsNullParameter_ReturnsFalse()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;

            Assert.NotEqual(producerContact, null);
        }

        [Fact]
        public void ProducerContact_EqualsObjectParameter_ReturnsFalse()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;

            Assert.NotEqual(producerContact, new object());
        }

        [Fact]
        public void ProducerContact_EqualsSameInstance_ReturnsTrue()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;

            Assert.Equal(producerContact, producerContact);
        }

        [Fact]
        public void ProducerContact_EqualsProducerContactSameDetails_ReturnsTrue()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;
            var producerContact2 = ProducerContactBuilder.NewProducerContact;

            Assert.Equal(producerContact, producerContact2);
        }

        [Fact]
        public void ProducerContact_EqualsProducerContactDifferentTitle_ReturnsFalse()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;
            var producerContact2 = ProducerContactBuilder.WithTitle("Mrs");

            Assert.NotEqual(producerContact, producerContact2);
        }

        [Fact]
        public void ProducerContact_EqualsProducerContactDifferentForename_ReturnsFalse()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;
            var producerContact2 = ProducerContactBuilder.WithForename("Forename2");

            Assert.NotEqual(producerContact, producerContact2);
        }

        [Fact]
        public void ProducerContact_EqualsProducerContactDifferentSurname_ReturnsFalse()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;
            var producerContact2 = ProducerContactBuilder.WithSurname("Surname2");

            Assert.NotEqual(producerContact, producerContact2);
        }

        [Fact]
        public void ProducerContact_EqualsProducerContactDifferentTelephone_ReturnsFalse()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;
            var producerContact2 = ProducerContactBuilder.WithTelephone("Telephone2");

            Assert.NotEqual(producerContact, producerContact2);
        }

        [Fact]
        public void ProducerContact_EqualsProducerContactDifferentMobile_ReturnsFalse()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;
            var producerContact2 = ProducerContactBuilder.WithMobile("Mobile2");

            Assert.NotEqual(producerContact, producerContact2);
        }

        [Fact]
        public void ProducerContact_EqualsProducerContactDifferentFax_ReturnsFalse()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;
            var producerContact2 = ProducerContactBuilder.WithFax("Fax2");

            Assert.NotEqual(producerContact, producerContact2);
        }

        [Fact]
        public void ProducerContact_EqualsProducerContactDifferentEmail_ReturnsFalse()
        {
            var producerContact = ProducerContactBuilder.NewProducerContact;
            var producerContact2 = ProducerContactBuilder.WithEmail("Email2");

            Assert.NotEqual(producerContact, producerContact2);
        }

        [Fact]
        public void ProducerContact_EqualsProducerContactDifferentProducerAddress_ReturnsFalse()
        {
            var producerContact = ProducerContactBuilder.WithProducerAddress(new AlwaysUnequalProducerAddress());
            var producerContact2 = ProducerContactBuilder.WithProducerAddress(new AlwaysUnequalProducerAddress());

            Assert.NotEqual(producerContact, producerContact2);
        }

        private class AlwaysEqualProducerAddress : ProducerAddress
        {
            public override bool Equals(ProducerAddress other)
            {
                return true;
            }
        }

        private class AlwaysUnequalProducerAddress : ProducerAddress
        {
            public override bool Equals(ProducerAddress other)
            {
                return false;
            }
        }

        private class ProducerContactBuilder
        {
            private string title = "Mr";
            private string forename = "TestForename";
            private string surname = "TestSurname";
            private string telephone = "01483000000";
            private string mobile = "07896000000";
            private string fax = "07896999999";
            private string email = "test@mail.co.uk";
            private ProducerAddress address = new AlwaysEqualProducerAddress();

            private ProducerContactBuilder()
            {
            }

            private ProducerContact Build()
            {
                return new ProducerContact(title,
                                 forename,
                                 surname,
                                 telephone,
                                 mobile,
                                 fax,
                                 email,
                                 address);
            }

            public static ProducerContact NewProducerContact
            {
                get { return new ProducerContactBuilder().Build(); }
            }

            public static ProducerContact WithTitle(string title)
            {
                var builder = new ProducerContactBuilder();
                builder.title = title;

                return builder.Build();
            }

            public static ProducerContact WithForename(string forename)
            {
                var builder = new ProducerContactBuilder();
                builder.forename = forename;

                return builder.Build();
            }

            public static ProducerContact WithSurname(string surname)
            {
                var builder = new ProducerContactBuilder();
                builder.surname = surname;

                return builder.Build();
            }

            public static ProducerContact WithTelephone(string telephone)
            {
                var builder = new ProducerContactBuilder();
                builder.telephone = telephone;

                return builder.Build();
            }

            public static ProducerContact WithMobile(string mobile)
            {
                var builder = new ProducerContactBuilder();
                builder.mobile = mobile;

                return builder.Build();
            }

            public static ProducerContact WithFax(string fax)
            {
                var builder = new ProducerContactBuilder();
                builder.fax = fax;

                return builder.Build();
            }

            public static ProducerContact WithEmail(string email)
            {
                var builder = new ProducerContactBuilder();
                builder.email = email;

                return builder.Build();
            }

            public static ProducerContact WithProducerAddress(ProducerAddress address)
            {
                var builder = new ProducerContactBuilder();
                builder.address = address;

                return builder.Build();
            }
        }
    }
}
