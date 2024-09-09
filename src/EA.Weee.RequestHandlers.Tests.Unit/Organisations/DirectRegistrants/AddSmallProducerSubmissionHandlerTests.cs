namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.RequestHandlers.Scheme.Interfaces;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class AddSmallProducerSubmissionHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly IGenerateFromXmlDataAccess generateFromXmlDataAccess;
        private readonly AddSmallProducerSubmissionHandler handler;

        public AddSmallProducerSubmissionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            generateFromXmlDataAccess = A.Fake<IGenerateFromXmlDataAccess>();

            handler = new AddSmallProducerSubmissionHandler(
                authorization,
                genericDataAccess,
                weeeContext,
                generateFromXmlDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = new AddSmallProducerSubmission(Guid.NewGuid());
            SetupValidDirectRegistrant(request.DirectRegistrantId);
            SetupValidPrn();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var request = new AddSmallProducerSubmission(Guid.NewGuid());
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            // Arrange
            var request = new AddSmallProducerSubmission(Guid.NewGuid());
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId);
            SetupValidPrn();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_WhenPrnAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new AddSmallProducerSubmission(Guid.NewGuid());
            SetupValidDirectRegistrant(request.DirectRegistrantId);
            var prn = "PRN123";
            SetupValidPrn(prn, true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WhenValidRequest_CreatesNewSubmission()
        {
            // Arrange
            var request = new AddSmallProducerSubmission(Guid.NewGuid());
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId);
            var prn = "PRN123";
            SetupValidPrn(prn);

            var directProducerSubmissionHistory = A.Fake<DirectProducerSubmissionHistory>();
            var directProducerSubmission = A.Fake<DirectProducerSubmission>();
            A.CallTo(() => directProducerSubmission.Id).Returns(Guid.NewGuid());
            A.CallTo(() => directProducerSubmissionHistory.DirectProducerSubmission).Returns(directProducerSubmission);

            DirectProducerSubmissionHistory addedHistory = null;
            A.CallTo(() => genericDataAccess.Add(A<DirectProducerSubmissionHistory>._))
                .Invokes((DirectProducerSubmissionHistory h) => addedHistory = h)
                .Returns(directProducerSubmissionHistory);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBe(directProducerSubmission.Id);
            addedHistory.Should().NotBeNull();
            addedHistory.DirectProducerSubmission.DirectRegistrant.Should().Be(directRegistrant);
            addedHistory.DirectProducerSubmission.RegisteredProducer.ProducerRegistrationNumber.Should().Be(prn);
            addedHistory.DirectProducerSubmission.ComplianceYear.Should().Be(SystemTime.UtcNow.Year);

            A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        private DirectRegistrant SetupValidDirectRegistrant(Guid directRegistrantId)
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.OrganisationId).Returns(Guid.NewGuid());

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            return directRegistrant;
        }

        private void SetupValidPrn(string prn = "PRN123", bool exists = false)
        {
            var prnQueue = new Queue<string>();
            prnQueue.Enqueue(prn);
            A.CallTo(() => generateFromXmlDataAccess.ComputePrns(1)).Returns(Task.FromResult(prnQueue));
            A.CallTo(() => generateFromXmlDataAccess.ProducerRegistrationExists(prn)).Returns(Task.FromResult(exists));
        }
    }
}