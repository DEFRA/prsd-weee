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
        public void Equals_NullParameter_ReturnsFalse()
        {
            AuthorisedRepresentative authorisedRepresentative = new AuthorisedRepresentative(
                "1",
                null);

            Assert.NotEqual(authorisedRepresentative, null);
        }

        [Fact]
        public void Equals_ObjectParameter_ReturnsFalse()
        {
            AuthorisedRepresentative authorisedRepresentative = new AuthorisedRepresentative(
                "1",
                null);

            Assert.NotEqual(authorisedRepresentative, new object());
        }

        [Fact]
        public void Equals_SameInstance_ReturnsTrue()
        {
            AuthorisedRepresentative authorisedRepresentative = new AuthorisedRepresentative(
                "1",
                null);

            Assert.Equal(authorisedRepresentative, authorisedRepresentative);
        }

        [Fact]
        public void Equals_AuthorisedRepresentativeWithSameDetails_ReturnsTrue()
        {
            ProducerContact contact = A.Dummy<ProducerContact>();
            A.CallTo(() => contact.IsOverseas).Returns(true);

            AuthorisedRepresentative authorisedRepresentative1 = new AuthorisedRepresentative(
                "1",
                contact);

            AuthorisedRepresentative authorisedRepresentative2 = new AuthorisedRepresentative(
                "1",
                contact);

            Assert.Equal(authorisedRepresentative1, authorisedRepresentative2);
        }

        [Fact]
        public void Equals_AuthorisedRepresentativeWithDifferentName_ReturnsFalse()
        {
            ProducerContact contact = A.Fake<ProducerContact>();
            A.CallTo(() => contact.IsOverseas).Returns(true);

            AuthorisedRepresentative authorisedRepresentative1 = new AuthorisedRepresentative(
                "1",
                contact);

            AuthorisedRepresentative authorisedRepresentative2 = new AuthorisedRepresentative(
                "2",
                contact);

            Assert.NotEqual(authorisedRepresentative1, authorisedRepresentative2);
        }

        [Fact]
        public void Equals_AuthorisedRepresentativeWithDifferentProducerContact_ReturnsFalse()
        {
            ProducerContact contact1 = A.Fake<ProducerContact>();
            A.CallTo(() => contact1.IsOverseas).Returns(true);
            A.CallTo(() => contact1.Equals(A<ProducerContact>._)).Returns(false);

            ProducerContact contact2 = A.Fake<ProducerContact>();
            A.CallTo(() => contact2.IsOverseas).Returns(true);
            A.CallTo(() => contact2.Equals(A<ProducerContact>._)).Returns(false);

            AuthorisedRepresentative authorisedRepresentative1 = new AuthorisedRepresentative(
                "1",
                contact1);

            AuthorisedRepresentative authorisedRepresentative2 = new AuthorisedRepresentative(
                "1",
                contact2);

            Assert.NotEqual(authorisedRepresentative1, authorisedRepresentative2);
        }

        [Fact]
        public void Equals_AuthorisedRepresentativeWithNullProducerContact_ReturnsFalse()
        {
            ProducerContact contact = A.Fake<ProducerContact>();
            A.CallTo(() => contact.IsOverseas).Returns(true);

            AuthorisedRepresentative authorisedRepresentative1 = new AuthorisedRepresentative(
                "1",
                contact);

            AuthorisedRepresentative authorisedRepresentative2 = new AuthorisedRepresentative(
                "2",
                null);

            Assert.NotEqual(authorisedRepresentative1, authorisedRepresentative2);
        }

        /// <summary>
        /// This test ensures that an authorised representative cannot be created with an
        /// overseas producer that is based in the UK.
        /// </summary>
        [Fact]
        public void AuthorisedRepresentative_WithOverseasProducerBasedInUK_ThrowsArgumentException()
        {
            // Arrange
            ProducerContact contact = A.Fake<ProducerContact>();
            A.CallTo(() => contact.IsOverseas).Returns(false);

            // Act
            Func<AuthorisedRepresentative> action = () => new AuthorisedRepresentative("Name", contact);

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        /// <summary>
        /// This test ensures that an authorised representative can be created with an
        /// overseas producer that is not based in the UK.
        /// </summary>
        [Fact]
        public void AuthorisedRepresentative_WithOverseasProducerNotBasedInUK_DoesntThrowException()
        {
            // Arrange
            ProducerContact contact = A.Fake<ProducerContact>();
            A.CallTo(() => contact.IsOverseas).Returns(true);

            // Act
            AuthorisedRepresentative result = new AuthorisedRepresentative("Name", contact);

            // Assert
            // No exception thrown.
        }

        /// <summary>
        /// This test ensures that an authorised representative can be created without an
        /// overseas producer.
        /// </summary>
        [Fact]
        public void AuthorisedRepresentative_WithNoOverseasProducer_DoesntThrowException()
        {
            // Arrange

            // Act
            AuthorisedRepresentative result = new AuthorisedRepresentative("Name", null);

            // Assert
            // No exception thrown.
        }
    }
}
