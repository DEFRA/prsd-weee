namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.DirectRegistrants
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
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
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.DataReturns;


    public class GetSmallProducerSubmissionHandlerTests : SimpleUnitTestBase
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;
        private readonly GetSmallProducerSubmissionHandler handler;

        public GetSmallProducerSubmissionHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            mapper = A.Fake<IMapper>();
            var systemDataAccess = A.Fake<ISystemDataDataAccess>();

            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(new DateTime(SystemTime.UtcNow.Year,
                SystemTime.UtcNow.Month, SystemTime.UtcNow.Day));

            handler = new GetSmallProducerSubmissionHandler(authorization, genericDataAccess, mapper, systemDataAccess);
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_IsCalled()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            SetupValidDirectRegistrant(request.DirectRegistrantId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_AuthorizationCheck_NotAuthorized_ThrowsSecurityException()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            A.CallTo(() => authorization.EnsureCanAccessExternalArea()).Throws<SecurityException>();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityException>(
                async () => await handler.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_EnsureOrganisationAccess_IsCalled()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId);

            // Act
            await handler.HandleAsync(request);

            // Assert
            A.CallTo(() => authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_WhenCurrentYearSubmissionExists_ReturnsSmallProducerSubmissionData()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId, true);
            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.OrganisationData.Should().Be(organisationData);
            result.CurrentSubmission.Should().NotBeNull();
        }

        [Fact]
        public async Task HandleAsync_WhenNoCurrentYearSubmission_ReturnsNull()
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            SetupValidDirectRegistrant(request.DirectRegistrantId, false);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public async Task HandleAsync_WhenCurrentYearSubmissionExists_SetsOrganisationDetailsCompleteCorrectly(bool hasBusinessAddressId, bool expectedOrganisationDetailsComplete)
        {
            // Arrange
            var request = new GetSmallProducerSubmission(Guid.NewGuid());
            var directRegistrant = SetupValidDirectRegistrant(request.DirectRegistrantId, true);
            var organisationData = A.Fake<OrganisationData>();
            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation)).Returns(organisationData);
            
            var currentYearSubmission = A.Fake<DirectProducerSubmissionHistory>();

            A.CallTo(() => currentYearSubmission.BusinessAddressId).Returns(hasBusinessAddressId ? (Guid?)Guid.NewGuid() : null);
            A.CallTo(() => directRegistrant.DirectProducerSubmissions.ElementAt(0).CurrentSubmission)
                .Returns(currentYearSubmission);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.CurrentSubmission.Should().NotBeNull();
            result.CurrentSubmission.OrganisationDetailsComplete.Should().Be(expectedOrganisationDetailsComplete);
        }

        [Fact]
        public async Task HandleAsync_ThrowsException_WhenAuthorizationFails()
        {
            // Arrange
            A.CallTo(() => authorization.EnsureCanAccessExternalArea())
                .Throws<SecurityException>();

            // Act
            Func<Task> act = async () => await handler.HandleAsync(new GetSmallProducerSubmission { DirectRegistrantId = Guid.NewGuid() });

            // Assert
            await act.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_ReturnsNull_WhenNoCurrentYearSubmission()
        {
            // Arrange
            var directRegistrantId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var currentYear = 2023;

            SetupFakesForValidRequest(directRegistrantId, organisationId, currentYear);

            // Act
            var result = await handler.HandleAsync(new GetSmallProducerSubmission { DirectRegistrantId = directRegistrantId });

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_ReturnsCorrectData_WhenCurrentYearSubmissionExists()
        {
            // Arrange
            var directRegistrantId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var currentYear = 2023;

            SetupFakesForValidRequest(directRegistrantId, organisationId, currentYear);
            SetupFakesForCurrentYearSubmission(currentYear);

            // Act
            var result = await handler.HandleAsync(new GetSmallProducerSubmission { DirectRegistrantId = directRegistrantId });

            // Assert
            result.Should().NotBeNull();
            result.DirectRegistrantId.Should().Be(directRegistrantId);
            result.HasAuthorisedRepresentitive.Should().BeTrue();
            result.CurrentSubmission.Should().NotBeNull();
            result.CurrentSubmission.EEEDetailsComplete.Should().BeTrue();
            // Add more assertions here to check all properties of result.CurrentSubmission
        }

        private void SetupFakesForValidRequest(Guid directRegistrantId, Guid organisationId, int currentYear)
        {
            var directRegistrant = new DirectRegistrant { Id = directRegistrantId, OrganisationId = organisationId, AuthorisedRepresentativeId = Guid.NewGuid() };
            var organisation = new Organisation { Id = organisationId };

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(directRegistrant);

            A.CallTo(() => mapper.Map<Organisation, OrganisationData>(A<Organisation>._))
                .Returns(new OrganisationData());

            A.CallTo(() => systemDataDataAccess.GetSystemDateTime())
                .Returns(new DateTime(currentYear, 1, 1));
        }

        private void SetupFakesForCurrentYearSubmission(int currentYear)
        {
            var currentSubmission = new DirectProducerSubmission
            {
                ComplianceYear = currentYear,
                CurrentSubmission = new DirectRegistrantSubmission
                {
                    EeeOutputReturnVersion = new EeeOutputReturnVersion(),
                    BusinessAddress = new Address(),
                    BrandName = new BrandName { Name = "Test Brand" },
                    CompanyName = "Test Company",
                    TradingName = "Test Trading Name",
                    CompanyRegistrationNumber = "12345678",
                    SellingTechniqueType = Domain.Producer.SellingTechniqueType.DistanceSellingFromInsideUK,
                    ContactAddress = new Address(),
                    Contact = new Contact(),
                    AuthorisedRepresentative = new AuthorisedRepresentative(),
                    ServiceOfNoticeAddress = new Address()
                }
            };

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(A<Guid>._))
                .Returns(new DirectRegistrant
                {
                    DirectProducerSubmissions = new List<DirectProducerSubmission> { currentSubmission },
                    Organisation = new Organisation(),
                    AuthorisedRepresentative = new AuthorisedRepresentative()
                });

            A.CallTo(() => mapper.Map<Domain.Organisation.Address, AddressData>(A<Address>._))
                .Returns(new AddressData());

            A.CallTo(() => mapper.Map<Contact, ContactData>(A<Contact>._))
                .Returns(new ContactData());

            A.CallTo(() => mapper.Map<AuthorisedRepresentative, AuthorisedRepresentitiveData>(A<AuthorisedRepresentative>._))
                .Returns(new AuthorisedRepresentitiveData());

            A.CallTo(() => mapper.Map<EeeOutputReturnVersion, IList<Eee>>(A<EeeOutputReturnVersion>._))
                .Returns(new List<Eee>());
        }

        private DirectRegistrant SetupValidDirectRegistrant(Guid directRegistrantId, bool hasCurrentYearSubmission = false)
        {
            var directRegistrant = A.Fake<DirectRegistrant>();
            A.CallTo(() => directRegistrant.OrganisationId).Returns(Guid.NewGuid());
            A.CallTo(() => directRegistrant.Organisation).Returns(A.Fake<Organisation>());

            var submissions = new List<DirectProducerSubmission>();
            if (hasCurrentYearSubmission)
            {
                var currentYearSubmission = A.Fake<DirectProducerSubmission>();
                A.CallTo(() => currentYearSubmission.ComplianceYear).Returns(SystemTime.UtcNow.Year);
                submissions.Add(currentYearSubmission);
            }
            A.CallTo(() => directRegistrant.DirectProducerSubmissions).Returns(submissions);

            A.CallTo(() => genericDataAccess.GetById<DirectRegistrant>(directRegistrantId))
                .Returns(Task.FromResult(directRegistrant));

            return directRegistrant;
        }
    }
}