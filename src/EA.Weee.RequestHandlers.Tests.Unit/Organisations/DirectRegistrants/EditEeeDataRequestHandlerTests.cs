namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using EA.Weee.Core.Helpers;
    using EA.Prsd.Core;
    using EA.Weee.Domain.DataReturns;

    public class EditEeeDataRequestHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly EditEeeDataRequestHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();

        public EditEeeDataRequestHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            weeeContext = A.Fake<WeeeContext>();
            systemDataAccess = A.Fake<ISystemDataDataAccess>();

            handler = new EditEeeDataRequestHandler(authorization, genericDataAccess, weeeContext, systemDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant(request.DirectRegistrantId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_WhenEeeOutputReturnVersionDoesNotExist_CreatesNewVersion()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId);
            var currentSubmission = SetupCurrentSubmission(directRegistrant, null);

            // Act
            await handler.HandleAsync(request);

            // Assert
            currentSubmission.Should().NotBeNull();
            currentSubmission.CurrentSubmission.EeeOutputReturnVersion.EeeOutputAmounts.Should().HaveCount(request.TonnageData.Count);
        }

        [Fact]
        public async Task HandleAsync_WhenEeeOutputReturnVersionExists_UpdatesExistingVersion()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId);
            var existingEeeVersion = new EeeOutputReturnVersion();
            var currentSubmission = SetupCurrentSubmission(directRegistrant, existingEeeVersion);

            // Act
            await handler.HandleAsync(request);

            // Assert
            currentSubmission.CurrentSubmission.EeeOutputReturnVersion.EeeOutputAmounts.Should().BeSameAs(existingEeeVersion);
            currentSubmission.CurrentSubmission.EeeOutputReturnVersion.EeeOutputAmounts.EeeOutputAmounts.Should().HaveCount(request.TonnageData.Count);
        }

        [Fact]
        public async Task HandleAsync_UpdatesSellingTechniqueType()
        {
            // Arrange
            var request = CreateValidRequest();
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId);
            var currentSubmission = SetupCurrentSubmission(directRegistrant, new EeeOutputReturnVersion());

            // Act
            await handler.HandleAsync(request);

            // Assert
            currentSubmission.SellingTechniqueType.Should().Be(request.SellingTechniqueType.ToInt());
        }

        [Fact]
        public async Task HandleAsync_SaveChanges_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupValidDirectRegistrant(request.DirectRegistrantId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        private EditEeeDataRequest CreateValidRequest()
        {
            return new EditEeeDataRequest(
                Guid.NewGuid(),
                Core.Shared.SellingTechniqueType.Both,
                new List<Core.Shared.TonnageData>
                {
                    new Core.Shared.TonnageData { ObligationType = 1, Category = 1, Tonnage = 10 },
                    new Core.Shared.TonnageData { ObligationType = 2, Category = 2, Tonnage = 20 }
                });
        }

        private DirectRegistrant SetupValidDirectRegistrant(Guid directRegistrantId)
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.OrganisationId).Returns(Guid.NewGuid());
            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));
            return directRegistrant;
        }

        private DirectProducerSubmission SetupCurrentSubmission(DirectRegistrant directRegistrant, EeeOutputReturnVersion eeeVersion)
        {
            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(Task.FromResult(DateTime.UtcNow));

            var directProducerSubmissionCurrentYear = new DirectProducerSubmission(directRegistrant,
                A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year);
            var directProducerSubmissionNotCurrentYear = new DirectProducerSubmission(directRegistrant,
                A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year + 1);

            directProducerSubmissionCurrentYear.CurrentSubmission =
                new DirectProducerSubmissionHistory(directProducerSubmissionCurrentYear);

            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmissionCurrentYear);
            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmissionNotCurrentYear);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            return directProducerSubmissionCurrentYear;
        }
    }
}