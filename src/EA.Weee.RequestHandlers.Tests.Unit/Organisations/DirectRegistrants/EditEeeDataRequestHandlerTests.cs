namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Organisations.DirectRegistrants;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class EditEeeDataRequestHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly EditEeeDataRequestHandler handler;
        private readonly Guid directRegistrantId = Guid.NewGuid();
        private DirectProducerSubmission directProducerSubmission;

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
            SetupCurrentSubmission();

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
            var directRegistrant = SetupCurrentSubmission();

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
            SetupCurrentSubmission();

            // Act
            await handler.HandleAsync(request);

            // Assert
            directProducerSubmission.CurrentSubmission.EeeOutputReturnVersion.Should().NotBeNull();
            directProducerSubmission.CurrentSubmission.EeeOutputReturnVersion.EeeOutputAmounts.Count.Should()
                .BeGreaterThan(0);
            directProducerSubmission.CurrentSubmission.EeeOutputReturnVersion.EeeOutputAmounts.Count.Should()
                .Be(request.TonnageData.Count);

            foreach (var expectedEee in request.TonnageData)
            {
                directProducerSubmission.CurrentSubmission.EeeOutputReturnVersion.EeeOutputAmounts.Should().Contain(amount =>
                    amount.ObligationType == (Domain.Obligation.ObligationType)expectedEee.ObligationType &&
                    amount.WeeeCategory == (Domain.Lookup.WeeeCategory)expectedEee.Category &&
                    amount.Tonnage == expectedEee.Tonnage);
            }
        }

        [Fact]
        public async Task HandleAsync_WhenEeeOutputReturnVersionExists_UpdatesExistingVersion()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupCurrentSubmission();
            var existingEeeVersion = new EeeOutputReturnVersion();

            // Add an existing amount that should be removed
            var amountToRemove = new EeeOutputAmount(
                Domain.Obligation.ObligationType.B2C,
                Domain.Lookup.WeeeCategory.MedicalDevices,
                30m,
                directProducerSubmission.RegisteredProducer);
            existingEeeVersion.EeeOutputAmounts.Add(amountToRemove);

            // Add an existing amount that should be updated
            var amountToUpdate = new EeeOutputAmount(
                (Domain.Obligation.ObligationType)request.TonnageData[0].ObligationType,
                (Domain.Lookup.WeeeCategory)request.TonnageData[0].Category,
                50m, // Different tonnage than in the request
                directProducerSubmission.RegisteredProducer);
            existingEeeVersion.EeeOutputAmounts.Add(amountToUpdate);

            directProducerSubmission.CurrentSubmission.EeeOutputReturnVersion = existingEeeVersion;

            // Act
            await handler.HandleAsync(request);

            // Assert
            directProducerSubmission.CurrentSubmission.EeeOutputReturnVersion.Should().BeSameAs(existingEeeVersion);
            var updatedAmounts = directProducerSubmission.CurrentSubmission.EeeOutputReturnVersion.EeeOutputAmounts;

            // Check that the count matches the request
            updatedAmounts.Should().HaveCount(request.TonnageData.Count);

            // Check that all requested items are present with correct values
            foreach (var expectedEee in request.TonnageData)
            {
                updatedAmounts.Should().Contain(amount =>
                    amount.ObligationType == (Domain.Obligation.ObligationType)expectedEee.ObligationType &&
                    amount.WeeeCategory == (Domain.Lookup.WeeeCategory)expectedEee.Category &&
                    amount.Tonnage == expectedEee.Tonnage);
            }

            // Check that the amount to remove is no longer present
            updatedAmounts.Should().NotContain(amount =>
                amount.ObligationType == amountToRemove.ObligationType &&
                amount.WeeeCategory == amountToRemove.WeeeCategory);

            // Check that the amount to update has been updated
            updatedAmounts.Should().Contain(amount =>
                amount.ObligationType == amountToUpdate.ObligationType &&
                amount.WeeeCategory == amountToUpdate.WeeeCategory &&
                amount.Tonnage == request.TonnageData[0].Tonnage); // Should now have the tonnage from the request
        }

        [Fact]
        public async Task HandleAsync_UpdatesSellingTechniqueType()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupCurrentSubmission();

            // Act
            await handler.HandleAsync(request);

            // Assert
            directProducerSubmission.CurrentSubmission.SellingTechniqueType.Should().Be(request.SellingTechniqueType.ToInt());
        }

        [Fact]
        public async Task HandleAsync_SaveChanges_IsCalled()
        {
            // Arrange
            var request = CreateValidRequest();
            SetupCurrentSubmission();

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => weeeContext.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        private EditEeeDataRequest CreateValidRequest()
        {
            return new EditEeeDataRequest(
                directRegistrantId,
                new List<Eee>
                {
                    new Eee(20m, WeeeCategory.AutomaticDispensers, ObligationType.B2C),
                    new Eee(10m, WeeeCategory.DisplayEquipment, ObligationType.B2B)
                },
                TestFixture.Create<SellingTechniqueType>());
        }

        private DirectRegistrant SetupCurrentSubmission()
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(Guid.NewGuid());

            var directRegistrant = new DirectRegistrant(organisation);

            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(Task.FromResult(DateTime.UtcNow));

            directProducerSubmission = new DirectProducerSubmission(directRegistrant,
                A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year);
            var directProducerSubmissionNotCurrentYear = new DirectProducerSubmission(directRegistrant,
                A.Fake<RegisteredProducer>(), SystemTime.UtcNow.Year + 1);

            directProducerSubmission.CurrentSubmission =
                new DirectProducerSubmissionHistory(directProducerSubmission);

            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmission);
            directRegistrant.DirectProducerSubmissions.Add(directProducerSubmissionNotCurrentYear);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            return directRegistrant;
        }
    }
}