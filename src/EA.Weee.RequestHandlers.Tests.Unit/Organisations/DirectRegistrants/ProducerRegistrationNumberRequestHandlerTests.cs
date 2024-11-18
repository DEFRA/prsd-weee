namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ProducerRegistrationNumberRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly IGenerateFromXmlDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;
        private readonly IRequestHandler<ProducerRegistrationNumberRequest, bool> handler;

        public ProducerRegistrationNumberRequestHandlerTests()
        {
            dataAccess = A.Fake<IGenerateFromXmlDataAccess>();
            authorization = A.Fake<IWeeeAuthorization>();

            handler = new ProducerRegistrationNumberRequestHandler(dataAccess, authorization);
        }

        [Fact]
        public async Task WhenProducerRegistrationNumberExists_ReturnsTrue()
        {
            // Arrange
            var producerRegistrationNumber = "12345";
            var request = new ProducerRegistrationNumberRequest(producerRegistrationNumber);

            A.CallTo(() => authorization.EnsureCanAccessExternalArea());

            A.CallTo(() => dataAccess.SchemeProducerRegistrationExists(producerRegistrationNumber))
                .Returns(Task.FromResult(true));

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task WhenProducerRegistrationNumberDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var producerRegistrationNumber = "NONEXISTENTPRN";
            var request = new ProducerRegistrationNumberRequest(producerRegistrationNumber);

            A.CallTo(() => authorization.EnsureCanAccessExternalArea());

            A.CallTo(() => dataAccess.SchemeProducerRegistrationExists(producerRegistrationNumber))
                .Returns(Task.FromResult(false));

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task WhenUserIsNotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var producerRegistrationNumber = "12345";
            var request = new ProducerRegistrationNumberRequest(producerRegistrationNumber);

            A.CallTo(() => authorization.EnsureCanAccessExternalArea())
                .Throws(new SecurityException("User is not authorized."));

            // Act
            Func<Task> act = async () => await handler.HandleAsync(request);

            // Assert
            await act.Should().ThrowAsync<SecurityException>()
                .WithMessage("User is not authorized.");
        }
    }
}