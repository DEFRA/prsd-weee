namespace EA.Weee.Domain.Tests.Unit.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Producer;
    using FakeItEasy;
    using Xunit;

    public class AuthorisedRepresentativeTests
    {
        [Fact]
        public void AuthorisedRepresentative_EqualsNullParameter_ReturnsFalse()
        {
            var authorisedRepresentative = AuthorisedRepresentativeBuilder.NewAuthorisedRepresentative;

            Assert.NotEqual(authorisedRepresentative, null);
        }

        [Fact]
        public void AuthorisedRepresentative_EqualsObjectParameter_ReturnsFalse()
        {
            var authorisedRepresentative = AuthorisedRepresentativeBuilder.NewAuthorisedRepresentative;

            Assert.NotEqual(authorisedRepresentative, new object());
        }

        [Fact]
        public void AuthorisedRepresentative_EqualsSameInstance_ReturnsTrue()
        {
            var authorisedRepresentative = AuthorisedRepresentativeBuilder.NewAuthorisedRepresentative;

            Assert.Equal(authorisedRepresentative, authorisedRepresentative);
        }

        [Fact]
        public void AuthorisedRepresentative_EqualsAuthorisedRepresentativeSameDetails_ReturnsTrue()
        {
            var authorisedRepresentative = AuthorisedRepresentativeBuilder.NewAuthorisedRepresentative;
            var authorisedRepresentative2 = AuthorisedRepresentativeBuilder.NewAuthorisedRepresentative;

            Assert.Equal(authorisedRepresentative, authorisedRepresentative2);
        }

        [Fact]
        public void AuthorisedRepresentative_EqualsAuthorisedRepresentativeDifferentName_ReturnsFalse()
        {
            var authorisedRepresentative = AuthorisedRepresentativeBuilder.NewAuthorisedRepresentative;
            var authorisedRepresentative2 = AuthorisedRepresentativeBuilder.WithName("name test");

            Assert.NotEqual(authorisedRepresentative, authorisedRepresentative2);
        }

        [Fact]
        public void AuthorisedRepresentative_EqualsAuthorisedRepresentativeDifferentProducerContact_ReturnsFalse()
        {
            var authorisedRepresentative = AuthorisedRepresentativeBuilder.WithProducerContact(new AlwaysUnequalProducerContact());
            var authorisedRepresentative2 = AuthorisedRepresentativeBuilder.WithProducerContact(new AlwaysUnequalProducerContact());

            Assert.NotEqual(authorisedRepresentative, authorisedRepresentative2);
        }

        [Fact]
        public void AuthorisedRepresentative_EqualsAuthorisedRepresentativeNullProducerContact_ReturnsFalse()
        {
            var authorisedRepresentative = AuthorisedRepresentativeBuilder.NewAuthorisedRepresentative;
            var authorisedRepresentative2 = AuthorisedRepresentativeBuilder.WithProducerContact(null);

            Assert.NotEqual(authorisedRepresentative, authorisedRepresentative2);
        }

        private class AlwaysEqualProducerContact : ProducerContact
        {
            public override bool Equals(ProducerContact other)
            {
                return true;
            }
        }

        private class AlwaysUnequalProducerContact : ProducerContact
        {
            public override bool Equals(ProducerContact other)
            {
                return false;
            }
        }

        private class AuthorisedRepresentativeBuilder
        {
            private string name = "name";
            private ProducerContact contact = new AlwaysEqualProducerContact();

            private AuthorisedRepresentativeBuilder()
            {
            }

            private AuthorisedRepresentative Build()
            {
                AuthorisedRepresentative authorisedRepresentative;

                if (contact != null)
                {
                    authorisedRepresentative = new AuthorisedRepresentative(name, contact);
                }
                else
                {
                    authorisedRepresentative = new AuthorisedRepresentative(name);
                }

                return authorisedRepresentative;
            }

            public static AuthorisedRepresentative NewAuthorisedRepresentative
            {
                get { return new AuthorisedRepresentativeBuilder().Build(); }
            }

            public static AuthorisedRepresentative WithName(string name)
            {
                var builder = new AuthorisedRepresentativeBuilder();
                builder.name = name;

                return builder.Build();
            }

            public static AuthorisedRepresentative WithProducerContact(ProducerContact contact)
            {
                var builder = new AuthorisedRepresentativeBuilder();
                builder.contact = contact;

                return builder.Build();
            }
        }
    }
}
